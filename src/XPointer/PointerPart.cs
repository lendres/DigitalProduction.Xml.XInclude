using System.Xml;

namespace GotDotNet.XPointer;

internal abstract class PointerPart
{
	public abstract XmlNodeList? Evaluate(XmlDocument doc, XmlNamespaceManager nm);
}
