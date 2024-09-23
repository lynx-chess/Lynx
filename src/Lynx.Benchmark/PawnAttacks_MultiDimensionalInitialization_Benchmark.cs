/*
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method                           | data   | Mean            | Error         | StdDev        | Ratio  | RatioSD | Gen0      | Gen1     | Gen2     | Allocated   | Alloc Ratio |
 *  |--------------------------------- |------- |----------------:|--------------:|--------------:|-------:|--------:|----------:|---------:|---------:|------------:|------------:|
 *  | InitializePawnAttacks_Sequential | 64     |        272.5 ns |       1.75 ns |       1.46 ns |   1.00 |    0.00 |    0.0124 |        - |        - |     1.04 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel   | 64     |      2,484.1 ns |      47.70 ns |      54.93 ns |   9.15 |    0.24 |    0.0343 |        - |        - |     3.01 KB |        2.89 |
 *  | InitializePawnAttacks_Parallel_2 | 64     |      4,403.8 ns |      86.95 ns |     190.86 ns |  16.12 |    0.74 |    0.0610 |        - |        - |     6.67 KB |        6.42 |
 *  | InitializePawnAttacks_Parallel_3 | 64     |     34,240.5 ns |     656.66 ns |     702.62 ns | 126.11 |    2.90 |    1.2817 |        - |        - |   106.99 KB |      102.97 |
 *  |                                  |        |                 |               |               |        |         |           |          |          |             |             |
 *  | InitializePawnAttacks_Sequential | 1000   |      3,676.0 ns |      35.90 ns |      33.58 ns |   1.00 |    0.00 |    0.1907 |        - |        - |    15.66 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel   | 1000   |      6,999.9 ns |     125.70 ns |     144.75 ns |   1.91 |    0.05 |    0.2213 |   0.0076 |        - |    17.83 KB |        1.14 |
 *  | InitializePawnAttacks_Parallel_2 | 1000   |     11,397.5 ns |     226.78 ns |     269.96 ns |   3.10 |    0.09 |    0.2747 |   0.0153 |        - |    21.57 KB |        1.38 |
 *  | InitializePawnAttacks_Parallel_3 | 1000   |    362,207.2 ns |   6,170.70 ns |   6,858.72 ns |  98.45 |    2.23 |   19.5313 |        - |        - |   1635.5 KB |      104.41 |
 *  |                                  |        |                 |               |               |        |         |           |          |          |             |             |
 *  | InitializePawnAttacks_Sequential | 10000  |    104,653.1 ns |     429.35 ns |     401.61 ns |   1.00 |    0.00 |   49.9268 |  49.9268 |  49.9268 |   156.32 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel   | 10000  |     86,700.3 ns |   1,248.29 ns |   1,042.38 ns |   0.83 |    0.01 |   49.9268 |  49.9268 |  49.9268 |   158.52 KB |        1.01 |
 *  | InitializePawnAttacks_Parallel_2 | 10000  |    124,677.8 ns |   1,750.78 ns |   1,552.02 ns |   1.19 |    0.02 |   49.8047 |  49.8047 |  49.8047 |   162.38 KB |        1.04 |
 *  | InitializePawnAttacks_Parallel_3 | 10000  |  4,100,542.5 ns |  80,668.78 ns | 143,388.67 ns |  38.58 |    1.72 |  187.5000 |  46.8750 |  46.8750 | 16333.27 KB |      104.48 |
 *  |                                  |        |                 |               |               |        |         |           |          |          |             |             |
 *  | InitializePawnAttacks_Sequential | 100000 |    990,735.8 ns |   4,593.80 ns |   4,072.28 ns |   1.00 |    0.00 |  498.0469 | 498.0469 | 498.0469 |  1562.87 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel   | 100000 |    710,565.0 ns |   5,996.52 ns |   5,315.76 ns |   0.72 |    0.01 |  499.0234 | 499.0234 | 499.0234 |   1565.2 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel_2 | 100000 |  1,002,582.5 ns |   6,260.73 ns |   5,549.97 ns |   1.01 |    0.01 |  498.0469 | 498.0469 | 498.0469 |  1569.13 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel_3 | 100000 | 34,800,610.1 ns | 493,833.43 ns | 437,770.47 ns |  35.13 |    0.50 | 2400.0000 | 466.6667 | 466.6667 | 163307.2 KB |      104.49 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *
 *  | Method                           | data   | Mean            | Error         | StdDev       | Ratio | RatioSD | Gen0       | Gen1     | Gen2     | Allocated    | Alloc Ratio |
 *  |--------------------------------- |------- |----------------:|--------------:|-------------:|------:|--------:|-----------:|---------:|---------:|-------------:|------------:|
 *  | InitializePawnAttacks_Sequential | 64     |        296.3 ns |       5.81 ns |      7.76 ns |  1.00 |    0.00 |     0.0634 |        - |        - |      1.04 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel   | 64     |      2,161.9 ns |      13.18 ns |     12.33 ns |  7.25 |    0.20 |     0.1831 |        - |        - |         3 KB |        2.89 |
 *  | InitializePawnAttacks_Parallel_2 | 64     |      3,768.4 ns |      19.53 ns |     18.27 ns | 12.65 |    0.37 |     0.4120 |   0.0038 |        - |      6.67 KB |        6.42 |
 *  | InitializePawnAttacks_Parallel_3 | 64     |     26,065.9 ns |     498.81 ns |    533.72 ns | 87.54 |    3.37 |     6.6528 |   0.1526 |        - |    107.01 KB |      102.99 |
 *  |                                  |        |                 |               |              |       |         |            |          |          |              |             |
 *  | InitializePawnAttacks_Sequential | 1000   |      4,105.3 ns |      81.59 ns |    145.03 ns |  1.00 |    0.00 |     0.9537 |        - |        - |     15.66 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel   | 1000   |      6,070.8 ns |      82.74 ns |     77.40 ns |  1.44 |    0.04 |     1.1063 |   0.0610 |        - |     17.85 KB |        1.14 |
 *  | InitializePawnAttacks_Parallel_2 | 1000   |     10,765.2 ns |     213.82 ns |    306.66 ns |  2.59 |    0.12 |     1.4038 |   0.0763 |        - |     21.62 KB |        1.38 |
 *  | InitializePawnAttacks_Parallel_3 | 1000   |    275,142.8 ns |   5,060.83 ns |  4,486.29 ns | 65.41 |    2.28 |   101.5625 |   8.3008 |        - |   1635.38 KB |      104.40 |
 *  |                                  |        |                 |               |              |       |         |            |          |          |              |             |
 *  | InitializePawnAttacks_Sequential | 10000  |     95,174.6 ns |   1,598.49 ns |  1,495.23 ns |  1.00 |    0.00 |    49.9268 |  49.9268 |  49.9268 |    156.31 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel   | 10000  |     67,108.6 ns |   1,140.89 ns |    952.69 ns |  0.71 |    0.01 |    49.9268 |  49.9268 |  49.9268 |     158.5 KB |        1.01 |
 *  | InitializePawnAttacks_Parallel_2 | 10000  |     95,176.9 ns |   1,283.88 ns |  1,200.94 ns |  1.00 |    0.03 |    49.9268 |  49.9268 |  49.9268 |     162.4 KB |        1.04 |
 *  | InitializePawnAttacks_Parallel_3 | 10000  |  2,574,036.6 ns |  19,790.50 ns | 18,512.05 ns | 27.05 |    0.48 |  1000.0000 |  46.8750 |  46.8750 |  16330.76 KB |      104.48 |
 *  |                                  |        |                 |               |              |       |         |            |          |          |              |             |
 *  | InitializePawnAttacks_Sequential | 100000 |    894,571.3 ns |  14,315.33 ns | 13,390.57 ns |  1.00 |    0.00 |   499.0234 | 499.0234 | 499.0234 |    1562.7 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel   | 100000 |    592,455.6 ns |   7,184.00 ns |  6,719.92 ns |  0.66 |    0.01 |   499.0234 | 499.0234 | 499.0234 |   1565.03 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel_2 | 100000 |    830,699.5 ns |  15,683.04 ns | 18,060.62 ns |  0.93 |    0.02 |   499.0234 | 499.0234 | 499.0234 |   1568.98 KB |        1.00 |
 *  | InitializePawnAttacks_Parallel_3 | 100000 | 26,380,771.2 ns | 111,378.89 ns | 93,006.42 ns | 29.39 |    0.37 | 10468.7500 | 781.2500 | 500.0000 | 163286.63 KB |      104.49 |
 *
*/

using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class PawnAttacks_MultiDimensionalInitialization_Benchmark : BaseBenchmark
{
    public static IEnumerable<int> Data => [64, 1_000, 10_000, 100_000];

    /// <summary>
    /// Best for data <= 1000 (64 in real life)
    /// </summary>
    /// <param name="data"></param>
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public void InitializePawnAttacks_Sequential(int data)
    {
        BitBoard[,] pawnAttacks = new BitBoard[2, data];

        for (int square = 0; square < data; ++square)
        {
            pawnAttacks[0, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: false);
            pawnAttacks[1, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: true);
        }
    }

    /// <summary>
    /// Only starts to makes sense somewhere between 1_000 < data <= 10_000
    /// </summary>
    /// <param name="data"></param>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void InitializePawnAttacks_Parallel(int data)
    {
        BitBoard[,] pawnAttacks = new BitBoard[2, data];

        Parallel.For(0, data, (square) =>
        {
            pawnAttacks[0, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: false);
            pawnAttacks[1, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: true);
        });
    }

    /// <summary>
    /// ~2x slower than <see cref="InitializePawnAttacks_Parallel"/>
    /// </summary>
    /// <param name="data"></param>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void InitializePawnAttacks_Parallel_2(int data)
    {
        BitBoard[,] pawnAttacks = new BitBoard[2, data];

        Parallel.For(0, 2, (n) =>
            Parallel.For(0, data, (square) =>
                pawnAttacks[n, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: false)));
    }

    /// <summary>
    /// Completely useless (> 50 times slower than anything else)
    /// </summary>
    /// <param name="data"></param>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void InitializePawnAttacks_Parallel_3(int data)
    {
        BitBoard[,] pawnAttacks = new BitBoard[2, data];

        Parallel.For(0, data, (square) =>
            Parallel.For(0, 2, (n) =>
                pawnAttacks[n, square] = AttackGenerator.MaskPawnAttacks(square, isWhite: false)));
    }
}
