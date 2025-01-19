using System.Xml;

namespace DigitalProduction.Xml.XPointer;

internal class ShorthandPointer(string n) : Pointer
{
	private readonly string _NCName = n;

	public string NCName => _NCName;

	public override XmlNodeList Evaluate(XmlDocument doc)
	{
		XmlNodeList? xmlNodeList = doc.SelectNodes("id('" + _NCName + "')");
		return xmlNodeList != null && xmlNodeList.Count > 0 ? xmlNodeList : throw new NotFoundException("XPointer doesn't identify any subresource");
	}
}