namespace DigitalProduction.Xml.XPointer;

public abstract class XPointerException : ApplicationException
{
	public XPointerException(string message)
	  : base(message)
	{
	}

	public XPointerException(string message, Exception innerException)
	  : base(message, innerException)
	{
	}
}
