using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal class XmlnsSchemaPointerPart(string prefix, string uri) : PointerPart
{
	private readonly string _prefix	= prefix;
	private readonly string _uri	= uri;

	public string Prefix => _prefix;

	public string Uri => _uri;

	public override XmlNodeList? Evaluate(XmlDocument doc, XmlNamespaceManager nm)
	{
		nm.AddNamespace(_prefix, _uri);
		return null;
	}

	public static XmlnsSchemaPointerPart? ParseSchemaData(XPointerLexer lexer)
	{
		lexer.NextLexeme();
		if (lexer.Kind != XPointerLexer.LexKind.NCName)
		{
			Console.Error.WriteLine("Syntax error in xmlns() schema data: Invalid token in XmlnsSchemaData");
			return null;
		}
		string ncName = lexer.NCName;
		lexer.SkipWhiteSpace();
		lexer.NextLexeme();
		if (lexer.Kind != XPointerLexer.LexKind.Eq)
		{
			Console.Error.WriteLine("Syntax error in xmlns() schema data: Invalid token in XmlnsSchemaData");
			return null;
		}
		lexer.SkipWhiteSpace();
		string escapedData;
		try
		{
			escapedData = lexer.ParseEscapedData();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Syntax error in xmlns() schema data: " + ex.Message);
			return null;
		}
		return new XmlnsSchemaPointerPart(ncName, escapedData);
	}
}