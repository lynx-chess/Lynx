namespace Lynx;

public class AssertException : Exception
{
    public AssertException()
    {
    }

    public AssertException(string? message) : base(message)
    {
    }

    public AssertException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
