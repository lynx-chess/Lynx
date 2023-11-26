/*
 *  BenchmarkDotNet v0.13.10, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                         | Mean     | Error     | StdDev    | Ratio | Gen0     | Gen1     | Gen2     | Allocated | Alloc Ratio |
 *  |------------------------------- |---------:|----------:|----------:|------:|---------:|---------:|---------:|----------:|------------:|
 *  | AttackGenerator_Original       | 5.575 ms | 0.0278 ms | 0.0260 ms |  1.00 | 484.3750 | 484.3750 | 484.3750 |   2.25 MB |        1.00 |
 *  | AttackGenerator_SkipLocalsInit | 5.574 ms | 0.0250 ms | 0.0222 ms |  1.00 | 468.7500 | 468.7500 | 468.7500 |   2.25 MB |        1.00 |
 *  | AttackGenerator_Other          | 5.573 ms | 0.0333 ms | 0.0312 ms |  1.00 | 468.7500 | 468.7500 | 468.7500 |   2.25 MB |        1.00 |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class SkipLocalsInit : BaseBenchmark
{
    [Benchmark(Baseline = true)]
    public ulong AttackGenerator_Original()
    {
        var KingAttacks = SkipLocalsInit_AttackGenerator_Original.InitializeKingAttacks();
        var PawnAttacks = SkipLocalsInit_AttackGenerator_Original.InitializePawnAttacks();
        var KnightAttacks = SkipLocalsInit_AttackGenerator_Original.InitializeKnightAttacks();

        var bishopOcuppancy = SkipLocalsInit_AttackGenerator_Original.InitializeBishopOccupancy();
        var rookOcuppancy = SkipLocalsInit_AttackGenerator_Original.InitializeRookOccupancy();

        (var _bishopOccupancyMasks, var _bishopAttacks) = SkipLocalsInit_AttackGenerator_Original.InitializeBishopMagicAttacks();
        (var _rookOccupancyMasks, var _rookAttacks) = SkipLocalsInit_AttackGenerator_Original.InitializeRookMagicAttacks();

        return KingAttacks[0] ^ PawnAttacks[0, 0] ^ KnightAttacks[0] ^ _bishopOccupancyMasks[0]
            ^ _rookOccupancyMasks[0] ^ _bishopAttacks[0, 0] ^ _rookAttacks[0, 0]
            ^ bishopOcuppancy[0] ^ rookOcuppancy[0];
    }

    [Benchmark]
    public ulong AttackGenerator_SkipLocalsInit()
    {
        var KingAttacks = SkipLocalsInit_AttackGenerator_SkipLocalsInit.InitializeKingAttacks();
        var PawnAttacks = SkipLocalsInit_AttackGenerator_SkipLocalsInit.InitializePawnAttacks();
        var KnightAttacks = SkipLocalsInit_AttackGenerator_SkipLocalsInit.InitializeKnightAttacks();

        var bishopOcuppancy = SkipLocalsInit_AttackGenerator_SkipLocalsInit.InitializeBishopOccupancy();
        var rookOcuppancy = SkipLocalsInit_AttackGenerator_SkipLocalsInit.InitializeRookOccupancy();

        (var _bishopOccupancyMasks, var _bishopAttacks) = SkipLocalsInit_AttackGenerator_SkipLocalsInit.InitializeBishopMagicAttacks();
        (var _rookOccupancyMasks, var _rookAttacks) = SkipLocalsInit_AttackGenerator_SkipLocalsInit.InitializeRookMagicAttacks();

        return KingAttacks[0] ^ PawnAttacks[0, 0] ^ KnightAttacks[0] ^ _bishopOccupancyMasks[0]
            ^ _rookOccupancyMasks[0] ^ _bishopAttacks[0, 0] ^ _rookAttacks[0, 0]
            ^ bishopOcuppancy[0] ^ rookOcuppancy[0];
    }

    [Benchmark]
    public ulong AttackGenerator_Other()
    {
        var KingAttacks = SkipLocalsInit_AttackGenerator_Other.InitializeKingAttacks();
        var PawnAttacks = SkipLocalsInit_AttackGenerator_Other.InitializePawnAttacks();
        var KnightAttacks = SkipLocalsInit_AttackGenerator_Other.InitializeKnightAttacks();

        var bishopOcuppancy = SkipLocalsInit_AttackGenerator_Other.InitializeBishopOccupancy();
        var rookOcuppancy = SkipLocalsInit_AttackGenerator_Other.InitializeRookOccupancy();

        (var _bishopOccupancyMasks, var _bishopAttacks) = SkipLocalsInit_AttackGenerator_Other.InitializeBishopMagicAttacks();
        (var _rookOccupancyMasks, var _rookAttacks) = SkipLocalsInit_AttackGenerator_Other.InitializeRookMagicAttacks();

        return KingAttacks[0] ^ PawnAttacks[0, 0] ^ KnightAttacks[0] ^ _bishopOccupancyMasks[0]
            ^ _rookOccupancyMasks[0] ^ _bishopAttacks[0, 0] ^ _rookAttacks[0, 0]
            ^ bishopOcuppancy[0] ^ rookOcuppancy[0];
    }

    private static class SkipLocalsInit_AttackGenerator_Original
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
            }

            return pawnAttacks;
        }

        public static BitBoard[] InitializeKnightAttacks()
        {
            BitBoard[] knightAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                knightAttacks[square] = MaskKnightAttacks(square);
            }

            return knightAttacks;
        }

        public static BitBoard[] InitializeKingAttacks()
        {
            BitBoard[] kingAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                kingAttacks[square] = MaskKingAttacks(square);
            }

            return kingAttacks;
        }

        public static BitBoard[] InitializeBishopOccupancy()
        {
            BitBoard[] bishopAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                bishopAttacks[square] = MaskBishopOccupancy(square);
            }

            return bishopAttacks;
        }

        public static BitBoard[] InitializeRookOccupancy()
        {
            BitBoard[] rookAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                rookAttacks[square] = MaskRookOccupancy(square);
            }

            return rookAttacks;
        }

        /// <summary>
        /// Returns bishop occupancy masks and attacks
        /// </summary>
        /// <returns>(BitBoard[64], BitBoard[64, 512])</returns>
        public static (BitBoard[] BishopOccupancyMasks, BitBoard[,] BishopAttacks) InitializeBishopMagicAttacks()
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

                    var magicIndex = (occupancy * Constants.BishopMagicNumbers[square]) >> (64 - relevantBitsCount);

                    attacks[square, magicIndex] = GenerateBishopAttacksOnTheFly(square, occupancy);
                }
            }

            return (occupancyMasks, attacks);
        }

        /// <summary>
        /// Returns rook occupancy masks and attacks
        /// </summary>
        /// <returns>(BitBoard[64], BitBoard[64, 512])</returns>
        public static (BitBoard[] RookOccupancyMasks, BitBoard[,] RookAttacks) InitializeRookMagicAttacks()
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

                    var magicIndex = (occupancy * Constants.RookMagicNumbers[square]) >> (64 - relevantBitsCount);

                    attacks[square, magicIndex] = GenerateRookAttacksOnTheFly(square, occupancy);
                }
            }

            return (occupancyMasks, attacks);
        }

        public static BitBoard MaskPawnAttacks(int squareIndex, bool isWhite)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            // Piece bitboard
            BitBoard bitBoard = default;

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            if (isWhite)
            {
                /*
                 * 0 0 0 X 0
                 * 0 0 1 0 0
                 * 0 0 0 0 0
                 */
                var right = bitBoard >> 7;
                if ((right & Constants.NotAFile) != default)
                {
                    attacks |= right;
                }

                /*
                 * 0 X 0 0 0
                 * 0 0 1 0 0
                 * 0 0 0 0 0
                 */
                var left = bitBoard >> 9;
                if ((left & Constants.NotHFile) != default)
                {
                    attacks |= left;
                }
            }
            else
            {
                /*
                 * 0 0 0 0 0
                 * 0 0 1 0 0
                 * 0 X 0 0 0
                 */
                var left = bitBoard << 7;
                if ((left & Constants.NotHFile) != default)
                {
                    attacks |= left;
                }

                /*
                 * 0 0 0 0 0
                 * 0 0 1 0 0
                 * 0 0 0 X 0
                 */
                var right = bitBoard << 9;
                if ((right & Constants.NotAFile) != default)
                {
                    attacks |= right;
                }
            }

            return attacks;
        }

        public static BitBoard MaskKnightAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            // Piece bitboard
            BitBoard bitBoard = default;

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            /*
             * 0 X 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            var attack = bitBoard >> 17;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 X 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard >> 15;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 X 0 0 0
             */
            attack = bitBoard << 15;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 X 0
             */
            attack = bitBoard << 17;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * X 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard >> 10;
            if ((attack & Constants.NotHGFiles) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 X
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard >> 6;
            if ((attack & Constants.NotABFiles) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * X 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard << 6;
            if ((attack & Constants.NotHGFiles) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 X
             * 0 0 0 0 0
             */
            attack = bitBoard << 10;
            if ((attack & Constants.NotABFiles) != default)
            {
                attacks |= attack;
            }

            return attacks;
        }

        public static BitBoard MaskKingAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            // Piece bitboard
            BitBoard bitBoard = default;

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            /*
             * X 0 0
             * 0 1 0
             * 0 0 0
             */
            var attack = bitBoard >> 9;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 X 0
             * 0 1 0
             * 0 0 0
             */
            attacks |= bitBoard >> 8;

            /*
             * 0 0 X
             * 0 1 0
             * 0 0 0
             */
            attack = bitBoard >> 7;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * X 1 0
             * 0 0 0
             */
            attack = bitBoard >> 1;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * 0 1 X
             * 0 0 0
             */
            attack = bitBoard << 1;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * 0 1 0
             * X 0 0
             */
            attack = bitBoard << 7;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * 0 1 0
             * 0 X 0
             */
            attacks |= bitBoard << 8;

            /*
             * 0 0 0
             * 0 1 0
             * X 0 0
             */
            attack = bitBoard << 9;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
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
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
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
        /// <param name="occupancyMask">Bishop or rook occupancy (<see cref="AttackGenerator.MaskBishopOccupancy(int)"/> and <see cref="AttackGenerator.MaskRookOccupancy(int)"/>)</param>
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
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
                {
                    break;
                }
            }

            return attacks;
        }

        public static BitBoard GenerateRookAttacksOnTheFly(int squareIndex, BitBoard occupiedSquares)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
                attacks |= square;

                if ((square & occupiedSquares) != default)
                {
                    break;
                }
            }

            return attacks;
        }
    }

    private static class SkipLocalsInit_AttackGenerator_SkipLocalsInit
    {
        /// <summary>
        /// BitBoard[isWhite, square]
        /// </summary>
        [SkipLocalsInit]
        public static BitBoard[,] InitializePawnAttacks()
        {
            BitBoard[,] pawnAttacks = new BitBoard[2, 64];

            for (int square = 0; square < 64; ++square)
            {
                pawnAttacks[0, square] = MaskPawnAttacks(square, isWhite: false);
                pawnAttacks[1, square] = MaskPawnAttacks(square, isWhite: true);
            }

            return pawnAttacks;
        }

        [SkipLocalsInit]
        public static BitBoard[] InitializeKnightAttacks()
        {
            BitBoard[] knightAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                knightAttacks[square] = MaskKnightAttacks(square);
            }

            return knightAttacks;
        }

        [SkipLocalsInit]
        public static BitBoard[] InitializeKingAttacks()
        {
            BitBoard[] kingAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                kingAttacks[square] = MaskKingAttacks(square);
            }

            return kingAttacks;
        }

        [SkipLocalsInit]
        public static BitBoard[] InitializeBishopOccupancy()
        {
            BitBoard[] bishopAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                bishopAttacks[square] = MaskBishopOccupancy(square);
            }

            return bishopAttacks;
        }

        [SkipLocalsInit]
        public static BitBoard[] InitializeRookOccupancy()
        {
            BitBoard[] rookAttacks = new BitBoard[64];

            for (int square = 0; square < 64; ++square)
            {
                rookAttacks[square] = MaskRookOccupancy(square);
            }

            return rookAttacks;
        }

        /// <summary>
        /// Returns bishop occupancy masks and attacks
        /// </summary>
        /// <returns>(BitBoard[64], BitBoard[64, 512])</returns>
        [SkipLocalsInit]
        public static (BitBoard[] BishopOccupancyMasks, BitBoard[,] BishopAttacks) InitializeBishopMagicAttacks()
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

                    var magicIndex = (occupancy * Constants.BishopMagicNumbers[square]) >> (64 - relevantBitsCount);

                    attacks[square, magicIndex] = GenerateBishopAttacksOnTheFly(square, occupancy);
                }
            }

            return (occupancyMasks, attacks);
        }

        /// <summary>
        /// Returns rook occupancy masks and attacks
        /// </summary>
        /// <returns>(BitBoard[64], BitBoard[64, 512])</returns>
        [SkipLocalsInit]
        public static (BitBoard[] RookOccupancyMasks, BitBoard[,] RookAttacks) InitializeRookMagicAttacks()
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

                    var magicIndex = (occupancy * Constants.RookMagicNumbers[square]) >> (64 - relevantBitsCount);

                    attacks[square, magicIndex] = GenerateRookAttacksOnTheFly(square, occupancy);
                }
            }

            return (occupancyMasks, attacks);
        }

        public static BitBoard MaskPawnAttacks(int squareIndex, bool isWhite)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            // Piece bitboard
            BitBoard bitBoard = default;

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            if (isWhite)
            {
                /*
                 * 0 0 0 X 0
                 * 0 0 1 0 0
                 * 0 0 0 0 0
                 */
                var right = bitBoard >> 7;
                if ((right & Constants.NotAFile) != default)
                {
                    attacks |= right;
                }

                /*
                 * 0 X 0 0 0
                 * 0 0 1 0 0
                 * 0 0 0 0 0
                 */
                var left = bitBoard >> 9;
                if ((left & Constants.NotHFile) != default)
                {
                    attacks |= left;
                }
            }
            else
            {
                /*
                 * 0 0 0 0 0
                 * 0 0 1 0 0
                 * 0 X 0 0 0
                 */
                var left = bitBoard << 7;
                if ((left & Constants.NotHFile) != default)
                {
                    attacks |= left;
                }

                /*
                 * 0 0 0 0 0
                 * 0 0 1 0 0
                 * 0 0 0 X 0
                 */
                var right = bitBoard << 9;
                if ((right & Constants.NotAFile) != default)
                {
                    attacks |= right;
                }
            }

            return attacks;
        }

        public static BitBoard MaskKnightAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            // Piece bitboard
            BitBoard bitBoard = default;

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            /*
             * 0 X 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            var attack = bitBoard >> 17;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 X 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard >> 15;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 X 0 0 0
             */
            attack = bitBoard << 15;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 X 0
             */
            attack = bitBoard << 17;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * X 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard >> 10;
            if ((attack & Constants.NotHGFiles) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 X
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard >> 6;
            if ((attack & Constants.NotABFiles) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * X 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard << 6;
            if ((attack & Constants.NotHGFiles) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 X
             * 0 0 0 0 0
             */
            attack = bitBoard << 10;
            if ((attack & Constants.NotABFiles) != default)
            {
                attacks |= attack;
            }

            return attacks;
        }

        public static BitBoard MaskKingAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            // Piece bitboard
            BitBoard bitBoard = default;

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            /*
             * X 0 0
             * 0 1 0
             * 0 0 0
             */
            var attack = bitBoard >> 9;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 X 0
             * 0 1 0
             * 0 0 0
             */
            attacks |= bitBoard >> 8;

            /*
             * 0 0 X
             * 0 1 0
             * 0 0 0
             */
            attack = bitBoard >> 7;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * X 1 0
             * 0 0 0
             */
            attack = bitBoard >> 1;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * 0 1 X
             * 0 0 0
             */
            attack = bitBoard << 1;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * 0 1 0
             * X 0 0
             */
            attack = bitBoard << 7;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * 0 1 0
             * 0 X 0
             */
            attacks |= bitBoard << 8;

            /*
             * 0 0 0
             * 0 1 0
             * X 0 0
             */
            attack = bitBoard << 9;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
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
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
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
        /// <param name="occupancyMask">Bishop or rook occupancy (<see cref="AttackGenerator.MaskBishopOccupancy(int)"/> and <see cref="AttackGenerator.MaskRookOccupancy(int)"/>)</param>
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
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
                {
                    break;
                }
            }

            return attacks;
        }

        public static BitBoard GenerateRookAttacksOnTheFly(int squareIndex, BitBoard occupiedSquares)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
                attacks |= square;

                if ((square & occupiedSquares) != default)
                {
                    break;
                }
            }

            return attacks;
        }
    }

    private static class SkipLocalsInit_AttackGenerator_Other
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
            }

            return pawnAttacks;
        }

        public static BitBoard[] InitializeKnightAttacks()
        {
            return Enumerable.Range(0, 64)
                .Select(MaskKnightAttacks)
                .ToArray();
        }

        public static BitBoard[] InitializeKingAttacks()
        {
            return Enumerable.Range(0, 64)
                .Select(MaskKingAttacks)
                .ToArray();
        }

        public static BitBoard[] InitializeBishopOccupancy()
        {
            return Enumerable.Range(0, 64)
                .Select(MaskBishopOccupancy)
                .ToArray();
        }

        public static BitBoard[] InitializeRookOccupancy()
        {
            return Enumerable.Range(0, 64)
                .Select(MaskRookOccupancy)
                .ToArray();
        }

        /// <summary>
        /// Returns bishop occupancy masks and attacks
        /// </summary>
        /// <returns>(BitBoard[64], BitBoard[64, 512])</returns>
        public static (BitBoard[] BishopOccupancyMasks, BitBoard[,] BishopAttacks) InitializeBishopMagicAttacks()
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

                    var magicIndex = (occupancy * Constants.BishopMagicNumbers[square]) >> (64 - relevantBitsCount);

                    attacks[square, magicIndex] = GenerateBishopAttacksOnTheFly(square, occupancy);
                }
            }

            return (occupancyMasks, attacks);
        }

        /// <summary>
        /// Returns rook occupancy masks and attacks
        /// </summary>
        /// <returns>(BitBoard[64], BitBoard[64, 512])</returns>
        public static (BitBoard[] RookOccupancyMasks, BitBoard[,] RookAttacks) InitializeRookMagicAttacks()
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

                    var magicIndex = (occupancy * Constants.RookMagicNumbers[square]) >> (64 - relevantBitsCount);

                    attacks[square, magicIndex] = GenerateRookAttacksOnTheFly(square, occupancy);
                }
            }

            return (occupancyMasks, attacks);
        }

        public static BitBoard MaskPawnAttacks(int squareIndex, bool isWhite)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            // Piece bitboard
            BitBoard bitBoard = default;

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            if (isWhite)
            {
                /*
                 * 0 0 0 X 0
                 * 0 0 1 0 0
                 * 0 0 0 0 0
                 */
                var right = bitBoard >> 7;
                if ((right & Constants.NotAFile) != default)
                {
                    attacks |= right;
                }

                /*
                 * 0 X 0 0 0
                 * 0 0 1 0 0
                 * 0 0 0 0 0
                 */
                var left = bitBoard >> 9;
                if ((left & Constants.NotHFile) != default)
                {
                    attacks |= left;
                }
            }
            else
            {
                /*
                 * 0 0 0 0 0
                 * 0 0 1 0 0
                 * 0 X 0 0 0
                 */
                var left = bitBoard << 7;
                if ((left & Constants.NotHFile) != default)
                {
                    attacks |= left;
                }

                /*
                 * 0 0 0 0 0
                 * 0 0 1 0 0
                 * 0 0 0 X 0
                 */
                var right = bitBoard << 9;
                if ((right & Constants.NotAFile) != default)
                {
                    attacks |= right;
                }
            }

            return attacks;
        }

        public static BitBoard MaskKnightAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            // Piece bitboard
            BitBoard bitBoard = default;

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            /*
             * 0 X 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            var attack = bitBoard >> 17;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 X 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard >> 15;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 X 0 0 0
             */
            attack = bitBoard << 15;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 X 0
             */
            attack = bitBoard << 17;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * X 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard >> 10;
            if ((attack & Constants.NotHGFiles) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 X
             * 0 0 1 0 0
             * 0 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard >> 6;
            if ((attack & Constants.NotABFiles) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * X 0 0 0 0
             * 0 0 0 0 0
             */
            attack = bitBoard << 6;
            if ((attack & Constants.NotHGFiles) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0 0 0
             * 0 0 0 0 0
             * 0 0 1 0 0
             * 0 0 0 0 X
             * 0 0 0 0 0
             */
            attack = bitBoard << 10;
            if ((attack & Constants.NotABFiles) != default)
            {
                attacks |= attack;
            }

            return attacks;
        }

        public static BitBoard MaskKingAttacks(int squareIndex)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            // Piece bitboard
            BitBoard bitBoard = default;

            // Set piece on board
            bitBoard.SetBit(squareIndex);

            /*
             * X 0 0
             * 0 1 0
             * 0 0 0
             */
            var attack = bitBoard >> 9;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 X 0
             * 0 1 0
             * 0 0 0
             */
            attacks |= bitBoard >> 8;

            /*
             * 0 0 X
             * 0 1 0
             * 0 0 0
             */
            attack = bitBoard >> 7;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * X 1 0
             * 0 0 0
             */
            attack = bitBoard >> 1;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * 0 1 X
             * 0 0 0
             */
            attack = bitBoard << 1;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * 0 1 0
             * X 0 0
             */
            attack = bitBoard << 7;
            if ((attack & Constants.NotHFile) != default)
            {
                attacks |= attack;
            }

            /*
             * 0 0 0
             * 0 1 0
             * 0 X 0
             */
            attacks |= bitBoard << 8;

            /*
             * 0 0 0
             * 0 1 0
             * X 0 0
             */
            attack = bitBoard << 9;
            if ((attack & Constants.NotAFile) != default)
            {
                attacks |= attack;
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
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, file);
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
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
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
                attacks |= 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
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
        /// <param name="occupancyMask">Bishop or rook occupancy (<see cref="AttackGenerator.MaskBishopOccupancy(int)"/> and <see cref="AttackGenerator.MaskRookOccupancy(int)"/>)</param>
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
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
                {
                    break;
                }
            }

            return attacks;
        }

        public static BitBoard GenerateRookAttacksOnTheFly(int squareIndex, BitBoard occupiedSquares)
        {
            // Results attack bitboard
            BitBoard attacks = default;

            int rank, file;

            // Next target square within the attack ray of a sliding piece
            int targetRank = Math.DivRem(squareIndex, 8, out int targetFile);

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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(targetRank, file);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
                attacks |= square;

                if ((square & occupiedSquares) != default)
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
                ulong square = 1UL << BitBoardExtensions.SquareIndex(rank, targetFile);
                attacks |= square;

                if ((square & occupiedSquares) != default)
                {
                    break;
                }
            }

            return attacks;
        }
    }
}
