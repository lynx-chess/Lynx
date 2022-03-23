using System.Threading.Channels;

namespace Lynx.Api;

public class EngineOutputWrapper
{
    public ChannelReader<string> Channel { get; }

    public EngineOutputWrapper(ChannelReader<string> engineOutputReader)
    {
        Channel = engineOutputReader;
    }
}

public class UserInputWrapper
{
    public Channel<string> Channel { get; }

    public UserInputWrapper(Channel<string> engineOutputReader)
    {
        Channel = engineOutputReader;
    }
}
