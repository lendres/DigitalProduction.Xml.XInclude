using System.Collections;
using System.Xml;

namespace GotDotNet.XPointer;

public class XPointerReader : XmlReader
{
	private XmlReader _reader;
	private XmlReader _extReader;
	private Uri _uri;
	private string _xpointer;
	private Stream _stream;
	private XmlNameTable _nt;
	private IEnumerator _pointedNodes;

	public XPointerReader(Uri uri, Stream stream, XmlNameTable nt, string xpointer)
	{
		this._uri = uri;
		this._stream = stream;
		this._nt = nt;
		this._xpointer = xpointer;
	}

	public XPointerReader(XmlReader reader, XmlNameTable nt, string xpointer)
	{
		this._extReader = reader;
		this._xpointer = xpointer;
		this._nt = nt;
	}

	public override int AttributeCount => this._reader.AttributeCount;

	public override string BaseURI => this._reader.BaseURI;

	public override bool HasValue => this._reader.HasValue;

	public override bool IsDefault => this._reader.IsDefault;

	public override string Name => this._reader.Name;

	public override string LocalName => this._reader.LocalName;

	public override string NamespaceURI => this._reader.NamespaceURI;

	public override XmlNameTable NameTable => this._reader.NameTable;

	public override XmlNodeType NodeType => this._reader.NodeType;

	public override string Prefix => this._reader.Prefix;

	public override char QuoteChar => this._reader.QuoteChar;

	public override void Close()
	{
		if (this._reader == null)
			return;
		this._reader.Close();
	}

	public override int Depth => this._reader.Depth;

	public override bool EOF => this._reader.EOF;

	public override string GetAttribute(int i) => this._reader.GetAttribute(i);

	public override string GetAttribute(string name) => this._reader.GetAttribute(name);

	public override string GetAttribute(string name, string namespaceURI)
	{
		return this._reader.GetAttribute(name, namespaceURI);
	}

	public override bool IsEmptyElement => this._reader.IsEmptyElement;

	public override string LookupNamespace(string prefix) => this._reader.LookupNamespace(prefix);

	public override void MoveToAttribute(int i) => this._reader.MoveToAttribute(i);

	public override bool MoveToAttribute(string name) => this._reader.MoveToAttribute(name);

	public override bool MoveToAttribute(string name, string ns)
	{
		return this._reader.MoveToAttribute(name, ns);
	}

	public override bool MoveToElement() => this._reader.MoveToElement();

	public override bool MoveToFirstAttribute() => this._reader.MoveToFirstAttribute();

	public override bool MoveToNextAttribute() => this._reader.MoveToNextAttribute();

	public override bool ReadAttributeValue() => this._reader.ReadAttributeValue();

	public override ReadState ReadState => this._reader.ReadState;

	public override string this[int i] => this._reader[i];

	public override string this[string name] => this._reader[name];

	public override string this[string name, string namespaceURI]
	{
		get => this._reader[name, namespaceURI];
	}

	public override void ResolveEntity() => this._reader.ResolveEntity();

	public override string XmlLang => this._reader.XmlLang;

	public override XmlSpace XmlSpace => this._reader.XmlSpace;

	public override string Value => this._reader.Value;

	public override string ReadInnerXml() => this._reader.ReadInnerXml();

	public override string ReadOuterXml() => this._reader.ReadOuterXml();

	public override string ReadString() => this._reader.ReadString();

	public override bool Read()
	{
		if (this._reader == null)
		{
			XmlDocument doc = new XmlDocument(this._nt);
			doc.PreserveWhitespace = true;
			if (this._extReader == null)
				doc.Load((XmlReader)new XmlTextReader(this._uri.AbsoluteUri, (TextReader)new StreamReader(this._stream)));
			else
				doc.Load(this._extReader);
			this._pointedNodes = XPointerParser.ParseXPointer(this._xpointer).Evaluate(doc).GetEnumerator();
			this._pointedNodes.MoveNext();
			this._reader = (XmlReader)new XmlNodeReader(this._pointedNodes.Current as XmlNode);
		}
		if (this._reader.Read())
			return true;
		if (this._pointedNodes == null || !this._pointedNodes.MoveNext())
			return false;
		this._reader = (XmlReader)new XmlNodeReader(this._pointedNodes.Current as XmlNode);
		return this._reader.Read();
	}
}
