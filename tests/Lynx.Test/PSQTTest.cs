using Lynx.Model;
using NUnit.Framework;

using static Lynx.EvaluationPSQTs;
using static Lynx.TunableEvalParameters;

namespace Lynx.Test;

public class PSQTTest
{
    [Test]
    public void PackedEvaluation()
    {
        short[][][] mgFriend =
        [
            MiddleGamePawnTable,
            MiddleGameKnightTable,
            MiddleGameBishopTable,
            MiddleGameRookTable,
            MiddleGameQueenTable,
            MiddleGameKingTable
        ];

        short[][][] egFriend =
        [
            EndGamePawnTable,
            EndGameKnightTable,
            EndGameBishopTable,
            EndGameRookTable,
            EndGameQueenTable,
            EndGameKingTable
        ];

        short[][][] mgEnemy =
        [
            MiddleGameEnemyPawnTable,
            MiddleGameEnemyKnightTable,
            MiddleGameEnemyBishopTable,
            MiddleGameEnemyRookTable,
            MiddleGameEnemyQueenTable,
            MiddleGameEnemyKingTable
        ];

        short[][][] egEnemy =
        [
            EndGameEnemyPawnTable,
            EndGameEnemyKnightTable,
            EndGameEnemyBishopTable,
            EndGameEnemyRookTable,
            EndGameEnemyQueenTable,
            EndGameEnemyKingTable
        ];

        for (int friendBucket = 0; friendBucket < PSQTBucketCount; ++friendBucket)
        {
            for (int enemyBucket = 0; enemyBucket < PSQTBucketCount; ++enemyBucket)
            {
                for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
                {
                    for (int sq = 0; sq < 64; ++sq)
                    {
                        int mg, eg;

                        if (piece < (int)Piece.p) // white piece
                        {
                            mg =
                                MiddleGamePieceValues[0][friendBucket][piece] + mgFriend[piece][friendBucket][sq]
                                + MiddleGamePieceValues[1][enemyBucket][piece] + mgEnemy[piece][enemyBucket][sq];

                            eg =
                                EndGamePieceValues[0][friendBucket][piece] + egFriend[piece][friendBucket][sq]
                                + EndGamePieceValues[1][enemyBucket][piece] + egEnemy[piece][enemyBucket][sq];
                        }
                        else // black piece
                        {
                            int basePiece = piece - 6;
                            // Mirror square and subtract (equivalent to adding the pre-negated table)
                            var mirror = sq ^ 56;

                            mg =
                                MiddleGamePieceValues[0][friendBucket][piece] - mgFriend[basePiece][friendBucket][mirror]
                                + MiddleGamePieceValues[1][enemyBucket][piece] - mgEnemy[basePiece][enemyBucket][mirror];

                            eg =
                                EndGamePieceValues[0][friendBucket][piece] - egFriend[basePiece][friendBucket][mirror]
                                + EndGamePieceValues[1][enemyBucket][piece] - egEnemy[basePiece][enemyBucket][mirror];
                        }

                        var packed = PSQT(friendBucket, enemyBucket, piece, sq);
                        Assert.AreEqual(mg, Utils.UnpackMG(packed), $"MG mismatch piece {piece} sq {sq} fb {friendBucket} eb {enemyBucket}");
                        Assert.AreEqual(eg, Utils.UnpackEG(packed), $"EG mismatch piece {piece} sq {sq} fb {friendBucket} eb {enemyBucket}");
                    }
                }
            }
        }
    }
}
