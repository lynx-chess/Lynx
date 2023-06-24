using Lynx.Model;

namespace Lynx;

public sealed partial class Engine
{
    /// <summary>
    /// NegaMax algorithm implementation using alpha-beta pruning, quiescence search and Iterative Deepeting Depth-First Search (IDDFS)
    /// </summary>
    /// <param name="position"></param>
    /// <param name="minDepth"></param>
    /// <param name="maxDepth"></param>
    /// <param name="depth"></param>
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
    private int NegaMax(in Position position, int minDepth, int maxDepth, int depth, int alpha, int beta, bool isVerifyingNullMoveCutOff, bool ancestorWasNullMove = false)
    {
        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
        if (depth > minDepth)
        {
            _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        ++_nodes;
        _maxDepthReached[depth] = depth;

        var pvIndex = PVTable.Indexes[depth];
        var nextPvIndex = PVTable.Indexes[depth + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        bool isPvNode = beta - alpha > 1;
        if (!isPvNode && depth > 0)
        {
            var transpositionTableValue = Game.TranspositionTable.ProbeHash(position, maxDepth, depth, alpha, beta);
            if (transpositionTableValue != EvaluationConstants.NoHashEntry)
            {
                return transpositionTableValue;
            }
        }

        bool isInCheck = position.IsInCheck();

        if (depth >= maxDepth)
        {
            foreach (var candidateMove in position.AllPossibleMoves(Game.MovePool))
            {
                if (new Position(in position, candidateMove).WasProduceByAValidMove())
                {
                    return QuiescenceSearch(in position, depth, alpha, beta);
                }
            }

            return Position.EvaluateFinalPosition(depth, isInCheck, Game.PositionHashHistory, _halfMovesWithoutCaptureOrPawnMove);
        }

        // 🔍 Null-move pruning
        bool isFailHigh = false;    // In order to detect zugzwangs
        if (depth > Configuration.EngineSettings.NullMovePruning_R
            && !isInCheck
            && !ancestorWasNullMove
            && (!isVerifyingNullMoveCutOff || depth < maxDepth - 1))    // verify == true and depth == maxDepth -1 -> No null pruning, since verification will not be possible)
                                                                        // following pv?
        {
            var newPosition = new Position(in position, nullMove: true);

            var repetitions = Utils.UpdatePositionHistory(in newPosition, Game.PositionHashHistory);

            var evaluation = -NegaMax(in newPosition, minDepth, maxDepth, depth + 1 + Configuration.EngineSettings.NullMovePruning_R, -beta, -beta + 1, isVerifyingNullMoveCutOff, ancestorWasNullMove: true);

            Utils.RevertPositionHistory(in newPosition, Game.PositionHashHistory, repetitions);

            if (evaluation >= beta) // Fail high
            {
                // TODO null moves in tt?
                //Game.TranspositionTable.RecordHash(newPosition, maxDepth, depth, move: null, beta, NodeType.Beta);

                if (isVerifyingNullMoveCutOff)
                {
                    ++depth;
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

        var pseudoLegalMoves = SortMoves(position.AllPossibleMoves(Game.MovePool), in position, depth);

        foreach (var move in pseudoLegalMoves)
        {
            var newPosition = new Position(in position, move);
            if (!newPosition.WasProduceByAValidMove())
            {
                continue;
            }
            isAnyMoveValid = true;

            PrintPreMove(in position, depth, move);

            // Before making a move
            var oldValue = _halfMovesWithoutCaptureOrPawnMove;
            _halfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, _halfMovesWithoutCaptureOrPawnMove);
            var repetitions = Utils.UpdatePositionHistory(in newPosition, Game.PositionHashHistory);

            int evaluation;

            if (movesSearched == 0)
            {
                evaluation = -NegaMax(in newPosition, minDepth, maxDepth, depth + 1, -beta, -alpha, isVerifyingNullMoveCutOff);
            }
            else
            {
                // 🔍 Late Move Reduction (LMR)
                if (movesSearched >= Configuration.EngineSettings.LMR_FullDepthMoves
                    && depth >= Configuration.EngineSettings.LMR_ReductionLimit
                    && !_isFollowingPV
                    && !isInCheck
                    //&& !newPosition.IsInCheck()
                    && !move.IsCapture()
                    && move.PromotedPiece() == default)
                {
                    // Search with reduced depth
                    evaluation = -NegaMax(in newPosition, minDepth, maxDepth, depth + 1 + Configuration.EngineSettings.LMR_DepthReduction, -alpha - 1, -alpha, isVerifyingNullMoveCutOff);
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
                        evaluation = -NegaMax(in newPosition, minDepth, maxDepth, depth + 1, -alpha - 1, -alpha, isVerifyingNullMoveCutOff);

                        if (evaluation > alpha && evaluation < beta)
                        {
                            // Hipothesis invalidated -> search with full depth and full score bandwidth
                            evaluation = -NegaMax(in newPosition, minDepth, maxDepth, depth + 1, -beta, -alpha, isVerifyingNullMoveCutOff);
                        }
                    }
                    else
                    {
                        evaluation = -NegaMax(in newPosition, minDepth, maxDepth, depth + 1, -beta, -alpha, isVerifyingNullMoveCutOff);
                    }
                }
            }

            // After making a move
            _halfMovesWithoutCaptureOrPawnMove = oldValue;
            Utils.RevertPositionHistory(in newPosition, Game.PositionHashHistory, repetitions);

            PrintMove(depth, move, evaluation);

            // Fail-hard beta-cutoff
            if (evaluation >= beta)
            {
                PrintMessage($"Pruning: {move} is enough");

                // 🔍 Killer moves
                if (!move.IsCapture())
                {
                    _killerMoves[1, depth] = _killerMoves[0, depth];
                    _killerMoves[0, depth] = move;
                }

                Game.TranspositionTable.RecordHash(position, maxDepth, depth, move, beta, NodeType.Beta);

                return beta;    // TODO return evaluation?
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
                bestMove = move;

                // 🔍 History moves
                if (!move.IsCapture())
                {
                    _historyMoves[move.Piece(), move.TargetSquare()] += depth << 2;
                }

                _pVTable[pvIndex] = move;
                CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - depth - 1);

                nodeType = NodeType.Alpha;
            }

            ++movesSearched;
        }

        // [Null-move pruning] If there is a fail-high report, but no cutoff was found, the position is a zugzwang and has to be re-searched with the original depth
        if (isFailHigh && alpha < beta)
        {
            --depth;
            isFailHigh = false;
            isVerifyingNullMoveCutOff = true;
            goto VerifiedNullMovePruning_SearchAgain;
        }

        if (bestMove is null && !isAnyMoveValid)
        {
            var eval = Position.EvaluateFinalPosition(depth, isInCheck, Game.PositionHashHistory, _halfMovesWithoutCaptureOrPawnMove);

            Game.TranspositionTable.RecordHash(position, maxDepth, depth, bestMove, eval, NodeType.Exact);

            return eval;
        }

        Game.TranspositionTable.RecordHash(position, maxDepth, depth, bestMove, alpha, nodeType);

        // Node fails low
        return alpha;
    }

    /// <summary>
    /// Quiescence search implementation, NegaMax alpha-beta style, fail-hard
    /// </summary>
    /// <param name="position"></param>
    /// <param name="depth"></param>
    /// <param name="alpha">
    /// Best score White can achieve, assuming best play by Black.
    /// Defaults to the worse possible score for white, Int.MinValue.
    /// </param>
    /// <param name="beta">
    /// Best score Black can achieve, assuming best play by White
    /// Defaults to the works possible score for Black, Int.MaxValue
    /// </param>
    /// <returns></returns>
    public int QuiescenceSearch(in Position position, int depth, int alpha, int beta)
    {
        _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
        //_cancellationToken.Token.ThrowIfCancellationRequested();

        ++_nodes;
        _maxDepthReached[depth] = depth;

        var pvIndex = PVTable.Indexes[depth];
        var nextPvIndex = PVTable.Indexes[depth + 1];
        _pVTable[pvIndex] = _defaultMove;   // Nulling the first value before any returns

        var staticEvaluation = position.StaticEvaluation(Game.PositionHashHistory, _halfMovesWithoutCaptureOrPawnMove);

        // Fail-hard beta-cutoff (updating alpha after this check)
        if (staticEvaluation >= beta)
        {
            PrintMessage(depth - 1, "Pruning before starting quiescence search");
            return staticEvaluation;
        }

        // Better move
        if (staticEvaluation > alpha)
        {
            alpha = staticEvaluation;
        }

        // Quiescence search limitation
        //if (depth >= Configuration.EngineSettings.QuiescenceSearchDepth) return alpha;

        var generatedMoves = position.AllCapturesMoves(Game.MovePool);
        if (!generatedMoves.Any())
        {
            return staticEvaluation;  // TODO check if in check or drawn position
        }

        var movesToEvaluate = SortCaptures(generatedMoves, in position, depth);

        Move? bestMove = null;
        bool isAnyMoveValid = false;

        foreach (var move in movesToEvaluate)
        {
            var newPosition = new Position(in position, move);
            if (!newPosition.WasProduceByAValidMove())
            {
                continue;
            }
            isAnyMoveValid = true;

            PrintPreMove(in position, depth, move, isQuiescence: true);

            var oldValue = _halfMovesWithoutCaptureOrPawnMove;
            _halfMovesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, _halfMovesWithoutCaptureOrPawnMove);
            var repetitions = Utils.UpdatePositionHistory(in newPosition, Game.PositionHashHistory);

            var evaluation = -QuiescenceSearch(in newPosition, depth + 1, -beta, -alpha);

            _halfMovesWithoutCaptureOrPawnMove = oldValue;
            Utils.RevertPositionHistory(in newPosition, Game.PositionHashHistory, repetitions);

            PrintMove(depth, move, evaluation);

            // Fail-hard beta-cutoff
            if (evaluation >= beta)
            {
                PrintMessage($"Pruning: {move} is enough to discard this line");
                return evaluation; // The refutation doesn't matter, since it'll be pruned
            }

            if (evaluation > alpha)
            {
                alpha = evaluation;
                bestMove = move;

                _pVTable[pvIndex] = move;
                CopyPVTableMoves(pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - depth - 1);
            }
        }

        if (bestMove is null)
        {
            if (isAnyMoveValid)
            {
                return alpha;
            }

            foreach (var move in position.AllPossibleMoves(Game.MovePool))
            {
                if (new Position(in position, move).WasProduceByAValidMove())
                {
                    return alpha;
                }
            }

            return Position.EvaluateFinalPosition(depth, position.IsInCheck(), Game.PositionHashHistory, _halfMovesWithoutCaptureOrPawnMove);
        }

        // Node fails low
        return alpha;
    }
}
