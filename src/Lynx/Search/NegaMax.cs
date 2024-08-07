﻿using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx;

public sealed partial class Engine
{
    /// <summary>
    /// NegaMax algorithm implementation using alpha-beta pruning, quiescence search and Iterative Deepening Depth-First Search (IDDFS)
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="ply"></param>
    /// <param name="alpha">
    /// Best score the Side to move can achieve, assuming best play by the opponent.
    /// Defaults to the worse possible score for Side to move, Int.MinValue.
    /// </param>
    /// <param name="beta">
    /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
    /// Defaults to the worse possible score for Side to move's opponent, Int.MaxValue
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
            return position.StaticEvaluation().Score;
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
        int ttEvaluation = default;
        int ttScore = default;

        if (!isRoot)
        {
            (ttEvaluation, ttBestMove, ttElementType, ttScore) = _tt.ProbeHash(_ttMask, position, depth, ply, alpha, beta);
            if (!pvNode && ttEvaluation != EvaluationConstants.NoHashEntry)
            {
                return ttEvaluation;
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
                return QuiescenceSearch(ply, alpha, beta);
            }

            var finalPositionEvaluation = Position.EvaluateFinalPosition(ply, isInCheck);
            _tt.RecordHash(_ttMask, position, depth, ply, finalPositionEvaluation, NodeType.Exact);
            return finalPositionEvaluation;
        }
        else if (!pvNode)
        {
            (staticEval, phase) = position.StaticEvaluation();

            // From smol.cs
            // ttEvaluation can be used as a better positional evaluation:
            // If the score is outside what the current bounds are, but it did match flag and depth,
            // then we can trust that this score is more accurate than the current static evaluation,
            // and we can update our static evaluation for better accuracy in pruning
            if (ttElementType != default && ttElementType != (ttScore > staticEval ? NodeType.Alpha : NodeType.Beta))
            {
                staticEval = ttScore;
            }

            if (depth <= Configuration.EngineSettings.RFP_MaxDepth)
            {
                // 🔍 Reverse Futility Pruning (RFP) - https://www.chessprogramming.org/Reverse_Futility_Pruning
                // Return formula by Ciekce, instead of just returning static eval
                if (staticEval - (Configuration.EngineSettings.RFP_DepthScalingFactor * depth) >= beta)
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
                            var qSearchScore = QuiescenceSearch(ply, alpha, beta);

                            return qSearchScore > score
                                ? qSearchScore
                                : score;
                        }

                        score += Configuration.EngineSettings.Razoring_NotDepth1Bonus;

                        if (score < beta)               // Static evaluation indicates fail-low node
                        {
                            var qSearchScore = QuiescenceSearch(ply, alpha, beta);
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

            // 🔍 Null Move Pruning (NMP) - our position is so good that we can potentially afford giving our opponent a double move and still remain ahead of beta
            if (depth >= Configuration.EngineSettings.NMP_MinDepth
                && staticEval >= beta
                && !parentWasNullMove
                && phase > 2   // Zugzwang risk reduction: pieces other than pawn presents
                && (ttElementType != NodeType.Alpha || ttEvaluation >= beta))   // TT suggests NMP will fail: entry must not be a fail-low entry with a score below beta - Stormphrax and Ethereal
            {
                var nmpReduction = Configuration.EngineSettings.NMP_BaseDepthReduction + ((depth + Configuration.EngineSettings.NMP_DepthIncrement) / Configuration.EngineSettings.NMP_DepthDivisor);   // Clarity

                // TODO more advanced adaptative reduction, similar to what Ethereal and Stormphrax are doing
                //var nmpReduction = Math.Min(
                //    depth,
                //    3 + (depth / 3) + Math.Min((staticEval - beta) / 200, 3));

                var gameState = position.MakeNullMove();
                var evaluation = -NegaMax(depth - 1 - nmpReduction, ply + 1, -beta, -beta + 1, parentWasNullMove: true);
                position.UnMakeNullMove(gameState);

                if (evaluation >= beta)
                {
                    return evaluation;
                }
            }
        }

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, moves);

        Span<int> scores = stackalloc int[pseudoLegalMoves.Length];
        if (_isFollowingPV)
        {
            _isFollowingPV = false;
            for (int i = 0; i < pseudoLegalMoves.Length; ++i)
            {
                scores[i] = ScoreMove(pseudoLegalMoves[i], ply, isNotQSearch: true, ttBestMove);

                if (pseudoLegalMoves[i] == _pVTable[depth])
                {
                    _isFollowingPV = true;
                    _isScoringPV = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < pseudoLegalMoves.Length; ++i)
            {
                scores[i] = ScoreMove(pseudoLegalMoves[i], ply, isNotQSearch: true, ttBestMove);
            }
        }

        var nodeType = NodeType.Alpha;
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
                if (scores[j] > scores[moveIndex])
                {
                    (scores[moveIndex], scores[j], pseudoLegalMoves[moveIndex], pseudoLegalMoves[j]) = (scores[j], scores[moveIndex], pseudoLegalMoves[j], pseudoLegalMoves[moveIndex]);
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
            Game.PositionHashHistory.Add(position.UniqueIdentifier);
            Game.PushToMoveStack(ply, move);

            int evaluation;
            if (canBeRepetition && (Game.IsThreefoldRepetition() || Game.Is50MovesRepetition()))
            {
                evaluation = 0;

                // We don't need to evaluate further down to know it's a draw.
                // Since we won't be evaluating further down, we need to clear the PV table because those moves there
                // don't belong to this line and if this move were to beat alpha, they'd incorrectly copied to pv line.
                Array.Clear(_pVTable, nextPvIndex, _pVTable.Length - nextPvIndex);
            }
            else if (pvNode && visitedMovesCounter == 0)
            {
                PrefetchTTEntry();
                evaluation = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
            }
            else
            {
                if (!pvNode && !isInCheck
                    && scores[moveIndex] < EvaluationConstants.PromotionMoveScoreValue) // Quiet move
                {
                    // Late Move Pruning (LMP) - all quiet moves can be pruned
                    // after searching the first few given by the move ordering algorithm
                    if (depth <= Configuration.EngineSettings.LMP_MaxDepth
                        && moveIndex >= Configuration.EngineSettings.LMP_BaseMovesToTry + (Configuration.EngineSettings.LMP_MovesDepthMultiplier * depth)) // Based on formula suggested by Antares
                    {
                        // After making a move
                        Game.HalfMovesWithoutCaptureOrPawnMove = oldHalfMovesWithoutCaptureOrPawnMove;
                        Game.PositionHashHistory.RemoveAt(Game.PositionHashHistory.Count - 1);
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
                        Game.PositionHashHistory.RemoveAt(Game.PositionHashHistory.Count - 1);
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
                    && scores[moveIndex] < EvaluationConstants.PromotionMoveScoreValue
                    && scores[moveIndex] >= EvaluationConstants.BadCaptureMoveBaseScoreValue)
                {
                    reduction += Configuration.EngineSettings.SEE_BadCaptureReduction;
                    reduction = Math.Clamp(reduction, 0, depth - 1);
                }

                // Search with reduced depth
                evaluation = -NegaMax(depth - 1 - reduction, ply + 1, -alpha - 1, -alpha);

                // 🔍 Principal Variation Search (PVS)
                if (evaluation > alpha && reduction > 0)
                {
                    // Optimistic search, validating that the rest of the moves are worse than bestmove.
                    // It should produce more cutoffs and therefore be faster.
                    // https://web.archive.org/web/20071030220825/http://www.brucemo.com/compchess/programming/pvs.htm

                    // Search with full depth but narrowed score bandwidth
                    evaluation = -NegaMax(depth - 1, ply + 1, -alpha - 1, -alpha);
                }

                if (evaluation > alpha && evaluation < beta)
                {
                    // PVS Hypothesis invalidated -> search with full depth and full score bandwidth
                    evaluation = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
                }
            }

            // After making a move
            // Game.PositionHashHistory is update above
            Game.HalfMovesWithoutCaptureOrPawnMove = oldHalfMovesWithoutCaptureOrPawnMove;
            Game.PositionHashHistory.RemoveAt(Game.PositionHashHistory.Count - 1);
            position.UnmakeMove(move, gameState);

            PrintMove(position, ply, move, evaluation);

            // Fail-hard beta-cutoff - refutation found, no need to keep searching this line
            if (evaluation >= beta)
            {
                PrintMessage($"Pruning: {move} is enough");

                if (isCapture)
                {
                    var piece = move.Piece();
                    var targetSquare = move.TargetSquare();
                    var capturedPiece = move.CapturedPiece();

                    _captureHistory[piece][targetSquare][capturedPiece] = ScoreHistoryMove(
                        _captureHistory[piece][targetSquare][capturedPiece],
                        EvaluationConstants.HistoryBonus[depth]);

                    // 🔍 Capture history penalty/malus
                    // When a capture fails high, penalize previous visited captures
                    for (int i = 0; i < visitedMovesCounter; ++i)
                    {
                        var visitedMove = visitedMoves[i];

                        if (visitedMove.IsCapture())
                        {
                            var visitedMovePiece = visitedMove.Piece();
                            var visitedMoveTargetSquare = visitedMove.TargetSquare();
                            var visitedMoveCapturedPiece = visitedMove.CapturedPiece();

                            _captureHistory[visitedMovePiece][visitedMoveTargetSquare][visitedMoveCapturedPiece] = ScoreHistoryMove(
                                _captureHistory[visitedMovePiece][visitedMoveTargetSquare][visitedMoveCapturedPiece],
                                -EvaluationConstants.HistoryBonus[depth]);
                        }
                    }
                }
                else
                {
                    // 🔍 Quiet history moves
                    // Doing this only in beta cutoffs (instead of when eval > alpha) was suggested by Sirius author
                    var piece = move.Piece();
                    var targetSquare = move.TargetSquare();

                    _quietHistory[piece][targetSquare] = ScoreHistoryMove(
                        _quietHistory[piece][targetSquare],
                        EvaluationConstants.HistoryBonus[depth]);

                    // 🔍 Continuation history
                    // - Counter move history (continuation history, ply - 1)
                    var previousMove = Game.PopFromMoveStack(ply - 1);
                    var previousMovePiece = previousMove.Piece();
                    var previousTargetSquare = previousMove.TargetSquare();

                    var continuationHistoryIndex = ContinuationHistoryIndex(piece, targetSquare, previousMovePiece, previousTargetSquare, 0);

                    _continuationHistory[continuationHistoryIndex] = ScoreHistoryMove(
                        _continuationHistory[continuationHistoryIndex],
                        EvaluationConstants.HistoryBonus[depth]);

                    //    var previousPreviousMove = Game.MoveStack[ply - 2];
                    //    var previousPreviousMovePiece = previousPreviousMove.Piece();
                    //    var previousPreviousMoveTargetSquare = previousPreviousMove.TargetSquare();

                    //    _continuationHistory[piece][targetSquare][1][previousPreviousMovePiece][previousPreviousMoveTargetSquare] = ScoreHistoryMove(
                    //        _continuationHistory[piece][targetSquare][1][previousPreviousMovePiece][previousPreviousMoveTargetSquare],
                    //        EvaluationConstants.HistoryBonus[depth]);

                    for (int i = 0; i < visitedMovesCounter - 1; ++i)
                    {
                        var visitedMove = visitedMoves[i];

                        if (!visitedMove.IsCapture())
                        {
                            var visitedMovePiece = visitedMove.Piece();
                            var visitedMoveTargetSquare = visitedMove.TargetSquare();

                            // 🔍 Quiet history penalty / malus
                            // When a quiet move fails high, penalize previous visited quiet moves
                            _quietHistory[visitedMovePiece][visitedMoveTargetSquare] = ScoreHistoryMove(
                                _quietHistory[visitedMovePiece][visitedMoveTargetSquare],
                                -EvaluationConstants.HistoryBonus[depth]);

                            // 🔍 Continuation history penalty / malus
                            continuationHistoryIndex = ContinuationHistoryIndex(visitedMovePiece, visitedMoveTargetSquare, previousMovePiece, previousTargetSquare, 0);

                            _continuationHistory[continuationHistoryIndex] = ScoreHistoryMove(
                                _continuationHistory[continuationHistoryIndex],
                                -EvaluationConstants.HistoryBonus[depth]);
                        }
                    }

                    if (move.PromotedPiece() == default && move != _killerMoves[0][ply])
                    {
                        // 🔍 Killer moves
                        if (move != _killerMoves[1][ply])
                        {
                            _killerMoves[2][ply] = _killerMoves[1][ply];
                        }

                        _killerMoves[1][ply] = _killerMoves[0][ply];
                        _killerMoves[0][ply] = move;

                        // 🔍 Countermoves
                        _counterMoves[CounterMoveIndex(previousMovePiece, previousTargetSquare)] = move;
                    }
                }

                _tt.RecordHash(_ttMask, position, depth, ply, beta, NodeType.Beta, bestMove);

                return beta;    // TODO return evaluation?
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
                bestMove = move;

                _pVTable[pvIndex] = move;
                CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - ply - 1);

                nodeType = NodeType.Exact;
            }

            ++visitedMovesCounter;
        }

        if (bestMove is null && !isAnyMoveValid)
        {
            var eval = Position.EvaluateFinalPosition(ply, isInCheck);

            _tt.RecordHash(_ttMask, position, depth, ply, eval, NodeType.Exact);
            return eval;
        }

        _tt.RecordHash(_ttMask, position, depth, ply, alpha, nodeType, bestMove);

        // Node fails low
        return alpha;
    }

    /// <summary>
    /// Quiescence search implementation, NegaMax alpha-beta style, fail-hard
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
            return position.StaticEvaluation().Score;
        }

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        var ttProbeResult = _tt.ProbeHash(_ttMask, position, 0, ply, alpha, beta);
        if (ttProbeResult.Evaluation != EvaluationConstants.NoHashEntry)
        {
            return ttProbeResult.Evaluation;
        }
        ShortMove ttBestMove = ttProbeResult.BestMove;

        _maxDepthReached[ply] = ply;

        var staticEvaluation = position.StaticEvaluation().Score;

        // Fail-hard beta-cutoff (updating alpha after this check)
        if (staticEvaluation >= beta)
        {
            PrintMessage(ply - 1, "Pruning before starting quiescence search");
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
            return staticEvaluation;
        }

        var nodeType = NodeType.Alpha;
        Move? bestMove = null;
        bool isThereAnyValidCapture = false;

        Span<int> scores = stackalloc int[pseudoLegalMoves.Length];
        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            scores[i] = ScoreMove(pseudoLegalMoves[i], ply, isNotQSearch: false, ttBestMove);
        }

        for (int i = 0; i < pseudoLegalMoves.Length; ++i)
        {
            // Incremental move sorting, inspired by https://github.com/jw1912/Chess-Challenge and suggested by toanth
            // There's no need to sort all the moves since most of them don't get checked anyway
            // So just find the first unsearched one with the best score and try it
            for (int j = i + 1; j < pseudoLegalMoves.Length; j++)
            {
                if (scores[j] > scores[i])
                {
                    (scores[i], scores[j], pseudoLegalMoves[i], pseudoLegalMoves[j]) = (scores[j], scores[i], pseudoLegalMoves[j], pseudoLegalMoves[i]);
                }
            }

            var move = pseudoLegalMoves[i];

            // Prune bad captures
            if (scores[i] < EvaluationConstants.PromotionMoveScoreValue && scores[i] >= EvaluationConstants.BadCaptureMoveBaseScoreValue)
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
            isThereAnyValidCapture = true;

            PrintPreMove(position, ply, move, isQuiescence: true);

            // No need to check for threefold or 50 moves repetitions, since we're only searching captures, promotions, and castles
            // Theoretically there could be a castling move that caused the 50 moves repetitions, but it's highly unlikely
            Game.PushToMoveStack(ply, move);

            int evaluation = -QuiescenceSearch(ply + 1, -beta, -alpha);
            position.UnmakeMove(move, gameState);

            PrintMove(position, ply, move, evaluation);

            // Fail-hard beta-cutoff
            if (evaluation >= beta)
            {
                PrintMessage($"Pruning: {move} is enough to discard this line");

                _tt.RecordHash(_ttMask, position, 0, ply, beta, NodeType.Beta, bestMove);

                return evaluation; // The refutation doesn't matter, since it'll be pruned
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
                bestMove = move;

                _pVTable[pvIndex] = move;
                CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - ply - 1);

                nodeType = NodeType.Exact;
            }
        }

        if (bestMove is null
            && !isThereAnyValidCapture
            && !MoveGenerator.CanGenerateAtLeastAValidMove(position))
        {
            var finalEval = Position.EvaluateFinalPosition(ply, position.IsInCheck());
            _tt.RecordHash(_ttMask, position, 0, ply, finalEval, NodeType.Exact);

            return finalEval;
        }

        _tt.RecordHash(_ttMask, position, 0, ply, alpha, nodeType, bestMove);

        return alpha;
    }
}
