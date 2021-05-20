using Lynx.Model;
using NUnit.Framework;
using System.Linq;

namespace Lynx.NUnit.Test
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class EngineTest
    {
        [TestCase("8/8/1p2k3/3bn3/3K4/8/7r/1q6 b - - 0 1", new[] { "b1d3" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 1")]

        [TestCase("8/pN3R2/1b2k1K1/n4R2/pp1p4/3B1P1n/3B1PNP/3r3Q w - -", new[] { "d2f4" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=114463")]
        [TestCase("KQ4R1/8/8/8/4N3/8/5p2/6bk w - -", new[] { "b8b2" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=1")]
        [TestCase("8/8/8/8/8/3n1N2/8/3Q1K1k w - -", new[] { "d1b1" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=1669")]
        [TestCase("RNBKRNRQ/PPPPPPPP/8/pppppppp/rnbqkbnr/8/8/8 b - -", new[] { "g4h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=1630")]

        [TestCase("4rqk1/3R1prR/p1p5/1p2PQp1/5p2/1P6/P1B2PP1/6K1 w - -", new[] { "f5h3" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=111285")]
        [TestCase("1b6/2p2N1K/1p2Bp1p/3Pp2R/4kp1p/1N6/p1P1PPb1/r1R4r w - -", new[] { "h7h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=251481")]
        [TestCase("5B2/3Q4/8/7p/6N1/6k1/8/5K2 w - -", new[] { "f8h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=248898")]
        [TestCase("nb5B/1N1Q4/6RK/3pPP2/RrrkN3/2pP3b/qpP1PP2/3n4 w - -", new[] { "g6g3" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=228148")]

        [TestCase("r2k3r/p1p2ppp/2p5/2P5/6nq/2NB4/PPPP2PP/R1BQR1K1 w - - 0 13", null, new[] { "g2h3" },
            Category = "LongRunning", Explicit = true, Description = "Avoid Mate in 2, fails with initial implementation of MiniMax depth 4")]

        [TestCase("r2k3r/p1p2ppp/2p5/2P5/6nq/2NB4/PPPP2PP/R1BQR1K1 w - - 0 13", null, new[] { "g2h3", "e1e4" },
            Category = "LongRunning", Explicit = true, Description = "Avoid Mate in 4, fails with MiniMax depth 4")]

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
        public void BestMove(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            var engine = new Engine();
            // Arrange
            engine.SetGame(new Game(fen));
            //Configuration.Parameters.Depth = 5;

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
