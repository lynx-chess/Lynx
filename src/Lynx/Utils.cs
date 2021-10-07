using Lynx.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lynx
{
    public static class Utils
    {
        /// <summary>
        /// Side.White -> 0
        /// Side.Black -> 6
        /// </summary>
        public static int PieceOffset(Side side) => PieceOffset((int)side);

        /// <summary>
        /// Side.White -> 0
        /// Side.Black -> 6
        /// </summary>
        public static int PieceOffset(int side)
        {
            GuardAgainstSideBoth(side);

            return 6 - (6 * side);
        }
        /// <summary>
        /// Side.Black -> Side.White
        /// Side.White -> Side.Black
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static int OppositeSide(Side side)
        {
            GuardAgainstSideBoth((int)side);

            return (int)side ^ 1;     // or  (int)Side.White - (int)side
        }

        public static int ShortCastleRookTargetSquare(Side side) => ShortCastleRookTargetSquare((int)side);
        public static int ShortCastleRookTargetSquare(int side)
        {
            GuardAgainstSideBoth(side);

            return Constants.BlackShortCastleRookSquare + (7 * 8 * side);
        }

        public static int LongCastleRookTargetSquare(Side side) => LongCastleRookTargetSquare((int)side);
        public static int LongCastleRookTargetSquare(int side)
        {
            GuardAgainstSideBoth(side);

            return Constants.BlackLongCastleRookSquare + (7 * 8 * side);
        }

        public static int ShortCastleRookSourceSquare(Side side) => ShortCastleRookSourceSquare((int)side);
        public static int ShortCastleRookSourceSquare(int side)
        {
            GuardAgainstSideBoth(side);

            return (int)BoardSquare.h8 + (7 * 8 * side);
        }

        public static int LongCastleRookSourceSquare(Side side) => LongCastleRookSourceSquare((int)side);
        public static int LongCastleRookSourceSquare(int side)
        {
            GuardAgainstSideBoth(side);

            return (int)BoardSquare.a8 + (7 * 8 * side);
        }

        public static int UpdatePositionHistory(string newPositionFEN, Dictionary<string, int> positionHistory)
        {
            positionHistory.TryGetValue(newPositionFEN, out int repetitions);

            return positionHistory[newPositionFEN] = ++repetitions;
        }

        public static void RevertPositionHistory(string newPositionFEN, Dictionary<string, int> positionHistory, int repetitions)
        {
            if (repetitions == 1)
            {
                positionHistory.Remove(newPositionFEN);
            }
            else
            {
                --positionHistory[newPositionFEN];
            }
        }

        // Chaking movesWithoutCaptureOrPawnMove >= 50 since a caoture/pawn move don't necessarily 'clear' the variable.
        // i.e. at depth 2 0.0, 50 rules move apply
        // If the engine searches at depth 4, 50 rules must apply even if at depth 3 a capture happened
        public static int Update50movesRule(Move moveToPlay, int movesWithoutCaptureOrPawnMove)
        {
            if (moveToPlay.IsCapture())
            {
                return movesWithoutCaptureOrPawnMove >= 50 ? movesWithoutCaptureOrPawnMove : 0;
            }
            else
            {
                var pieceToMove = moveToPlay.Piece();

                return ((pieceToMove == (int)Piece.P || pieceToMove == (int)Piece.p)) && movesWithoutCaptureOrPawnMove < 50
                    ? 0
                    : movesWithoutCaptureOrPawnMove + 1;
            }
        }

        [Conditional("DEBUG")]
        private static void GuardAgainstSideBoth(int side)
        {
            if (side == (int)Side.Both)
            {
                throw new ArgumentException($"{Side.Both} wasn't expected");
            }
        }
    }
}
