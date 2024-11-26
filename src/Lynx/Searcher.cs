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

    private Engine _engine;
    private int _currentTranspositionTableSize;
    private TranspositionTable _ttWrapper;

    public Position CurrentPosition => _engine.Game.CurrentPosition;

    public string FEN => _engine.Game.FEN;

    public Searcher(ChannelReader<string> uciReader, ChannelWriter<object> engineWriter)
    {
        _uciReader = uciReader;
        _engineWriter = engineWriter;

        _ttWrapper = new TranspositionTable();
        _engine = new Engine(_engineWriter, in _ttWrapper);

        _logger = LogManager.GetCurrentClassLogger();

        _currentTranspositionTableSize = Configuration.EngineSettings.TranspositionTableSize;

        InitializeStaticClasses();
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

        if (_currentTranspositionTableSize == Configuration.EngineSettings.TranspositionTableSize)
        {
            _ttWrapper.Clear();
            _engine.NewGame();
        }
        else
        {
            _logger.Info("Resizing TT ({CurrentSize} MB -> {NewSize} MB)", _currentTranspositionTableSize, Configuration.EngineSettings.TranspositionTableSize);
            _currentTranspositionTableSize = Configuration.EngineSettings.TranspositionTableSize;

            _ttWrapper = new TranspositionTable();

            _engine.FreeResources();
            _engine = new Engine(_engineWriter, in _ttWrapper);
        }

#pragma warning disable S1215 // "GC.Collect" should not be called
        GC.Collect();
        GC.WaitForPendingFinalizers();
#pragma warning restore S1215 // "GC.Collect" should not be called
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

    private static void InitializeStaticClasses()
    {
        _ = PVTable.Indexes[0];
        _ = Attacks.KingAttacks;
        _ = ZobristTable.SideHash();
        _ = Masks.FileMasks;
        _ = EvaluationConstants.HistoryBonus[1];
        _ = MoveGenerator.Init();
        _ = GoCommand.Init();
    }
}
