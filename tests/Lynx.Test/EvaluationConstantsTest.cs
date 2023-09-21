using Lynx.Model;
using NUnit.Framework;
using static Lynx.EvaluationConstants;

namespace Lynx.Test;
public class EvaluationConstantsTestº
{
    /// <summary>
    /// Shy from 14k
    /// </summary>
    private readonly int _sensibleEvaluation =
        2 * (Math.Max(MiddleGamePositionalTables[(int)Piece.B].Max(), EndGamePositionalTables[(int)Piece.B].Max()) + Configuration.EngineSettings.BishopMobilityBonus * 64) +
        2 * (Math.Max(MiddleGamePositionalTables[(int)Piece.N].Max(), EndGamePositionalTables[(int)Piece.N].Max())) +
        2 * (Math.Max(MiddleGamePositionalTables[(int)Piece.R].Max(), EndGamePositionalTables[(int)Piece.R].Max()) + Configuration.EngineSettings.OpenFileRookBonus + Configuration.EngineSettings.SemiOpenFileRookBonus) +
        9 * (Math.Max(MiddleGamePositionalTables[(int)Piece.Q].Max(), EndGamePositionalTables[(int)Piece.Q].Max()) + Configuration.EngineSettings.QueenMobilityBonus * 64) +
        1 * (Math.Max(MiddleGamePositionalTables[(int)Piece.K].Max(), EndGamePositionalTables[(int)Piece.K].Max()) + Configuration.EngineSettings.KingShieldBonus * 8) +
        MiddleGamePositionalTables[(int)Piece.Q].Max(); // just in case

    [TestCase(PositiveCheckmateDetectionLimit)]
    [TestCase(-NegativeCheckmateDetectionLimit)]
    public void CheckmateDetectionLimitConstants(int checkmateDetectionLimit)
    {
        Assert.Greater(CheckMateBaseEvaluation - Constants.AbsoluteMaxDepth * CheckmateDepthFactor,
            checkmateDetectionLimit);

        Assert.Greater(checkmateDetectionLimit, _sensibleEvaluation);
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
                if (MostValueableVictimLeastValuableAttacker[s, t] > maxMVVLVAMoveValue)
                {
                    maxMVVLVAMoveValue = MostValueableVictimLeastValuableAttacker[s, t];
                }
            }
        }
        Assert.Greater(TTMoveScoreValue, maxMVVLVAMoveValue + CaptureMoveBaseScoreValue);
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

        for (int s = (int)Piece.P; s <= (int)Piece.r; ++s)
        {
            for (int t = (int)Piece.P; t <= (int)Piece.r; ++t)
            {
                if (MostValueableVictimLeastValuableAttacker[s, t] < minMVVLVAMoveValue)
                {
                    minMVVLVAMoveValue = MostValueableVictimLeastValuableAttacker[s, t];
                }
            }
        }

        checked
        {
#pragma warning disable S3949 // Calculations should not overflow - well, we're adding checked just in case
            Assert.Less(FirstKillerMoveValue, minMVVLVAMoveValue + CaptureMoveBaseScoreValue);
#pragma warning restore S3949 // Calculations should not overflow
        }

        Assert.Less(FirstKillerMoveValue, TTMoveScoreValue);

        Assert.Greater(FirstKillerMoveValue, SecondKillerMoveValue);
    }

    [Test]
    public void SecondKillerMoveValueConstant()
    {
        var minMVVLVAMoveValue = int.MaxValue;

        for (int s = (int)Piece.P; s <= (int)Piece.r; ++s)
        {
            for (int t = (int)Piece.P; t <= (int)Piece.r; ++t)
            {
                if (MostValueableVictimLeastValuableAttacker[s, t] < minMVVLVAMoveValue)
                {
                    minMVVLVAMoveValue = MostValueableVictimLeastValuableAttacker[s, t];
                }
            }
        }

        checked
        {
#pragma warning disable S3949 // Calculations should not overflow - well, we're adding checked just in case
            Assert.Less(SecondKillerMoveValue, minMVVLVAMoveValue + CaptureMoveBaseScoreValue);
#pragma warning restore S3949 // Calculations should not overflow
        }

        Assert.Less(SecondKillerMoveValue, FirstKillerMoveValue);

        Assert.Greater(SecondKillerMoveValue, default);
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
    public void PSQTSize()
    {
        Assert.AreEqual(12, MiddleGamePositionalTables.Length);
        foreach (var table in MiddleGamePositionalTables)
        {
            Assert.AreEqual(64, table.Length);
        }

        Assert.AreEqual(12, EndGamePositionalTables.Length);
        foreach (var table in EndGamePositionalTables)
        {
            Assert.AreEqual(64, table.Length);
        }

        Assert.AreEqual(5, GamePhaseByPiece.Length);
        Assert.AreEqual(12, MiddleGamePieceValues.Length);
        Assert.AreEqual(12, EndGamePieceValues.Length);
    }
}