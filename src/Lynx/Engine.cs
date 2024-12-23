﻿using Lynx.Model;
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
    private readonly string _id;
    private readonly ChannelWriter<object> _engineWriter;
    private readonly TranspositionTable _tt;
    private SearchConstraints _searchConstraints;

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

    public double AverageDepth { get; private set; }

    public Game Game { get; private set; }

    public bool PendingConfirmation { get; set; }

    private CancellationToken _searchCancellationToken = CancellationToken.None;
    private CancellationToken _absoluteSearchCancellationToken = CancellationToken.None;
    public Engine(ChannelWriter<object> engineWriter) : this("0", engineWriter, new()) { }

#pragma warning disable RCS1163 // Unused parameter - used in Release mode
    public Engine(string id, ChannelWriter<object> engineWriter, in TranspositionTable tt, bool warmup = false)
#pragma warning restore RCS1163 // Unused parameter
    {
        _id = id;
        _engineWriter = engineWriter;
        _tt = tt;

        AverageDepth = 0;
        Game = new Game(Constants.InitialPositionFEN);

        // Update ResetEngine() after any changes here
        _quietHistory = new int[12][];
        _moveNodeCount = new ulong[12][];
        for (int i = 0; i < _quietHistory.Length; ++i)
        {
            _quietHistory[i] = new int[64];
            _moveNodeCount[i] = new ulong[64];
        }

#if !DEBUG
        if (warmup)
        {
            // Temporary channel so that no output is generated
            _engineWriter = Channel.CreateUnbounded<object>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false }).Writer;
            WarmupEngine();

            _engineWriter = engineWriter;

            NewGame();
        }
#endif

        _logger.Info("Engine {0} initialized", _id);
    }

    private bool IsMainEngine() => _id == Searcher.MainEngineId;

#pragma warning disable S1144 // Unused private types or members should be removed - used in Release mode
    private void WarmupEngine()
    {
        _logger.Info("Warming up engine");
        var sw = Stopwatch.StartNew();

        AdjustPosition(Constants.SuperLongPositionCommand);

        const string goWarmupCommand = "go depth 10";   // ~300 ms
        var command = new GoCommand(goWarmupCommand);

        BestMove(command);

        Bench(2);

        sw.Stop();
        _logger.Info("Warm-up finished in {0}ms", sw.ElapsedMilliseconds);
    }
#pragma warning restore S1144 // Unused private types or members should be removed

    private void ResetEngine()
    {
        _tt.Clear();

        // Clear histories
        for (int i = 0; i < 12; ++i)
        {
            Array.Clear(_quietHistory[i]);
            Array.Clear(_moveNodeCount[i]);
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

        ResetEngine();
    }

    [SkipLocalsInit]
    public void AdjustPosition(ReadOnlySpan<char> rawPositionCommand)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        Game.FreeResources();
        Game = PositionCommand.ParseGame(rawPositionCommand, moves);
    }

    public void PonderHit()
    {
        _isPonderHit = true;
    }

    /// <summary>
    /// Uses <see cref="TimeManager.CalculateTimeManagement(Game, GoCommand)"/> internally
    /// </summary>
    public SearchResult BestMove(GoCommand goCommand)
    {
        var searchConstraints = TimeManager.CalculateTimeManagement(Game, goCommand);

        return BestMove(in searchConstraints, CancellationToken.None, CancellationToken.None);
    }

    public SearchResult BestMove(in SearchConstraints searchConstrains, CancellationToken absoluteSearchCancellationToken, CancellationToken searchCancellationToken)
    {
        _searchConstraints = searchConstrains;

        using var jointCts = CancellationTokenSource.CreateLinkedTokenSource(absoluteSearchCancellationToken, searchCancellationToken);

        SearchResult resultToReturn = IDDFS(jointCts.Token);
        //SearchResult resultToReturn = await SearchBestMove(maxDepth, decisionTime);

        Game.ResetCurrentPositionToBeforeSearchState();
        if (!_isPondering
            && resultToReturn.BestMove != default
            && !absoluteSearchCancellationToken.IsCancellationRequested)
        {
            Game.MakeMove(resultToReturn.BestMove);
            Game.UpdateInitialPosition();
        }

        AverageDepth += (resultToReturn.Depth - AverageDepth) / Math.Ceiling(0.5 * Game.PositionHashHistoryLength());

        return resultToReturn;
    }

#pragma warning disable S1144 // Unused private types or members should be removed - wanna keep this around
    private async ValueTask<SearchResult> SearchBestMove(CancellationToken absoluteSearchCancellationToken, CancellationToken searchCancellationToken)
#pragma warning restore S1144 // Unused private types or members should be removed
    {
        using var jointCts = CancellationTokenSource.CreateLinkedTokenSource(absoluteSearchCancellationToken, searchCancellationToken);

        if (!Configuration.EngineSettings.UseOnlineTablebaseInRootPositions || Game.CurrentPosition.CountPieces() > Configuration.EngineSettings.OnlineTablebaseMaxSupportedPieces)
        {
            return IDDFS(jointCts.Token)!;
        }

        // Local copy of positionHashHistory and HalfMovesWithoutCaptureOrPawnMove so that it doesn't interfere with regular search
        var currentHalfMovesWithoutCaptureOrPawnMove = Game.HalfMovesWithoutCaptureOrPawnMove;

        var tasks = new Task<SearchResult?>[] {
                // Other copies of positionHashHistory and HalfMovesWithoutCaptureOrPawnMove (same reason)
                ProbeOnlineTablebase(Game.CurrentPosition, Game.CopyPositionHashHistory(),  Game.HalfMovesWithoutCaptureOrPawnMove),
                Task.Run(()=>(SearchResult?)IDDFS(jointCts.Token))
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

    public SearchResult? Search(GoCommand goCommand, in SearchConstraints searchConstraints, CancellationToken absoluteSearchCancellationToken, CancellationToken searchCancellationToken)
    {
        if (_isSearching)
        {
            _logger.Warn("Search already in progress");
        }

        _isSearching = true;
        _absoluteSearchCancellationToken = absoluteSearchCancellationToken;
        _searchCancellationToken = searchCancellationToken;

        Thread.CurrentThread.Priority = ThreadPriority.Highest;

        // TODO consider using linked cancellation token source
        //using var jointCts = CancellationTokenSource.CreateLinkedTokenSource(absoluteSearchCancellationToken, searchCancellationToken);
        //_absoluteSearchCancellationToken = jointCts.Token;

        try
        {
            _isPondering = goCommand.Ponder;
            var searchResult = BestMove(in searchConstraints, absoluteSearchCancellationToken, searchCancellationToken);

            if (_isPondering)
            {
                // Avoiding the scenario where search finishes early (i.e. mate detected, max depth reached) and results comes
                // before a potential ponderhit command
                SpinWait.SpinUntil(() => _isPonderHit || absoluteSearchCancellationToken.IsCancellationRequested);

                if (_isPonderHit && !absoluteSearchCancellationToken.IsCancellationRequested)
                {
                    _isPonderHit = false;
                    _isPondering = false;

                    searchResult = BestMove(in searchConstraints, absoluteSearchCancellationToken, searchCancellationToken);
                }
            }

            return searchResult;
        }
        catch (Exception e)
        {
            _logger.Fatal(e, "[#{EngineId}] Error in {Method} for position {Position}", _id, nameof(Search), Game.CurrentPosition.FEN());
            return null;
        }
        finally
        {
            _isSearching = false;
            _isPondering = false;
        }
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
#pragma warning disable S3234 // "GC.SuppressFinalize" should not be invoked for types without destructors - https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        GC.SuppressFinalize(this);
#pragma warning restore S3234 // "GC.SuppressFinalize" should not be invoked for types without destructors
    }
}
