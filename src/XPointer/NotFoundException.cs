namespace GotDotNet.XPointer;

public class NotFoundException : XPointerException
{
	public NotFoundException(string message)
	  : base(message)
	{
	}

	public NotFoundException(string message, Exception innerException)
	  : base(message, innerException)
	{
	}
}
