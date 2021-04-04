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

                //Logger.WriteLine($" {(BoardSquares)square}");
                //kingAttacks[square].Print();
            }

            return kingAttacks;
        }

        public static BitBoard[] InitializeBishopOccupancy()
        {
            BitBoard[] bishopAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                bishopAttacks[square] = MaskBishopOccupancy(square);

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
                rookAttacks[square] = MaskRookOccupancy(square);

                //Logger.WriteLine($" {(BoardSquares)square}");
                //rookAttacks[square].Print();
            }

            return rookAttacks;
        }

        /// <summary>
        /// Returns bishop occupancy masks and attacks
        /// </summary>
        /// <returns>(BitBoard[64], BitBoard[64, 512])</returns>
        public static (BitBoard[] BishopOccupancyMasks, BitBoard[,] BishopAttacks) InitializeBishopAttacks()
        {
            BitBoard[] occupancyMasks = new BitBoard[64];
            BitBoard[,] attacks = new BitBoard[64, 512];

            for (int square = 0; square < 64; ++square)
            {
                occupancyMasks[square] = MaskBishopOccupancy(square);

                var relevantBitsCount = Constants.BishopRelevantOccupancyBits[square];

                int occupancyIndexes = (1 << relevantBitsCount);

                for (int index = 0; index < occupancyIndexes; ++index)
                {
                    var occupancy = SetBishopOrRookOccupancy(index, occupancyMasks[square]);

                    var magicIndex = (occupancy.Board * Constants.BishopMagicNumbers[square]) >> (64 - relevantBitsCount);

                    attacks[square, magicIndex] = GenerateBishopAttacksOnTheFly(square, occupancy);
                }
            }

            return (occupancyMasks, attacks);
        }

        /// <summary>
        /// Returns rook occupancy masks and attacks
        /// </summary>
        /// <returns>(BitBoard[64], BitBoard[64, 512])</returns>
        public static (BitBoard[] RookOccupancyMasks, BitBoard[,] RookAttacks) InitializeRookAttacks()
        {
            BitBoard[] occupancyMasks = new BitBoard[64];
            BitBoard[,] attacks = new BitBoard[64, 4096];

            for (int square = 0; square < 64; ++square)
            {
                occupancyMasks[square] = MaskRookOccupancy(square);

                var relevantBitsCount = Constants.RookRelevantOccupancyBits[square];

                int occupancyIndexes = (1 << relevantBitsCount);

                for (int index = 0; index < occupancyIndexes; ++index)
                {
                    var occupancy = SetBishopOrRookOccupancy(index, occupancyMasks[square]);

                    var magicIndex = (occupancy.Board * Constants.RookMagicNumbers[square]) >> (64 - relevantBitsCount);

                    attacks[square, magicIndex] = GenerateRookAttacksOnTheFly(square, occupancy);
                }
            }

            return (occupancyMasks, attacks);
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
        public static BitBoard MaskBishopOccupancy(int squareIndex)
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
        public static BitBoard MaskRookOccupancy(int squareIndex)
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

        /// <summary>
        /// Populate occupancy sets from Bishop or Rook attack masks depending on <paramref name="index"/>
        /// </summary>
        /// <param name="index">
        /// Index within the range of possible occupancies within the bitboard.
        /// Between 0 and <paramref name="occupancyMask"/>.CountBits() - 1
        /// </param>
        /// <param name="occupancyMask">Bishop or rook occupancy (<see cref="AttacksGenerator.MaskBishopOccupancy(int)"/> and <see cref="AttacksGenerator.MaskRookOccupancy(int)"/>)</param>
        /// <returns>An occupancy set for the given index</returns>
        public static BitBoard SetBishopOrRookOccupancy(int index, BitBoard occupancyMask)
        {
            var bitsInMask = occupancyMask.CountBits();
            var occupancy = new BitBoard();

            // Loop over the range of bits within attack mask
            for (int count = 0; count < bitsInMask; ++count)
            {
                // Extract LS1B and reset it
                int squareIndex = occupancyMask.GetLS1BIndex();
                occupancyMask.PopBit(squareIndex);

                // Make sure occupancy is on board
                if ((index & (1 << count)) != default)
                {
                    // Update occupancy
                    occupancy.SetBit(squareIndex);
                }
            }

            return occupancy;
        }

        public static BitBoard GenerateBishopAttacksOnTheFly(int squareIndex, BitBoard occupiedSquares)
        {
            // Results attack bitboard
            BitBoard attacks = new(0UL);

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = squareIndex / 8;
            int targetFile = squareIndex % 8;

            // Generate bishop attacks

            /*
             * 0 0 0 0 0
             * 0 1 0 0 0
             * 0 0 X 0 0        ↘️
             * 0 0 0 X 0
             * 0 0 0 0 0
             */
            for (rank = targetRank + 1, file = targetFile + 1; rank <= 7 && file <= 7; ++rank, ++file)
            {
                ulong square = 1UL << BitBoard.SquareIndex(rank, file);
                attacks.Board |= square;

                if ((square & occupiedSquares.Board) != default)
                {
                    break;
                }
            }

            /*
             * 0 0 0 0 0
             * 0 X 0 0 0
             * 0 0 X 0 0        ↖️
             * 0 0 0 1 0
             * 0 0 0 0 0
             */
            for (rank = targetRank - 1, file = targetFile - 1; rank >= 0 && file >= 0; --rank, --file)
            {
                ulong square = 1UL << BitBoard.SquareIndex(rank, file);
                attacks.Board |= square;

                if ((square & occupiedSquares.Board) != default)
                {
                    break;
                }
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 X 0
             * 0 0 X 0 0        ↗️
             * 0 1 0 0 0
             * 0 0 0 0 0
             */
            for (rank = targetRank - 1, file = targetFile + 1; rank >= 0 && file <= 7; --rank, ++file)
            {
                ulong square = 1UL << BitBoard.SquareIndex(rank, file);
                attacks.Board |= square;

                if ((square & occupiedSquares.Board) != default)
                {
                    break;
                }
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 1 0
             * 0 0 X 0 0        ↙️
             * 0 X 0 0 0
             * 0 0 0 0 0
             */
            for (rank = targetRank + 1, file = targetFile - 1; rank <= 7 && file >= 0; ++rank, --file)
            {
                ulong square = 1UL << BitBoard.SquareIndex(rank, file);
                attacks.Board |= square;

                if ((square & occupiedSquares.Board) != default)
                {
                    break;
                }
            }

            return attacks;
        }

        public static BitBoard GenerateRookAttacksOnTheFly(int squareIndex, BitBoard occupiedSquares)
        {
            // Results attack bitboard
            BitBoard attacks = new(0UL);

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = squareIndex / 8;
            int targetFile = squareIndex % 8;

            // Generate rook attacks

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 1 X X X 0      →
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            for (file = targetFile + 1; file <= 7; ++file)
            {
                ulong square = 1UL << BitBoard.SquareIndex(targetRank, file);
                attacks.Board |= square;

                if ((square & occupiedSquares.Board) != default)
                {
                    break;
                }
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 X X X 1        ←
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            for (file = targetFile - 1; file >= 0; --file)
            {
                ulong square = 1UL << BitBoard.SquareIndex(targetRank, file);
                attacks.Board |= square;

                if ((square & occupiedSquares.Board) != default)
                {
                    break;
                }
            }

            /*
             * 0 0 1 0 0
             * 0 0 X 0 0
             * 0 0 X 0 0        ↓
             * 0 0 X 0 0
             * 0 0 0 0 0
             */
            for (rank = targetRank + 1; rank <= 7; ++rank)
            {
                ulong square = 1UL << BitBoard.SquareIndex(rank, targetFile);
                attacks.Board |= square;

                if ((square & occupiedSquares.Board) != default)
                {
                    break;
                }
            }

            /*
             * 0 0 0 0 0
             * 0 0 X 0 0
             * 0 0 X 0 0        ↑
             * 0 0 X 0 0
             * 0 0 1 0 0
             */
            for (rank = targetRank - 1; rank >= 0; --rank)
            {
                ulong square = 1UL << BitBoard.SquareIndex(rank, targetFile);
                attacks.Board |= square;

                if ((square & occupiedSquares.Board) != default)
                {
                    break;
                }
            }

            return attacks;
        }
    }
}
