using Lynx.UCI.Commands.GUI;
using NUnit.Framework;

namespace Lynx.Test.BestMove
{
    public class RegressionTest : BaseTest
    {
        [TestCase("r2k3r/p1p2ppp/2p5/2P5/6nq/2NB4/PPPP2PP/R1BQR1K1 w - - 0 13", null, new[] { "g2h3" },
            Description = "Avoid Mate in 2, fails with initial implementation of MiniMax depth 4")]

        [TestCase("r2k3r/p1p2ppp/2p5/2P5/6nq/2NB4/PPPP2PP/R1BQR1K1 w - - 0 13", null, new[] { "g2h3", "e1e4" },
            Description = "Avoid Mate in 4, fails with MiniMax depth 4")]

        [TestCase("q5k1/2p5/1bb1pQ2/1p6/1P6/5N2/P4PPP/3RK2R b K - 0 1", null, new[] { "a8a2" },
            Description = "Avoid Mate in 3, seen with NegaMax depth 4 (but weirdly not with AlphaBeta depth 4)")]

        [TestCase("r2qr1k1/ppp2ppp/5n2/2nPp3/1b4b1/P1NPP1P1/1P2NPBP/R1BQ1RK1 b - - 0 1", new[] { "b4c3", "g4e2" }, new[] { "b4a5" },
            Description = "Fails with simple MiniMax depth 3 and 4: b4a5 is played")]

        [TestCase("r2qkb1r/ppp2ppp/2n2n2/3pp3/8/PPP3Pb/3PPP1P/RNBQK1NR w KQkq - 0 1", new[] { "g1h3" },
            Description = "Used to fail")]

        [TestCase("8/n2k4/bpr4p/1q1p4/4pp1b/P3P1Pr/R2K4/1r2n1Nr w - - 0 1", new[]
        {
            "a3a4",
            "a2a1", "a2b2", "a2c2",
            "e3f4", "f3g4",
            "g3f4", "g3g4", "g3h4",
            "g3f4", "g3g4", "g3h4",
            "g1h3", "g1f3", "g1e2"
        }, Description = "Used to return an illegal move in the very first versions")]

        public void GeneralRegression(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth: 5);
        }

        [TestCase("r1bq2k1/1pp1n2p/2nppr1Q/p7/2PP2P1/5N2/PP3P1P/2KR1B1R w - - 0 15", new[] { "h6f6" },
            Category = "LongRunning", Explicit = true, Description = "AlphaBeta/NegaMax depth 5 spends almost 3 minutes with a simple retake")]
        public void SlowRecapture(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth: 5);
        }

        [TestCase("6k1/1R6/5Kn1/3p1N2/1P6/8/8/3r4 b - - 10 37", new[] { "g6f8" }, new[] { "g6f4" },
            Category = "LongRunning", Explicit = true, Description = "Avoid mate in 4 https://lichess.org/XkZsoXLA#74",
            Ignore = "Not good enough yet")]
        public void AvoidMate(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
        }

        [TestCase(5)]
        [TestCase(6)]
        public void KeepNonGoodQuiescenceMoves(int depth)
        {
            const string fen = Constants.InitialPositionFEN;
            var engine = GetEngine(fen);
            var goCommand = new GoCommand($"go depth {depth}");

            var bestResult = engine.BestMove(goCommand);

            switch (depth)
            {
                case 5:
                    Assert.AreNotEqual("f6e4", bestResult.Moves[4].UCIString());
                    break;
                case 6:
                    Assert.AreNotEqual("f3e5", bestResult.Moves[5].UCIString());
                    break;
                default:
                    Assert.True(false);
                    break;
            }

            Assert.AreEqual(depth, bestResult.Moves.Count);
        }

        [TestCase(5, Constants.InitialPositionFEN, "d8d5")]
        [TestCase(5, "rq2k2r/ppp2pb1/2n1pnpp/1Q1p1b2/3P1B2/2N1PNP1/PPP2PBP/R3K2R w KQkq - 0 1", "e5f4")]
        public void TrashInPVTable(int depth, string fen, string notExpectedMove)
        {
            var goCommand = new GoCommand($"go depth {depth}");

            var engine = GetEngine(fen);
            var bestResult = engine.BestMove(goCommand);

            if (bestResult.Moves.Count > depth)
            {
                Assert.AreNotEqual(notExpectedMove, bestResult.Moves[depth].UCIString());
            }
        }

        [Test]
        public void TrashInPV()
        {
            const string positionCommand = "position startpos moves c2c4";
            var goCommand = new GoCommand("go depth 5");

            var engine = GetEngine();

            engine.AdjustPosition(positionCommand);
            Assert.DoesNotThrow(() => engine.BestMove(goCommand));
        }

        [TestCase("3rk2r/ppq1pp2/2p1n1pp/7n/4P3/2P1BQP1/P1P2PBP/R3R1K1 w k - 0 18", null, new[] { "e3a7" },
            Category = "LongRunning", Explicit = true, Description = "At depth 3 White takes the pawn",
            Ignore = "Not good enough yet")]
        [TestCase("r1bqk2r/ppp2ppp/2n1p3/8/Q1pP4/2b2NP1/P3PPBP/1RB2RK1 b kq - 1 10", null, new[] { "c3d4" },
            Category = "LongRunning", Explicit = true, Description = "It failed at depth 6 in https://lichess.org/nZVw6G5D/black#19",
            Ignore = "Not good enough yet")]
        [TestCase("r1bqkb1r/ppp2ppp/2n1p3/3pP3/3Pn3/5P2/PPP1N1PP/R1BQKBNR b KQkq - 0 1", null, new[] { "f8b4" },
            Category = "LongRunning", Explicit = true, Description = "It failed at depth 5 in https://lichess.org/rtTsj9Sr/black",
            Ignore = "Not good enough yet")]
        public void GeneralFailures(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
        }
    }
}
