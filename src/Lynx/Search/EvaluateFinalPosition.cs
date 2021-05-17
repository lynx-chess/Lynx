using Lynx.Model;

namespace Lynx.Search
{
    public static partial class SearchAlgorithms
    {
        public const int CheckMateEvaluation = 1_000_000_000;

        /// <summary>
        /// Assuming a current position has no moves, evaluates the final position to determine if it was a loss or a draw
        /// </summary>
        /// <param name="position"></param>
        /// <returns>+-<see cref="CheckMateEvaluation"/> +- <paramref name="depth"/> if Position.Side lost, or 0 if Position.Side was stalemated</returns>
        public static int EvaluateFinalPosition(Position position, int depth = 0)
        {
            if (Attacks.IsSquaredAttackedBySide(
                position.PieceBitBoards[(int)Piece.K + Utils.PieceOffset(position.Side)].GetLS1BIndex(),
                position,
                (Side)Utils.OppositeSide(position.Side)))
            {
                return position.Side == Side.White
                    ? -CheckMateEvaluation + depth
                    : CheckMateEvaluation - depth;
            }
            else
            {
                return 0;
            }
        }
    }
}
