namespace Lynx;

public class AssertException : Exception
{
    public AssertException() : base()
    {
    }

    public AssertException(string? message) : base(message)
    {
    }

    public AssertException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
