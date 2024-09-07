using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Lynx;

public sealed partial class Engine
{
    internal const int DefaultMaxDepth = 5;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly ChannelWriter<string> _engineWriter;

    private bool _isSearching;

    /// <summary>
    /// Ongoing search is a pondering one and there has been a ponder hit
    /// </summary>
    private bool _isPonderHit;

    /// <summary>
    /// Ongoing search is a pondering one
    /// </summary>
    private bool _isPondering;

    /// <summary>
    /// Stop requested during pondering
    /// </summary>
    private bool _stopRequested;

#pragma warning disable IDE0052, CS0414, S4487 // Remove unread private members
    private bool _isNewGameCommandSupported;
    private bool _isNewGameComing;
#pragma warning restore IDE0052, CS0414 // Remove unread private members

    private Move? _moveToPonder;

    public double AverageDepth { get; private set; }

    public RegisterCommand? Registration { get; set; }

    public Game Game { get; private set; }

    public bool PendingConfirmation { get; set; }

    private CancellationTokenSource _searchCancellationTokenSource;
    private CancellationTokenSource _absoluteSearchCancellationTokenSource;

    public Engine(ChannelWriter<string> engineWriter)
    {
        AverageDepth = 0;
        Game = new Game();
        _isNewGameComing = true;
        _searchCancellationTokenSource = new();
        _absoluteSearchCancellationTokenSource = new();
        _engineWriter = engineWriter;

        // Update ResetEngine() after any changes here
        _captureHistory = new int[12 * 64 * 12];
        _continuationHistory = new int[12 * 64 * 12 * 64 * EvaluationConstants.ContinuationHistoryPlyCount];
        _counterMoves = new int[12 * 64];

        _quietHistory = new int[12][];
        for (int i = 0; i < _quietHistory.Length; ++i)
        {
            _quietHistory[i] = new int[64];
        }

        _killerMoves = new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin][];
        for (int i = 0; i < Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin; ++i)
        {
            _killerMoves[i] = new Move[3];
        }

        InitializeTT();

#if !DEBUG
        // Temporary channel so that no output is generated
        _engineWriter = Channel.CreateUnbounded<string>(new UnboundedChannelOptions() { SingleReader = true, SingleWriter = false }).Writer;
        WarmupEngine();

        _engineWriter = engineWriter;
        ResetEngine();
#endif

#pragma warning disable S1215 // "GC.Collect" should not be called
        GC.Collect();
        GC.WaitForPendingFinalizers();
#pragma warning restore S1215 // "GC.Collect" should not be called
    }

#pragma warning disable S1144 // Unused private types or members should be removed - used in Release mode
    private void WarmupEngine()
    {
        _logger.Info("Warming up engine");
        var sw = Stopwatch.StartNew();

        InitializeStaticClasses();
        const string goWarmupCommand = "go depth 10";   // ~300 ms

        AdjustPosition(Constants.SuperLongPositionCommand);
        BestMove(new(goWarmupCommand));

        Bench(2);

        sw.Stop();
        _logger.Info("Warm-up finished in {0}ms", sw.ElapsedMilliseconds);
    }
#pragma warning restore S1144 // Unused private types or members should be removed

    private void ResetEngine()
    {
        InitializeTT(); // TODO SPRT clearing instead

        // Clear histories
        for (int i = 0; i < 12; ++i)
        {
            Array.Clear(_quietHistory[i]);
        }

        Array.Clear(_captureHistory);
        Array.Clear(_continuationHistory);
        Array.Clear(_counterMoves);

        // No need to clear killer move or pv table because they're cleared on every search (IDDFS)
    }

    internal void SetGame(Game game)
    {
        Game = game;
    }

    public void NewGame()
    {
        AverageDepth = 0;
        Game = new Game();
        _isNewGameComing = true;
        _isNewGameCommandSupported = true;
        _stopRequested = false;

        ResetEngine();

#pragma warning disable S1215 // "GC.Collect" should not be called
        GC.Collect();
        GC.WaitForPendingFinalizers();
#pragma warning restore S1215 // "GC.Collect" should not be called
    }

    [SkipLocalsInit]
    public void AdjustPosition(ReadOnlySpan<char> rawPositionCommand)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];

        Game = PositionCommand.ParseGame(rawPositionCommand, moves);
        _isNewGameComing = false;
        _stopRequested = false;
    }

    public void PonderHit()
    {
        _isPonderHit = true;
        StopSearching();
    }

    public SearchResult BestMove(GoCommand goCommand)
    {
        bool isPondering = goCommand.Ponder;

        _searchCancellationTokenSource = new();
        _absoluteSearchCancellationTokenSource = new();

        int maxDepth = -1;
        int hardLimitTimeBound;
        int softLimitTimeBound = int.MaxValue;

        double millisecondsLeft;
        int millisecondsIncrement;
        if (Game.CurrentPosition.Side == Side.White)
        {
            millisecondsLeft = goCommand.WhiteTime;
            millisecondsIncrement = goCommand.WhiteIncrement;
        }
        else
        {
            millisecondsLeft = goCommand.BlackTime;
            millisecondsIncrement = goCommand.BlackIncrement;
        }

        // Inspired by Alexandria: time overhead to avoid timing out in the engine-gui communication process
        const int engineGuiCommunicationTimeOverhead = 50;

        if (!isPondering)
        {
            if (goCommand.WhiteTime != 0 || goCommand.BlackTime != 0)  // Cutechess sometimes sends negative wtime/btime
            {
                const int minSearchTime = 50;

                var movesDivisor = goCommand.MovesToGo == 0
                    ? ExpectedMovesLeft(Game.PositionHashHistory.Count) * 3 / 2
                    : goCommand.MovesToGo;

                millisecondsLeft -= engineGuiCommunicationTimeOverhead;
                millisecondsLeft = Math.Clamp(millisecondsLeft, minSearchTime, int.MaxValue); // Avoiding 0/negative values

                hardLimitTimeBound = (int)(millisecondsLeft * Configuration.EngineSettings.HardTimeBoundMultiplier);

                var softLimitBase = (millisecondsLeft / movesDivisor) + (millisecondsIncrement * Configuration.EngineSettings.SoftTimeBaseIncrementMultiplier);
                softLimitTimeBound = Math.Min(hardLimitTimeBound, (int)(softLimitBase * Configuration.EngineSettings.SoftTimeBoundMultiplier));

                _logger.Info("Soft time bound: {0}s", 0.001 * softLimitTimeBound);
                _logger.Info("Hard time bound: {0}s", 0.001 * hardLimitTimeBound);

                _searchCancellationTokenSource.CancelAfter(hardLimitTimeBound);
            }
            else if (goCommand.MoveTime > 0)
            {
                softLimitTimeBound = hardLimitTimeBound = goCommand.MoveTime - engineGuiCommunicationTimeOverhead;
                _logger.Info("Time to move: {0}s", 0.001 * hardLimitTimeBound);

                _searchCancellationTokenSource.CancelAfter(hardLimitTimeBound);
            }
            else if (goCommand.Depth > 0)
            {
                maxDepth = goCommand.Depth > Constants.AbsoluteMaxDepth ? Constants.AbsoluteMaxDepth : goCommand.Depth;
            }
            else if (goCommand.Infinite)
            {
                maxDepth = Configuration.EngineSettings.MaxDepth;
                _logger.Info("Infinite search (depth {0})", maxDepth);
            }
            else
            {
                maxDepth = DefaultMaxDepth;
                _logger.Warn("Unexpected or unsupported go command");
            }
        }
        else
        {
            maxDepth = Configuration.EngineSettings.MaxDepth;
            _logger.Info("Pondering search (depth {0})", maxDepth);
        }

        SearchResult resultToReturn = IDDFS(maxDepth, softLimitTimeBound);
        //SearchResult resultToReturn = await SearchBestMove(maxDepth, decisionTime);

        Game.ResetCurrentPositionToBeforeSearchState();
        if (!isPondering
            && resultToReturn.BestMove != default
            && !_absoluteSearchCancellationTokenSource.IsCancellationRequested)
        {
            Game.MakeMove(resultToReturn.BestMove);
            Game.UpdateInitialPosition();
        }

        AverageDepth += (resultToReturn.Depth - AverageDepth) / Math.Ceiling(0.5 * Game.PositionHashHistory.Count);

        return resultToReturn;
    }

    /// <summary>
    /// Straight from expositor's author paper, https://expositor.dev/pdf/movetime.pdf
    /// </summary>
    /// <param name="plies_played"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ExpectedMovesLeft(int plies_played)
    {
        double p = (double)(plies_played);

        return (int)Math.Round(
            (59.3 + (72830.0 - p * 2330.0) / (p * p + p * 10.0 + 2644.0))   // Plies remaining
            / 2.0); // Full moves remaining
    }

    private async ValueTask<SearchResult> SearchBestMove(int maxDepth, int softLimitTimeBound)
    {
        if (!Configuration.EngineSettings.UseOnlineTablebaseInRootPositions || Game.CurrentPosition.CountPieces() > Configuration.EngineSettings.OnlineTablebaseMaxSupportedPieces)
        {
            return IDDFS(maxDepth, softLimitTimeBound)!;
        }

        // Local copy of positionHashHistory and HalfMovesWithoutCaptureOrPawnMove so that it doesn't interfere with regular search
        var currentHalfMovesWithoutCaptureOrPawnMove = Game.HalfMovesWithoutCaptureOrPawnMove;

        var tasks = new Task<SearchResult?>[] {
                // Other copies of positionHashHistory and HalfMovesWithoutCaptureOrPawnMove (same reason)
                ProbeOnlineTablebase(Game.CurrentPosition, new(Game.PositionHashHistory),  Game.HalfMovesWithoutCaptureOrPawnMove),
                Task.Run(()=>(SearchResult?)IDDFS(maxDepth, softLimitTimeBound))
            };

        var resultList = await Task.WhenAll(tasks);
        var searchResult = resultList[1];
        var tbResult = resultList[0];

        if (searchResult is not null)
        {
            _logger.Info("Search evaluation result - eval: {0}, mate: {1}, depth: {2}, pv: {3}",
                searchResult.Evaluation, searchResult.Mate, searchResult.Depth, string.Join(", ", searchResult.Moves.Select(m => m.UCIString())));
        }

        if (tbResult is not null)
        {
            _logger.Info("Online tb probing result - mate: {0}, moves: {1}",
                tbResult.Mate, string.Join(", ", tbResult.Moves.Select(m => m.UCIString())));

            if (searchResult?.Mate > 0 && searchResult.Mate <= tbResult.Mate && searchResult.Mate + currentHalfMovesWithoutCaptureOrPawnMove < 96)
            {
                _logger.Info("Relying on search result mate line due to dtm match and low enough dtz");
                ++searchResult.Depth;
                tbResult = null;
            }
        }

        return tbResult ?? searchResult!;
    }

    public void Search(GoCommand goCommand)
    {
        if (_isSearching)
        {
            _logger.Warn("Search already in progress");
        }
        _isSearching = true;

        Thread.CurrentThread.Priority = ThreadPriority.Highest;

        try
        {
            _isPondering = goCommand.Ponder;
            var searchResult = BestMove(goCommand);

            if (_isPondering)
            {
                // Using either field or local copy for the rest of the method, since goCommand.Ponder could change

                // Avoiding the scenario where search finishes early (i.e. mate detected, max depth reached) and results comes
                // before a potential ponderhit command
                // _absoluteSearchCancellationTokenSource.IsCancellationRequested isn't reliable because
                // if stop command is processed before go command, a new cancellation token sour
                SpinWait.SpinUntil(() => _isPonderHit || _stopRequested);

                if (_isPonderHit)
                {
                    _isPonderHit = false;
                    _isPondering = false;
                    goCommand.DisablePonder();

                    searchResult = BestMove(goCommand);
                }
            }

            // We print best move even in case of go ponder + stop, and IDEs are expected to ignore it
            _moveToPonder = searchResult.Moves.Count >= 2 ? searchResult.Moves[1] : null;
            _engineWriter.TryWrite(BestMoveCommand.BestMove(searchResult.BestMove, _moveToPonder));
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Error in {0} while calculating BestMove", nameof(Search));
        }
        finally
        {
            _isSearching = false;
            _isPondering = false;
            _stopRequested = false;
        }
    }

    public void StopSearching()
    {
        _stopRequested = true;
        _absoluteSearchCancellationTokenSource.Cancel();
    }

    private void InitializeTT()
    {
        (int ttLength, _ttMask) = TranspositionTableExtensions.CalculateLength(Configuration.EngineSettings.TranspositionTableSize);
        _tt = GC.AllocateArray<TranspositionTableElement>(ttLength, pinned: true);
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
