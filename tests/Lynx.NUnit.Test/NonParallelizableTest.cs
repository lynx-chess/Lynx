using NUnit.Framework;

namespace Lynx.NUnit.Test
{
    [NonParallelizable]
    public class NonParallelizableTest : BaseTest
    {
        [TestCase("r2qkb1r/ppp2ppp/2n2n2/1B1p1b2/3P4/2N2N2/PPP2PPP/R1BQ1RK1 b kq - 0 1", 1, 8,
            null,
            new[] { "f5c2", "f5d3", "f5h3", "f8c5", "f8a3" },
            Category = "NonParallelizable", Explicit = true, Description = "Avoid trading pawn for minor piece or sacrificing pieces for nothing")]
        [TestCase("r2qkb1r/1pp2ppp/p1n2n2/1B1p1b2/3P4/2N2N2/PPP2PPP/R1BQ1RK1 w kq - 0 2", 1, 8,
            new[] { "b5c6", "b5a4", "f1e1", "f3h4", "d1e1" },
            new[] { "b5c1", "c3d5" },
            Category = "NonParallelizable", Explicit = true, Description = "Originally, it captured in c1")]
        [TestCase("r1bq1b1r/ppppk2p/2n1pp2/3n2B1/3P4/P4N2/1PP2PPP/RN1QKB1R w KQ - 0 1", 1, 8,
            new[] { "g5h4", "g5e3", "g5d2", "g5c1", "c2c4" },
            new[] { "a3a4" },
            Category = "NonParallelizable", Explicit = true, Description = "Avoid allowing pieces to be captured")]
        [TestCase("2kr3q/pbppppp1/1p1P3r/4bB2/1n2n1Q1/8/PPPPNBPP/R4RK1 b Q - 0 1", 3, 12,
            new[] { "e5h2" },
            Category = "NonParallelizable", Explicit = true, Description = "Mate in 6 with quiescence, https://gameknot.com/chess-puzzle.pl?pz=257112")]
        public void BestMove_Quiescence(string fen, int depth, int quiescenceSearchDepth, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            int originalDepth = Configuration.EngineSettings.Depth;
            int originalQuiescenceSearchDepth = Configuration.EngineSettings.QuiescenceSearchDepth;

            try
            {
                Configuration.EngineSettings.Depth = depth;
                Configuration.EngineSettings.QuiescenceSearchDepth = quiescenceSearchDepth;
                TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
            }
            finally
            {
                Configuration.EngineSettings.Depth = originalDepth;
                Configuration.EngineSettings.QuiescenceSearchDepth = originalQuiescenceSearchDepth;
            }
        }
    }
}
