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
    /// 2 x PSQTBucketCount x PSQTBucketCount x 12 x 64
    /// </summary>
    internal static readonly int[] _packedPSQT = GC.AllocateArray<int>(2 * PSQTBucketCount * PSQTBucketCount * 12 * 64, pinned: true);

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

        for (int friendBucket = 0; friendBucket < PSQTBucketCount; ++friendBucket)
        {
            for (int enemyBucket = 0; enemyBucket < PSQTBucketCount; ++enemyBucket)
            {
                for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
                {
                    for (int sq = 0; sq < 64; ++sq)
                    {
                        const int Friend = 0;
                        const int Enemy = 1;

                        _packedPSQT[PSQTIndex(friendBucket, enemyBucket, piece, sq)] =
                            Utils.Pack(
                                (short)(MiddleGamePieceValues[Friend][friendBucket][piece] + mgPositionalTables[Friend][piece][friendBucket][sq]),
                                (short)(EndGamePieceValues[Friend][friendBucket][piece] + egPositionalTables[Friend][piece][friendBucket][sq]))
                            + Utils.Pack(
                                (short)(MiddleGamePieceValues[Enemy][enemyBucket][piece] + mgPositionalTables[Enemy][piece][enemyBucket][sq]),
                                (short)(EndGamePieceValues[Enemy][enemyBucket][piece] + egPositionalTables[Enemy][piece][enemyBucket][sq]));

                        _packedPSQT[PSQTIndex(friendBucket, enemyBucket, piece + 6, sq)] =
                            Utils.Pack(
                                (short)(MiddleGamePieceValues[Friend][friendBucket][piece + 6] - mgPositionalTables[Friend][piece][friendBucket][sq ^ 56]),
                                (short)(EndGamePieceValues[Friend][friendBucket][piece + 6] - egPositionalTables[Friend][piece][friendBucket][sq ^ 56]))
                            + Utils.Pack(
                                (short)(MiddleGamePieceValues[Enemy][enemyBucket][piece + 6] - mgPositionalTables[Enemy][piece][enemyBucket][sq ^ 56]),
                                (short)(EndGamePieceValues[Enemy][enemyBucket][piece + 6] - egPositionalTables[Enemy][piece][enemyBucket][sq ^ 56]));
                    }
                }
            }
        }
    }

    /// <summary>
    /// [2][PSQTBucketCount][12][64]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PSQT(int friendBucket, int enemyBucket, int piece, int square)
    {
        var index = PSQTIndex(friendBucket, enemyBucket, piece, square);
        Debug.Assert(index >= 0 && index < _packedPSQT.Length);

        return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_packedPSQT), index);
    }

    /// <summary>
    /// [PSQTBucketCount][PSQTBucketCount][12][64]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PSQTIndex(int friendBucket, int enemyBucket, int piece, int square)
    {
        const int friendBucketOffset = PSQTBucketCount * 12 * 64;
        const int enemyBucketOffset = 12 * 64;
        const int pieceOffset = 64;

        return (friendBucket * friendBucketOffset)
            + (enemyBucket * enemyBucketOffset)
            + (piece * pieceOffset)
            + square;
    }
}
