using Lynx.Model;
using NUnit.Framework;
using static Lynx.EvaluationConstants;

namespace Lynx.Test;
public class EvaluationConstantsTest
{
    /// <summary>
    /// Shy from 14k
    /// </summary>
    private readonly int _sensibleEvaluation =
        (2 * (Math.Max(MiddleGameBishopTable.Max(), EndGameBishopTable.Max()) + Configuration.EngineSettings.BishopMobilityBonus[13].MG)) +
        (2 * (Math.Max(MiddleGameKnightTable.Max(), EndGameKnightTable.Max()))) +
        (2 * (Math.Max(MiddleGameRookTable.Max(), EndGameRookTable.Max()) + Configuration.EngineSettings.OpenFileRookBonus.MG + Configuration.EngineSettings.SemiOpenFileRookBonus.MG)) +
        (9 * (Math.Max(MiddleGameQueenTable.Max(), EndGameQueenTable.Max()) + (Configuration.EngineSettings.QueenMobilityBonus.MG * 64))) +
        (1 * (Math.Max(MiddleGameKingTable.Max(), EndGameKingTable.Max()) + (Configuration.EngineSettings.KingShieldBonus.MG * 8))) +
        MiddleGameQueenTable.Max(); // just in case

    [TestCase(PositiveCheckmateDetectionLimit)]
    [TestCase(-NegativeCheckmateDetectionLimit)]
    public void CheckmateDetectionLimitConstants(int checkmateDetectionLimit)
    {
        Assert.Greater(CheckMateBaseEvaluation - (Constants.AbsoluteMaxDepth * CheckmateDepthFactor),
            checkmateDetectionLimit);

        Assert.Greater(checkmateDetectionLimit, _sensibleEvaluation);

        Assert.Greater(short.MaxValue, checkmateDetectionLimit);
    }

    [Test]
    public void NoHashEntryConstant()
    {
        Assert.Greater(NoHashEntry, _sensibleEvaluation);
        Assert.Greater(PositiveCheckmateDetectionLimit, NoHashEntry);
        Assert.Greater(-NegativeCheckmateDetectionLimit, NoHashEntry);
    }

    [Test]
    public void EvaluationFitsIntoDepth16()
    {
        Assert.Greater(short.MaxValue, PositiveCheckmateDetectionLimit);
        Assert.Greater(short.MaxValue, NoHashEntry);
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
    public void PVMoveScoreValueConstant()
    {
        Assert.Greater(PVMoveScoreValue, TTMoveScoreValue);
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
            Assert.Less(minMVVLVAMoveValue + BadCaptureMoveBaseScoreValue, ThirdKillerMoveValue);
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
    public void ThirdKillerMoveValueConstant()
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
            Assert.Less(ThirdKillerMoveValue, minMVVLVAMoveValue + GoodCaptureMoveBaseScoreValue);
            Assert.Less(maxMVVLVAMoveValue + BadCaptureMoveBaseScoreValue, ThirdKillerMoveValue);
#pragma warning restore S3949 // Calculations should not overflow
        }

        Assert.Less(ThirdKillerMoveValue, SecondKillerMoveValue);

        Assert.Greater(ThirdKillerMoveValue, default);
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
        Assert.NotZero(EvaluationConstants.SingleMoveEvaluation);
        Assert.Greater(EvaluationConstants.SingleMoveEvaluation, 100);
        Assert.Less(EvaluationConstants.SingleMoveEvaluation, 400);
    }

    [Test]
    public void PackedEvaluation()
    {
        short[] middleGamePawnTableBlack = MiddleGamePawnTable.Select((_, index) => (short)-MiddleGamePawnTable[index ^ 56]).ToArray();
        short[] endGamePawnTableBlack = EndGamePawnTable.Select((_, index) => (short)-EndGamePawnTable[index ^ 56]).ToArray();

        short[] middleGameKnightTableBlack = MiddleGameKnightTable.Select((_, index) => (short)-MiddleGameKnightTable[index ^ 56]).ToArray();
        short[] endGameKnightTableBlack = EndGameKnightTable.Select((_, index) => (short)-EndGameKnightTable[index ^ 56]).ToArray();

        short[] middleGameBishopTableBlack = MiddleGameBishopTable.Select((_, index) => (short)-MiddleGameBishopTable[index ^ 56]).ToArray();
        short[] endGameBishopTableBlack = EndGameBishopTable.Select((_, index) => (short)-EndGameBishopTable[index ^ 56]).ToArray();

        short[] middleGameRookTableBlack = MiddleGameRookTable.Select((_, index) => (short)-MiddleGameRookTable[index ^ 56]).ToArray();
        short[] endGameRookTableBlack = EndGameRookTable.Select((_, index) => (short)-EndGameRookTable[index ^ 56]).ToArray();

        short[] middleGameQueenTableBlack = MiddleGameQueenTable.Select((_, index) => (short)-MiddleGameQueenTable[index ^ 56]).ToArray();
        short[] EndGameQueenTableBlack = EndGameQueenTable.Select((_, index) => (short)-EndGameQueenTable[index ^ 56]).ToArray();

        short[] middleGameKingTableBlack = MiddleGameKingTable.Select((_, index) => (short)-MiddleGameKingTable[index ^ 56]).ToArray();
        short[] endGameKingTableBlack = EndGameKingTable.Select((_, index) => (short)-EndGameKingTable[index ^ 56]).ToArray();

        short[][] mgPositionalTables =
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

        short[][] egPositionalTables =
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

        for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
        {
            for (int sq = 0; sq < 64; ++sq)
            {
                var mg = (short)(MiddleGamePieceValues[piece] + mgPositionalTables[piece][sq]);
                var eg = (short)(EndGamePieceValues[piece] + egPositionalTables[piece][sq]);

                Assert.AreEqual(Utils.UnpackMG(PackedPSQT[piece][sq]), mg);
                Assert.AreEqual(Utils.UnpackEG(PackedPSQT[piece][sq]), eg);
            }
        }
    }
}