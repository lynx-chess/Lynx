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

    internal const int MainEngineId = 1;
    private bool _isProcessingGoCommand;
    private bool _isPonderHit;

    private int _searchThreadsCount;
    private Engine _mainEngine;
    private Engine[] _extraEngines = [];
    private TranspositionTable _ttWrapper;

    private CancellationTokenSource _searchCancellationTokenSource;
    private CancellationTokenSource _absoluteSearchCancellationTokenSource;

    public Position CurrentPosition => _mainEngine.Game.CurrentPosition;

    public string FEN => _mainEngine.Game.FEN;

    private bool _firstRun;

    public Searcher(ChannelReader<string> uciReader, ChannelWriter<object> engineWriter)
    {
        InitializeStaticClasses();

        _firstRun = true;
        _uciReader = uciReader;
        _engineWriter = engineWriter;

        _ttWrapper = new TranspositionTable();
        _mainEngine = new Engine(MainEngineId, _engineWriter, in _ttWrapper);
        _absoluteSearchCancellationTokenSource = new();
        _searchCancellationTokenSource = new();

        _searchThreadsCount = Configuration.EngineSettings.Threads;

        _logger = LogManager.GetCurrentClassLogger();
        _logger.Info("Threads:\t{0}", _searchThreadsCount);

        AllocateExtraEngines();

#if !DEBUG
        Warmup();
#endif

        // Even if we didn't have Warmup(), this .Clear() zeroes the otherwise lazily zero-ed memory (due to using GC.AllocateArray instead of AllocateUninitializedArray)
        // It might help performance though due to preventing that zeroing from happenning during search
        // See https://stackoverflow.com/questions/2688466/why-mallocmemset-is-slower-than-calloc/2688522#2688522
        _ttWrapper.Clear();

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
        _isProcessingGoCommand = true;

        if (!_absoluteSearchCancellationTokenSource.TryReset())
        {
            _absoluteSearchCancellationTokenSource.Dispose();
            _absoluteSearchCancellationTokenSource = new();
        }

        if (!_searchCancellationTokenSource.TryReset())
        {
            _searchCancellationTokenSource.Dispose();
            _searchCancellationTokenSource = new();
        }

        if (_searchThreadsCount == 1)
        {
            SingleThreadedSearch(goCommand);
        }
        else
        {
            await MultiThreadedSearch(goCommand);
        }

        _isProcessingGoCommand = false;
    }

    private void SingleThreadedSearch(GoCommand goCommand)
    {
        var searchConstraints = TimeManager.CalculateTimeManagement(_mainEngine.Game, goCommand);
        var isPondering = Configuration.EngineSettings.IsPonder && goCommand.Ponder;

        if (!isPondering)
        {
            if (searchConstraints.HardLimitTimeBound != SearchConstraints.DefaultHardLimitTimeBound)
            {
                _searchCancellationTokenSource.CancelAfter(searchConstraints.HardLimitTimeBound);
            }

            var searchResult = _mainEngine.Search(in searchConstraints, isPondering: false, _absoluteSearchCancellationTokenSource.Token, _searchCancellationTokenSource.Token);

            if (searchResult is not null)
            {
                // Final info command
                _engineWriter.TryWrite(searchResult);

                // bestmove command
                _engineWriter.TryWrite(new BestMoveCommand(searchResult));
            }
        }
        else
        {
            // Pondering
            _logger.Debug("Pondering");

            SearchResult? searchResult = null;

            // This check takes care of early ponderhits that may have cancelled the ct
            // before it was reset in OnGoCommand and therefore stay undetected
            if (!_isPonderHit)
            {
                searchResult = _mainEngine.Search(in searchConstraints, isPondering: true, _absoluteSearchCancellationTokenSource.Token, CancellationToken.None);

                if (searchResult is not null)
                {
                    // Final info command
                    _engineWriter.TryWrite(searchResult);

                    // We don't print bestmove command when ponder + ponderhit though
                }
            }

            // Avoiding the scenario where search finishes early (i.e. mate detected, max depth reached) and results comes
            // before a potential ponderhit command
            SpinWait.SpinUntil(() => _isPonderHit || _absoluteSearchCancellationTokenSource.IsCancellationRequested);

            if (_isPonderHit)
            {
                // PonderHit cancelled the token from _absoluteSearchCancellationTokenSource
                _absoluteSearchCancellationTokenSource.Dispose();
                _absoluteSearchCancellationTokenSource = new();

                if (searchResult is null
                    || searchResult.Depth < Configuration.EngineSettings.PonderHitMinDepthToStopSearch
                    || searchConstraints.HardLimitTimeBound >= Configuration.EngineSettings.PonderHitMinTimeToContinueSearch)
                {
                    _logger.Debug("Ponder hit - restarting search now with time constraints");

                    if (searchConstraints.HardLimitTimeBound != SearchConstraints.DefaultHardLimitTimeBound)
                    {
                        _searchCancellationTokenSource.CancelAfter(searchConstraints.HardLimitTimeBound);
                    }

                    searchResult = _mainEngine.Search(in searchConstraints, isPondering: false, _absoluteSearchCancellationTokenSource.Token, _searchCancellationTokenSource.Token);
                }
                else
                {
                    _logger.Info("Ponder hit - settling for initial pondering search result due to low hard limit: " +
                        "{HardLimitTimeBound}ms (< {MinTime}ms), depth: {Depth} (>= {MaxDepth})",
                        searchConstraints.HardLimitTimeBound, Configuration.EngineSettings.PonderHitMinTimeToContinueSearch, searchResult.Depth, Configuration.EngineSettings.PonderHitMinDepthToStopSearch);
                }

                if (searchResult is not null)
                {
                    // Final info command
                    _engineWriter.TryWrite(searchResult);
                }

                _isPonderHit = false;
            }

            if (searchResult is not null)
            {
                // We print best move even in case of go ponder + stop, in which case IDEs are expected to ignore it
                _engineWriter.TryWrite(new BestMoveCommand(searchResult));
            }
        }
    }

    private async Task MultiThreadedSearch(GoCommand goCommand)
    {
        // Basic lazy SMP implementation
        // Extra engines run in "go infinite" mode and their purpose is to populate the TT
        // Not UCI output is produced by them nor their search results are taken into account
        var extraEnginesSearchConstraints = SearchConstraints.InfiniteSearchConstraint;

        var searchConstraints = TimeManager.CalculateTimeManagement(_mainEngine.Game, goCommand);
        var isPondering = Configuration.EngineSettings.IsPonder && goCommand.Ponder;

        if (!isPondering)
        {
            var finalSearchResult = MultithreadedSearch(searchConstraints, extraEnginesSearchConstraints);
        }
        else
        {
#if MULTITHREAD_DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var lastElapsed = sw.ElapsedMilliseconds;
#endif

            // Pondering
            _logger.Debug("Pondering");

            SearchResult? finalSearchResult = null;

            // This check takes care of early ponderhits that may have cancelled the ct
            // before it was reset in OnGoCommand and therefore stay undetected
            if (!_isPonderHit)
            {
                var tasks = _extraEngines
                    .Select(engine =>
                        Task.Run(() => engine.Search(in extraEnginesSearchConstraints, isPondering: true, _absoluteSearchCancellationTokenSource.Token, CancellationToken.None)))
                    .ToArray();

#if MULTITHREAD_DEBUG
                _logger.Info("[Pondering] End of extra searches prep, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
                lastElapsed = sw.ElapsedMilliseconds;
#endif

                finalSearchResult = _mainEngine.Search(in searchConstraints, isPondering: true, _absoluteSearchCancellationTokenSource.Token, CancellationToken.None);

#if MULTITHREAD_DEBUG
                _logger.Info("[Pondering] End of main search, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
                lastElapsed = sw.ElapsedMilliseconds;
#endif

                await _absoluteSearchCancellationTokenSource.CancelAsync();

#if MULTITHREAD_DEBUG
                _logger.Info("[Pondering] End of extra searches, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
#endif

                if (finalSearchResult is not null)
                {
                    // We wait just for the node count, so there's room for improvement here with thread voting
                    // and other strategies that take other thread results into account
                    await foreach (var extraResult in Task.WhenEach(tasks))
                    {
                        finalSearchResult.Nodes += (await extraResult)?.Nodes ?? 0;
                    }

                    finalSearchResult.NodesPerSecond = Utils.CalculateNps(finalSearchResult.Nodes, 0.001 * finalSearchResult.Time);

#if MULTITHREAD_DEBUG
                    _logger.Info("[Pondering] End of multithread calculations, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
#endif

                    // Final info command
                    _engineWriter.TryWrite(finalSearchResult);

                    // We don't print bestmove command when ponder + ponderhit though
                }
            }

            // Avoiding the scenario where search finishes early (i.e. mate detected, max depth reached) and results comes
            // before a potential ponderhit command
            SpinWait.SpinUntil(() => _isPonderHit || _absoluteSearchCancellationTokenSource.IsCancellationRequested);

            if (_isPonderHit)
            {
                // PonderHit cancelled the token from _absoluteSearchCancellationTokenSource
                _absoluteSearchCancellationTokenSource.Dispose();
                _absoluteSearchCancellationTokenSource = new();

                if (finalSearchResult is null
                    || finalSearchResult.Depth < Configuration.EngineSettings.PonderHitMinDepthToStopSearch
                    || searchConstraints.HardLimitTimeBound >= Configuration.EngineSettings.PonderHitMinTimeToContinueSearch)
                {
                    _logger.Debug("Ponder hit - restarting search now with time constraints");

                    if (searchConstraints.HardLimitTimeBound != SearchConstraints.DefaultHardLimitTimeBound)
                    {
                        _searchCancellationTokenSource.CancelAfter(searchConstraints.HardLimitTimeBound);
                    }

                    finalSearchResult = await MultithreadedSearch(searchConstraints, extraEnginesSearchConstraints);
                }
                else
                {
                    _logger.Info("Ponder hit - settling for initial pondering search result due to low hard limit: " +
                        "{HardLimitTimeBound}ms (< {MinTime}ms), depth: {Depth} (>= {MaxDepth})",
                        searchConstraints.HardLimitTimeBound, Configuration.EngineSettings.PonderHitMinTimeToContinueSearch, finalSearchResult.Depth, Configuration.EngineSettings.PonderHitMinDepthToStopSearch);

                    // Final info command
                    _engineWriter.TryWrite(finalSearchResult);

                    // bestmove command
                    _engineWriter.TryWrite(new BestMoveCommand(finalSearchResult));
                }

                _isPonderHit = false;
            }
            else
            {
                if (finalSearchResult is not null)
                {
                    // We print best move even in case of go ponder + stop, in which case IDEs are expected to ignore it
                    _engineWriter.TryWrite(new BestMoveCommand(finalSearchResult));
                }
            }
        }
    }

    private async Task<SearchResult?> MultithreadedSearch(SearchConstraints searchConstraints, SearchConstraints extraEnginesSearchConstraints)
    {
#if MULTITHREAD_DEBUG
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var lastElapsed = sw.ElapsedMilliseconds;
#endif

        if (searchConstraints.HardLimitTimeBound != SearchConstraints.DefaultHardLimitTimeBound)
        {
            _searchCancellationTokenSource.CancelAfter(searchConstraints.HardLimitTimeBound);
        }

        SearchResult? finalSearchResult = null;

        var tasks = _extraEngines
            .Select(engine =>
                Task.Run(() => engine.Search(in extraEnginesSearchConstraints, isPondering: false, _absoluteSearchCancellationTokenSource.Token, CancellationToken.None)))
            .ToArray();

#if MULTITHREAD_DEBUG
        _logger.Info("[MT] End of extra searches prep, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
        lastElapsed = sw.ElapsedMilliseconds;
#endif

        finalSearchResult = _mainEngine.Search(in searchConstraints, isPondering: false, _absoluteSearchCancellationTokenSource.Token, _searchCancellationTokenSource.Token);

#if MULTITHREAD_DEBUG
        _logger.Info("[MT] End of main search, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
        lastElapsed = sw.ElapsedMilliseconds;
#endif

        await _absoluteSearchCancellationTokenSource.CancelAsync();

#if MULTITHREAD_DEBUG
        _logger.Info("[MT] End of extra searches, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
        lastElapsed = sw.ElapsedMilliseconds;
#endif

        var totalNodes = finalSearchResult?.Nodes ?? 0;
        var finalTime = finalSearchResult?.Time ?? 0;

        await foreach (var task in Task.WhenEach(tasks))
        {
            var extraResult = await task;

            if (extraResult is not null)
            {
                totalNodes += extraResult.Nodes;

                if (finalSearchResult is null)
                {
                    finalSearchResult = extraResult;
                    finalTime = extraResult.Time;
                    continue;
                }

                // Thread voting, original impl sligtly corrected based on by Heimdall's (based on Berserk's)
                if (extraResult.BestMove != default)
                {
#if MULTITHREAD_DEBUG
                    var previousEngineId = finalSearchResult.EngineId;
                    var previousDepth = finalSearchResult.Depth;
                    var previousScore = finalSearchResult.Score;
                    var previousMate = finalSearchResult.Mate;
                    var previousBestMove = finalSearchResult.BestMove;
#endif

                    finalSearchResult = finalSearchResult.Mate switch
                    {
                        0                                                                       // No mate detected in main thread:
                            when                                                                //      Extra thread:
                                extraResult.Mate > 0                                            //          Mate
                                    || extraResult.Depth > finalSearchResult.Depth              //          ||  Higher depth
                                    || (extraResult.Depth == finalSearchResult.Depth            //          ||  Same depth, better score
                                        && extraResult.Score > finalSearchResult.Score)

                                => extraResult,
                        > 0                                                                     // Mating in main thread:
                            when                                                                //      Extra thread:
                                extraResult.Mate > 0                                            //          Still mating
                                    && (extraResult.Mate < finalSearchResult.Mate               //          &&  [But faster (shorter mate)
                                        || (extraResult.Mate == finalSearchResult.Mate          //              || Same mating distance, but higher depth]
                                            && extraResult.Depth > finalSearchResult.Depth))

                                => extraResult,

                        < 0                                                                     // Mated in main thread:
                            when                                                                //      Extra thread:
                                extraResult.Depth > finalSearchResult.Depth                     //          Higher depth
                                    || (extraResult.Depth == finalSearchResult.Depth            //          || Same depth
                                        && (extraResult.Mate >= 0                               //              && [But mating or at least not mated any more
                                            || extraResult.Mate < finalSearchResult.Mate))      //                  || Still mated, but longer mate]

                                => extraResult,

                        _ => finalSearchResult
                    };

#if MULTITHREAD_DEBUG
                    if (previousEngineId != finalSearchResult.EngineId)
                    {
                        if (_logger.IsInfoEnabled)
                        {
                            if (previousBestMove != finalSearchResult.BestMove)
                            {
                                _logger.Info("[MT] Thread voting: #{EngineId1} ({BestMove1}, Depth {Depth1}, cp {Score1}, mate {Mate1}) -> #{EngineId2} ({BestMove2}, (Depth {Depth2}, cp {Score2}, mate {Mate2}) | {FEN}",
                                    previousEngineId, previousBestMove.UCIStringMemoized(), previousDepth, previousScore, previousMate.ToString(Constants.NumberWithSignFormat),
                                    finalSearchResult.EngineId, finalSearchResult.BestMove.UCIStringMemoized(), finalSearchResult.Depth, finalSearchResult.Score, finalSearchResult.Mate.ToString(Constants.NumberWithSignFormat),
                                    _mainEngine.Game.PositionBeforeLastSearch.FEN(_mainEngine.Game.HalfMovesWithoutCaptureOrPawnMove));
                            }
                        }
                        else if (_logger.IsDebugEnabled)
                        {
                            _logger.Debug("[MT] Thread voting: #{EngineId1} ({BestMove1}, Depth {Depth1}, cp {Score1}, mate {Mate1}) -> #{EngineId2} ({BestMove2}, (Depth {Depth2}, cp {Score2}, mate {Mate2}) | {FEN}",
                                previousEngineId, previousBestMove.UCIStringMemoized(), previousDepth, previousScore, previousMate.ToString(Constants.NumberWithSignFormat),
                                finalSearchResult.EngineId, finalSearchResult.BestMove.UCIStringMemoized(), finalSearchResult.Depth, finalSearchResult.Score, finalSearchResult.Mate.ToString(Constants.NumberWithSignFormat),
                                _mainEngine.Game.PositionBeforeLastSearch.FEN(_mainEngine.Game.HalfMovesWithoutCaptureOrPawnMove));
                        }
                    }
#endif
                }
            }
        }

        if (finalSearchResult is not null)
        {
            finalSearchResult.Nodes = totalNodes;
            finalSearchResult.Time = finalTime;

            finalSearchResult.NodesPerSecond = Utils.CalculateNps(finalSearchResult.Nodes, 0.001 * finalSearchResult.Time);

#if MULTITHREAD_DEBUG
            _logger.Info("[MT] End of multithread calculations, {0} ms", sw.ElapsedMilliseconds - lastElapsed);
#endif

            // Final info command
            _engineWriter.TryWrite(finalSearchResult);

            // bestmove command
            _engineWriter.TryWrite(new BestMoveCommand(finalSearchResult));
        }

        return finalSearchResult;
    }

    public void AdjustPosition(ReadOnlySpan<char> command)
    {
        // Can't update MainEngine.Game until previous search is completed
        // Some GUIs wait until a bestmove is sent before sending a new position + go command (cutechess)
        // but some others don't (WinBoard)
        SpinWait.SpinUntil(() => !_isProcessingGoCommand);

        _mainEngine.AdjustPosition(command);

        foreach (var engine in _extraEngines)
        {
            engine.AdjustPosition(command);
        }
    }

    public async Task StopSearching()
    {
        await _absoluteSearchCancellationTokenSource.CancelAsync();
    }

    public async Task PonderHit()
    {
        _isPonderHit = true;
        await _absoluteSearchCancellationTokenSource.CancelAsync();
    }

    public void NewGame()
    {
        var sw = Stopwatch.StartNew();

        var averageDepth = _mainEngine.AverageDepth;
        if (averageDepth > 0 && averageDepth < int.MaxValue)
        {
            _logger.Info("Average depth: {0}", averageDepth);
        }

        // Threads update - before hash update to take advantage of multithreaded TT initialization (clearing)
        // if .Clear() is ever moved to TranspositionTable constructor.
        var threadsUpdated = UpdateThreads();
        if (threadsUpdated)
        {
            _logger.Warn("Unexpected threads update - should have happened on 'setoption'");
        }

        // Hash update - after hash update to potentially take advantage of multithreaded TT
        // initialization (clearing/zeroing), if .Clear() is ever moved to TranspositionTable constructor.
        var hashUpdated = UpdateHash();
        if (hashUpdated)
        {
            _logger.Warn("Unexpected hash update - should have happened on 'setoption'\");");
        }

        // We don't need to reset the main engine in case of hash update
        // because it was alredy reset there, but whetever
        _mainEngine.NewGame();

        // We don't need to reset the extra engines in case of hash or threads update
        // because they were alredy reset there, but whetever
        foreach (var engine in _extraEngines)
        {
            engine.NewGame();
        }

        // During the first run, TT is cleared at the end of the constructor
        if (!_firstRun && !hashUpdated)
        {
            _ttWrapper.Clear();
        }
        _firstRun = false;

        sw.Stop();
        _logger.Info("ucinewgame duration: {Time}ms", sw.ElapsedMilliseconds);

        ForceGCCollection();
    }

    public bool UpdateThreads()
    {
        if (_searchThreadsCount != Configuration.EngineSettings.Threads)
        {
            _logger.Info("Updating search threads count ({CurrentCount} threads -> {NewCount} threads)", _searchThreadsCount, Configuration.EngineSettings.Threads);

            // Before invoking AllocateExtraEngines
            _searchThreadsCount = Configuration.EngineSettings.Threads;

            AllocateExtraEngines();

            return true;
        }

        return false;
    }

    public bool UpdateHash()
    {
        if (_ttWrapper.Size != Configuration.EngineSettings.TranspositionTableSize)
        {
            _logger.Info("Resizing TT ({CurrentSize} MB -> {NewSize} MB)", _ttWrapper.Size, Configuration.EngineSettings.TranspositionTableSize);

            _ttWrapper = new TranspositionTable();

            // This .Clear() zeroes the otherwise lazily zero-ed memory (due to using GC.AllocateArray instead of AllocateUninitializedArray), but isn't functional
            // It might impact performance though, due to preventing that zeroing from happenning during search
            // See https://stackoverflow.com/questions/2688466/why-mallocmemset-is-slower-than-calloc/2688522#2688522
            _ttWrapper.Clear();

            _mainEngine.FreeResources();
            _mainEngine = new Engine(MainEngineId, _engineWriter, in _ttWrapper);

            // We need extra engines to know about the nwe TT
            AllocateExtraEngines();

            return true;
        }

        return false;
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
        using var engine = new Engine(-1, SilentChannelWriter<object>.Instance, in _ttWrapper);
        var results = engine.Bench(depth);

        // Can't use engine, or results won't be printed
        await _mainEngine.PrintBenchResults(results);
    }

    public async ValueTask RunVerboseBench(int depth)
    {
        using var engine = new Engine(-1, _engineWriter, in _ttWrapper);
        var results = engine.Bench(depth);

        await engine.PrintBenchResults(results);
    }

    /// <summary>
    /// Removes existing <see cref="_extraEngines"/> and allocates new ones baed on <see cref="_searchThreadsCount"/>
    /// </summary>
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
                _extraEngines[i] = new Engine(i + 2,
#if MULTITHREAD_DEBUG
                _logger.IsDebugEnabled
                    ? _engineWriter
                    : SilentChannelWriter<object>.Instance,
#else
                    SilentChannelWriter<object>.Instance,
#endif
                    in _ttWrapper);
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
        _ = Masks.IsolatedPawnMasks;
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

#pragma warning disable S1144, RCS1213 // Unused private types or members should be removed - used in Release mode
    private void Warmup()
    {
        _logger.Debug("Warming-up engine");
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var warmupCount = Math.Min(8, _extraEngines.Length + 1);

        Parallel.For(0, warmupCount, i =>
        {
            var silentEngineWriter = Channel.CreateUnbounded<object>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false }).Writer;
            var engine = new Engine(-i, silentEngineWriter, in _ttWrapper);

            engine.Warmup();
        });

        _logger.Info("Warm-up time:\t{0} ms", sw.ElapsedMilliseconds);
    }
#pragma warning restore S1144, RCS1213 // Unused private types or members should be removed
}
