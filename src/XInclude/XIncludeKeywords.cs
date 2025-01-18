using System.Xml;

namespace DigitalProduction.Xml.XInclude;

/// <summary>
/// XInclude syntax keyword collection.	
/// </summary>
/// <author>Oleg Tkachenko, oleg@tkachenko.com</author>
internal class XIncludeKeywords
{
	readonly XmlNameTable nameTable;

	// Keyword strings
	private const string s_xIncludeNamespace	= "http://www.w3.org/2003/XInclude";
	private const string s_oldXIncludeNamespace	= "http://www.w3.org/2001/XInclude";
	private const string s_include				= "include";
	private const string s_href					= "href";
	private const string s_parse				= "parse";
	private const string s_xml					= "xml";
	private const string s_text					= "text";
	private const string s_xPointer				= "xpointer";
	private const string s_accept				= "accept";
	private const string s_acceptCharset		= "accept-charset";
	private const string s_acceptLanguage		= "accept-language";
	private const string s_encoding				= "encoding";
	private const string s_fallback				= "fallback";
	private const string s_xmlNamespace			= "http://www.w3.org/XML/1998/namespace";
	private const string s_base					= "base";
	private const string s_xmlBase				= "xml:base";

	// Properties
	private readonly string	_xIncludeNamespace;
	private readonly string	_oldXIncludeNamespace;
	private readonly string	_Include;
	private readonly string	_Href;
	private readonly string	_parse;
	private string?			_xml;
	private string?			_text;
	private string?			_xPointer;
	private string?			_accept;
	private string?			_acceptCharset;
	private string?			_acceptLanguage;
	private string?			_encoding;
	private string?			_fallback;
	private string?			_xmlNamespace;
	private string?			_base;
	private string?			_xmlBase;

	internal XIncludeKeywords(XmlNameTable nt)
	{
		nameTable				= nt;
		
		// Preload some keywords.
		_xIncludeNamespace		= nameTable.Add(s_xIncludeNamespace);
		_oldXIncludeNamespace	= nameTable.Add(s_oldXIncludeNamespace);
		_Include				= nameTable.Add(s_include);
		_Href					= nameTable.Add(s_href);
		_parse					= nameTable.Add(s_parse);
	}

	// http://www.w3.org/2003/XInclude
	internal string XIncludeNamespace { get => _xIncludeNamespace; }

	// http://www.w3.org/2001/XInclude
	internal string OldXIncludeNamespace { get => _oldXIncludeNamespace; }

	// include
	internal string Include { get => _Include; }

	// href
	internal string Href { get => _Href; }

	// parse
	internal string Parse { get => _parse; }

	// xml
	internal string Xml
	{
		get
		{
			_xml ??= nameTable.Add(s_xml);
			return _xml;
		}
	}

	// text
	internal string Text
	{
		get
		{
			_text ??= nameTable.Add(s_text);
			return _text;
		}
	}

	// xpointer
	internal string Xpointer
	{
		get
		{
			_xPointer ??= nameTable.Add(s_xPointer);
			return _xPointer;
		}
	}

	// accept
	internal string Accept
	{
		get
		{
			_accept ??= nameTable.Add(s_accept);
			return _accept;
		}
	}

	// accept-charset
	internal string AcceptCharset
	{
		get
		{
			_acceptCharset ??= nameTable.Add(s_acceptCharset);
			return _acceptCharset;
		}
	}

	// accept-language
	internal string AcceptLanguage
	{
		get
		{
			_acceptLanguage ??= nameTable.Add(s_acceptLanguage);
			return _acceptLanguage;
		}
	}

	// encoding
	internal string Encoding
	{
		get
		{
			_encoding ??= nameTable.Add(s_encoding);
			return _encoding;
		}
	}

	// fallback
	internal string Fallback
	{
		get
		{
			_fallback ??= nameTable.Add(s_fallback);
			return _fallback;
		}
	}

	// Xml namespace
	internal string XmlNamespace
	{
		get
		{
			_xmlNamespace ??= nameTable.Add(s_xmlNamespace);
			return _xmlNamespace;
		}
	}

	// Base
	internal string Base
	{
		get
		{
			_base ??= nameTable.Add(s_base);
			return _base;
		}
	}

	// xml:base
	internal string XmlBase
	{
		get
		{
			_xmlBase ??= nameTable.Add(s_xmlBase);
			return _xmlBase;
		}
	}

	// Comparison
	internal static bool Equals(string keyword1, string keyword2)
	{
		return (object)keyword1 == (object)keyword2;
	}
}