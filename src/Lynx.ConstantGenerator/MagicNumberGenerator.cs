using Lynx.Model;

namespace Lynx.ConstantGenerator;

#pragma warning disable S106, S2228 // Standard outputs should not be used directly to log anything

public static class MagicNumberGenerator
{
    private static readonly Random _generator = new(1160218972);

    public static ulong GetRandomU64()
    {
        static ulong GenerateRandomNumber()
        {
            // Slicing 16 bits from MS1B side
#pragma warning disable CA5394 // Do not use insecure randomness
            return ((ulong)_generator.Next()) & 0xFFFF;
#pragma warning restore CA5394 // Do not use insecure randomness
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
        Span<BitBoard> occupancies = stackalloc BitBoard[4096];
        Span<BitBoard> attacks = stackalloc BitBoard[4096];
        Span<BitBoard> usedAttacks = stackalloc BitBoard[4096];

        var occupancyMask = isBishop
            ? AttackGenerator.MaskBishopOccupancy(squareIndex)
            : AttackGenerator.MaskRookOccupancy(squareIndex);

        var relevantOccupancyBits = isBishop // or occupancyMask.CountBits()
            ? Constants.BishopRelevantOccupancyBits[squareIndex]
            : Constants.RookRelevantOccupancyBits[squareIndex];
        var occupancyIndexes = 1 << relevantOccupancyBits;

        for (int index = 0; index < occupancyIndexes; ++index)
        {
            occupancies[index] = AttackGenerator.SetBishopOrRookOccupancy(index, occupancyMask);

            attacks[index] = isBishop
                ? AttackGenerator.GenerateBishopAttacksOnTheFly(squareIndex, occupancies[index])
                : AttackGenerator.GenerateRookAttacksOnTheFly(squareIndex, occupancies[index]);
        }

        // Test magic numbers
        for (int randomCount = 0; randomCount < int.MaxValue; ++randomCount)
        {
            var magicNumber = GenerateMagicNumber();

            // Skip inappropriate magic numbers
#pragma warning disable S3937 // Number patterns should be regular
            var n = (occupancyMask * magicNumber) & 0xFF000_000_000_000_00;
#pragma warning restore S3937 // Number patterns should be regular
            if (n.CountBits() < 6)
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
                int magicIndex = (int)((occupancies[index] * magicNumber) >> (64 - relevantOccupancyBits));

                // If magic index works
                if (usedAttacks[magicIndex] == default)
                {
                    // Init used attacks
                    usedAttacks[magicIndex] = attacks[index];
                }
                else if (usedAttacks[magicIndex] != attacks[index])
                {
#pragma warning disable S127 // "for" loop stop conditions should be invariant - intentional
                    // Magic index doesn't work
                    fail = true;
#pragma warning restore S127 // "for" loop stop conditions should be invariant
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
    /// Used to generate <see cref="Constants.RookMagicNumbers"/> and <see cref="Constants.BishopMagicNumbers"/>
    /// </summary>
    public static void InitializeMagicNumbers()
    {
        Console.Write("\tpublic static ReadOnlySpan<BitBoard> RookMagicNumbers =>\n\t[\n\t\t");
        for (int square = 0; square < 64; ++square)
        {
            var magicRook = FindMagicNumbers(square, false);
            Console.Write($"0x{magicRook:x},".PadRight(22));

            if ((square + 1) % 8 == 0 && square != 63)
            {
                Console.Write($"{Environment.NewLine}\t\t");
            }
        }

        Console.WriteLine("\n\t];\n");

        Console.Write("\tpublic static ReadOnlySpan<BitBoard> BishopMagicNumbers =>\n\t[\n\t\t");
        for (int square = 0; square < 64; ++square)
        {
            var magicBishop = FindMagicNumbers(square, true);
            Console.Write($"0x{magicBishop:x},".PadRight(22));

            if ((square + 1) % 8 == 0 && square != 63)
            {
                Console.Write($"{Environment.NewLine}\t\t");
            }
        }

        Console.WriteLine("\n\t];\n");
    }
}

#pragma warning restore S106, S2228 // Standard outputs should not be used directly to log anything
