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

    private readonly Engine _engine;

    public Position CurrentPosition => _engine.Game.CurrentPosition;

    public string FEN => _engine.Game.FEN;

    public Searcher(ChannelReader<string> uciReader, ChannelWriter<object> engineWriter)
    {
        _uciReader = uciReader;
        _engineWriter = engineWriter;

        TranspositionTable _ttWrapper = new();
        _engine = new Engine(_engineWriter, _ttWrapper);

        _logger = LogManager.GetCurrentClassLogger();
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

    public void PrintCurrentPosition() => _engine.Game.CurrentPosition.Print();

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

    public void AdjustPosition(ReadOnlySpan<char> command)
    {
        _engine.AdjustPosition(command);
    }

    public void StopSearching()
    {
        _engine.StopSearching();
    }

    public void PonderHit()
    {
        _engine.PonderHit();
    }

    public void NewGame()
    {
        var averageDepth = _engine.AverageDepth;
        if (averageDepth > 0 && averageDepth < int.MaxValue)
        {
            _logger.Info("Average depth: {0}", averageDepth);
        }

        _engine.NewGame();
    }

    public void Quit()
    {
        var averageDepth = _engine.AverageDepth;
        if (averageDepth > 0 && averageDepth < int.MaxValue)
        {
            _logger.Info("Average depth: {0}", averageDepth);
        }
    }

    public async ValueTask RunBench(int depth)
    {
        var results = _engine.Bench(depth);
        await _engine.PrintBenchResults(results);
    }
}
