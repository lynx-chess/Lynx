﻿using Lynx.Model;

namespace Lynx.Internal;

#pragma warning disable S3353 // Unchanged local variables should be "const" - FP https://community.sonarsource.com/t/fp-s3353-value-modified-in-ref-extension-method/132389
#pragma warning disable S106, S2228 // Standard outputs should not be used directly to log anything

internal static class ConstantGenerator
{
    public static BitBoard NotAFile()
    {
        BitBoard b = default;

        for (int rank = 0; rank < 8; ++rank)
        {
            for (int file = 0; file < 8; ++file)
            {
                var squareIndex = BitBoardExtensions.SquareIndex(rank, file);

                if (file > 0)
                {
                    b.SetBit(squareIndex);
                }
            }
        }

        b.Print();

        return b;
    }

    public static BitBoard NotHFile()
    {
        BitBoard b = default;

        for (int rank = 0; rank < 8; ++rank)
        {
            for (int file = 0; file < 8; ++file)
            {
                var squareIndex = BitBoardExtensions.SquareIndex(rank, file);

                if (file < 7)
                {
                    b.SetBit(squareIndex);
                }
            }
        }

        b.Print();

        return b;
    }

    public static BitBoard NotABFiles()
    {
        BitBoard b = default;

        for (int rank = 0; rank < 8; ++rank)
        {
            for (int file = 0; file < 8; ++file)
            {
                var squareIndex = BitBoardExtensions.SquareIndex(rank, file);

                if (file > 1)
                {
                    b.SetBit(squareIndex);
                }
            }
        }

        b.Print();

        return b;
    }

    public static BitBoard NotHGFiles()
    {
        BitBoard b = default;

        for (int rank = 0; rank < 8; ++rank)
        {
            for (int file = 0; file < 8; ++file)
            {
                var squareIndex = BitBoardExtensions.SquareIndex(rank, file);

                if (file < 6)
                {
                    b.SetBit(squareIndex);
                }
            }
        }

        b.Print();

        return b;
    }

    public static void PrintSquares()
    {
        for (int rank = 8; rank >= 1; --rank)
        {
            //Console.WriteLine($"a{rank}, b{rank}, c{rank}, d{rank}, e{rank}, f{rank}, g{rank}, h{rank},");
            Console.WriteLine($"\"a{rank}\", \"b{rank}\", \"c{rank}\", \"d{rank}\", \"e{rank}\", \"f{rank}\", \"g{rank}\", \"h{rank}\",");
        }
    }

    public static void PrintCoordinates()
    {
        for (int rank = 8; rank >= 1; --rank)
        {
            Console.WriteLine($"a{rank}, b{rank}, c{rank}, d{rank}, e{rank}, f{rank}, g{rank}, h{rank},");
        }
    }

    public static void BishopRelevantOccupancyBits()
    {
        for (var rank = 0; rank < 8; ++rank)
        {
            for (var file = 0; file < 8; ++file)
            {
                int square = BitBoardExtensions.SquareIndex(rank, file);

                var bishopOccupancy = AttackGenerator.MaskBishopOccupancy(square);
                Console.Write($"{bishopOccupancy.CountBits()}, ");
            }

            Console.WriteLine();
        }
    }

    public static void RookRelevantOccupancyBits()
    {
        for (var rank = 0; rank < 8; ++rank)
        {
            for (var file = 0; file < 8; ++file)
            {
                int square = BitBoardExtensions.SquareIndex(rank, file);

                var bishopOccupancy = AttackGenerator.MaskRookOccupancy(square);
                Console.Write($"{bishopOccupancy.CountBits()}, ");
            }

            Console.WriteLine();
        }
    }
}

#pragma warning restore S3353 // Unchanged local variables should be "const"
#pragma warning restore S106, S2228 // Standard outputs should not be used directly to log anything
