using Lynx.Model;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        /// <summary>
        /// NegaMax algorithm implementation using alpha-beta prunning and quiescence search
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
        public static (int Evaluation, Result MoveList) NegaMax_AlphaBeta_Quiescence(Position position, int depthLimit, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (plies >= depthLimit)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return QuiescenceSearch_NegaMax_AlphaBeta(position, Configuration.Parameters.QuiescenceSearchDepth, plies + 1, alpha, beta);
                }
                else
                {
                    return (position.EvaluateFinalPosition_NegaMax(plies), result);
                }
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
            {
                var move = pseudoLegalMoves[moveIndex];
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move);

                var (evaluation, bestMoveExistingMoveList) = NegaMax_AlphaBeta_Quiescence(newPosition, depthLimit, plies + 1, -beta, -alpha);
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
                    Logger.Trace($"Prunning: {bestMove} is enough to discard this line");
                    return (maxEval, new Result()); // The refutation doesn't matter, since it'll be pruned
                }

                alpha = Max(alpha, evaluation);
            }

            if (bestMove is null)
            {
                return (position.EvaluateFinalPosition_NegaMax(plies), new Result());
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
        public static (int Evaluation, Result MoveList) QuiescenceSearch_NegaMax_AlphaBeta(Position position, int quiescenceDepthLimit, int plies, int alpha, int beta)
        {
            var staticEvaluation = position.StaticEvaluation_NegaMax();

            // Fail-hard beta-cutoff (updating alpha after this check)
            if (staticEvaluation >= beta)
            {
                PrintMessage(plies - 1, "Prunning before starting quiescence search");
                return (staticEvaluation, new Result());
            }

            // Better move
            if (staticEvaluation > alpha)
            {
                alpha = staticEvaluation;
            }

            if (plies >= quiescenceDepthLimit)
            {
                return (alpha, new Result());   // Alpha?
            }

            var movesToEvaluate = position.AllCapturesMoves();

            if (movesToEvaluate.Count == 0)
            {
                return (staticEvaluation, new Result());  // TODO check if in check or drawn position
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            for (int moveIndex = 0; moveIndex < movesToEvaluate.Count; ++moveIndex)
            {
                var move = movesToEvaluate[moveIndex];
                var newPosition = new Position(position, move);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move, isQuiescence: true);

                var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch_NegaMax_AlphaBeta(newPosition, quiescenceDepthLimit, plies + 1, -beta, -alpha);
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
                    Logger.Trace($"Prunning: {bestMove} is enough to discard this line");
                    return (maxEval, new Result()); // The refutation doesn't matter, since it'll be pruned
                }

                alpha = Max(alpha, evaluation);
            }

            if (bestMove is null)
            {
                return (position.EvaluateFinalPosition_NegaMax(plies), new Result());
            }

            // Node fails low
            Debug.Assert(existingMoveList is not null);
            existingMoveList!.Moves.Add(bestMove!.Value);

            return (alpha, existingMoveList);
        }

    }
}
