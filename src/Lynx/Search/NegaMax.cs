using Lynx.Model;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static (int Evaluation, Result MoveList) NegaMax(Position position, Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove, Dictionary<long, PriorityQueue<Move, int>> orderedMoves, int[,] killerMoves, int minDepth, int depthLimit, ref int nodes, int plies, int alpha, int beta, CancellationToken cancellationToken, CancellationToken absoluteCancellationToken)
        {
            absoluteCancellationToken.ThrowIfCancellationRequested();
            if (plies > minDepth)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            var positionId = position.UniqueIdentifier;

            if (orderedMoves.TryGetValue(positionId, out var pseudoLegalMoves))
            {
                // We make sure that the dequeuing process doesn't affect deeper evaluations, in case the position is repeated
                orderedMoves.Remove(positionId);
                //Debug.Assert(pseudoLegalMoves.Count != 0);    // It can be 0 in stalemate positions
            }
            else
            {
                pseudoLegalMoves = new(position.AllPossibleMoves(killerMoves, plies).Select(i => (i, 1)));
            }

            if (plies >= depthLimit)
            {
                ++nodes;

                while (pseudoLegalMoves.TryDequeue(out var candidateMove, out _))
                {
                    if (new Position(position, candidateMove).WasProduceByAValidMove())
                    {
                        orderedMoves.Remove(positionId);
                        return QuiescenceSearch(position, positionHistory, movesWithoutCaptureOrPawnMove, Configuration.EngineSettings.QuiescenceSearchDepth, ref nodes, plies + 1, alpha, beta, cancellationToken, absoluteCancellationToken);
                    }
                }

                orderedMoves.Remove(positionId);
                return (position.EvaluateFinalPosition(plies, positionHistory, movesWithoutCaptureOrPawnMove), new Result { MaxDepth = plies });
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            PriorityQueue<Move, int> newPriorityQueue = new(pseudoLegalMoves.Count);

            while (pseudoLegalMoves.TryDequeue(out Move move, out _))
            {
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move);

                var oldValue = movesWithoutCaptureOrPawnMove;
                movesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, movesWithoutCaptureOrPawnMove);
                var repetitions = Utils.UpdatePositionHistory(newPosition, positionHistory);
                var (evaluation, bestMoveExistingMoveList) = NegaMax(newPosition, positionHistory, movesWithoutCaptureOrPawnMove, orderedMoves, killerMoves, minDepth, depthLimit, ref nodes, plies + 1, -beta, -alpha, cancellationToken, absoluteCancellationToken);
                movesWithoutCaptureOrPawnMove = oldValue;
                Utils.RevertPositionHistory(newPosition, positionHistory, repetitions);

                // Since SimplePriorityQueue has lower priority at the top, we do this before inverting the sign of the evaluation
                newPriorityQueue.Enqueue(move, evaluation);

                evaluation = -evaluation;

                PrintMove(plies, move, evaluation);

                if (evaluation > maxEval)
                {
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                // Fail-hard beta-cutoff
                if (evaluation >= beta)
                {
                    _logger.Trace($"Pruning: {bestMove} is enough");

                    // Add the non-evaluated moves with a higher priority than the existing one, so that they're evaluated later.
                    var nonEvaluatedMovesEval = -evaluation + 100_000;  // Using the inverted evaluation
                    while (pseudoLegalMoves.TryDequeue(out Move nonEvaluatedMove, out _))
                    {
                        newPriorityQueue.Enqueue(nonEvaluatedMove, nonEvaluatedMovesEval);
                    }
                    orderedMoves[positionId] = newPriorityQueue;

                    //if (!move.IsCapture())
                    {
                        killerMoves[1, plies] = killerMoves[0, plies];
                        killerMoves[0, plies] = move.EncodedMove;
                    }

                    return (beta, new Result());
                }

                alpha = Max(alpha, evaluation);
            }

            orderedMoves[positionId] = newPriorityQueue;
            if (bestMove is null)
            {
                ++nodes;
                return (position.EvaluateFinalPosition(plies, positionHistory, movesWithoutCaptureOrPawnMove), new Result());
            }

            // Node fails low
            Debug.Assert(existingMoveList is not null);
            existingMoveList!.Moves.Add(bestMove!.Value);

            return (maxEval, existingMoveList);
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
        public static (int Evaluation, Result MoveList) QuiescenceSearch(Position position, Dictionary<long, int> positionHistory, int movesWithoutCaptureOrPawnMove, int quiescenceDepthLimit, ref int nodes, int plies, int alpha, int beta, CancellationToken cancellationToken, CancellationToken absoluteCancellationToken)
        {
            absoluteCancellationToken.ThrowIfCancellationRequested();
            //cancellationToken.ThrowIfCancellationRequested();

            var staticEvaluation = position.StaticEvaluation(positionHistory, movesWithoutCaptureOrPawnMove);

            // Fail-hard beta-cutoff (updating alpha after this check)
            if (staticEvaluation >= beta)
            {
                ++nodes;
                PrintMessage(plies - 1, "Pruning before starting quiescence search");
                return (staticEvaluation, new Result() { MaxDepth = plies });
            }

            // Better move
            if (staticEvaluation > alpha)
            {
                alpha = staticEvaluation;
            }

            if (plies >= quiescenceDepthLimit)
            {
                ++nodes;
                return (alpha, new Result { MaxDepth = plies });   // Alpha?
            }

            var movesToEvaluate = position.AllCapturesMoves();

            if (!movesToEvaluate.Any())
            {
                ++nodes;
                return (staticEvaluation, new Result { MaxDepth = plies });  // TODO check if in check or drawn position
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            foreach (var move in movesToEvaluate)
            {
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move, isQuiescence: true);

                var oldValue = movesWithoutCaptureOrPawnMove;
                movesWithoutCaptureOrPawnMove = Utils.Update50movesRule(move, movesWithoutCaptureOrPawnMove);
                var repetitions = Utils.UpdatePositionHistory(newPosition, positionHistory);
                var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch(newPosition, positionHistory, movesWithoutCaptureOrPawnMove, quiescenceDepthLimit, ref nodes, plies + 1, -beta, -alpha, cancellationToken, absoluteCancellationToken);
                movesWithoutCaptureOrPawnMove = oldValue;
                Utils.RevertPositionHistory(newPosition, positionHistory, repetitions);

                evaluation = -evaluation;

                PrintMove(plies, move, evaluation);

                if (evaluation > maxEval)
                {
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                // Fail-hard beta-cutoff
                if (evaluation >= beta)
                {
                    _logger.Trace($"Pruning: {bestMove} is enough to discard this line");
                    return (maxEval, new Result()); // The refutation doesn't matter, since it'll be pruned
                }

                alpha = Max(alpha, evaluation); // TODO optimize branch prediction -> Should alpha be generally greater than eval?
            }

            if (bestMove is null)
            {
                ++nodes;

                var eval = position.AllPossibleMoves().Any(move => new Position(position, move).WasProduceByAValidMove())
                    ? position.StaticEvaluation(positionHistory, movesWithoutCaptureOrPawnMove)
                    : position.EvaluateFinalPosition(plies, positionHistory, movesWithoutCaptureOrPawnMove);

                return (eval, new Result() { MaxDepth = plies });
            }

            // Node fails low
            Debug.Assert(existingMoveList is not null);

            // If the best quiescence move produces no gain, don't add it to the PV
            // because there might be better, unexplored non-quiescence alternatives (unless a Zugzwang-style position with only captures as moves)
            if (maxEval >= alpha)
            {
                existingMoveList!.Moves.Add(bestMove!.Value);
            }

            return (alpha, existingMoveList);
        }
    }
}
