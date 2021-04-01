using SharpFish.Model;

namespace SharpFish
{
    public static class AttacksGenerator
    {
        /// <summary>
        /// BitBoard[isWhite, square]
        /// </summary>
        public static BitBoard[,] InitializePawnAttacks()
        {
            BitBoard[,] pawnAttacks = new BitBoard[2, 64];

            for (int square = 0; square < 64; ++square)
            {
                pawnAttacks[0, square] = MaskPawnAttacks(square, isWhite: false);
                pawnAttacks[1, square] = MaskPawnAttacks(square, isWhite: true);

                //Logger.WriteLine($" {(BoardSquares)square} (White)");
                //pawnAttacks[1, square].Print();

                //Logger.WriteLine($" {(BoardSquares)square} (Black)");
                //pawnAttacks[0, square].Print();
            }

            return pawnAttacks;
        }

        public static BitBoard[] InitializeKnightAttacks()
        {
            BitBoard[] knightAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                knightAttacks[square] = MaskKnightAttacks(square);

                //Logger.WriteLine($" {(BoardSquares)square}");
                //knightAttacks[square].Print();
            }

            return knightAttacks;
        }

        public static BitBoard[] InitializeKingAttacks()
        {
            BitBoard[] kingAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                kingAttacks[square] = MaskKingAttacks(square);

                Logger.WriteLine($" {(BoardSquares)square}");
                kingAttacks[square].Print();
            }

            return kingAttacks;
        }

        public static BitBoard[] InitializeBishopOccupancy()
        {
            BitBoard[] bishopAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                bishopAttacks[square] = MaskBishopAttacks(square);

                //Logger.WriteLine($" {(BoardSquares)square}");
                //bishopAttacks[square].Print();
            }

            return bishopAttacks;
        }

        public static BitBoard[] InitializeRookOccupancy()
        {
            BitBoard[] rookAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                rookAttacks[square] = MaskRookAttacks(square);

                //Logger.WriteLine($" {(BoardSquares)square}");
                //rookAttacks[square].Print();
            }

            return rookAttacks;
        }

        public static BitBoard MaskPawnAttacks(int squareIndex, bool isWhite)
        {
            // Results attack bitboard
            BitBoard attacks = new(0UL);

            // Piece bitboard
            BitBoard bitBoard = new(0UL);

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            if (isWhite)
            {
                /*
                 * 0 0 0 X 0
                 * 0 0 1 0 0
                 * 0 0 0 0 0
                 */
                var right = bitBoard.Board >> 7;
                if ((right & Constants.NotAFile) != default)
                {
                    attacks.Board |= right;
                }

                /*
                 * 0 X 0 0 0
                 * 0 0 1 0 0
                 * 0 0 0 0 0
                 */
                var left = bitBoard.Board >> 9;
                if ((left & Constants.NotHFile) != default)
                {
                    attacks.Board |= left;
                }
            }
            else
            {
                /*
                 * 0 0 0 0 0
                 * 0 0 1 0 0
                 * 0 X 0 0 0
                 */
                var left = bitBoard.Board << 7;
                if ((left & Constants.NotHFile) != default)
                {
                    attacks.Board |= left;
                }

                /*
                 * 0 0 0 0 0
                 * 0 0 1 0 0
                 * 0 0 0 X 0
                 */
                var right = bitBoard.Board << 9;
                if ((right & Constants.NotAFile) != default)
                {
                    attacks.Board |= right;
                }
            }

            return attacks;
        }

        public static BitBoard MaskKnightAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = new(0UL);

            // Piece bitboard
            BitBoard bitBoard = new(0UL);

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            /*
             * 0 X 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            var attack = bitBoard.Board >> 17;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0 X 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard.Board >> 15;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 X 0 0 0
             */
            attack = bitBoard.Board << 15;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 X 0
             */
            attack = bitBoard.Board << 17;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0 0 0
             * X 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard.Board >> 10;
            if ((attack & Constants.NotHGFiles) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 X
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard.Board >> 6;
            if ((attack & Constants.NotABFiles) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * X 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard.Board << 6;
            if ((attack & Constants.NotHGFiles) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 X
             * 0 0 0 0 0
             */
            attack = bitBoard.Board << 10;
            if ((attack & Constants.NotABFiles) != default)
            {
                attacks.Board |= attack;
            }

            return attacks;
        }

        public static BitBoard MaskKingAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = new(0UL);

            // Piece bitboard
            BitBoard bitBoard = new(0UL);

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            /*
             * X 0 0
             * 0 1 0
             * 0 0 0
             */
            var attack = bitBoard.Board >> 9;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 X 0
             * 0 1 0
             * 0 0 0
             */
            attacks.Board |= bitBoard.Board >> 8;

            /*
             * 0 0 X
             * 0 1 0
             * 0 0 0
             */
            attack = bitBoard.Board >> 7;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0
             * X 1 0
             * 0 0 0
             */
            attack = bitBoard.Board >> 1;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0
             * 0 1 X
             * 0 0 0
             */
            attack = bitBoard.Board << 1;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0
             * 0 1 0
             * X 0 0
             */
            attack = bitBoard.Board << 7;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks.Board |= attack;
            }

            /*
             * 0 0 0
             * 0 1 0
             * 0 X 0
             */
            attacks.Board |= bitBoard.Board << 8;

            /*
             * 0 0 0
             * 0 1 0
             * X 0 0
             */
            attack = bitBoard.Board << 9;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks.Board |= attack;
            }

            return attacks;
        }

        /// <summary>
        /// Returns relevant 'bishop occupancy squares' (attacks)
        /// Outer squares don't matter in terms of occupancy (see https://www.chessprogramming.org/First_Rank_Attacks#TheOuterSquares)
        /// Therefore, there are max 6 occupancy squares per direction (if a bishop is placed on a corner)
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <returns></returns>
        public static BitBoard MaskBishopAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = new(0UL);

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = squareIndex / 8;
            int targetFile = squareIndex % 8;

            // Mask relevant bishop occupancy bits (squares)

            /*
             * 0 0 0 0 0
             * 0 1 0 0 0
             * 0 0 X 0 0        ↘️
             * 0 0 0 X 0
             * 0 0 0 0 0
             */
            for (rank = targetRank + 1, file = targetFile + 1; rank <= 6 && file <= 6; ++rank, ++file)
            {
                attacks.Board |= 1UL << BitBoard.SquareIndex(rank, file);
            }

            /*
             * 0 0 0 0 0
             * 0 X 0 0 0
             * 0 0 X 0 0        ↖️
             * 0 0 0 1 0
             * 0 0 0 0 0
             */
            for (rank = targetRank - 1, file = targetFile - 1; rank >= 1 && file >= 1; --rank, --file)
            {
                attacks.Board |= 1UL << BitBoard.SquareIndex(rank, file);
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 X 0
             * 0 0 X 0 0        ↗️
             * 0 1 0 0 0
             * 0 0 0 0 0
             */
            for (rank = targetRank - 1, file = targetFile + 1; rank >= 1 && file <= 6; --rank, ++file)
            {
                attacks.Board |= 1UL << BitBoard.SquareIndex(rank, file);
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 1 0
             * 0 0 X 0 0        ↙️
             * 0 X 0 0 0
             * 0 0 0 0 0
             */
            for (rank = targetRank + 1, file = targetFile - 1; rank <= 6 && file >= 1; ++rank, --file)
            {
                attacks.Board |= 1UL << BitBoard.SquareIndex(rank, file);
            }

            return attacks;
        }

        /// <summary>
        /// Returns relevant 'rook occupancy squares' (attacks)
        /// Outer squares don't matter in terms of occupancy (see https://www.chessprogramming.org/First_Rank_Attacks#TheOuterSquares)
        /// Therefore, there are max 6 occupancy squares per direction (if a rook is placed on a corner)
        /// </summary>
        /// <param name="squareIndex"></param>
        /// <returns></returns>
        public static BitBoard MaskRookAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = new(0UL);

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = squareIndex / 8;
            int targetFile = squareIndex % 8;

            // Mask relevant rook occupancy bits (squares)

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 1 X X X 0      →
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            for (file = targetFile + 1; file <= 6; ++file)
            {
                attacks.Board |= 1UL << BitBoard.SquareIndex(targetRank, file);
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 X X X 1        ←
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            for (file = targetFile - 1; file >= 1; --file)
            {
                attacks.Board |= 1UL << BitBoard.SquareIndex(targetRank, file);
            }

            /*
             * 0 0 1 0 0
             * 0 0 X 0 0
             * 0 0 X 0 0        ↓
             * 0 0 X 0 0
             * 0 0 0 0 0
             */
            for (rank = targetRank + 1; rank <= 6; ++rank)
            {
                attacks.Board |= 1UL << BitBoard.SquareIndex(rank, targetFile);
            }

            /*
             * 0 0 0 0 0
             * 0 0 X 0 0
             * 0 0 X 0 0        ↑
             * 0 0 X 0 0
             * 0 0 1 0 0
             */
            for (rank = targetRank - 1; rank >= 1; --rank)
            {
                attacks.Board |= 1UL << BitBoard.SquareIndex(rank, targetFile);
            }

            return attacks;
        }
    }
}
