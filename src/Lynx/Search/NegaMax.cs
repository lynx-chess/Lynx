using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx;

public sealed partial class Engine
{
    /// <summary>
    /// NegaMax algorithm implementation using alpha-beta pruning and quiescence search
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="ply"></param>
    /// <param name="alpha">
    /// Best score the Side to move can achieve, assuming best play by the opponent.
    /// </param>
    /// <param name="beta">
    /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
    /// </param>
    /// <returns></returns>
    [SkipLocalsInit]
    private int NegaMax(int depth, int ply, int alpha, int beta, bool parentWasNullMove = false)
    {
        var position = Game.CurrentPosition;

        // Prevents runtime failure in case depth is increased due to check extension, since we're using ply when calculating pvTable index,
        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth {0} reached", Configuration.EngineSettings.MaxDepth);
            return position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove).Score;
        }

        _maxDepthReached[ply] = ply;
        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        bool isRoot = ply == 0;
        bool pvNode = beta - alpha > 1;
        ShortMove ttBestMove = default;
        NodeType ttElementType = default;
        int ttScore = default;
        int ttRawScore = default;

        if (!isRoot)
        {
            (ttScore, ttBestMove, ttElementType, ttRawScore) = _tt.ProbeHash(_ttMask, position, depth, ply, alpha, beta);
            if (!pvNode && ttScore != EvaluationConstants.NoHashEntry)
            {
                if (ttScore <= EvaluationConstants.MinEval)
                {
                    _logger.Debug("Returning {MinEval} from TT at depth {Depth}", ttScore, depth);
                }

                return ttScore;
            }

            // Internal iterative reduction (IIR)
            // If this position isn't found in TT, it has never been searched before,
            // so the search will be potentially expensive.
            // Therefore, we search with reduced depth for now, expecting to record a TT move
            // which we'll be able to use later for the full depth search
            if (ttElementType == default && depth >= Configuration.EngineSettings.IIR_MinDepth)
            {
                --depth;
            }
        }

        // Before any time-consuming operations
        _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        bool isInCheck = position.IsInCheck();
        int staticEval = int.MaxValue, phase = int.MaxValue;

        if (isInCheck)
        {
            ++depth;
        }
        else if (depth <= 0)
        {
            if (MoveGenerator.CanGenerateAtLeastAValidMove(position))
            {
                var qsearchScore = QuiescenceSearch(ply, alpha, beta);

                if (qsearchScore <= EvaluationConstants.MinEval)
                {
                    _logger.Debug("Returning {MinEval} from QSearch call at depth {Depth}", qsearchScore, depth);
                }

                return qsearchScore;
            }

            var finalPositionEvaluation = Position.EvaluateFinalPosition(ply, isInCheck);
            _tt.RecordHash(_ttMask, position, depth, ply, finalPositionEvaluation, NodeType.Exact);


            if (finalPositionEvaluation <= EvaluationConstants.MinEval)
            {
                _logger.Debug("[QUI] Returning {MinEval} from final position evaluation 0 at depth {Depth} at ply {Ply}", finalPositionEvaluation, depth, ply);
            }
            return finalPositionEvaluation;
        }
        else if (!pvNode)
        {
            (staticEval, phase) = position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove);

            // From smol.cs
            // ttEvaluation can be used as a better positional evaluation:
            // If the score is outside what the current bounds are, but it did match flag and depth,
            // then we can trust that this score is more accurate than the current static evaluation,
            // and we can update our static evaluation for better accuracy in pruning
            if (ttElementType != default && ttElementType != (ttRawScore > staticEval ? NodeType.Alpha : NodeType.Beta))
            {
                staticEval = ttRawScore;
            }

            if (depth <= Configuration.EngineSettings.RFP_MaxDepth)
            {
                // 🔍 Reverse Futility Pruning (RFP) - https://www.chessprogramming.org/Reverse_Futility_Pruning
                // Return formula by Ciekce, instead of just returning static eval
                if (staticEval - (Configuration.EngineSettings.RFP_DepthScalingFactor * depth) >= beta)
                {
#pragma warning disable S3949 // Calculations should not overflow - value is being set at the beginning of the else if (!pvNode)
                    int rfpScore = (staticEval + beta) / 2;

                    if (rfpScore <= EvaluationConstants.MinEval)
                    {
                        _logger.Debug("Returning {MinEval} from RFP at depth {Depth}", rfpScore, depth);
                    }

                    return rfpScore;
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
                            var qSearchScore = QuiescenceSearch(ply, alpha, beta);

                            var razoringScore = qSearchScore > score
                                ? qSearchScore
                                : score;

                            if (razoringScore <= EvaluationConstants.MinEval)
                            {
                                _logger.Debug("Returning {MinEval} from Razoring at depth {Depth}", razoringScore, depth);
                            }

                            return razoringScore;
                        }

                        score += Configuration.EngineSettings.Razoring_NotDepth1Bonus;

                        if (score < beta)               // Static evaluation indicates fail-low node
                        {
                            var qSearchScore = QuiescenceSearch(ply, alpha, beta);
                            if (qSearchScore < beta)    // Quiescence score also indicates fail-low node
                            {
                                var razoringScore = qSearchScore > score
                                    ? qSearchScore
                                    : score;

                                if (razoringScore <= EvaluationConstants.MinEval)
                                {
                                    _logger.Debug("Returning {MinEval} from Razoring II at depth {Depth}", razoringScore, depth);
                                }

                                return razoringScore;
                            }
                        }
                    }
                }
            }

            // 🔍 Null Move Pruning (NMP) - our position is so good that we can potentially afford giving our opponent a double move and still remain ahead of beta
            if (depth >= Configuration.EngineSettings.NMP_MinDepth
                && staticEval >= beta
                && !parentWasNullMove
                && phase > 2   // Zugzwang risk reduction: pieces other than pawn presents
                && (ttElementType != NodeType.Alpha || ttScore >= beta))   // TT suggests NMP will fail: entry must not be a fail-low entry with a score below beta - Stormphrax and Ethereal
            {
                var nmpReduction = Configuration.EngineSettings.NMP_BaseDepthReduction + ((depth + Configuration.EngineSettings.NMP_DepthIncrement) / Configuration.EngineSettings.NMP_DepthDivisor);   // Clarity

                // TODO more advanced adaptative reduction, similar to what Ethereal and Stormphrax are doing
                //var nmpReduction = Math.Min(
                //    depth,
                //    3 + (depth / 3) + Math.Min((staticEval - beta) / 200, 3));

                var gameState = position.MakeNullMove();
                var nmpScore = -NegaMax(depth - 1 - nmpReduction, ply + 1, -beta, -beta + 1, parentWasNullMove: true);
                position.UnMakeNullMove(gameState);

                if (nmpScore >= beta)
                {
                    if (nmpScore <= EvaluationConstants.MinEval)
                    {
                        _logger.Debug("Returning {MinEval} from NMP at depth {Depth}", nmpScore, depth);
                    }

                    return nmpScore;
                }
            }
        }

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, moves);

        Span<int> moveScores = stackalloc int[pseudoLegalMoves.Length];

        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            moveScores[i] = ScoreMove(pseudoLegalMoves[i], ply, isNotQSearch: true, ttBestMove);
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

            var gameState = position.MakeMove(move);

            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            visitedMoves[visitedMovesCounter] = move;

            ++_nodes;
            isAnyMoveValid = true;
            var isCapture = move.IsCapture();

            PrintPreMove(position, ply, move);

            // Before making a move
            var oldHalfMovesWithoutCaptureOrPawnMove = Game.HalfMovesWithoutCaptureOrPawnMove;
            var canBeRepetition = Game.Update50movesRule(move, isCapture);
            Game.AddToPositionHashHistory(position.UniqueIdentifier);
            Game.PushToMoveStack(ply, move);

            int score;
            if (canBeRepetition && (Game.IsThreefoldRepetition() || Game.Is50MovesRepetition()))
            {
                score = 0;

                // We don't need to evaluate further down to know it's a draw.
                // Since we won't be evaluating further down, we need to clear the PV table because those moves there
                // don't belong to this line and if this move were to beat alpha, they'd incorrectly copied to pv line.
                Array.Clear(_pVTable, nextPvIndex, _pVTable.Length - nextPvIndex);
            }
            else if (visitedMovesCounter == 0)
            {
                PrefetchTTEntry();
#pragma warning disable S2234 // Arguments should be passed in the same order as the method parameters
                score = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
#pragma warning restore S2234 // Arguments should be passed in the same order as the method parameters

                if (score <= EvaluationConstants.MinEval)
                {
                    _logger.Debug("Full search in first visited move returned {MinEval}", score);
                }
            }
            else
            {
                // If we prune while getting checkmated, we risk not finding any move and having an empty PV
                bool isNotGettingCheckmated = bestScore > EvaluationConstants.NegativeCheckmateDetectionLimit;

                if (!pvNode && !isInCheck && isNotGettingCheckmated
                    && moveScores[moveIndex] < EvaluationConstants.PromotionMoveScoreValue) // Quiet move
                {
                    // Late Move Pruning (LMP) - all quiet moves can be pruned
                    // after searching the first few given by the move ordering algorithm
                    if (depth <= Configuration.EngineSettings.LMP_MaxDepth
                        && moveIndex >= Configuration.EngineSettings.LMP_BaseMovesToTry + (Configuration.EngineSettings.LMP_MovesDepthMultiplier * depth)) // Based on formula suggested by Antares
                    {
                        // After making a move
                        Game.HalfMovesWithoutCaptureOrPawnMove = oldHalfMovesWithoutCaptureOrPawnMove;
                        Game.RemoveFromPositionHashHistory();
                        position.UnmakeMove(move, gameState);

                        break;
                    }

                    // Futility Pruning (FP) - all quiet moves can be pruned
                    // once it's considered that they don't have potential to raise alpha
                    if (visitedMovesCounter > 0
                        //&& alpha < EvaluationConstants.PositiveCheckmateDetectionLimit
                        //&& beta > EvaluationConstants.NegativeCheckmateDetectionLimit
                        && depth <= Configuration.EngineSettings.FP_MaxDepth
                        && staticEval + Configuration.EngineSettings.FP_Margin + (Configuration.EngineSettings.FP_DepthScalingFactor * depth) <= alpha)
                    {
                        // After making a move
                        Game.HalfMovesWithoutCaptureOrPawnMove = oldHalfMovesWithoutCaptureOrPawnMove;
                        Game.RemoveFromPositionHashHistory();
                        position.UnmakeMove(move, gameState);

                        break;
                    }
                }

                PrefetchTTEntry();

                int reduction = 0;

                // 🔍 Late Move Reduction (LMR) - search with reduced depth
                // Impl. based on Ciekce (Stormphrax) and Martin (Motor) advice, and Stormphrax & Akimbo implementations
                if (visitedMovesCounter >= (pvNode ? Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves : Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves - 1)
                    && depth >= Configuration.EngineSettings.LMR_MinDepth
                    && !isCapture)
                {
                    reduction = EvaluationConstants.LMRReductions[depth][visitedMovesCounter];

                    if (pvNode)
                    {
                        --reduction;
                    }
                    if (position.IsInCheck())   // i.e. move gives check
                    {
                        --reduction;
                    }

                    if (ttBestMove != default && isCapture)
                    {
                        ++reduction;
                    }

                    // -= history/(maxHistory/2)
                    reduction -= 2 * _quietHistory[move.Piece()][move.TargetSquare()] / Configuration.EngineSettings.History_MaxMoveValue;

                    // Don't allow LMR to drop into qsearch or increase the depth
                    // depth - 1 - depth +2 = 1, min depth we want
                    reduction = Math.Clamp(reduction, 0, depth - 2);
                }

                // 🔍 Static Exchange Evaluation (SEE) reduction
                // Bad captures are reduced more
                if (!isInCheck
                    && moveScores[moveIndex] < EvaluationConstants.PromotionMoveScoreValue
                    && moveScores[moveIndex] >= EvaluationConstants.BadCaptureMoveBaseScoreValue)
                {
                    reduction += Configuration.EngineSettings.SEE_BadCaptureReduction;
                    reduction = Math.Clamp(reduction, 0, depth - 1);
                }

                if (bestScore <= EvaluationConstants.MinEval)
                {
                    _logger.Debug("Before reduced depth, [alpha = {Alpha}, beta = {Beta}], bestScore = {MinEval}", alpha, beta, bestScore);
                }

                // Search with reduced depth
                score = -NegaMax(depth - 1 - reduction, ply + 1, -alpha - 1, -alpha);

                if (score <= EvaluationConstants.MinEval)
                {
                    _logger.Debug("Reduced depth result, {Score}", score);
                }

                // 🔍 Principal Variation Search (PVS)
                if (score > alpha && reduction > 0)
                {
                    // Optimistic search, validating that the rest of the moves are worse than bestmove.
                    // It should produce more cutoffs and therefore be faster.
                    // https://web.archive.org/web/20071030220825/http://www.brucemo.com/compchess/programming/pvs.htm

                    // Search with full depth but narrowed score bandwidth
                    score = -NegaMax(depth - 1, ply + 1, -alpha - 1, -alpha);

                    if (score <= EvaluationConstants.MinEval)
                    {
                        _logger.Debug("Narrowed full depth result, {Score}", score);
                    }
                }

                if (score > alpha && score < beta)
                {
                    // PVS Hypothesis invalidated -> search with full depth and full score bandwidth
#pragma warning disable S2234 // Arguments should be passed in the same order as the method parameters
                    score = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
#pragma warning restore S2234 // Arguments should be passed in the same order as the method parameters

                    if (score <= EvaluationConstants.MinEval)
                    {
                        _logger.Debug("Full bandwith full depth result, {Score}", score);
                    }
                }
            }

            // After making a move
            // Game.PositionHashHistory is update above
            Game.HalfMovesWithoutCaptureOrPawnMove = oldHalfMovesWithoutCaptureOrPawnMove;
            Game.RemoveFromPositionHashHistory();
            position.UnmakeMove(move, gameState);

            PrintMove(position, ply, move, score);

            if (score <= EvaluationConstants.MinEval)
            {
                _logger.Debug("We somehow managed to get a score <= to minEval: {Score}", score);
                if (EvaluationConstants.MinEval == bestScore)
                {
                    _logger.Debug("Expected issue, bestScore is also minEval");
                }
            }

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

                    if (isCapture)
                    {
                        UpdateMoveOrderingHeuristicsOnCaptureBetaCutoff(depth, visitedMoves, visitedMovesCounter, move);
                    }
                    else
                    {
                        UpdateMoveOrderingHeuristicsOnQuietBetaCutoff(depth, ply, visitedMoves, visitedMovesCounter, move);
                    }

                    _tt.RecordHash(_ttMask, position, depth, ply, bestScore, NodeType.Beta, bestMove);

                    if (bestScore <= EvaluationConstants.MinEval)
                    {
                        _logger.Debug("Returning {MinEval} from Beta cutoff at depth {Depth}", bestScore, depth);
                    }

                    return bestScore;
                }
            }

            ++visitedMovesCounter;
        }

        if (!isAnyMoveValid)
        {
            Debug.Assert(bestMove is null);

            var finalEval = Position.EvaluateFinalPosition(ply, isInCheck);
            _tt.RecordHash(_ttMask, position, depth, ply, finalEval, NodeType.Exact);

            _logger.Debug("No legal moves found for {Position}, position evaluated as {FinalEval}", position.FEN(), finalEval);

            if (finalEval <= EvaluationConstants.MinEval)
            {
                _logger.Debug("Returning {MinEval} from final position evaluation at depth {Depth}", finalEval, depth);
            }

            return finalEval;
        }

        _tt.RecordHash(_ttMask, position, depth, ply, bestScore, nodeType, bestMove);

        // Node fails low

        if (bestScore <= EvaluationConstants.MinEval)
        {
            _logger.Debug("Returning {MinEval} from fail low node at depth {Depth}, ply {Ply}, after {Moves}. Alpha was {Alpha}, beta was {Beta}",
                bestScore, depth, ply, visitedMovesCounter, alpha, beta);
            _logger.Debug("Visited moves: {Moves}", string.Join(' ', visitedMoves.ToArray()));
        }

        return bestScore;
    }

    /// <summary>
    /// Quiescence search implementation, NegaMax alpha-beta style, fail-soft
    /// </summary>
    /// <param name="ply"></param>
    /// <param name="alpha">
    /// Best score White can achieve, assuming best play by Black.
    /// Defaults to the worse possible score for white, Int.MinValue.
    /// </param>
    /// <param name="beta">
    /// Best score Black can achieve, assuming best play by White
    /// Defaults to the works possible score for Black, Int.MaxValue
    /// </param>
    /// <returns></returns>
    [SkipLocalsInit]
    public int QuiescenceSearch(int ply, int alpha, int beta)
    {
        var position = Game.CurrentPosition;

        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
        _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth {0} reached", Configuration.EngineSettings.MaxDepth);
            return position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove).Score;
        }

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        var ttProbeResult = _tt.ProbeHash(_ttMask, position, 0, ply, alpha, beta);
        if (ttProbeResult.Score != EvaluationConstants.NoHashEntry)
        {
            if (ttProbeResult.Score <= EvaluationConstants.MinEval)
            {
                _logger.Debug("[QUI] Returning {MinEval} from TT at ply {Ply}", ttProbeResult.Score, ply);
            }

            return ttProbeResult.Score;
        }
        ShortMove ttBestMove = ttProbeResult.BestMove;

        _maxDepthReached[ply] = ply;

        var staticEvaluation = position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove).Score;

        // Beta-cutoff (updating alpha after this check)
        if (staticEvaluation >= beta)
        {
            PrintMessage(ply - 1, "Pruning before starting quiescence search");


            if (staticEvaluation <= EvaluationConstants.MinEval)
            {
                _logger.Debug("[QUI] Returning {MinEval} from static evaluation beta cutoff at ply {Ply}", staticEvaluation, ply);
            }
            return staticEvaluation;
        }

        // Better move
        if (staticEvaluation > alpha)
        {
            alpha = staticEvaluation;
        }

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var pseudoLegalMoves = MoveGenerator.GenerateAllCaptures(position, moves);
        if (pseudoLegalMoves.Length == 0)
        {
            // Checking if final position first: https://github.com/lynx-chess/Lynx/pull/358


            if (staticEvaluation <= EvaluationConstants.MinEval)
            {
                _logger.Debug("[QUI] Returning {MinEval} from static evaluation II (final pos) at ply {Ply}", staticEvaluation, ply);
            }

            return staticEvaluation;
        }

        var nodeType = NodeType.Alpha;
        Move? bestMove = null;
        int bestScore = staticEvaluation;

        bool isAnyCaptureValid = false;

        Span<int> moveScores = stackalloc int[pseudoLegalMoves.Length];
        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            moveScores[i] = ScoreMove(pseudoLegalMoves[i], ply, isNotQSearch: false, ttBestMove);
        }

        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            for (int j = i + 1; j < pseudoLegalMoves.Length; j++)
            {
                if (moveScores[j] > moveScores[i])
                {
                    (moveScores[i], moveScores[j], pseudoLegalMoves[i], pseudoLegalMoves[j]) = (moveScores[j], moveScores[i], pseudoLegalMoves[j], pseudoLegalMoves[i]);
                }
            }

            var move = pseudoLegalMoves[i];

            // 🔍 QSearch SEE pruning: pruning bad captures
            if (moveScores[i] < EvaluationConstants.PromotionMoveScoreValue && moveScores[i] >= EvaluationConstants.BadCaptureMoveBaseScoreValue)
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
            isAnyCaptureValid = true;

            PrintPreMove(position, ply, move, isQuiescence: true);

            // No need to check for threefold or 50 moves repetitions, since we're only searching captures, promotions, and castles
            Game.PushToMoveStack(ply, move);

#pragma warning disable S2234 // Arguments should be passed in the same order as the method parameters
            int score = -QuiescenceSearch(ply + 1, -beta, -alpha);
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

                    _tt.RecordHash(_ttMask, position, 0, ply, bestScore, NodeType.Beta, bestMove);

                    if (bestScore <= EvaluationConstants.MinEval)
                    {
                        _logger.Debug("[QUI] Returning {MinEval} from Beta cutoff at ply {Ply}", bestScore, ply);
                    }

                    return bestScore; // The refutation doesn't matter, since it'll be pruned
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
        }

        if (!isAnyCaptureValid
            && !MoveGenerator.CanGenerateAtLeastAValidMove(position)) // Bad captures can be pruned, so all moves need to be generated for now
        {
            Debug.Assert(bestMove is null);

            var finalEval = Position.EvaluateFinalPosition(ply, position.IsInCheck());
            _tt.RecordHash(_ttMask, position, 0, ply, finalEval, NodeType.Exact);

            _logger.Debug("[QUI] No legal moves found for {Position}, position evaluated as {FinalEval}", position.FEN(), finalEval);

            if (finalEval <= EvaluationConstants.MinEval)
            {
                _logger.Debug("[QUI] Returning {MinEval} from final position evaluation at ply {Ply}", finalEval, ply);
            }

            return finalEval;
        }

        _tt.RecordHash(_ttMask, position, 0, ply, bestScore, nodeType, bestMove);

        if (bestScore <= EvaluationConstants.MinEval)
        {
            _logger.Debug("[QUI] Returning {MinEval} from fail low node at ply {Ply}", bestScore, ply);
        }

        return bestScore;
    }
}
