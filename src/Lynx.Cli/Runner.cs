using Lynx.UCI.Commands.Engine;
using NLog;
using System.Threading.Channels;
using Lynx.Import;

namespace Lynx.Cli;

public static class Runner
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static async Task Run(params string[] args)
    {
        var uciChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = true, FullMode = BoundedChannelFullMode.Wait });
        var engineChannel = Channel.CreateBounded<object>(new BoundedChannelOptions(2 * Configuration.EngineSettings.MaxDepth) { SingleReader = true, SingleWriter = false, FullMode = BoundedChannelFullMode.DropOldest });

        using CancellationTokenSource source = new();
        CancellationToken cancellationToken = source.Token;

        var searcher = new Searcher(uciChannel, engineChannel);
        var uciHandler = new UCIHandler(uciChannel, engineChannel, searcher);
        var writer = new Writer(engineChannel);
        var listener = new Listener(uciHandler);

        var tasks = new List<Task>
        {
            Task.Run(() => writer.Run(cancellationToken)),
            Task.Run(() => searcher.Run(cancellationToken)),
            Task.Run(() => listener.Run(cancellationToken, args)),
            uciChannel.Reader.Completion,
            engineChannel.Reader.Completion
        };

        try
        {
            // CLI arg: --load-viriformat <path>
            if (args is not null && args.Length >= 2)
            {
                for (int i = 0; i < args.Length - 1; ++i)
                {
                    if (args[i] == "--load-viriformat")
                    {
                        var path = args[i + 1];
                        try
                        {
                            var (game, scores) = ViriformatLoader.LoadFile(path);
                            // Set into searcher
                            searcher.SetInitialGame(game);
                            _logger.Info("Loaded viriformat file {0} with {1} move scores", path, scores.Count);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, "Error loading viriformat file {0}", path);
                        }
                    }
                }
            }
            Console.WriteLine($"{IdCommand.EngineName} {IdCommand.GetLynxVersion()} by {IdCommand.EngineAuthor}");
            await Task.WhenAny(tasks);
        }
        catch (AggregateException ae)
        {
            foreach (var e in ae.InnerExceptions)
            {
                if (e is TaskCanceledException taskCanceledException)
                {
                    Console.WriteLine("Cancellation requested exception: {0}", taskCanceledException.Message);
                    _logger.Fatal(ae, "Cancellation requested exception: {0}", taskCanceledException.Message);
                }
                else
                {
                    Console.WriteLine("Exception {0}: {1}", e.GetType().Name, e.Message);
                    _logger.Fatal(ae, "Exception {0}: {1}", e.GetType().Name, e.Message);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Unexpected exception");
            Console.WriteLine(e.Message);

            _logger.Fatal(e, "Unexpected exception: {Exception}", e.Message);
        }
        finally
        {
            engineChannel.Writer.TryComplete();
            uciChannel.Writer.TryComplete();
            //source.Cancel();
            LogManager.Shutdown(); // Flush and close down internal threads and timers
        }
    }
}
