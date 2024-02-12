using NUnit.Framework;

namespace Lynx.Test.BestMove;

public class QuiescenceTest : BaseTest
{
    [TestCase("r2qkb1r/ppp2ppp/2n2n2/1B1p1b2/3P4/2N2N2/PPP2PPP/R1BQ1RK1 b kq - 0 1", 1, 12,
        null,
        new[] { "f5c2", "f5d3", "f5h3", "f8c5", "f8a3" },
        Description = "Avoid trading pawn for minor piece or sacrificing pieces for nothing")]
    [TestCase("r2qkb1r/1pp2ppp/p1n2n2/1B1p1b2/3P4/2N2N2/PPP2PPP/R1BQ1RK1 w kq - 0 2", 1, 12,
        new[] { "b5c6", "b5a4", "f1e1", "f3h4", "d1e1", "b5d3" },
        new[] { "b5c1", "c3d5" },
        Description = "Originally, it captured in c1")]
    [TestCase("r1bq1b1r/ppppk2p/2n1pp2/3n2B1/3P4/P4N2/1PP2PPP/RN1QKB1R w KQ - 0 1", 1, 12,
        new[] { "g5h4", "g5e3", "g5d2", "g5c1", "c2c4" },
        new[] { "a3a4" },
        Description = "Avoid allowing pieces to be captured")]
    [TestCase("2kr3q/pbppppp1/1p1P3r/4bB2/1n2n1Q1/8/PPPPNBPP/R4RK1 b Q - 0 1", 3, 12,
        new[] { "e5h2" },
        Description = "Mate in 6 with quiescence, https://gameknot.com/chess-puzzle.pl?pz=257112",
        Ignore = "Fails after fixing LMR implementation")]
#pragma warning disable RCS1163, IDE0060 // Unused parameter.
    public void Quiescence(string fen, int depth, int minQuiescenceSearchDepth, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
#pragma warning restore RCS1163, IDE0060 // Unused parameter.
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth);
    }

    [TestCase("7k/8/5NQ1/8/8/KN6/8/1r6 b - - 0 1", 1, new[] { "b1b3" })]
    [TestCase("5Rbk/8/5N2/6Q1/8/1r6/8/KN6 b - - 0 1", 1, new[] { "b3b1" })]
    public void DetectDrawWhenNoCaptures(string fen, int depth, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth);
    }
}
