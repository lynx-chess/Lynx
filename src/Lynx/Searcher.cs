using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Threading.Channels;

namespace Lynx;

public sealed class Searcher
{
    private readonly ChannelReader<string> _uciReader;
    private readonly ChannelWriter<object> _engineWriter;
    private readonly Logger _logger;
    private readonly TranspositionTable _tt;

    public Searcher(ChannelReader<string> uciReader, ChannelWriter<object> engineWriter)
    {
        _uciReader = uciReader;
        _engineWriter = engineWriter;

        _tt = new TranspositionTable();
        _logger = LogManager.GetCurrentClassLogger();
        Engine = new Engine(_engineWriter, _tt);
    }

    public Engine Engine { get; }

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
                        OnGoCommand(new GoCommand(rawCommand));
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error trying to read/parse UCI command");
                }
            }
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Error in search thread");
        }
        finally
        {
            _logger.Info("Finishing {0}", nameof(Searcher));
        }
    }

    private void OnGoCommand(GoCommand goCommand)
    {
        var searchConstraints = TimeManager.CalculateTimeManagement(Engine.Game, goCommand);

        var searchResult = Engine.Search(goCommand, searchConstraints);

        if (searchResult is not null)
        {
            // We always print best move, even in case of go ponder + stop, in which case IDEs are expected to ignore it
            _engineWriter.TryWrite(new BestMoveCommand(searchResult));
        }
    }
}
