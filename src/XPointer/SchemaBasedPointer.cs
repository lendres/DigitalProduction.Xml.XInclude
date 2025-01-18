using System.Collections;
using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal class SchemaBasedPointer(ArrayList parts) : Pointer
{
	private readonly ArrayList _parts = parts;

	public ArrayList Parts => _parts;

	public override XmlNodeList Evaluate(XmlDocument doc)
	{
		XmlNamespaceManager nm = new(doc.NameTable);
		for (int index = 0; index < _parts.Count; ++index)
		{
			PointerPart? pointerPart = _parts[index] as PointerPart;
			XmlNodeList? xmlNodeList = pointerPart?.Evaluate(doc, nm);
			if (xmlNodeList != null && xmlNodeList.Count > 0)
			{
				return xmlNodeList;
			}
		}
		throw new NotFoundException("XPointer doesn't identify any subresource");
	}
}