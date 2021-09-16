﻿using Lynx.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        public static SearchResult NegaMax_AlphaBeta_Quiescence_IDDFS(Position position, int minDepth, int? maxDepth, CancellationToken cancellationToken, CancellationToken absoluteCancellationToken)
        {
            int bestEvaluation = 0;
            Result? bestResult = new();
            int depth = 1;
            int nodes = 0;
            var sw = new Stopwatch();

            try
            {
                var orderedMoves = new Dictionary<string, PriorityQueue<Move, int>>(10_000);

                sw.Start();

                do
                {
                    absoluteCancellationToken.ThrowIfCancellationRequested();
                    if (depth > minDepth)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    nodes = 0;
                    (bestEvaluation, bestResult) = NegaMax_AlphaBeta_Quiescence_IDDFS(position, orderedMoves, minDepth: minDepth, depthLimit: depth, nodes: ref nodes, plies: 0, alpha: MinValue, beta: MaxValue, cancellationToken);
                } while (stopSearchCondition(++depth, maxDepth, bestEvaluation));
            }
            catch (OperationCanceledException)
            {
                Logger.Info("Search cancellation requested, best move will be returned");
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected error ocurred during the search, best move will be returned" +
                    Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
            }
            finally
            {
                sw.Stop();
            }

            bestResult?.Moves.Reverse();
            return new SearchResult(bestResult!.Moves.FirstOrDefault(), bestEvaluation, depth, bestResult!.MaxDepth ?? depth, nodes, sw.ElapsedMilliseconds, Convert.ToInt64(Math.Clamp(nodes / (0.001 * sw.ElapsedMilliseconds + 1), 0, Int64.MaxValue)), bestResult!.Moves);

            static bool stopSearchCondition(int depth, int? depthLimit, int bestEvaluation)
            {
                if (Math.Abs(bestEvaluation) > 0.1 * Position.CheckMateEvaluation)   // Mate detected
                {
                    return false;
                }

                if (depthLimit is not null)
                {
                    return depth <= depthLimit;
                }

                return true;
            }
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
        private static (int Evaluation, Result MoveList) NegaMax_AlphaBeta_Quiescence_IDDFS(Position position, Dictionary<string, PriorityQueue<Move, int>> orderedMoves, int minDepth, int depthLimit, ref int nodes, int plies, int alpha = MinValue, int beta = MaxValue, CancellationToken? cancellationToken = null, CancellationToken? absoluteCancellationToken = null)
        {
            absoluteCancellationToken?.ThrowIfCancellationRequested();
            if (plies + 1 > minDepth)
            {
                cancellationToken?.ThrowIfCancellationRequested();
            }

            var positionId = position.FEN();

            if (orderedMoves.TryGetValue(positionId, out var pseudoLegalMoves))
            {
                // We make sure that the dequeuing process doesn't affect deeper evaluations, in case the position is repeated
                orderedMoves.Remove(positionId);
                //Debug.Assert(pseudoLegalMoves.Count != 0);    // It can be 0 in stalemate positions
            }
            else
            {
                pseudoLegalMoves = new(position.AllPossibleMoves().Select(i => (i, 1)));
            }

            if (plies >= depthLimit)
            {
                ++nodes;

                while (pseudoLegalMoves.TryDequeue(out var candidateMove, out _))
                {
                    if (new Position(position, candidateMove).WasProduceByAValidMove())
                    {
                        orderedMoves.Remove(positionId);
                        return QuiescenceSearch_NegaMax_AlphaBeta(position, Configuration.EngineSettings.QuiescenceSearchDepth, ref nodes, plies + 1, alpha, beta, cancellationToken, absoluteCancellationToken);
                    }
                }

                orderedMoves.Remove(positionId);
                return (position.EvaluateFinalPosition_NegaMax(plies), new Result { MaxDepth = plies });
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

                var (evaluation, bestMoveExistingMoveList) = NegaMax_AlphaBeta_Quiescence_IDDFS(newPosition, orderedMoves, minDepth, depthLimit, ref nodes, plies + 1, -beta, -alpha, cancellationToken, absoluteCancellationToken);

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
                ++nodes;
                return (position.EvaluateFinalPosition_NegaMax(plies), new Result());
            }

            // Node fails low
            Debug.Assert(existingMoveList is not null);
            existingMoveList!.Moves.Add(bestMove!.Value);

            return (maxEval, existingMoveList);
        }
    }
}
