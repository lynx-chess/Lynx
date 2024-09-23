using NLog;
using System.Threading.Channels;

namespace Lynx.Cli;

public sealed class Writer
{
    private readonly ChannelReader<object> _engineOutputReader;
    private readonly Logger _logger;

    public Writer(ChannelReader<object> engineOutputReader)
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
                var str = output.ToString();
                Console.WriteLine(str);

                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("[Lynx]\t{0}", str);
                }
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
