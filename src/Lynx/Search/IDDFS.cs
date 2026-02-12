using Lynx.Model;
using NLog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Lynx;

public sealed partial class Engine
{
    private readonly Stopwatch _stopWatch = new();
    private readonly Move[] _pVTable = GC.AllocateArray<Move>(Configuration.EngineSettings.MaxDepth * (Configuration.EngineSettings.MaxDepth + 1 + Constants.ArrayDepthMargin) / 2, pinned: true);

    /// <summary>
    /// 2 x (<see cref="Configuration.EngineSettings.MaxDepth"/> + <see cref="Constants.ArrayDepthMargin"/>)
    /// </summary>
    private readonly int[] _killerMoves = GC.AllocateArray<int>(2 * (Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin), pinned: true);

    /// <summary>
    /// 12 x 64
    /// </summary>
    private readonly int[] _counterMoves = GC.AllocateArray<int>(12 * 64, pinned: true);

    private const int QuietHistoryLength = 12 * 64 * 2 * 2;

    /// <summary>
    /// 12 x 64 x 2 x 2
    /// piece x target square x source is attacked x target is attacked
    /// </summary>
    private readonly short[] _quietHistory = GC.AllocateArray<short>(QuietHistoryLength, pinned: true);

    /// <summary>
    /// 12 x 64 x 12,
    /// piece x target square x captured piece
    /// </summary>
    private readonly short[] _captureHistory = GC.AllocateArray<short>(12 * 64 * 12, pinned: true);

    /// <summary>
    /// 12 x 64 x 12 x 64 x <see cref="EvaluationConstants.ContinuationHistoryPlyCount"/>
    /// piece x target square x last piece x last target square x plies back
    /// ply 0 -> Continuation move history
    /// ply 1 -> Follow-up move history
    /// </summary>
    private readonly short[] _continuationHistory = GC.AllocateArray<short>(12 * 64 * 12 * 64 * EvaluationConstants.ContinuationHistoryPlyCount, pinned: true);

    /// <summary>
    /// <see cref="Constants.PawnCorrHistoryHashSize"/> x 2
    /// Pawn hash x side to move
    /// </summary>
    private readonly short[] _pawnCorrHistory = GC.AllocateArray<short>(Constants.PawnCorrHistoryHashSize * 2, pinned: true);

    /// <summary>
    /// <see cref="Constants.NonPawnCorrHistoryHashMask"/> x 2 x 2
    /// Non-pawn side hash x side to move x piece hash side
    /// </summary>
    private readonly short[] _nonPawnCorrHistory = GC.AllocateArray<short>(Constants.NonPawnCorrHistoryHashSize * 2 * 2, pinned: true);

    /// <summary>
    /// <see cref="Constants.MinorCorrHistoryHashSize"/> x 2
    /// Minor hash x side to move
    /// </summary>
    private readonly short[] _minorCorrHistory = GC.AllocateArray<short>(Constants.MinorCorrHistoryHashSize * 2, pinned: true);

    /// <summary>
    /// <see cref="Constants.MajorCorrHistoryHashSize"/> x 2
    /// Major hash x side to move
    /// </summary>
    private readonly short[] _majorCorrHistory = GC.AllocateArray<short>(Constants.MajorCorrHistoryHashSize * 2, pinned: true);

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

        AgeQuietHistory();

        int bestScore = 0;
        int alpha = EvaluationConstants.MinEval;
        int beta = EvaluationConstants.MaxEval;
        SearchResult? lastSearchResult = null;
        int depth = 1;
        Move firstLegalMove = default;

        _stopWatch.Restart();

        var logLevel = IsMainEngine
            ? LogLevel.Debug
#if MULTITHREAD_DEBUG
            : LogLevel.Trace;
#else
            : LogLevel.Off;
#endif

        try
        {
            if (!isPondering && OnlyOneLegalMove(ref firstLegalMove, out var onlyOneLegalMoveSearchResult))
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

                    if (_logger.IsEnabled(logLevel))
                    {
                        _logger.Log(logLevel,
                            "[#{EngineId}] Depth {Depth}: asp-win [{Alpha}, {Beta}] for previous search score {Score}, nodes {Nodes}",
                            _id, depth, alpha, beta, lastSearchResult.Score, _nodes);
                    }

                    Debug.Assert(
                        lastSearchResult.Mate == 0
                            ? lastSearchResult.Score < EvaluationConstants.PositiveCheckmateDetectionLimit && lastSearchResult.Score > EvaluationConstants.NegativeCheckmateDetectionLimit
                            : Math.Abs(lastSearchResult.Score) < EvaluationConstants.CheckMateBaseEvaluation && (lastSearchResult.Score > EvaluationConstants.PositiveCheckmateDetectionLimit || lastSearchResult.Score < EvaluationConstants.NegativeCheckmateDetectionLimit));

                    while (true)
                    {
                        var depthToSearch = depth - failHighReduction;
                        Debug.Assert(depthToSearch > 0);

                        if (_logger.IsEnabled(logLevel))
                        {
                            _logger.Log(logLevel,
                                "[#{EngineId}] Asp-win depth {Depth} ({DepthWithoutReduction} - {Reduction}), window {Window}: [{Alpha}, {Beta}] for score {Score}, time {Time}, nodes {Nodes}",
                                _id, depthToSearch, depth, failHighReduction, window, alpha, beta, bestScore, _stopWatch.ElapsedMilliseconds, _nodes);
                        }

                        bestScore = NegaMax(depth: depthToSearch, ply: 0, alpha, beta, cutnode: false, cancellationToken);
                        Debug.Assert(bestScore > EvaluationConstants.MinEval && bestScore < EvaluationConstants.MaxEval);

                        // 13, 19, 28, 42, 63, 94, 141, 211, 316, 474, 711, 1066, 1599, 2398, 3597, 5395, 8092, 12138, 18207, 27310, EvaluationConstants.MaxEval
                        window = Math.Min(EvaluationConstants.MaxEval, (int)(window * Configuration.EngineSettings.AspirationWindow_Multiplier));

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
                                _id, depth, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove), bestScore);

                            bestScore = EvaluationConstants.PositiveCheckmateDetectionLimit + 1;

                            break;
                        }

                        if (bestScore < -EvaluationConstants.CheckMateBaseEvaluation)
                        {
                            // TODO bug
                            _logger.Warn(
                            //_logger.Info(
                                "[#{EngineId}] Depth {Depth}: potential -X checkmate detected in position {Position}, but score {BestScore} outside of the limits",
                                _id, depth, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove), bestScore);

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
                        _id, depth, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove), bestScore, mate);

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
            } while (StopSearchCondition(lastSearchResult?.BestMove, depth++, mate, bestScore, isPondering));
        }
        catch (OperationCanceledException)
        {
            // One degree higher than log level, but can't add 1 to Off
            var higherLogLevel = IsMainEngine
                ? LogLevel.Info
#if MULTITHREAD_DEBUG
                : LogLevel.Debug;
#else
                : LogLevel.Off;
#endif

#pragma warning disable S6667 // Logging in a catch clause should pass the caught exception as a parameter - expected exception we want to ignore
            _logger.Log(higherLogLevel,
                "[#{EngineId}] Depth {Depth}: main search cancellation requested after {Time}ms (>= {HardLimitTime}ms). Nodes {Nodes}, best move will be returned",
                _id, depth, _stopWatch.ElapsedMilliseconds, _searchConstraints.HardLimitTimeBound, _nodes);
#pragma warning restore S6667 // Logging in a catch clause should pass the caught exception as a parameter.

            for (int i = 0; i < lastSearchResult?.Moves.Length; ++i)
            {
                _pVTable[i] = lastSearchResult.Moves[i];
            }
        }
        catch (Exception e) when (e is not LynxException)
        {
            _logger.Error(e,
                "[#{EngineId}] Depth {Depth}: unexpected error occurred during the search of position {Position}, best move will be returned\n",
                _id, depth, Game.PositionBeforeLastSearch.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));
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
        var logLevel = IsMainEngine
            ? LogLevel.Info
#if MULTITHREAD_DEBUG
            : LogLevel.Debug;
#else
                : LogLevel.Off;
#endif

        if (depth + 1 >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Log(logLevel,
                "[#{EngineId}] Max depth reached: {MaxDepth}",
                _id, Configuration.EngineSettings.MaxDepth);

            return false;
        }

        if (bestMove is null || bestMove == 0)
        {
            _logger.Warn(
                "[#{EngineId}] Depth {Depth}: search continues, due to lack of best move", _id, depth);

            return true;
        }

        if (mate != 0)
        {
            if (mate == EvaluationConstants.MaxMate || mate == EvaluationConstants.MinMate)
            {
                //_logger.Warn( // TODO bug
                _logger.Info(
                    "[#{EngineId}] Depth {Depth}: mate {Mate} outside of range detected, stopping search and playing best move so far: {BestMove}",
                    _id, depth, mate, bestMove.Value.UCIString());

                return false;
            }

            var winningMateThreshold = (100 - Game.HalfMovesWithoutCaptureOrPawnMove) / 2;

            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] Depth {Depth}: mate in {Mate} detected (score {Score}, {MateThreshold} moves until draw by repetition)",
                    _id, depth, mate, bestScore, winningMateThreshold);
            }

            if (!isPondering && (mate < 0 || mate + Constants.MateDistanceMarginToStopSearching < winningMateThreshold))
            {
                if (_searchConstraints.SoftLimitTimeBound < Configuration.EngineSettings.StopSearchOnMate_MaxSoftTimeBoundLimit
                    && (depth >= Configuration.EngineSettings.StopSearchOnMate_MinDepth
                        || depth >= mate * 2))
                {
                    _logger.Log(logLevel,
                        "[#{EngineId}] Stopping, since mate is short enough and we're short on time: soft limit {SoftLimit}ms",
                        _id, _searchConstraints.SoftLimitTimeBound);

                    return false;
                }

                _logger.Log(logLevel,
                    "[#{EngineId}] Could stop search, since mate is short enough",
                    _id, _searchConstraints.SoftLimitTimeBound);
            }

            _logger.Log(logLevel, "[#{EngineId}] Search continues, hoping to find a faster mate", _id);
        }

        var maxDepth = _searchConstraints.MaxDepth;
        if (maxDepth > 0)
        {
            var shouldContinue = depth + 1 <= maxDepth;

            if (!shouldContinue)
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] Depth {Depth}: stopping, max. depth reached",
                    _id, depth);
            }

            return shouldContinue;
        }

        if (!isPondering)
        {
            var elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;

            var bestMoveNodeCount = _moveNodeCount[bestMove.Value.Piece()][bestMove.Value.TargetSquare()];
            var scaledSoftLimitTimeBound = TimeManager.SoftLimit(_searchConstraints, depth, bestMoveNodeCount, _nodes, _bestMoveStability, _scoreDelta);

            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] [TM] {ElapsedMilliseconds}ms | Depth {Depth}: hard limit {HardLimit}, base soft limit {BaseSoftLimit}ms, scaled soft limit {ScaledSoftLimit}ms",
                    _id, elapsedMilliseconds, depth, _searchConstraints.HardLimitTimeBound, _searchConstraints.SoftLimitTimeBound, scaledSoftLimitTimeBound);
            }

            if (elapsedMilliseconds > scaledSoftLimitTimeBound)
            {
                _logger.Log(logLevel,
                    "[#{EngineId}] [TM] Stopping at depth {0} (nodes {1}): {2}ms > {3}ms",
                    _id, depth, _nodes, elapsedMilliseconds, scaledSoftLimitTimeBound);

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

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        foreach (var move in MoveGenerator.GenerateAllMoves(Game.CurrentPosition, ref evaluationContext, moves))
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

            // We don't have or need any eval, and we return a fake but recognizable one
            // See constant XML for details
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
                $"[#{_id}] Depth {depth}: search cancelled with no result for position {Game.PositionBeforeLastSearch.FEN()} (hard limit {_searchConstraints.HardLimitTimeBound}ms, soft limit {_searchConstraints.SoftLimitTimeBound}ms). Choosing an emergency move";

            // In the event of a quick ponderhit/stop while pondering because the opponent moved quickly, we don't want no warning triggered here
            //  when cancelling the pondering search
            // The other condition reflects what happens in helper engines when a mate is quickly detected in the main:
            //  search in helper engines sometimes get cancelled before any meaningful result is found, so we don't want a warning either
            if (isPondering || !IsMainEngine)
            {
                _logger.Debug(noDepth1Message);
            }
            else
            {
                _logger.Warn(noDepth1Message);
            }

            finalSearchResult = BestMoveRoot(firstLegalMove);
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

    /// <summary>
    /// Find the best move without searching, based on TT and <see cref="ScoreMove(int, int, short)"/>
    /// </summary>
    private SearchResult BestMoveRoot(Move firstLegalMove)
    {
        var score = 0;
        ShortMove ttBestMove = default;

        using var position = new Position(Game.PositionBeforeLastSearch);
        var ttHit = _tt.ProbeHash(position, Game.HalfMovesWithoutCaptureOrPawnMove, ply: 0, out var ttEntry);

        if (ttHit)
        {
            ttBestMove = ttEntry.BestMove;
            score = ttEntry.Score;

            if (ttEntry.Score == EvaluationConstants.NoScore)
            {
                score = ttEntry.StaticEval;
            }
        }

        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        Span<Move> pseudoLegalMoves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, ref evaluationContext, pseudoLegalMoves);

        Span<int> moveScores = stackalloc int[pseudoLegalMoves.Length];
        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            moveScores[i] = ScoreMove(position, pseudoLegalMoves[i], 0, ref evaluationContext, ttBestMove);
        }

        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            // Incremental move sorting
            for (int j = i + 1; j < pseudoLegalMoves.Length; j++)
            {
                if (moveScores[j] > moveScores[i])
                {
                    (moveScores[i], moveScores[j], pseudoLegalMoves[i], pseudoLegalMoves[j]) = (moveScores[j], moveScores[i], pseudoLegalMoves[j], pseudoLegalMoves[i]);
                }
            }

            var move = pseudoLegalMoves[i];

            var gameState = position.MakeMove(move);
            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            // We don't have or need any eval, and we don't want to return 0 or a negative eval that
            // could make the GUI resign or take a draw from this position.
            // Since this only happens in root, we don't really care about being more precise for raising
            // alphas or betas of parent moves, so let's just return +-2 pawns depending on the side to move
            var singleMoveEval = Game.CurrentPosition.Side == Side.White
                ? EvaluationConstants.EmergencyMoveScore        // -0.66
                : -EvaluationConstants.EmergencyMoveScore;      // +0.66

            return new SearchResult(
#if MULTITHREAD_DEBUG
                _id,
#endif
                move, singleMoveEval, 0, [move])
            {
                DepthReached = 0
            };
        }

        _logger.Error("No valid move found while looking for an emergency move for position {Fen}", position.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));

        return new(
#if MULTITHREAD_DEBUG
                _id,
#endif
            firstLegalMove, 0, 0, [firstLegalMove]);
    }
}
