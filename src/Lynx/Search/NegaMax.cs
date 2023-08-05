﻿using Lynx.Model;

namespace Lynx;

public sealed partial class Engine
{
    /// <summary>
    /// NegaMax algorithm implementation using alpha-beta pruning, quiescence search and Iterative Deepeting Depth-First Search (IDDFS)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="minDepth">Minimum number of depth (plies), regardless of time constrains</param>
    /// <param name="targetDepth"></param>
    /// <param name="ply">Current depth or number of half moves</param>
    /// <param name="alpha">
    /// Best score the Side to move can achieve, assuming best play by the opponent.
    /// Defaults to the worse possible score for Side to move, Int.MinValue.
    /// </param>
    /// <param name="beta">
    /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
    /// Defaults to the worse possible score for Side to move's opponent, Int.MaxValue
    /// </param>
    /// <param name="isVerifyingNullMoveCutOff">Indicates if the search is verifying an ancestors null-move that failed high, or the root node</param>
    /// <param name="ancestorWasNullMove">Indicates whether the immediate ancestor node was a null move</param>
    /// <returns></returns>
    private int NegaMax(in Position position, int minDepth, int targetDepth, int ply, int alpha, int beta, bool isVerifyingNullMoveCutOff, bool ancestorWasNullMove = false)
    {
        _maxDepthReached[ply] = ply;
        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        Move ttBestMove = default;

        if (ply > 0)
        {
            var ttProbeResult = _transpositionTable.ProbeHash(position, targetDepth, ply, alpha, beta);
            if (ttProbeResult.Evaluation != EvaluationConstants.NoHashEntry)
            {
                return ttProbeResult.Evaluation;
            }
            ttBestMove = ttProbeResult.BestMove;
        }

        // Before any time-consuming operations
        _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        bool isInCheck = position.IsInCheck();

        if (ply >= targetDepth)
        {
            if (isInCheck)
            {
                ++targetDepth;
            }
            else
            {
                foreach (var candidateMove in position.AllPossibleMoves(Game.MovePool))
                {
                    if (new Position(in position, candidateMove).WasProduceByAValidMove())
                    {
                        return QuiescenceSearch(in position, targetDepth, ply, alpha, beta);
                    }
                }

                var finalPositionEvaluation = Position.EvaluateFinalPosition(ply, isInCheck);
                _transpositionTable.RecordHash(position, targetDepth, ply, finalPositionEvaluation, NodeType.Exact);
                return finalPositionEvaluation;
            }
        }

        // Prevents runtime failure, in case targetDepth is increased due to check extension
        if (ply >= Configuration.EngineSettings.MaxDepth)
        {
            _logger.Info("Max depth {0} reached", Configuration.EngineSettings.MaxDepth);
            return position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove, _searchCancellationTokenSource.Token);
        }

        ++_nodes;

        // 🔍 Null-move pruning
        bool isFailHigh = false;    // In order to detect zugzwangs
        if (ply > Configuration.EngineSettings.NullMovePruning_R
            && !isInCheck
            && !ancestorWasNullMove
            && (!isVerifyingNullMoveCutOff || ply < targetDepth - 1))    // verify == true and ply == targetDepth -1 -> No null pruning, since verification will not be possible)
                                                                         // following pv?
        {
            var newPosition = new Position(in position, nullMove: true);

            var evaluation = -NegaMax(in newPosition, minDepth, targetDepth, ply + 1 + Configuration.EngineSettings.NullMovePruning_R, -beta, -beta + 1, isVerifyingNullMoveCutOff, ancestorWasNullMove: true);

            if (evaluation >= beta) // Fail high
            {
                if (isVerifyingNullMoveCutOff)
                {
                    ++ply;
                    isVerifyingNullMoveCutOff = false;
                    isFailHigh = true;
                }
                else
                {
                    // cutoff in a sub-tree with fail-high report
                    return evaluation;
                }
            }
        }

        VerifiedNullMovePruning_SearchAgain:

        var nodeType = NodeType.Alpha;
        int movesSearched = 0;
        Move? bestMove = null;
        bool isAnyMoveValid = false;

        var pseudoLegalMoves = SortMoves(position.AllPossibleMoves(Game.MovePool), in position, ply, ttBestMove);

        foreach (var move in pseudoLegalMoves)
        {
            var newPosition = new Position(in position, move);
            if (!newPosition.WasProduceByAValidMove())
            {
                continue;
            }
            isAnyMoveValid = true;

            PrintPreMove(in position, ply, move);

            // Before making a move
            var oldValue = Game.HalfMovesWithoutCaptureOrPawnMove;
            Game.HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, Game.HalfMovesWithoutCaptureOrPawnMove);
            var isThreeFoldRepetition = !Game.PositionHashHistory.Add(newPosition.UniqueIdentifier);

            int evaluation;
            if (isThreeFoldRepetition || Game.Is50MovesRepetition())
            {
                evaluation = 0;

                // We don't need to evaluate further down to know it's a draw.
                // Since we won't be evaluating further down, we need to clear the PV table because those moves there
                // don't belong to this line and if this move were to beat alpha, they'd incorrectly copied to pv line.
                Array.Clear(_pVTable, nextPvIndex, _pVTable.Length - nextPvIndex);
            }
            else if (movesSearched == 0)
            {
                evaluation = -NegaMax(in newPosition, minDepth, targetDepth, ply + 1, -beta, -alpha, isVerifyingNullMoveCutOff);
            }
            else
            {
                // 🔍 Late Move Reduction (LMR)
                if (movesSearched >= Configuration.EngineSettings.LMR_FullDepthMoves
                    && ply >= Configuration.EngineSettings.LMR_ReductionLimit
                    && !_isFollowingPV
                    && !isInCheck
                    //&& !newPosition.IsInCheck()
                    && !move.IsCapture()
                    && move.PromotedPiece() == default)
                {
                    // Search with reduced depth
                    evaluation = -NegaMax(in newPosition, minDepth, targetDepth, ply + 1 + Configuration.EngineSettings.LMR_DepthReduction, -alpha - 1, -alpha, isVerifyingNullMoveCutOff);
                }
                else
                {
                    // Ensuring full depth search takes place
                    evaluation = alpha + 1;
                }

                if (evaluation > alpha)
                {
                    // 🔍 Principal Variation Search (PVS)
                    if (bestMove is not null)
                    {
                        // Optimistic search, validating that the rest of the moves are worse than bestmove.
                        // It should produce more cutoffs and therefore be faster.
                        // https://web.archive.org/web/20071030220825/http://www.brucemo.com/compchess/programming/pvs.htm

                        // Search with full depth but narrowed score bandwidth
                        evaluation = -NegaMax(in newPosition, minDepth, targetDepth, ply + 1, -alpha - 1, -alpha, isVerifyingNullMoveCutOff);

                        if (evaluation > alpha && evaluation < beta)
                        {
                            // Hipothesis invalidated -> search with full depth and full score bandwidth
                            evaluation = -NegaMax(in newPosition, minDepth, targetDepth, ply + 1, -beta, -alpha, isVerifyingNullMoveCutOff);
                        }
                    }
                    else
                    {
                        evaluation = -NegaMax(in newPosition, minDepth, targetDepth, ply + 1, -beta, -alpha, isVerifyingNullMoveCutOff);
                    }
                }
            }

            // After making a move
            Game.HalfMovesWithoutCaptureOrPawnMove = oldValue;
            if (!isThreeFoldRepetition)
            {
                Game.PositionHashHistory.Remove(newPosition.UniqueIdentifier);
            }

            PrintMove(ply, move, evaluation);

            // Fail-hard beta-cutoff - refutation found, no need to keep searching this line
            if (evaluation >= beta)
            {
                PrintMessage($"Pruning: {move} is enough");

                // 🔍 Killer moves
                if (!move.IsCapture())
                {
                    _killerMoves[1, ply] = _killerMoves[0, ply];
                    _killerMoves[0, ply] = move;
                }

                _transpositionTable.RecordHash(position, targetDepth, ply, beta, NodeType.Beta, bestMove);

                return beta;    // TODO return evaluation?
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
                bestMove = move;

                // 🔍 History moves
                if (!move.IsCapture())
                {
                    _historyMoves[move.Piece(), move.TargetSquare()] += ply << 2;
                }

                _pVTable[pvIndex] = move;
                CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - ply - 1);

                nodeType = NodeType.Exact;
            }

            ++movesSearched;
        }

        // [Null-move pruning] If there is a fail-high report, but no cutoff was found, the position is a zugzwang and has to be re-searched with the original depth
        if (isFailHigh && alpha < beta)
        {
            --ply;
            isFailHigh = false;
            isVerifyingNullMoveCutOff = true;
            goto VerifiedNullMovePruning_SearchAgain;
        }

        if (bestMove is null && !isAnyMoveValid)
        {
            var eval = Position.EvaluateFinalPosition(ply, isInCheck);

            _transpositionTable.RecordHash(position, targetDepth, ply, eval, NodeType.Exact);
            return eval;
        }

        _transpositionTable.RecordHash(position, targetDepth, ply, alpha, nodeType, bestMove);

        // Node fails low
        return alpha;
    }

    /// <summary>
    /// Quiescence search implementation, NegaMax alpha-beta style, fail-hard
    /// </summary>
    /// <param name="position"></param>
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
    public int QuiescenceSearch(in Position position, int targetDepth, int ply, int alpha, int beta)
    {
        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
        //_searchCancellationTokenSource.Token.ThrowIfCancellationRequested();

        var pvIndex = PVTable.Indexes[ply];
        var nextPvIndex = PVTable.Indexes[ply + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        Move ttBestMove = default;

        var ttProbeResult = _transpositionTable.ProbeHash(position, targetDepth, ply, alpha, beta); // We need to make sure that
        //if (ttProbeResult.Evaluation != EvaluationConstants.NoHashEntry)
        //{
        //    return ttProbeResult.Evaluation;
        //}
        ttBestMove = ttProbeResult.BestMove;

        ++_nodes;
        _maxDepthReached[ply] = ply;

        var staticEvaluation = position.StaticEvaluation(Game.HalfMovesWithoutCaptureOrPawnMove, _searchCancellationTokenSource.Token);

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

        var generatedMoves = position.AllCapturesMoves(Game.MovePool);
        if (!generatedMoves.Any())
        {
            return staticEvaluation;  // TODO check if in check or drawn position
        }

        var nodeType = NodeType.Alpha;
        Move? bestMove = null;
        bool isAnyMoveValid = false;
        var localPosition = position;

        var pseudoLegalMoves = generatedMoves.OrderByDescending(move => Score(move, in localPosition, ply, /*ttBestMove*/));

        foreach (var move in pseudoLegalMoves)
        {
            var newPosition = new Position(in position, move);
            if (!newPosition.WasProduceByAValidMove())
            {
                continue;
            }
            isAnyMoveValid = true;

            PrintPreMove(in position, ply, move, isQuiescence: true);

            // Before making a move
            var oldValue = Game.HalfMovesWithoutCaptureOrPawnMove;
            Game.HalfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, Game.HalfMovesWithoutCaptureOrPawnMove);
            var isThreeFoldRepetition = !Game.PositionHashHistory.Add(newPosition.UniqueIdentifier);

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
                evaluation = -QuiescenceSearch(in newPosition, targetDepth, ply + 1, -beta, -alpha);
            }

            // After making a move
            Game.HalfMovesWithoutCaptureOrPawnMove = oldValue;
            if (!isThreeFoldRepetition)
            {
                Game.PositionHashHistory.Remove(newPosition.UniqueIdentifier);
            }

            PrintMove(ply, move, evaluation);

            // Fail-hard beta-cutoff
            if (evaluation >= beta)
            {
                PrintMessage($"Pruning: {move} is enough to discard this line");

                _transpositionTable.RecordHash(position, targetDepth, ply, beta, NodeType.Beta, bestMove);

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

        if (bestMove is null)
        {
            if (isAnyMoveValid)
            {
                goto ReturnAlpha;
            }

            foreach (var move in position.AllPossibleMoves(Game.MovePool))
            {
                if (new Position(in position, move).WasProduceByAValidMove())
                {
                    goto ReturnAlpha;
                }
            }

            var finalEval = Position.EvaluateFinalPosition(ply, position.IsInCheck());
            _transpositionTable.RecordHash(position, targetDepth, ply, finalEval, NodeType.Exact);

            return finalEval;
        }

        ReturnAlpha:
        _transpositionTable.RecordHash(position, targetDepth, ply, alpha, nodeType, bestMove);

        // Node fails low
        return alpha;
    }
}
