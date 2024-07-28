using System.Collections;

namespace GotDotNet.XPointer
{
	public class XPointerParser
	{
		private static Hashtable _schemas = XPointerSchema.Schemas;

		public static Pointer ParseXPointer(string xpointer)
		{
			XPointerLexer lexer = new XPointerLexer(xpointer);
			lexer.NextLexeme();
			if (lexer.Kind == XPointerLexer.LexKind.NCName && !lexer.CanBeSchemaName)
			{
				Pointer xpointer1 = (Pointer) new ShorthandPointer(lexer.NCName);
				lexer.NextLexeme();
				if (lexer.Kind != XPointerLexer.LexKind.Eof)
					throw new XPointerSyntaxException("Invalid token after shorthand pointer");
				return xpointer1;
			}
			ArrayList parts = new ArrayList();
			while (lexer.Kind != XPointerLexer.LexKind.Eof)
			{
				if (lexer.Kind != XPointerLexer.LexKind.NCName && lexer.Kind != XPointerLexer.LexKind.QName || !lexer.CanBeSchemaName)
					throw new XPointerSyntaxException("Invalid token");
				XPointerSchema.SchemaType schema = XPointerParser.GetSchema(lexer, parts);
				lexer.NextLexeme();
				switch (schema)
				{
					case XPointerSchema.SchemaType.Element:
						ElementSchemaPointerPart schemaData1 = ElementSchemaPointerPart.ParseSchemaData(lexer);
						if (schemaData1 != null)
						{
							parts.Add((object)schemaData1);
							break;
						}
						break;
					case XPointerSchema.SchemaType.Xmlns:
						XmlnsSchemaPointerPart schemaData2 = XmlnsSchemaPointerPart.ParseSchemaData(lexer);
						if (schemaData2 != null)
						{
							parts.Add((object)schemaData2);
							break;
						}
						break;
					case XPointerSchema.SchemaType.XPath1:
						XPath1SchemaPointerPart schemaData3 = XPath1SchemaPointerPart.ParseSchemaData(lexer);
						if (schemaData3 != null)
						{
							parts.Add((object)schemaData3);
							break;
						}
						break;
					case XPointerSchema.SchemaType.XPointer:
						XPointerSchemaPointerPart schemaData4 = XPointerSchemaPointerPart.ParseSchemaData(lexer);
						if (schemaData4 != null)
						{
							parts.Add((object)schemaData4);
							break;
						}
						break;
					default:
						lexer.ParseEscapedData();
						break;
				}
				lexer.NextLexeme();
				if (lexer.Kind == XPointerLexer.LexKind.Space)
					lexer.NextLexeme();
			}
			return (Pointer)new SchemaBasedPointer(parts);
		}

		private static XPointerSchema.SchemaType GetSchema(XPointerLexer lexer, ArrayList parts)
		{
			string str;
			if (lexer.Prefix != string.Empty)
			{
				str = (string)null;
				for (int index = parts.Count - 1; index >= 0; --index)
				{
					PointerPart part = (PointerPart) parts[index];
					if (part is XmlnsSchemaPointerPart)
					{
						XmlnsSchemaPointerPart schemaPointerPart = (XmlnsSchemaPointerPart) part;
						if (schemaPointerPart.Prefix == lexer.Prefix)
						{
							str = schemaPointerPart.Uri;
							break;
						}
					}
				}
				if (str == null)
					throw new XPointerSyntaxException("Undeclared prefix " + lexer.Prefix);
			}
			else
				str = string.Empty;
			object schema = XPointerParser._schemas[(object) (str + (object) ':' + lexer.NCName)];
			return schema == null ? XPointerSchema.SchemaType.Unknown : (XPointerSchema.SchemaType)schema;
		}
	}
}
