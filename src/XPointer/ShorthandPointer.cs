using System.Xml;

namespace GotDotNet.XPointer;

internal class ShorthandPointer : Pointer
{
	private string _NCName;

	public string NCName => this._NCName;

	public ShorthandPointer(string n) => this._NCName = n;

	public override XmlNodeList Evaluate(XmlDocument doc)
	{
		XmlNodeList? xmlNodeList = doc.SelectNodes("id('" + this._NCName + "')");
		return xmlNodeList != null && xmlNodeList.Count > 0 ? xmlNodeList : throw new NotFoundException("XPointer doesn't identify any subresource");
	}
}
