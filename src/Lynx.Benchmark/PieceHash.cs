using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
public class PieceHash : BaseBenchmark
{
    private static readonly long[,] _table = Initialize();

    [Benchmark(Baseline = true)]
    private long Module() => Module((int)BoardSquare.c6);

    [Benchmark]
    private long AndTrick() => AndTrick((int)BoardSquare.c6);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Module(int enPassantSquare)
    {
        if (enPassantSquare == (int)BoardSquare.noSquare)
        {
            return default;
        }

        var file = enPassantSquare % 8;

        return _table[file, (int)Piece.P];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long AndTrick(int enPassantSquare)
    {
        if (enPassantSquare == (int)BoardSquare.noSquare)
        {
            return default;
        }

        var file = enPassantSquare & 0x03;

        return _table[file, (int)Piece.P];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long[,] Initialize()
    {
        var zobristTable = new long[64, 12];
        var randomInstance = new Random(int.MaxValue);

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                zobristTable[squareIndex, pieceIndex] = randomInstance.NextInt64();
            }
        }

        return zobristTable;
    }
}
