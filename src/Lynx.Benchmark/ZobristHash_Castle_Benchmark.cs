/*
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                      | Size  | Mean          | Error        | StdDev       | Median        | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------------------------- |------ |--------------:|-------------:|-------------:|--------------:|------:|--------:|----------:|------------:|
 *  | Naive                       | 1     |      24.68 ns |     0.055 ns |     0.052 ns |      24.70 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 1     |      29.08 ns |     0.209 ns |     0.195 ns |      28.97 ns |  1.18 |    0.01 |         - |          NA |
 *  | Switch                      | 1     |      10.86 ns |     0.247 ns |     0.433 ns |      10.91 ns |  0.44 |    0.03 |         - |          NA |
 *  | Switch_Precalculated        | 1     |      11.59 ns |     0.073 ns |     0.061 ns |      11.61 ns |  0.47 |    0.00 |         - |          NA |
 *  | Switch_Ordered              | 1     |      10.94 ns |     0.042 ns |     0.040 ns |      10.93 ns |  0.44 |    0.00 |         - |          NA |
 *  | Switch_NoStaticCalculations | 1     |      19.09 ns |     0.123 ns |     0.103 ns |      19.03 ns |  0.77 |    0.00 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 10    |     251.37 ns |     1.534 ns |     1.360 ns |     251.20 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 10    |     269.87 ns |     1.809 ns |     1.604 ns |     269.20 ns |  1.07 |    0.01 |         - |          NA |
 *  | Switch                      | 10    |     112.35 ns |     0.402 ns |     0.376 ns |     112.49 ns |  0.45 |    0.00 |         - |          NA |
 *  | Switch_Precalculated        | 10    |     111.98 ns |     2.227 ns |     2.187 ns |     112.19 ns |  0.45 |    0.01 |         - |          NA |
 *  | Switch_Ordered              | 10    |     114.96 ns |     0.402 ns |     0.376 ns |     114.76 ns |  0.46 |    0.00 |         - |          NA |
 *  | Switch_NoStaticCalculations | 10    |     174.59 ns |     1.055 ns |     0.987 ns |     174.10 ns |  0.69 |    0.01 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 100   |   2,475.07 ns |    16.205 ns |    15.158 ns |   2,467.75 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 100   |   2,675.68 ns |    15.079 ns |    14.105 ns |   2,671.43 ns |  1.08 |    0.01 |         - |          NA |
 *  | Switch                      | 100   |   1,149.91 ns |     0.913 ns |     0.809 ns |   1,149.97 ns |  0.46 |    0.00 |         - |          NA |
 *  | Switch_Precalculated        | 100   |   1,283.78 ns |     9.838 ns |     8.721 ns |   1,283.32 ns |  0.52 |    0.00 |         - |          NA |
 *  | Switch_Ordered              | 100   |   1,155.97 ns |     7.557 ns |     6.699 ns |   1,156.00 ns |  0.47 |    0.00 |         - |          NA |
 *  | Switch_NoStaticCalculations | 100   |   1,747.19 ns |     9.473 ns |     8.861 ns |   1,741.39 ns |  0.71 |    0.01 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 1000  |  24,649.64 ns |   100.182 ns |    93.710 ns |  24,654.70 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 1000  |  26,588.11 ns |    48.474 ns |    40.478 ns |  26,576.87 ns |  1.08 |    0.00 |         - |          NA |
 *  | Switch                      | 1000  |  11,139.36 ns |     3.007 ns |     2.348 ns |  11,139.41 ns |  0.45 |    0.00 |         - |          NA |
 *  | Switch_Precalculated        | 1000  |  11,176.29 ns |    64.354 ns |    53.739 ns |  11,148.51 ns |  0.45 |    0.00 |         - |          NA |
 *  | Switch_Ordered              | 1000  |  11,174.93 ns |    42.290 ns |    39.558 ns |  11,169.34 ns |  0.45 |    0.00 |         - |          NA |
 *  | Switch_NoStaticCalculations | 1000  |  17,362.08 ns |    82.865 ns |    69.196 ns |  17,339.66 ns |  0.70 |    0.00 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 10000 | 246,627.88 ns | 1,694.619 ns | 1,585.148 ns | 247,470.53 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 10000 | 281,611.63 ns | 1,986.274 ns | 1,857.962 ns | 280,492.32 ns |  1.14 |    0.01 |         - |          NA |
 *  | Switch                      | 10000 | 111,581.61 ns |   349.897 ns |   327.293 ns | 111,391.54 ns |  0.45 |    0.00 |         - |          NA |
 *  | Switch_Precalculated        | 10000 | 111,665.72 ns |   299.743 ns |   280.380 ns | 111,512.24 ns |  0.45 |    0.00 |         - |          NA |
 *  | Switch_Ordered              | 10000 | 113,724.65 ns | 1,983.175 ns | 3,258.414 ns | 111,922.24 ns |  0.46 |    0.02 |         - |          NA |
 *  | Switch_NoStaticCalculations | 10000 | 173,277.99 ns |   262.216 ns |   245.277 ns | 173,209.24 ns |  0.70 |    0.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                      | Size  | Mean          | Error        | StdDev       | Median        | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------------------------- |------ |--------------:|-------------:|-------------:|--------------:|------:|--------:|----------:|------------:|
 *  | Naive                       | 1     |      25.16 ns |     0.255 ns |     0.213 ns |      25.08 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 1     |      29.34 ns |     0.023 ns |     0.020 ns |      29.35 ns |  1.17 |    0.01 |         - |          NA |
 *  | Switch                      | 1     |      11.02 ns |     0.253 ns |     0.401 ns |      11.23 ns |  0.44 |    0.01 |         - |          NA |
 *  | Switch_Precalculated        | 1     |      12.35 ns |     0.279 ns |     0.714 ns |      12.74 ns |  0.50 |    0.02 |         - |          NA |
 *  | Switch_Ordered              | 1     |      11.06 ns |     0.247 ns |     0.346 ns |      11.21 ns |  0.44 |    0.02 |         - |          NA |
 *  | Switch_NoStaticCalculations | 1     |      19.69 ns |     0.214 ns |     0.200 ns |      19.64 ns |  0.78 |    0.01 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 10    |     251.35 ns |     1.081 ns |     0.959 ns |     251.43 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 10    |     282.71 ns |     0.463 ns |     0.410 ns |     282.62 ns |  1.12 |    0.00 |         - |          NA |
 *  | Switch                      | 10    |     110.75 ns |     2.255 ns |     5.657 ns |     113.69 ns |  0.43 |    0.03 |         - |          NA |
 *  | Switch_Precalculated        | 10    |     120.46 ns |     2.645 ns |     7.800 ns |     124.45 ns |  0.48 |    0.03 |         - |          NA |
 *  | Switch_Ordered              | 10    |     108.44 ns |     2.189 ns |     5.245 ns |     111.66 ns |  0.43 |    0.03 |         - |          NA |
 *  | Switch_NoStaticCalculations | 10    |     180.33 ns |     2.099 ns |     1.861 ns |     179.91 ns |  0.72 |    0.01 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 100   |   2,453.86 ns |     5.426 ns |     4.236 ns |   2,455.22 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 100   |   2,829.83 ns |     4.001 ns |     3.341 ns |   2,829.11 ns |  1.15 |    0.00 |         - |          NA |
 *  | Switch                      | 100   |   1,089.23 ns |    21.646 ns |    56.262 ns |   1,117.59 ns |  0.44 |    0.03 |         - |          NA |
 *  | Switch_Precalculated        | 100   |   1,344.27 ns |    28.561 ns |    84.214 ns |   1,396.54 ns |  0.55 |    0.03 |         - |          NA |
 *  | Switch_Ordered              | 100   |   1,118.89 ns |    22.219 ns |    60.823 ns |   1,148.98 ns |  0.46 |    0.02 |         - |          NA |
 *  | Switch_NoStaticCalculations | 100   |   1,758.36 ns |    32.312 ns |    28.644 ns |   1,746.31 ns |  0.72 |    0.01 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 1000  |  24,610.55 ns |   168.177 ns |   157.313 ns |  24,595.37 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 1000  |  28,174.57 ns |    16.681 ns |    13.023 ns |  28,179.33 ns |  1.14 |    0.01 |         - |          NA |
 *  | Switch                      | 1000  |  10,789.15 ns |   213.495 ns |   543.414 ns |  11,123.17 ns |  0.44 |    0.01 |         - |          NA |
 *  | Switch_Precalculated        | 1000  |  11,817.58 ns |   273.753 ns |   807.166 ns |  12,373.93 ns |  0.48 |    0.04 |         - |          NA |
 *  | Switch_Ordered              | 1000  |  11,076.94 ns |   219.922 ns |   571.607 ns |  11,440.60 ns |  0.45 |    0.02 |         - |          NA |
 *  | Switch_NoStaticCalculations | 1000  |  17,319.22 ns |    38.593 ns |    32.227 ns |  17,327.31 ns |  0.70 |    0.00 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 10000 | 244,709.02 ns | 1,109.882 ns |   983.882 ns | 244,579.97 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 10000 | 281,704.22 ns |   168.523 ns |   157.636 ns | 281,758.79 ns |  1.15 |    0.00 |         - |          NA |
 *  | Switch                      | 10000 | 107,173.38 ns | 2,138.195 ns | 5,403.491 ns | 109,713.33 ns |  0.43 |    0.03 |         - |          NA |
 *  | Switch_Precalculated        | 10000 | 118,760.10 ns | 3,213.607 ns | 9,475.394 ns | 123,713.07 ns |  0.49 |    0.04 |         - |          NA |
 *  | Switch_Ordered              | 10000 | 107,422.65 ns | 2,141.170 ns | 5,861.419 ns | 110,851.50 ns |  0.43 |    0.03 |         - |          NA |
 *  | Switch_NoStaticCalculations | 10000 | 179,160.24 ns | 1,335.161 ns | 1,042.406 ns | 179,444.87 ns |  0.73 |    0.00 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                      | Size  | Mean          | Error        | StdDev       | Median        | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------------------------- |------ |--------------:|-------------:|-------------:|--------------:|------:|--------:|----------:|------------:|
 *  | Naive                       | 1     |      29.47 ns |     0.627 ns |     1.030 ns |      29.11 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 1     |      27.06 ns |     0.557 ns |     0.899 ns |      26.81 ns |  0.92 |    0.04 |         - |          NA |
 *  | Switch                      | 1     |      28.81 ns |     0.542 ns |     0.580 ns |      28.67 ns |  0.97 |    0.04 |         - |          NA |
 *  | Switch_Precalculated        | 1     |      34.48 ns |     0.727 ns |     0.778 ns |      34.37 ns |  1.16 |    0.05 |         - |          NA |
 *  | Switch_Ordered              | 1     |      29.06 ns |     0.610 ns |     0.541 ns |      28.86 ns |  0.98 |    0.04 |         - |          NA |
 *  | Switch_NoStaticCalculations | 1     |      42.07 ns |     0.791 ns |     0.739 ns |      41.98 ns |  1.43 |    0.06 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 10    |     292.56 ns |     5.784 ns |     8.109 ns |     291.03 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 10    |     280.23 ns |     6.809 ns |    18.867 ns |     272.90 ns |  0.93 |    0.05 |         - |          NA |
 *  | Switch                      | 10    |     288.54 ns |     2.681 ns |     2.508 ns |     288.17 ns |  0.98 |    0.03 |         - |          NA |
 *  | Switch_Precalculated        | 10    |     345.66 ns |     6.851 ns |    10.042 ns |     342.25 ns |  1.18 |    0.05 |         - |          NA |
 *  | Switch_Ordered              | 10    |     289.63 ns |     3.502 ns |     3.276 ns |     289.29 ns |  0.99 |    0.03 |         - |          NA |
 *  | Switch_NoStaticCalculations | 10    |     422.70 ns |     4.509 ns |     3.997 ns |     423.08 ns |  1.45 |    0.03 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 100   |   2,884.85 ns |    56.912 ns |    50.451 ns |   2,869.42 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 100   |   2,390.88 ns |    47.252 ns |    50.559 ns |   2,392.14 ns |  0.83 |    0.02 |         - |          NA |
 *  | Switch                      | 100   |   2,823.95 ns |    19.397 ns |    18.144 ns |   2,818.42 ns |  0.98 |    0.02 |         - |          NA |
 *  | Switch_Precalculated        | 100   |   3,345.11 ns |    65.682 ns |    61.439 ns |   3,342.60 ns |  1.16 |    0.03 |         - |          NA |
 *  | Switch_Ordered              | 100   |   2,785.94 ns |    22.894 ns |    21.416 ns |   2,783.51 ns |  0.97 |    0.02 |         - |          NA |
 *  | Switch_NoStaticCalculations | 100   |   4,242.60 ns |    63.234 ns |    56.055 ns |   4,233.87 ns |  1.47 |    0.02 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 1000  |  28,798.70 ns |   466.903 ns |   413.897 ns |  28,734.57 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 1000  |  26,239.31 ns |   514.555 ns |   456.140 ns |  26,155.55 ns |  0.91 |    0.02 |         - |          NA |
 *  | Switch                      | 1000  |  28,216.94 ns |   156.290 ns |   138.547 ns |  28,251.69 ns |  0.98 |    0.02 |         - |          NA |
 *  | Switch_Precalculated        | 1000  |  32,983.93 ns |   631.915 ns |   727.715 ns |  32,775.29 ns |  1.15 |    0.03 |         - |          NA |
 *  | Switch_Ordered              | 1000  |  28,159.22 ns |   176.242 ns |   156.234 ns |  28,189.98 ns |  0.98 |    0.02 |         - |          NA |
 *  | Switch_NoStaticCalculations | 1000  |  42,257.86 ns |   465.715 ns |   412.844 ns |  42,135.52 ns |  1.47 |    0.02 |         - |          NA |
 *  |                             |       |               |              |              |               |       |         |           |             |
 *  | Naive                       | 10000 | 287,414.82 ns | 5,510.296 ns | 5,895.955 ns | 286,416.97 ns |  1.00 |    0.00 |         - |          NA |
 *  | Dictionary                  | 10000 | 258,743.02 ns | 4,524.173 ns | 4,010.562 ns | 257,417.35 ns |  0.90 |    0.02 |         - |          NA |
 *  | Switch                      | 10000 | 281,562.13 ns | 2,526.334 ns | 2,363.135 ns | 281,082.79 ns |  0.98 |    0.02 |         - |          NA |
 *  | Switch_Precalculated        | 10000 | 327,535.21 ns | 4,923.327 ns | 4,605.283 ns | 326,621.64 ns |  1.14 |    0.03 |         - |          NA |
 *  | Switch_Ordered              | 10000 | 282,499.61 ns | 2,724.090 ns | 2,548.115 ns | 282,932.90 ns |  0.99 |    0.02 |         - |          NA |
 *  | Switch_NoStaticCalculations | 10000 | 425,623.51 ns | 8,180.638 ns | 7,652.174 ns | 422,796.51 ns |  1.48 |    0.03 |         - |          NA |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;
using static Lynx.Benchmark.LocalZobristTable;

namespace Lynx.Benchmark;

#pragma warning disable IDE1006 // Naming Styles

public class ZobristHash_Castle_Benchmark : BaseBenchmark
{
    [Params(1, 10, 100, 1_000, 10_000)]
    public int Size { get; set; }

    private static readonly Position[] _positions =
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    ];

    [Benchmark(Baseline = true)]
    public long Naive()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += CalculateMethod(position.Castle);
            }
        }

        return count;
    }

    [Benchmark]
    public long Dictionary()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += DictionaryMethod(position.Castle);
            }
        }

        return count;
    }

    [Benchmark]
    public long Switch()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += SwitchMethod(position.Castle);
            }
        }

        return count;
    }

    [Benchmark]
    public long Switch_Precalculated()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += SwitchMethodPrecalculated(position.Castle);
            }
        }

        return count;
    }

    [Benchmark]
    public long Switch_Ordered()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += SwitchMethodOrdered(position.Castle);
            }
        }

        return count;
    }

    [Benchmark]
    public long Switch_NoStaticCalculations()
    {
        long count = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in _positions)
            {
                count += SwitchMethodNoStaticCalculations(position.Castle);
            }
        }

        return count;
    }
}

file static class LocalZobristTable
{
    private static readonly long[,] _table = Initialize();

    private static readonly long WK_Hash = _table[(int)BoardSquare.a8, (int)Piece.p];
    private static readonly long WQ_Hash = _table[(int)BoardSquare.b8, (int)Piece.p];
    private static readonly long BK_Hash = _table[(int)BoardSquare.c8, (int)Piece.p];
    private static readonly long BQ_Hash = _table[(int)BoardSquare.d8, (int)Piece.p];

    private static readonly long WK_Hash_XOR_WQ_Hash = WK_Hash ^ WQ_Hash;
    private static readonly long WK_Hash_XOR_BK_Hash = WK_Hash ^ BK_Hash;
    private static readonly long WK_Hash_XOR_BQ_Hash = WK_Hash ^ BQ_Hash;
    private static readonly long WQ_Hash_XOR_BK_Hash = WQ_Hash ^ BK_Hash;
    private static readonly long WQ_Hash_XOR_BQ_Hash = WQ_Hash ^ BQ_Hash;
    private static readonly long BK_Hash_XOR_BQ_Hash = BK_Hash ^ BQ_Hash;

    private static readonly long WK_Hash_XOR_WQ_Hash_XOR_BK_Hash = WK_Hash ^ WQ_Hash ^ BK_Hash;
    private static readonly long WK_Hash_XOR_WQ_Hash_XOR_BQ_Hash = WK_Hash ^ WQ_Hash ^ BQ_Hash;
    private static readonly long WK_Hash_XOR_BK_Hash_XOR_BQ_Hash = WK_Hash ^ BK_Hash ^ BQ_Hash;
    private static readonly long WQ_Hash_XOR_BK_Hash_XOR_BQ_Hash = WQ_Hash ^ BK_Hash ^ BQ_Hash;

    private static readonly long WK_Hash_XOR_WQ_Hash_XOR_BK_Hash_XOR_BQ_Hash = WK_Hash ^ WQ_Hash ^ BK_Hash ^ BQ_Hash;

    private static readonly Dictionary<byte, long> _castleHashDictionary = new()
    {
        [0] = 0,                                // -    | -
        [(byte)CastlingRights.WK] = WK_Hash,    // K    | -
        [(byte)CastlingRights.WQ] = WQ_Hash,    // Q    | -
        [(byte)CastlingRights.BK] = BK_Hash,    // -    | k
        [(byte)CastlingRights.BQ] = BQ_Hash,    // -    | q

        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ] = WK_Hash ^ WQ_Hash,    // KQ   | -
        [(byte)CastlingRights.WK | (byte)CastlingRights.BK] = WK_Hash ^ BK_Hash,    // K    | k
        [(byte)CastlingRights.WK | (byte)CastlingRights.BQ] = WK_Hash ^ BQ_Hash,    // K    | q
        [(byte)CastlingRights.WQ | (byte)CastlingRights.BK] = WQ_Hash ^ BK_Hash,    // Q    | k
        [(byte)CastlingRights.WQ | (byte)CastlingRights.BQ] = WQ_Hash ^ BQ_Hash,    // Q    | q
        [(byte)CastlingRights.BK | (byte)CastlingRights.BQ] = BK_Hash ^ BQ_Hash,    // -    | kq

        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK] = WK_Hash ^ WQ_Hash ^ BK_Hash,    // KQ   | k
        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ] = WK_Hash ^ WQ_Hash ^ BQ_Hash,    // KQ   | q
        [(byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ] = WK_Hash ^ BK_Hash ^ BQ_Hash,    // K    | kq
        [(byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ] = WQ_Hash ^ BK_Hash ^ BQ_Hash,    // Q    | kq

        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ] =       // KQ   | kq
            WK_Hash ^ WQ_Hash ^ BK_Hash ^ BQ_Hash
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long CalculateMethod(byte castle)
    {
        long combinedHash = 0;

        if ((castle & (int)CastlingRights.WK) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.a8, (int)Piece.p];        // a8
        }

        if ((castle & (int)CastlingRights.WQ) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.b8, (int)Piece.p];        // b8
        }

        if ((castle & (int)CastlingRights.BK) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.c8, (int)Piece.p];        // c8
        }

        if ((castle & (int)CastlingRights.BQ) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.d8, (int)Piece.p];        // d8
        }

        return combinedHash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DictionaryMethod(byte castle) => _castleHashDictionary[castle];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SwitchMethod(byte castle)
    {
        return castle switch
        {
            0 => 0,                                // -    | -

            (byte)CastlingRights.WK => WK_Hash,    // K    | -
            (byte)CastlingRights.WQ => WQ_Hash,    // Q    | -
            (byte)CastlingRights.BK => BK_Hash,    // -    | k
            (byte)CastlingRights.BQ => BQ_Hash,    // -    | q

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ => WK_Hash ^ WQ_Hash,    // KQ   | -
            (byte)CastlingRights.WK | (byte)CastlingRights.BK => WK_Hash ^ BK_Hash,    // K    | k
            (byte)CastlingRights.WK | (byte)CastlingRights.BQ => WK_Hash ^ BQ_Hash,    // K    | q
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WQ_Hash ^ BK_Hash,    // Q    | k
            (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WQ_Hash ^ BQ_Hash,    // Q    | q
            (byte)CastlingRights.BK | (byte)CastlingRights.BQ => BK_Hash ^ BQ_Hash,    // -    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WK_Hash ^ WQ_Hash ^ BK_Hash,    // KQ   | k
            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WK_Hash ^ WQ_Hash ^ BQ_Hash,    // KQ   | q
            (byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WK_Hash ^ BK_Hash ^ BQ_Hash,    // K    | kq
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WQ_Hash ^ BK_Hash ^ BQ_Hash,    // Q    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ =>       // KQ   | kq
                WK_Hash ^ WQ_Hash ^ BK_Hash ^ BQ_Hash,

            _ => new()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SwitchMethodNoStaticCalculations(byte castle)
    {
#pragma warning disable S1117 // Local variables should not shadow class fields or properties
        var WQ_Hash = _table[(int)BoardSquare.b8, (int)Piece.p];
        var BK_Hash = _table[(int)BoardSquare.c8, (int)Piece.p];
        var BQ_Hash = _table[(int)BoardSquare.d8, (int)Piece.p];
        var WK_Hash = _table[(int)BoardSquare.a8, (int)Piece.p];
#pragma warning restore S1117 // Local variables should not shadow class fields or properties

        return castle switch
        {
            0 => 0,                                // -    | -

            (byte)CastlingRights.WK => WK_Hash,    // K    | -
            (byte)CastlingRights.WQ => WQ_Hash,    // Q    | -
            (byte)CastlingRights.BK => BK_Hash,    // -    | k
            (byte)CastlingRights.BQ => BQ_Hash,    // -    | q

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ => WK_Hash ^ WQ_Hash,    // KQ   | -
            (byte)CastlingRights.WK | (byte)CastlingRights.BK => WK_Hash ^ BK_Hash,    // K    | k
            (byte)CastlingRights.WK | (byte)CastlingRights.BQ => WK_Hash ^ BQ_Hash,    // K    | q
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WQ_Hash ^ BK_Hash,    // Q    | k
            (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WQ_Hash ^ BQ_Hash,    // Q    | q
            (byte)CastlingRights.BK | (byte)CastlingRights.BQ => BK_Hash ^ BQ_Hash,    // -    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WK_Hash ^ WQ_Hash ^ BK_Hash,    // KQ   | k
            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WK_Hash ^ WQ_Hash ^ BQ_Hash,    // KQ   | q
            (byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WK_Hash ^ BK_Hash ^ BQ_Hash,    // K    | kq
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WQ_Hash ^ BK_Hash ^ BQ_Hash,    // Q    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ =>       // KQ   | kq
                WK_Hash ^ WQ_Hash ^ BK_Hash ^ BQ_Hash,

            _ => new()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SwitchMethodOrdered(byte castle)
    {
        return castle switch
        {
            0 => 0,                                // -    | -

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ =>       // KQ   | kq
                WK_Hash ^ WQ_Hash ^ BK_Hash ^ BQ_Hash,
            (byte)CastlingRights.WK | (byte)CastlingRights.WQ => WK_Hash ^ WQ_Hash,    // KQ   | -
            (byte)CastlingRights.BK | (byte)CastlingRights.BQ => BK_Hash ^ BQ_Hash,    // -    | kq
            (byte)CastlingRights.WK | (byte)CastlingRights.BK => WK_Hash ^ BK_Hash,    // K    | k
            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WK_Hash ^ WQ_Hash ^ BK_Hash,    // KQ   | k
            (byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WK_Hash ^ BK_Hash ^ BQ_Hash,    // K    | kq
            (byte)CastlingRights.WK => WK_Hash,    // K    | -
            (byte)CastlingRights.BK => BK_Hash,    // -    | k
            (byte)CastlingRights.WQ => WQ_Hash,    // Q    | -
            (byte)CastlingRights.BQ => BQ_Hash,    // -    | q

            (byte)CastlingRights.WK | (byte)CastlingRights.BQ => WK_Hash ^ BQ_Hash,    // K    | q
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WQ_Hash ^ BK_Hash,    // Q    | k
            (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WQ_Hash ^ BQ_Hash,    // Q    | q

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WK_Hash ^ WQ_Hash ^ BQ_Hash,    // KQ   | q
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WQ_Hash ^ BK_Hash ^ BQ_Hash,    // Q    | kq

            _ => new()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SwitchMethodPrecalculated(byte castle)
    {
        return castle switch
        {
            0 => 0,                                                                                                             // -    | -

            (byte)CastlingRights.WK => WK_Hash,                                                                                 // K    | -
            (byte)CastlingRights.WQ => WQ_Hash,                                                                                 // Q    | -
            (byte)CastlingRights.BK => BK_Hash,                                                                                 // -    | k
            (byte)CastlingRights.BQ => BQ_Hash,                                                                                 // -    | q

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ => WK_Hash_XOR_WQ_Hash,                                           // KQ   | -
            (byte)CastlingRights.WK | (byte)CastlingRights.BK => WK_Hash_XOR_BK_Hash,                                           // K    | k
            (byte)CastlingRights.WK | (byte)CastlingRights.BQ => WK_Hash_XOR_BQ_Hash,                                           // K    | q
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WQ_Hash_XOR_BK_Hash,                                           // Q    | k
            (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WQ_Hash_XOR_BQ_Hash,                                           // Q    | q
            (byte)CastlingRights.BK | (byte)CastlingRights.BQ => BK_Hash_XOR_BQ_Hash,                                           // -    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK => WK_Hash_XOR_WQ_Hash_XOR_BK_Hash,     // KQ   | k
            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => WK_Hash_XOR_WQ_Hash_XOR_BQ_Hash,     // KQ   | q
            (byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WK_Hash_XOR_BK_Hash_XOR_BQ_Hash,     // K    | kq
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => WQ_Hash_XOR_BK_Hash_XOR_BQ_Hash,     // Q    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ =>            // KQ   | kq
                WK_Hash_XOR_WQ_Hash_XOR_BK_Hash_XOR_BQ_Hash,

#pragma warning disable S112 // General or reserved exceptions should never be thrown
            _ => throw new($"Unexpected castle encoded number: {castle}")
#pragma warning restore S112 // General or reserved exceptions should never be thrown
        };
    }

    /// <summary>
    /// Initializes Zobrist table (long[64, 12])
    /// </summary>
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

#pragma warning restore IDE1006 // Naming Styles
