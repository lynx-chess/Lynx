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

    public static readonly int[][][][] RegularArray;

    public static readonly ImmutableArray<ImmutableArray<ImmutableArray<ImmutableArray<int>>>> ImmutableArray;

    static ImmutableArrayBenchmark()
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
