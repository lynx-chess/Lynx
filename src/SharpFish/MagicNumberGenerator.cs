using SharpFish.Model;
using System;

namespace SharpFish
{
    public static class MagicNumberGenerator
    {
        private static readonly Random _generator = new Random(1160218972);

        public static ulong GetRandomU64()
        {
            static ulong GenerateRandomNumber()
            {
                // Slicing 16 bits from MS1B side
                return ((ulong)_generator.Next()) & 0xFFFF;
            }

            // Define 4 random numbers
            var n1 = GenerateRandomNumber();
            var n2 = GenerateRandomNumber();
            var n3 = GenerateRandomNumber();
            var n4 = GenerateRandomNumber();

            return n1 | (n2 << 16) | (n3 << 32) | (n4 << 48);
        }

        public static ulong GenerateMagicNumber()
        {
#pragma warning disable S1764 // Identical expressions should not be used on both sides of a binary operator
            return GetRandomU64()
                & GetRandomU64()
                & GetRandomU64();
#pragma warning restore S1764 // Identical expressions should not be used on both sides of a binary operator
        }

        public static ulong FindMagicNumbers(int squareIndex, bool isBishop)
        {
            BitBoard[] occupancies = new BitBoard[4096];

            BitBoard[] attacks = new BitBoard[4096];

            ulong[] usedAttacks = new ulong[4096];

            var occupancyMask = isBishop
                ? AttacksGenerator.MaskBishopOccupancy(squareIndex)
                : AttacksGenerator.MaskRookOccupancy(squareIndex);

            var relevantOccupancyBits = isBishop // or occupancyMask.CountBits();
                ? Constants.BishopRelevantOccupancyBits[squareIndex]
                : Constants.RookRelevantOccupancyBits[squareIndex];
            var occupancyIndexes = 1 << relevantOccupancyBits;

            for (int index = 0; index < occupancyIndexes; ++index)
            {
                occupancies[index] = AttacksGenerator.SetBishopOrRookOccupancy(index, occupancyMask);

                attacks[index] = isBishop
                    ? AttacksGenerator.GenerateBishopAttacksOnTheFly(squareIndex, occupancies[index])
                    : AttacksGenerator.GenerateRookAttacksOnTheFly(squareIndex, occupancies[index]);
            }

            // Test magic numbers
            for (int randomCount = 0; randomCount < int.MaxValue; ++randomCount)
            {
                var magicNumber = GenerateMagicNumber();

                // Skip inappropriate magic numbers
                var n = (occupancyMask.Board * magicNumber) & 0xFF000_000_000_000_00;
                if (BitBoard.CountBits(n) < 6)
                {
                    continue;
                }

                // Init used attacks
                for (int i = 0; i < usedAttacks.Length; ++i) usedAttacks[i] = default;

                int index;
                bool fail;

                // Test magic index loop
                for (index = 0, fail = false; !fail && index < occupancyIndexes; ++index)
                {
                    // Initialize magic index
                    int magicIndex = (int)((occupancies[index].Board * magicNumber) >> (64 - relevantOccupancyBits));

                    // If magic index works
                    if (usedAttacks[magicIndex] == default)
                    {
                        // Init used attacks
                        usedAttacks[magicIndex] = attacks[index].Board;
                    }
                    else if (usedAttacks[magicIndex] != attacks[index].Board)
                    {
                        // Magic index doesn't work
                        fail = true;
                    }
                }

                if (!fail)
                {
                    return magicNumber;
                }
            }

            Console.WriteLine("Error generating magic numbers");
            return default;
        }

        /// <summary>
        /// *Untested*
        /// </summary>
        public static void InitializeMagicNumbers()
        {
            for (int square = 0; square < 64; ++square)
            {
                // Rook
                var magicRook = FindMagicNumbers(square, false);
                Console.WriteLine(magicRook);

                Console.WriteLine();

                // Bishop
                var magicBishop = FindMagicNumbers(square, true);
                Console.WriteLine(magicBishop);
            }
        }
    }
}
