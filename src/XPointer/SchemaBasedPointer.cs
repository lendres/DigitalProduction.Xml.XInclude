using System.Collections;
using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal class SchemaBasedPointer : Pointer
{
	private ArrayList _parts;

	public ArrayList Parts => this._parts;

	public SchemaBasedPointer(ArrayList parts) => this._parts = parts;

	public override XmlNodeList Evaluate(XmlDocument doc)
	{
		XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
		for (int index = 0; index < this._parts.Count; ++index)
		{
			XmlNodeList xmlNodeList = ((PointerPart) this._parts[index]).Evaluate(doc, nm);
			if (xmlNodeList != null && xmlNodeList.Count > 0)
				return xmlNodeList;
		}
		throw new NotFoundException("XPointer doesn't identify any subresource");
	}
}
