using System.Threading.Channels;

namespace Lynx.Cli;

public sealed class Writer
{
    private readonly ChannelReader<object> _engineOutputReader;

    public Writer(ChannelReader<object> engineOutputReader)
    {
        _engineOutputReader = engineOutputReader;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var output in _engineOutputReader.ReadAllAsync(cancellationToken))
            {
                var str = output.ToString();
                Console.WriteLine(str);
            }
        }
        catch (Exception e)
        {
        }
    }
}
