using NUnit.Framework;

namespace Lynx.Test.BestMove;

#pragma warning disable S4144, IDE0060, RCS1163  // Methods should not have identical implementations, unused arguments

/// <summary>
/// All the tests in this class used to pass when null-pruning wasn't implemented
/// </summary>
[Explicit]
[Category(Categories.NoPruning)]
public class MatesInExactlyXTest : BaseTest
{
    public MatesInExactlyXTest()
    {
        Configuration.EngineSettings.ReverseFPMaxDepth = 0;
        Configuration.EngineSettings.LMR_DepthReduction = 0;
        Configuration.EngineSettings.NullMovePruning_R = 0;
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_1))]
    public async Task Mate_in_Exactly_1(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await TestBestMove(fen, allowedUCIMoveString, null, depth: 1);
        Assert.AreEqual(1, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_2))]
    public async Task Mate_in_Exactly_2(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await TestBestMove(fen, allowedUCIMoveString, null, depth: 3);
        Assert.AreEqual(2, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_3))]
    public async Task Mate_in_Exactly_3(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await TestBestMove(fen, allowedUCIMoveString, null, depth: 5);
        Assert.AreEqual(3, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_4))]
    public async Task Mate_in_Exactly_4(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await TestBestMove(fen, allowedUCIMoveString, null, depth: 8);
        Assert.AreEqual(4, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_4_Collection))]
    public async Task Mate_in_Exactly_4_Collection(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await TestBestMove(fen, allowedUCIMoveString, null, depth: 8);
        Assert.AreEqual(4, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_5))]
    public async Task Mate_in_Exactly_5(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await TestBestMove(fen, allowedUCIMoveString, null, depth: 10);
        Assert.AreEqual(5, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_6))]
    public async Task Mate_in_Exactly_6(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await TestBestMove(fen, allowedUCIMoveString, null, depth: 12);
        Assert.AreEqual(6, result.Mate);
    }

    [TestCaseSource(typeof(MatePositions), nameof(MatePositions.Mates_in_7))]
    public async Task Mate_in_Exactly_7(string fen, string[]? allowedUCIMoveString, string description)
    {
        var result = await TestBestMove(fen, allowedUCIMoveString, null, depth: 14);
        Assert.AreEqual(7, result.Mate);
    }
}

#pragma warning restore S4144, IDE0060, RCS1163  // Methods should not have identical implementations, unused arguments