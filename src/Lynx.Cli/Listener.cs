﻿using NLog;
using System.Threading.Channels;

namespace Lynx.Cli;

public sealed class Listener
{
    private readonly Channel<string> _guiInputReader;
    private readonly Logger _logger;

    public Listener(Channel<string> guiInputReader)
    {
        _guiInputReader = guiInputReader;
        _logger = LogManager.GetCurrentClassLogger();
    }

    public async Task Run(CancellationToken cancellationToken, params string[] args)
    {
        try
        {
            foreach (var arg in args)
            {
                await _guiInputReader.Writer.WriteAsync(arg, cancellationToken);
            }

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
            _logger.Info("Finishing {0}", nameof(Listener));
        }
    }
}
