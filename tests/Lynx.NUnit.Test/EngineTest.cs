using Lynx.Model;
using NUnit.Framework;
using System.Linq;

namespace Lynx.NUnit.Test
{
    public class EngineTest
    {
        [TestCase("8/8/1p2k3/3bn3/3K4/8/7r/1q6 b - - 0 1", new[] { "b1d3" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 1")]
        public void BestMove_Mate_in_1(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            Assert.DoesNotThrow(() => TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString));
        }

        [TestCase("8/pN3R2/1b2k1K1/n4R2/pp1p4/3B1P1n/3B1PNP/3r3Q w - -", new[] { "d2f4" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=114463")]
        [TestCase("KQ4R1/8/8/8/4N3/8/5p2/6bk w - -", new[] { "b8b2" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=1")]
        [TestCase("8/8/8/8/8/3n1N2/8/3Q1K1k w - -", new[] { "d1b1" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=1669")]
        [TestCase("RNBKRNRQ/PPPPPPPP/8/pppppppp/rnbqkbnr/8/8/8 b - -", new[] { "g4h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=1630")]
        public void BestMove_Mate_in_2(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            Assert.DoesNotThrow(() => TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString));
        }

        [TestCase("4rqk1/3R1prR/p1p5/1p2PQp1/5p2/1P6/P1B2PP1/6K1 w - -", new[] { "f5h3" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=111285")]
        [TestCase("1b6/2p2N1K/1p2Bp1p/3Pp2R/4kp1p/1N6/p1P1PPb1/r1R4r w - -", new[] { "h7h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=251481")]
        [TestCase("5B2/3Q4/8/7p/6N1/6k1/8/5K2 w - -", new[] { "f8h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=248898")]
        [TestCase("nb5B/1N1Q4/6RK/3pPP2/RrrkN3/2pP3b/qpP1PP2/3n4 w - -", new[] { "g6g3" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=228148")]
        public void BestMove_Mate_in_3(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            Assert.DoesNotThrow(() => TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString));
        }

        [TestCase("r2k3r/p1p2ppp/2p5/2P5/6nq/2NB4/PPPP2PP/R1BQR1K1 w - - 0 13", null, new[] { "g2h3" },
            Category = "LongRunning", Explicit = true, Description = "Avoid Mate in 2, fails with initial implementation of MiniMax depth 4")]

        [TestCase("r2k3r/p1p2ppp/2p5/2P5/6nq/2NB4/PPPP2PP/R1BQR1K1 w - - 0 13", null, new[] { "g2h3", "e1e4" },
            Category = "LongRunning", Explicit = true, Description = "Avoid Mate in 4, fails with MiniMax depth 4")]

        [TestCase("q5k1/2p5/1bb1pQ2/1p6/1P6/5N2/P4PPP/3RK2R b K - 0 1", null, new[] { "a8a2" },
            Category = "LongRunning", Explicit = true, Description = "Avoid Mate in 3, seen with NegaMax depth 4 (but weirdly not with AlphaBeta depth 4)")]

        [TestCase("r2qr1k1/ppp2ppp/5n2/2nPp3/1b4b1/P1NPP1P1/1P2NPBP/R1BQ1RK1 b - - 0 1", new[] { "b4c3", "g4e2" }, new[] { "b4a5" },
            Category = "LongRunning", Explicit = true, Description = "Fails with simple MiniMax depth 3 and 4: b4a5 is played")]

        [TestCase("r2qkb1r/ppp2ppp/2n2n2/3pp3/8/PPP3Pb/3PPP1P/RNBQK1NR w KQkq - 0 1", new[] { "g1h3" },
            Category = "LongRunning", Explicit = true, Description = "Used to fail")]

        [TestCase("8/n2k4/bpr4p/1q1p4/4pp1b/P3P1Pr/R2K4/1r2n1Nr w - - 0 1", new[]
        {
            "a3a4",
            "a2a1", "a2b2", "a2c2",
            "e3f4", "f3g4",
            "g3f4", "g3g4", "g3h4",
            "g3f4", "g3g4", "g3h4",
            "g1h3", "g1f3", "g1e2"
        }, Category = "LongRunning", Explicit = true, Description = "Used to return an illegal move in the very first versions")]

        [TestCase("r1bq2k1/1pp1n2p/2nppr1Q/p7/2PP2P1/5N2/PP3P1P/2KR1B1R w - - 0 15", new[] { "h6f6" },
            Category = "LongRunning", Explicit = true, Description = "AlphaBeta/NegaMax depth 5 spends almost 3 minutes with a simple retake")]
        public void BestMove_Regression(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            Assert.DoesNotThrow(() => TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString));
        }

        [NonParallelizable]
        [TestCase("r2qkb1r/ppp2ppp/2n2n2/1B1p1b2/3P4/2N2N2/PPP2PPP/R1BQ1RK1 b kq - 0 1", 1, 8,
            null,
            new[] { "f5c2", "f5d3", "f5h3", "f8c5", "f8a3" },
            Category = "LongRunning", Explicit = true, Description = "Avoid trading pawn for minor piece or sacrificing pieces for nothing")]
        [TestCase("r2qkb1r/1pp2ppp/p1n2n2/1B1p1b2/3P4/2N2N2/PPP2PPP/R1BQ1RK1 w kq - 0 2", 1, 8,
            new[] { "b5c6", "b5a4", "f1e1", "f3h4" },
            new[] { "b5c1", "c3d5" },
            Category = "LongRunning", Explicit = true, Description = "Originally, it captured in c1")]
        [TestCase("r1bq1b1r/ppppk2p/2n1pp2/3n2B1/3P4/P4N2/1PP2PPP/RN1QKB1R w KQ - 0 1", 1, 8,
            new[] { "g5h4", "g5e3", "g5d2", "g5d1", "c2c4" },
            new[] { "a3a4" },
            Category = "LongRunning", Explicit = true, Description = "Avoid allowing pieces to be captured")]
        [TestCase("2kr3q/pbppppp1/1p1P3r/4bB2/1n2n1Q1/8/PPPPNBPP/R4RK1 b Q - 0 1", 3, 12,
            new[] { "e5h2" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 6 with quiescence, https://gameknot.com/chess-puzzle.pl?pz=257112")]
        public void BestMove_Quiescence(string fen, int depth, int quiescenceSearchDepth, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            int originalDepth = Configuration.Parameters.Depth;
            int originalQuiescenceSearchDepth = Configuration.Parameters.QuiescenceSearchDepth;

            try
            {
                Configuration.Parameters.Depth = depth;
                Configuration.Parameters.QuiescenceSearchDepth = quiescenceSearchDepth;
                Assert.DoesNotThrow(() => TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString));
            }
            finally
            {
                Configuration.Parameters.Depth = originalDepth;
                Configuration.Parameters.QuiescenceSearchDepth = originalQuiescenceSearchDepth;
            }
        }

        private static void TestBestMove(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString)
        {
            // Arrange
            var engine = new Engine();
            engine.SetGame(new Game(fen));

            // Act
            var bestMoveFound = engine.BestMove();

            // Assert
            if (allowedUCIMoveString is not null)
            {
                Assert.Contains(bestMoveFound.UCIString(), allowedUCIMoveString);
            }

            if (excludedUCIMoveString is not null)
            {
                Assert.False(excludedUCIMoveString.Contains(bestMoveFound.UCIString()));
            }
        }
    }
}
