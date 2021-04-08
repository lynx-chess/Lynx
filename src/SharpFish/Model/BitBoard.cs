namespace SharpFish.Model
{
    public struct BitBoard
    {
        public ulong Board { get; set; }

        public bool Empty => Board == default;

        public BitBoard(ulong value) { Board = value; }

        public BitBoard(params BoardSquares[] occupiedSquares)
        {
            Board = default;

            foreach (var square in occupiedSquares)
            {
                SetBit(square);
            }
        }

        public void Clear() { Board = default; }

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
            Board &= ~(1UL << square);
        }

        /// <summary>
        /// https://www.chessprogramming.org/General_Setwise_Operations#Separation
        /// Cannot use (Board & -Board) - 1 due to limitation applying unary - to ulong
        /// </summary>
        /// <returns>-1 in case of empty board</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLS1BIndex() => GetLS1BIndex(Board);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResetLS1B()
        {
            Board = ResetLS1B(Board);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CountBits() => CountBits(Board);

        /// <summary>
        /// https://www.chessprogramming.org/Population_Count#Single_Populated_Bitboards
        /// </summary>
        public bool IsSinglePopulated()
        {
            return Board != default && ResetLS1B(Board) == default;
        }

        #region Static methods

        public static int SquareIndex(int rank, int file) => (rank * 8) + file;

        public static int GetLS1BIndex(ulong bitboard)
        {
            if (bitboard == default)
            {
                return -1;
            }

            return CountBits(bitboard ^ (bitboard - 1)) - 1;
        }

        /// <summary>
        /// https://www.chessprogramming.org/General_Setwise_Operations#LS1BReset
        /// </summary>
        /// <returns>Bitboard</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ResetLS1B(ulong bitboard)
        {
            return bitboard & (bitboard - 1);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountBits(ulong bitboard)
        {
            int counter = 0;

            // Consecutively reset LSB
            while (bitboard != default)
            {
                ++counter;
                bitboard = ResetLS1B(bitboard);
            }

            return counter;
        }

        #endregion

        #region Methods accepting BoardSquares

        public bool GetBit(BoardSquares square) => GetBit((int)square);

        public void SetBit(BoardSquares square) => SetBit((int)square);

        public void PopBit(BoardSquares square) => PopBit((int)square);

        #endregion
    }
}
