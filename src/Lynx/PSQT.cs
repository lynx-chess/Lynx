using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Lynx.TunableEvalParameters;

namespace Lynx;

public static class EvaluationPSQTs
{
    public const int PSQTBucketCount = 23;

#pragma warning disable S4663 // Comments should not be empty - comes from C++
    public static ReadOnlySpan<int> PSQTBucketLayout =>
    [
        15, 16, 17, 18, 19, 20, 21, 22,
        15, 16, 17, 18, 19, 20, 21, 22,
        15, 16, 17, 18, 19, 20, 21, 22,
        15, 16, 17, 18, 19, 20, 21, 22,
        15, 8, 9, 10, 11, 12, 13, 14,
        15, 8, 9, 10, 11, 12, 13, 14,
        0, 8, 9, 10, 11, 12, 13, 14,
        0, 1, 2, 3, 4, 5, 6, 7, //
    ];
#pragma warning restore S4663 // Comments should not be empty

    /// <summary>
    /// 2 x PSQTBucketCount x 12 x 64
    /// </summary>
    internal static readonly int[] _packedPSQT = GC.AllocateArray<int>(2 * PSQTBucketCount * 12 * 64, pinned: true);

    static EvaluationPSQTs()
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

        for (int friendEnemy = 0; friendEnemy < 2; ++friendEnemy)
        {
            for (int bucket = 0; bucket < PSQTBucketCount; ++bucket)
            {
                for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
                {
                    for (int sq = 0; sq < 64; ++sq)
                    {
                        _packedPSQT[PSQTIndex(friendEnemy, bucket, piece, sq)] = Utils.Pack(
                            (short)(MiddleGamePieceValues[friendEnemy][bucket][piece] + mgPositionalTables[friendEnemy][piece][bucket][sq]),
                            (short)(EndGamePieceValues[friendEnemy][bucket][piece] + egPositionalTables[friendEnemy][piece][bucket][sq]));

                        _packedPSQT[PSQTIndex(friendEnemy, bucket, piece + 6, sq)] = Utils.Pack(
                            (short)(MiddleGamePieceValues[friendEnemy][bucket][piece + 6] - mgPositionalTables[friendEnemy][piece][bucket][sq ^ 56]),
                            (short)(EndGamePieceValues[friendEnemy][bucket][piece + 6] - egPositionalTables[friendEnemy][piece][bucket][sq ^ 56]));
                    }
                }
            }
        }
    }

    /// <summary>
    /// [2][PSQTBucketCount][12][64]
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PSQT(int friendEnemy, int bucket, int piece, int square)
    {
        var index = PSQTIndex(friendEnemy, bucket, piece, square);
        Debug.Assert(index >= 0 && index < _packedPSQT.Length);

        return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_packedPSQT), index);
    }

    /// <summary>
    /// [2][PSQTBucketCount][12][64]
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PSQTIndex(int friendEnemy, int bucket, int piece, int square)
    {
        const int friendEnemyOffset = PSQTBucketCount * 12 * 64;
        const int bucketOffset = 12 * 64;
        const int pieceOffset = 64;

        return (friendEnemy * friendEnemyOffset)
            + (bucket * bucketOffset)
            + (piece * pieceOffset)
            + square;
    }
}

[GeneratedNamespace.GeneratePSQT]
public partial class UserClass
{
    [GeneratedNamespace.GeneratePackedConstant]
    public static readonly TaperedEvaluationTerm Test = 66;

    partial void UserMethod();
}

public partial class UserClass2
{
    //[GeneratedNamespace.GeneratePackedConstant]
    //public int IsolatedPawnPenalty = Utils.Pack(40, 2);

    partial void UserMethod();
}