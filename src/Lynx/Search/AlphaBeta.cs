using Lynx.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        /// <summary>
        /// Alpha-beta algorithm
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depth"></param>
        /// <param name="isWhite"></param>
        /// <param name="alpha">
        /// Best score Shite can achieve, assuming best play by Black.
        /// Defaults to the worse possible score for white, Int.MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Black can achieve, assuming best play by White
        /// Defaults to the works possible score for Black, Int.MaxValue
        /// </param>
        /// <returns></returns>
        private static int AlphaBeta_Theoretical(Position position, int depth, bool isWhite, int alpha = int.MinValue, int beta = int.MaxValue)
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

                    var eval = AlphaBeta_Theoretical(newPosition, depth - 1, isWhite: true, alpha, beta);
                    maxEval = Max(maxEval, eval);   // Branch prediction optimized - should have started with most likely positions
                    alpha = Max(alpha, eval);       // maxTODO optimize branch prediction -> Should alpha be generally greater than eval?

                    if (beta <= alpha)
                    {
                        break;
                    }
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

                    var eval = AlphaBeta_Theoretical(newPosition, depth - 1, isWhite: false, alpha, beta);
                    minEval = Min(minEval, eval);   // Branch prediction optimized - should have started with most likely positions
                    beta = Min(beta, eval);        // TODO optimize branch prediction -> Should beta be generally less than eval?

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return minEval;
            }
        }

        /// <summary>
        /// Alpha-beta algorithm implementation
        /// </summary>
        /// <param name="position"></param>
        /// <param name="depth"></param>
        /// <param name="alpha">
        /// Best score Shite can achieve, assuming best play by Black.
        /// Defaults to the worse possible score for white, Int.MinValue.
        /// </param>
        /// <param name="beta">
        /// Best score Black can achieve, assuming best play by White
        /// Defaults to the works possible score for Black, Int.MaxValue
        /// </param>
        /// <returns></returns>
        private static int RealAlphaBeta(Position position, int depth, int alpha = int.MinValue, int beta = int.MaxValue)
        {
            static bool IsGameFinished(Position position) => throw new();

            if (depth == 0 || IsGameFinished(position))
            {
                return position.StaticEvaluation();
            }

            var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position);
            bool legalMovesFound = false;

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
                    legalMovesFound = true;

                    var eval = RealAlphaBeta(newPosition, depth - 1, alpha, beta);
                    maxEval = Max(maxEval, eval);   // Branch prediction optimized - should have started with most likely positions
                    alpha = Max(alpha, eval);       // maxTODO optimize branch prediction -> Should alpha be generally greater than eval?

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return legalMovesFound
                    ? maxEval
                    : position.EvaluateFinalPosition(depth);
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
                    legalMovesFound = true;

                    var eval = RealAlphaBeta(newPosition, depth - 1, alpha, beta);
                    minEval = Min(minEval, eval);   // Branch prediction optimized - should have started with most likely positions
                    beta = Min(beta, eval);        // TODO optimize branch prediction -> Should beta be generally less than eval?

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return legalMovesFound
                    ? minEval
                    : position.EvaluateFinalPosition(depth);
            }
        }
    }
}
