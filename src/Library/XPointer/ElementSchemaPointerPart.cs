using System.Collections;
using System.Text;
using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal class ElementSchemaPointerPart : PointerPart
{
	private string		_NCName			= "";
	private string		_xPath			= "";
	private ArrayList	_childSequence	= [];

	public string NCName { get => _NCName; set => _NCName = value; }

	public string XPath { get => _xPath; set => _xPath = value; }

	public ArrayList ChildSequence { get => _childSequence; set => _childSequence = value; }

	public override XmlNodeList? Evaluate(XmlDocument doc, XmlNamespaceManager nm)
	{
		return doc.SelectNodes(_xPath, nm);
	}

	public static ElementSchemaPointerPart? ParseSchemaData(XPointerLexer lexer)
	{
		StringBuilder stringBuilder = new();
		ElementSchemaPointerPart schemaData = new();
		lexer.NextLexeme();
		if (lexer.Kind == XPointerLexer.LexKind.NCName)
		{
			schemaData.NCName = lexer.NCName;
			stringBuilder.Append("id('");
			stringBuilder.Append(lexer.NCName);
			stringBuilder.Append("')");
			lexer.NextLexeme();
		}
		schemaData.ChildSequence = [];
		while (lexer.Kind == XPointerLexer.LexKind.Slash)
		{
			lexer.NextLexeme();
			if (lexer.Kind != XPointerLexer.LexKind.Number)
			{
				Console.Error.WriteLine("Syntax error in element() schema data: Invalid token in ChildSequence");
				return null;
			}
			if (lexer.Number == 0)
			{
				Console.Error.WriteLine("Syntax error in element() schema data: 0 index ChildSequence");
				return null;
			}
			schemaData.ChildSequence.Add((object)lexer.Number);
			stringBuilder.Append("/*[");
			stringBuilder.Append(lexer.Number);
			stringBuilder.Append(']');
			lexer.NextLexeme();
		}
		if (lexer.Kind != XPointerLexer.LexKind.RRBracket)
		{
			Console.Error.WriteLine("Syntax error in element() schema data: Invalid token in ChildSequence");
			return null;
		}
		if (schemaData.NCName == null && schemaData.ChildSequence.Count == 0)
		{
			Console.Error.WriteLine("Syntax error in element() schema data: empty XPointer");
			return null;
		}
		schemaData.XPath = stringBuilder.ToString();
		return schemaData;
	}
}