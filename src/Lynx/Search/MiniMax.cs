using Lynx.Model;
using System.Diagnostics;
using System.Linq;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        /// <summary>
        /// MiniMax algorithm implementation
        /// I quickly made up a possibly wrong way of tracking the moves
        /// DepthLeft decreases instead of increaasing
        /// This currently doesn`t fully work as expected due to the Position.Evaluate function, which favours mates in longer lines
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depthLeft"></param>
        /// <param name="evaluation"></param>
        public static int MiniMax_InitialImplementation(Position position, int depthLeft, Result evaluation)
        {
            if (depthLeft == 0)
            {
                return position.StaticEvaluation();
            }

            var pseudoLegalMoves = position.AllPossibleMoves();
            Move? bestMove = null;

            if (position.Side == Side.White)
            {
                var maxEval = MinValue;
                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    var eval = MiniMax_InitialImplementation(newPosition, depthLeft - 1, evaluation);
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
                    return position.EvaluateFinalPosition_AlphaBeta(depthLeft);
                }
            }
            else
            {
                var minEval = MaxValue;

                for (int moveIndex = 0; moveIndex < pseudoLegalMoves.Count; ++moveIndex)
                {
                    var newPosition = new Position(position, pseudoLegalMoves[moveIndex]);
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    var eval = MiniMax_InitialImplementation(newPosition, depthLeft - 1, evaluation);

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
                    return position.EvaluateFinalPosition_AlphaBeta(depthLeft);
                }
            }
        }

        /// <summary>
        /// Second MiniMax algorithm implementation
        /// Tracks the right moves back to the user
        /// </summary>
        /// <param name="position"></param>
        /// <param name="plies"></param>
        public static (int Evaluation, Result MoveList) MiniMax(Position position, int plies = default)
        {
            var pseudoLegalMoves = position.AllPossibleMoves();

            if (plies == Configuration.Parameters.Depth)
            {
                var result = new Result();
                if (pseudoLegalMoves.Any(move => new Position(position, move).IsValid()))
                {
                    return (position.StaticEvaluation(), result);
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
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move);

                    var (evaluation, bestMoveExistingMoveList) = MiniMax(newPosition, plies + 1);

                    PrintMove(plies, move, evaluation, position);

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
                    if (!newPosition.IsValid())
                    {
                        continue;
                    }

                    PrintPreMove(position, plies, move);

                    var (evaluation, bestMoveExistingMoveList) = MiniMax(newPosition, plies + 1);

                    PrintMove(plies, move, evaluation, position);

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
                    return (position.EvaluateFinalPosition_AlphaBeta(plies), new Result());
                }
            }
        }
    }
}
