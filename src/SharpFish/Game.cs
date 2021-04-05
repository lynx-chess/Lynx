using SharpFish.Model;

namespace SharpFish
{
    public class Game
    {
        private readonly BitBoard[] _bishopOccupancyMasks;
        private readonly BitBoard[] _rookOccupancyMasks;

        /// <summary>
        /// [2 (W/B), 64 (Squares)]
        /// </summary>
        private readonly BitBoard[,] _pawnAttacks;
        private readonly BitBoard[] _knightAttacks;
        private readonly BitBoard[] _kingAttacks;

        /// <summary>
        /// [64 (Squares), 512 (Occupancies)]
        /// Use <see cref="GetBishopAttacks(int, BitBoard)"/>
        /// </summary>
        private readonly BitBoard[,] _bishopAttacks;

        /// <summary>
        /// [64 (Squares), 4096 (Occupancies)]
        /// Use <see cref="GetRookAttacks(int, BitBoard)"/>
        /// </summary>
        private readonly BitBoard[,] _rookAttacks;

        private readonly BitBoard[] _pieceBitBoards = new BitBoard[12];

        /// <summary>
        /// Black, White, Both
        /// </summary>
        private readonly BitBoard[] _occupancyBitBoards = new BitBoard[3];

        private Side side = Side.Both;

        private BoardSquares _enpassant = BoardSquares.noSquare;

        private int _castle;

        public Game()
        {
            _kingAttacks = AttacksGenerator.InitializeKingAttacks();
            _pawnAttacks = AttacksGenerator.InitializePawnAttacks();
            _knightAttacks = AttacksGenerator.InitializeKnightAttacks();

            (_bishopOccupancyMasks, _bishopAttacks) = AttacksGenerator.InitializeBishopAttacks();
            (_rookOccupancyMasks, _rookAttacks) = AttacksGenerator.InitializeRookAttacks();
        }

        public BitBoard[] PieceBitBoards => _pieceBitBoards;

        /// <summary>
        /// Get Bishop attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy"></param>
        /// <returns></returns>
        public BitBoard GetBishopAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy.Board & _bishopOccupancyMasks[squareIndex].Board;
            occ *= Constants.BishopMagicNumbers[squareIndex];
            occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

            return _bishopAttacks[squareIndex, occ];
        }

        /// <summary>
        /// Get Rook attacks assuming current board occupancy
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <param name="occupancy"></param>
        /// <returns></returns>
        public BitBoard GetRookAttacks(int squareIndex, BitBoard occupancy)
        {
            var occ = occupancy.Board & _rookOccupancyMasks[squareIndex].Board;
            occ *= Constants.RookMagicNumbers[squareIndex];
            occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

            return _rookAttacks[squareIndex, occ];
        }
    }
}
