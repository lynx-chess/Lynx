using SharpFish.Model;

namespace SharpFish
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
        public static int PieceOffset(int side) => 6 - (6 * side);

        /// <summary>
        /// Side.Black -> Side.White
        /// Side.White -> Side.Black
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static int OppositeSide(Side side) => (int)Side.White - (int)side;

        public static int ShortCastleRookTargetSquare(Side side) => ShortCastleRookTargetSquare((int)side);
        public static int ShortCastleRookTargetSquare(int side) => Constants.BlackShortCastleRookSquare + (7 * 8 * side);

        public static int LongCastleRookTargetSquare(Side side) => LongCastleRookTargetSquare((int)side);
        public static int LongCastleRookTargetSquare(int side) => Constants.BlackLongCastleRookSquare + (7 * 8 * side);

        public static int ShortCastleRookSourceSquare(Side side) => ShortCastleRookSourceSquare((int)side);
        public static int ShortCastleRookSourceSquare(int side) => (int)BoardSquares.h8 + (7 * 8 * side);

        public static int LongCastleRookSourceSquare(Side side) => LongCastleRookSourceSquare((int)side);
        public static int LongCastleRookSourceSquare(int side) => (int)BoardSquares.a8 + (7 * 8 * side);
    }
}
