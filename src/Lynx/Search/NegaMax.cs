﻿using Lynx.Model;

namespace Lynx;

public sealed partial class Engine
{
    /// <summary>
    /// NegaMax algorithm implementation using alpha-beta pruning, quiescence search and Iterative Deepeting Depth-First Search (IDDFS)
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
    private int NegaMax(int depth, int ply, int alpha, int beta, bool parentWasNullMove = false)
    {
        var position = Game.CurrentPosition;

        // Prevents runtime failure in case depth is increased due to check extension, since we're using ply when calculating pvTable index,
        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth {0} reached", Configuration.EngineSettings.MaxDepth);
            return position.StaticEvaluation().Evaluation;
        }

        _maxDepthReached[ply] = ply;
        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        bool isRoot = ply == 0;
        bool pvNode = beta - alpha > 1;

        Move ttBestMove = default;
        NodeType ttElementType = default;
        int ttEval = EvaluationConstants.NoHashEntry;

        if (!isRoot)
        {
            var ttEntry = _tt.ProbeHash(_ttMask, position, depth, ply, alpha, beta);
            if (ttEntry.Evaluation != EvaluationConstants.NoHashEntry)
            {
                return ttEntry.Evaluation;
            }

            ttEval = ttEntry.Evaluation;
            ttBestMove = ttEntry.BestMove;
            ttElementType = ttEntry.Type;
        }

        // Before any time-consuming operations
        _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        bool isInCheck = position.IsInCheck();

        if (isInCheck)
        {
            ++depth;
        }
        if (depth <= 0)
        {
            if (MoveGenerator.CanGenerateAtLeastAValidMove(position))
            {
                return QuiescenceSearch(ply, alpha, beta);
            }

            var finalPositionEvaluation = Position.EvaluateFinalPosition(ply, isInCheck);
            _tt.RecordHash(_ttMask, position, depth, ply, finalPositionEvaluation, NodeType.Exact);
            return finalPositionEvaluation;
        }

        if (!pvNode && !isInCheck)
        {
            var staticEvalResult = position.StaticEvaluation();
            var staticEval = staticEvalResult.Evaluation;

            // 🔍 Null Move Pruning (NMP) - our position is so good that we can potentially afford giving ouropponent a double move and still remain ahead of beta
            if (depth >= Configuration.EngineSettings.NMP_MinDepth
                && staticEval >= beta
                && !parentWasNullMove                           // We'd get to the same position
                && staticEvalResult.Phase > 2   // Zugzwang risk reduction: pieces other than pawn presents
                && (ttEval == EvaluationConstants.NoHashEntry
                    || ttElementType != NodeType.Alpha          // Not a fail low entry
                    || ttEval >= beta))                         // If it is, its score should be over beta
            {
                var nmpReduction =
                    Configuration.EngineSettings.NMP_BaseDepthReduction
                + (depth / Configuration.EngineSettings.NMP_DepthIncrementDivisor)
                + ((staticEval - beta) / Configuration.EngineSettings.NMP_StaticEvalBetaDeltaIncrementDivisor);
                //+ Math.Min((staticEval - beta) / Configuration.EngineSettings.NMP_StaticEvalBetaDeltaIncrementDivisor, 3);

                // TODO adaptative reduction

                var gameState = position.MakeNullMove();
                var evaluation = -NegaMax(depth - 1 - nmpReduction, ply + 1, -beta, -beta + 1, parentWasNullMove: true);
                position.UnMakeNullMove(gameState);

                if (evaluation >= beta)
                {
                    return evaluation;
                }
            }

            if (depth <= Configuration.EngineSettings.RFP_MaxDepth)
            {
                // 🔍 Reverse Futility Pruning (RFP) - https://www.chessprogramming.org/Reverse_Futility_Pruning
                if (staticEval - (Configuration.EngineSettings.RFP_DepthScalingFactor * depth) >= beta)
                {
                    return staticEval;
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
        }

        var nodeType = NodeType.Alpha;
        int movesSearched = 0;
        Move? bestMove = null;
        bool isAnyMoveValid = false;

        var pseudoLegalMoves = SortMoves(MoveGenerator.GenerateAllMoves(position, Game.MovePool), ply, ttBestMove);

        for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
        {
            var move = pseudoLegalMoves[moveIndex];

            var gameState = position.MakeMove(move);

            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            ++_nodes;
            isAnyMoveValid = true;

            PrintPreMove(position, ply, move);

            // Before making a move
            var oldValue = Game.HalfMovesWithoutCaptureOrPawnMove;
            Game.HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, Game.HalfMovesWithoutCaptureOrPawnMove);
            var isThreeFoldRepetition = !Game.PositionHashHistory.Add(position.UniqueIdentifier);

            int evaluation;
            if (isThreeFoldRepetition || Game.Is50MovesRepetition())
            {
                evaluation = 0;

                // We don't need to evaluate further down to know it's a draw.
                // Since we won't be evaluating further down, we need to clear the PV table because those moves there
                // don't belong to this line and if this move were to beat alpha, they'd incorrectly copied to pv line.
                Array.Clear(_pVTable, nextPvIndex, _pVTable.Length - nextPvIndex);
            }
            else if (pvNode && movesSearched == 0)
            {
                evaluation = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
            }
            else
            {
                int reduction = 0;

                // 🔍 Late Move Reduction (LMR) - search with reduced depth
                // Impl. based on Ciekce advice (Stormphrax) and Stormphrax & Akimbo implementations
                if (movesSearched >= (pvNode ? Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves : Configuration.EngineSettings.LMR_MinFullDepthSearchedMoves - 1)
                    && depth >= Configuration.EngineSettings.LMR_MinDepth
                    && !isInCheck
                    && !move.IsCapture())
                {
                    reduction = EvaluationConstants.LMRReductions[depth, movesSearched];

                    if (pvNode)
                    {
                        --reduction;
                    }
                    if (position.IsInCheck())   // i.e. move gives check
                    {
                        --reduction;
                    }

                    // Don't allow LMR to drop into qsearch or increase the depth
                    // depth - 1 - depth +2 = 1, min depth we want
                    reduction = Math.Clamp(reduction, 0, depth - 2);
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
                    // PVS Hipothesis invalidated -> search with full depth and full score bandwidth
                    evaluation = -NegaMax(depth - 1, ply + 1, -beta, -alpha);
                }
            }

            // After making a move
            Game.HalfMovesWithoutCaptureOrPawnMove = oldValue;
            if (!isThreeFoldRepetition)
            {
                Game.PositionHashHistory.Remove(position.UniqueIdentifier);
            }
            position.UnmakeMove(move, gameState);

            PrintMove(ply, move, evaluation);

            // Fail-hard beta-cutoff - refutation found, no need to keep searching this line
            if (evaluation >= beta)
            {
                PrintMessage($"Pruning: {move} is enough");

                // 🔍 Killer moves
                if (!move.IsCapture() && move.PromotedPiece() == default && move != _killerMoves[0, ply])
                {
                    _killerMoves[1, ply] = _killerMoves[0, ply];
                    _killerMoves[0, ply] = move;
                }

                _tt.RecordHash(_ttMask, position, depth, ply, beta, NodeType.Beta, bestMove);

                return beta;    // TODO return evaluation?
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
                bestMove = move;

                // 🔍 History moves
                if (!move.IsCapture())
                {
                    var piece = move.Piece();
                    var targetSquare = move.TargetSquare();

                    _historyMoves[piece, targetSquare] = ScoreHistoryMove(_historyMoves[piece, targetSquare], ply << 2);
                }

                _pVTable[pvIndex] = move;
                CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - ply - 1);

                nodeType = NodeType.Exact;
            }

            ++movesSearched;
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
    public int QuiescenceSearch(int ply, int alpha, int beta)
    {
        var position = Game.CurrentPosition;

        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
        _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth {0} reached", Configuration.EngineSettings.MaxDepth);
            return position.StaticEvaluation().Evaluation;
        }

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        Move ttBestMove = default;

        var ttProbeResult = _tt.ProbeHash(_ttMask, position, 0, ply, alpha, beta);
        if (ttProbeResult.Evaluation != EvaluationConstants.NoHashEntry)
        {
            return ttProbeResult.Evaluation;
        }
        ttBestMove = ttProbeResult.BestMove;

        _maxDepthReached[ply] = ply;

        var staticEvaluation = position.StaticEvaluation().Evaluation;

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

        var generatedMoves = MoveGenerator.GenerateAllMoves(position, Game.MovePool, capturesOnly: true);
        if (!generatedMoves.Any())
        {
            // Checking if final position first: https://github.com/lynx-chess/Lynx/pull/358
            return staticEvaluation;
        }

        var nodeType = NodeType.Alpha;
        Move? bestMove = null;
        bool isThereAnyValidCapture = false;

        var pseudoLegalMoves = generatedMoves.OrderByDescending(move => ScoreMove(move, ply, false, ttBestMove));

        foreach (var move in pseudoLegalMoves)
        {
            var gameState = position.MakeMove(move);
            if (!position.WasProduceByAValidMove())
            {
                position.UnmakeMove(move, gameState);
                continue;
            }

            ++_nodes;
            isThereAnyValidCapture = true;

            PrintPreMove(position, ply, move, isQuiescence: true);

            // Before making a move
            var oldValue = Game.HalfMovesWithoutCaptureOrPawnMove;
            Game.HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, Game.HalfMovesWithoutCaptureOrPawnMove);
            var isThreeFoldRepetition = !Game.PositionHashHistory.Add(position.UniqueIdentifier);

            int evaluation;
            if (isThreeFoldRepetition || Game.Is50MovesRepetition())
            {
                evaluation = 0;

                // We don't need to evaluate further down to know it's a draw.
                // Since we won't be evaluating further down, we need to clear the PV table because those moves there
                // don't belong to this line and if this move were to beat alpha, they'd incorrectly copied to pv line.
                Array.Clear(_pVTable, nextPvIndex, _pVTable.Length - nextPvIndex);
            }
            else
            {
                evaluation = -QuiescenceSearch(ply + 1, -beta, -alpha);
            }

            // After making a move
            Game.HalfMovesWithoutCaptureOrPawnMove = oldValue;
            if (!isThreeFoldRepetition)
            {
                Game.PositionHashHistory.Remove(position.UniqueIdentifier);
            }
            position.UnmakeMove(move, gameState);

            PrintMove(ply, move, evaluation);

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
