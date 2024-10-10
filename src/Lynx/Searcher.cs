using Lynx.UCI.Commands.GUI;
using System.Threading.Channels;

namespace Lynx;

public sealed class Searcher
{
    private readonly ChannelReader<string> _uciReader;
    private readonly Engine _engine;

    public Searcher(ChannelReader<string> uciReader, Engine engine)
    {
        _uciReader = uciReader;
        _engine = engine;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        try
        {
            while (await _uciReader.WaitToReadAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_uciReader.TryRead(out var rawCommand))
                    {
                        _engine.Search(new GoCommand(rawCommand));
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
        catch (Exception e)
        {
        }
        finally
        {
        }
    }
}
