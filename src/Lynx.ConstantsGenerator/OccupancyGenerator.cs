using Lynx.Model;

namespace Lynx.ConstantsGenerator;

public static class OccupancyGenerator
{
    public static void BishopRelevantOccupancyBits()
    {
        for (var rank = 0; rank < 8; ++rank)
        {
            for (var file = 0; file < 8; ++file)
            {
                int square = BitboardExtensions.SquareIndex(rank, file);

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
                int square = BitboardExtensions.SquareIndex(rank, file);

                var bishopOccupancy = AttackGenerator.MaskRookOccupancy(square);
                Console.Write($"{bishopOccupancy.CountBits()}, ");
            }

            Console.WriteLine();
        }
    }
}
