namespace Lynx;

public class ConfigurationException : LynxException
{
    public ConfigurationException()
    {
    }

    public ConfigurationException(string? message) : base(message)
    {
    }

    public ConfigurationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
