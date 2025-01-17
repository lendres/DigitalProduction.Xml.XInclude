using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal class XPath1SchemaPointerPart : PointerPart
{
	private string _xpath;

	public string XPath
	{
		get => this._xpath;
		set => this._xpath = value;
	}

	public override XmlNodeList Evaluate(XmlDocument doc, XmlNamespaceManager nm)
	{
		try
		{
			return doc.SelectNodes(this._xpath, nm);
		}
		catch
		{
			return (XmlNodeList)null;
		}
	}

	public static XPath1SchemaPointerPart ParseSchemaData(XPointerLexer lexer)
	{
		XPath1SchemaPointerPart schemaData = new XPath1SchemaPointerPart();
		try
		{
			schemaData.XPath = lexer.ParseEscapedData();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Syntax error in xpath1() schema data: " + ex.Message);
			return (XPath1SchemaPointerPart)null;
		}
		return schemaData;
	}
}
