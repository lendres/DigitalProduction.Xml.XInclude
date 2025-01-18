using System.Xml;

namespace DigitalProduction.Xml.XPointer;

public abstract class Pointer
{
	public abstract XmlNodeList Evaluate(XmlDocument doc);
}