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
        //private static int NegaMax_Theoretical(Position position, int depth, bool isWhite, int alpha = int.MinValue, int beta = int.MaxValue)
        //{
        //    static bool IsGameFinished(Position position) => throw new();

        //    if (depth == 0 || IsGameFinished(position))
        //    {
        //        return position.StaticEvaluation_NegaMax();
        //    }

        //    var maxEval = int.MinValue;
        //    var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
        //    for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
        //    {
        //        var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
        //        if (!newPosition.IsValid())
        //        {
        //            continue;
        //        }

        //        var eval = -NegaMax_Theoretical(newPosition, depth - 1, isWhite: !isWhite, -beta, -alpha);

        //        maxEval = Max(maxEval, eval);   // Branch prediction optimized - should have started with most likely positions
        //        alpha = Max(alpha, eval);       // maxTODO optimize branch prediction -> Should alpha be generally greater than eval?

        //        if (beta <= alpha)
        //        {
        //            break;
        //        }
        //    }

        //    return maxEval;
        //}

        /// <summary>
        /// NegaMax algorithm implementation
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
        public static (int Evaluation, Result MoveList) NegaMax_InitialImplementation(Position position, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (plies == Configuration.Parameters.Depth)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return (position.StaticEvaluation_NegaMax(), result);
                }
                else
                {
                    return (position.EvaluateFinalPosition(plies), result);
                }
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
            {
                var move = pseudoLegalMoves[moveIndex];
                var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move);

                var (evaluation, bestMoveExistingMoveList) = NegaMax_InitialImplementation(newPosition, plies + 1, -beta, -alpha);
                evaluation = -evaluation;

                PrintMove(plies, move, evaluation, position);

                if (evaluation > maxEval)
                {
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                alpha = Max(alpha, evaluation);

                if (beta <= alpha)
                {
                    Logger.Trace($"Prunning: {bestMove} is enough");
                    break;
                }
            }

            if (bestMove is not null)
            {
                Debug.Assert(existingMoveList is not null);
                existingMoveList!.Moves.Add(bestMove!.Value);

                return (maxEval, existingMoveList);
            }
            else
            {
                return (position.EvaluateFinalPosition(plies), new Result());
            }
        }

        /// <summary>
        /// NegaMax algorithm implementation
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
        public static (int Evaluation, Result MoveList) NegaMax_Quiescence(Position position, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (plies == Configuration.Parameters.Depth)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return QuiescenceSearch_NegaMax(position, plies, alpha, beta);
                }
                else
                {
                    return (position.EvaluateFinalPosition(plies), result);
                }
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
            {
                var move = pseudoLegalMoves[moveIndex];
                var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move);

                var (evaluation, bestMoveExistingMoveList) = NegaMax_Quiescence(newPosition, plies + 1, -beta, -alpha);
                evaluation = -evaluation;

                PrintMove(plies, move, evaluation, position);

                if (evaluation > maxEval)
                {
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                alpha = Max(alpha, evaluation);

                if (beta <= alpha)
                {
                    Logger.Trace($"Prunning: {bestMove} is enough");
                    break;
                }
            }

            if (bestMove is not null)
            {
                Debug.Assert(existingMoveList is not null);
                existingMoveList!.Moves.Add(bestMove!.Value);

                return (maxEval, existingMoveList);
            }
            else
            {
                return (position.EvaluateFinalPosition(plies), new Result());
            }
        }

        /// <summary>
        /// Quiescence search implementation, NegaMax style
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
        public static (int Evaluation, Result MoveList) QuiescenceSearch_NegaMax(Position position, int plies, int alpha, int beta)
        {
            var staticEvaluation = position.StaticEvaluation_NegaMax();

            alpha = Max(alpha, staticEvaluation);

            if (beta <= alpha || plies >= Configuration.Parameters.QuescienceSearchDepth)
            {
                PrintMessage(plies - 1, "Prunning before starting quiescence search");
                return (alpha, new Result());
            }

            var movesToEvaluate = position.AllCapturesMoves();

            Move? bestMove = null;
            Result? existingMoveList = null;

            var maxEval = MinValue;

            for (int moveIndex = 0; moveIndex < movesToEvaluate.Count; ++moveIndex)
            {
                var move = movesToEvaluate[moveIndex];
                var newPosition = new Position(position, movesToEvaluate[moveIndex]);
                if (!newPosition.WasProduceByAValidMove())
                {
                    continue;
                }

                PrintPreMove(position, plies, move, isQuiescence: true);

                var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch_NegaMax(newPosition, plies + 1, -beta, -alpha);
                evaluation = -evaluation;

                //if(evaluation == -996000000)
                //{
                //    PrintMessage(plies, $"MaxEval: {maxEval}, Alpha: {alpha}, beta: {beta}");
                //}

                if (evaluation > maxEval)
                {
                    maxEval = evaluation;
                    existingMoveList = bestMoveExistingMoveList;
                    bestMove = move;
                }

                alpha = Max(alpha, evaluation);       // TODO optimize branch prediction -> Should alpha be generally greater than eval?

                PrintMove(plies, move, evaluation, position, isQuiescence: true, beta <= alpha);

                if (beta <= alpha)
                {
                    break;
                }
            }

            if (bestMove is not null)
            {
                Debug.Assert(existingMoveList is not null);
                existingMoveList!.Moves.Add(bestMove!.Value);

                return (maxEval, existingMoveList);
            }
            else
            {
                // TODO: What happens with positions like r1b1k2r/p1p3pp/2B3n1/8/1bP5/4K3/P5Pq/R1BQ3R b kq - 0 1 or r1b2rk1/p1p3pp/2p3n1/4q3/1bP1B3/4K3/P5PP/R1BQ3R w - - 2 3
                // MovesToEvaluate that are captures, there are 2, but none of them is valid
                return movesToEvaluate.Count > 0
                    ? (position.EvaluateFinalPosition(plies), new Result())
                    : (staticEvaluation, new Result());
            }
        }
    }
}
