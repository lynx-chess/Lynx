/*
 * Not super clean conclusions
 *
 *  BenchmarkDotNet v0.13.7, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *  |          Method |   iter |            Mean |        Error |       StdDev | Ratio |        Gen0 |      Gen1 |     Allocated | Alloc Ratio |
 *  |---------------- |------- |----------------:|-------------:|-------------:|------:|------------:|----------:|--------------:|------------:|
 *  |          Arrays |      1 |        41.60 us |     0.062 us |     0.055 us |  1.00 |      1.4648 |         - |      26.85 KB |        1.00 |
 *  | ImmutableArrays |      1 |        41.67 us |     0.067 us |     0.059 us |  1.00 |      1.4648 |         - |      26.85 KB |        1.00 |
 *  |                 |        |                 |              |              |       |             |           |               |             |
 *  |          Arrays |     10 |       408.67 us |     1.441 us |     1.348 us |  1.00 |     14.6484 |         - |     268.48 KB |        1.00 |
 *  | ImmutableArrays |     10 |       418.75 us |     0.513 us |     0.455 us |  1.02 |     14.6484 |         - |     268.48 KB |        1.00 |
 *  |                 |        |                 |              |              |       |             |           |               |             |
 *  |          Arrays |   1000 |    41,790.62 us |    68.653 us |    64.218 us |  1.00 |   1416.6667 |         - |    26848.1 KB |        1.00 |
 *  | ImmutableArrays |   1000 |    42,096.94 us |   179.422 us |   167.831 us |  1.01 |   1416.6667 |         - |    26848.1 KB |        1.00 |
 *  |                 |        |                 |              |              |       |             |           |               |             |
 *  |          Arrays |  10000 |   420,239.60 us |   677.067 us |   528.609 us |  1.00 |  14000.0000 |         - |  268481.11 KB |        1.00 |
 *  | ImmutableArrays |  10000 |   422,514.31 us | 1,878.245 us | 1,665.016 us |  1.01 |  14000.0000 |         - |  268481.11 KB |        1.00 |
 *  |                 |        |                 |              |              |       |             |           |               |             |
 *  |          Arrays | 100000 | 4,229,568.11 us | 6,061.068 us | 5,669.526 us |  1.00 | 147000.0000 | 2000.0000 | 2684798.91 KB |        1.00 |
 *  | ImmutableArrays | 100000 | 4,205,192.73 us | 7,873.600 us | 7,364.970 us |  0.99 | 147000.0000 | 2000.0000 | 2684798.91 KB |        1.00 |
 *
 *  BenchmarkDotNet v0.13.7, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *
 *
 *  |          Method |   iter |            Mean |         Error |        StdDev | Ratio | RatioSD |        Gen0 |      Gen1 |     Allocated | Alloc Ratio |
 *  |---------------- |------- |----------------:|--------------:|--------------:|------:|--------:|------------:|----------:|--------------:|------------:|
 *  |          Arrays |      1 |        47.40 us |      1.266 us |      3.732 us |  1.00 |    0.00 |      1.0376 |         - |      26.85 KB |        1.00 |
 *  | ImmutableArrays |      1 |        43.21 us |      0.847 us |      1.343 us |  0.98 |    0.06 |      1.0376 |         - |      26.85 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |     10 |       458.44 us |      8.994 us |     14.524 us |  1.00 |    0.00 |     10.2539 |         - |     268.47 KB |        1.00 |
 *  | ImmutableArrays |     10 |       464.92 us |      9.272 us |     20.928 us |  1.01 |    0.07 |     10.2539 |         - |     268.47 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |   1000 |    43,162.13 us |    851.929 us |  1,579.106 us |  1.00 |    0.00 |   1000.0000 |         - |   26846.91 KB |        1.00 |
 *  | ImmutableArrays |   1000 |    44,701.37 us |    861.469 us |  2,177.041 us |  1.04 |    0.07 |   1000.0000 |         - |   26846.91 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |  10000 |   441,775.52 us |  8,677.563 us | 17,330.032 us |  1.00 |    0.00 |  10000.0000 |         - |  268469.67 KB |        1.00 |
 *  | ImmutableArrays |  10000 |   437,281.72 us |  8,689.599 us | 19,435.534 us |  0.99 |    0.06 |  10000.0000 |         - |  268469.67 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays | 100000 | 4,503,057.66 us | 73,151.820 us | 64,847.183 us |  1.00 |    0.00 | 105000.0000 | 1000.0000 | 2684678.81 KB |        1.00 |
 *  | ImmutableArrays | 100000 | 4,240,336.73 us | 70,315.686 us | 58,716.782 us |  0.94 |    0.02 | 105000.0000 | 1000.0000 | 2684678.81 KB |        1.00 |
 *
 *  BenchmarkDotNet v0.13.7, Windows 10 (10.0.20348.1906) (Hyper-V)
 *  Intel Xeon CPU E5-2673 v3 2.40GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *
 *  |          Method |   iter |            Mean |         Error |        StdDev | Ratio | RatioSD |        Gen0 |      Gen1 |     Allocated | Alloc Ratio |
 *  |---------------- |------- |----------------:|--------------:|--------------:|------:|--------:|------------:|----------:|--------------:|------------:|
 *  |          Arrays |      1 |        50.99 us |      0.802 us |      0.750 us |  1.00 |    0.00 |      1.7090 |         - |      26.85 KB |        1.00 |
 *  | ImmutableArrays |      1 |        41.09 us |      0.611 us |      0.541 us |  0.81 |    0.02 |      1.7090 |         - |      26.85 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |     10 |       443.30 us |      8.198 us |      7.668 us |  1.00 |    0.00 |     17.0898 |         - |     268.49 KB |        1.00 |
 *  | ImmutableArrays |     10 |       413.65 us |      8.029 us |     10.440 us |  0.92 |    0.03 |     17.0898 |         - |     268.49 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |   1000 |    38,320.54 us |    755.464 us |  1,437.349 us |  1.00 |    0.00 |   1692.3077 |         - |   26848.84 KB |        1.00 |
 *  | ImmutableArrays |   1000 |    37,964.76 us |    735.056 us |    955.780 us |  0.99 |    0.04 |   1714.2857 |         - |   26848.88 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |  10000 |   384,313.32 us |  7,428.782 us |  7,296.057 us |  1.00 |    0.00 |  17000.0000 |         - |  268489.36 KB |        1.00 |
 *  | ImmutableArrays |  10000 |   373,332.67 us |  6,305.011 us |  5,897.711 us |  0.97 |    0.02 |  17000.0000 |         - |  268489.36 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays | 100000 | 3,873,367.47 us | 53,232.383 us | 79,675.725 us |  1.00 |    0.00 | 175000.0000 | 2000.0000 | 2684878.64 KB |        1.00 |
 *  | ImmutableArrays | 100000 | 4,198,284.97 us | 29,017.047 us | 27,142.563 us |  1.07 |    0.03 | 175000.0000 | 2000.0000 | 2684878.64 KB |        1.00 |
 *
 *  BenchmarkDotNet v0.13.7, Windows 10 (10.0.20348.1906) (Hyper-V)
 *  Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX2
 *
 *
 *  |          Method |   iter |            Mean |         Error |        StdDev | Ratio | RatioSD |        Gen0 |      Gen1 |     Allocated | Alloc Ratio |
 *  |---------------- |------- |----------------:|--------------:|--------------:|------:|--------:|------------:|----------:|--------------:|------------:|
 *  |          Arrays |      1 |        47.35 us |      0.944 us |      1.088 us |  1.00 |    0.00 |      1.4648 |         - |      26.85 KB |        1.00 |
 *  | ImmutableArrays |      1 |        46.57 us |      0.465 us |      0.388 us |  0.98 |    0.02 |      1.4648 |         - |      26.85 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |     10 |       472.44 us |      9.372 us |      9.624 us |  1.00 |    0.00 |     14.6484 |         - |     268.48 KB |        1.00 |
 *  | ImmutableArrays |     10 |       465.66 us |      1.191 us |      1.056 us |  0.99 |    0.02 |     14.6484 |         - |     268.48 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |   1000 |    46,737.23 us |    225.305 us |    210.750 us |  1.00 |    0.00 |   1454.5455 |         - |   26848.23 KB |        1.00 |
 *  | ImmutableArrays |   1000 |    47,293.74 us |    932.314 us |  1,073.655 us |  1.02 |    0.03 |   1454.5455 |         - |    26848.2 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |  10000 |   468,128.54 us |  5,444.327 us |  4,826.254 us |  1.00 |    0.00 |  14000.0000 |         - |  268480.78 KB |        1.00 |
 *  | ImmutableArrays |  10000 |   466,478.78 us |  7,587.221 us |  6,725.873 us |  1.00 |    0.02 |  14000.0000 |         - |  268480.78 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays | 100000 | 4,679,014.42 us | 34,887.729 us | 29,132.834 us |  1.00 |    0.00 | 147000.0000 | 2000.0000 | 2684798.58 KB |        1.00 |
 *  | ImmutableArrays | 100000 | 4,705,577.22 us | 29,587.189 us | 26,228.272 us |  1.00 |    0.01 | 147000.0000 | 2000.0000 | 2684798.58 KB |        1.00 |
 *
 *   BenchmarkDotNet v0.13.7, macOS Monterey 12.6.7 (21G651) [Darwin 21.6.0]
 *   Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *   .NET SDK 8.0.100-preview.7.23376.3
 *     [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *     DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *
 *   |          Method |   iter |            Mean |          Error |         StdDev |          Median | Ratio | RatioSD |        Gen0 |      Gen1 |     Allocated | Alloc Ratio |
 *   |---------------- |------- |----------------:|---------------:|---------------:|----------------:|------:|--------:|------------:|----------:|--------------:|------------:|
 *   |          Arrays |      1 |        56.70 us |       6.190 us |      18.251 us |        49.87 us |  1.00 |    0.00 |      4.2725 |         - |      26.86 KB |        1.00 |
 *   | ImmutableArrays |      1 |        36.61 us |       0.689 us |       1.052 us |        36.37 us |  0.87 |    0.20 |      4.3335 |         - |      26.86 KB |        1.00 |
 *   |                 |        |                 |                |                |                 |       |         |             |           |               |             |
 *   |          Arrays |     10 |       447.64 us |      14.069 us |      41.260 us |       465.79 us |  1.00 |    0.00 |     43.4570 |    0.4883 |     268.56 KB |        1.00 |
 *   | ImmutableArrays |     10 |       402.72 us |       7.619 us |       8.152 us |       402.82 us |  0.93 |    0.08 |     42.9688 |         - |     268.56 KB |        1.00 |
 *   |                 |        |                 |                |                |                 |       |         |             |           |               |             |
 *   |          Arrays |   1000 |    38,615.67 us |     733.331 us |     953.537 us |    38,609.20 us |  1.00 |    0.00 |   4300.0000 |         - |    26856.4 KB |        1.00 |
 *   | ImmutableArrays |   1000 |    36,178.08 us |     484.936 us |     404.944 us |    36,235.33 us |  0.94 |    0.02 |   4000.0000 |         - |   26858.77 KB |        1.00 |
 *   |                 |        |                 |                |                |                 |       |         |             |           |               |             |
 *   |          Arrays |  10000 |   400,966.25 us |   3,075.635 us |   2,568.294 us |   401,267.46 us |  1.00 |    0.00 |  43000.0000 |         - |  268564.03 KB |        1.00 |
 *   | ImmutableArrays |  10000 |   392,823.11 us |   7,684.592 us |   9,147.961 us |   388,576.76 us |  0.99 |    0.02 |  43000.0000 |         - |  268564.03 KB |        1.00 |
 *   |                 |        |                 |                |                |                 |       |         |             |           |               |             |
 *   |          Arrays | 100000 | 4,300,519.64 us | 153,287.752 us | 449,566.443 us | 4,387,075.73 us |  1.00 |    0.00 | 438000.0000 | 5000.0000 | 2685630.98 KB |        1.00 |
 *   | ImmutableArrays | 100000 | 3,768,899.25 us |  71,739.147 us |  67,104.842 us | 3,745,878.87 us |  0.90 |    0.07 | 438000.0000 | 6000.0000 | 2685630.98 KB |        1.00 |
 *
 *  BenchmarkDotNet v0.13.7, macOS Monterey 12.6.8 (21G725) [Darwin 21.6.0]
 *  Intel Xeon CPU E5-1650 v2 3.50GHz (Max: 3.34GHz), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.100-preview.7.23376.3
 *    [Host]     : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *    DefaultJob : .NET 8.0.0 (8.0.23.37506), X64 RyuJIT AVX
 *
 *
 *  |          Method |   iter |            Mean |         Error |        StdDev | Ratio | RatioSD |        Gen0 |      Gen1 |     Allocated | Alloc Ratio |
 *  |---------------- |------- |----------------:|--------------:|--------------:|------:|--------:|------------:|----------:|--------------:|------------:|
 *  |          Arrays |      1 |        49.43 us |      2.820 us |      8.181 us |  1.00 |    0.00 |      4.3335 |         - |      26.86 KB |        1.00 |
 *  | ImmutableArrays |      1 |        46.17 us |      2.161 us |      6.130 us |  0.95 |    0.16 |      4.2725 |         - |      26.86 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |     10 |       384.56 us |      7.249 us |      8.903 us |  1.00 |    0.00 |     43.4570 |    0.4883 |     268.56 KB |        1.00 |
 *  | ImmutableArrays |     10 |       383.71 us |      6.881 us |      6.437 us |  1.00 |    0.02 |     43.4570 |    0.4883 |     268.56 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |   1000 |    37,494.06 us |    580.845 us |    485.032 us |  1.00 |    0.00 |   4307.6923 |         - |   26856.34 KB |        1.00 |
 *  | ImmutableArrays |   1000 |    38,776.87 us |    756.190 us |    809.115 us |  1.03 |    0.03 |   4333.3333 |         - |   26856.44 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays |  10000 |   378,807.69 us |  7,151.021 us |  8,235.127 us |  1.00 |    0.00 |  43000.0000 |         - |  268564.03 KB |        1.00 |
 *  | ImmutableArrays |  10000 |   371,917.55 us |  4,544.532 us |  3,794.890 us |  0.98 |    0.03 |  43000.0000 |         - |  268564.03 KB |        1.00 |
 *  |                 |        |                 |               |               |       |         |             |           |               |             |
 *  |          Arrays | 100000 | 3,848,441.03 us | 75,838.385 us | 74,483.434 us |  1.00 |    0.00 | 438000.0000 | 6000.0000 | 2685630.98 KB |        1.00 |
 *  | ImmutableArrays | 100000 | 3,819,221.79 us | 32,212.327 us | 30,131.430 us |  0.99 |    0.02 | 438000.0000 | 6000.0000 | 2685630.98 KB |        1.00 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Collections.Immutable;

namespace Lynx.Benchmark;
public class PsqtImmutableArrays_Benchmark : BaseBenchmark
{
    public static IEnumerable<int> Data => [1, 10, 1_000, 10_000, 100_000,];

    public static IEnumerable<Position> Positions =>
        [
            new(Constants.InitialPositionFEN),
            new(Constants.TrickyTestPositionFEN),
            new(Constants.TrickyTestPositionReversedFEN),
            new(Constants.CmkTestPositionFEN),
            new(Constants.TTPositionFEN),
            new("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10"),
            new("3K4/8/8/8/8/8/4p3/2k2R2 b - - 0 1"),
            new("2K2r2/4P3/8/8/8/8/8/3k4 w - - 0 1"),
            new("8/p7/8/1P6/K1k3p1/6P1/7P/8 w - -"),
        ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Arrays(int iter)
    {
        int eval = 0;
        for (int i = 0; i < iter; ++i)
        {
            foreach (var position in Positions)
            {
                for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
                {
                    eval += Arrays_PositionEvaluation(position, pieceIndex);
                }
            }
        }

        return eval;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int ImmutableArrays(int iter)
    {
        int eval = 0;
        for (int i = 0; i < iter; ++i)
        {
            foreach (var position in Positions)
            {
                for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
                {
                    eval += ImmutableArrays_PositionEvaluation(position, pieceIndex);
                }
            }
        }

        return eval;
    }

    private static int ImmutableArrays_PositionEvaluation(Position position, int piece)
    {
        var bitBoard = position.PieceBitBoards[piece];
        int eval = 0;

        while (!bitBoard.Empty())
        {
            var pieceSquareIndex = bitBoard.GetLS1BIndex();
            bitBoard.ResetLS1B();
            eval += PsqtImmutableArrays_ImmutableArrayEvaluationConstants.MiddleGameTable[piece, pieceSquareIndex];
        }

        return eval;
    }

    private static int Arrays_PositionEvaluation(Position position, int piece)
    {
        var bitBoard = position.PieceBitBoards[piece];
        int eval = 0;

        while (!bitBoard.Empty())
        {
            var pieceSquareIndex = bitBoard.GetLS1BIndex();
            bitBoard.ResetLS1B();
            eval += PsqtImmutableArrays_ArrayEvaluationConstants.MiddleGameTable[piece, pieceSquareIndex];
        }

        return eval;
    }

    private static class PsqtImmutableArrays_ImmutableArrayEvaluationConstants
    {
        public static readonly ImmutableArray<int> MiddleGamePieceValues =
        [
            +82, +337, +365, +477, +1025, 0,
            -82, -337, -365, -477, -1025, 0,
        ];

        public static readonly ImmutableArray<int> EndGamePieceValues =
        [
            +94, +281, +297, +512, +936, 0,
            -94, -281, -297, -512, -936, 0
        ];

        public static readonly ImmutableArray<int> MiddleGamePawnTable =
        [
              0, 0, 0, 0, 0, 0, 0, 0,
             98, 134, 61, 95, 68, 126, 34, -11,
             -6, 7, 26, 31, 65, 56, 25, -20,
            -14, 13, 6, 21, 23, 12, 17, -23,
            -27, -2, -5, 12, 17, 6, 10, -25,
            -26, -4, -4, -10, 3, 3, 33, -12,
            -35, -1, -20, -23, -15, 24, 38, -22,
              0, 0, 0, 0, 0, 0, 0, 0
        ];

        public static readonly ImmutableArray<int> EndGamePawnTable =
        [
              0, 0, 0, 0, 0, 0, 0, 0,
            178, 173, 158, 134, 147, 132, 165, 187,
             94, 100, 85, 67, 56, 53, 82, 84,
             32, 24, 13, 5, -2, 4, 17, 17,
             13, 9, -3, -7, -7, -8, 3, -1,
              4, 7, -6, 1, 0, -5, -1, -8,
             13, 8, 8, 10, 13, 0, 2, -7,
              0, 0, 0, 0, 0, 0, 0, 0
        ];

        public static readonly ImmutableArray<int> MiddleGameKnightTable =
        [
            -167, -89, -34, -49, 61, -97, -15, -107,
             -73, -41, 72, 36, 23, 62, 7, -17,
             -47, 60, 37, 65, 84, 129, 73, 44,
              -9, 17, 19, 53, 37, 69, 18, 22,
             -13, 4, 16, 13, 28, 19, 21, -8,
             -23, -9, 12, 10, 19, 17, 25, -16,
             -29, -53, -12, -3, -1, 18, -14, -19,
            -105, -21, -58, -33, -17, -28, -19, -23
        ];

        public static readonly ImmutableArray<int> EndGameKnightTable =
        [
            -58, -38, -13, -28, -31, -27, -63, -99,
            -25, -8, -25, -2, -9, -25, -24, -52,
            -24, -20, 10, 9, -1, -9, -19, -41,
            -17, 3, 22, 22, 22, 11, 8, -18,
            -18, -6, 16, 25, 16, 17, 4, -18,
            -23, -3, -1, 15, 10, -3, -20, -22,
            -42, -20, -10, -5, -2, -20, -23, -44,
            -29, -51, -23, -15, -22, -18, -50, -64
        ];

        public static readonly ImmutableArray<int> MiddleGameBishopTable =
        [
            -29, 4, -82, -37, -25, -42, 7, -8,
            -26, 16, -18, -13, 30, 59, 18, -47,
            -16, 37, 43, 40, 35, 50, 37, -2,
             -4, 5, 19, 50, 37, 37, 7, -2,
             -6, 13, 13, 26, 34, 12, 10, 4,
              0, 15, 15, 15, 14, 27, 18, 10,
              4, 15, 16, 0, 7, 21, 33, 1,
            -33, -3, -14, -21, -13, -12, -39, -21
        ];

        public static readonly ImmutableArray<int> EndGameBishopTable =
        [
            -14, -21, -11, -8, -7, -9, -17, -24,
             -8, -4, 7, -12, -3, -13, -4, -14,
              2, -8, 0, -1, -2, 6, 0, 4,
             -3, 9, 12, 9, 14, 10, 3, 2,
             -6, 3, 13, 19, 7, 10, -3, -9,
            -12, -3, 8, 10, 13, 3, -7, -15,
            -14, -18, -7, -1, 4, -9, -15, -27,
            -23, -9, -23, -5, -9, -16, -5, -17
        ];

        public static readonly ImmutableArray<int> MiddleGameRookTable =
        [
             32, 42, 32, 51, 63, 9, 31, 43,
             27, 32, 58, 62, 80, 67, 26, 44,
             -5, 19, 26, 36, 17, 45, 61, 16,
            -24, -11, 7, 26, 24, 35, -8, -20,
            -36, -26, -12, -1, 9, -7, 6, -23,
            -45, -25, -16, -17, 3, 0, -5, -33,
            -44, -16, -20, -9, -1, 11, -6, -71,
            -19, -13, 1, 17, 16, 7, -37, -26
        ];

        public static readonly ImmutableArray<int> EndGameRookTable =
        [
            13, 10, 18, 15, 12, 12, 8, 5,
            11, 13, 13, 11, -3, 3, 8, 3,
             7, 7, 7, 5, 4, -3, -5, -3,
             4, 3, 13, 1, 2, 1, -1, 2,
             3, 5, 8, 4, -5, -6, -8, -11,
            -4, 0, -5, -1, -7, -12, -8, -16,
            -6, -6, 0, 2, -9, -9, -11, -3,
            -9, 2, 3, -1, -5, -13, 4, -20
        ];

        public static readonly ImmutableArray<int> MiddleGameQueenTable =
        [
            -28, 0, 29, 12, 59, 44, 43, 45,
            -24, -39, -5, 1, -16, 57, 28, 54,
            -13, -17, 7, 8, 29, 56, 47, 57,
            -27, -27, -16, -16, -1, 17, -2, 1,
             -9, -26, -9, -10, -2, -4, 3, -3,
            -14, 2, -11, -2, -5, 2, 14, 5,
            -35, -8, 11, 2, 8, 15, -3, 1,
             -1, -18, -9, 10, -15, -25, -31, -50
        ];

        public static readonly ImmutableArray<int> EndGameQueenTable =
        [
             -9, 22, 22, 27, 27, 19, 10, 20,
            -17, 20, 32, 41, 58, 25, 30, 0,
            -20, 6, 9, 49, 47, 35, 19, 9,
              3, 22, 24, 45, 57, 40, 57, 36,
            -18, 28, 19, 47, 31, 34, 39, 23,
            -16, -27, 15, 6, 9, 17, 10, 5,
            -22, -23, -30, -16, -16, -23, -36, -32,
            -33, -28, -22, -43, -5, -32, -20, -41
        ];

        public static readonly ImmutableArray<int> MiddleGameKingTable =
        [
            -65, 23, 16, -15, -56, -34, 2, 13,
             29, -1, -20, -7, -8, -4, -38, -29,
             -9, 24, 2, -16, -20, 6, 22, -22,
            -17, -20, -12, -27, -30, -25, -14, -36,
            -49, -1, -27, -39, -46, -44, -33, -51,
            -14, -14, -22, -46, -44, -30, -15, -27,
              1, 7, -8, -64, -43, -16, 9, 8,
            -15, 36, 12, -54, 8, -28, 24, 14
        ];

        public static readonly ImmutableArray<int> EndGameKingTable =
        [
            -74, -35, -18, -18, -11, 15, 4, -17,
            -12, 17, 14, 17, 17, 38, 23, 11,
             10, 17, 23, 15, 20, 45, 44, 13,
             -8, 22, 24, 27, 26, 33, 26, 3,
            -18, -4, 21, 24, 27, 23, 9, -11,
            -19, -3, 11, 21, 23, 16, 7, -9,
            -27, -11, 4, 13, 14, 4, -5, -17,
            -53, -34, -21, -11, -28, -14, -24, -43
        ];

        public static readonly ImmutableArray<int> MiddleGamePawnTableBlack = MiddleGamePawnTable.Select((_, index) => -MiddleGamePawnTable[index ^ 56]).ToImmutableArray();
        public static readonly ImmutableArray<int> EndGamePawnTableBlack = EndGamePawnTable.Select((_, index) => -EndGamePawnTable[index ^ 56]).ToImmutableArray();

        public static readonly ImmutableArray<int> MiddleGameKnightTableBlack = MiddleGameKnightTable.Select((_, index) => -MiddleGameKnightTable[index ^ 56]).ToImmutableArray();
        public static readonly ImmutableArray<int> EndGameKnightTableBlack = EndGameKnightTable.Select((_, index) => -EndGameKnightTable[index ^ 56]).ToImmutableArray();

        public static readonly ImmutableArray<int> MiddleGameBishopTableBlack = MiddleGameBishopTable.Select((_, index) => -MiddleGameBishopTable[index ^ 56]).ToImmutableArray();
        public static readonly ImmutableArray<int> EndGameBishopTableBlack = EndGameBishopTable.Select((_, index) => -EndGameBishopTable[index ^ 56]).ToImmutableArray();

        public static readonly ImmutableArray<int> MiddleGameRookTableBlack = MiddleGameRookTable.Select((_, index) => -MiddleGameRookTable[index ^ 56]).ToImmutableArray();
        public static readonly ImmutableArray<int> EndGameRookTableBlack = EndGameRookTable.Select((_, index) => -EndGameRookTable[index ^ 56]).ToImmutableArray();

        public static readonly ImmutableArray<int> MiddleGameQueenTableBlack = MiddleGameQueenTable.Select((_, index) => -MiddleGameQueenTable[index ^ 56]).ToImmutableArray();
        public static readonly ImmutableArray<int> EndGameQueenTableBlack = EndGameQueenTable.Select((_, index) => -EndGameQueenTable[index ^ 56]).ToImmutableArray();

        public static readonly ImmutableArray<int> MiddleGameKingTableBlack = MiddleGameKingTable.Select((_, index) => -MiddleGameKingTable[index ^ 56]).ToImmutableArray();
        public static readonly ImmutableArray<int> EndGameKingTableBlack = EndGameKingTable.Select((_, index) => -EndGameKingTable[index ^ 56]).ToImmutableArray();

        public static readonly ImmutableArray<ImmutableArray<int>> MiddleGamePositionalTables =
        [
            MiddleGamePawnTable,
            MiddleGameKnightTable,
            MiddleGameBishopTable,
            MiddleGameRookTable,
            MiddleGameQueenTable,
            MiddleGameKingTable,

            MiddleGamePawnTableBlack,
            MiddleGameKnightTableBlack,
            MiddleGameBishopTableBlack,
            MiddleGameRookTableBlack,
            MiddleGameQueenTableBlack,
            MiddleGameKingTableBlack
        ];

        public static readonly ImmutableArray<ImmutableArray<int>> EndGamePositionalTables =
        [
            EndGamePawnTable,
            EndGameKnightTable,
            EndGameBishopTable,
            EndGameRookTable,
            EndGameQueenTable,
            EndGameKingTable,

            EndGamePawnTableBlack,
            EndGameKnightTableBlack,
            EndGameBishopTableBlack,
            EndGameRookTableBlack,
            EndGameQueenTableBlack,
            EndGameKingTableBlack
        ];

        public static readonly int[,] MiddleGameTable = new int[12, 64];
        public static readonly int[,] EndGameTable = new int[12, 64];

        static PsqtImmutableArrays_ImmutableArrayEvaluationConstants()
        {
            for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
            {
                for (int sq = 0; sq < 64; ++sq)
                {
                    MiddleGameTable[piece, sq] = MiddleGamePieceValues[piece] + MiddleGamePositionalTables[piece][sq];
                    EndGameTable[piece, sq] = EndGamePieceValues[piece] + EndGamePositionalTables[piece][sq];
                }
            }
        }
    }

    private static class PsqtImmutableArrays_ArrayEvaluationConstants
    {
        public static readonly int[] MiddleGamePieceValues =
        [
        +82, +337, +365, +477, +1025, 0,
        -82, -337, -365, -477, -1025, 0,
        ];

        public static readonly int[] EndGamePieceValues =
        [
        +94, +281, +297, +512, +936, 0,
        -94, -281, -297, -512, -936, 0
        ];

        public static readonly int[] MiddleGamePawnTable =
        [
          0,   0,   0,   0,   0,   0,  0,   0,
         98, 134,  61,  95,  68, 126, 34, -11,
         -6,   7,  26,  31,  65,  56, 25, -20,
        -14,  13,   6,  21,  23,  12, 17, -23,
        -27,  -2,  -5,  12,  17,   6, 10, -25,
        -26,  -4,  -4, -10,   3,   3, 33, -12,
        -35,  -1, -20, -23, -15,  24, 38, -22,
          0,   0,   0,   0,   0,   0,  0,   0
    ];

        public static readonly int[] EndGamePawnTable =
        [
          0,   0,   0,   0,   0,   0,   0,   0,
        178, 173, 158, 134, 147, 132, 165, 187,
         94, 100,  85,  67,  56,  53,  82,  84,
         32,  24,  13,   5,  -2,   4,  17,  17,
         13,   9,  -3,  -7,  -7,  -8,   3,  -1,
          4,   7,  -6,   1,   0,  -5,  -1,  -8,
         13,   8,   8,  10,  13,   0,   2,  -7,
          0,   0,   0,   0,   0,   0,   0,   0
    ];

        public static readonly int[] MiddleGameKnightTable =
        [
        -167, -89, -34, -49,  61, -97, -15, -107,
         -73, -41,  72,  36,  23,  62,   7,  -17,
         -47,  60,  37,  65,  84, 129,  73,   44,
          -9,  17,  19,  53,  37,  69,  18,   22,
         -13,   4,  16,  13,  28,  19,  21,   -8,
         -23,  -9,  12,  10,  19,  17,  25,  -16,
         -29, -53, -12,  -3,  -1,  18, -14,  -19,
        -105, -21, -58, -33, -17, -28, -19,  -23
    ];

        public static readonly int[] EndGameKnightTable =
        [
        -58, -38, -13, -28, -31, -27, -63, -99,
        -25,  -8, -25,  -2,  -9, -25, -24, -52,
        -24, -20,  10,   9,  -1,  -9, -19, -41,
        -17,   3,  22,  22,  22,  11,   8, -18,
        -18,  -6,  16,  25,  16,  17,   4, -18,
        -23,  -3,  -1,  15,  10,  -3, -20, -22,
        -42, -20, -10,  -5,  -2, -20, -23, -44,
        -29, -51, -23, -15, -22, -18, -50, -64
    ];

        public static readonly int[] MiddleGameBishopTable =
        [
        -29,   4, -82, -37, -25, -42,   7,  -8,
        -26,  16, -18, -13,  30,  59,  18, -47,
        -16,  37,  43,  40,  35,  50,  37,  -2,
         -4,   5,  19,  50,  37,  37,   7,  -2,
         -6,  13,  13,  26,  34,  12,  10,   4,
          0,  15,  15,  15,  14,  27,  18,  10,
          4,  15,  16,   0,   7,  21,  33,   1,
        -33,  -3, -14, -21, -13, -12, -39, -21
    ];

        public static readonly int[] EndGameBishopTable =
        [
        -14, -21, -11,  -8, -7,  -9, -17, -24,
         -8,  -4,   7, -12, -3, -13,  -4, -14,
          2,  -8,   0,  -1, -2,   6,   0,   4,
         -3,   9,  12,   9, 14,  10,   3,   2,
         -6,   3,  13,  19,  7,  10,  -3,  -9,
        -12,  -3,   8,  10, 13,   3,  -7, -15,
        -14, -18,  -7,  -1,  4,  -9, -15, -27,
        -23,  -9, -23,  -5, -9, -16,  -5, -17
    ];

        public static readonly int[] MiddleGameRookTable =
        [
         32,  42,  32,  51, 63,  9,  31,  43,
         27,  32,  58,  62, 80, 67,  26,  44,
         -5,  19,  26,  36, 17, 45,  61,  16,
        -24, -11,   7,  26, 24, 35,  -8, -20,
        -36, -26, -12,  -1,  9, -7,   6, -23,
        -45, -25, -16, -17,  3,  0,  -5, -33,
        -44, -16, -20,  -9, -1, 11,  -6, -71,
        -19, -13,   1,  17, 16,  7, -37, -26
    ];

        public static readonly int[] EndGameRookTable =
        [
        13, 10, 18, 15, 12,  12,   8,   5,
        11, 13, 13, 11, -3,   3,   8,   3,
         7,  7,  7,  5,  4,  -3,  -5,  -3,
         4,  3, 13,  1,  2,   1,  -1,   2,
         3,  5,  8,  4, -5,  -6,  -8, -11,
        -4,  0, -5, -1, -7, -12,  -8, -16,
        -6, -6,  0,  2, -9,  -9, -11,  -3,
        -9,  2,  3, -1, -5, -13,   4, -20
    ];

        public static readonly int[] MiddleGameQueenTable =
        [
        -28,   0,  29,  12,  59,  44,  43,  45,
        -24, -39,  -5,   1, -16,  57,  28,  54,
        -13, -17,   7,   8,  29,  56,  47,  57,
        -27, -27, -16, -16,  -1,  17,  -2,   1,
         -9, -26,  -9, -10,  -2,  -4,   3,  -3,
        -14,   2, -11,  -2,  -5,   2,  14,   5,
        -35,  -8,  11,   2,   8,  15,  -3,   1,
         -1, -18,  -9,  10, -15, -25, -31, -50
    ];

        public static readonly int[] EndGameQueenTable =
        [
         -9,  22,  22,  27,  27,  19,  10,  20,
        -17,  20,  32,  41,  58,  25,  30,   0,
        -20,   6,   9,  49,  47,  35,  19,   9,
          3,  22,  24,  45,  57,  40,  57,  36,
        -18,  28,  19,  47,  31,  34,  39,  23,
        -16, -27,  15,   6,   9,  17,  10,   5,
        -22, -23, -30, -16, -16, -23, -36, -32,
        -33, -28, -22, -43,  -5, -32, -20, -41
    ];

        public static readonly int[] MiddleGameKingTable =
        [
        -65,  23,  16, -15, -56, -34,   2,  13,
         29,  -1, -20,  -7,  -8,  -4, -38, -29,
         -9,  24,   2, -16, -20,   6,  22, -22,
        -17, -20, -12, -27, -30, -25, -14, -36,
        -49,  -1, -27, -39, -46, -44, -33, -51,
        -14, -14, -22, -46, -44, -30, -15, -27,
          1,   7,  -8, -64, -43, -16,   9,   8,
        -15,  36,  12, -54,   8, -28,  24,  14
    ];

        public static readonly int[] EndGameKingTable =
        [
        -74, -35, -18, -18, -11,  15,   4, -17,
        -12,  17,  14,  17,  17,  38,  23,  11,
         10,  17,  23,  15,  20,  45,  44,  13,
         -8,  22,  24,  27,  26,  33,  26,   3,
        -18,  -4,  21,  24,  27,  23,   9, -11,
        -19,  -3,  11,  21,  23,  16,   7,  -9,
        -27, -11,   4,  13,  14,   4,  -5, -17,
        -53, -34, -21, -11, -28, -14, -24, -43
    ];

        public static readonly int[] MiddleGamePawnTableBlack = MiddleGamePawnTable.Select((_, index) => -MiddleGamePawnTable[index ^ 56]).ToArray();
        public static readonly int[] EndGamePawnTableBlack = EndGamePawnTable.Select((_, index) => -EndGamePawnTable[index ^ 56]).ToArray();

        public static readonly int[] MiddleGameKnightTableBlack = MiddleGameKnightTable.Select((_, index) => -MiddleGameKnightTable[index ^ 56]).ToArray();
        public static readonly int[] EndGameKnightTableBlack = EndGameKnightTable.Select((_, index) => -EndGameKnightTable[index ^ 56]).ToArray();

        public static readonly int[] MiddleGameBishopTableBlack = MiddleGameBishopTable.Select((_, index) => -MiddleGameBishopTable[index ^ 56]).ToArray();
        public static readonly int[] EndGameBishopTableBlack = EndGameBishopTable.Select((_, index) => -EndGameBishopTable[index ^ 56]).ToArray();

        public static readonly int[] MiddleGameRookTableBlack = MiddleGameRookTable.Select((_, index) => -MiddleGameRookTable[index ^ 56]).ToArray();
        public static readonly int[] EndGameRookTableBlack = EndGameRookTable.Select((_, index) => -EndGameRookTable[index ^ 56]).ToArray();

        public static readonly int[] MiddleGameQueenTableBlack = MiddleGameQueenTable.Select((_, index) => -MiddleGameQueenTable[index ^ 56]).ToArray();
        public static readonly int[] EndGameQueenTableBlack = EndGameQueenTable.Select((_, index) => -EndGameQueenTable[index ^ 56]).ToArray();

        public static readonly int[] MiddleGameKingTableBlack = MiddleGameKingTable.Select((_, index) => -MiddleGameKingTable[index ^ 56]).ToArray();
        public static readonly int[] EndGameKingTableBlack = EndGameKingTable.Select((_, index) => -EndGameKingTable[index ^ 56]).ToArray();

        public static readonly int[][] MiddleGamePositionalTables =
        [
        MiddleGamePawnTable,
        MiddleGameKnightTable,
        MiddleGameBishopTable,
        MiddleGameRookTable,
        MiddleGameQueenTable,
        MiddleGameKingTable,

        MiddleGamePawnTableBlack,
        MiddleGameKnightTableBlack,
        MiddleGameBishopTableBlack,
        MiddleGameRookTableBlack,
        MiddleGameQueenTableBlack,
        MiddleGameKingTableBlack
    ];

        public static readonly int[][] EndGamePositionalTables =
        [
        EndGamePawnTable,
        EndGameKnightTable,
        EndGameBishopTable,
        EndGameRookTable,
        EndGameQueenTable,
        EndGameKingTable,

        EndGamePawnTableBlack,
        EndGameKnightTableBlack,
        EndGameBishopTableBlack,
        EndGameRookTableBlack,
        EndGameQueenTableBlack,
        EndGameKingTableBlack
    ];

        public static readonly int[,] MiddleGameTable = new int[12, 64];
        public static readonly int[,] EndGameTable = new int[12, 64];

        static PsqtImmutableArrays_ArrayEvaluationConstants()
        {
            for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
            {
                for (int sq = 0; sq < 64; ++sq)
                {
                    MiddleGameTable[piece, sq] = MiddleGamePieceValues[piece] + MiddleGamePositionalTables[piece][sq];
                    EndGameTable[piece, sq] = EndGamePieceValues[piece] + EndGamePositionalTables[piece][sq];
                }
            }
        }
    }
}
