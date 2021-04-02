namespace SharpFish.Model
{
    public struct BitBoard
    {
        public ulong Board { get; set; }

        public BitBoard(ulong value) { Board = value; }

        public BitBoard(BoardSquares[] occupiedSquares)
        {
            Board = default;

            foreach (var square in occupiedSquares)
            {
                SetBit(square);
            }
        }

        public static int SquareIndex(int rank, int file) => (rank * 8) + file;

        public void Print()
        {
            const string separator = "____________________________________________________";
            Logger.WriteLine(separator);

            for (var rank = 0; rank < 8; ++rank)
            {
                for (var file = 0; file < 8; ++file)
                {
                    if (file == 0)
                    {
                        Logger.Write($"{8 - rank}  ");
                    }

                    var squareIndex = SquareIndex(rank, file);

                    //Logger.Write($"|{squareIndex,-2}");
                    Logger.Write($" {(GetBit(squareIndex) ? "1" : "0")}");
                }

                Logger.WriteLine();
            }

            Logger.Write("\n    a b c d e f g h\n");

            Logger.WriteLine($"\n    Bitboard: {Board} (0x{Board:X})");
            Logger.WriteLine(separator);
        }

        public bool GetBit(int squareIndex)
        {
            return (Board & (1UL << squareIndex)) != default;
        }

        public void SetBit(int square)
        {
            Board |= (1UL << square);
        }

        public void PopBit(int square)
        {
            if (GetBit(square))
            {
                Board ^= (1UL << square);
            }
        }

        public bool Empty() => Board == default;

        /// <summary>
        /// https://www.chessprogramming.org/Population_Count#Single_Populated_Bitboards
        /// </summary>
        public bool IsSinglePopulated()
        {
            return Board != default && (Board & (Board - 1)) == default;
        }

        public bool GetBit(BoardSquares square) => GetBit((int)square);

        public void SetBit(BoardSquares square) => SetBit((int)square);

        public void PopBit(BoardSquares square) => PopBit((int)square);
    }
}
