using System.Collections;

namespace DigitalProduction.Xml.XPointer;

public class XPointerSchema
{
	internal static Hashtable Schemas = XPointerSchema.CreateSchemasTable();

	internal static Hashtable CreateSchemasTable()
	{
		return new Hashtable()
		{
			{
				(object) ":element",
				(object) XPointerSchema.SchemaType.Element
			},
			{
				(object) ":xmlns",
				(object) XPointerSchema.SchemaType.Xmlns
			},
			{
				(object) ":xpath1",
				(object) XPointerSchema.SchemaType.XPath1
			},
			{
				(object) ":xpointer",
				(object) XPointerSchema.SchemaType.XPointer
			}
		};
	}

	internal enum SchemaType
	{
		Element,
		Xmlns,
		XPath1,
		XPointer,
		Unknown,
	}
}
