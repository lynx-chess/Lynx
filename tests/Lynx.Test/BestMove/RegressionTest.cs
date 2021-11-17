using Lynx.UCI.Commands.GUI;
using NUnit.Framework;

namespace Lynx.Test.BestMove
{
    public class RegressionTest : BaseTest
    {
        [TestCase("r1bq2k1/1pp1n2p/2nppr1Q/p7/2PP2P1/5N2/PP3P1P/2KR1B1R w - - 0 15", new[] { "h6f6" },
            Description = "AlphaBeta/NegaMax depth 5 spends almost 3 minutes with a simple retake")]
        public void SlowRecapture(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
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
