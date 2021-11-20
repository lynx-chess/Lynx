using NUnit.Framework;

#pragma warning disable S4144, IDE0060, RCS1163  // Methods should not have identical implementations, unused arguments

namespace Lynx.Test.BestMove;

public class MatesTest : BaseTest
{
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_1))]
    public void Mate_in_1(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = SearchBestMove(fen);
        Assert.AreEqual(1, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_2))]
    public void Mate_in_2(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = SearchBestMove(fen);
        Assert.AreEqual(2, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_3), Category = "LongRunning")]
    public void Mate_in_3(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_4), Category = "LongRunning")]
    public void Mate_in_4(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    /// <summary>
    /// http://www.talkchess.com/forum3/viewtopic.php?f=7&t=78583
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="allowedUCIMoveString"></param>
    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_4_Collection), Category = "ExtraLongRunning")]
    public void Mate_in_4_Collection(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_5), Category = "LongRunning")]
    public void Mate_in_5(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_6), Category = "ExtraLongRunning")]
    public void Mate_in_6(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    [Explicit]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_7), Category = "TooLongToBeRun")]
    public void Mate_in_7(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = SearchBestMove(fen);
        Assert.AreNotEqual(0, result.Mate);
    }
}

#pragma warning restore S4144, IDE0060, RCS1163  // Methods should not have identical implementations, unused arguments