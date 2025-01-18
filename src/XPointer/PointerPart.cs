using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal abstract class PointerPart
{
	public abstract XmlNodeList? Evaluate(XmlDocument doc, XmlNamespaceManager nm);
}