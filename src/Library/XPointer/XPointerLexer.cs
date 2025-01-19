using System.Text;
using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal class XPointerLexer
{
	private readonly string			_ptr;
	private int						_ptrIndex;
	private XPointerLexer.LexKind	_kind;
	private char					_currChar;
	private int						_number;
	private string					_ncname				= string.Empty;
	private string					_prefix				= string.Empty;
	private bool					_canBeSchemaName;

	public XPointerLexer(string pointer)
	{
		_ptr = pointer ?? throw new ArgumentNullException(nameof(pointer), "XPointer pointer cannot be null");
		NextChar();
	}

	public bool NextChar()
	{
		if (_ptrIndex < _ptr.Length)
		{
			_currChar = _ptr[_ptrIndex++];
			return true;
		}
		_currChar = char.MinValue;
		return false;
	}

	public XPointerLexer.LexKind Kind => _kind;

	public int Number => _number;

	public string NCName => _ncname;

	public string Prefix => _prefix;

	public bool CanBeSchemaName => _canBeSchemaName;

	public void SkipWhiteSpace()
	{
		while (LexUtils.IsWhitespace(_currChar))
			NextChar();
	}

	public bool NextLexeme()
	{
		switch (_currChar)
		{
			case char.MinValue:
				_kind = XPointerLexer.LexKind.Eof;
				return false;
			case '(':
			case ')':
			case '/':
			case '=':
				_kind = (XPointerLexer.LexKind)Convert.ToInt32(_currChar);
				NextChar();
				break;
			case '^':
				NextChar();
				if (_currChar != '^' && _currChar != '(' && _currChar != ')')
					throw new XPointerSyntaxException("Circumflex character must be escaped");
				_kind = XPointerLexer.LexKind.EscapedData;
				NextChar();
				break;
			default:
				if (char.IsDigit(_currChar))
				{
					_kind = XPointerLexer.LexKind.Number;
					int startIndex = _ptrIndex - 1;
					int length = 0;
					while (char.IsDigit(_currChar))
					{
						NextChar();
						length++;
					}
					_number = XmlConvert.ToInt32(_ptr.Substring(startIndex, length));
					break;
				}
				if (LexUtils.IsStartNameChar(_currChar))
				{
					_kind = XPointerLexer.LexKind.NCName;
					_prefix = string.Empty;
					_ncname = ParseName();
					if (_currChar == ':')
					{
						NextChar();
						_prefix = _ncname;
						_kind = XPointerLexer.LexKind.QName;
						if (LexUtils.IsStartNCNameChar(_currChar))
							_ncname = ParseName();
						else
							throw new XPointerSyntaxException("Wrong Name token: " + _prefix + ":" + (object)_currChar);
					}
					_canBeSchemaName = _currChar == '(';
					break;
				}
				if (LexUtils.IsWhitespace(_currChar))
				{
					_kind = XPointerLexer.LexKind.Space;
					while (LexUtils.IsWhitespace(_currChar))
						NextChar();
					break;
				}
				_kind = XPointerLexer.LexKind.EscapedData;
				break;
		}
		return true;
	}

	private string ParseName()
	{
		int startIndex = _ptrIndex - 1;
		int length = 0;
		while (LexUtils.IsNCNameChar(_currChar))
		{
			NextChar();
			++length;
		}
		return _ptr.Substring(startIndex, length);
	}

	public string ParseEscapedData()
	{
		int num = 0;
		StringBuilder stringBuilder = new();
		do
		{
			switch (_currChar)
			{
				case '(':
					++num;
					goto default;
				case ')':
					if (num-- == 0)
					{
						NextLexeme();
						return stringBuilder.ToString();
					}
					goto default;
				case '^':
					if (!NextChar())
						throw new XPointerSyntaxException("Unexpected end of schema data");
					if (_currChar != '^' && _currChar != '(' && _currChar != ')')
						throw new XPointerSyntaxException("Circumflex character must be escaped");
					stringBuilder.Append(_currChar);
					break;
				default:
					stringBuilder.Append(_currChar);
					break;
			}
		}
		while (NextChar());
		throw new XPointerSyntaxException("Unexpected end of schema data");
	}

	public enum LexKind
	{
		LRBracket = 40, // 0x00000028
		RRBracket = 41, // 0x00000029
		Slash = 47, // 0x0000002F
		Eq = 61, // 0x0000003D
		EscapedData = 68, // 0x00000044
		Eof = 69, // 0x00000045
		NCName = 78, // 0x0000004E
		QName = 81, // 0x00000051
		Space = 83, // 0x00000053
		Circumflex = 94, // 0x0000005E
		Number = 100, // 0x00000064
	}
}