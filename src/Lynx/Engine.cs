using Lynx.Model;
using Lynx.UCI.Commands.Engine;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Diagnostics;
using System.Threading.Channels;

namespace Lynx;

public sealed partial class Engine
{
    internal const int DefaultMaxDepth = 5;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly ChannelWriter<string> _engineWriter;

#pragma warning disable IDE0052, CS0414, S4487 // Remove unread private members
    private bool _isNewGameCommandSupported;
    private bool _isNewGameComing;
    private bool _isPondering;
#pragma warning restore IDE0052, CS0414 // Remove unread private members

    private Move? _moveToPonder;
    public double AverageDepth { get; private set; }

    public RegisterCommand? Registration { get; set; }

    public Game Game { get; private set; }

    public bool IsSearching { get; private set; }

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
        _quietHistory = new int[12][];
        for (int i = 0; i < _quietHistory.Length; ++i)
        {
            _quietHistory[i] = new int[64];
        }

        _captureHistory = new int[12][][];
        for (int i = 0; i < 12; ++i)
        {
            _captureHistory[i] = new int[64][];
            for (var j = 0; j < 64; ++j)
            {
                _captureHistory[i][j] = new int[12];
            }
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
            for (var j = 0; j < 64; ++j)
            {
                Array.Clear(_captureHistory[i][j]);
            }
        }

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

        ResetEngine();

#pragma warning disable S1215 // "GC.Collect" should not be called
        GC.Collect();
        GC.WaitForPendingFinalizers();
#pragma warning restore S1215 // "GC.Collect" should not be called
    }

    public void AdjustPosition(ReadOnlySpan<char> rawPositionCommand)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];

        Game = PositionCommand.ParseGame(rawPositionCommand, moves);
        _isNewGameComing = false;
    }

    public void PonderHit()
    {
        Game.MakeMove(_moveToPonder!.Value);   // TODO ponder: do we also receive the position command? If so, remove this line
        _isPondering = false;
    }

    public SearchResult BestMove(GoCommand goCommand)
    {
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

        if (goCommand.WhiteTime != 0 || goCommand.BlackTime != 0)  // Cutechess sometimes sends negative wtime/btime
        {
            const int minSearchTime = 50;

            var movesDivisor = goCommand.MovesToGo == 0
                ? Configuration.EngineSettings.DefaultMovesToGo
                : goCommand.MovesToGo;

            millisecondsLeft -= engineGuiCommunicationTimeOverhead;
            millisecondsLeft = Math.Clamp(millisecondsLeft, minSearchTime, int.MaxValue); // Avoiding 0/negative values

            hardLimitTimeBound = (int)(millisecondsLeft * Configuration.EngineSettings.HardTimeBoundMultiplier);

            var softLimitBase = (millisecondsLeft / movesDivisor) + (millisecondsIncrement * Configuration.EngineSettings.SoftTimeBaseIncrementMultiplier);
            softLimitTimeBound = Math.Min(hardLimitTimeBound, (int)(softLimitBase * Configuration.EngineSettings.SoftTimeBoundMultiplier));

            _logger.Info("Soft time bound: {0}s", 0.001 * softLimitTimeBound);
            _logger.Info("Hard time bound: {0}s", 0.001 * hardLimitTimeBound);

            //return default!;

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

        SearchResult resultToReturn = IDDFS(maxDepth, softLimitTimeBound);
        //SearchResult resultToReturn = await SearchBestMove(maxDepth, decisionTime);

        Game.ResetCurrentPositionToBeforeSearchState();
        if (resultToReturn.BestMove != default && !_absoluteSearchCancellationTokenSource.IsCancellationRequested)
        {
            Game.MakeMove(resultToReturn.BestMove);
            Game.UpdateInitialPosition();
        }

        AverageDepth += (resultToReturn.Depth - AverageDepth) / Math.Ceiling(0.5 * Game.PositionHashHistory.Count);

        return resultToReturn;
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
                searchResult.Evaluation, searchResult.Mate, searchResult.Depth, string.Join(", ", searchResult.Moves.Select(m => m.ToMoveString())));
        }

        if (tbResult is not null)
        {
            _logger.Info("Online tb probing result - mate: {0}, moves: {1}",
                tbResult.Mate, string.Join(", ", tbResult.Moves.Select(m => m.ToMoveString())));

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
        if (IsSearching)
        {
            _logger.Warn("Search already in progress");
        }

        _isPondering = goCommand.Ponder;
        IsSearching = true;

        Thread.CurrentThread.Priority = ThreadPriority.Highest;

        try
        {
            var searchResult = BestMove(goCommand);
            _moveToPonder = searchResult.Moves.Count >= 2 ? searchResult.Moves[1] : null;
            _engineWriter.TryWrite(BestMoveCommand.BestMove(searchResult.BestMove, _moveToPonder));
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Error in {0} while calculating BestMove", nameof(Search));
        }
        finally
        {
            IsSearching = false;
        }
        // TODO ponder: if ponder, continue with PonderAction, which is searching indefinitely for a move
    }

    public void StopSearching()
    {
        _absoluteSearchCancellationTokenSource.Cancel();
        IsSearching = false;
    }

    private void InitializeTT()
    {
        if (Configuration.EngineSettings.TranspositionTableEnabled)
        {
            (int ttLength, _ttMask) = TranspositionTableExtensions.CalculateLength(Configuration.EngineSettings.TranspositionTableSize);
            _tt = GC.AllocateArray<TranspositionTableElement>(ttLength, pinned: true);
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
}
