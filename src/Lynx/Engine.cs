using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Lynx;

public sealed partial class Engine : IDisposable
{
    internal const int DefaultMaxDepth = 5;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly ChannelWriter<object> _engineWriter;
    private readonly TranspositionTable _tt;

    private bool _disposedValue;

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

    public double AverageDepth { get; private set; }

    public Game Game { get; private set; }

    public bool PendingConfirmation { get; set; }

    private CancellationTokenSource _searchCancellationTokenSource;
    private CancellationTokenSource _absoluteSearchCancellationTokenSource;

    public Engine(ChannelWriter<object> engineWriter) : this(engineWriter, new()) { }

    public Engine(ChannelWriter<object> engineWriter, TranspositionTable tt)
    {
        AverageDepth = 0;
        Game = new Game(Constants.InitialPositionFEN);
        _isNewGameComing = true;
        _searchCancellationTokenSource = new();
        _absoluteSearchCancellationTokenSource = new();
        _engineWriter = engineWriter;
        _tt = tt;
        // Update ResetEngine() after any changes here

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

#if !DEBUG
        // Temporary channel so that no output is generated
        _engineWriter = Channel.CreateUnbounded<object>(new UnboundedChannelOptions() { SingleReader = true, SingleWriter = false }).Writer;
        WarmupEngine();

        _engineWriter = engineWriter;

        NewGame();
#endif
    }

#pragma warning disable S1144 // Unused private types or members should be removed - used in Release mode
    private void WarmupEngine()
    {
        _logger.Info("Warming up engine");
        var sw = Stopwatch.StartNew();

        const string goWarmupCommand = "go depth 10";   // ~300 ms
        var command = new GoCommand(goWarmupCommand);

        AdjustPosition(Constants.SuperLongPositionCommand);

        var searchConstrains = TimeManager.CalculateTimeManagement(Game, command);
        BestMove(command, in searchConstrains);

        Bench(2);

        sw.Stop();
        _logger.Info("Warm-up finished in {0}ms", sw.ElapsedMilliseconds);
    }
#pragma warning restore S1144 // Unused private types or members should be removed

    private void ResetEngine()
    {
        _tt.Reset();

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
        Game.FreeResources();
        Game = game;
    }

    public void NewGame()
    {
        AverageDepth = 0;
        Game.FreeResources();
        Game = new Game(Constants.InitialPositionFEN);
        _isNewGameComing = true;
        _isNewGameCommandSupported = true;
        _stopRequested = false;

        ResetEngine();
    }

    [SkipLocalsInit]
    public void AdjustPosition(ReadOnlySpan<char> rawPositionCommand)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        Game.FreeResources();
        Game = PositionCommand.ParseGame(rawPositionCommand, moves);
        _isNewGameComing = false;
        _stopRequested = false;
    }

    public void PonderHit()
    {
        _isPonderHit = true;
        StopSearching();
    }

    /// <summary>
    /// Uses <see cref="TimeManager.CalculateTimeManagement(Game, GoCommand)"/> internally
    /// </summary>
    public SearchResult BestMove(GoCommand goCommand)
    {
        var searchConstraints = TimeManager.CalculateTimeManagement(Game, goCommand);

        return BestMove(goCommand, in searchConstraints);
    }

    public SearchResult BestMove(GoCommand goCommand, in SearchConstraints searchConstrains)
    {
        _searchCancellationTokenSource = new();
        _absoluteSearchCancellationTokenSource = new();

        if (searchConstrains.HardLimitTimeBound != SearchConstraints.DefaultHardLimitTimeBound)
        {
            _searchCancellationTokenSource.CancelAfter(searchConstrains.HardLimitTimeBound);
        }

        SearchResult resultToReturn = IDDFS(searchConstrains.MaxDepth, searchConstrains.SoftLimitTimeBound);
        //SearchResult resultToReturn = await SearchBestMove(maxDepth, decisionTime);

        Game.ResetCurrentPositionToBeforeSearchState();
        if (!goCommand.Ponder
            && resultToReturn.BestMove != default
            && !_absoluteSearchCancellationTokenSource.IsCancellationRequested)
        {
            Game.MakeMove(resultToReturn.BestMove);
            Game.UpdateInitialPosition();
        }

        AverageDepth += (resultToReturn.Depth - AverageDepth) / Math.Ceiling(0.5 * Game.PositionHashHistoryLength());

        return resultToReturn;
    }

#pragma warning disable S1144 // Unused private types or members should be removed - wanna keep this around
    private async ValueTask<SearchResult> SearchBestMove(int maxDepth, int softLimitTimeBound)
#pragma warning restore S1144 // Unused private types or members should be removed
    {
        if (!Configuration.EngineSettings.UseOnlineTablebaseInRootPositions || Game.CurrentPosition.CountPieces() > Configuration.EngineSettings.OnlineTablebaseMaxSupportedPieces)
        {
            return IDDFS(maxDepth, softLimitTimeBound)!;
        }

        // Local copy of positionHashHistory and HalfMovesWithoutCaptureOrPawnMove so that it doesn't interfere with regular search
        var currentHalfMovesWithoutCaptureOrPawnMove = Game.HalfMovesWithoutCaptureOrPawnMove;

        var tasks = new Task<SearchResult?>[] {
                // Other copies of positionHashHistory and HalfMovesWithoutCaptureOrPawnMove (same reason)
                ProbeOnlineTablebase(Game.CurrentPosition, Game.CopyPositionHashHistory(),  Game.HalfMovesWithoutCaptureOrPawnMove),
                Task.Run(()=>(SearchResult?)IDDFS(maxDepth, softLimitTimeBound))
            };

        var resultList = await Task.WhenAll(tasks);
        var searchResult = resultList[1];
        var tbResult = resultList[0];

        if (searchResult is not null)
        {
            _logger.Info("Search evaluation result - score: {0}, mate: {1}, depth: {2}, pv: {3}",
                searchResult.Score, searchResult.Mate, searchResult.Depth, string.Join(", ", searchResult.Moves.Select(m => m.UCIString())));
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

    public SearchResult? Search(GoCommand goCommand, in SearchConstraints searchConstraints)
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
            var searchResult = BestMove(goCommand, in searchConstraints);

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

                    searchResult = BestMove(goCommand, in searchConstraints);
                }
            }

            return searchResult;
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "Error in {0} while calculating BestMove", nameof(Search));
            return null;
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

    public void FreeResources()
    {
        Game.FreeResources();

        _disposedValue = true;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                FreeResources();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
