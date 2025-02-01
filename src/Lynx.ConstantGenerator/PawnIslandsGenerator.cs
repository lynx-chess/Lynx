using Lynx.Model;

namespace Lynx.ConstantGenerator;

public static class PawnIslandsGenerator
{
    /// <summary>
    /// Used to generate <see cref="EvaluationConstants.PawnIslandsCount"/>
    /// </summary>
    public static void GeneratePawnIslands()
    {
        Span<int> result = stackalloc int[byte.MaxValue];

        for (byte n = byte.MinValue; n < byte.MaxValue; ++n)
        {
#pragma warning disable S3353 // Unchanged local variables should be "const" - FP https://community.sonarsource.com/t/fp-s3353-value-modified-in-ref-extension-method/132389
            BitBoard bitboard = 0;
#pragma warning restore S3353 // Unchanged local variables should be "const"

            for (int file = 0; file < 8; ++file)
            {
                var pawnInFile = n.GetBit(file);

                if (pawnInFile)
                {
                    bitboard.SetBit(file);
                }
            }

            result[n] = IdentifyIslands(bitboard);
        }

        Console.Write("\t[\n\t\t");

        for (int i = 0; i < result.Length; ++i)
        {
            Console.Write(result[i]);

            if ((i + 1) % 10 == 0)
            {
                Console.Write($",{Environment.NewLine}\t\t");
            }
            else
            {
                Console.Write(", ");
            }
        }

        // Manual last bit, all pawns
        Console.WriteLine("\n\t\t1");

        Console.Write("\t]");
    }

    private static bool GetBit(this byte board, int file)
    {
        return (board & (1 << file)) != default;
    }

    public static int IdentifyIslands(BitBoard pawns)
    {
        const int n = 1;

        Span<int> files = stackalloc int[8];

        while (pawns != default)
        {
            var squareIndex = pawns.GetLS1BIndex();
            pawns.ResetLS1B();

            files[Constants.File[squareIndex]] = n;
        }

        var islandCount = 0;
        var isIsland = false;

        for (int file = 0; file < files.Length; ++file)
        {
            if (files[file] == n)
            {
                if (!isIsland)
                {
                    isIsland = true;
                    ++islandCount;
                }
            }
            else
            {
                isIsland = false;
            }
        }

        return islandCount;
    }
}
