/*
*
*  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
*  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
*  .NET SDK 8.0.100
*    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
*    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
*
*  | Method               | Size  | Mean           | Error         | StdDev        | Ratio | RatioSD | Allocated | Alloc Ratio |
*  |--------------------- |------ |---------------:|--------------:|--------------:|------:|--------:|----------:|------------:|
*  | Naive                | 1     |      24.720 ns |     0.1152 ns |     0.1021 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 1     |      29.323 ns |     0.1852 ns |     0.1732 ns |  1.19 |    0.01 |         - |          NA |
*  | Switch               | 1     |      10.945 ns |     0.0424 ns |     0.0396 ns |  0.44 |    0.00 |         - |          NA |
*  | Switch_Precalculated | 1     |       8.767 ns |     0.0461 ns |     0.0431 ns |  0.35 |    0.00 |         - |          NA |
*  |                      |       |                |               |               |       |         |           |             |
*  | Naive                | 10    |     250.591 ns |     1.5384 ns |     1.4391 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 10    |     267.163 ns |     0.4338 ns |     0.3846 ns |  1.07 |    0.01 |         - |          NA |
*  | Switch               | 10    |     111.927 ns |     0.3434 ns |     0.2868 ns |  0.45 |    0.00 |         - |          NA |
*  | Switch_Precalculated | 10    |      83.823 ns |     0.2308 ns |     0.2159 ns |  0.33 |    0.00 |         - |          NA |
*  |                      |       |                |               |               |       |         |           |             |
*  | Naive                | 100   |   2,445.000 ns |     8.1811 ns |     6.8316 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 100   |   2,804.201 ns |     9.7132 ns |     9.0857 ns |  1.15 |    0.01 |         - |          NA |
*  | Switch               | 100   |   1,150.124 ns |     0.5981 ns |     0.5594 ns |  0.47 |    0.00 |         - |          NA |
*  | Switch_Precalculated | 100   |     844.910 ns |     2.1514 ns |     2.0124 ns |  0.35 |    0.00 |         - |          NA |
*  |                      |       |                |               |               |       |         |           |             |
*  | Naive                | 1000  |  24,657.100 ns |   100.4669 ns |    93.9768 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 1000  |  26,637.086 ns |    97.0745 ns |    90.8036 ns |  1.08 |    0.01 |         - |          NA |
*  | Switch               | 1000  |  11,114.282 ns |   217.0870 ns |   324.9255 ns |  0.45 |    0.02 |         - |          NA |
*  | Switch_Precalculated | 1000  |   8,388.205 ns |    38.5408 ns |    36.0511 ns |  0.34 |    0.00 |         - |          NA |
*  |                      |       |                |               |               |       |         |           |             |
*  | Naive                | 10000 | 244,306.105 ns | 1,034.7181 ns |   917.2506 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 10000 | 278,922.144 ns |   237.0023 ns |   210.0964 ns |  1.14 |    0.00 |         - |          NA |
*  | Switch               | 10000 | 111,157.571 ns | 2,066.9065 ns | 1,933.3856 ns |  0.45 |    0.01 |         - |          NA |
*  | Switch_Precalculated | 10000 |  83,847.896 ns |   423.4214 ns |   396.0686 ns |  0.34 |    0.00 |         - |          NA |
*
*
*  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
*  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
*  .NET SDK 8.0.100
*    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
*    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
*
*  | Method               | Size  | Mean           | Error         | StdDev        | Median         | Ratio | RatioSD | Allocated | Alloc Ratio |
*  |--------------------- |------ |---------------:|--------------:|--------------:|---------------:|------:|--------:|----------:|------------:|
*  | Naive                | 1     |      25.039 ns |     0.0581 ns |     0.0454 ns |      25.035 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 1     |      29.354 ns |     0.0116 ns |     0.0090 ns |      29.354 ns |  1.17 |    0.00 |         - |          NA |
*  | Switch               | 1     |      10.844 ns |     0.2016 ns |     0.1683 ns |      10.911 ns |  0.43 |    0.01 |         - |          NA |
*  | Switch_Precalculated | 1     |       8.787 ns |     0.0374 ns |     0.0350 ns |       8.774 ns |  0.35 |    0.00 |         - |          NA |
*  |                      |       |                |               |               |                |       |         |           |             |
*  | Naive                | 10    |     253.652 ns |     0.4784 ns |     0.4475 ns |     253.546 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 10    |     284.040 ns |     0.4747 ns |     0.4440 ns |     283.972 ns |  1.12 |    0.00 |         - |          NA |
*  | Switch               | 10    |     108.695 ns |     2.2012 ns |     4.7382 ns |     111.581 ns |  0.43 |    0.02 |         - |          NA |
*  | Switch_Precalculated | 10    |      84.387 ns |     0.5648 ns |     0.5007 ns |      84.393 ns |  0.33 |    0.00 |         - |          NA |
*  |                      |       |                |               |               |                |       |         |           |             |
*  | Naive                | 100   |   2,457.589 ns |     7.3332 ns |     6.5006 ns |   2,456.675 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 100   |   2,827.262 ns |     2.1649 ns |     1.8078 ns |   2,826.887 ns |  1.15 |    0.00 |         - |          NA |
*  | Switch               | 100   |   1,076.499 ns |    23.1672 ns |    68.3091 ns |   1,117.677 ns |  0.45 |    0.02 |         - |          NA |
*  | Switch_Precalculated | 100   |     850.107 ns |     2.2053 ns |     2.0628 ns |     850.173 ns |  0.35 |    0.00 |         - |          NA |
*  |                      |       |                |               |               |                |       |         |           |             |
*  | Naive                | 1000  |  24,523.943 ns |    65.6137 ns |    61.3751 ns |  24,522.876 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 1000  |  28,223.102 ns |    26.7723 ns |    23.7330 ns |  28,220.316 ns |  1.15 |    0.00 |         - |          NA |
*  | Switch               | 1000  |  11,115.422 ns |   219.5332 ns |   517.4655 ns |  11,445.611 ns |  0.46 |    0.02 |         - |          NA |
*  | Switch_Precalculated | 1000  |   8,435.975 ns |    54.8240 ns |    51.2824 ns |   8,430.962 ns |  0.34 |    0.00 |         - |          NA |
*  |                      |       |                |               |               |                |       |         |           |             |
*  | Naive                | 10000 | 246,858.210 ns |   624.2585 ns |   553.3889 ns | 246,893.701 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 10000 | 282,037.061 ns |   189.9960 ns |   148.3364 ns | 282,066.748 ns |  1.14 |    0.00 |         - |          NA |
*  | Switch               | 10000 | 106,492.667 ns | 2,610.3488 ns | 7,696.6722 ns | 111,328.833 ns |  0.44 |    0.02 |         - |          NA |
*  | Switch_Precalculated | 10000 |  84,086.209 ns |   226.2048 ns |   211.5921 ns |  84,040.710 ns |  0.34 |    0.00 |         - |          NA |
*
*
*  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
*  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
*  .NET SDK 8.0.100
*    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
*    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
*
*  | Method               | Size  | Mean           | Error         | StdDev         | Median         | Ratio | RatioSD | Allocated | Alloc Ratio |
*  |--------------------- |------ |---------------:|--------------:|---------------:|---------------:|------:|--------:|----------:|------------:|
*  | Naive                | 1     |      38.184 ns |     0.6108 ns |      0.5415 ns |      38.303 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 1     |      33.928 ns |     0.5382 ns |      0.4771 ns |      34.043 ns |  0.89 |    0.02 |         - |          NA |
*  | Switch               | 1     |      40.382 ns |     0.8582 ns |      0.8027 ns |      40.145 ns |  1.06 |    0.03 |         - |          NA |
*  | Switch_Precalculated | 1     |       9.232 ns |     0.2198 ns |      0.1836 ns |       9.153 ns |  0.24 |    0.01 |         - |          NA |
*  |                      |       |                |               |                |                |       |         |           |             |
*  | Naive                | 10    |     361.811 ns |    10.2376 ns |     30.0251 ns |     371.738 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 10    |     340.772 ns |    13.2154 ns |     38.7586 ns |     347.311 ns |  0.95 |    0.15 |         - |          NA |
*  | Switch               | 10    |     378.646 ns |     7.4876 ns |     14.0635 ns |     379.538 ns |  1.11 |    0.13 |         - |          NA |
*  | Switch_Precalculated | 10    |      89.707 ns |     1.3888 ns |      1.8058 ns |      89.878 ns |  0.29 |    0.02 |         - |          NA |
*  |                      |       |                |               |                |                |       |         |           |             |
*  | Naive                | 100   |   3,507.128 ns |   122.1937 ns |    360.2908 ns |   3,610.560 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 100   |   3,162.251 ns |    63.1922 ns |    148.9515 ns |   3,114.441 ns |  0.94 |    0.10 |         - |          NA |
*  | Switch               | 100   |   3,019.342 ns |   113.1666 ns |    315.4631 ns |   2,870.732 ns |  0.88 |    0.10 |         - |          NA |
*  | Switch_Precalculated | 100   |     713.917 ns |    14.0836 ns |     20.1983 ns |     710.728 ns |  0.19 |    0.01 |         - |          NA |
*  |                      |       |                |               |                |                |       |         |           |             |
*  | Naive                | 1000  |  29,303.931 ns |   582.5555 ns |    623.3279 ns |  29,296.593 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 1000  |  25,915.442 ns |   594.2885 ns |  1,752.2730 ns |  25,621.321 ns |  0.86 |    0.06 |         - |          NA |
*  | Switch               | 1000  |  29,887.694 ns |   595.4858 ns |  1,493.9574 ns |  29,332.834 ns |  1.05 |    0.04 |         - |          NA |
*  | Switch_Precalculated | 1000  |   7,245.023 ns |   144.2683 ns |    187.5895 ns |   7,212.165 ns |  0.25 |    0.01 |         - |          NA |
*  |                      |       |                |               |                |                |       |         |           |             |
*  | Naive                | 10000 | 287,375.284 ns | 5,580.5670 ns |  4,947.0273 ns | 287,207.969 ns |  1.00 |    0.00 |         - |          NA |
*  | Dictionary           | 10000 | 244,038.977 ns | 4,804.6802 ns |  8,663.8294 ns | 242,586.133 ns |  0.85 |    0.04 |         - |          NA |
*  | Switch               | 10000 | 312,948.536 ns | 6,227.9810 ns | 11,999.2050 ns | 313,618.784 ns |  1.10 |    0.05 |         - |          NA |
*  | Switch_Precalculated | 10000 |  83,442.668 ns | 1,658.0538 ns |  3,639.4683 ns |  82,906.254 ns |  0.29 |    0.02 |         - |          NA |
*
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;
using static Lynx.Benchmark.LocalZobristTable;

namespace Lynx.Benchmark;
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

            _ => throw new($"Unexpected castle encoded number: {castle}")
        };
    }

    /// <summary>
    /// Initializes Zobrist table (long[64, 12])
    /// </summary>
    /// <returns></returns>
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