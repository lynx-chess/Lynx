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
    /// PSQTBucketCount x 12 x 64 x 2
    /// </summary>
    internal static readonly int[] _packedPSQT = GC.AllocateArray<int>(PSQTBucketCount * 12 * 64 * 2, pinned: true);

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

        for (int bucket = 0; bucket < PSQTBucketCount; ++bucket)
        {
            for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
            {
                for (int sq = 0; sq < 64; ++sq)
                {
                    for (int friendEnemy = 0; friendEnemy < 2; ++friendEnemy)
                    {
                        _packedPSQT[PSQTIndex(bucket, piece, sq, friendEnemy)] = Utils.Pack(
                            (short)(MiddleGamePieceValues[friendEnemy][bucket][piece] + mgPositionalTables[friendEnemy][piece][bucket][sq]),
                            (short)(EndGamePieceValues[friendEnemy][bucket][piece] + egPositionalTables[friendEnemy][piece][bucket][sq]));

                        _packedPSQT[PSQTIndex(bucket, piece + 6, sq, friendEnemy)] = Utils.Pack(
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PSQT(int friendEnemy, int bucket, int piece, int square)
    {
        var index = PSQTIndex(bucket, piece, square, friendEnemy);
        Debug.Assert(index >= 0 && index < _packedPSQT.Length);

        return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_packedPSQT), index);
    }

    /// <summary>
    /// [PSQTBucketCount][12][64][2]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PSQTIndex(int bucket, int piece, int square, int friendEnemy)
    {
        const int bucketOffset = 12 * 64 * 2;
        const int pieceOffset = 64 * 2;
        const int squareOffset = 2;

        return (bucket * bucketOffset)
            + (piece * pieceOffset)
            + (square * squareOffset)
            + friendEnemy;
    }
}
