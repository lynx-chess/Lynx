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
        2 * (MaterialScore[(int)Piece.B] + PositionalScore[(int)Piece.B].Max() + Configuration.EngineSettings.BishopMobilityBonus * 64) +
        2 * (MaterialScore[(int)Piece.N] + PositionalScore[(int)Piece.N].Max()) +
        2 * (MaterialScore[(int)Piece.R] + PositionalScore[(int)Piece.R].Max() + Configuration.EngineSettings.OpenFileRookBonus + Configuration.EngineSettings.SemiOpenFileRookBonus) +
        9 * (MaterialScore[(int)Piece.Q] + PositionalScore[(int)Piece.Q].Max() + Configuration.EngineSettings.QueenMobilityBonus * 64) +
        Configuration.EngineSettings.KingShieldBonus * 8 +
        MaterialScore[(int)Piece.Q]; // just in case

    [TestCase(PositiveCheckmateDetectionLimit)]
    [TestCase(-NegativeCheckmateDetectionLimit)]
    public void CheckmateDetectionLimitConstants(int checkmateDetectionLimit)
    {
        Assert.Greater(CheckMateBaseEvaluation - Constants.AbsoluteMaxDepth * CheckmateDepthFactor,
            checkmateDetectionLimit);

        Assert.Greater(checkmateDetectionLimit, _sensibleEvaluation);

        Assert.Greater(checkmateDetectionLimit, PVMoveScoreValue);
        Assert.Greater(checkmateDetectionLimit, FirstKillerMoveValue);
        Assert.Greater(checkmateDetectionLimit, SecondKillerMoveValue);
    }

    [Test]
    public void NoHashEntryConstant()
    {
        Assert.Greater(NoHashEntry, _sensibleEvaluation);
        Assert.Greater(PositiveCheckmateDetectionLimit, NoHashEntry);
        Assert.Greater(-NegativeCheckmateDetectionLimit, NoHashEntry);

        Assert.Greater(NoHashEntry, PVMoveScoreValue);
        Assert.Greater(NoHashEntry, FirstKillerMoveValue);
        Assert.Greater(NoHashEntry, SecondKillerMoveValue);
    }

    [Test]
    public void EvaluationFitsIntoDepth16()
    {
        Assert.Greater(short.MaxValue, PositiveCheckmateDetectionLimit);
        Assert.Greater(short.MaxValue, NoHashEntry);
        Assert.Greater(short.MaxValue, _sensibleEvaluation);
    }
}