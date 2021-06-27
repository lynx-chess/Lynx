using Lynx.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        public static (int Evaluation, Result MoveList) NegaMax_AlphaBeta_Quiescence_IDDFS(Position position)
        {
            var orderedMoves = new Dictionary<string, PriorityQueue<Move, int>>(10_000);

            Unsafe.SkipInit(out int bestEvaluation);
            Unsafe.SkipInit(out Result bestResult);

            var sw = new Stopwatch();
            for (int depth = 1; depth <= Configuration.Parameters.Depth; ++depth)
            {
                (bestEvaluation, bestResult) = NegaMax_AlphaBeta_Quiescence_IDDFS(position, orderedMoves, depth);

                if (sw.ElapsedMilliseconds > 30_000 || bestEvaluation >= Position.CheckMateEvaluation - (Position.DepthFactor * depth)) // TODO uncomment
                {
                    break;
                }
            }

            return (bestEvaluation, bestResult);
        }

        /// <summary>
        /// NegaMax algorithm implementation using alpha-beta prunning, quiescence search and Iterative Deepeting Depth-First Search (IDDFS)
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
        private static (int Evaluation, Result MoveList) NegaMax_AlphaBeta_Quiescence_IDDFS(Position position, Dictionary<string, PriorityQueue<Move, int>> orderedMoves, int depthLimit, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var positionId = position.FEN();

            if (orderedMoves.TryGetValue(positionId, out var pseudoLegalMoves))
            {
                // We make sure that the dequeuing process doesn't affect deeper evaluations, in case the position is repeated
                orderedMoves.Remove(positionId);
                Debug.Assert(pseudoLegalMoves.Count != 0);
            }
            else
            {
                pseudoLegalMoves = new(position.AllPossibleMoves().Select(i => (i, 1)));
            }

            if (plies >= depthLimit)
            {
                var result = new Result();

                while (pseudoLegalMoves.TryDequeue(out var candidateMove, out _))
                {
                    if (new Position(position, candidateMove).WasProduceByAValidMove())
                    {
                        orderedMoves.Remove(positionId);
                        return QuiescenceSearch_NegaMax_AlphaBeta(position, Configuration.Parameters.QuiescenceSearchDepth, plies + 1, alpha, beta);
                    }
                }

                orderedMoves.Remove(positionId);
                return (position.EvaluateFinalPosition_NegaMax(plies), result);
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

                var (evaluation, bestMoveExistingMoveList) = NegaMax_AlphaBeta_Quiescence_IDDFS(newPosition, orderedMoves, depthLimit, plies + 1, -beta, -alpha);

                // Since SimplePriorityQueue has lower priority at the top, we do this before inverting the sign of the evaluation
                newPriorityQueue.Enqueue(move, evaluation);

                evaluation = -evaluation;

                PrintMove(plies, move, evaluation, position);

                if (evaluation > maxEval)
                {
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                // Fail-hard beta-cutoff
                if (evaluation >= beta)
                {
                    Logger.Trace($"Prunning: {bestMove} is enough");

                    // Add the non-evaluated moves with a higher priority than the existing one, so that they're evaluated later.
                    var nonEvaluatedMovesEval = -evaluation + 100_000;  // Using the inverted evaluation
                    while (pseudoLegalMoves.TryDequeue(out Move nonEvaluatedMove, out _))
                    {
                        newPriorityQueue.Enqueue(nonEvaluatedMove, nonEvaluatedMovesEval);
                    }
                    orderedMoves[positionId] = newPriorityQueue;

                    return (beta, new Result());
                }

                alpha = Max(alpha, evaluation);
            }

            orderedMoves[positionId] = newPriorityQueue;
            if (bestMove is null)
            {
                return (position.EvaluateFinalPosition_NegaMax(plies), new Result());
            }

            // Node fails low
            Debug.Assert(existingMoveList is not null);
            existingMoveList!.Moves.Add(bestMove!.Value);

            return (maxEval, existingMoveList);
        }
    }
}
