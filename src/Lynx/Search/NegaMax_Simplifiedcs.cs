using Lynx.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static (int Evaluation, Result MoveList) NegaMax_AlphaBeta_Quiescence_Simplified(Position position, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();
            Console.WriteLine(Configuration.Parameters.Depth);
            if (plies >= Configuration.Parameters.Depth)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return QuiescenceSearch_NegaMax_AlphaBeta_Simplified(position, plies + 1, alpha, beta);
                }
                else
                {
                    return (position.EvaluateFinalPosition_NegaMax(plies), result);
                }
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
            {
                var move = pseudoLegalMoves[moveIndex];
                var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move);

                var (evaluation, bestMoveExistingMoveList) = NegaMax_AlphaBeta_Quiescence(newPosition, plies + 1, -beta, -alpha);
                evaluation = -evaluation;

                PrintMove(plies, move, evaluation, position);

                // Fail-hard beta-cutoff
                if (evaluation >= beta)
                {
                    Logger.Trace($"Prunning: {bestMove} is enough");

                    return (beta, existingMoveList ?? new Result());
                }

                // Better move found
                if (evaluation > alpha)
                {
                    // PV node (move)
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                    alpha = evaluation;
                }
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
        public static (int Evaluation, Result MoveList) QuiescenceSearch_NegaMax_AlphaBeta_Simplified(Position position, int plies, int alpha, int beta)
        {
            var staticEvaluation = position.StaticEvaluation_NegaMax();

            // Fail-hard beta-cutoff (updating alpha after this check)
            if (staticEvaluation >= beta)
            {
                PrintMessage(plies - 1, "Prunning before starting quiescence search");
                return (beta, new Result());
            }

            // Better move
            if (staticEvaluation > alpha)
            {
                alpha = staticEvaluation;
            }

            if (plies >= Configuration.Parameters.QuiescenceSearchDepth)
            {
                return (alpha, new Result());   // Alpha?
            }

            var movesToEvaluate = position.AllCapturesMoves();

            Move? bestMove = null;
            Result? existingMoveList = null;

            for (int moveIndex = 0; moveIndex < movesToEvaluate.Count; ++moveIndex)
            {
                var move = movesToEvaluate[moveIndex];
                var newPosition = new Position(position, movesToEvaluate[moveIndex]);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move, isQuiescence: true);

                var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch_NegaMax_AlphaBeta(newPosition, plies + 1, -beta, -alpha);
                evaluation = -evaluation;

                // Fail-hard beta-cutoff (updating alpha after this check)
                if (evaluation >= beta)
                {
                    Logger.Trace($"Prunning: {bestMove} is enough");

                    return (beta, new Result());
                }

                // Better move found
                if (evaluation > alpha)
                {
                    // PV node (move)
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                    alpha = evaluation;
                }

                PrintMove(plies, move, evaluation, position, isQuiescence: true, beta <= alpha);
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

        /// <summary>
        /// Quiescence search implementation, NegaMax alpha-beta style
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
        public static (int Evaluation, Result MoveList) QuiescenceSearch_NegaMax_AlphaBeta_Simplified_2(Position position, int plies, int alpha, int beta)
        {
            var staticEvaluation = position.StaticEvaluation_NegaMax();

            // Fail-hard beta-cutoff (updating alpha after this check)
            if (staticEvaluation >= beta)
            {
                PrintMessage(plies - 1, "Prunning before starting quiescence search");
                return (beta, new Result());
            }

            // Better move
            if (staticEvaluation > alpha)
            {
                alpha = staticEvaluation;
            }

            if (plies >= Configuration.Parameters.QuiescenceSearchDepth)
            {
                return (alpha, new Result());   // Alpha?
            }

            var movesToEvaluate = position.AllCapturesMoves();

            Move? bestMove = null;
            Result? existingMoveList = null;
            int maxEval = MinValue;

            for (int moveIndex = 0; moveIndex < movesToEvaluate.Count; ++moveIndex)
            {
                var move = movesToEvaluate[moveIndex];
                var newPosition = new Position(position, movesToEvaluate[moveIndex]);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move, isQuiescence: true);

                var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch_NegaMax_AlphaBeta(newPosition, plies + 1, -beta, -alpha);
                evaluation = -evaluation;

                // Better move found
                if (evaluation > maxEval)
                {
                    // PV node (move)
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                PrintMove(plies, move, evaluation, position, isQuiescence: true, beta <= alpha);

                // Fail-hard beta-cutoff (updating alpha after this check)
                if (beta <= alpha)
                {
                    return (beta, new Result());
                }

                alpha = Max(alpha, MaxValue);
            }

            if (bestMove is null)
            {
                return (position.EvaluateFinalPosition_NegaMax(plies), new Result());
            }

            Debug.Assert(existingMoveList is not null);
            existingMoveList!.Moves.Add(bestMove!.Value);

            return (maxEval, existingMoveList);
        }
    }
}
