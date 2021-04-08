using SharpFish.Model;

namespace SharpFish
{
    public static class Utils
    {
        /// <summary>
        /// Side.White -> 0
        /// Side.Black -> 6
        /// </summary>
        public static int PieceOffset(Side side) => 6 - (6 * (int)side);

        /// <summary>
        /// Side.Black -> Side.White
        /// Side.White -> Side.Black
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static int OppositeSide(Side side) => (int)Side.White - (int)side;
    }
}
