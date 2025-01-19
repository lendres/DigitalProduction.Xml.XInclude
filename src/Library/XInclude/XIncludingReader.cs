using DigitalProduction.Xml.XPointer;
using System.Collections;
using System.Net;
using System.Security;
using System.Xml;

namespace DigitalProduction.Xml.XInclude;
/*
Open Issues:
o Depth on included items?
o GetAttribute(int i)?
o MoveToAttribute(int i)?
o GetEntity on custom XmlResolver
*/

internal struct FallbackState
{
	// Fallback is being processed.
	public bool	Fallbacking;

	// xi:fallback element depth.
	public int	FallbackDepth;

	// Fallback processed flag.
	public bool	FallbackProcessed;
}

/// <summary>
/// XInclude 1.0 aware XmlReader.
/// </summary>
/// <author>Oleg Tkachenko, oleg@tkachenko.com</author>	
public class XIncludingReader : XmlReader
{
	#region Private fields

	// XInclude keywords.
	private readonly XIncludeKeywords	_keywords;
	// Current reader.
	private XmlReader					_reader;
	// Stack of readers.
	private readonly Stack				_readers				= new();
	// Stack of Base URIs - to prevent circular inclusion.
	private readonly Stack				_baseURIs				= new();
	// Top base URI.
	private readonly Uri				_topBaseUri;
	// Top-level included item flag.
	private bool						_topLevel				= false;
	// Internal state.
	private XIncludingReaderState		_state;
	// Name table.
	private readonly XmlNameTable		_nameTable;
	// Normalization.
	private bool						_normalization;
	// Whitespace handling.
	private WhitespaceHandling			_whiteSpaceHandling;
	// Emit relative xml:base URIs.
	private bool						_relativeBaseUri		= true;
	// Current fallback state.
	private FallbackState				_fallbackState;
	// Previous fallback state (imagine enclosed deep xi:fallback/xi:include tree).
	private FallbackState				_prevFallbackState;
	// XmlResolver to resolve URIs.
	XmlResolver?						_xmlResolver;

	#endregion

	#region Constructors

	/// <summary>
	/// Creates new instance of <c>XIncludingReader</c> class with
	/// specified underlying <c>XmlReader</c> reader.
	/// </summary>
	/// <param name="reader">Underlying reader to read from.</param>
	public XIncludingReader(XmlReader reader)
	{
		if (reader is XmlTextReader xtr)
		{
			_normalization = xtr.Normalization;
			_whiteSpaceHandling = xtr.WhitespaceHandling;
		}
		_reader		= reader;
		_nameTable	= reader.NameTable;
		_keywords	= new XIncludeKeywords(NameTable);
		_topBaseUri	= new Uri(_reader.BaseURI);
		_baseURIs.Push(_topBaseUri);
		_state		= XIncludingReaderState.Default;
	}

	/// <summary>
	/// Creates new instance of <c>XIncludingReader</c> class with
	/// specified URL.
	/// </summary>
	/// <param name="url">Document location.</param>
	public XIncludingReader(string url) :
		this(new XmlTextReader(url))
	{
	}

	/// <summary>
	/// Creates new instance of <c>XIncludingReader</c> class with
	/// specified URL and nametable.
	/// </summary>
	/// <param name="url">Document location.</param>
	/// <param name="nt">Name table.</param>
	public XIncludingReader(string url, XmlNameTable nt) :
		this(new XmlTextReader(url, nt))
	{
	}

	/// <summary>
	/// Creates new instance of <c>XIncludingReader</c> class with
	/// specified <c>TextReader</c> reader.
	/// </summary>
	/// <param name="reader"><c>TextReader</c>.</param>
	public XIncludingReader(TextReader reader) :
		this(new XmlTextReader(reader))
	{
	}

	/// <summary>
	/// Creates new instance of <c>XIncludingReader</c> class with
	/// specified <c>TextReader</c> reader and nametable.
	/// </summary>
	/// <param name="reader"><c>TextReader</c>.</param>
	/// <param name="nt">Nametable.</param>
	public XIncludingReader(TextReader reader, XmlNameTable nt) :
		this(new XmlTextReader(reader, nt))
	{
	}

	/// <summary>
	/// Creates new instance of <c>XIncludingReader</c> class with
	/// specified <c>Stream</c>.
	/// </summary>
	/// <param name="input"><c>Stream</c>.</param>
	public XIncludingReader(Stream input) :
		this(new XmlTextReader(input))
	{
	}

	/// <summary>
	/// Creates new instance of <c>XIncludingReader</c> class with
	/// specified <c>Stream</c> and nametable.
	/// </summary>
	/// <param name="input"><c>Stream</c>.</param>
	/// <param name="nt">Nametable</param>
	public XIncludingReader(Stream input, XmlNameTable nt) :
		this(new XmlTextReader(input, nt))
	{
	}

	#endregion

	#region XmlReader's overriden members

	public override int AttributeCount
	{
		get
		{
			if (_topLevel && _reader.GetAttribute(_keywords.Base, _keywords.XmlNamespace) == null)
			{
				return _reader.AttributeCount+1;
			}
			else
			{
				return _reader.AttributeCount;
			}
		}
	}

	public override string BaseURI { get => _reader.BaseURI; }

	public override bool HasValue
	{
		get
		{
			return _state switch
			{
				XIncludingReaderState.ExposingXmlBaseAttr => true,
				XIncludingReaderState.ExposingXmlBaseAttrValue => true,
				_ => _reader.HasValue,
			};
		}
	}

	public override bool IsDefault
	{
		get
		{
			return _state switch
			{
				XIncludingReaderState.ExposingXmlBaseAttr => false,
				XIncludingReaderState.ExposingXmlBaseAttrValue => false, //TODO: May be wrong if xml:base exists and it does default
				_ => _reader.IsDefault,
			};
		}
	}

	public override string Name
	{
		get
		{
			return _state switch
			{
				XIncludingReaderState.ExposingXmlBaseAttr => _keywords.XmlBase,
				XIncludingReaderState.ExposingXmlBaseAttrValue => String.Empty,
				_ => _reader.Name,
			};
		}
	}

	public override string LocalName
	{
		get
		{
			return _state switch
			{
				XIncludingReaderState.ExposingXmlBaseAttr => _keywords.Base,
				XIncludingReaderState.ExposingXmlBaseAttrValue => String.Empty,
				_ => _reader.LocalName,
			};
		}
	}

	public override string NamespaceURI
	{
		get
		{
			return _state switch
			{
				XIncludingReaderState.ExposingXmlBaseAttr => _keywords.XmlNamespace,
				XIncludingReaderState.ExposingXmlBaseAttrValue => String.Empty,
				_ => _reader.NamespaceURI,
			};
		}
	}

	public override XmlNameTable NameTable { get => _nameTable; }

	public override XmlNodeType NodeType
	{
		get
		{
			return _state switch
			{
				XIncludingReaderState.ExposingXmlBaseAttr => XmlNodeType.Attribute,
				XIncludingReaderState.ExposingXmlBaseAttrValue => XmlNodeType.Text,
				_ => _reader.NodeType,
			};
		}
	}

	public override string Prefix
	{
		get
		{
			return _state switch
			{
				XIncludingReaderState.ExposingXmlBaseAttr => _keywords.Xml,
				XIncludingReaderState.ExposingXmlBaseAttrValue => String.Empty,
				_ => _reader.Prefix,
			};
		}
	}

	public override char QuoteChar
	{
		get
		{
			return _state switch
			{
				XIncludingReaderState.ExposingXmlBaseAttr => '"',
				_ => _reader.QuoteChar,
			};
		}
	}

	public override void Close()
	{
		_reader.Close();
		// Close all readers in the stack.
		while (_readers.Count > 0)
		{
			_reader = (XmlReader)_readers.Pop()!;
			_reader.Close();
		}
	}

	public override int Depth { get => _reader.Depth; }

	public override bool EOF { get =>  _reader.EOF; }

	public override string GetAttribute(int i)
	{
		return _reader.GetAttribute(i);
	}

	public override string? GetAttribute(string name)
	{
		if (_topLevel && XIncludeKeywords.Equals(name, _keywords.XmlBase))
		{
			return _reader.BaseURI;
		}
		else
		{
			return _reader.GetAttribute(name);
		}
	}

	public override string? GetAttribute(string name, string? namespaceURI)
	{
		if (_topLevel && XIncludeKeywords.Equals(name, _keywords.Base) && XIncludeKeywords.Equals(namespaceURI ?? string.Empty, _keywords.XmlNamespace))
		{
			return _reader.BaseURI;
		}
		else
		{
			return _reader.GetAttribute(name, namespaceURI);
		}
	}

	public override bool IsEmptyElement { get => _reader.IsEmptyElement; }

	public override String? LookupNamespace(String prefix)
	{
		return _reader.LookupNamespace(prefix);
	}

	public override void MoveToAttribute(int i)
	{
		_reader.MoveToAttribute(i);
	}

	public override bool MoveToAttribute(string name)
	{
		if (_topLevel && XIncludeKeywords.Equals(name, _keywords.XmlBase))
		{
			_state = XIncludingReaderState.ExposingXmlBaseAttr;
			return true;
		}
		else
		{
			return _reader.MoveToAttribute(name);
		}
	}

	public override bool MoveToAttribute(string name, string? ns)
	{
		if (_topLevel && XIncludeKeywords.Equals(name, _keywords.Base) && XIncludeKeywords.Equals(ns ?? string.Empty, _keywords.XmlNamespace))
		{
			_state = XIncludingReaderState.ExposingXmlBaseAttr;
			return true;
		}
		else
		{
			return _reader.MoveToAttribute(name, ns);
		}
	}

	public override bool MoveToElement()
	{
		return _reader.MoveToElement();
	}

	public override bool MoveToFirstAttribute()
	{
		bool res = _reader.MoveToFirstAttribute();
		if (_topLevel && !res)
		{
			_state = XIncludingReaderState.ExposingXmlBaseAttr;
			return true;
		}
		else
		{
			return _reader.MoveToFirstAttribute();
		}
	}

	public override bool MoveToNextAttribute()
	{
		bool res = _reader.MoveToNextAttribute();
		if (_topLevel && !res &&
			_reader.GetAttribute(_keywords.Base, _keywords.XmlNamespace)==null)
		{
			//End of attributes and there is no xml:base - expose virtual one
			switch (_state)
			{
				case XIncludingReaderState.ExposingXmlBaseAttr:
				case XIncludingReaderState.ExposingXmlBaseAttrValue:
					_state = XIncludingReaderState.Default;
					return false;
				default:
					_state = XIncludingReaderState.ExposingXmlBaseAttr;
					return true;
			}
		}
		else if (_topLevel && XIncludeKeywords.Equals(_reader.LocalName, _keywords.Base) && XIncludeKeywords.Equals(_reader.NamespaceURI, _keywords.XmlNamespace))
		{
			//There is xml:base already - substitute its value    
			if (res)
			{
				_state = XIncludingReaderState.ExposingXmlBaseAttr;
				return true;
			}
			else
			{
				//No more attributes - clean up
				_state = XIncludingReaderState.Default;
				return false;
			}
		}
		else
		{
			return res;
		}
	}

	public override bool ReadAttributeValue()
	{
		switch (_state)
		{
			case XIncludingReaderState.ExposingXmlBaseAttr:
				_state = XIncludingReaderState.ExposingXmlBaseAttrValue;
				return true;
			case XIncludingReaderState.ExposingXmlBaseAttrValue:
				return false;
			default:
				return _reader.ReadAttributeValue();
		}
	}

	public override ReadState ReadState { get => _reader.ReadState; }

	public override String this[int i] { get => GetAttribute(i); }

	public override string? this[string name] { get => GetAttribute(name); }

	public override string? this[string name, string? namespaceURI] { get => GetAttribute(name, namespaceURI); }

	public override void ResolveEntity()
	{
		_reader.ResolveEntity();
	}

	public override string XmlLang { get => _reader.XmlLang; }

	public override XmlSpace XmlSpace { get => _reader.XmlSpace; }

	public override string Value
	{
		get
		{
			switch (_state)
			{
				case XIncludingReaderState.ExposingXmlBaseAttr:
				case XIncludingReaderState.ExposingXmlBaseAttrValue:
					if (_reader.BaseURI == String.Empty)
					{
						// Stupid reader.
						Uri? baseUri = _baseURIs.Peek() as Uri;
						return baseUri?.AbsoluteUri ?? string.Empty;
					}
					if (_relativeBaseUri)
					{
						Uri baseUri = new(_reader.BaseURI);
						return _topBaseUri.MakeRelativeUri(baseUri).ToString();
					}
					else
					{
						return _reader.BaseURI;
					}
				default:
					return _reader.Value;
			}
		}
	}

	public override string ReadInnerXml()
	{
		return _state switch
		{
			XIncludingReaderState.ExposingXmlBaseAttr => _reader.BaseURI,
			XIncludingReaderState.ExposingXmlBaseAttrValue => String.Empty,
			_ => _reader.ReadInnerXml(),
		};
	}

	public override string ReadOuterXml()
	{
		return _state switch
		{
			XIncludingReaderState.ExposingXmlBaseAttr => @"xml:base="" + _reader.BaseURI + @""",
			XIncludingReaderState.ExposingXmlBaseAttrValue => String.Empty,
			_ => _reader.ReadOuterXml(),
		};
	}

	public override string ReadString()
	{
		return _state switch
		{
			XIncludingReaderState.ExposingXmlBaseAttr => String.Empty,
			XIncludingReaderState.ExposingXmlBaseAttrValue => _reader.BaseURI,
			_ => _reader.ReadString(),
		};
	}

	// Binary content access methods.
	// Added by LAE to allow including reader to read binary content.  Code taken from XmlTextReader.cs source code in C# .NET and adapted to
	// use "_reader" instead of "impl."
	// http://www.dotnetframework.org/default.aspx/Net/Net/3@5@50727@3053/DEVDIV/depot/DevDiv/releases/whidbey/netfxsp/ndp/fx/src/Xml/System/Xml/Core/XmlTextReader@cs/1/XmlTextReader@cs
	public override bool CanReadBinaryContent { get => true; }

	public override int ReadContentAsBase64(byte[] buffer, int index, int count)
	{
		return _reader.ReadContentAsBase64(buffer, index, count);
	}

	public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
	{
		return _reader.ReadElementContentAsBase64(buffer, index, count);
	}

	public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
	{
		return _reader.ReadContentAsBinHex(buffer, index, count);
	}

	public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
	{
		return _reader.ReadElementContentAsBinHex(buffer, index, count);
	}
	// End binary content access method.

	public override bool Read()
	{
		//Read internal reader
		bool baseRead = _reader.Read();
		if (baseRead)
		{
			// If we are including and including reader is at 0 depth - we are in top level included item.
			_topLevel = (_readers.Count>0 && _reader.Depth == 0);
			if (_topLevel && _reader.NodeType == XmlNodeType.Attribute)
			{
				//Attempt to include an attribute
				throw new AttributeOrNamespaceInIncludeLocationError("Include location identifies an attribute or namespace node!");
			}
			switch (_reader.NodeType)
			{
				case XmlNodeType.XmlDeclaration:
				case XmlNodeType.Document:
				case XmlNodeType.DocumentType:
				case XmlNodeType.DocumentFragment:
					//This stuff should not be included                   
					return _readers.Count>0 ? Read() : baseRead;
				case XmlNodeType.Element:
					//Check for xi:include
					if (IsIncludeElement())
					{
						//xi:include element found
						//Save current reader to possible fallback processing
						XmlReader current = _reader;
						try
						{
							return ProcessIncludeElement();
						}
						catch (FatalException)
						{
							throw;
						}
						catch (Exception e)
						{
							//Let's be liberal - any exceptions other than fatal one 
							//should be treated as resource error
							//Console.WriteLine("Resource error has been detected: " + e.Message);
							//Start fallback processing
							if (!current.Equals(_reader))
							{
								_reader.Close();
								_reader = current;
							}
							_prevFallbackState = _fallbackState;
							return ProcessFallback(_reader.Depth, e);
						}
						//No, it's not xi:include, check it for xi:fallback    
					}
					else if (IsFallbackElement())
					{
						//Found xi:fallback not child of xi:include
						if (_reader is XmlTextReader r)
						{
							throw new SyntaxError("xi:fallback element must be direct child of xi:include element."
								+ _reader.BaseURI.ToString() + ", Line " + r.LineNumber + ", Position " + r.LinePosition);
						}
						else
						{
							throw new SyntaxError("xi:fallback element must be direct child of xi:include element." + _reader.BaseURI.ToString());
						}
					}
					else
					{
						goto default;
					}
				case XmlNodeType.EndElement:
					//Looking for end of xi:fallback
					if (_fallbackState.Fallbacking &&
						_reader.Depth == _fallbackState.FallbackDepth &&
						IsFallbackElement())
					{
						//End of fallback processing
						_fallbackState.FallbackProcessed = true;
						//Now read other ignored content till </xi:fallback>
						return ProcessFallback(_reader.Depth-1, null);
					}
					else
					{
						goto default;
					}
				default:
					return baseRead;
			}
		}
		else
		{
			//No more input - finish possible xi:include processing
			if (_topLevel)
			{
				_topLevel = false;
			}

			if (_readers.Count > 0)
			{
				//Pop BaseURI
				_baseURIs.Pop();
				_reader.Close();
				//Pop previous reader
				_reader = (XmlReader)_readers.Pop()!;
				//Successful include - skip xi:include content
				if (!_reader.IsEmptyElement)
				{
					CheckAndSkipContent();
				}
				return Read();
			}
			else
			{
				//That's all, folks
				return false;
			}
		}
	} // Read()

	#endregion

	#region Public members

	public bool Normalization { get => _normalization; set => _normalization = value; }

	public WhitespaceHandling WhitespaceHandling { get => _whiteSpaceHandling; set => _whiteSpaceHandling = value; }

	/// <summary>
	/// XmlResolver to resolve external URI references
	/// </summary>
	public XmlResolver XmlResolver { set => _xmlResolver = value; }

	/// <summary>
	/// Flag indicating whether to emit xml:base as relative URI.
	/// Note, it's true by default
	/// </summary>
	public bool RelativeBaseUri { get => _relativeBaseUri; set => _relativeBaseUri = value; }

	#endregion

	#region Private Methods

	private bool IsIncludeElement()
	{
		if (
			(
				XIncludeKeywords.Equals(_reader.NamespaceURI, _keywords.XIncludeNamespace) ||
				XIncludeKeywords.Equals(_reader.NamespaceURI, _keywords.OldXIncludeNamespace)
			) &&
				XIncludeKeywords.Equals(_reader.LocalName, _keywords.Include))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool IsFallbackElement()
	{
		if (
			(
			XIncludeKeywords.Equals(_reader.NamespaceURI, _keywords.XIncludeNamespace) ||
			XIncludeKeywords.Equals(_reader.NamespaceURI, _keywords.OldXIncludeNamespace)
			) &&
			XIncludeKeywords.Equals(_reader.LocalName, _keywords.Fallback))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	internal static Stream GetResource(string href, Uri includeLocation, string accept, string acceptCharset, string acceptLanguage, out WebResponse response)
	{
		WebRequest webRequest;
		try
		{
			webRequest = WebRequest.Create(includeLocation);
		}
		catch (NotSupportedException nse)
		{
			throw new ResourceException("URI schema is not supported: '" + href + "'", nse);
		}
		catch (SecurityException se)
		{
			throw new ResourceException("Security exception while fetching '" + href + "'", se);
		}
		// Add accept headers if this is HTTP request.
		if (webRequest is HttpWebRequest httpWebRequest)
		{
			if (accept != null)
			{
				if (httpWebRequest.Accept == null || httpWebRequest.Accept == String.Empty)
				{
					httpWebRequest.Accept = accept;
				}
				else
				{
					httpWebRequest.Accept += "," + accept;
				}
			}
			if (acceptCharset != null)
			{
				if (httpWebRequest.Headers["Accept-Charset"] == null)
				{
					httpWebRequest.Headers.Add("Accept-Charset", acceptCharset);
				}
				else
				{
					httpWebRequest.Headers["Accept-Charset"] += ","+acceptCharset;
				}
			}
			if (acceptLanguage != null)
			{
				httpWebRequest.Headers.Add("Accept-Language", "ru");
				if (httpWebRequest.Headers["Accept-Language"] == null)
				{
					httpWebRequest.Headers.Add("Accept-Language", acceptLanguage);
				}
				else
				{
					httpWebRequest.Headers["Accept-Language"] += ","+acceptLanguage;
				}
			}
		}
		try
		{
			response = webRequest.GetResponse();
		}
		catch (WebException we)
		{
			throw new ResourceException("Resource '" + href + "' cannot be fetched", we);
		}
		return response.GetResponseStream();
	}

	/// <summary>
	/// Processes xi:include element.
	/// </summary>		
	private bool ProcessIncludeElement()
	{
		string	href		= _reader.GetAttribute(_keywords.Href) ?? string.Empty;
		string?	xpointer	= _reader.GetAttribute(_keywords.Xpointer);
		if (href == null)
		{
			if (xpointer == null)
			{
				// Both href and xpointer attributes are absent, critical error.
				if (_reader is XmlTextReader r)
				{
					throw new MissingHrefAndXpointerException("'href' or 'xpointer' attribute is required on xi:include element. "
						+ _reader.BaseURI.ToString() + ", Line " + r.LineNumber + ", Position " + r.LinePosition);
				}
				else
				{
					throw new MissingHrefAndXpointerException("'href' or 'xpointer' attribute is required on xi:include element. " + _reader.BaseURI.ToString());
				}
			}
			else
			{
				// No href - intra-document reference.
				throw new NotImplementedException("Intra-document references are not implemented yet!");
			}
		}
		string? parse = _reader.GetAttribute(_keywords.Parse);
		if (parse == null || parse.Equals(_keywords.Xml))
		{
			// Include document as XML.                                .
			Uri includeLocation = ResolveHref(href);
			if (_xmlResolver == null)
			{
				// No custom resolver.
				Stream stream =  GetResource(href, includeLocation,
					_reader.GetAttribute(_keywords.Accept) ?? string.Empty,
					_reader.GetAttribute(_keywords.AcceptCharset) ?? string.Empty,
					_reader.GetAttribute(_keywords.AcceptLanguage) ?? string.Empty,
					out WebResponse wRes);
				// Push new base URI to the stack.
				_baseURIs.Push(includeLocation);
				// Push current reader to the stack.
				_readers.Push(_reader);
				if (xpointer != null)
				{
					_reader = new XPointerReader(wRes.ResponseUri, stream, _nameTable, xpointer);
				}
				else if (includeLocation.Fragment != String.Empty)
				{
					_reader = new XPointerReader(wRes.ResponseUri, stream, _nameTable, includeLocation.Fragment[1..]);
				}
				else
				{
					_reader = new XmlTextReader(wRes.ResponseUri.AbsoluteUri, stream, _nameTable);
					((XmlTextReader)_reader).Normalization = _normalization;
					((XmlTextReader)_reader).WhitespaceHandling = _whiteSpaceHandling;
				}
				bool res = Read();
				return res;
			}
			else
			{
				// Custom resolver provided, let's ask him.
				object? resource;
				try
				{
					resource = _xmlResolver.GetEntity(includeLocation, null, null);
				}
				catch (Exception e)
				{
					throw new ResourceException("An exception has occured during GetEntity call to custom XmlResolver", e);
				}
				if (resource == null)
				{
					throw new ResourceException("Custom XmlResolver returned null");
				}
				// Ok, we accept Stream and XmlReader only.
				XmlReader r;
				if (resource is Stream stream)
				{
					r = new XmlTextReader(includeLocation.AbsoluteUri, stream, _nameTable);
				}
				else if (resource is XmlReader reader)
				{
					r = reader;
				}
				else
				{
					// Unsupported type.
					throw new ResourceException("Custom XmlResolver returned object of unsupported type.");
				}
				// Push new base URI to the stack.
				_baseURIs.Push(includeLocation);
				// Push current reader to the stack.
				_readers.Push(_reader);
				if (xpointer != null)
				{
					_reader = new XPointerReader(r, _nameTable, xpointer);
				}
				else
				{
					_reader = r;
				}
				bool res = Read();
				return res;
			}
		}
		else if (parse.Equals(_keywords.Text))
		{
			// Include document as text.
			string encoding = GetAttribute(_keywords.Encoding) ?? string.Empty;
			Uri includeLocation = ResolveHref(href);
			// Push new base URI to the stack.
			_baseURIs.Push(includeLocation);
			// Push current reader to the stack.
			_readers.Push(_reader);
			_reader = new TextIncludingReader(
				includeLocation, encoding,
				_reader.GetAttribute(_keywords.Accept) ?? string.Empty,
				_reader.GetAttribute(_keywords.AcceptCharset) ?? string.Empty,
				_reader.GetAttribute(_keywords.AcceptLanguage) ?? string.Empty
			);
			return Read();
		}
		else
		{
			// Unknown "parse" attribute value, critical error.
			if (_reader is XmlTextReader r)
			{
				throw new UnknownParseAttributeValueException(parse, _reader.BaseURI.ToString(), r.LineNumber, r.LinePosition);
			}
			else
			{
				throw new UnknownParseAttributeValueException(parse);
			}
		}
	}

	/// <summary>
	/// Resolves include locatation.
	/// </summary>
	/// <param name="href">href value</param>
	/// <returns>Include location.</returns>
	private Uri ResolveHref(string href)
	{
		Uri includeLocation;
		try
		{
			if (_xmlResolver == null)
			{
				includeLocation = new Uri(new Uri(_reader.BaseURI), href);
			}
			else
			{
				includeLocation = _xmlResolver.ResolveUri(new Uri(_reader.BaseURI), href);
			}
		}
		catch (UriFormatException ufe)
		{
			throw new ResourceException("Invalid URI '" + href + "'", ufe);
		}
		catch (Exception e)
		{
			throw new ResourceException("Unable to resolve URI reference '" + href + "'", e);
		}
		// Check circular inclusion.
		if (_baseURIs.Contains(includeLocation))
		{
			if (_reader is XmlTextReader reader)
			{
				throw new CircularInclusionException(includeLocation, BaseURI.ToString(), reader.LineNumber, reader.LinePosition);
			}
			else
			{
				throw new CircularInclusionException(includeLocation);
			}
		}
		return includeLocation;
	}

	/// <summary>
	/// Skips content of an element using directly current reader's methods.
	/// </summary>
	private void SkipContent()
	{
		if (!_reader.IsEmptyElement)
		{
			int depth = _reader.Depth;
			while (_reader.Read() && depth<_reader.Depth)
				;
		}
	}

	/// <summary>
	/// Fallback processing.
	/// </summary>
	/// <param name="depth"><c>xi:include</c> depth level.</param>    
	/// <param name="exception">Resource error, which caused this processing.</param>
	/// <remarks>When inluding fails due to any resource error, <c>xi:inlcude</c> 
	/// element content is processed as follows: each child <c>xi:include</c> - 
	/// fatal error, more than one child <c>xi:fallback</c> - fatal error. No 
	/// <c>xi:fallback</c> - the resource error results in a fatal error.
	/// Content of first <c>xi:fallback</c> element is included in a usual way.</remarks>
	private bool ProcessFallback(int depth, Exception? exception)
	{
		//Read to the xi:include end tag
		while (_reader.Read() && depth<_reader.Depth)
		{
			switch (_reader.NodeType)
			{
				case XmlNodeType.Element:
					if (IsIncludeElement())
					{
						//xi:include child of xi:include - fatal error
						if (_reader is XmlTextReader r)
						{
							throw new SyntaxError("xi:include element cannot be child of xi:include element." + BaseURI.ToString() + ", Line " + r.LineNumber + ", Position " + r.LinePosition);
						}
						else
							throw new SyntaxError("xi:include element cannot be child of xi:include element.");
					}
					if (IsFallbackElement())
					{
						//Found xi:fallback
						if (_fallbackState.FallbackProcessed)
						{
							//Two xi:fallback
							if (_reader is XmlTextReader r)
							{
								throw new SyntaxError("xi:include element cannot contain more than one xi:fallback element." + BaseURI.ToString() + ", Line " + r.LineNumber + ", Position " + r.LinePosition);
							}
							else
							{
								throw new SyntaxError("xi:include element cannot contain more than one xi:fallback element.");
							}
						}
						if (_reader.IsEmptyElement)
						{
							//Empty xi:fallback - nothing to include
							_fallbackState.FallbackProcessed = true;
							break;
						}
						_fallbackState.Fallbacking = true;
						_fallbackState.FallbackDepth = _reader.Depth;
						return Read();
					}
					else
						//Ignore anything else along with its content
						SkipContent();
					break;
				default:
					break;
			}
		}
		//xi:include content is read
		if (!_fallbackState.FallbackProcessed)
		{
			//No xi:fallback, fatal error
			if (exception == null)
			{
				throw new FatalResourceException();
			}
			else
			{
				throw new FatalResourceException(exception);
			}
		}
		else
		{
			//End of xi:include content processing, reset and go forth
			_fallbackState = _prevFallbackState;
			return Read();
		}
	}

	/// <summary>
	/// Skips xi:include element's content, while checking XInclude syntax (no 
	/// xi:include, no more than one xi:fallback).
	/// </summary>
	private void CheckAndSkipContent()
	{
		int depth = _reader.Depth;
		bool fallbackElem = false;
		while (_reader.Read() && depth<_reader.Depth)
		{
			switch (_reader.NodeType)
			{
				case XmlNodeType.Element:
					if (IsIncludeElement())
					{
						//xi:include child of xi:include - fatal error
						if (_reader is XmlTextReader r)
						{
							throw new SyntaxError("xi:include element cannot be child of xi:include element." 
								+ _reader.BaseURI.ToString() + ", Line " + r.LineNumber + ", Position " + r.LinePosition);
						}
						else
						{
							throw new SyntaxError("xi:include element cannot be child of xi:include element.");
						}
					}
					if (IsFallbackElement())
					{
						//Found xi:fallback
						if (fallbackElem)
						{
							//More than one xi:fallback
							if (_reader is XmlTextReader r)
							{
								throw new SyntaxError("xi:include element cannot contain more than one xi:fallback element."
									+ _reader.BaseURI.ToString() + ", Line " + r.LineNumber + ", Position " + r.LinePosition);
							}
							else
								throw new SyntaxError("xi:include element cannot contain more than one xi:fallback element.");
						}
						else
						{
							fallbackElem = true;
							SkipContent();
						}
					}
					else
						//Ignore everything else
						SkipContent();
					break;
				default:
					break;
			}
		}
	} // CheckAndSkipContent()

	#endregion
}