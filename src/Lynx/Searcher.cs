using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Diagnostics;
using System.Threading.Channels;

namespace Lynx;

public sealed class Searcher
{
    private readonly ChannelReader<string> _uciReader;
    private readonly ChannelWriter<object> _engineWriter;
    private readonly Logger _logger;

    private int _searchThreadsCount;
    private Engine _mainEngine;
    private Engine[] _extraEngines = [];
    private TranspositionTable _ttWrapper;

    public Position CurrentPosition => _mainEngine.Game.CurrentPosition;

    public string FEN => _mainEngine.Game.FEN;

    public Searcher(ChannelReader<string> uciReader, ChannelWriter<object> engineWriter)
    {
        _uciReader = uciReader;
        _engineWriter = engineWriter;

        _ttWrapper = new TranspositionTable();
        _mainEngine = new Engine("1", _engineWriter, in _ttWrapper, warmup: true);

        _searchThreadsCount = Configuration.EngineSettings.Threads;
        AllocateExtraEngines();

        _logger = LogManager.GetCurrentClassLogger();
        _logger.Info("Threads:\t{0}", _searchThreadsCount);

        InitializeStaticClasses();

        ForceGCCollection();
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
                        await OnGoCommand(new GoCommand(rawCommand));
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

    public void PrintCurrentPosition() => _mainEngine.Game.CurrentPosition.Print();

    private async Task OnGoCommand(GoCommand goCommand)
    {
        //TaskFactory taskFactory = new();
        var searchConstraints = TimeManager.CalculateTimeManagement(_mainEngine.Game, goCommand);

        if (_searchThreadsCount == 1)
        {
            SingleThreadedSearch(goCommand);
        }
        else
        {
            await MultiThreadedSearch(goCommand);
        }
    }

    private void SingleThreadedSearch(GoCommand goCommand)
    {
        var searchConstraints = TimeManager.CalculateTimeManagement(_mainEngine.Game, goCommand);
        var searchResult = _mainEngine.Search(goCommand, searchConstraints);

        if (searchResult is not null)
        {
            // We always print best move, even in case of go ponder + stop, in which case IDEs are expected to ignore it
            _engineWriter.TryWrite(new BestMoveCommand(searchResult));
        }
    }

    private async Task MultiThreadedSearch(GoCommand goCommand)
    {
        var searchConstraints = TimeManager.CalculateTimeManagement(_mainEngine.Game, goCommand);

#if MULTITHREAD_DEBUG
        var sw = Stopwatch.StartNew();
        var lastElapsed = sw.ElapsedMilliseconds;
#endif

        // Lazy SMP implementation

        // Extra engines run in "go infinite" mode
        // TODO: fix pondering, since pondering logic updated the boolean in the constraint object instead of a field
        var extraEnginesSearchConstraint = SearchConstraints.InfiniteSearchConstraint;

        var tasks = _extraEngines
            .Select(engine =>
                Task.Run(() => engine.Search(goCommand, extraEnginesSearchConstraint)))
            //taskFactory.StartNew(() => engine.Search(goCommand, extraEnginesSearchConstraint)))
            .ToArray();

#if MULTITHREAD_DEBUG
        _logger.Warn("End of extra searches prep, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
        lastElapsed = sw.ElapsedMilliseconds;
#endif

        SearchResult? finalSearchResult = _mainEngine.Search(goCommand, searchConstraints);

#if MULTITHREAD_DEBUG
        _logger.Warn("End of main search, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
        lastElapsed = sw.ElapsedMilliseconds;
#endif

        foreach (var engine in _extraEngines)
        {
            engine.StopSearching();
        }

        var extraResults = await Task.WhenAll(tasks);

#if MULTITHREAD_DEBUG
        _logger.Warn("End of extra searches, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
#endif

        if (finalSearchResult is not null)
        {
            foreach (var extraResult in extraResults)
            {
                finalSearchResult.Nodes += extraResult?.Nodes ?? 0;
            }

            finalSearchResult.NodesPerSecond = Utils.CalculateNps(finalSearchResult.Nodes, 0.001 * finalSearchResult.Time);

#if MULTITHREAD_DEBUG
            _logger.Warn("End of multithread calculations, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
#endif

            // Final info command
            _engineWriter.TryWrite(finalSearchResult);

            // We always print best move, even in case of go ponder + stop, in which case IDEs are expected to ignore it
            _engineWriter.TryWrite(new BestMoveCommand(finalSearchResult));
        }
    }

    public void AdjustPosition(ReadOnlySpan<char> command)
    {
        _mainEngine.AdjustPosition(command);

        foreach (var engine in _extraEngines)
        {
            engine.AdjustPosition(command);
        }
    }

    public void StopSearching()
    {
        foreach (var engine in _extraEngines)
        {
            engine.StopSearching();
        }

        _mainEngine.StopSearching();
    }

    public void PonderHit()
    {
        _mainEngine.PonderHit();
    }

    public void NewGame()
    {
        var averageDepth = _mainEngine.AverageDepth;
        if (averageDepth > 0 && averageDepth < int.MaxValue)
        {
            _logger.Info("Average depth: {0}", averageDepth);
        }

        // Hash update
        if (_ttWrapper.Size == Configuration.EngineSettings.TranspositionTableSize)
        {
            _ttWrapper.Clear();
            _mainEngine.NewGame();
        }
        else
        {
            _logger.Info("Resizing TT ({CurrentSize} MB -> {NewSize} MB)", _ttWrapper.Size, Configuration.EngineSettings.TranspositionTableSize);

            _ttWrapper = new TranspositionTable();

            _mainEngine.FreeResources();
            _mainEngine = new Engine("1", _engineWriter, in _ttWrapper, warmup: true);
        }

        // Threads update
        if (_searchThreadsCount == Configuration.EngineSettings.Threads)
        {
            foreach (var engine in _extraEngines)
            {
                engine.NewGame();
            }
        }
        else
        {
            _logger.Info("Updating search threads count ({CurrentCount} threads -> {NewCount} threads)", _searchThreadsCount, Configuration.EngineSettings.Threads);
            _searchThreadsCount = Configuration.EngineSettings.Threads;

            AllocateExtraEngines();
        }

        ForceGCCollection();
    }

    public void Quit()
    {
        var averageDepth = _mainEngine.AverageDepth;
        if (averageDepth > 0 && averageDepth < int.MaxValue)
        {
            _logger.Info("Average depth: {0}", averageDepth);
        }
    }

    public async ValueTask RunBench(int depth)
    {
        var results = _mainEngine.Bench(depth);
        await _mainEngine.PrintBenchResults(results);
    }

    private void AllocateExtraEngines()
    {
        // _searchThreadsCount includes _mainEngine
        const int mainEngineOffset = 1;

        foreach (var engine in _extraEngines)
        {
            engine.FreeResources();
        }

        if (_searchThreadsCount > 1)
        {
            _extraEngines = new Engine[_searchThreadsCount - mainEngineOffset];

            for (int i = 0; i < _searchThreadsCount - mainEngineOffset; ++i)
            {
                _extraEngines[i] = new Engine($"{i + 2}", _engineWriter, in _ttWrapper, warmup: false);
            }
        }
        else
        {
            _extraEngines = [];
        }
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

    private static void ForceGCCollection()
    {
#pragma warning disable S1215 // "GC.Collect" should not be called
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
#pragma warning restore S1215 // "GC.Collect" should not be called
    }
}
