using Lynx.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        /// <summary>
        /// NegaMax algorithm implementation using alpha-beta pruning, quiescence search and Iterative Deepeting Depth-First Search (IDDFS)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="plies"></param>
        /// <param name="alpha">
        /// Best score the Side to move can achieve, assuming best play by the opponent.
        /// Defaults to the worse possible score for Side to move, Int.MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Side's to move's opponent can achieve, assuming best play by Side to move.
        /// Defaults to the worse possible score for Side to move's opponent, Int.MaxValue
        /// </param>
        /// <returns></returns>
        private static (int Evaluation, int MaxDepth) NegaMax(Position position, Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove, Move[] pvTable, int pvIndex, int[,] killerMoves, int minDepth, int depthLimit, ref int nodes, int plies, int alpha, int beta, CancellationToken cancellationToken, CancellationToken absoluteCancellationToken)
        {
            absoluteCancellationToken.ThrowIfCancellationRequested();
            if (plies > minDepth)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            ++nodes;

            pvTable[pvIndex] = new Move();
            var nextPvIndex = PVTable.Indexes[plies + 1];

            var pseudoLegalMoves = position.AllPossibleMoves(killerMoves, plies);

            if (plies >= depthLimit)
            {
                foreach (var candidateMove in pseudoLegalMoves)
                {
                    if (new Position(position, candidateMove).WasProduceByAValidMove())
                    {
                        return QuiescenceSearch(position, positionHistory, movesWithoutCaptureOrPawnMove, pvTable, pvIndex, Configuration.EngineSettings.QuiescenceSearchDepth, ref nodes, plies, alpha, beta, cancellationToken, absoluteCancellationToken);
                    }
                }

                return (position.EvaluateFinalPosition(plies, positionHistory, movesWithoutCaptureOrPawnMove), plies);
            }

            Move? bestMove = null;

            var isAnyMoveValid = false;

            foreach (var move in pseudoLegalMoves)
            {
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }
                isAnyMoveValid = true;

                PrintPreMove(position, plies, move);

                var oldValue = movesWithoutCaptureOrPawnMove;
                movesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, movesWithoutCaptureOrPawnMove);
                var repetitions = Utils.UpdatePositionHistory(newPosition, positionHistory);
                var (evaluation, bestMoveExistingMoveList) = NegaMax(newPosition, positionHistory, movesWithoutCaptureOrPawnMove, pvTable, nextPvIndex, killerMoves, minDepth, depthLimit, ref nodes, plies + 1, -beta, -alpha, cancellationToken, absoluteCancellationToken);
                movesWithoutCaptureOrPawnMove = oldValue;
                Utils.RevertPositionHistory(newPosition, positionHistory, repetitions);

                evaluation = -evaluation;

                PrintMove(plies, move, evaluation);

                // Fail-hard beta-cutoff
                if (evaluation >= beta)
                {
                    _logger.Trace($"Pruning: {move} is enough");

                    //if (!move.IsCapture())
                    {
                        killerMoves[1, plies] = killerMoves[0, plies];
                        killerMoves[0, plies] = move.EncodedMove;
                    }

                    return (beta, plies);    // TODO return evaluation?
                }

                if (evaluation > alpha)
                {
                    alpha = evaluation;
                    bestMove = move;

                    pvTable[pvIndex] = move;
                    CopyMoves(pvTable, pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - plies - 1);
                }
            }

            if (bestMove is null)
            {
                return (isAnyMoveValid ? alpha : position.EvaluateFinalPosition(plies, positionHistory, movesWithoutCaptureOrPawnMove), plies);
            }

            // Node fails low
            return (alpha, plies);
        }

        /// <summary>
        /// Quiescence search implementation, NegaMax alpha-beta style, fail-hard
        /// </summary>
        /// <param name="position"></param>
        /// <param name="plies"></param>
        /// <param name="alpha">
        /// Best score White can achieve, assuming best play by Black.
        /// Defaults to the worse possible score for white, Int.MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Black can achieve, assuming best play by White
        /// Defaults to the works possible score for Black, Int.MaxValue
        /// </param>
        /// <returns></returns>
        public static (int Evaluation, int MaxDepth) QuiescenceSearch(Position position, Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove, Move[] pvTable, int pvIndex, int quiescenceDepthLimit, ref int nodes, int plies, int alpha, int beta, CancellationToken cancellationToken, CancellationToken absoluteCancellationToken)
        {
            absoluteCancellationToken.ThrowIfCancellationRequested();
            //cancellationToken.ThrowIfCancellationRequested();

            ++nodes;

            pvTable[pvIndex] = new Move();
            var nextPvIndex = PVTable.Indexes[plies + 1];

            var staticEvaluation = position.StaticEvaluation(positionHistory, movesWithoutCaptureOrPawnMove);

            // Fail-hard beta-cutoff (updating alpha after this check)
            if (staticEvaluation >= beta)
            {
                PrintMessage(plies - 1, "Pruning before starting quiescence search");
                return (staticEvaluation, plies);
            }

            // Better move
            if (staticEvaluation > alpha)
            {
                alpha = staticEvaluation;
            }

            if (plies >= quiescenceDepthLimit)
            {
                return (alpha, plies);   // Alpha?
            }

            var movesToEvaluate = position.AllCapturesMoves();

            if (!movesToEvaluate.Any())
            {
                return (staticEvaluation, plies);  // TODO check if in check or drawn position
            }

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

                PrintPreMove(position, plies, move, isQuiescence: true);

                var oldValue = movesWithoutCaptureOrPawnMove;
                movesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, movesWithoutCaptureOrPawnMove);
                var repetitions = Utils.UpdatePositionHistory(newPosition, positionHistory);
                var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch(newPosition, positionHistory, movesWithoutCaptureOrPawnMove, pvTable, nextPvIndex, quiescenceDepthLimit, ref nodes, plies + 1, -beta, -alpha, cancellationToken, absoluteCancellationToken);
                movesWithoutCaptureOrPawnMove = oldValue;
                Utils.RevertPositionHistory(newPosition, positionHistory, repetitions);

                evaluation = -evaluation;

                PrintMove(plies, move, evaluation);

                // Fail-hard beta-cutoff
                if (evaluation >= beta)
                {
                    _logger.Trace($"Pruning: {move} is enough to discard this line");
                    return (evaluation, plies); // The refutation doesn't matter, since it'll be pruned
                }

                if (evaluation > alpha)
                {
                    alpha = evaluation;
                    bestMove = move;

                    pvTable[pvIndex] = move;
                    CopyMoves(pvTable, pvIndex + 1, nextPvIndex, Configuration.EngineSettings.MaxDepth - plies - 1);
                }
            }

            if (bestMove is null)
            {
                var eval = isAnyMoveValid || position.AllPossibleMoves().Any(move => new Position(position, move).WasProduceByAValidMove())
                    ? alpha
                    : position.EvaluateFinalPosition(plies, positionHistory, movesWithoutCaptureOrPawnMove);

                return (eval, plies);
            }

            // Node fails low
            return (alpha, plies);
        }
    }
}
