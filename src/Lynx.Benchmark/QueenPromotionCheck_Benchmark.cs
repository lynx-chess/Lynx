/*
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.87GHz), 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *  
 *  | Method           | Size   | Mean         | Error       | StdDev      | Ratio | Allocated | Alloc Ratio |
 *  |----------------- |------- |-------------:|------------:|------------:|------:|----------:|------------:|
 *  | Modulo           | 1000   |   1,466.7 ns |     1.47 ns |     1.30 ns |  1.00 |         - |          NA |
 *  | DirectComparison | 1000   |     747.5 ns |     0.87 ns |     0.77 ns |  0.51 |         - |          NA |
 *  |                  |        |              |             |             |       |           |             |
 *  | Modulo           | 10000  |  14,685.3 ns |     9.90 ns |     8.27 ns |  1.00 |         - |          NA |
 *  | DirectComparison | 10000  |   7,498.5 ns |     6.43 ns |     5.70 ns |  0.51 |         - |          NA |
 *  |                  |        |              |             |             |       |           |             |
 *  | Modulo           | 100000 | 246,935.8 ns |   943.37 ns |   787.76 ns |  1.00 |         - |          NA |
 *  | DirectComparison | 100000 | 173,039.6 ns | 1,201.83 ns | 1,180.35 ns |  0.70 |         - |          NA |
 *  
 * 
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32370/24H2/2024Update/HudsonValley) (Hyper-V)
 *  Intel Xeon Platinum 8370C CPU 2.80GHz (Max: 2.79GHz), 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v4
 *  
 *  | Method           | Size   | Mean         | Error       | StdDev      | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------------- |------- |-------------:|------------:|------------:|------:|--------:|----------:|------------:|
 *  | Modulo           | 1000   |   1,523.7 ns |     5.02 ns |     4.45 ns |  1.00 |    0.00 |         - |          NA |
 *  | DirectComparison | 1000   |     752.8 ns |     0.81 ns |     0.75 ns |  0.49 |    0.00 |         - |          NA |
 *  |                  |        |              |             |             |       |         |           |             |
 *  | Modulo           | 10000  |  14,950.3 ns |    21.68 ns |    19.22 ns |  1.00 |    0.00 |         - |          NA |
 *  | DirectComparison | 10000  |   7,619.7 ns |    25.54 ns |    23.89 ns |  0.51 |    0.00 |         - |          NA |
 *  |                  |        |              |             |             |       |         |           |             |
 *  | Modulo           | 100000 | 244,691.2 ns | 4,837.49 ns | 6,457.91 ns |  1.00 |    0.04 |         - |          NA |
 *  | DirectComparison | 100000 | 171,289.0 ns | 1,375.95 ns | 1,219.75 ns |  0.70 |    0.02 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *  
 *  | Method           | Size   | Mean         | Error       | StdDev      | Median       | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------------- |------- |-------------:|------------:|------------:|-------------:|------:|--------:|----------:|------------:|
 *  | Modulo           | 1000   |   1,200.4 ns |    61.52 ns |   181.39 ns |   1,146.7 ns |  1.02 |    0.21 |         - |          NA |
 *  | DirectComparison | 1000   |     521.5 ns |    13.13 ns |    37.03 ns |     510.4 ns |  0.44 |    0.07 |         - |          NA |
 *  |                  |        |              |             |             |              |       |         |           |             |
 *  | Modulo           | 10000  |  10,882.5 ns |   427.69 ns | 1,233.99 ns |  10,427.8 ns |  1.01 |    0.15 |         - |          NA |
 *  | DirectComparison | 10000  |   6,128.6 ns |   174.26 ns |   485.75 ns |   6,086.9 ns |  0.57 |    0.07 |         - |          NA |
 *  |                  |        |              |             |             |              |       |         |           |             |
 *  | Modulo           | 100000 | 159,687.7 ns | 3,164.07 ns | 7,269.98 ns | 159,640.0 ns |  1.00 |    0.06 |         - |          NA |
 *  | DirectComparison | 100000 | 129,524.6 ns | 2,578.74 ns | 3,698.35 ns | 129,092.2 ns |  0.81 |    0.04 |         - |          NA |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *  
 *  | Method           | Size   | Mean         | Error       | StdDev     | Median       | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |----------------- |------- |-------------:|------------:|-----------:|-------------:|------:|--------:|----------:|------------:|
 *  | Modulo           | 1000   |   2,168.5 ns |   123.38 ns |   354.0 ns |   1,959.1 ns |  1.02 |    0.22 |         - |          NA |
 *  | DirectComparison | 1000   |     859.8 ns |    62.50 ns |   181.3 ns |     807.9 ns |  0.41 |    0.10 |         - |          NA |
 *  |                  |        |              |             |            |              |       |         |           |             |
 *  | Modulo           | 10000  |  27,865.3 ns | 1,070.72 ns | 3,002.4 ns |  26,934.8 ns |  1.01 |    0.15 |         - |          NA |
 *  | DirectComparison | 10000  |  11,477.7 ns |   227.79 ns |   575.7 ns |  11,502.3 ns |  0.42 |    0.05 |         - |          NA |
 *  |                  |        |              |             |            |              |       |         |           |             |
 *  | Modulo           | 100000 | 314,643.3 ns | 6,276.03 ns | 7,707.5 ns | 313,417.2 ns |  1.00 |    0.03 |         - |          NA |
 *  | DirectComparison | 100000 | 166,966.5 ns | 3,337.01 ns | 8,371.9 ns | 166,053.4 ns |  0.53 |    0.03 |         - |          NA |
 * 
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class QueenPromotionCheck_Benchmark : BaseBenchmark
{
    /// <summary>
    /// Simulated promotedPiece values extracted from moves.
    /// Realistic distribution: ~90% non-promotion (0), rest split among promotion pieces.
    /// </summary>
    private int[] _promotedPieces = null!;

    [Params(1_000, 10_000, 100_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Realistic distribution: most moves are not promotions
        // P=0 N=1 B=2 R=3 Q=4 K=5 p=6 n=7 b=8 r=9 q=10 k=11
        // promotedPiece == 0 means no promotion
        // Valid promotion pieces: N(1), B(2), R(3), Q(4), n(7), b(8), r(9), q(10)
        var rng = new Random(42);
        _promotedPieces = new int[Size];

        for (int i = 0; i < Size; ++i)
        {
            var roll = rng.Next(100);
            _promotedPieces[i] = roll switch
            {
                < 90 => 0,                     // 90% no promotion
                < 93 => (int)Piece.Q,           // 3% white queen promotion
                < 96 => (int)Piece.q,           // 3% black queen promotion
                < 97 => (int)Piece.N,           // 1% white knight promotion
                < 98 => (int)Piece.n,           // 1% black knight promotion
                < 99 => (int)Piece.R,           // 0.5% white rook promotion
                _ => (int)Piece.B,              // 0.5% white bishop promotion
            };
        }
    }

    [Benchmark(Baseline = true)]
    public int Modulo()
    {
        int count = 0;
        var data = _promotedPieces;

        for (int i = 0; i < data.Length; ++i)
        {
            var promotedPiece = data[i];
            var isPromotion = promotedPiece != default;

            if ((promotedPiece + 2) % 6 == 0)
            {
                ++count;
            }

            if (!isPromotion)
            {
                ++count;
            }
        }

        return count;
    }

    [Benchmark]
    public int DirectComparison()
    {
        int count = 0;
        var data = _promotedPieces;

        for (int i = 0; i < data.Length; ++i)
        {
            var promotedPiece = data[i];
            var isPromotion = promotedPiece != default;

            if (isPromotion && (promotedPiece == (int)Piece.Q || promotedPiece == (int)Piece.q))
            {
                ++count;
            }

            if (!isPromotion)
            {
                ++count;
            }
        }

        return count;
    }
}
