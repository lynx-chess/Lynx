using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Collections.Frozen;

namespace Lynx.Benchmark;
public class EnPassantCaptureSquares : BaseBenchmark
{
    public static IEnumerable<BoardSquare> Data => new[] {
        BoardSquare.a3, BoardSquare.c3, BoardSquare.e3, BoardSquare.g3,
        BoardSquare.b6, BoardSquare.d6, BoardSquare.f6, BoardSquare.h6
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Dictionary(BoardSquare square) => EnPassantCaptureSquaresDictionary[(int)square];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Array(BoardSquare square) => Constants.EnPassantCaptureSquares[(int)square];

    private static readonly FrozenDictionary<int, int> EnPassantCaptureSquaresDictionary = new Dictionary<int, int>(16)
    {
        [(int)BoardSquare.a6] = (int)BoardSquare.a6 + 8,
        [(int)BoardSquare.b6] = (int)BoardSquare.b6 + 8,
        [(int)BoardSquare.c6] = (int)BoardSquare.c6 + 8,
        [(int)BoardSquare.d6] = (int)BoardSquare.d6 + 8,
        [(int)BoardSquare.e6] = (int)BoardSquare.e6 + 8,
        [(int)BoardSquare.f6] = (int)BoardSquare.f6 + 8,
        [(int)BoardSquare.g6] = (int)BoardSquare.g6 + 8,
        [(int)BoardSquare.h6] = (int)BoardSquare.h6 + 8,

        [(int)BoardSquare.a3] = (int)BoardSquare.a3 - 8,
        [(int)BoardSquare.b3] = (int)BoardSquare.b3 - 8,
        [(int)BoardSquare.c3] = (int)BoardSquare.c3 - 8,
        [(int)BoardSquare.d3] = (int)BoardSquare.d3 - 8,
        [(int)BoardSquare.e3] = (int)BoardSquare.e3 - 8,
        [(int)BoardSquare.f3] = (int)BoardSquare.f3 - 8,
        [(int)BoardSquare.g3] = (int)BoardSquare.g3 - 8,
        [(int)BoardSquare.h3] = (int)BoardSquare.h3 - 8,
    }.ToFrozenDictionary();
}
