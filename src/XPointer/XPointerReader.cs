using System.Collections;
using System.Xml;

namespace DigitalProduction.Xml.XPointer;

public class XPointerReader : XmlReader
{
	private XmlReader?				_reader;
	private readonly XmlReader?		_extReader;
	private readonly Uri?			_uri;
	private readonly string			_xpointer;
	private readonly Stream?		_stream;
	private readonly XmlNameTable	_nt;
	private IEnumerator?			_pointedNodes;

	public XPointerReader(Uri uri, Stream stream, XmlNameTable nt, string xpointer)
	{
		_uri		= uri;
		_stream		= stream;
		_nt			= nt;
		_xpointer	= xpointer;
	}

	public XPointerReader(XmlReader reader, XmlNameTable nt, string xpointer)
	{
		_extReader	= reader;
		_xpointer	= xpointer;
		_nt			= nt;
	}

	public override int AttributeCount => _reader?.AttributeCount ?? 0;

	public override string BaseURI => _reader?.BaseURI ?? string.Empty;

	public override bool HasValue => _reader?.HasValue ?? false;

	public override bool IsDefault => _reader?.IsDefault ?? false;

	public override string Name => _reader?.Name ?? string.Empty;

	public override string LocalName => _reader?.LocalName ?? string.Empty;

	public override string NamespaceURI => _reader?.NamespaceURI ?? string.Empty;

	public override XmlNameTable NameTable => _reader?.NameTable ?? _nt;

	public override XmlNodeType NodeType => _reader?.NodeType ?? XmlNodeType.None;

	public override string Prefix => _reader?.Prefix ?? string.Empty;

	public override char QuoteChar => _reader?.QuoteChar ?? char.MinValue;

	public override void Close()
	{
		_reader?.Close();
	}

	public override int Depth => _reader?.Depth ?? 0;

	public override bool EOF => _reader?.EOF ?? true;

	public override string GetAttribute(int i) => _reader?.GetAttribute(i) ?? string.Empty;

	public override string? GetAttribute(string name) => _reader?.GetAttribute(name);

	public override string? GetAttribute(string name, string? namespaceURI)
	{
		return _reader?.GetAttribute(name, namespaceURI);
	}

	public override bool IsEmptyElement => _reader?.IsEmptyElement ?? true;

	public override string? LookupNamespace(string prefix) => _reader?.LookupNamespace(prefix);

	public override void MoveToAttribute(int i) => _reader?.MoveToAttribute(i);

	public override bool MoveToAttribute(string name) => _reader?.MoveToAttribute(name) ?? false;

	public override bool MoveToAttribute(string name, string? ns)
	{
		return _reader?.MoveToAttribute(name, ns) ?? false;
	}

	public override bool MoveToElement() => _reader?.MoveToElement() ?? false;

	public override bool MoveToFirstAttribute() => _reader?.MoveToFirstAttribute() ?? false;

	public override bool MoveToNextAttribute() => _reader?.MoveToNextAttribute() ?? false;

	public override bool ReadAttributeValue() => _reader?.ReadAttributeValue() ?? false;

	public override ReadState ReadState => _reader?.ReadState ?? ReadState.Initial;

	public override string this[int i] => _reader?[i] ?? string.Empty;

	public override string? this[string name] => _reader?[name];

	public override string this[string name, string? namespaceURI]
	{
		get => _reader?[name, namespaceURI] ?? string.Empty;
	}

	public override void ResolveEntity() => _reader?.ResolveEntity();

	public override string XmlLang => _reader?.XmlLang ?? string.Empty;

	public override XmlSpace XmlSpace => _reader?.XmlSpace ?? XmlSpace.None;

	public override string Value => _reader?.Value ?? string.Empty;

	public override string ReadInnerXml() => _reader?.ReadInnerXml() ?? string.Empty;

	public override string ReadOuterXml() => _reader?.ReadOuterXml() ?? string.Empty;

	public override string ReadString() => _reader?.ReadString() ?? string.Empty;

	public override bool Read()
	{
		if (_reader == null)
		{
			XmlDocument doc = new(_nt)
			{
				PreserveWhitespace = true
			};
			if (_extReader == null)
			{
				doc.Load((XmlReader)new XmlTextReader(_uri!.AbsoluteUri, (TextReader)new StreamReader(_stream!)));
			}
			else
			{
				doc.Load(_extReader);
			}
			_pointedNodes = XPointerParser.ParseXPointer(_xpointer).Evaluate(doc).GetEnumerator();
			_pointedNodes.MoveNext();
			_reader = new XmlNodeReader((XmlNode)_pointedNodes.Current);
		}

		if (_reader.Read())
		{
			return true;
		}

		if (_pointedNodes == null || !_pointedNodes.MoveNext())
		{
			return false;
		}

		_reader = new XmlNodeReader((XmlNode)_pointedNodes.Current);
		return _reader.Read();
	}
}