using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal class XPath1SchemaPointerPart : PointerPart
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

	public static XPath1SchemaPointerPart? ParseSchemaData(XPointerLexer lexer)
	{
		XPath1SchemaPointerPart schemaData = new();
		try
		{
			schemaData.XPath = lexer.ParseEscapedData();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Syntax error in xpath1() schema data: " + ex.Message);
			return null;
		}
		return schemaData;
	}
}