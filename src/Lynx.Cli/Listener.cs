using NLog;

namespace Lynx.Cli;

public sealed class Listener
{
    private readonly Logger _logger;
    private readonly UCIHandler _uciHandler;

    public Listener(UCIHandler uCIHandler)
    {
        _uciHandler = uCIHandler;
        _logger = LogManager.GetCurrentClassLogger();
    }

    public async Task Run(CancellationToken cancellationToken, params string[] args)
    {
        try
        {
            foreach (var arg in args)
            {
                await _uciHandler.Handle(arg, cancellationToken);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                var input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                {
                    await _uciHandler.Handle(input!, cancellationToken);
                }
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
