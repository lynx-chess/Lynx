using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class PawnIslandsBenchmark: BaseBenchmark
{
    private static readonly string[] Data = Engine._benchmarkFens;

    [Benchmark(Baseline = true)]
    public int Original()
    {
        var sum = 0;

        foreach (var fen in Engine._benchmarkFens)
        {
            var pieces = FENParser.ParseFEN(fen).PieceBitBoards;
            BitBoard whitePawns = pieces[(int)Piece.P];
            sum += Original(whitePawns);
        }

        return sum;
    }

    [Benchmark]
    public int BitBoard()
    {
        var sum = 0;

        foreach (var fen in Data)
        {
            var pieces = FENParser.ParseFEN(fen).PieceBitBoards;
            BitBoard whitePawns = pieces[(int)Piece.P];
            sum += BitBoard(whitePawns);
        }

        return sum;
    }

    private static int Original(BitBoard pawns)
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

    private static int BitBoard(BitBoard pawns)
    {
        byte pawnFileBitBoard = 0;

        while (pawns != default)
        {
            var squareIndex = pawns.GetLS1BIndex();
            pawns.ResetLS1B();

            // BitBoard.SetBit equivalent but for byte instead of ulong
            pawnFileBitBoard |= (byte)(1 << Constants.File[squareIndex]);
        }

        return PawnIslandsCount[pawnFileBitBoard];
    }

    private static ReadOnlySpan<int> PawnIslandsCount =>
    [
        0, 1, 1, 1, 1, 2, 1, 1, 1, 2,
        2, 2, 1, 2, 1, 1, 1, 2, 2, 2,
        2, 3, 2, 2, 1, 2, 2, 2, 1, 2,
        1, 1, 1, 2, 2, 2, 2, 3, 2, 2,
        2, 3, 3, 3, 2, 3, 2, 2, 1, 2,
        2, 2, 2, 3, 2, 2, 1, 2, 2, 2,
        1, 2, 1, 1, 1, 2, 2, 2, 2, 3,
        2, 2, 2, 3, 3, 3, 2, 3, 2, 2,
        2, 3, 3, 3, 3, 4, 3, 3, 2, 3,
        3, 3, 2, 3, 2, 2, 1, 2, 2, 2,
        2, 3, 2, 2, 2, 3, 3, 3, 2, 3,
        2, 2, 1, 2, 2, 2, 2, 3, 2, 2,
        1, 2, 2, 2, 1, 2, 1, 1, 1, 2,
        2, 2, 2, 3, 2, 2, 2, 3, 3, 3,
        2, 3, 2, 2, 2, 3, 3, 3, 3, 4,
        3, 3, 2, 3, 3, 3, 2, 3, 2, 2,
        2, 3, 3, 3, 3, 4, 3, 3, 3, 4,
        4, 4, 3, 4, 3, 3, 2, 3, 3, 3,
        3, 4, 3, 3, 2, 3, 3, 3, 2, 3,
        2, 2, 1, 2, 2, 2, 2, 3, 2, 2,
        2, 3, 3, 3, 2, 3, 2, 2, 2, 3,
        3, 3, 3, 4, 3, 3, 2, 3, 3, 3,
        2, 3, 2, 2, 1, 2, 2, 2, 2, 3,
        2, 2, 2, 3, 3, 3, 2, 3, 2, 2,
        1, 2, 2, 2, 2, 3, 2, 2, 1, 2,
        2, 2, 1, 2, 1,
        1
    ];

}
