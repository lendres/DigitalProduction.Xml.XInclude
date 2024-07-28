using System.Xml;

namespace GotDotNet.XPointer;

public abstract class Pointer
{
	public abstract XmlNodeList Evaluate(XmlDocument doc);
}
