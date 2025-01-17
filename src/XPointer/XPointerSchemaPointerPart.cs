using System.Xml;

namespace DigitalProduction.Xml.XPointer
{
	internal class XPointerSchemaPointerPart : PointerPart
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

		public static XPointerSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
		{
			XPointerSchemaPointerPart schemaData = new XPointerSchemaPointerPart();
			try
			{
				schemaData.XPath = lexer.ParseEscapedData();
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Syntax error in xpointer() schema data: " + ex.Message);
				return (XPointerSchemaPointerPart)null;
			}
			return schemaData;
		}
	}
}
