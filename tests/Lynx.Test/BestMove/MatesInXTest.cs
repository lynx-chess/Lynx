using NUnit.Framework;

namespace Lynx.Test.BestMove;

#pragma warning disable S4144, IDE0060, RCS1163  // Methods should not have identical implementations, unused arguments

public class MatesInXTest : BaseTest
{
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_1))]
    public void Mate_in_1(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = TestBestMove(fen, allowedUCIMoveString, null, depth: 1);
        Assert.AreEqual(1, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_2))]
    public void Mate_in_2(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = TestBestMove(fen, allowedUCIMoveString, null, depth: 3);
        Assert.AreEqual(2, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_3), Category = "NoPruning")]
    public void Mate_in_3(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = TestBestMove(fen, allowedUCIMoveString, null, depth: 5);
        Assert.AreEqual(3, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_4), Category = "NoPruning")]
    public void Mate_in_4(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = TestBestMove(fen, allowedUCIMoveString, null, depth: 8);
        Assert.AreEqual(4, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_4_Collection), Category = "NoPruning")]
    public void Mate_in_4_Collection(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = TestBestMove(fen, allowedUCIMoveString, null, depth: 8);
        Assert.AreEqual(4, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_5), Category = "NoPruning")]
    public void Mate_in_5(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = TestBestMove(fen, allowedUCIMoveString, null, depth: 10);
        Assert.AreEqual(5, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_6), Category = "TooLongToBeRun")]
    public void Mate_in_6(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = TestBestMove(fen, allowedUCIMoveString, null, depth: 12);
        Assert.AreEqual(6, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_7), Category = "TooLongToBeRun")]
    public void Mate_in_7(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = TestBestMove(fen, allowedUCIMoveString, null, depth: 14);
        Assert.AreEqual(7, result.Mate);
    }
}

#pragma warning restore S4144, IDE0060, RCS1163  // Methods should not have identical implementations, unused arguments