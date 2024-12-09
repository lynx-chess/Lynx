using Lynx.Model;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Lynx;

public sealed partial class Engine
{
    private readonly Stopwatch _stopWatch = new();
    private readonly Move[] _pVTable = GC.AllocateArray<Move>(Configuration.EngineSettings.MaxDepth * (Configuration.EngineSettings.MaxDepth + 1) / 2, pinned: true);

    /// <summary>
    /// 3 x (<see cref="Configuration.EngineSettings.MaxDepth"/> + <see cref="Constants.ArrayDepthMargin"/>)
    /// </summary>
    private readonly int[] _killerMoves = GC.AllocateArray<int>(3 * (Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin), pinned: true);

    /// <summary>
    /// 12 x 64
    /// </summary>
    private readonly int[] _counterMoves = GC.AllocateArray<int>(12 * 64, pinned: true);

    /// <summary>
    /// 12 x 64
    /// piece x target square
    /// </summary>
    private readonly int[][] _quietHistory;

    /// <summary>
    /// 12 x 64 x 12,
    /// piece x target square x captured piece
    /// </summary>
    private readonly int[] _captureHistory = GC.AllocateArray<int>(12 * 64 * 12, pinned: true);

    /// <summary>
    /// 12 x 64 x 12 x 64 x ContinuationHistoryPlyCount
    /// piece x target square x last piece x last target square x plies back
    /// ply 0 -> Continuation move history
    /// ply 1 -> Follow-up move history
    /// </summary>
    private readonly int[] _continuationHistory = GC.AllocateArray<int>(12 * 64 * 12 * 64 * EvaluationConstants.ContinuationHistoryPlyCount, pinned: true);

    /// <summary>
    /// 12 x 64
    /// piece x target square
    /// </summary>
    private readonly ulong[][] _moveNodeCount;

    private readonly int[] _maxDepthReached = GC.AllocateArray<int>(Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin, pinned: true);

    private ulong _nodes;

    private SearchResult? _previousSearchResult;

    private readonly Move _defaultMove = default;

    private int _bestMoveStability = 0;
    private int _scoreDelta = 0;

    /// <summary>
    /// Iterative Deepening Depth-First Search (IDDFS) using alpha-beta pruning.
    /// Requires <see cref="_searchConstraints"/> to be populated before invoking it
    /// </summary>
    /// <returns>Not null <see cref="SearchResult"/>, although made nullable in order to match online tb probing signature</returns>
    [SkipLocalsInit]
    private SearchResult IDDFS()
    {
        // Cleanup
        _nodes = 0;

        Array.Clear(_pVTable);
        Array.Clear(_maxDepthReached);
        for (int i = 0; i < 12; ++i)
        {
            Array.Clear(_moveNodeCount[i]);
        }

        int bestScore = 0;
        int alpha = EvaluationConstants.MinEval;
        int beta = EvaluationConstants.MaxEval;
        SearchResult? lastSearchResult = null;
        int depth = 1;
        bool isMateDetected = false;
        Move firstLegalMove = default;

        _stopWatch.Restart();

        try
        {
            if (OnlyOneLegalMove(ref firstLegalMove, out var onlyOneLegalMoveSearchResult))
            {
                _engineWriter.TryWrite(onlyOneLegalMoveSearchResult);

                return onlyOneLegalMoveSearchResult;
            }

            Array.Clear(_killerMoves);
            // Not clearing _quietHistory on purpose
            // Not clearing _captureHistory on purpose

            if (lastSearchResult is not null)
            {
                _engineWriter.TryWrite(lastSearchResult);
            }

            int mate = 0;

            do
            {
                _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
                _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (depth < Configuration.EngineSettings.AspirationWindow_MinDepth
                    || lastSearchResult?.Score is null)
                {
                    bestScore = NegaMax(depth: depth, ply: 0, alpha, beta, cutnode: false);
                }
                else
                {
                    // 🔍 Aspiration windows - search using a window around an expected score (the previous search one)
                    // If the resulting score doesn't fall inside of the window, it is widened it until it does
                    var window = Configuration.EngineSettings.AspirationWindow_Base;

                    // A temporary reduction is used for fail highs, because the verification for those 'too good' lines
                    // are expected to happen at lower depths
                    int failHighReduction = 0;

                    alpha = Math.Max(EvaluationConstants.MinEval, lastSearchResult.Score - window);
                    beta = Math.Min(EvaluationConstants.MaxEval, lastSearchResult.Score + window);

                    _logger.Debug("Aspiration windows depth {Depth}: [{Alpha}, {Beta}] for previous search score {Score}, nodes {Nodes}",
                        depth, alpha, beta, lastSearchResult.Score, _nodes);
                    Debug.Assert(lastSearchResult.Mate == 0 && lastSearchResult.Score > EvaluationConstants.NegativeCheckmateDetectionLimit && lastSearchResult.Score < EvaluationConstants.PositiveCheckmateDetectionLimit);

                    while (true)
                    {
                        _logger.Debug("Aspiration windows depth {Depth}: [{Alpha}, {Beta}] for score {Score}, nodes {Nodes}",
                            depth, alpha, beta, bestScore, _nodes);

                        bestScore = NegaMax(depth: depth - failHighReduction, ply: 0, alpha, beta, cutnode: false);

                        // 13, 19, 28, 42, 63, 94, 141, 211, 316, 474, 711, 1066, 1599, 2398, 3597, 5395, 8092, 12138, 18207, 27310, |EvaluationConstants.MaxEval|, 40965
                        window += window >> 1;   // window / 2

                        // Depth change: https://github.com/lynx-chess/Lynx/pull/440
                        if (alpha >= bestScore)     // Fail low
                        {
                            alpha = Math.Max(bestScore - window, EvaluationConstants.MinEval);
                            beta = (alpha + beta) >> 1;  // (alpha + beta) / 2
                            failHighReduction = 0;
                        }
                        else if (beta <= bestScore)     // Fail high
                        {
                            beta = Math.Min(bestScore + window, EvaluationConstants.MaxEval);
                            ++failHighReduction;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                //PrintPvTable(depth: depth);
                ValidatePVTable();
                Debug.Assert(bestScore != EvaluationConstants.MinEval);

                var bestScoreAbs = Math.Abs(bestScore);
                isMateDetected = bestScoreAbs > EvaluationConstants.PositiveCheckmateDetectionLimit;
                mate = isMateDetected
                    ? Utils.CalculateMateInX(bestScore, bestScoreAbs)
                    : 0;

                var oldBestMove = lastSearchResult?.BestMove;
                var oldScore = lastSearchResult?.Score ?? 0;
                lastSearchResult = UpdateLastSearchResult(lastSearchResult, bestScore, depth, mate);

                if (oldBestMove == lastSearchResult.BestMove)
                {
                    ++_bestMoveStability;
                }
                else
                {
                    _bestMoveStability = 0;
                }

                _scoreDelta = oldScore - lastSearchResult.Score;

                _engineWriter.TryWrite(lastSearchResult);
            } while (StopSearchCondition(lastSearchResult.BestMove, ++depth, mate));
        }
        catch (OperationCanceledException)
        {
#pragma warning disable S6667 // Logging in a catch clause should pass the caught exception as a parameter - expected exception we want to ignore
            _logger.Info(
#if MULTITHREAD_DEBUG
                $"[#{_id}] " +
#endif
                "Search cancellation requested after {0}ms (depth {1}, nodes {2}), best move will be returned", _stopWatch.ElapsedMilliseconds, depth, _nodes);
#pragma warning restore S6667 // Logging in a catch clause should pass the caught exception as a parameter.

            for (int i = 0; i < lastSearchResult?.Moves.Length; ++i)
            {
                _pVTable[i] = lastSearchResult.Moves[i];
            }
        }
        catch (Exception e) when (e is not AssertException)
        {
            _logger.Error(e,
#if MULTITHREAD_DEBUG
                $"[#{_id}] " +
#endif
                "Unexpected error ocurred during the search of position {0} at depth {1}, best move will be returned\n{2}", Game.PositionBeforeLastSearch.FEN(), depth, e.StackTrace);
        }
        finally
        {
            _stopWatch.Stop();
        }

        var finalSearchResult = GenerateFinalSearchResult(lastSearchResult, bestScore, depth, firstLegalMove);

        if (Configuration.EngineSettings.UseOnlineTablebaseInRootPositions
            && isMateDetected
            && (finalSearchResult.Mate * 2) + Game.HalfMovesWithoutCaptureOrPawnMove < Constants.MaxMateDistanceToStopSearching)
        {
            _searchCancellationTokenSource.Cancel();
            _logger.Info("Engine search found a short enough mate, cancelling online tb probing if still active");
        }

        return finalSearchResult;
    }

    private bool StopSearchCondition(Move bestMove, int depth, int mate)
    {
        if (bestMove == default)
        {
            _logger.Warn(
#if s
                $"[#{_id}] " +
#endif
                "Search at depth {0} didn't produce a best move. Mate in {1} detected, and/but search continues", depth - 1, mate);
            return true;
        }

        if (mate != 0)
        {
            var winningMateThreshold = (100 - Game.HalfMovesWithoutCaptureOrPawnMove) / 2;
            _logger.Info(
#if MULTITHREAD_DEBUG
                $"[#{_id}] " +
#endif
                "Depth {0}: mate in {1} detected ({2} moves until draw by repetition)", depth - 1, mate, winningMateThreshold);

            if (mate < 0 || mate + Constants.MateDistanceMarginToStopSearching < winningMateThreshold)
            {
                _logger.Info(
#if MULTITHREAD_DEBUG
                $"[#{_id}] " +
#endif
                    "Stopping search: mate is short enough");
                return false;
            }

            _logger.Info(
#if MULTITHREAD_DEBUG
                $"[#{_id}] " +
#endif
                "Search continues, hoping to find a faster mate");
        }

        if (depth >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth reached: {0}", Configuration.EngineSettings.MaxDepth);
            return false;
        }

        var maxDepth = _searchConstraints.MaxDepth;
        if (maxDepth > 0)
        {
            var shouldContinue = depth <= maxDepth;

            if (!shouldContinue)
            {
                _logger.Info("Stopping at depth {0}: max. depth reached", depth - 1);
            }

            return shouldContinue;
        }

        var elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;

        var bestMoveNodeCount = _moveNodeCount[bestMove.Piece()][bestMove.TargetSquare()];
        var scaledSoftLimitTimeBound = TimeManager.SoftLimit(_searchConstraints, depth - 1, bestMoveNodeCount, _nodes, _bestMoveStability, _scoreDelta);
        _logger.Debug(
#if MULTITHREAD_DEBUG
                $"[#{_id}] " +
#endif
            "[TM] Depth {Depth}: hard limit {HardLimit}, base soft limit {BaseSoftLimit}, scaled soft limit {ScaledSoftLimit}", depth - 1, _searchConstraints.HardLimitTimeBound, _searchConstraints.SoftLimitTimeBound, scaledSoftLimitTimeBound);

        if (elapsedMilliseconds > scaledSoftLimitTimeBound)
        {
            _logger.Info(
#if MULTITHREAD_DEBUG
                $"[#{_id}] " +
#endif
                "[TM] Stopping at depth {0} (nodes {1}): {2}ms > {3}ms", depth - 1, _nodes, elapsedMilliseconds, scaledSoftLimitTimeBound);
            return false;
        }

        return true;
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool OnlyOneLegalMove(ref Move firstLegalMove, [NotNullWhen(true)] out SearchResult? result)
    {
        bool onlyOneLegalMove = false;

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        foreach (var move in MoveGenerator.GenerateAllMoves(Game.CurrentPosition, moves))
        {
            var gameState = Game.CurrentPosition.MakeMove(move);
            bool isPositionValid = Game.CurrentPosition.WasProduceByAValidMove();
            Game.CurrentPosition.UnmakeMove(move, gameState);

            if (isPositionValid)
            {
                // We save the first legal move and check if there's at least another one
                if (firstLegalMove == default)
                {
                    firstLegalMove = move;
                    onlyOneLegalMove = true;
                }
                // If there's a second legal move, we exit and let the search continue
                else
                {
                    onlyOneLegalMove = false;
                    break;
                }
            }
        }

        // Detect if there was only one legal move
        if (onlyOneLegalMove)
        {
            _logger.Debug("One single move found");

            // We don't have or need any eval, and we don't want to return 0 or a negative eval that
            // could make the GUI resign or take a draw from this position.
            // Since this only happens in root, we don't really care about being more precise for raising
            // alphas or betas of parent moves, so let's just return +-2 pawns depending on the side to move
            var score = Game.CurrentPosition.Side == Side.White
                ? +EvaluationConstants.SingleMoveScore
                : -EvaluationConstants.SingleMoveScore;

            result = new SearchResult(
#if MULTITHREAD_DEBUG
                _id,
#endif
                firstLegalMove, score, 0, [firstLegalMove])
            {
                DepthReached = 0,
                Nodes = 0,
                Time = 0,
                NodesPerSecond = 0
            };

            return true;
        }

        result = null;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SearchResult UpdateLastSearchResult(SearchResult? lastSearchResult,
        int bestScore, int depth, int mate)
    {
        var pvTableSpan = _pVTable.AsSpan();
        var pvMoves = pvTableSpan[..pvTableSpan.IndexOf(0)].ToArray();

        var maxDepthReached = _maxDepthReached.LastOrDefault(item => item != default);

        var elapsedSeconds = Utils.CalculateElapsedSeconds(_stopWatch);

        _previousSearchResult = lastSearchResult;
        return new SearchResult(
#if MULTITHREAD_DEBUG
                _id,
#endif
            pvMoves.FirstOrDefault(), bestScore, depth, pvMoves, mate)
        {
            DepthReached = maxDepthReached,
            Nodes = _nodes,
            Time = Utils.CalculateUCITime(elapsedSeconds),
            NodesPerSecond = Utils.CalculateNps(_nodes, elapsedSeconds)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SearchResult GenerateFinalSearchResult(SearchResult? lastSearchResult,
        int bestScore, int depth, Move firstLegalMove)
    {
        SearchResult finalSearchResult;
        if (lastSearchResult is null)
        {
            // In the event of a quick ponderhit/stop while pondering because the opponent moved quickly, we don't want no warning triggered here
            // when cancelling the pondering search
            if (!_isPondering)
            {
                _logger.Warn("Search cancelled at depth 1, choosing first found legal move as best one");
            }
            finalSearchResult = new(
#if MULTITHREAD_DEBUG
                _id,
#endif
                firstLegalMove, 0, 0, [firstLegalMove]);
        }
        else
        {
            finalSearchResult = _previousSearchResult = lastSearchResult;
        }

        var elapsedSeconds = Utils.CalculateElapsedSeconds(_stopWatch);

        finalSearchResult.DepthReached = Math.Max(finalSearchResult.DepthReached, _maxDepthReached.LastOrDefault(item => item != default));
        finalSearchResult.Nodes = _nodes;
        finalSearchResult.Time = Utils.CalculateUCITime(elapsedSeconds);
        finalSearchResult.NodesPerSecond = Utils.CalculateNps(_nodes, elapsedSeconds);
        finalSearchResult.HashfullPermill = _tt.HashfullPermillApprox();
        if (Configuration.EngineSettings.ShowWDL)
        {
            finalSearchResult.WDL = WDL.WDLModel(bestScore, depth);
        }

        return finalSearchResult;
    }
}
