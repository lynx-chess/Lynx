namespace Lynx;

public class LynxException : Exception
{
    public LynxException()
    {
    }

    public LynxException(string? message) : base(message)
    {
    }

    public LynxException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
