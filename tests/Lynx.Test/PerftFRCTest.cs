using NUnit.Framework;
using Lynx.Model;

namespace Lynx.Test;

/// <summary>
/// https://www.chessprogramming.org/Perft_Results
/// </summary>
[TestFixture(Category = Categories.Perft, Explicit = true)]
public class PerftFRCTest
{
    public PerftFRCTest()
    {
        Configuration.EngineSettings.IsChess960 = true;
        UpdateCurrentInstance();
    }

    /// <summary>
    /// https://github.com/toanth/motors/blob/main/gears/src/games/chess/perft_tests.rs
    /// </summary>
    [TestCase("2r1kr2/8/8/8/8/8/8/1R2K1R1 w GBfc - 0 1", 1, 22, 501, 11459)]
    [TestCase("2r1kr2/8/8/8/8/8/8/1R2K1R1 w GBfc - 0 1", 2, 501, 11459)]
    [TestCase("2r1kr2/8/8/8/8/8/8/1R2K1R1 w GBfc - 0 1", 3, 11459)]
    [TestCase("rkr5/8/8/8/8/8/8/5RKR w HFca - 0 1", 1, 22)]
    [TestCase("rkr5/8/8/8/8/8/8/5RKR w HFca - 0 1", 2, 442)]
    [TestCase("rkr5/8/8/8/8/8/8/5RKR w HFca - 0 1", 3, 10217)]
    [TestCase("2r3kr/8/8/8/8/8/8/2KRR3 w h - 3 2", 1, 3)]
    [TestCase("2r3kr/8/8/8/8/8/8/2KRR3 w h - 3 2", 2, 72)]
    [TestCase("2r3kr/8/8/8/8/8/8/2KRR3 w h - 3 2", 3, 1371)]
    [TestCase("5rkr/8/8/8/8/8/8/RKR5 w CAhf - 0 1", 1, 22)]
    [TestCase("5rkr/8/8/8/8/8/8/RKR5 w CAhf - 0 1", 2, 442)]
    [TestCase("5rkr/8/8/8/8/8/8/RKR5 w CAhf - 0 1", 3, 10206)]
    [TestCase("3rkr2/8/8/8/8/8/8/R3K2R w HAfd - 0 1", 1, 20)]
    [TestCase("3rkr2/8/8/8/8/8/8/R3K2R w HAfd - 0 1", 2, 452)]
    [TestCase("3rkr2/8/8/8/8/8/8/R3K2R w HAfd - 0 1", 3, 9873)]
    [TestCase("4k3/8/8/8/8/8/8/4KR2 w F - 0 1", 1, 14)]
    [TestCase("4k3/8/8/8/8/8/8/4KR2 w F - 0 1", 2, 47)]
    [TestCase("4k3/8/8/8/8/8/8/4KR2 w F - 0 1", 3, 781)]
    [TestCase("4kr2/8/8/8/8/8/8/4K3 w f - 0 1", 1, 3)]
    [TestCase("4kr2/8/8/8/8/8/8/4K3 w f - 0 1", 2, 42)]
    [TestCase("4kr2/8/8/8/8/8/8/4K3 w f - 0 1", 3, 246)]
    [TestCase("4k3/8/8/8/8/8/8/2R1K3 w C - 0 1", 1, 16)]
    [TestCase("4k3/8/8/8/8/8/8/2R1K3 w C - 0 1", 2, 71)]
    [TestCase("4k3/8/8/8/8/8/8/2R1K3 w C - 0 1", 3, 1277)]
    [TestCase("2r1k3/8/8/8/8/8/8/4K3 w c - 0 1", 1, 5)]
    [TestCase("2r1k3/8/8/8/8/8/8/4K3 w c - 0 1", 2, 80)]
    [TestCase("2r1k3/8/8/8/8/8/8/4K3 w c - 0 1", 3, 448)]
    public void MotorsFRC(string fen, int depth, long expectedNumberOfNodes)
    {
        Configuration.EngineSettings.IsChess960 = true;

        Validate(fen, depth, expectedNumberOfNodes);
    }

    /// <summary>
    /// https://github.com/toanth/motors/blob/main/gears/src/games/chess/perft_tests.rs
    /// Only meant to make sure DFRC works assuming Chess960 and normal chess movegen already works.
    /// </summary>
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 1, 32)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 2, 1093)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 3, 34210)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 4, 1187103)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 5, 37188628)]
    [TestCase("r1q1k1rn/1p1ppp1p/1npb2b1/p1N3p1/8/1BP4P/PP1PPPP1/1RQ1KRBN w BFag - 0 9", 6, 1308319545)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 1, 31)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 2, 841)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 3, 23877)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 4, 711547)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 5, 20894205)]
    [TestCase("rk3r2/8/8/5r2/6R1/8/8/R3K1R1 w AGaf - 0 1", 6, 644033568)]
    public void MotorsDFRC(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    private static void Validate(string fen, int depth, long expectedNumberOfNodes)
    {
        Assert.AreEqual(expectedNumberOfNodes, Perft.PerftRecursiveImpl(new Position(fen), depth, default));
    }
}
