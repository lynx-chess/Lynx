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
    private readonly Engine _engine;
    private readonly Logger _logger;
    private readonly TranspositionTable _tt;

    public Searcher(ChannelReader<string> uciReader, ChannelWriter<object> engineWriter, Engine engine)
    {
        _uciReader = uciReader;
        _engineWriter = engineWriter;

        _tt = new TranspositionTable();
        _engine = engine;

        _logger = LogManager.GetCurrentClassLogger();
    }

    public Engine Engine => _engine;

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
        var searchConstraints = TimeManager.CalculateTimeManagement(_engine.Game, goCommand);

        var searchResult = _engine.Search(goCommand, searchConstraints);

        if (searchResult is not null)
        {
            // We always print best move, even in case of go ponder + stop, in which case IDEs are expected to ignore it
            _engineWriter.TryWrite(new BestMoveCommand(searchResult));
        }
    }
}
