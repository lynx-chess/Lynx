using Lynx.Model;
using System.Collections.Generic;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        /// <summary>
        /// MiniMax algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depth"></param>
        /// <param name="isWhite"></param>
        /// <returns></returns>
        private static int Theoretical_MiniMax(Position position, int depth, bool isWhite)
        {
            static bool IsGameFinished(Position position) => throw new();

            if (depth == 0 || IsGameFinished(position))
            {
                return position.StaticEvaluation();
            }

            if (isWhite)
            {
                var maxEval = int.MinValue;

                var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    var eval = Theoretical_MiniMax(newPosition, depth - 1, isWhite: false);
                    maxEval = Max(maxEval, eval);
                }

                return maxEval;
            }
            else
            {
                var minEval = int.MaxValue;

                var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    var eval = Theoretical_MiniMax(newPosition, depth - 1, isWhite: true);
                    minEval = Min(minEval, eval);
                }

                return minEval;
            }
        }

        public class Result
        {
            public List<Move> Moves { get; set; } = new List<Move>(150);

            public Result()
            {

            }
            public Result(Move move)
            {
                Moves = new List<Move>(150);
                Moves.Add(move);
            }
        }

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

            var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
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
                    return EvaluateFinalPosition(position);
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
                    return EvaluateFinalPosition(position);
                }
            }
        }

        /// <summary>
        /// Second MiniMax algorithm implementation
        /// Trying to return the right moves
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depth"></param>
        public static (int Evaluation, Result? MoveList) MiniMax_InitialImplementation_2(Position position, int depth)
        {
            if (depth == 0)
            {
                return (position.StaticEvaluation(), null);
            }

            var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
            Move? bestMove = null;
            Result? existingMoveList = null;

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

                    var (evaluation, bestMoveExistingMoveList) = MiniMax_InitialImplementation_2(newPosition, depth - 1);

                    if (evaluation > maxEval)
                    {
                        maxEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = pseudoLegalMoves[moveIndex];
                    }
                }

                if (bestMove is not null)
                {
                    if (existingMoveList is null)
                    {
                        existingMoveList = new Result(bestMove!.Value);
                    }
                    else
                    {
                        existingMoveList.Moves.Add(bestMove!.Value);
                    }
                    return (maxEval, existingMoveList);
                }
                else
                {
                    return (EvaluateFinalPosition(position, depth), existingMoveList);
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

                    var (evaluation, bestMoveExistingMoveList) = MiniMax_InitialImplementation_2(newPosition, depth - 1);

                    if (evaluation < minEval)
                    {
                        minEval = evaluation;
                        existingMoveList = bestMoveExistingMoveList;
                        bestMove = pseudoLegalMoves[moveIndex];
                    }
                }

                if (bestMove is not null)
                {
                    if (existingMoveList is null)
                    {
                        existingMoveList = new Result(bestMove!.Value);
                    }
                    else
                    {
                        existingMoveList.Moves.Add(bestMove!.Value);
                    }
                    return (minEval, existingMoveList);
                }
                else
                {
                    return (EvaluateFinalPosition(position, depth), existingMoveList);
                }
            }
        }
    }
}
