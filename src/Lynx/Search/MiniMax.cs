using Lynx.Model;
using System.Diagnostics;
using System.Linq;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        //private static int Theoretical_MiniMax(Position position, int depth, bool isWhite)
        //{
        //    static bool IsGameFinished(Position position) => throw new();

        //    if (depth == 0 || IsGameFinished(position))
        //    {
        //        return position.StaticEvaluation();
        //    }

        //    if (isWhite)
        //    {
        //        var maxEval = int.MinValue;

        //        var pseudoLegalMoves = position.AllPossibleMoves();
        //        for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
        //        {
        //            var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
        //            if (!newPosition.IsValid())
        //            {
        //                continue;
        //            }

        //            var eval = Theoretical_MiniMax(newPosition, depth - 1, isWhite: false);
        //            maxEval = Max(maxEval, eval);
        //        }

        //        return maxEval;
        //    }
        //    else
        //    {
        //        var minEval = int.MaxValue;

        //        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
        //        for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
        //        {
        //            var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
        //            if (!newPosition.IsValid())
        //            {
        //                continue;
        //            }

        //            var eval = Theoretical_MiniMax(newPosition, depth - 1, isWhite: true);
        //            minEval = Min(minEval, eval);
        //        }

        //        return minEval;
        //    }
        //}

        /// <summary>
        /// MiniMax algorithm implementation
        /// I quickly made up a possibly wrong way of tracking the moves
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depth"></param>
        public static int MiniMax_InitialImplementation(Position position, int depth, Result evaluation)
        {
            if (depth == 0)
            {
                return position.StaticEvaluation();
            }

            var pseudoLegalMoves = position.AllPossibleMoves();
            Move? bestMove = null;

            if (position.Side == Side.White)
            {
                var maxEval = int.MinValue;
                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    var eval = MiniMax_InitialImplementation(newPosition, depth - 1, evaluation);
                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        bestMove = pseudoLegalMoves[moveIndex];
                    }
                }

                if (bestMove is not null)
                {
                    evaluation.Moves.Add(bestMove!.Value);
                    return maxEval;
                }
                else // No IsValid() positions found -> Draw by Stalemate or Loss by Checkmate
                {
                    return position.EvaluateFinalPosition(depth);
                }
            }
            else
            {
                var minEval = int.MaxValue;

                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    var eval = MiniMax_InitialImplementation(newPosition, depth - 1, evaluation);

                    if (eval < minEval)
                    {
                        minEval = eval;
                        bestMove = pseudoLegalMoves[moveIndex];
                    }
                }

                if (bestMove is not null)
                {
                    evaluation.Moves.Add(bestMove!.Value);
                    return minEval;
                }
                else
                {
                    return position.EvaluateFinalPosition(depth);
                }
            }
        }

        /// <summary>
        /// Second MiniMax algorithm implementation
        /// Tracks the right moves back to the user
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depthLeft"></param>
        public static (int Evaluation, Result MoveList) MiniMax(Position position, int depthLeft)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (depthLeft == 0)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).IsValid()))
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
                var maxEval = int.MinValue;

                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var move = pseudoLegalMoves[moveIndex];
                    var newPosition = new Position(position, move);
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    PrintPreMove(position, depthLeft, move);

                    var (evaluation, bestMoveExistingMoveList) = MiniMax(newPosition, depthLeft - 1);

                    PrintMove(depthLeft, move, evaluation, position);

                    if (evaluation > maxEval)
                    {
                        maxEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
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
                var minEval = int.MaxValue;

                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var move = pseudoLegalMoves[moveIndex];
                    var newPosition = new Position(position, move);
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    PrintPreMove(position, depthLeft, move);

                    var (evaluation, bestMoveExistingMoveList) = MiniMax(newPosition, depthLeft - 1);

                    PrintMove(depthLeft, move, evaluation, position);

                    if (evaluation < minEval)
                    {
                        minEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = move;
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
    }
}
