using Lynx.Model;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        /// <summary>
        /// MiniMax algorithm implementation using alpha-beta prunning
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
        public static (int Evaluation, Result MoveList) MiniMax_AlphaBeta(Position position, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (plies == Configuration.Parameters.Depth)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return (position.StaticEvaluation_MiniMax(), result);
                }
                else
                {
                    return (position.EvaluateFinalPosition_AlphaBeta(plies), result);
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
                    var newPosition = new Position(position, move);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move);

                    var (evaluation, bestMoveExistingMoveList) = MiniMax_AlphaBeta(newPosition, plies + 1, alpha, beta);

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
                    return (position.EvaluateFinalPosition_AlphaBeta(plies), new Result());
                }
            }
            else
            {
                var minEval = MaxValue;

                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var move = pseudoLegalMoves[moveIndex];
                    var newPosition = new Position(position, move);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move);

                    var (evaluation, bestMoveExistingMoveList) = MiniMax_AlphaBeta(newPosition, plies + 1, alpha, beta);

                    PrintMove(plies, move, evaluation, position);

                    beta = Min(beta, evaluation);

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
                    return (position.EvaluateFinalPosition_AlphaBeta(plies), new Result());
                }
            }
        }

        /// <summary>
        /// MiniMax algorithm implementation using alpha-beta prunning and quiescence search
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
        public static (int Evaluation, Result MoveList) MiniMax_AlphaBeta_Quiescence(Position position, int plies = default, int alpha = MinValue, int beta = MaxValue)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (plies == Configuration.Parameters.Depth)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).WasProduceByAValidMove()))
                {
                    return QuiescenceSearch_MiniMax_AlphaBeta(position, plies + 1, alpha, beta);
                }
                else
                {
                    return (position.EvaluateFinalPosition_AlphaBeta(plies), result);
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
                    var newPosition = new Position(position, move);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move);

                    var (evaluation, bestMoveExistingMoveList) = MiniMax_AlphaBeta_Quiescence(newPosition, plies + 1, alpha, beta);

                    if (evaluation > maxEval)
                    {
                        maxEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
                    }

                    alpha = Max(alpha, evaluation);

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
                    return (position.EvaluateFinalPosition_AlphaBeta(plies), new Result());
                }
            }
            else
            {
                var minEval = MaxValue;

                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var move = pseudoLegalMoves[moveIndex];
                    var newPosition = new Position(position, move);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move);

                    var (evaluation, bestMoveExistingMoveList) = MiniMax_AlphaBeta_Quiescence(newPosition, plies + 1, alpha, beta);

                    if (evaluation < minEval)
                    {
                        minEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
                    }

                    beta = Min(beta, evaluation);

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
                    return (position.EvaluateFinalPosition_AlphaBeta(plies), new Result());
                }
            }
        }

        /// <summary>
        /// Quiescence search implementation, MiniMax alpha-beta style
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
        public static (int Evaluation, Result MoveList) QuiescenceSearch_MiniMax_AlphaBeta(Position position, int plies, int alpha, int beta)
        {
            var staticEvaluation = position.StaticEvaluation_MiniMax();

            if (position.Side == Side.White)
            {
                alpha = Max(alpha, staticEvaluation);
            }
            else
            {
                beta = Min(beta, staticEvaluation);
            }

            if (beta <= alpha || plies >= Configuration.Parameters.QuiescenceSearchDepth)
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
                    var newPosition = new Position(position, move);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move, isQuiescence: true);

                    var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch_MiniMax_AlphaBeta(newPosition, plies + 1, alpha, beta);

                    if (evaluation > maxEval)
                    {
                        maxEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
                    }

                    alpha = Max(alpha, evaluation);

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
                    return movesToEvaluate.Count > 0
                        ? (position.EvaluateFinalPosition_AlphaBeta(plies), new Result())
                        : (staticEvaluation, new Result());
                }
            }
            else
            {
                var minEval = MaxValue;

                for (int moveIndex = 0; moveIndex < movesToEvaluate.Count; ++moveIndex)
                {
                    var move = movesToEvaluate[moveIndex];
                    var newPosition = new Position(position, move);
                    if (!newPosition.WasProduceByAValidMove())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move, isQuiescence: true);

                    var (evaluation, bestMoveExistingMoveList) = QuiescenceSearch_MiniMax_AlphaBeta(newPosition, plies + 1, alpha, beta);

                    if (evaluation < minEval)
                    {
                        minEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
                    }

                    beta = Min(beta, evaluation);

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
                    return movesToEvaluate.Count > 0
                        ? (position.EvaluateFinalPosition_AlphaBeta(plies), new Result())
                        : (staticEvaluation, new Result());
                }
            }
        }
    }
}
