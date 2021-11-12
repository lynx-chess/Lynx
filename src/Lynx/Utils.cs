﻿using Lynx.Model;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Lynx
{
    public static class Utils
    {
        /// <summary>
        /// Side.White -> 0
        /// Side.Black -> 6
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PieceOffset(Side side) => PieceOffset((int)side);

        /// <summary>
        /// Side.White -> 0
        /// Side.Black -> 6
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int OppositeSide(Side side)
        {
            GuardAgainstSideBoth((int)side);

            return (int)side ^ 1;     // or  (int)Side.White - (int)side
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InCheck(Position position)
        {
            var kingSquare = position.PieceBitBoards[(int)Piece.K + PieceOffset(position.Side)].GetLS1BIndex();

            return Attacks.IsSquaredAttacked(kingSquare, position.Side, position.PieceBitBoards, position.OccupancyBitBoards);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ShortCastleRookTargetSquare(Side side) => ShortCastleRookTargetSquare((int)side);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ShortCastleRookTargetSquare(int side)
        {
            GuardAgainstSideBoth(side);

            return Constants.BlackShortCastleRookSquare + (7 * 8 * side);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LongCastleRookTargetSquare(Side side) => LongCastleRookTargetSquare((int)side);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LongCastleRookTargetSquare(int side)
        {
            GuardAgainstSideBoth(side);

            return Constants.BlackLongCastleRookSquare + (7 * 8 * side);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ShortCastleRookSourceSquare(Side side) => ShortCastleRookSourceSquare((int)side);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ShortCastleRookSourceSquare(int side)
        {
            GuardAgainstSideBoth(side);

            return (int)BoardSquare.h8 + (7 * 8 * side);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LongCastleRookSourceSquare(Side side) => LongCastleRookSourceSquare((int)side);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LongCastleRookSourceSquare(int side)
        {
            GuardAgainstSideBoth(side);

            return (int)BoardSquare.a8 + (7 * 8 * side);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UpdatePositionHistory(Position newPosition, Dictionary<long, int> positionHistory)
        {
            var id = newPosition.UniqueIdentifier;

            return positionHistory.TryAdd(id, 1)
                ? 1
                : ++positionHistory[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RevertPositionHistory(Position newPosition, Dictionary<long, int> positionHistory, int repetitions)
        {
            var id = newPosition.UniqueIdentifier;

            if (repetitions == 1)
            {
                positionHistory.Remove(id);
            }
            else
            {
                --positionHistory[id];
            }
        }

        /// <summary>
        /// Updates <paramref name="movesWithoutCaptureOrPawnMove"/>
        /// </summary>
        /// <param name="moveToPlay"></param>
        /// <param name="movesWithoutCaptureOrPawnMove"></param>
        /// <remarks>
        /// Checking movesWithoutCaptureOrPawnMove >= 50 since a capture/pawn move doesn't necessarily 'clear' the variable.
        /// i.e. while the engine is searching:
        ///     At depth 2, 50 rules move applied and eval is 0
        ///     At depth 3, there's a capture, but the eval should still be 0
        ///     At depth 4 there's no capture, but the eval should still be 0
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Update50movesRule(Move moveToPlay, int movesWithoutCaptureOrPawnMove)
        {
            if (moveToPlay.IsCapture())
            {
                return movesWithoutCaptureOrPawnMove >= 50
                    ? movesWithoutCaptureOrPawnMove
                    : 0;
            }
            else
            {
                var pieceToMove = moveToPlay.Piece();

                return (pieceToMove == (int)Piece.P || pieceToMove == (int)Piece.p) && movesWithoutCaptureOrPawnMove < 50
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

        [Conditional("DEBUG")]
        public static void Assert(bool value, string errorMessage = "Assertion failed")
        {
            if (!value)
            {
                throw new AssertException(errorMessage);
            }
        }
    }
}
