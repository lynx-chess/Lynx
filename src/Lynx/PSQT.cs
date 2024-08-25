using Lynx.Model;
using static Lynx.TunableEvalParameters;

namespace Lynx;

public static class PSQT
{
    public const int PSQTBucketCount = 23;

    public static readonly int[] PSQTBucketLayout =
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

    /// <summary>
    /// 2 x PSQTBucketCount x 12 x 64
    /// </summary>
    public static readonly int[][][][] PackedPSQT = new int[2][][][];

    static PSQT()
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
            PackedPSQT[friendEnemy] = new int[PSQTBucketCount][][];

            for (int bucket = 0; bucket < PSQTBucketCount; ++bucket)
            {
                PackedPSQT[friendEnemy][bucket] = new int[12][];
                for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
                {
                    PackedPSQT[friendEnemy][bucket][piece] = new int[64];
                    PackedPSQT[friendEnemy][bucket][piece + 6] = new int[64];

                    for (int sq = 0; sq < 64; ++sq)
                    {
                        PackedPSQT[friendEnemy][bucket][piece][sq] = Utils.Pack(
                            (short)(MiddleGamePieceValues[friendEnemy][bucket][piece] + mgPositionalTables[friendEnemy][piece][bucket][sq]),
                            (short)(EndGamePieceValues[friendEnemy][bucket][piece] + egPositionalTables[friendEnemy][piece][bucket][sq]));

                        PackedPSQT[friendEnemy][bucket][piece + 6][sq] = Utils.Pack(
                            (short)(MiddleGamePieceValues[friendEnemy][bucket][piece + 6] - mgPositionalTables[friendEnemy][piece][bucket][sq ^ 56]),
                            (short)(EndGamePieceValues[friendEnemy][bucket][piece + 6] - egPositionalTables[friendEnemy][piece][bucket][sq ^ 56]));
                    }
                }
            }
        }
    }
}
