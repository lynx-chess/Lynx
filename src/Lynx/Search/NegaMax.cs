using Lynx.Model;

namespace Lynx
{
    public sealed partial class Engine
    {
        /// <summary>
        /// NegaMax algorithm implementation using alpha-beta pruning, quiescence search and Iterative Deepeting Depth-First Search (IDDFS)
        /// </summary>
        /// <param name="position"></param>
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
        /// <returns></returns>
        private int NegaMax(Position position, int minDepth, int maxDepth, int depth, int alpha, int beta)
        {
            _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
            if (depth > minDepth)
            {
                _searchCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

            ++_nodes;
            var pseudoLegalMoves = SortMoves(position.AllPossibleMoves(), position, depth);

            if (depth >= maxDepth)
            {
                foreach (var candidateMove in pseudoLegalMoves)
                {
                    if (new Position(position, candidateMove).WasProduceByAValidMove())
                    {
                        return QuiescenceSearch(position, depth, alpha, beta);
                    }
                }

                return position.EvaluateFinalPosition(depth, Game.PositionHashHistory, _movesWithoutCaptureOrPawnMove);
            }

            Move? bestMove = null;
            bool isAnyMoveValid = false;

            var pvIndex = PVTable.Indexes[depth];
            var nextPvIndex = PVTable.Indexes[depth + 1];
            _pVTable[pvIndex] = _defaultMove;

            foreach (var move in pseudoLegalMoves)
            {
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }
                isAnyMoveValid = true;

                PrintPreMove(position, depth, move);

                var oldValue = _movesWithoutCaptureOrPawnMove;
                _movesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, _movesWithoutCaptureOrPawnMove);
                var repetitions = Utils.UpdatePositionHistory(newPosition, Game.PositionHashHistory);

                int evaluation;
                if (bestMove is not null)
                {
                    // Optimistic search, validating that the rest of the moves are worse than bestmove.
                    // It should produce more cutoffs and therefore be faster.
                    // https://web.archive.org/web/20071030220825/http://www.brucemo.com/compchess/programming/pvs.htm
                    evaluation = -NegaMax(newPosition, minDepth, maxDepth, depth + 1, -alpha - 1, -alpha);

                    if (evaluation > alpha && evaluation < beta)
                    {
                        // Hipothesis invalidated -> Regular search
                        evaluation = -NegaMax(newPosition, minDepth, maxDepth, depth + 1, -beta, -alpha);
                    }
                }
                else
                {
                    evaluation = -NegaMax(newPosition, minDepth, maxDepth, depth + 1, -beta, -alpha);
                }

                _movesWithoutCaptureOrPawnMove = oldValue;
                Utils.RevertPositionHistory(newPosition, Game.PositionHashHistory, repetitions);

                PrintMove(depth, move, evaluation);

                // Fail-hard beta-cutoff
                if (evaluation >= beta)
                {
                    PrintMessage($"Pruning: {move} is enough");

                    //if (!move.IsCapture())
                    {
                        _killerMoves[1, depth] = _killerMoves[0, depth];
                        _killerMoves[0, depth] = move.EncodedMove;
                    }
                    return beta;    // TODO return evaluation?
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
                return isAnyMoveValid
                    ? alpha
                    : position.EvaluateFinalPosition(depth, Game.PositionHashHistory, _movesWithoutCaptureOrPawnMove);
            }

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
        public int QuiescenceSearch(Position position, int depth, int alpha, int beta)
        {
            _absoluteSearchCancellationTokenSource.Token.ThrowIfCancellationRequested();
            //_cancellationToken.Token.ThrowIfCancellationRequested();

            ++_nodes;

            var staticEvaluation = position.StaticEvaluation(Game.PositionHashHistory, _movesWithoutCaptureOrPawnMove);

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

            if (depth >= Configuration.EngineSettings.QuiescenceSearchDepth)
            {
                return alpha;   // Alpha?
            }

            var generatedMoves = position.AllCapturesMoves();
            if (generatedMoves.Count == 0)
            {
                return staticEvaluation;  // TODO check if in check or drawn position
            }

            var movesToEvaluate = SortCaptures(generatedMoves, position, depth);

            var pvIndex = PVTable.Indexes[depth];
            var nextPvIndex = PVTable.Indexes[depth + 1];
            _pVTable[pvIndex] = _defaultMove;

            Move? bestMove = null;
            bool isAnyMoveValid = false;

            foreach (var move in movesToEvaluate)
            {
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }
                isAnyMoveValid = true;

                PrintPreMove(position, depth, move, isQuiescence: true);

                var oldValue = _movesWithoutCaptureOrPawnMove;
                _movesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, _movesWithoutCaptureOrPawnMove);
                var repetitions = Utils.UpdatePositionHistory(newPosition, Game.PositionHashHistory);

                var evaluation = -QuiescenceSearch(newPosition, depth + 1, -beta, -alpha);

                _movesWithoutCaptureOrPawnMove = oldValue;
                Utils.RevertPositionHistory(newPosition, Game.PositionHashHistory, repetitions);

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
                var eval = isAnyMoveValid || position.AllPossibleMoves().Any(move => new Position(position, move).WasProduceByAValidMove())
                    ? alpha
                    : position.EvaluateFinalPosition(depth, Game.PositionHashHistory, _movesWithoutCaptureOrPawnMove);

                return eval;
            }

            // Node fails low
            return alpha;
        }
    }
}
