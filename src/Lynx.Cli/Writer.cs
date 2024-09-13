﻿using NLog;
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
                _logger.Debug("[Lynx]\t{0}", output);
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
