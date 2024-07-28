using System.Text;
using System.Xml;

namespace GotDotNet.XPointer;

internal class XPointerLexer
{
	private string _ptr;
	private int _ptrIndex;
	private XPointerLexer.LexKind _kind;
	private char _currChar;
	private int _number;
	private string _ncname;
	private string _prefix;
	private bool _canBeSchemaName;

	public XPointerLexer(string p)
	{
		this._ptr = p != null ? p : throw new ArgumentNullException("pointer", "XPointer pointer cannot be null");
		this.NextChar();
	}

	public bool NextChar()
	{
		if (this._ptrIndex < this._ptr.Length)
		{
			this._currChar = this._ptr[this._ptrIndex++];
			return true;
		}
		this._currChar = char.MinValue;
		return false;
	}

	public XPointerLexer.LexKind Kind => this._kind;

	public int Number => this._number;

	public string NCName => this._ncname;

	public string Prefix => this._prefix;

	public bool CanBeSchemaName => this._canBeSchemaName;

	public void SkipWhiteSpace()
	{
		while (LexUtils.IsWhitespace(this._currChar))
			this.NextChar();
	}

	public bool NextLexeme()
	{
		switch (this._currChar)
		{
			case char.MinValue:
				this._kind = XPointerLexer.LexKind.Eof;
				return false;
			case '(':
			case ')':
			case '/':
			case '=':
				this._kind = (XPointerLexer.LexKind)Convert.ToInt32(this._currChar);
				this.NextChar();
				break;
			case '^':
				this.NextChar();
				if (this._currChar != '^' && this._currChar != '(' && this._currChar != ')')
					throw new XPointerSyntaxException("Circumflex character must be escaped");
				this._kind = XPointerLexer.LexKind.EscapedData;
				this.NextChar();
				break;
			default:
				if (char.IsDigit(this._currChar))
				{
					this._kind = XPointerLexer.LexKind.Number;
					int startIndex = this._ptrIndex - 1;
					int length = 0;
					while (char.IsDigit(this._currChar))
					{
						this.NextChar();
						++length;
					}
					this._number = XmlConvert.ToInt32(this._ptr.Substring(startIndex, length));
					break;
				}
				if (LexUtils.IsStartNameChar(this._currChar))
				{
					this._kind = XPointerLexer.LexKind.NCName;
					this._prefix = string.Empty;
					this._ncname = this.ParseName();
					if (this._currChar == ':')
					{
						this.NextChar();
						this._prefix = this._ncname;
						this._kind = XPointerLexer.LexKind.QName;
						if (LexUtils.IsStartNCNameChar(this._currChar))
							this._ncname = this.ParseName();
						else
							throw new XPointerSyntaxException("Wrong Name token: " + this._prefix + ":" + (object)this._currChar);
					}
					this._canBeSchemaName = this._currChar == '(';
					break;
				}
				if (LexUtils.IsWhitespace(this._currChar))
				{
					this._kind = XPointerLexer.LexKind.Space;
					while (LexUtils.IsWhitespace(this._currChar))
						this.NextChar();
					break;
				}
				this._kind = XPointerLexer.LexKind.EscapedData;
				break;
		}
		return true;
	}

	private string ParseName()
	{
		int startIndex = this._ptrIndex - 1;
		int length = 0;
		while (LexUtils.IsNCNameChar(this._currChar))
		{
			this.NextChar();
			++length;
		}
		return this._ptr.Substring(startIndex, length);
	}

	public string ParseEscapedData()
	{
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		do
		{
			switch (this._currChar)
			{
				case '(':
					++num;
					goto default;
				case ')':
					if (num-- == 0)
					{
						this.NextLexeme();
						return stringBuilder.ToString();
					}
					goto default;
				case '^':
					if (!this.NextChar())
						throw new XPointerSyntaxException("Unexpected end of schema data");
					if (this._currChar != '^' && this._currChar != '(' && this._currChar != ')')
						throw new XPointerSyntaxException("Circumflex character must be escaped");
					stringBuilder.Append(this._currChar);
					break;
				default:
					stringBuilder.Append(this._currChar);
					break;
			}
		}
		while (this.NextChar());
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
