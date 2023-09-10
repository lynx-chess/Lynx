using NUnit.Framework;

#pragma warning disable S4144, IDE0060, RCS1163  // Methods should not have identical implementations, unused arguments

namespace Lynx.Test.BestMove;

public class MatesTest : BaseTest
{
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_1))]
    public async Task Mate_in_1(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await SearchBestMove(fen);
        Assert.AreEqual(1, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_2))]
    public async Task Mate_in_2(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    [Category(Categories.LongRunning)]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_3))]
    public async Task Mate_in_3(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    [Explicit]
    [Category(Categories.LongRunning)]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_4))]
    public async Task Mate_in_4(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    /// <summary>
    /// http://www.talkchess.com/forum3/viewtopic.php?f=7&t=78583
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="allowedUCIMoveString"></param>
    [Explicit]
    [Category(Categories.LongRunning)]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_4_Collection))]
    public async Task Mate_in_4_Collection(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    [Explicit]
    [Category(Categories.LongRunning)]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_5))]
    public async Task Mate_in_5(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    [Explicit]
    [Category(Categories.LongRunning)]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_6))]
    public async Task Mate_in_6(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await SearchBestMove(fen);
        Assert.AreNotEqual(default, result.Mate);
    }

    [Explicit]
    [Category(Categories.LongRunning)]
    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_7))]
    public async Task Mate_in_7(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await SearchBestMove(fen, depth: 14);
        Assert.AreNotEqual(default, result.Mate);
    }
}

#pragma warning restore S4144, IDE0060, RCS1163  // Methods should not have identical implementations, unused arguments