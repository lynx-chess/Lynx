using Lynx.UCI.Commands;
using Lynx.UCI.Commands.Engine;
using NLog;
using System.Threading.Channels;

namespace Lynx.Cli;

public sealed class Writer
{
    private readonly ChannelReader<EngineBaseCommand> _engineOutputReader;
    private readonly Logger _logger;

    public Writer(ChannelReader<EngineBaseCommand> engineOutputReader)
    {
        _engineOutputReader = engineOutputReader;
        _logger = LogManager.GetCurrentClassLogger();
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var output in _engineOutputReader.ReadAllAsync(cancellationToken))
            {
                Console.WriteLine(output.ToString());
            }
        }
        catch (Exception e)
        {
            _logger.Fatal(e);
        }
        finally
        {
            _logger.Info("Finishing {0}", nameof(Writer));
        }
    }
}
