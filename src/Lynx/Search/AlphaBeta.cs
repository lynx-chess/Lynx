using Lynx.Model;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        //private static int AlphaBeta_Theoretical(Position position, int depth, bool isWhite, int alpha = MinValue, int beta = MaxValue)
        //{
        //    static bool IsGameFinished(Position position) => throw new();

        //    if (depth == 0 || IsGameFinished(position))
        //    {
        //        return position.StaticEvaluation();
        //    }

        //    if (isWhite)
        //    {
        //        var maxEval = MinValue;

        //        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
        //        for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
        //        {
        //            var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
        //            if (!newPosition.IsValid())
        //            {
        //                continue;
        //            }

        //            var eval = AlphaBeta_Theoretical(newPosition, depth - 1, isWhite: true, alpha, beta);
        //            maxEval = Max(maxEval, eval);   // Branch prediction optimized - should have started with most likely positions
        //            alpha = Max(alpha, eval);       // maxTODO optimize branch prediction -> Should alpha be generally greater than eval?

        //            if (beta <= alpha)
        //            {
        //                break;
        //            }
        //        }

        //        return maxEval;
        //    }
        //    else
        //    {
        //        var minEval = MaxValue;

        //        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
        //        for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
        //        {
        //            var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
        //            if (!newPosition.IsValid())
        //            {
        //                continue;
        //            }

        //            var eval = AlphaBeta_Theoretical(newPosition, depth - 1, isWhite: false, alpha, beta);
        //            minEval = Min(minEval, eval);   // Branch prediction optimized - should have started with most likely positions
        //            beta = Min(beta, eval);        // TODO optimize branch prediction -> Should beta be generally less than eval?

        //            if (beta <= alpha)
        //            {
        //                break;
        //            }
        //        }

        //        return minEval;
        //    }
        //}

        /// <summary>
        /// Alpha-beta algorithm implementation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depthLeft"></param>
        /// <param name="alpha">
        /// Best score White can achieve, assuming best play by Black.
        /// Defaults to the worse possible score for white, MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Black can achieve, assuming best play by White
        /// Defaults to the worse possible score for Black, MaxValue
        /// </param>
        /// <returns></returns>
        public static (int Evaluation, Result MoveList) AlphaBeta_InitialImplementation(Position position, int depthLeft, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (depthLeft == 0)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return (position.StaticEvaluation(), result);
                }
                else
                {
                    return (position.EvaluateFinalPosition(depthLeft), result);
                }
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            if (position.Side == Side.White)
            {
                var maxEval = MinValue;

                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var move = pseudoLegalMoves[moveIndex];
                    var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, depthLeft, move);

                    var (evaluation, bestMoveExistingMoveList) = AlphaBeta_InitialImplementation(newPosition, depthLeft - 1, alpha, beta);

                    PrintMove(depthLeft, move, evaluation, position);

                    if (evaluation > maxEval)
                    {
                        maxEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
                    }

                    // maxEval = Max(maxEval, evaluation);   // Branch prediction optimized - should have started with most likely positions
                    alpha = Max(alpha, evaluation);       // TODO optimize branch prediction -> Should alpha be generally greater than eval?

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
                    return (position.EvaluateFinalPosition(depthLeft), new Result());
                }
            }
            else
            {
                var minEval = MaxValue;

                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var move = pseudoLegalMoves[moveIndex];
                    var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, depthLeft, move);

                    var (evaluation, bestMoveExistingMoveList) = AlphaBeta_InitialImplementation(newPosition, depthLeft - 1, alpha, beta);

                    PrintMove(depthLeft, move, evaluation, position);

                    // minEval = Min(minEval, evaluation);   // Branch prediction optimized - should have started with most likely positions
                    beta = Min(beta, evaluation);        // TODO optimize branch prediction -> Should beta be generally less than eval?

                    if (evaluation < minEval)
                    {
                        minEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
                    }

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                if (bestMove is not null)
                {
                    Debug.Assert(existingMoveList is not null);
                    existingMoveList!.Moves.Add(bestMove!.Value);

                    return (minEval, existingMoveList);
                }
                else
                {
                    return (position.EvaluateFinalPosition(depthLeft), new Result());
                }
            }
        }

        /// <summary>
        /// Alpha-beta algorithm implementation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="plies"></param>
        /// <param name="alpha">
        /// Best score White can achieve, assuming best play by Black.
        /// Defaults to the worse possible score for white, MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Black can achieve, assuming best play by White
        /// Defaults to the worse possible score for Black, MaxValue
        /// </param>
        /// <returns></returns>
        public static (int Evaluation, Result MoveList) AlphaBeta_Quiescence(Position position, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (plies == Configuration.Parameters.Depth)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return QuiescenceSearch_AlphaBeta(position, plies, alpha, beta);
                }
                else
                {
                    return (position.EvaluateFinalPosition(plies), result);
                }
            }

            Move? bestMove = null;
            Result? existingMoveList = null;

            if (position.Side == Side.White)
            {
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

                    var (evaluation, bestMoveExistingMoveList) = AlphaBeta_Quiescence(newPosition, plies + 1, alpha, beta);

                    if (evaluation > maxEval)
                    {
                        maxEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
                    }

                    // maxEval = Max(maxEval, evaluation);   // Branch prediction optimized - should have started with most likely positions
                    alpha = Max(alpha, evaluation);       // TODO optimize branch prediction -> Should alpha be generally greater than eval?

                    PrintMove(plies, move, evaluation, position, beta <= alpha);

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
                    return (position.EvaluateFinalPosition(plies), new Result());
                }
            }
            else
            {
                var minEval = MaxValue;

                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var move = pseudoLegalMoves[moveIndex];
                    var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move);

                    var (evaluation, bestMoveExistingMoveList) = AlphaBeta_Quiescence(newPosition, plies + 1, alpha, beta);

                    // minEval = Min(minEval, evaluation);   // Branch prediction optimized - should have started with most likely positions
                    beta = Min(beta, evaluation);        // TODO optimize branch prediction -> Should beta be generally less than eval?

                    if (evaluation < minEval)
                    {
                        minEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
                    }

                    PrintMove(plies, move, evaluation, position, beta <= alpha);

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                if (bestMove is not null)
                {
                    Debug.Assert(existingMoveList is not null);
                    existingMoveList!.Moves.Add(bestMove!.Value);

                    return (minEval, existingMoveList);
                }
                else
                {
                    return (position.EvaluateFinalPosition(plies), new Result());
                }
            }
        }

        /// <summary>
        /// Quiescence search implementation, AlphaBeta style
        /// </summary>
        /// <param name="position"></param>
        /// <param name="plies"></param>
        /// <param name="alpha">
        /// Best score White can achieve, assuming best play by Black.
        /// Defaults to the worse possible score for white, MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Black can achieve, assuming best play by White
        /// Defaults to the works possible score for Black, MaxValue
        /// </param>
        /// <returns></returns>
        public static (int Evaluation, Result MoveList) QuiescenceSearch_AlphaBeta(Position position, int plies, int alpha, int beta)
        {
            var staticEvaluation = position.StaticEvaluation();

            if (position.Side == Side.White)
            {
                alpha = Max(alpha, staticEvaluation);
            }
            else
            {
                beta = Min(beta, staticEvaluation);
            }

            if (beta <= alpha || plies >= Configuration.Parameters.QuescienceSearchDepth)
            {
                PrintMessage(plies - 1, "Prunning before starting quiescence search");

                return position.Side == Side.White
                    ? (alpha, new Result())
                    : (beta, new Result());
            }

            var movesToEvaluate = position.AllCapturesMoves();

            Move? bestMove = null;
            Result? existingMoveList = null;

            if (position.Side == Side.White)
            {
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

                    var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch_AlphaBeta(newPosition, plies + 1, alpha, beta);

                    //if (evaluation == 996000000)
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
            else
            {
                var minEval = MaxValue;

                for (int moveIndex = 0; moveIndex < movesToEvaluate.Count; ++moveIndex)
                {
                    var move = movesToEvaluate[moveIndex];
                    var newPosition = new Position(position, movesToEvaluate[moveIndex]);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move, isQuiescence: true);

                    var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch_AlphaBeta(newPosition, plies + 1, alpha, beta);

                    if (evaluation < minEval)
                    {
                        minEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
                    }

                    // minEval = Min(minEval, evaluation);   // Branch prediction optimized - should have started with most likely positions
                    beta = Min(beta, evaluation);        // TODO optimize branch prediction -> Should beta be generally less than eval?

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

                    return (minEval, existingMoveList);
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
}
