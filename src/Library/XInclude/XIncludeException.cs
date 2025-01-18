namespace DigitalProduction.Xml.XInclude;

/// <summary>
/// Generic XInclude exception.	
/// </summary>
public abstract class XIncludeException : ApplicationException
{
	public XIncludeException(string message) : 
		base(message)
	{
	}

	public XIncludeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}

/// <summary>
/// <c>ResourceException</c> represents resource error as per XInclude specification.
/// </summary>
/// <remarks>
/// Resource error is internal error and should lead to fallback processing.
/// <c>ResourceException</c> therefore should never be thrown outside 
/// the XInclude Processor.
/// </remarks>
internal class ResourceException : XIncludeException
{
	public ResourceException(string message) :
		base(message)
	{
	}
	
	public ResourceException(string message, Exception innerException) :
		base(message, innerException)
	{
	}
}

/// <summary>
/// <c>FatalException</c> represents fatal error as per XInclude spcification.
/// </summary>
public abstract class FatalException : XIncludeException
{
	public FatalException(string message) : 
		base(message)
	{
	}
	
	public FatalException(string message, Exception innerException) :
		base(message, innerException)
	{
	}
}

/// <summary>
/// Missing both "href" and "xpointer" attributes exception.
/// </summary>
public class MissingHrefAndXpointerException(string message) :
	FatalException(message)
{
}

/// <summary>
/// Unknown "parse" attribute value exception.
/// </summary>
public class UnknownParseAttributeValueException : FatalException
{
	public UnknownParseAttributeValueException(string attrValue) :
		base("Unknown 'parse' attribute value: '" + attrValue + "'.")
	{
	}

	public UnknownParseAttributeValueException(string attrValue, string uri, int line, int position) :
		base("Unknown 'parse' attribute value: '" + attrValue + "'." + uri + ", Line " + line + ", Position " + position)
	{
	}
}

/// <summary>
/// Non XML character in a document to include exception.
/// </summary>
public class NonXmlCharacterException(char c) :
	FatalException("Included document contains forbidden in XML character: 0x"+ ((int)c).ToString("X2"))
{
}

/// <summary>
/// Circular inclusion exception.
/// </summary>
public class CircularInclusionException : FatalException
{
	public CircularInclusionException(Uri uri) :
		base("Circular inclusion has been detected, inclusion location: " + uri.AbsoluteUri)
	{
	}

	public CircularInclusionException(Uri uri, string locationUri, int line, int position) :
		base("Circular inclusion has been detected, inclusion location: " + uri.AbsoluteUri + "."
		+ locationUri + ", Line " + line + ", Position " + position)
	{
	}
}

/// <summary>
/// Resource error not backed up by xi:fallback exception.
/// </summary>	
public class FatalResourceException : FatalException
{
	public FatalResourceException(Exception re) :
		base("Resource error has occured and no fallback has been provided: " + re.Message, re)
	{
	}

	public FatalResourceException() :
		base("Resource error has occured and no fallback has been provided.")
	{
	}
}

/// <summary>
/// XInclude syntax error exception.
/// </summary>
public class SyntaxError(string message) :
	FatalException(message)
{
}

/// <summary>
/// Include location identifies an attribute or namespace node.
/// </summary>
public class AttributeOrNamespaceInIncludeLocationError(string message) :
	FatalException(message)
{
}