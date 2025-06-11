#pragma warning disable S1192 // String literals should not be duplicated - it's assertion message strings

using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx;

public sealed partial class Engine
{
    /// <summary>
    /// NegaMax algorithm implementation using alpha-beta pruning and quiescence search
    /// </summary>
    /// <param name="alpha">
    /// Best score the Side to move can achieve, assuming best play by the opponent.
    /// </param>
    /// <param name="beta">
    /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
    /// </param>
    [SkipLocalsInit]
    private int NegaMax(int depth, int ply, int alpha, int beta, bool cutnode, CancellationToken cancellationToken,
        bool parentWasNullMove = false, bool isVerifyingSE = false)
    {
        var position = Game.CurrentPosition;

        Debug.Assert(depth >= 0 || !position.IsInCheck(), "Assertion failed", "Current check extension impl won't work otherwise");

        // Prevents runtime failure in case depth is increased due to check extension, since we're using ply when calculating pvTable index,
        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            if (IsMainEngine)
            {
                _logger.Debug("[#{EngineId}] Max depth {Depth} reached - position {FEN}",
                    _id, Configuration.EngineSettings.MaxDepth, position.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));
            }
#if MULTITHREAD_DEBUG
            else
            {
                _logger.Trace("[#{EngineId}] Max depth {Depth} reached - position {FEN}",
                _id, Configuration.EngineSettings.MaxDepth, position.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));
            }
#endif
            return position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove, _pawnEvalTable).Score;
        }

        _maxDepthReached[ply] = ply;

        cancellationToken.ThrowIfCancellationRequested();

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        bool isRoot = ply == 0;
        bool pvNode = beta - alpha > 1;
        int depthExtension = 0;

        ShortMove ttBestMove = default;
        NodeType ttElementType = NodeType.Unknown;
        int ttScore = EvaluationConstants.NoHashEntry;
        int ttStaticEval = int.MinValue;
        int ttDepth = default;
        bool ttWasPv = false;

        bool ttHit = false;
        bool ttEntryHasBestMove = false;
        bool ttMoveIsCapture = false;

        Debug.Assert(!pvNode || !cutnode);

        if (!isRoot)
        {
            (ttScore, ttBestMove, ttElementType, ttStaticEval, ttDepth, ttWasPv) = _tt.ProbeHash(position, Game.HalfMovesWithoutCaptureOrPawnMove, ply);

            // ttScore shouldn't be used, since it'll be 0 for default structs
            ttHit = ttElementType != NodeType.Unknown && ttElementType != NodeType.None;

            ttEntryHasBestMove = ttBestMove != default;

            // TT cutoffs
            if (!isVerifyingSE && ttHit && ttDepth >= depth)
            {
                if (ttElementType == NodeType.Exact
                    || (ttElementType == NodeType.Alpha && ttScore <= alpha)
                    || (ttElementType == NodeType.Beta && ttScore >= beta))
                {
                    if (!pvNode)
                    {
                        return ttScore;
                    }

                    // In PV nodes, instead of the cutoff we reduce the depth
                    // Suggested by Calvin author, originally from Motor
                    // I had to add the not-in-check guard
                    if (!position.IsInCheck())
                    {
                        --depthExtension;
                    }
                }
                else if (!pvNode
                    && depth <= Configuration.EngineSettings.TTHit_NoCutoffExtension_MaxDepth
                    && ply < depth * 4) // To avoid weird search explosions, see HighSeldepthAtDepth2 test. Patch suggested by Sirius author
                {
                    // Extension idea from Stormphrax
                    ++depthExtension;
                }
            }

            ttMoveIsCapture = ttHit && ttEntryHasBestMove && position.Board[((int)ttBestMove).TargetSquare()] != (int)Piece.None;

            // Internal iterative reduction (IIR)
            // If this position isn't found in TT, it has never been searched before,
            // so the search will be potentially expensive.
            // Therefore, we search with reduced depth for now, expecting to record a TT move
            // which we'll be able to use later for the full depth search
            if (depth >= Configuration.EngineSettings.IIR_MinDepth
                && (!ttHit || !ttEntryHasBestMove))
            {
                --depthExtension;
            }
        }

        var ttPv = pvNode || ttWasPv;

        // 🔍 Improving heuristic: the current position has a better static evaluation than
        // the previous evaluation from the same side (ply - 2).
        // When true, we can:
        // - Prune more aggressively when evaluation is too high: current position is even getter
        // - Prune less aggressively when evaluation is low low: uncertainty on how bad the position really is
        bool improving = false;

        // From Potential
        double improvingRate = 0;

        bool isInCheck = position.IsInCheck();
        int rawStaticEval, staticEval;
        int phase = int.MaxValue;
        ref var stack = ref Game.Stack(ply);

        if (isInCheck)
        {
            ++depthExtension;
        }

        if (depth + depthExtension <= 0)
        {
            if (MoveGenerator.CanGenerateAtLeastAValidMove(position))
            {
                return QuiescenceSearch(ply, alpha, beta, pvNode, cancellationToken);
            }

            var finalPositionEvaluation = Position.EvaluateFinalPosition(ply, isInCheck);
            _tt.RecordHash(position, Game.HalfMovesWithoutCaptureOrPawnMove, finalPositionEvaluation, depth, ply, finalPositionEvaluation, NodeType.Exact, ttPv);
            return finalPositionEvaluation;
        }
        else if (!pvNode && !isInCheck)
        {
            if (ttElementType != NodeType.Unknown)   // Equivalent to ttHit || ttElementType == NodeType.None
            {
                Debug.Assert(ttStaticEval != int.MinValue);

                rawStaticEval = ttStaticEval;
                staticEval = CorrectStaticEvaluation(position, rawStaticEval);
                phase = position.Phase();
            }
            else
            {
                (rawStaticEval, phase) = position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove, _pawnEvalTable);
                _tt.SaveStaticEval(position, Game.HalfMovesWithoutCaptureOrPawnMove, rawStaticEval, ttPv);
                staticEval = CorrectStaticEvaluation(position, rawStaticEval);
            }

            stack.StaticEval = staticEval;

            if (ply >= 2)
            {
                var evalDiff = staticEval - Game.ReadStaticEvalFromStack(ply - 2);
                improving = evalDiff >= 0;
                improvingRate = evalDiff / 50.0;
            }

            // From smol.cs
            // ttEvaluation can be used as a better positional evaluation:
            // If the score is outside what the current bounds are, but it did match flag and depth,
            // then we can trust that this score is more accurate than the current static evaluation,
            // and we can update our static evaluation for better accuracy in pruning
            if (ttHit && ttElementType != (ttScore > staticEval ? NodeType.Alpha : NodeType.Beta))
            {
                staticEval = ttScore;
            }

            bool isNotGettingCheckmated = staticEval > EvaluationConstants.NegativeCheckmateDetectionLimit;

            // Fail-high pruning (moves with high scores) - prune more when improving
            if (isNotGettingCheckmated && !isVerifyingSE)
            {
                if (depth <= Configuration.EngineSettings.RFP_MaxDepth)
                {
                    // 🔍 Reverse Futility Pruning (RFP) - https://www.chessprogramming.org/Reverse_Futility_Pruning
                    // Return formula by Ciekce, instead of just returning static eval
                    // Improving impl. based on Potential's
                    var rfpMargin = improving ? 80 * (depth - 1) : 100 * depth;
                    var improvingFactor = improvingRate * (0.75 * depth);

                    var rfpThreshold = rfpMargin + improvingFactor;

                    if (staticEval - rfpThreshold >= beta)
                    {
#pragma warning disable S3949 // Calculations should not overflow - value is being set at the beginning of the else if (!pvNode)
                        return (staticEval + beta) / 2;
#pragma warning restore S3949 // Calculations should not overflow
                    }

                    // 🔍 Razoring - Strelka impl (CPW) - https://www.chessprogramming.org/Razoring#Strelka
                    if (depth <= Configuration.EngineSettings.Razoring_MaxDepth)
                    {
                        var score = staticEval + Configuration.EngineSettings.Razoring_Depth1Bonus;

                        if (score < beta)               // Static evaluation + bonus indicates fail-low node
                        {
                            if (depth == 1)
                            {
                                var qSearchScore = QuiescenceSearch(ply, alpha, beta, pvNode, cancellationToken);

                                return qSearchScore > score
                                    ? qSearchScore
                                    : score;
                            }

                            score += Configuration.EngineSettings.Razoring_NotDepth1Bonus;

                            if (score < beta)               // Static evaluation indicates fail-low node
                            {
                                var qSearchScore = QuiescenceSearch(ply, alpha, beta, pvNode, cancellationToken);
                                if (qSearchScore < beta)    // Quiescence score also indicates fail-low node
                                {
                                    return qSearchScore > score
                                        ? qSearchScore
                                        : score;
                                }
                            }
                        }
                    }
                }

                var staticEvalBetaDiff = staticEval - beta;

                // 🔍 Null Move Pruning (NMP) - our position is so good that we can potentially afford giving our opponent a double move and still remain ahead of beta
                if (depth >= Configuration.EngineSettings.NMP_MinDepth
                    && staticEvalBetaDiff >= 0
                    && !parentWasNullMove
                    && phase > 2   // Zugzwang risk reduction: pieces other than pawn presents
                    && (ttElementType != NodeType.Alpha || ttScore >= beta))   // TT suggests NMP will fail: entry must not be a fail-low entry with a score below beta - Stormphrax and Ethereal
                {
                    var nmpReduction = Configuration.EngineSettings.NMP_BaseDepthReduction
                        + ((depth + Configuration.EngineSettings.NMP_DepthIncrement) / Configuration.EngineSettings.NMP_DepthDivisor)   // Clarity
                        + Math.Min(
                            Configuration.EngineSettings.NMP_StaticEvalBetaMaxReduction,
                            staticEvalBetaDiff / Configuration.EngineSettings.NMP_StaticEvalBetaDivisor);

                    // TODO more advanced adaptative reduction, similar to what Ethereal and Stormphrax are doing
                    //var nmpReduction = Math.Min(
                    //    depth,
                    //    3 + (depth / 3) + Math.Min((staticEval - beta) / 200, 3));

                    var gameState = position.MakeNullMove();
                    var nmpScore = -NegaMax(depth - 1 - nmpReduction, ply + 1, -beta, -beta + 1, !cutnode, cancellationToken, parentWasNullMove: true);
                    position.UnMakeNullMove(gameState);

                    if (nmpScore >= beta)
                    {
                        return nmpScore;
                    }
                }
            }
        }
        else
        {
            rawStaticEval = position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove, _pawnEvalTable).Score;
            staticEval = CorrectStaticEvaluation(position, rawStaticEval);

            if (!ttHit)
            {
                _tt.SaveStaticEval(position, Game.HalfMovesWithoutCaptureOrPawnMove, rawStaticEval, ttPv);
            }
        }

        Debug.Assert(depth >= 0, "Assertion failed", "QSearch should have been triggered");

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, moves);

        Span<int> moveScores = stackalloc int[pseudoLegalMoves.Length];

        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            moveScores[i] = ScoreMove(pseudoLegalMoves[i], ply, ttBestMove);
        }

        var nodeType = NodeType.Alpha;
        int bestScore = EvaluationConstants.MinEval;
        Move? bestMove = null;
        bool isAnyMoveValid = false;

        Span<Move> visitedMoves = stackalloc Move[pseudoLegalMoves.Length];
        int visitedMovesCounter = 0;

        for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Length; ++moveIndex)
        {
            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            for (int j = moveIndex + 1; j < pseudoLegalMoves.Length; j++)
            {
                if (moveScores[j] > moveScores[moveIndex])
                {
                    (moveScores[moveIndex], moveScores[j], pseudoLegalMoves[moveIndex], pseudoLegalMoves[j]) = (moveScores[j], moveScores[moveIndex], pseudoLegalMoves[j], pseudoLegalMoves[moveIndex]);
                }
            }

            var move = pseudoLegalMoves[moveIndex];
            var isBestMove = (ShortMove)move == ttBestMove;
            if (isVerifyingSE && isBestMove)
            {
                continue;
            }

            var moveScore = moveScores[moveIndex];
            var isCapture = move.IsCapture();

            int? quietHistory = null;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            int QuietHistory() => quietHistory ??=
                _quietHistory[move.Piece()][move.TargetSquare()]
                + ContinuationHistoryEntry(move.Piece(), move.TargetSquare(), ply - 1);

            // If we prune while getting checmated, we risk not finding any move and having an empty PV
            bool isNotGettingCheckmated = bestScore > EvaluationConstants.NegativeCheckmateDetectionLimit;

            // Fail-low pruning (moves with low scores) - prune less when improving
            // LMP, HP and FP can happen either before after MakeMove
            // PVS SEE pruning needs to happen before MakeMove in a make-unmake framework (it needs original position)
            if (visitedMovesCounter > 0
                && !pvNode
                && !isInCheck
                && isNotGettingCheckmated
                && moveScore < EvaluationConstants.PromotionMoveScoreValue) // Quiet or bad capture
            {
                // 🔍 Late Move Pruning (LMP) - all quiet moves can be pruned
                // after searching the first few given by the move ordering algorithm
                if (moveIndex >= Configuration.EngineSettings.LMP_BaseMovesToTry + (Configuration.EngineSettings.LMP_MovesDepthMultiplier * depth * (improving ? 2 : 1))) // Based on formula suggested by Antares
                {
                    break;
                }

                // 🔍 History pruning -  all quiet moves can be pruned
                // once we find one with a history score too low
                if (!isCapture
                    && moveScore < EvaluationConstants.CounterMoveValue
                    && depth < Configuration.EngineSettings.HistoryPrunning_MaxDepth    // TODO use LMR depth
                    && QuietHistory() < Configuration.EngineSettings.HistoryPrunning_Margin * (depth - 1))
                {
                    break;
                }

                // 🔍 Futility Pruning (FP) - all quiet moves can be pruned
                // once it's considered that they don't have potential to raise alpha
                if (depth <= Configuration.EngineSettings.FP_MaxDepth
                    && staticEval + Configuration.EngineSettings.FP_Margin + (Configuration.EngineSettings.FP_DepthScalingFactor * depth) <= alpha)
                {
                    break;
                }

                // 🔍 PVS SEE pruning
                if (isCapture)
                {
                    var threshold = Configuration.EngineSettings.PVS_SEE_Threshold_Noisy * depth * depth;

                    if (!SEE.IsGoodCapture(position, move, threshold))
                    {
                        continue;
                    }
                }
                else
                {
                    var threshold = Configuration.EngineSettings.PVS_SEE_Threshold_Quiet * depth;

                    if (!SEE.HasPositiveScore(position, move, threshold))
                    {
                        continue;
                    }
                }
            }

            var gameState = position.MakeMove(move);

            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            int singularDepthExtensions = 0;

            // 🔍 Singular extensions (SE) - extend TT move when it looks better than every other move
            // We check if that's the case by doing a reduced-depth search, excluding TT move and with
            // zero-depth search (using TT score-based alpha/beta values).
            // If that search fails low, the move is 'singular' (very good) and therefore we extend it
            if (
                //!isVerifyingSE        // Implicit, otherwise the move would have been skipped already
                isBestMove      // Ensures !isRoot and TT hit (otherwise there wouldn't be a TT move)
                && depth >= Configuration.EngineSettings.SE_MinDepth
                && ttDepth + Configuration.EngineSettings.SE_TTDepthOffset >= depth
                //&& Math.Abs(ttScore) < EvaluationConstants.PositiveCheckmateDetectionLimit
                && ttElementType != NodeType.Alpha)
            {
                position.UnmakeMove(move, gameState);

                var verificationDepth = (depth + depthExtension - 1) / 2;    // TODO tune?
                var singularBeta = ttScore - (depth * Configuration.EngineSettings.SE_DepthMultiplier);
                singularBeta = Math.Max(EvaluationConstants.NegativeCheckmateDetectionLimit, singularBeta);

                var singularScore = NegaMax(verificationDepth, ply, singularBeta - 1, singularBeta, cutnode, cancellationToken, isVerifyingSE: true);

                // Singular extension
                if (singularScore < singularBeta)
                {
                    ++singularDepthExtensions;

                    // Double extension
                    if (!pvNode
                        && singularScore + Configuration.EngineSettings.SE_DoubleExtensions_Margin < singularBeta)
                    {
                        ++singularDepthExtensions;
                    }
                }
                // Multicut
                else if (singularBeta >= beta)
                {
                    return singularBeta;
                }
                // Negative extension
                else if (ttScore >= beta)
                {
                    --singularDepthExtensions;
                }

                gameState = position.MakeMove(move);
            }

            var previousNodes = _nodes;
            visitedMoves[visitedMovesCounter] = move;

            ++_nodes;
            isAnyMoveValid = true;

            PrintPreMove(position, ply, move);

            // Before making a move
            var oldHalfMovesWithoutCaptureOrPawnMove = Game.HalfMovesWithoutCaptureOrPawnMove;
            var canBeRepetition = Game.Update50movesRule(move, isCapture);
            Game.AddToPositionHashHistory(position.UniqueIdentifier);
            stack.Move = move;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void RevertMove()
            {
                Game.HalfMovesWithoutCaptureOrPawnMove = oldHalfMovesWithoutCaptureOrPawnMove;
                Game.RemoveFromPositionHashHistory();
                position.UnmakeMove(move, gameState);
            }

            int score = 0;

            if (canBeRepetition && (Game.IsThreefoldRepetition() || Game.Is50MovesRepetition()))
            {
                score = 0;

                // We don't need to evaluate further down to know it's a draw.
                // Since we won't be evaluating further down, we need to clear the PV table because those moves there
                // don't belong to this line and if this move were to beat alpha, they'd incorrectly copied to pv line.
                Array.Clear(_pVTable, nextPvIndex, _pVTable.Length - nextPvIndex);
            }
            else
            {
                var nextHalfMovesCounter = (move.IsCapture() || move.Piece() == (int)Piece.P || move.Piece() == (int)Piece.p)
                    ? 0
                    : Game.HalfMovesWithoutCaptureOrPawnMove + 1;

                _tt.PrefetchTTEntry(position, nextHalfMovesCounter);

                bool isCutNode = !pvNode && !cutnode;   // Linter 'simplification' of pvNode ? false : !cutnode

                var newDepth = depth + depthExtension - 1 + singularDepthExtensions;

                // 🔍 Late Move Reduction (LMR) - search with reduced depth
                // Impl. based on Ciekce (Stormphrax) and Martin (Motor) advice, and Stormphrax & Akimbo implementations
                if (visitedMovesCounter > 0)
                {
                    int reduction = 0;

                    if (isNotGettingCheckmated)
                    {
                        if (depth >= Configuration.EngineSettings.LMR_MinDepth
                            && visitedMovesCounter >=
                                (pvNode
                                    ? Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves_PV
                                    : Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves_NonPV))
                        {
                            if (isCapture)
                            {
                                reduction = EvaluationConstants.LMRReductions[1][depth][visitedMovesCounter];

                                reduction /= EvaluationConstants.LMRScaleFactor;

                                // ~ history/(0.75 * maxHistory/2/)
                                reduction -= CaptureHistoryEntry(move) / Configuration.EngineSettings.LMR_History_Divisor_Noisy;
                            }
                            else
                            {
                                reduction = EvaluationConstants.LMRReductions[0][depth][visitedMovesCounter]
                                    + Configuration.EngineSettings.LMR_Quiet;    // Quiet LMR

                                if (!improving)
                                {
                                    reduction += Configuration.EngineSettings.LMR_Improving;
                                }

                                if (cutnode)
                                {
                                    reduction += Configuration.EngineSettings.LMR_Cutnode;
                                }

                                if (!ttPv)
                                {
                                    reduction += Configuration.EngineSettings.LMR_TTPV;
                                }

                                if (ttMoveIsCapture)    // Move isn't a capture but TT move is
                                {
                                    reduction += Configuration.EngineSettings.LMR_TTCapture;
                                }

                                if (pvNode)
                                {
                                    reduction -= Configuration.EngineSettings.LMR_PVNode;
                                }

                                if (position.IsInCheck())   // i.e. move gives check
                                {
                                    reduction -= Configuration.EngineSettings.LMR_InCheck;
                                }

                                reduction /= EvaluationConstants.LMRScaleFactor;

                                // -= history/(maxHistory/2)

                                reduction -= QuietHistory() / Configuration.EngineSettings.LMR_History_Divisor_Quiet;
                            }
                        }

                        // 🔍 Static Exchange Evaluation (SEE) reduction
                        // Bad captures are reduced more
                        // Last attempt to move it inside of LMR conditions was https://github.com/lynx-chess/Lynx/pull/1589
                        if (!isInCheck
                            && moveScore < EvaluationConstants.PromotionMoveScoreValue
                            && moveScore >= EvaluationConstants.BadCaptureMoveBaseScoreValue)
                        {
                            reduction += Configuration.EngineSettings.SEE_BadCaptureReduction;
                        }

                        // Don't allow LMR to drop into qsearch or increase the depth: min depth 1
                        // (depth - 1) - depth + 2 = 1, min depth we want
                        // newDepth - newDepth + 1 = 1, min depth we want
                        reduction = Math.Max(0, Math.Min(reduction, newDepth - 1));
                    }

                    var reducedDepth = newDepth - reduction;

                    // Search with reduced depth and zero window
                    score = -NegaMax(reducedDepth, ply + 1, -alpha - 1, -alpha, cutnode: true, cancellationToken);

                    // 🔍 Principal Variation Search (PVS)
                    if (score > alpha && newDepth > reducedDepth)
                    {
                        // Optimistic search, validating that the rest of the moves are worse than bestmove.
                        // It should produce more cutoffs and therefore be faster.
                        // https://web.archive.org/web/20071030220825/http://www.brucemo.com/compchess/programming/pvs.htm

                        var deeper = score > bestScore + Configuration.EngineSettings.LMR_DeeperBase + (Configuration.EngineSettings.LMR_DeeperDepthMultiplier * depth);
                        var shallower = score < bestScore + depth;

                        if (deeper && !shallower && depth < Configuration.EngineSettings.MaxDepth)
                        {
                            ++newDepth;
                        }
                        else if (shallower && !deeper && newDepth > 1)
                        {
                            --newDepth;
                        }

                        if (newDepth > reducedDepth)
                        {
                            // Search with full depth but narrowed score bandwidth (zero-window search)
                            score = -NegaMax(newDepth, ply + 1, -alpha - 1, -alpha, !cutnode, cancellationToken);
                        }
                    }
                }

                // First searched move is always searched with full depth and full score bandwidth
                // Same if PVS hypothesis is invalidated
                if (visitedMovesCounter == 0 || (score > alpha && score < beta))
                {
#pragma warning disable S2234 // Arguments should be passed in the same order as the method parameters
                    score = -NegaMax(newDepth, ply + 1, -beta, -alpha, cutnode: false, cancellationToken);
#pragma warning restore S2234 // Arguments should be passed in the same order as the method parameters
                }
            }

            // After making a move
            RevertMove();
            if (isRoot)
            {
                var nodesSpentInThisMove = _nodes - previousNodes;
                UpdateMoveNodeCount(move, nodesSpentInThisMove);
            }

            PrintMove(position, ply, move, score);

            if (score > bestScore)
            {
                bestScore = score;

                // Improving alpha
                if (score > alpha)
                {
                    alpha = score;
                    bestMove = move;

                    if (pvNode)
                    {
                        _pVTable[pvIndex] = move;
                        CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - ply - 1);
                    }

                    nodeType = NodeType.Exact;
                }

                // Beta-cutoff - refutation found, no need to keep searching this line
                if (score >= beta)
                {
                    PrintMessage($"Pruning: {move} is enough");

                    var historyDepth = depth;

                    if (staticEval <= alpha)
                    {
                        ++historyDepth;
                    }

                    // Suggestion by Sirius author
                    if (bestScore >= beta + Configuration.EngineSettings.History_BestScoreBetaMargin)
                    {
                        ++historyDepth;
                    }

                    if (isCapture)
                    {
                        UpdateMoveOrderingHeuristicsOnCaptureBetaCutoff(historyDepth, visitedMoves, visitedMovesCounter, move);
                    }
                    else
                    {
                        UpdateMoveOrderingHeuristicsOnQuietBetaCutoff(historyDepth, ply, visitedMoves, visitedMovesCounter, move, isRoot, pvNode);
                    }

                    nodeType = NodeType.Beta;

                    break;
                }
            }

            ++visitedMovesCounter;
        }

        if (!isAnyMoveValid)
        {
            Debug.Assert(bestMove is null);

            bestScore = Position.EvaluateFinalPosition(ply, isInCheck);

            nodeType = NodeType.Exact;
            staticEval = bestScore;
        }

        if (!isVerifyingSE)
        {
            if (!(isInCheck
                || bestMove?.IsCapture() == true
                || bestMove?.IsPromotion() == true
                || (ttElementType == NodeType.Beta && bestScore <= staticEval)
                || (ttElementType == NodeType.Alpha && bestScore >= staticEval)))
            {
                UpdateCorrectionHistory(position, bestScore - staticEval, depth);
            }

            _tt.RecordHash(position, Game.HalfMovesWithoutCaptureOrPawnMove, rawStaticEval, depth, ply, bestScore, nodeType, ttPv, bestMove);
        }

        return bestScore;
    }

    /// <summary>
    /// Quiescence search implementation, NegaMax alpha-beta style, fail-soft
    /// </summary>
    /// <param name="alpha">
    /// Best score White can achieve, assuming best play by Black.
    /// Defaults to the worse possible score for white, Int.MinValue.
    /// </param>
    /// <param name="beta">
    /// Best score Black can achieve, assuming best play by White
    /// Defaults to the works possible score for Black, Int.MaxValue
    /// </param>
    [SkipLocalsInit]
    public int QuiescenceSearch(int ply, int alpha, int beta, bool pvNode, CancellationToken cancellationToken)
    {
        var position = Game.CurrentPosition;

        cancellationToken.ThrowIfCancellationRequested();

        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            if (IsMainEngine)
            {
                _logger.Debug("[#{EngineId}] Max depth {Depth} reached in qsearch - position {FEN}",
                _id, Configuration.EngineSettings.MaxDepth, position.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));
            }
#if MULTITHREAD_DEBUG
            else
            {
                _logger.Trace("[#{EngineId}] Max depth {Depth} reached in qsearch - position {FEN}",
                _id, Configuration.EngineSettings.MaxDepth, position.FEN(Game.HalfMovesWithoutCaptureOrPawnMove));
            }
#endif
            return position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove, _pawnEvalTable).Score;
        }

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        var ttProbeResult = _tt.ProbeHash(position, Game.HalfMovesWithoutCaptureOrPawnMove, ply);
        var ttScore = ttProbeResult.Score;
        var ttNodeType = ttProbeResult.NodeType;
        var ttHit = ttNodeType != NodeType.Unknown && ttNodeType != NodeType.None;
        var ttPv = pvNode || ttProbeResult.WasPv;

        // QS TT cutoff
        Debug.Assert(ttProbeResult.Depth >= 0, "Assertion failed", "We would need to add it as a TT cutoff condition");

        if (ttHit
            && (ttNodeType == NodeType.Exact
                || (ttNodeType == NodeType.Alpha && ttScore <= alpha)
                || (ttNodeType == NodeType.Beta && ttScore >= beta)))
        {
            return ttScore;
        }

        ShortMove ttBestMove = ttProbeResult.BestMove;
        _maxDepthReached[ply] = ply;

        /*
            var staticEval = ttHit
                ? ttProbeResult.StaticEval
                : position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove, _kingPawnHashTable).Score;
        */
        var rawStaticEval = position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove, _pawnEvalTable).Score;
        Debug.Assert(rawStaticEval != EvaluationConstants.NoHashEntry, "Assertion failed", "All TT entries should have a static eval");

        var staticEval = CorrectStaticEvaluation(position, rawStaticEval);

        ref var stack = ref Game.Stack(ply);
        stack.StaticEval = staticEval;

        int standPat =
            (ttNodeType == NodeType.Exact
                || (ttNodeType == NodeType.Alpha && ttScore < staticEval)
                || (ttNodeType == NodeType.Beta && ttScore > staticEval))
            ? ttScore
            : staticEval;

        var isInCheck = position.IsInCheck();

        if (!isInCheck)
        {
            if (!ttHit)
            {
                _tt.SaveStaticEval(position, Game.HalfMovesWithoutCaptureOrPawnMove, rawStaticEval, ttPv);
            }

            // Standing pat beta-cutoff (updating alpha after this check)
            if (standPat >= beta)
            {
                PrintMessage(ply - 1, "Pruning before starting quiescence search");
                return standPat;
            }
        }

        // Better move
        if (standPat > alpha)
        {
            alpha = standPat;
        }

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var pseudoLegalMoves = MoveGenerator.GenerateAllCaptures(position, moves);
        if (pseudoLegalMoves.Length == 0)
        {
            // Checking if final position first: https://github.com/lynx-chess/Lynx/pull/358
            return staticEval;
        }

        var nodeType = NodeType.Alpha;
        Move? bestMove = null;
        int bestScore = standPat;

        bool isAnyCaptureValid = false;

        Span<int> moveScores = stackalloc int[pseudoLegalMoves.Length];
        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            moveScores[i] = ScoreMoveQSearch(pseudoLegalMoves[i], ttBestMove);
        }

        Span<Move> visitedMoves = stackalloc Move[pseudoLegalMoves.Length];
        int visitedMovesCounter = 0;

        for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Length; ++moveIndex)
        {
            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            for (int j = moveIndex + 1; j < pseudoLegalMoves.Length; j++)
            {
                if (moveScores[j] > moveScores[moveIndex])
                {
                    (moveScores[moveIndex], moveScores[j], pseudoLegalMoves[moveIndex], pseudoLegalMoves[j]) = (moveScores[j], moveScores[moveIndex], pseudoLegalMoves[j], pseudoLegalMoves[moveIndex]);
                }
            }

            var move = pseudoLegalMoves[moveIndex];
            var moveScore = moveScores[moveIndex];

            // 🔍 QSearch SEE pruning: pruning bad captures
            if (moveScore < EvaluationConstants.PromotionMoveScoreValue && moveScore >= EvaluationConstants.BadCaptureMoveBaseScoreValue)
            {
                continue;
            }

            var gameState = position.MakeMove(move);
            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            ++_nodes;
            visitedMoves[visitedMovesCounter] = move;
            isAnyCaptureValid = true;

            PrintPreMove(position, ply, move, isQuiescence: true);

            // No need to check for threefold or 50 moves repetitions, since we're only searching captures, promotions, and castles
            stack.Move = move;

#pragma warning disable S2234 // Arguments should be passed in the same order as the method parameters
            int score = -QuiescenceSearch(ply + 1, -beta, -alpha, pvNode, cancellationToken);
#pragma warning restore S2234 // Arguments should be passed in the same order as the method parameters
            position.UnmakeMove(move, gameState);

            PrintMove(position, ply, move, score, isQuiescence: true);

            if (score > bestScore)
            {
                bestScore = score;

                // Beta-cutoff
                if (score >= beta)
                {
                    PrintMessage($"Pruning: {move} is enough to discard this line");

                    if (move.IsCapture())
                    {
                        UpdateMoveOrderingHeuristicsOnCaptureBetaCutoff(3, visitedMoves, visitedMovesCounter, move);
                    }

                    nodeType = NodeType.Beta;
                    break;
                }

                // Improving alpha
                if (score > alpha)
                {
                    alpha = score;
                    bestMove = move;

                    _pVTable[pvIndex] = move;
                    CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - ply - 1);

                    nodeType = NodeType.Exact;
                }
            }

            ++visitedMovesCounter;
        }

        if (!isAnyCaptureValid
            && !MoveGenerator.CanGenerateAtLeastAValidMove(position)) // Bad captures can be pruned, so all moves need to be generated for now
        {
            Debug.Assert(bestMove is null);

            bestScore = Position.EvaluateFinalPosition(ply, position.IsInCheck());

            nodeType = NodeType.Exact;
            staticEval = bestScore;
        }

        _tt.RecordHash(position, Game.HalfMovesWithoutCaptureOrPawnMove, rawStaticEval, 0, ply, bestScore, nodeType, ttPv, bestMove);

        return bestScore;
    }
}
