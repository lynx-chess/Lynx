using NLog;
using System.Threading.Channels;

namespace Lynx.Cli;

public sealed class Listener
{
    private readonly Channel<string> _guiInputReader;
    private readonly ILogger _logger;

    public Listener(Channel<string> guiInputReader)
    {
        _guiInputReader = guiInputReader;
        _logger = LogManager.GetCurrentClassLogger();
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = Console.ReadLine();
                await _guiInputReader.Writer.WriteAsync(input!, cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.Fatal(e);
        }
        finally
        {
            _logger.Info($"Finishing {nameof(Listener)}");
        }
    }
}
