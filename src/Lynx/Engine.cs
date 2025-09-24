using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Lynx;

public sealed partial class Engine : IDisposable
{
    internal const int DefaultMaxDepth = 5;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly int _id;
    private readonly ChannelWriter<object> _engineWriter;
    private readonly TranspositionTable _tt;
    private SearchConstraints _searchConstraints;

    private bool _disposedValue;

    private bool _isSearching;

    /// <summary>
    /// Number of ways a search method might be invoked with the same ply:
    /// Regular NegaMax, SE verification NegaMax, regular QSearch, SE verification QSearch
    /// </summary>
    const int _poolsPerPly = 4;

    /// <summary>
    /// 12 x <see cref="_poolsPerPly"/> x (<see cref="Configuration.EngineSettings.MaxDepth"/> + <see cref="Constants.ArrayDepthMargin"/>)
    /// </summary>
    private readonly BitBoard[] _attacksPool;

    /// <summary>
    /// 2 x <see cref="_poolsPerPly"/> x (<see cref="Configuration.EngineSettings.MaxDepth"/> + <see cref="Constants.ArrayDepthMargin"/>)
    /// </summary>
    private readonly BitBoard[] _attacksBySidePool;

    /// <summary>
    /// <see cref="Constants.MaxNumberOfPseudolegalMovesInAPosition"/> x <see cref="_poolsPerPly"/> x (<see cref="Configuration.EngineSettings.MaxDepth"/> + <see cref="Constants.ArrayDepthMargin"/>)
    /// </summary>
    private readonly Move[] _movesPool;

    /// <summary>
    /// <see cref="Constants.MaxNumberOfPseudolegalMovesInAPosition"/> x <see cref="_poolsPerPly"/> x (<see cref="Configuration.EngineSettings.MaxDepth"/> + <see cref="Constants.ArrayDepthMargin"/>)
    /// </summary>
    private readonly int[] _moveScoresPool;

    /// <summary>
    /// <see cref="Constants.MaxNumberOfPseudolegalMovesInAPosition"/> x <see cref="_poolsPerPly"/> x (<see cref="Configuration.EngineSettings.MaxDepth"/> + <see cref="Constants.ArrayDepthMargin"/>)
    /// </summary>
    private readonly Move[] _visitedMovesPool;

    public double AverageDepth { get; private set; }

#pragma warning disable IDISP008 // Don't assign member with injected and created disposables - caused by SetGame, internal-only for tests
    public Game Game { get; private set; }
#pragma warning restore IDISP008 // Don't assign member with injected and created disposables

    public bool PendingConfirmation { get; set; }

    private bool IsMainEngine => _id == Searcher.MainEngineId;

#pragma warning disable EPS09 // Pass an argument for an 'in' parameter explicitly
    public Engine(ChannelWriter<object> engineWriter) : this(0, engineWriter, new()) { }
#pragma warning restore EPS09 // Pass an argument for an 'in' parameter explicitly

#pragma warning disable RCS1163 // Unused parameter - used in Release mode
    public Engine(int id, ChannelWriter<object> engineWriter, in TranspositionTable tt)
#pragma warning restore RCS1163 // Unused parameter
    {
        _id = id;
        _engineWriter = engineWriter;
        _tt = tt;

        AverageDepth = 0;
        Game = new Game(Constants.InitialPositionFEN);

        // Update ResetEngine() after any changes here
        _moveNodeCount = new ulong[12][];
        for (int i = 0; i < _moveNodeCount.Length; ++i)
        {
            _moveNodeCount[i] = new ulong[64];
        }

        var maxDepth = Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin;

        int attacksSize = _poolsPerPly * maxDepth * 12;
        int attacksBySideSize = _poolsPerPly * maxDepth * 2;
        int movesSize = _poolsPerPly * maxDepth * Constants.MaxNumberOfPseudolegalMovesInAPosition;

        _attacksPool = new BitBoard[attacksSize];
        _attacksBySidePool = new BitBoard[attacksBySideSize];
        _movesPool = new Move[movesSize];
        _moveScoresPool = new int[movesSize];
        _visitedMovesPool = new Move[movesSize];

        _logger.Info("Engine {0} initialized", _id);
    }

    public void Warmup()
    {
        AdjustPosition(Configuration.EngineSettings.IsChess960 ? Constants.SuperLongPositionCommand_DFRC : Constants.SuperLongPositionCommand);

        const string goWarmupCommand = "go depth 10";   // ~300 ms
        var command = new GoCommand(goWarmupCommand);

        BestMove(command);

        Bench(2);
    }

    private void ResetEngine()
    {
        // Clear histories
        for (int i = 0; i < 12; ++i)
        {
            Array.Clear(_moveNodeCount[i]);
        }

        Array.Clear(_quietHistory);
        Array.Clear(_captureHistory);
        Array.Clear(_continuationHistory);
        Array.Clear(_counterMoves);

        Array.Clear(_pawnEvalTable);

        Array.Clear(_pawnCorrHistory);
        Array.Clear(_nonPawnCorrHistory);
        Array.Clear(_minorCorrHistory);
        Array.Clear(_majorCorrHistory);

        // No need to clear killer move or pv table because they're cleared on every search (IDDFS)
        // Same happens for pools
    }

    [Obsolete("Test only")]
    internal void SetGame(Game game)
    {
        Game.Dispose();
        Game = game;
    }

    public void NewGame()
    {
        AverageDepth = 0;
        Game.Dispose();
        Game = new Game(Constants.InitialPositionFEN);

        ResetEngine();
    }

    [SkipLocalsInit]
    public void AdjustPosition(ReadOnlySpan<char> rawPositionCommand)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Game.Dispose();
        Game = PositionCommand.ParseGame(rawPositionCommand, moves);
    }

    /// <summary>
    /// Uses <see cref="TimeManager.CalculateTimeManagement(Game, GoCommand)"/> internally
    /// </summary>
    public SearchResult BestMove(GoCommand goCommand)
    {
        var searchConstraints = TimeManager.CalculateTimeManagement(Game, goCommand);

        return BestMove(in searchConstraints, isPondering: false, CancellationToken.None, CancellationToken.None);
    }

    public SearchResult BestMove(in SearchConstraints searchConstrains, bool isPondering, CancellationToken absoluteSearchCancellationToken, CancellationToken searchCancellationToken)
    {
        _searchConstraints = searchConstrains;

        using var jointCts = CancellationTokenSource.CreateLinkedTokenSource(absoluteSearchCancellationToken, searchCancellationToken);

        SearchResult resultToReturn = IDDFS(isPondering, jointCts.Token);
        //SearchResult resultToReturn = await SearchBestMove(maxDepth, decisionTime);

        Game.ResetCurrentPositionToBeforeSearchState();
        if (!isPondering
            && resultToReturn.BestMove != default
            && !absoluteSearchCancellationToken.IsCancellationRequested)    // Extra engines position never gets updated, but even if we tried to do it here, we can never guarantee to update it with the overall chosen move
        {
            Game.MakeMove(resultToReturn.BestMove);
            Game.UpdateInitialPosition();
        }

        AverageDepth += (resultToReturn.Depth - AverageDepth) / Math.Ceiling(0.5 * Game.PositionHashHistoryLength());

        return resultToReturn;
    }

#pragma warning disable S1144 // Unused private types or members should be removed - wanna keep this around
    private async ValueTask<SearchResult> SearchBestMove(bool isPondering, CancellationToken absoluteSearchCancellationToken, CancellationToken searchCancellationToken)
#pragma warning restore S1144 // Unused private types or members should be removed
    {
        using var jointCts = CancellationTokenSource.CreateLinkedTokenSource(absoluteSearchCancellationToken, searchCancellationToken);

        if (!Configuration.EngineSettings.UseOnlineTablebaseInRootPositions || Game.CurrentPosition.CountPieces() > Configuration.EngineSettings.OnlineTablebaseMaxSupportedPieces)
        {
            return IDDFS(isPondering, jointCts.Token)!;
        }

        // Local copy of positionHashHistory and HalfMovesWithoutCaptureOrPawnMove so that it doesn't interfere with regular search
        var currentHalfMovesWithoutCaptureOrPawnMove = Game.HalfMovesWithoutCaptureOrPawnMove;

        var cancellationToken = jointCts.Token;
        var tasks = new Task<SearchResult?>[] {
                // Other copies of positionHashHistory and HalfMovesWithoutCaptureOrPawnMove (same reason)
                ProbeOnlineTablebase(Game.CurrentPosition, Game.CopyPositionHashHistory(),  Game.HalfMovesWithoutCaptureOrPawnMove, cancellationToken),
                Task.Run(()=>(SearchResult?)IDDFS(isPondering, cancellationToken))
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

    public SearchResult? Search(in SearchConstraints searchConstraints, bool isPondering, CancellationToken absoluteSearchCancellationToken, CancellationToken searchCancellationToken)
    {
#if MULTITHREAD_DEBUG
        _logger.Debug("[#{EngineId}] Starting search using thread {ThreadId}", _id, Environment.CurrentManagedThreadId);
#endif

        if (_isSearching)
        {
            _logger.Warn("Search already in progress");
        }
        _isSearching = true;

        Thread.CurrentThread.Priority = ThreadPriority.Highest;

        try
        {
            return BestMove(in searchConstraints, isPondering, absoluteSearchCancellationToken, searchCancellationToken);
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "[#{EngineId}] Error in {Method} for position {Position}", _id, nameof(Search), Game.CurrentPosition.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));
            return null;
        }
        finally
        {
            _isSearching = false;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Game.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);

#pragma warning disable S3234, IDISP024 // "GC.SuppressFinalize" should not be invoked for types without destructors - https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        GC.SuppressFinalize(this);
#pragma warning restore S3234, IDISP024 // "GC.SuppressFinalize" should not be invoked for types without destructors
    }
}
