/*
 *  |   Method | iterations |           Mean |         Error |        StdDev | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |--------- |----------- |---------------:|--------------:|--------------:|------:|--------:|----------:|------------:|
 *  | Constant |          1 |       4.663 ns |     0.1549 ns |     0.1449 ns |  1.00 |    0.00 |         - |          NA |
 *  |     Cast |          1 |       4.122 ns |     0.1462 ns |     0.1795 ns |  0.88 |    0.07 |         - |          NA |
 *  |          |            |                |               |               |       |         |           |             |
 *  | Constant |         10 |      22.844 ns |     0.5169 ns |     0.5077 ns |  1.00 |    0.00 |         - |          NA |
 *  |     Cast |         10 |      23.195 ns |     0.4863 ns |     0.7714 ns |  1.02 |    0.04 |         - |          NA |
 *  |          |            |                |               |               |       |         |           |             |
 *  | Constant |       1000 |   2,754.421 ns |    14.6464 ns |    12.9836 ns |  1.00 |    0.00 |         - |          NA |
 *  |     Cast |       1000 |   2,768.868 ns |    16.7819 ns |    14.8767 ns |  1.01 |    0.01 |         - |          NA |
 *  |          |            |                |               |               |       |         |           |             |
 *  | Constant |      10000 |  27,710.451 ns |   165.8820 ns |   138.5190 ns |  1.00 |    0.00 |         - |          NA |
 *  |     Cast |      10000 |  28,080.178 ns |   289.2892 ns |   270.6013 ns |  1.01 |    0.01 |         - |          NA |
 *  |          |            |                |               |               |       |         |           |             |
 *  | Constant |     100000 | 277,720.653 ns | 2,782.1333 ns | 2,602.4092 ns |  1.00 |    0.00 |         - |          NA |
 *  |     Cast |     100000 | 278,077.362 ns | 2,431.3312 ns | 2,274.2687 ns |  1.00 |    0.01 |         - |          NA |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class EnumCasting_Benchmark : BaseBenchmark
{
    public static IEnumerable<int> Data => new[] { 1, 10, 1_000, 10_000, 100_000 };

    private const int Pawn = (int)Piece.P;

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Constant(int iterations)
    {
        var sum = 0;
        for (int i = 0; i < iterations; ++i)
        {
            sum += EvaluationConstants.MiddleGamePieceValues[0][Pawn];
            sum += EvaluationConstants.MiddleGamePieceValues[0][Pawn];
            sum += EvaluationConstants.MiddleGamePieceValues[0][Pawn];
            sum += EvaluationConstants.MiddleGamePieceValues[0][Pawn];
            sum += EvaluationConstants.MiddleGamePieceValues[0][Pawn];
            sum += EvaluationConstants.MiddleGamePieceValues[0][Pawn];
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int Cast(int iterations)
    {
        var sum = 0;
        for (int i = 0; i < iterations; ++i)
        {
            sum += EvaluationConstants.MiddleGamePieceValues[0][(int)Piece.P];
            sum += EvaluationConstants.MiddleGamePieceValues[0][(int)Piece.P];
            sum += EvaluationConstants.MiddleGamePieceValues[0][(int)Piece.P];
            sum += EvaluationConstants.MiddleGamePieceValues[0][(int)Piece.P];
            sum += EvaluationConstants.MiddleGamePieceValues[0][(int)Piece.P];
            sum += EvaluationConstants.MiddleGamePieceValues[0][(int)Piece.P];
        }

        return sum;
    }
}
