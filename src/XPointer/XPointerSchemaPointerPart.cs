using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal class XPointerSchemaPointerPart : PointerPart
{
	private string _xpath	= string.Empty;

	public string XPath
	{
		get => _xpath;
		set => _xpath = value;
	}

	public override XmlNodeList? Evaluate(XmlDocument doc, XmlNamespaceManager nm)
	{
		try
		{
			return doc.SelectNodes(_xpath, nm);
		}
		catch
		{
			return null;
		}
	}

	public static XPointerSchemaPointerPart? ParseSchemaData(XPointerLexer lexer)
	{
		XPointerSchemaPointerPart schemaData = new();
		try
		{
			schemaData.XPath = lexer.ParseEscapedData();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Syntax error in xpointer() schema data: " + ex.Message);
			return null;
		}
		return schemaData;
	}
}