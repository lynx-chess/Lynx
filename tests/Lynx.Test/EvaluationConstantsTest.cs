using Lynx.Model;
using NUnit.Framework;
using static Lynx.EvaluationConstants;
using static Lynx.EvaluationParams;
using static Lynx.EvaluationPSQTs;
using static Lynx.TunableEvalParameters;
using static Lynx.Utils;

namespace Lynx.Test;
public class EvaluationConstantsTest
{
    /// <summary>
    /// Shy from 14k
    /// </summary>
    private readonly int _sensibleEvaluation =
        (2 * (Math.Max(MiddleGameBishopTable[0].Max(), EndGameBishopTable[0].Max()) + UnpackMG(BishopMobilityBonus[13]))) +
        (2 * (Math.Max(MiddleGameKnightTable[0].Max(), EndGameKnightTable[0].Max()))) +
        (2 * (Math.Max(MiddleGameRookTable[0].Max(), EndGameRookTable[0].Max()) + UnpackMG(OpenFileRookBonus[0][0]) + UnpackMG(SemiOpenFileRookBonus[0][0]))) +
        (9 * (Math.Max(MiddleGameQueenTable[0].Max(), EndGameQueenTable[0].Max()) + (UnpackMG(QueenMobilityBonus[27]) * 9))) +
        (1 * (Math.Max(MiddleGameKingTable[0].Max(), EndGameKingTable[0].Max()) + (UnpackMG(KingShieldBonus) * 8))) +
        MiddleGameQueenTable[0].Max(); // just in case

    [Test]
    public void PositiveCheckmateDetectionLimitTest()
    {
        Assert.Greater(CheckMateBaseEvaluation - (Constants.AbsoluteMaxDepth + 10),
            PositiveCheckmateDetectionLimit);

        Assert.Greater(PositiveCheckmateDetectionLimit, _sensibleEvaluation);

        Assert.Greater(short.MaxValue, PositiveCheckmateDetectionLimit);
    }

    [Test]
    public void NegativeCheckmateDetectionLimitTest()
    {
        Assert.Less(-(CheckMateBaseEvaluation - (Constants.AbsoluteMaxDepth + 10)),
            NegativeCheckmateDetectionLimit);

        Assert.Less(NegativeCheckmateDetectionLimit, -_sensibleEvaluation);

        Assert.Less(short.MinValue, NegativeCheckmateDetectionLimit);
    }

    [Test]
    public void CheckmateDepthFactorTest()
    {
        const int maxCheckmateValue = CheckMateBaseEvaluation - Constants.AbsoluteMaxDepth;
        Assert.Less(maxCheckmateValue, MaxEval);
        Assert.Greater(maxCheckmateValue, MinEval);

        Assert.Greater(maxCheckmateValue, PositiveCheckmateDetectionLimit);
        Assert.Greater(maxCheckmateValue, NegativeCheckmateDetectionLimit);

        const int minCheckmateValue = -CheckMateBaseEvaluation + Constants.AbsoluteMaxDepth;
        Assert.Less(minCheckmateValue, MaxEval);
        Assert.Greater(minCheckmateValue, MinEval);

        Assert.Less(minCheckmateValue, PositiveCheckmateDetectionLimit);
        Assert.Less(minCheckmateValue, NegativeCheckmateDetectionLimit);

        var recalculatedMaxCheckmateOnProbe = TranspositionTable.RecalculateMateScores(maxCheckmateValue, +Constants.AbsoluteMaxDepth);
        Assert.Less(recalculatedMaxCheckmateOnProbe, MaxEval);
        Assert.Greater(recalculatedMaxCheckmateOnProbe, MinEval);

        Assert.Greater(recalculatedMaxCheckmateOnProbe, PositiveCheckmateDetectionLimit);
        Assert.Greater(recalculatedMaxCheckmateOnProbe, NegativeCheckmateDetectionLimit);

        var recalculatedMaxCheckmateOnSave = TranspositionTable.RecalculateMateScores(maxCheckmateValue, -Constants.AbsoluteMaxDepth);
        Assert.Less(recalculatedMaxCheckmateOnSave, MaxEval);
        Assert.Greater(recalculatedMaxCheckmateOnSave, MinEval);

        Assert.Greater(recalculatedMaxCheckmateOnSave, PositiveCheckmateDetectionLimit);
        Assert.Greater(recalculatedMaxCheckmateOnSave, NegativeCheckmateDetectionLimit);

        var recalculatedMinCheckmateOnProbe = TranspositionTable.RecalculateMateScores(minCheckmateValue, +Constants.AbsoluteMaxDepth);
        Assert.Less(recalculatedMinCheckmateOnProbe, MaxEval);
        Assert.Greater(recalculatedMinCheckmateOnProbe, MinEval);

        Assert.Less(recalculatedMinCheckmateOnProbe, PositiveCheckmateDetectionLimit);
        Assert.Less(recalculatedMinCheckmateOnProbe, NegativeCheckmateDetectionLimit);

        var recalculatedMinCheckmateOnSave = TranspositionTable.RecalculateMateScores(minCheckmateValue, -Constants.AbsoluteMaxDepth);
        Assert.Less(recalculatedMinCheckmateOnSave, MaxEval);
        Assert.Greater(recalculatedMinCheckmateOnSave, MinEval);

        Assert.Less(recalculatedMinCheckmateOnSave, PositiveCheckmateDetectionLimit);
        Assert.Less(recalculatedMinCheckmateOnSave, NegativeCheckmateDetectionLimit);
    }

    [Test]
    public void MaxEvalTest()
    {
        Assert.Greater(MaxEval, PositiveCheckmateDetectionLimit + Constants.AbsoluteMaxDepth + 10);
        Assert.Greater(MaxEval, CheckMateBaseEvaluation + Constants.AbsoluteMaxDepth + 10);
        Assert.Greater(MaxEval, TranspositionTable.RecalculateMateScores(CheckMateBaseEvaluation, Constants.AbsoluteMaxDepth));
        Assert.Greater(MaxEval, TranspositionTable.RecalculateMateScores(CheckMateBaseEvaluation, -Constants.AbsoluteMaxDepth));
        Assert.Less(MaxEval, short.MaxValue);
    }

    [Test]
    public void MinEvalTest()
    {
        Assert.Less(MinEval, NegativeCheckmateDetectionLimit - (Constants.AbsoluteMaxDepth + 10));
        Assert.Less(MinEval, -CheckMateBaseEvaluation - (Constants.AbsoluteMaxDepth + 10));
        Assert.Less(MinEval, TranspositionTable.RecalculateMateScores(-CheckMateBaseEvaluation, Constants.AbsoluteMaxDepth));
        Assert.Less(MinEval, TranspositionTable.RecalculateMateScores(-CheckMateBaseEvaluation, -Constants.AbsoluteMaxDepth));
        Assert.Greater(MinEval, short.MinValue);
    }

    [Test]
    public void MaxStaticEvalTest()
    {
        Assert.Less(MaxStaticEval, PositiveCheckmateDetectionLimit);
    }

    [Test]
    public void MinStaticEvalTest()
    {
        Assert.Greater(MinStaticEval, NegativeCheckmateDetectionLimit);
    }

    [Test]
    public void NoHashEntryConstant()
    {
        Assert.Less(NoScore, -_sensibleEvaluation);
        Assert.Less(NoScore, NegativeCheckmateDetectionLimit);
        Assert.Less(NoScore, MinEval);
    }

    [Test]
    public void EvaluationFitsIntoDepth16()
    {
        Assert.Greater(short.MaxValue, PositiveCheckmateDetectionLimit);
        Assert.Greater(short.MaxValue, -NoScore);
        Assert.Greater(short.MaxValue, _sensibleEvaluation);
    }

    [Test]
    public void TTMoveScoreValueConstant()
    {
        var maxMVVLVAMoveValue = int.MinValue;

        for (int s = (int)Piece.P; s <= (int)Piece.r; ++s)
        {
            for (int t = (int)Piece.P; t <= (int)Piece.r; ++t)
            {
                if (MostValueableVictimLeastValuableAttacker[s][t] > maxMVVLVAMoveValue)
                {
                    maxMVVLVAMoveValue = MostValueableVictimLeastValuableAttacker[s][t];
                }
            }
        }
        Assert.Greater(TTMoveScoreValue, maxMVVLVAMoveValue + BadCaptureMoveBaseScoreValue);
    }

    [Test]
    public void FirstKillerMoveValueConstant()
    {
        var minMVVLVAMoveValue = int.MaxValue;
        var maxMVVLVAMoveValue = int.MinValue;

        for (int s = (int)Piece.P; s <= (int)Piece.r; ++s)
        {
            for (int t = (int)Piece.P; t <= (int)Piece.r; ++t)
            {
                if (MostValueableVictimLeastValuableAttacker[s][t] < minMVVLVAMoveValue)
                {
                    minMVVLVAMoveValue = MostValueableVictimLeastValuableAttacker[s][t];
                }

                if (MostValueableVictimLeastValuableAttacker[s][t] > maxMVVLVAMoveValue)
                {
                    maxMVVLVAMoveValue = MostValueableVictimLeastValuableAttacker[s][t];
                }
            }
        }

        checked
        {
#pragma warning disable S3949 // Calculations should not overflow - well, we're adding checked just in case
            Assert.Less(FirstKillerMoveValue, minMVVLVAMoveValue + GoodCaptureMoveBaseScoreValue);
            Assert.Less(maxMVVLVAMoveValue + BadCaptureMoveBaseScoreValue, FirstKillerMoveValue);
            Assert.Less(minMVVLVAMoveValue + BadCaptureMoveBaseScoreValue, SecondKillerMoveValue);
#pragma warning restore S3949 // Calculations should not overflow
        }

        Assert.Less(FirstKillerMoveValue, TTMoveScoreValue);

        Assert.Greater(FirstKillerMoveValue, SecondKillerMoveValue);
    }

    [Test]
    public void SecondKillerMoveValueConstant()
    {
        var minMVVLVAMoveValue = int.MaxValue;
        var maxMVVLVAMoveValue = int.MinValue;

        for (int s = (int)Piece.P; s <= (int)Piece.r; ++s)
        {
            for (int t = (int)Piece.P; t <= (int)Piece.r; ++t)
            {
                if (MostValueableVictimLeastValuableAttacker[s][t] < minMVVLVAMoveValue)
                {
                    minMVVLVAMoveValue = MostValueableVictimLeastValuableAttacker[s][t];
                }

                if (MostValueableVictimLeastValuableAttacker[s][t] > maxMVVLVAMoveValue)
                {
                    maxMVVLVAMoveValue = MostValueableVictimLeastValuableAttacker[s][t];
                }
            }
        }

        checked
        {
#pragma warning disable S3949 // Calculations should not overflow - well, we're adding checked just in case
            Assert.Less(SecondKillerMoveValue, minMVVLVAMoveValue + GoodCaptureMoveBaseScoreValue);
            Assert.Less(maxMVVLVAMoveValue + BadCaptureMoveBaseScoreValue, SecondKillerMoveValue);
#pragma warning restore S3949 // Calculations should not overflow
        }

        Assert.Less(SecondKillerMoveValue, FirstKillerMoveValue);

        Assert.Greater(SecondKillerMoveValue, default);
    }

    [Test]
    public void PromotionMoveValueConstant()
    {
        var maxMVVLVAMoveValue = int.MinValue;

        for (int s = (int)Piece.P; s <= (int)Piece.r; ++s)
        {
            for (int t = (int)Piece.P; t <= (int)Piece.r; ++t)
            {
                if (MostValueableVictimLeastValuableAttacker[s][t] > maxMVVLVAMoveValue)
                {
                    maxMVVLVAMoveValue = MostValueableVictimLeastValuableAttacker[s][t];
                }
            }
        }

        Assert.Less(BadCaptureMoveBaseScoreValue + maxMVVLVAMoveValue, PromotionMoveScoreValue);
    }

    /// <summary>
    /// Avoids drawish evals that can lead the GUI to declare a draw
    /// or negative ones that can lead it to resign
    /// </summary>
    [Test]
    public void SingleMoveEvaluation()
    {
        Assert.NotZero(SingleMoveScore);
        Assert.Greater(SingleMoveScore, 50);
    }

    /// <summary>
    /// Avoids drawish evals that can lead the GUI to declare a draw
    /// or negative ones that can lead it to resign
    /// </summary>
    [Test]
    public void EmergencyMoveEvaluation()
    {
        Assert.NotZero(EmergencyMoveScore);
        Assert.Less(EmergencyMoveScore, -50);
        Assert.Greater(EmergencyMoveScore, -200);
    }

    [Test]
    public void PackedEvaluation()
    {
        short[][] middleGamePawnTableBlack = [.. MiddleGamePawnTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];
        short[][] endGamePawnTableBlack = [.. EndGamePawnTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];

        short[][] middleGameKnightTableBlack = [.. MiddleGameKnightTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];
        short[][] endGameKnightTableBlack = [.. EndGameKnightTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];

        short[][] middleGameBishopTableBlack = [.. MiddleGameBishopTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];
        short[][] endGameBishopTableBlack = [.. EndGameBishopTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];

        short[][] middleGameRookTableBlack = [.. MiddleGameRookTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];
        short[][] endGameRookTableBlack = [.. EndGameRookTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];

        short[][] middleGameQueenTableBlack = [.. MiddleGameQueenTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];
        short[][] EndGameQueenTableBlack = [.. EndGameQueenTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];

        short[][] middleGameKingTableBlack = [.. MiddleGameKingTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];
        short[][] endGameKingTableBlack = [.. EndGameKingTable.Select(bucketedArray => bucketedArray.Select((_, index) => (short)-bucketedArray[index ^ 56]).ToArray())];

        short[][][] mgPositionalTables =
        [
            MiddleGamePawnTable,
            MiddleGameKnightTable,
            MiddleGameBishopTable,
            MiddleGameRookTable,
            MiddleGameQueenTable,
            MiddleGameKingTable,

            middleGamePawnTableBlack,
            middleGameKnightTableBlack,
            middleGameBishopTableBlack,
            middleGameRookTableBlack,
            middleGameQueenTableBlack,
            middleGameKingTableBlack
        ];

        short[][][] egPositionalTables =
        [
            EndGamePawnTable,
            EndGameKnightTable,
            EndGameBishopTable,
            EndGameRookTable,
            EndGameQueenTable,
            EndGameKingTable,

            endGamePawnTableBlack,
            endGameKnightTableBlack,
            endGameBishopTableBlack,
            endGameRookTableBlack,
            EndGameQueenTableBlack,
            endGameKingTableBlack
        ];

        for (int friendBucket = 0; friendBucket < PSQTBucketCount; ++friendBucket)
        {
            for (int enemyBucket = 0; enemyBucket < PSQTBucketCount; ++enemyBucket)
            {
                for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
                {
                    for (int sq = 0; sq < 64; ++sq)
                    {
                        var mg = (short)(MiddleGamePieceValues[0][friendBucket][piece] + mgPositionalTables[piece][friendBucket][sq]
                            + MiddleGamePieceValues[1][enemyBucket][piece] + mgPositionalTables[piece][enemyBucket][sq]);

                        var eg = (short)(EndGamePieceValues[0][friendBucket][piece] + egPositionalTables[piece][friendBucket][sq]
                            + EndGamePieceValues[1][enemyBucket][piece] + egPositionalTables[piece][enemyBucket][sq]);

                        var psqt = PSQT(friendBucket, enemyBucket, piece, sq);

                        Assert.AreEqual(Utils.UnpackEG(psqt), eg);
                        Assert.AreEqual(Utils.UnpackMG(psqt), mg);
                    }
                }
            }
        }
    }

    /// <summary>
    /// If this fails after a change, pawn eval calculations should be revisited, because phase isn't being added there
    /// </summary>
    [Test]
    public void GamePhaseByPiece_ForPawns_ShouldBeZero()
    {
        Assert.Zero(GamePhaseByPiece[(int)Piece.P]);
        Assert.Zero(GamePhaseByPiece[(int)Piece.p]);
    }
}
