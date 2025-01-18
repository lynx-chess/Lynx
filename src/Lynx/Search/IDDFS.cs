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

    /// <summary>
    /// <see cref="Constants.KingPawnHashSize"/>
    /// </summary>
    private readonly PawnTableElement[] _pawnEvalTable = GC.AllocateArray<PawnTableElement>(Constants.KingPawnHashSize, pinned: true);

    private ulong _nodes;

    private SearchResult? _previousSearchResult;

#pragma warning disable CA1805 // Do not initialize unnecessarily - Interferes with S3459
    private readonly Move _defaultMove = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily

    private int _bestMoveStability;
    private int _scoreDelta;

    /// <summary>
    /// Iterative Deepening Depth-First Search (IDDFS) using alpha-beta pruning.
    /// Requires <see cref="_searchConstraints"/> to be populated before invoking it
    /// </summary>
    /// <returns>Not null <see cref="SearchResult"/>, although made nullable in order to match online tb probing signature</returns>
    [SkipLocalsInit]
    private SearchResult IDDFS(bool isPondering, CancellationToken cancellationToken)
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

            int mate = 0;

            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (depth < Configuration.EngineSettings.AspirationWindow_MinDepth
                    || lastSearchResult?.Score is null)
                {
                    bestScore = NegaMax(depth: depth, ply: 0, alpha, beta, cutnode: false, cancellationToken);
                }
                else
                {
                    // 🔍 Aspiration windows - search using a window around an expected score (the previous search one)
                    // If the resulting score doesn't fall inside of the window, it is widened it until it does
                    var window = Configuration.EngineSettings.AspirationWindow_Base;

                    // A temporary reduction is used for fail highs, because the verification for those 'too good' lines
                    // are expected to happen at lower depths
                    int failHighReduction = 0;

                    alpha = Math.Clamp(lastSearchResult.Score - window, EvaluationConstants.MinEval, EvaluationConstants.MaxEval);
                    beta = Math.Clamp(lastSearchResult.Score + window, EvaluationConstants.MinEval, EvaluationConstants.MaxEval);

                    _logger.Info(
                        "[#{EngineId}] Depth {Depth}: aspiration windows [{Alpha}, {Beta}] for previous search score {Score}, nodes {Nodes}",
                        _id, depth, alpha, beta, lastSearchResult.Score, _nodes);
                    Debug.Assert(
                        lastSearchResult.Mate == 0
                            ? lastSearchResult.Score < EvaluationConstants.PositiveCheckmateDetectionLimit && lastSearchResult.Score > EvaluationConstants.NegativeCheckmateDetectionLimit
                            : Math.Abs(lastSearchResult.Score) < EvaluationConstants.CheckMateBaseEvaluation && (lastSearchResult.Score > EvaluationConstants.PositiveCheckmateDetectionLimit || lastSearchResult.Score < EvaluationConstants.NegativeCheckmateDetectionLimit));

                    while (true)
                    {
                        var depthToSearch = depth - failHighReduction;
                        Debug.Assert(depthToSearch > 0);

                        _logger.Info(
                            "[#{EngineId}] Aspiration windows depth {Depth} ({DepthWithoutReduction} - {Reduction}), window {Window}: [{Alpha}, {Beta}] for score {Score}, nodes {Nodes}",
                            _id, depthToSearch, depth, failHighReduction, window, alpha, beta, bestScore, _nodes);

                        bestScore = NegaMax(depth: depthToSearch, ply: 0, alpha, beta, cutnode: false, cancellationToken);
                        Debug.Assert(bestScore > EvaluationConstants.MinEval && bestScore < EvaluationConstants.MaxEval);

                        // 13, 19, 28, 42, 63, 94, 141, 211, 316, 474, 711, 1066, 1599, 2398, 3597, 5395, 8092, 12138, 18207, 27310, EvaluationConstants.MaxEval
                        window = Math.Min(EvaluationConstants.MaxEval, window + (window >> 1));   // window / 2

                        // Depth change: https://github.com/lynx-chess/Lynx/pull/440
                        if (alpha >= bestScore)     // Fail low
                        {
                            alpha = Math.Clamp(bestScore - window, EvaluationConstants.MinEval, EvaluationConstants.MaxEval);
                            beta = (alpha + beta) >> 1;  // (alpha + beta) / 2
                            failHighReduction = 0;
                        }
                        else if (beta <= bestScore)     // Fail high
                        {
                            beta = Math.Clamp(bestScore + window, EvaluationConstants.MinEval, EvaluationConstants.MaxEval);
                            if (failHighReduction < 3)
                            {
                                ++failHighReduction;
                            }
                        }
                        else
                        {
                            break;
                        }

                        if (bestScore > EvaluationConstants.CheckMateBaseEvaluation)
                        {
                            _logger.Warn(
                                "[#{EngineId}] Depth {Depth}: potential +X checkmate detected in position {Position}, but score {BestScore} outside of the limits",
                                _id, depth, Game.PositionBeforeLastSearch.FEN(), bestScore);

                            bestScore = EvaluationConstants.PositiveCheckmateDetectionLimit + 1;

                            break;
                        }

                        if (bestScore < -EvaluationConstants.CheckMateBaseEvaluation)
                        {
                            _logger.Warn(
                                "[#{EngineId}] Depth {Depth}: potential -X checkmate detected in position {Position}, but score {BestScore} outside of the limits",
                                _id, depth, Game.PositionBeforeLastSearch.FEN(), bestScore);

                            bestScore = EvaluationConstants.NegativeCheckmateDetectionLimit - 1;

                            break;
                        }
                    }
                }

                //PrintPvTable(depth: depth);
                ValidatePVTable();
                Debug.Assert(bestScore != EvaluationConstants.MinEval);

                var bestScoreAbs = Math.Abs(bestScore);
                bool isMateDetected = bestScoreAbs > EvaluationConstants.PositiveCheckmateDetectionLimit && bestScoreAbs < EvaluationConstants.CheckMateBaseEvaluation;
                mate = isMateDetected
                    ? Utils.CalculateMateInX(bestScore, bestScoreAbs)
                    : 0;

                var oldBestMove = lastSearchResult?.BestMove;
                var oldScore = lastSearchResult?.Score ?? 0;
                var lastSearchResultCandidate = UpdateLastSearchResult(lastSearchResult, bestScore, depth, mate);

                if (lastSearchResultCandidate.BestMove == default)
                {
                    _logger.Warn(
                        "[#{EngineId}] Depth {Depth}: search didn't produce a best move for position {Position}. Score {Score} (mate in {Mate}?) detected",
                        _id, depth, Game.PositionBeforeLastSearch.FEN(), bestScore, mate);

                    _bestMoveStability = 0;
                    _scoreDelta = 0;

                    continue;
                }

                lastSearchResult = lastSearchResultCandidate;

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
            } while (StopSearchCondition(lastSearchResult?.BestMove, ++depth, mate, bestScore, isPondering));
        }
        catch (OperationCanceledException)
        {
#pragma warning disable S6667 // Logging in a catch clause should pass the caught exception as a parameter - expected exception we want to ignore
            _logger.Info(
                "[#{EngineId}] Depth {Depth}: search cancellation requested after {Time}ms (nodes {Nodes}), best move will be returned",
                _id, depth, _stopWatch.ElapsedMilliseconds, _nodes);
#pragma warning restore S6667 // Logging in a catch clause should pass the caught exception as a parameter.

            for (int i = 0; i < lastSearchResult?.Moves.Length; ++i)
            {
                _pVTable[i] = lastSearchResult.Moves[i];
            }
        }
        catch (Exception e) when (e is not LynxException)
        {
            _logger.Error(e,
                "[#{EngineId}] Depth {Depth}: unexpected error ocurred during the search of position {Position}, best move will be returned\n{StackTrace}",
                _id, depth, Game.PositionBeforeLastSearch.FEN(), e.StackTrace);
        }
        finally
        {
            _stopWatch.Stop();
        }

        //TODO revisit
        //if (Configuration.EngineSettings.UseOnlineTablebaseInRootPositions
        //    && isMateDetected
        //    && (finalSearchResult.Mate * 2) + Game.HalfMovesWithoutCaptureOrPawnMove < Constants.MaxMateDistanceToStopSearching)
        //{
        //    _searchCancellationToken.Cancel();
        //    _logger.Info("Engine search found a short enough mate, cancelling online tb probing if still active");
        //}

        return GenerateFinalSearchResult(lastSearchResult, bestScore, depth, firstLegalMove, isPondering);
    }

    private bool StopSearchCondition(Move? bestMove, int depth, int mate, int bestScore, bool isPondering)
    {
        if (bestMove is null || bestMove == 0)
        {
            _logger.Warn(
                "[#{EngineId}] Depth {Depth}: search continues, due to lack of best move", _id, depth - 1);

            return true;
        }

        if (mate != 0)
        {
            if (mate == EvaluationConstants.MaxMate || mate == EvaluationConstants.MinMate)
            {
                _logger.Warn(
                    "[#{EngineId}] Depth {Depth}: mate outside of range detected, stopping search and playing best move {BestMove}",
                    _id, depth - 1, bestMove.Value.UCIString());

                return false;
            }

            var winningMateThreshold = (100 - Game.HalfMovesWithoutCaptureOrPawnMove) / 2;
            _logger.Info(
                "[#{EngineId}] Depth {Depth}: mate in {Mate} detected (score {Score}, {MateThreshold} moves until draw by repetition)",
                _id, depth - 1, mate, bestScore, winningMateThreshold);

            if (mate < 0 || mate + Constants.MateDistanceMarginToStopSearching < winningMateThreshold)
            {
                _logger.Info("[#{EngineId}] Could stop search, since mate is short enough", _id);
            }

            _logger.Info("[#{EngineId}] Search continues, hoping to find a faster mate", _id);
        }

        if (depth >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info(
                "[#{EngineId}] Max depth reached: {MaxDepth}",
                _id, Configuration.EngineSettings.MaxDepth);
            return false;
        }

        var maxDepth = _searchConstraints.MaxDepth;
        if (maxDepth > 0)
        {
            var shouldContinue = depth <= maxDepth;

            if (!shouldContinue)
            {
                _logger.Info("[#{EngineId}] Depth {Depth}: stopping, max. depth reached", _id, depth - 1);
            }

            return shouldContinue;
        }

        if (!isPondering)
        {
            var elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;

            var bestMoveNodeCount = _moveNodeCount[bestMove.Value.Piece()][bestMove.Value.TargetSquare()];
            var scaledSoftLimitTimeBound = TimeManager.SoftLimit(_searchConstraints, depth - 1, bestMoveNodeCount, _nodes, _bestMoveStability, _scoreDelta);
            _logger.Debug(
                "[#{EngineId}] [TM] Depth {Depth}: hard limit {HardLimit}, base soft limit {BaseSoftLimit}, scaled soft limit {ScaledSoftLimit}",
                _id, depth - 1, _searchConstraints.HardLimitTimeBound, _searchConstraints.SoftLimitTimeBound, scaledSoftLimitTimeBound);

            if (elapsedMilliseconds > scaledSoftLimitTimeBound)
            {
                _logger.Info(
                    "[#{EngineId}] [TM] Stopping at depth {0} (nodes {1}): {2}ms > {3}ms",
                    _id, depth - 1, _nodes, elapsedMilliseconds, scaledSoftLimitTimeBound);
                return false;
            }
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

        var maxDepthReached = _maxDepthReached.Max();

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
        int bestScore, int depth, Move firstLegalMove, bool isPondering)
    {
        SearchResult finalSearchResult;
        if (lastSearchResult is null)
        {
            var noDepth1Message =
                $"[#{_id}] Depth {depth}: search cancelled with no result for position {Game.CurrentPosition.FEN()} (hard limit {_searchConstraints.HardLimitTimeBound}ms, soft limit {_searchConstraints.SoftLimitTimeBound}ms). Choosing first found legal move as best one";

            // In the event of a quick ponderhit/stop while pondering because the opponent moved quickly, we don't want no warning triggered here
            //  when cancelling the pondering search
            // The other condition reflects what happens in helper engines when a mate is quickly detected in the main:
            //  search in helper engines sometimes get cancelled before any meaningful result is found, so we don't want a warning either
            if (isPondering || !IsMainEngine())
            {
                _logger.Info(noDepth1Message);
            }
            else
            {
                _logger.Warn(noDepth1Message);
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
