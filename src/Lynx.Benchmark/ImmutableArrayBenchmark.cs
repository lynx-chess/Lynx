/*
 *  BenchmarkDotNet v0.14.0, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method             | Size | Mean         | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |------------------- |----- |-------------:|----------:|----------:|------:|----------:|------------:|
 *  | RegularArrayRead   | 1    |     42.85 us |  0.237 us |  0.210 us |  1.00 |         - |          NA |
 *  | ImmutableArrayRead | 1    |     45.43 us |  0.193 us |  0.180 us |  1.06 |         - |          NA |
 *  |                    |      |              |           |           |       |           |             |
 *  | RegularArrayRead   | 10   |    427.73 us |  1.364 us |  1.209 us |  1.00 |         - |          NA |
 *  | ImmutableArrayRead | 10   |    452.97 us |  0.294 us |  0.246 us |  1.06 |         - |          NA |
 *  |                    |      |              |           |           |       |           |             |
 *  | RegularArrayRead   | 100  |  4,272.62 us |  4.661 us |  3.639 us |  1.00 |       6 B |        1.00 |
 *  | ImmutableArrayRead | 100  |  4,541.29 us | 22.068 us | 19.563 us |  1.06 |       6 B |        1.00 |
 *  |                    |      |              |           |           |       |           |             |
 *  | RegularArrayRead   | 1000 | 42,737.13 us | 56.820 us | 44.361 us |  1.00 |      61 B |        1.00 |
 *  | ImmutableArrayRead | 1000 | 45,354.81 us | 88.633 us | 69.199 us |  1.06 |      67 B |        1.10 |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2582) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method             | Size | Mean         | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |------------------- |----- |-------------:|----------:|----------:|------:|----------:|------------:|
 *  | RegularArrayRead   | 1    |     42.72 us |  0.064 us |  0.054 us |  1.00 |         - |          NA |
 *  | ImmutableArrayRead | 1    |     45.20 us |  0.031 us |  0.028 us |  1.06 |         - |          NA |
 *  |                    |      |              |           |           |       |           |             |
 *  | RegularArrayRead   | 10   |    426.98 us |  0.385 us |  0.342 us |  1.00 |         - |          NA |
 *  | ImmutableArrayRead | 10   |    452.45 us |  0.480 us |  0.401 us |  1.06 |         - |          NA |
 *  |                    |      |              |           |           |       |           |             |
 *  | RegularArrayRead   | 100  |  4,272.90 us |  3.572 us |  3.166 us |  1.00 |       3 B |        1.00 |
 *  | ImmutableArrayRead | 100  |  4,523.69 us |  5.562 us |  5.203 us |  1.06 |       3 B |        1.00 |
 *  |                    |      |              |           |           |       |           |             |
 *  | RegularArrayRead   | 1000 | 42,642.88 us | 36.591 us | 28.568 us |  1.00 |      33 B |        1.00 |
 *  | ImmutableArrayRead | 1000 | 45,267.93 us | 78.477 us | 69.568 us |  1.06 |      36 B |        1.09 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sonoma 14.6.1 (23G93) [Darwin 23.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
 *
 *  | Method             | Size | Mean         | Error      | StdDev     | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------------- |----- |-------------:|-----------:|-----------:|------:|--------:|----------:|------------:|
 *  | RegularArrayRead   | 1    |     38.24 us |   0.879 us |   2.563 us |  1.00 |    0.09 |         - |          NA |
 *  | ImmutableArrayRead | 1    |     34.00 us |   0.673 us |   0.826 us |  0.89 |    0.06 |         - |          NA |
 *  |                    |      |              |            |            |       |         |           |             |
 *  | RegularArrayRead   | 10   |    337.15 us |   6.721 us |   6.902 us |  1.00 |    0.03 |         - |          NA |
 *  | ImmutableArrayRead | 10   |    337.40 us |   6.686 us |   7.700 us |  1.00 |    0.03 |         - |          NA |
 *  |                    |      |              |            |            |       |         |           |             |
 *  | RegularArrayRead   | 100  |  3,324.06 us |  42.711 us |  39.952 us |  1.00 |    0.02 |       3 B |        1.00 |
 *  | ImmutableArrayRead | 100  |  3,322.17 us |  17.498 us |  13.661 us |  1.00 |    0.01 |       3 B |        1.00 |
 *  |                    |      |              |            |            |       |         |           |             |
 *  | RegularArrayRead   | 1000 | 33,188.84 us | 542.930 us | 481.294 us |  1.00 |    0.02 |      49 B |        1.00 |
 *  | ImmutableArrayRead | 1000 | 33,308.08 us | 563.796 us | 499.791 us |  1.00 |    0.02 |      46 B |        0.94 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.6.9 (22G830) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.401
 *    [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
 *
 *  | Method             | Size | Mean         | Error      | StdDev     | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |------------------- |----- |-------------:|-----------:|-----------:|------:|--------:|----------:|------------:|
 *  | RegularArrayRead   | 1    |     48.07 us |   0.954 us |   1.671 us |  1.00 |    0.05 |         - |          NA |
 *  | ImmutableArrayRead | 1    |     47.48 us |   1.272 us |   3.526 us |  0.99 |    0.08 |         - |          NA |
 *  |                    |      |              |            |            |       |         |           |             |
 *  | RegularArrayRead   | 10   |    421.04 us |   5.356 us |   4.748 us |  1.00 |    0.02 |         - |          NA |
 *  | ImmutableArrayRead | 10   |    439.20 us |   4.010 us |   3.348 us |  1.04 |    0.01 |         - |          NA |
 *  |                    |      |              |            |            |       |         |           |             |
 *  | RegularArrayRead   | 100  |  4,238.45 us |  77.235 us |  82.641 us |  1.00 |    0.03 |       6 B |        1.00 |
 *  | ImmutableArrayRead | 100  |  4,444.70 us |  85.675 us |  84.144 us |  1.05 |    0.03 |       6 B |        1.00 |
 *  |                    |      |              |            |            |       |         |           |             |
 *  | RegularArrayRead   | 1000 | 42,278.86 us | 582.666 us | 545.026 us |  1.00 |    0.02 |      74 B |        1.00 |
 *  | ImmutableArrayRead | 1000 | 36,701.69 us | 696.642 us | 651.640 us |  0.87 |    0.02 |      74 B |        1.00 |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Collections.Immutable;

using static Lynx.TunableEvalParameters;

namespace Lynx.Benchmark;

public class ImmutableArrayBenchmark : BaseBenchmark
{
    [Params(1, 10, 100, 1_000)]
    public int Size { get; set; }

    public const int PSQTBucketCount = 23;

#pragma warning disable S2386 // Mutable fields should not be "public static"
    public static readonly int[][][][] RegularArray;
#pragma warning restore S2386 // Mutable fields should not be "public static"

    public static readonly ImmutableArray<ImmutableArray<ImmutableArray<ImmutableArray<int>>>> ImmutableArray;

#pragma warning disable S3963 // "static" fields should be initialized inline
    static ImmutableArrayBenchmark()
#pragma warning restore S3963 // "static" fields should be initialized inline
    {
        short[][][][] mgPositionalTables =
        [
            [
                MiddleGamePawnTable,
                MiddleGameKnightTable,
                MiddleGameBishopTable,
                MiddleGameRookTable,
                MiddleGameQueenTable,
                MiddleGameKingTable
            ],
            [
                MiddleGameEnemyPawnTable,
                MiddleGameEnemyKnightTable,
                MiddleGameEnemyBishopTable,
                MiddleGameEnemyRookTable,
                MiddleGameEnemyQueenTable,
                MiddleGameEnemyKingTable
            ]

    ];

        short[][][][] egPositionalTables =
        [
            [
                EndGamePawnTable,
                EndGameKnightTable,
                EndGameBishopTable,
                EndGameRookTable,
                EndGameQueenTable,
                EndGameKingTable
            ],
            [
                EndGameEnemyPawnTable,
                EndGameEnemyKnightTable,
                EndGameEnemyBishopTable,
                EndGameEnemyRookTable,
                EndGameEnemyQueenTable,
                EndGameEnemyKingTable
            ]
        ];

        RegularArray = new int[2][][][];

        for (int friendEnemy = 0; friendEnemy < 2; ++friendEnemy)
        {
            RegularArray[friendEnemy] = new int[PSQTBucketCount][][];

            for (int bucket = 0; bucket < PSQTBucketCount; ++bucket)
            {
                RegularArray[friendEnemy][bucket] = new int[12][];
                for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
                {
                    RegularArray[friendEnemy][bucket][piece] = new int[64];
                    RegularArray[friendEnemy][bucket][piece + 6] = new int[64];

                    for (int sq = 0; sq < 64; ++sq)
                    {
                        RegularArray[friendEnemy][bucket][piece][sq] = Utils.Pack(
                            (short)(MiddleGamePieceValues[friendEnemy][bucket][piece] + mgPositionalTables[friendEnemy][piece][bucket][sq]),
                            (short)(EndGamePieceValues[friendEnemy][bucket][piece] + egPositionalTables[friendEnemy][piece][bucket][sq]));

                        RegularArray[friendEnemy][bucket][piece + 6][sq] = Utils.Pack(
                            (short)(MiddleGamePieceValues[friendEnemy][bucket][piece + 6] - mgPositionalTables[friendEnemy][piece][bucket][sq ^ 56]),
                            (short)(EndGamePieceValues[friendEnemy][bucket][piece + 6] - egPositionalTables[friendEnemy][piece][bucket][sq ^ 56]));
                    }

                    var immutable = RegularArray[friendEnemy][bucket][piece].ToImmutableArray();
                }
            }
        }

        ImmutableArray = RegularArray.Select(arr1 => arr1.Select(arr2 => arr2.Select(arr3 => arr3.ToImmutableArray()).ToImmutableArray()).ToImmutableArray()).ToImmutableArray();
    }

    [Benchmark(Baseline = true)]
    public ulong RegularArrayRead()
    {
        ulong sum = 0;
        for (int i = 0; i < Size; ++i)
        {
            for (int friendEnemy = 0; friendEnemy < 2; ++friendEnemy)
            {
                for (int bucket = 0; bucket < PSQTBucketCount; ++bucket)
                {
                    for (int piece = 0; piece < 12; ++piece)
                    {
                        for (int square = 0; square < 64; ++square)
                        {
                            sum += (ulong)RegularArray[friendEnemy][bucket][piece][square];
                        }
                    }
                }
            }
        }

        return sum;
    }

    [Benchmark]
    public ulong ImmutableArrayRead()
    {
        ulong sum = 0;
        for (int i = 0; i < Size; ++i)
        {
            for (int friendEnemy = 0; friendEnemy < 2; ++friendEnemy)
            {
                for (int bucket = 0; bucket < PSQTBucketCount; ++bucket)
                {
                    for (int piece = 0; piece < 12; ++piece)
                    {
                        for (int square = 0; square < 64; ++square)
                        {
                            sum += (ulong)ImmutableArray[friendEnemy][bucket][piece][square];
                        }
                    }
                }
            }
        }

        return sum;
    }
}
