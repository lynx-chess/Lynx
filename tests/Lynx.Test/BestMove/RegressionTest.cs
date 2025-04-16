using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NUnit.Framework;

namespace Lynx.Test.BestMove;

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

    [TestCase("7k/1Q4r1/2q1B3/1P2QNN1/8/R7/nN6/K1R5 b - - 0 1", new[] { "c6c1" },
        Description = "Get stalemated vs winning a queen")]

    public void GeneralRegression(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth: 5);
    }

    [TestCase("r1bq2k1/1pp1n2p/2nppr1Q/p7/2PP2P1/5N2/PP3P1P/2KR1B1R w - - 0 15", new[] { "h6f6" },
        Description = "AlphaBeta/NegaMax depth 5 spends almost 3 minutes with a simple retake")]
    public void SlowRecapture(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth: 5);
    }

    [Explicit]
    [Category(Categories.NotGoodEnough)]
    [TestCase("6k1/1R6/5Kn1/3p1N2/1P6/8/8/3r4 b - - 10 37", new[] { "g6f8" }, new[] { "g6f4" },
        Description = "Avoid mate in 4 https://lichess.org/XkZsoXLA#74")]
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
            case 5 when bestResult.Moves.Length >= 5:
                Assert.AreNotEqual("f6e4", bestResult.Moves[4].UCIString());
                break;
            case 6 when bestResult.Moves.Length >= 6:
                Assert.AreNotEqual("f3e5", bestResult.Moves[5].UCIString());
                break;
        }
    }

    [TestCase(5, Constants.InitialPositionFEN, "d8d5")]
    [TestCase(5, "rq2k2r/ppp2pb1/2n1pnpp/1Q1p1b2/3P1B2/2N1PNP1/PPP2PBP/R3K2R w KQkq - 0 1", "e5f4")]
    public void TrashInPVTable(int depth, string fen, string notExpectedMove)
    {
        var goCommand = new GoCommand($"go depth {depth}");

        var engine = GetEngine(fen);
        var bestResult = engine.BestMove(goCommand);

        if (bestResult.Moves.Length > depth)
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

    [Explicit]
    [Category(Categories.NotGoodEnough)]
    [TestCase("3rk2r/ppq1pp2/2p1n1pp/7n/4P3/2P1BQP1/P1P2PBP/R3R1K1 w k - 0 18", null, new[] { "e3a7" },
        Description = "At depth 3 White takes the pawn")]
    [TestCase("r1bqk2r/ppp2ppp/2n1p3/8/Q1pP4/2b2NP1/P3PPBP/1RB2RK1 b kq - 1 10", null, new[] { "c3d4" },
        Description = "It failed at depth 6 in https://lichess.org/nZVw6G5D/black#19")]
    [TestCase("r1bqkb1r/ppp2ppp/2n1p3/3pP3/3Pn3/5P2/PPP1N1PP/R1BQKBNR b KQkq - 0 1", null, new[] { "f8b4" },
        Description = "It failed at depth 5 in https://lichess.org/rtTsj9Sr/black")]
    public void GeneralFailures(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
    }

    /// <summary>
    /// Verifies that with both <see cref="Configuration.EngineSettings.MinDepth"/>
    /// and <see cref="Configuration.EngineSettings.DepthWhenLessThanMinMoveTime"/>
    /// the positions that result as a consequence of the given position commands
    /// aren't considered as drawn
    /// </summary>
    [TestCase("position startpos moves" +
        " c2c4 e7e5 b1c3 g8f6 g1f3 b8c6 e2e4 f8b4 d2d3 d7d6 a2a3 b4c5 b2b4 c5b6 c1e3" +
        " b6e3 f2e3 a7a6 b4b5 a6b5 c4b5 c6a7 d3d4 e5d4 d1d4 c7c5 d4d2 e8g8 e1c1 f8e8" +
        " e4e5 f6g4 e5d6 g4e3 d6d7 c8d7 d2d7 d8a5 d7d2 e3d1 c3d1 a7b5 d2a5 a8a5 a3a4" +
        " b5d4 f3d4 c5d4 f1b5 e8e5 d1f2 e5f5 h1e1 a5a8 f2e4 f5d5 g2g4 d4d3 c1d2 h7h5" +
        " g4h5 d5h5 e1h1 a8d8 d2c3 h5h3 e4f2 d8c8 c3d4 d3d2 f2h3 c8c1 h3f2 c1h1 b5e2" +
        " h1h2 d4e3 f7f5 f2d1 f5f4 e3d3 g7g5 d1c3 f4f3 e2f3 h2h3 d3e2 g8g7 c3d1 h3h2" +
        " e2e3 h2h3 e3e2 b7b6 f3g2 h3a3 g2c6 a3a2 d1e3 g7f6 e3c4 g5g4 c4b6 f6e5 c6a8" +
        " e5d4 a8d5 a2a1 e2d2 d4c5 d5e6 c5b6 e6g4 a1a4 g4e6 b6c5 d2d3 a4a1 e6f5 c5d6" +
        " d3e4 a1d1 f5c8 d1e1 e4f4 d6d5 c8b7 d5d4 b7g2 e1e2 f4f3 e2a2 f3g3 d4c5 g3f3" +
        " c5d5 f3g3 d5d6 g3f3 d6e5 f3g3 e5d6 g3f3 d6e5 g2h3 a2b2 f3g3 e5d5 g3f3 d5d4" +
        " h3g2 d4d5 f3g3 d5d6 g3f3 d6e6 f3g3 e6d6 g2a8 b2a2 a8b7 a2b2 b7a8 d6c7 g3f4" +
        " c7b8 f4e5 b8a8 e5d6 a8a7 d6c7 a7a8 c7c8 a8a7 c8c7")]

    [TestCase("position startpos moves" +
        " e2e4 e7e5 g1f3 b8c6 f1c4 f8c5 e1g1 g8f6 d2d3 d7d6 c2c3 a7a5 f1e1 e8g8 b1d2" +
        " c5b6 b2b3 f8e8 c1b2 c8g4 h2h3 g4h5 g2g4 h5g6 g4g5 d6d5 g5f6 d5c4 d2c4 d8f6" +
        " c4b6 c7b6 c3c4 g6h5 e1e3 c6d4 g1g2 f6g6 g2h2 d4f3 e3f3 g6d6 a1c1 a8d8 c1c3" +
        " f7f5 d1e2 h5f3 e2f3 f5e4 f3e4 d6h6 c3c2 h6f4 e4f4 e5f4 d3d4 e8e1 c2d2 g7g5" +
        " d4d5 d8d6 a2a4 h7h5 f2f3 e1e3 b2a3 d6d7 d2g2 e3b3 a3c1 d7g7 c1b2 g7g6 h3h4" +
        " b3f3 h4g5 f3e3 b2f6 f4f3 g2f2 e3e4 f2f3 e4c4 f3b3 c4a4 d5d6 a4h4 b3h3 h4e4" +
        " h3h5 e4e2 h2g1 e2e1 g1g2 e1d1 h5h8 g8f7 h8h7 f7e6 h7b7 d1d6 g2f3 e6d5 f3f4" +
        " d5c6 b7b8 c6d7 f4f5 d7c7 b8e8 d6f6 g5f6 g6g2 f6f7 g2f2 f5g6 f2g2 g6f5 g2f2" +
        " f5e6 a5a4 f7f8q f2f8 e8f8 b6b5 e6e5 c7b7 f8f7 b7a6 f7f8 b5b4 e5d5 b4b3 f8b8" +
        " a6a5 d5c5 a5a6 b8a8 a6b7 a8a4 b7c8 a4b4 c8d7 b4b3 d7e6 b3b8 e6f5 b8a8 f5e6" +
        " c5c4 e6d6 a8d8 d6c7 d8e8 c7d6 e8a8 d6e6 c4c5 e6f5 c5d5 f5f6 a8e8 f6f7 e8a8" +
        " f7g7 d5d4 g7f7 d4e5 f7e7 e5d5 e7f6 a8e8 f6f7 e8b8 f7f6 b8a8 f6g7 a8b8 g7f6" +
        " b8d8 f6f7 d5d6 f7g6 d6d5 g6f7 d5e5 f7e7 d8d6 e7f7 d6d8 f7e7")]

    [TestCase("position startpos moves" +
        " g1f3 d7d5 d2d4 g8f6 c1f4 c8f5 e2e3 e7e6 f1d3 f5g6 b1c3 f8d6 e1g1 e8g8 h2h3" +
        " g6h5 f4g5 d6e7 g2g4 h5g6 d3g6 f7g6 f1e1 b8c6 g5f4 f8e8 f3e5 c6e5 d4e5 f6e4" +
        " c3e4 d5e4 d1e2 c7c5 a1d1 d8b6 c2c4 g6g5 f4g3 a8d8 d1d8 e7d8 e1d1 b6c6 d1d6" +
        " c6c8 f2f3 d8c7 d6d2 c8b8 f3e4 c7e5 g3e5 b8e5 e2g2 e5c7 b2b3 e6e5 g2e2 b7b6" +
        " a2a4 a7a5 e2d3 c7c8 d3d6 h7h5 d6d5 g8h7 d5d7 c8d7 d2d7 h5g4 h3g4 e8e6 d7d5" +
        " g7g6 g1g2 h7g7 g2g1 g7g8 d5d7 g8f8 g1f2 f8e8 d7b7 e8d8 b7g7 e6d6 f2g1 d6d1" +
        " g1g2 d1d6 g2g1 d6d1 g1f2 d1d2 f2g3 d2d3 g3f3 d3d6 f3g2 d6d2 g2f3 d2d6 g7f7" +
        " d8e8 f7a7 d6d7 a7a8 d7d8 a8a7 d8d7 a7a8 d7d8 a8a6 d8d6 f3e2 e8e7 a6a8 e7d7" +
        " e2f3 d7e6 f3g3 e6d7 g3f3 d6f6 f3g3 d7c7 a8a5 b6a5 b3b4 a5b4 a4a5 b4b3 a5a6" +
        " f6a6 g3f3 c7b8 f3g3 b3b2 g3h3 b2b1q h3g3 b1e4 g3h3 e4c4 h3g3 c4g4 g3g4 c5c4" +
        " g4g5 e5e4 g5g4 g6g5 g4g5 c4c3 g5f5 b8a8 f5e4 c3c2 e4d5")]

    [TestCase("position startpos moves" +
        " e2e4 c7c5 g1f3 d7d6 d2d4 c5d4 f3d4 g8f6 b1c3 a7a6 c1g5 e7e6 f2f4 d8b6 d1d2" +
        " b6b2 a1b1 b2a3 e4e5 d6e5 f4e5 f6d5 c3d5 e6d5 c2c4 b8d7 e5e6 d7f6 g5f6 g7f6" +
        " c4d5 a3c5 e6f7 e8f7 d4e6 c5e7 d2e2 b7b5 e6f8 e7f8 b1c1 f8d8 e2h5 f7g8 c1d1" +
        " c8b7 h5g4 g8f8 g4b4 f8f7 d5d6 h8e8 e1f2 d8b6 b4d4 b6d4 d1d4 a8d8 f1d3 e8e6" +
        " d6d7 b7c6 h1d1 e6e7 d3f5 h7h6 d4d6 e7e5 d6c6 e5f5 f2g1 f7e7 c6c7 f5e5 g1f2" +
        " a6a5 g2g4 e5e4 f2f3 e4c4 c7a7 c4c3 f3e4 c3c2 a7a5 d8d7 d1d7 e7d7 h2h4 d7c7" +
        " a5b5 c2e2 e4f5 e2f2 f5e6 f2a2 e6f6 a2g2 b5h5 g2g4 h5h6 g4g2 f6e5 g2e2 e5d5" +
        " e2d2 d5e5 d2e2 e5d5 e2d2 d5e4 d2e2 e4d4 e2d2 d4e4 d2e2 e4d4 e2c2 h6h8 c2d2" +
        " d4e5 d2e2 e5d5 e2d2 d5e5 d2e2 e5d4 c7d6 h4h5 e2d2 d4e4 d2e2 e4f5 e2f2 f5e4" +
        " f2e2 e4d4 e2d2 d4c4 d6e5 h8e8 e5f5 e8h8 d2h2 c4d5 f5g5 h8g8 g5h6 d5d6 h2h5" +
        " g8a8 h6g7 d6e6 g7g6 e6d6 h5h2 d6d5 g6g7 d5d6 g7f6 d6d5 h2d2 d5e4 f6e6 e4e3" +
        " d2b2 e3d4 e6d6 d4e4 d6c5 a8c8 c5d6 c8a8 d6c7 e4e5 b2d2 a8f8 c7b7 e5e6 b7c7" +
        " e6e5 d2e2 e5d4 c7d6 f8a8 e2d2 d4c3 d2f2 c3d4 f2d2 d4c4 d6c7 c4c5 c7b7 a8e8" +
        " b7a6 c5c6 a6a5 c6c7 a5a6 c7b8 a6b6 b8a8 b6c7 a8a7 c7d7 a7a8 d7e8 a8b8 e8d8" +
        " b8a8")]

    [TestCase("position startpos moves" +
        " g1f3 d7d5 d2d4 g8f6 g2g3 b8c6 f1g2 e7e6 b1c3 f8d6 e1g1 e8g8 f1e1 a8b8 e2e4 d5e4 c3e4 f6e4 e1e4 d6e7" +
        " b2b3 b7b5 c1b2 c8b7 e4e3 e7f6 f3e5 c6e7 g2b7 b8b7 e5g4 e7f5 g4f6 d8f6 e3e5 f5e7 d1f1 e7c6 f1g2 b7b6" +
        " e5e4 f8d8 c2c3 f6f5 g3g4 f5g6 a1e1 b6a6 a2a3 c6a5 g2g3 d8c8 g3d3 a6b6 d3d1 f7f5 e4e5 g6g4 d1g4 f5g4" +
        " e5e6 b6e6 e1e6 a5b3 e6e5 c7c5 e5e2 b3a5 d4c5 c8c5 e2e8 g8f7 e8a8 c5c7 a8b8 a7a6 g1g2 c7c6 b8h8 a5c4" +
        " b2c1 h7h6 h8a8 g7g5 a8a7 f7g6 a7a8 c6f6 a8g8 g6h5 g8e8 f6f3 c1e3 c4a3 e8d8 a3c2 e3d2 h5g6 g2g1 f3f6" +
        " d8g8 g6f5 g8d8 c2a3 d2e3 a3c4 e3d4 c4e5 g1g2 h6h5 h2h3 g4h3 g2h3 f6e6 h3g2 f5e4 d4e3 g5g4 d8h8 e6c6" +
        " e3d4 e5g6 h8e8 e4f5 g2g1 h5h4 g1h2 c6e6 e8a8 f5f4 d4e3 f4f3 h2g1 g4g3 a8a7 g6e7 a7a8 e7d5 a8f8 f3g4" +
        " f8g8 g4h3 g1f1 d5e3 f2e3 e6e3 g8g5 e3c3 f1e2 g3g2 e2f2 b5b4 g5g2 c3c2 f2e3 c2g2 e3d4 b4b3 d4d5 b3b2" +
        " d5e5 b2b1q e5f4 b1d3 f4e5 g2g6 e5f4 d3e2 f4f5 e2e6 f5f4 g6g4 f4f3 e6e8 f3f2 g4f4 f2g1 f4f6 g1h1 e8e6" +
        " h1g1 e6e5 g1h1 e5d5 h1g1 d5d4 g1h1 d4e4 h1g1 f6g6 g1f1 e4d3 f1e1 g6g1 e1f2 g1g2 f2e1 g2g6 e1f2 d3d2" +
        " f2f3 d2e1 f3f4 e1e2 f4f5 e2e6 f5f4 g6g4 f4f3 e6e8 f3f2 g4f4 f2g1 f4f6 g1h1 e8e6 h1g1 e6e5 g1h1 e5d5",
        Ignore = "Now that we detect 2 move repetitions, this should fail")]
    public void FalseDrawnPositions(string positionCommand)
    {
        var engine = GetEngine();
        engine.AdjustPosition(positionCommand);

        var bestMove = engine.BestMove(new GoCommand($"go depth {Engine.DefaultMaxDepth}"));
        Assert.NotZero(bestMove.Score);

        //engine.AdjustPosition(positionCommand);
        //bestMove = engine.BestMove(new GoCommand($"go depth {Configuration.EngineSettings.DepthWhenLessThanMinMoveTime}"));
        //Assert.NotZero(bestMove.Evaluation);
    }

    [TestCase("position startpos moves" +
        " c2c4 e7e6 b1c3 g8f6 g1f3 f8b4 e2e3 e8g8 d2d4 c7c5 f1d3 d7d5 e1g1 b8c6 c4d5" +
        " e6d5 d4c5 b4c5 b2b3 f8e8 c1b2 b7b6 f1e1 c8b7 d1b1 h7h5 a2a3 a7a5 e3e4 f6g4" +
        " e1e2 d5d4 c3d5 g4e5 f3e5 c6e5 d3b5 e8e6 d5f4 e6h6 f4d3 e5d3 b1d3 h6d6 e4e5" +
        " d6g6 f2f3 d8d5 a1e1 d5f3 d3g6 f3e2 b5e2 f7g6 e2d3 g6g5 a3a4 a8d8 e5e6 d8e8" +
        " e1e5 h5h4 d3g6 e8e7 g6f7 g8f8 e5g5 b7c8 g5g6 e7a7 g2g3 h4h3 g6g5 a7f7 e6f7" +
        " f8f7 g5h5 d4d3 h5c5 b6c5 g1f2 c8e6 f2e3 f7g8 e3d3 e6b3 b2c3 c5c4 d3d4 b3a4" +
        " d4c4 a4d7 c3a5 g7g5 c4d4 g5g4 d4e5 d7c8 e5f4 c8e6 a5c7 e6c8 f4e4 g8g7 c7e5" +
        " g7f7 e5b2 f7e6 e4f4 e6d5 f4e3 c8d7 e3f4 d7e6 f4e3 d5d6 e3e4 e6d5 e4f4 d5e6" +
        " f4e3 e6d5 e3f4 d5f3 f4f5 d6d5 f5f4 d5d6 f4f5 d6d5 f5f4 f3d1 f4f5 d1e2 f5f4" +
        " d5d6 f4e4 d6e6 e4f4 e6d5 f4f5 d5c5 f5f6 c5b6 f6e7 b6a7 e7d8 a7a8 d8e7 a8b8" +
        " e7f8 b8a8 f8e7 a8b8 e7f8 b8a8")]
    public void FalseDrawnPositionBy50MovesRule(string positionCommand)
    {
        var engine = GetEngine();
        engine.AdjustPosition(positionCommand);
        Assert.False(engine.Game.Is50MovesRepetition());
        var bestMove = engine.BestMove(new GoCommand("go depth 1"));

        engine.AdjustPosition(positionCommand + " " + bestMove.BestMove.UCIString());
        Assert.IsFalse(engine.Game.Is50MovesRepetition());
    }

    // 8/2Q4p/4pppk/4P3/8/7P/q4PK1/1q6 w - - 0 1 - || PV Qc7 Kh6 e5 b1=Q Qxb1 Qxb1 exf6, with b1=Q as the first invalid move there
    [TestCase("position startpos moves d2d4 g8f6 c2c4 e7e6 g1f3 d7d5 b1c3 f8e7 c1f4 e8g8" +
        " e2e3 c7c5 d4c5 e7c5 a2a3 a7a6 d1d2 d5c4 f1c4 b7b5 c4d3 c8b7 b2b4 b7f3 b4c5 f3g2" +
        " h1g1 g2f3 g1g3 f3c6 f4h6 g7g6 h6f8 d8f8 a3a4 b5b4 c3a2 b4b3 a2c1 f8c5 c1b3 c5d5" +
        " b3a5 d5h1 d3f1 c6d5 d2c3 b8d7 c3c7 h1e4 f1c4 e4c2 a1a2 c2c1 e1e2 d7e5 c7e5 d5c4" +
        " a5c4 c1c4 e2f3 c4a2 e5f6 a2a4 f3g2 a8c8 g3f3 a4d7 f6e5 d7b5 e5d6 a6a5 d6e7 c8f8" +
        " e7d6 a5a4 e3e4 b5c4 f3e3 f8c8 h2h3 c4b5 e3f3 b5g5 f3g3 g5a5 g3d3 a5g5 d3g3 g5b5" +
        " g3f3 b5a5 f3d3 a5a8 d6b4 c8b8 b4c4 a4a3 d3d2 b8b2 d2b2 a3b2 c4b4 a8a2 b4b8 g8g7" +
        " b8e5 g7g8 e5b8 g8g7 b8e5 f7f6 e5c7 g7g8 c7b8 g8f7 b8c7 f7g8 c7b8 g8g7")]
    public void InvalidPV(string positionCommand)
    {
        var engine = GetEngine();
        engine.AdjustPosition(positionCommand);

        var bestMove = engine.BestMove(new GoCommand($"go depth {5}"));
        Assert.Zero(bestMove.Score);
        Assert.AreEqual(1, bestMove.Moves.Length);
        Assert.AreEqual("b8c7", bestMove.BestMove.UCIString());
    }

    // pv h2h1q a5a6 d5c4 a6a7 c4b4 c5c6 h1g1 b6a6, playing h2h1q again
    [TestCase("position startpos moves e2e4 c7c6 d2d4 d7d5 e4e5 c8f5 g1f3 e7e6 f1e2 c6c5 c1e3" +
        " c5d4 f3d4 g8e7 b1c3 f5g6 h2h4 h7h5 e2b5 b8d7 e3f4 a7a6 b5d3 g6d3 d1d3 e7g6 d4e6 g6f4" +
        " e6f4 d7e5 d3e2 d8d6 c3d5 g7g6 e1c1 f8h6 h1e1 f7f6 d5f6 d6f6 e2e5 f6e5 e1e5 e8f8 g2g3" +
        " a8e8 e5e3 h6f4 g3f4 h8h7 d1d6 h7f7 e3e8 f8e8 d6g6 f7f4 g6g8 e8f7 g8b8 b7b5 b8b6 f4a4" +
        " f2f3 a4h4 b6a6 h4h1 c1d2 h1h2 d2c3 h2f2 b2b3 h5h4 a6h6 f2f3 c3b4 f3f4 b4b5 f7g7 h6h5" +
        " g7f6 c2c4 f4f5 h5f5 f6f5 c4c5 f5e6 a2a4 h4h3 b5b6 h3h2 b3b4 h2h1q a4a5")]

    public void InvalidPV2(string positionCommand)
    {
        var engine = GetEngine();
        engine.AdjustPosition(positionCommand.AsSpan()[..^10]);  // 8/8/1K2k3/2P5/PP6/8/7p/8 b - - 0 46, ready to promote

        var bestMove = engine.BestMove(new GoCommand($"go depth {7}"));
        Assert.AreEqual("h2h1q", bestMove.BestMove.UCIString());

        engine.AdjustPosition(positionCommand);
        bestMove = engine.BestMove(new GoCommand($"go depth {7}"));
        Assert.AreNotEqual("h2h1q", bestMove.BestMove.UCIString());
    }

    [Explicit]
    [Category(Categories.LongRunning)]
    [TestCase(Constants.KillerTestPositionFEN)]
    [TestCase(Constants.TrickyTestPositionFEN)]
    public void PVTableCrash(string fen)
    {
        const int depthWhenMaxDepthInQuiescenceIsReached = 7;
        var engine = GetEngine(fen);

        var bestMove = engine.BestMove(new GoCommand($"go depth {depthWhenMaxDepthInQuiescenceIsReached}"));
        Assert.AreEqual(depthWhenMaxDepthInQuiescenceIsReached, bestMove.Depth);
    }

    [Explicit]
    [Category(Categories.LongRunning)]
    [Test]
    public void PonderingCrash()
    {
        var engine = GetEngine();
        engine.AdjustPosition("position startpos moves" +
            " e2e4 c7c5 g1f3 d7d6 d2d4 c5d4 f3d4 g8f6 b1c3 a7a6 f2f3 e7e5 d4b3 c8e6 c1e3 h7h5 c3d5 e6d5 e4d5 b8d7" +
            " f1e2 g7g6 d1d2 f8g7 c2c4 d8c7 e1c1 e8c8 h1e1 d7b6 d2c2 h5h4 a2a3 h4h3 g2h3 h8h3 e2f1 h3h5 h2h3 c8b8" +
            " c2c3 h5f5 b3d2 d8c8 c1b1 b6d5 c4d5 c7c3 b2c3 f6d5 d2e4 d5c3 e4c3 c8c3 b1b2 c3c6 b2b3 b8c8 a3a4 f5f6" +
            " h3h4 c6c7 f1h3 c8b8 h3g4 c7c6 e3g5 f6e6 g4e6 f7e6 g5e7 d6d5 e1g1 g7h6 g1g6 h6e3 a4a5 e3d4 f3f4 b7b6" +
            " a5b6 c6b6 b3a4 b6c6 e7b4 b8b7 f4e5 d4e5 d1e1 e5c7 e1e6 c6c4 e6a6 c4h4 a6e6 h4h1 a4b5 d5d4 g6g7 h1c1" +
            " g7d7 b7c8 d7d4 c1a1 e6e8 c8b7 d4d7 a1c1 b4a3 c1c2 b5a4 b7c6 d7e7 c2d2 a3c1 d2c2 c1g5 c2c5 g5e3 c5d5" +
            " e8a8 d5d3 a8a6 c6d5 e7d7 d5e4 d7c7 d3e3 a6a8 e4d5 a8d8 d5e6 a4b5 e3b3 b5c4 b3b1 d8e8 e6f6 c4d3 b1d1" +
            " d3c2 d1a1 c7d7 f6f5 d7f7 f5g6 e8e7 g6g5 c2d3 a1a2 d3d4 a2a4 d4e5 a4a5 e5d6 a5a2 f7g7 g5f4 g7f7 f4g4" +
            " f7g7 g4f4 g7f7 f4g4 e7b7 g4g5 d6e5 g5g4 f7f4 g4g5 b7g7 g5h5 f4f7 a2e2 e5d5 h5h6 g7h7 h6g5 h7g7 g5h5" +
            " d5d4 h5h6 f7a7 h6h5 d4d5 h5h6 d5d6 h6h5 a7f7 h5h6 d6d5 h6h5 f7a7 h5h6 d5d4 h6h5 g7g8 h5h6 d4d5 e2b2" +
            " d5d6 b2f2 a7a8 h6h7 d6d5 h7h6 d5e5 h6h7 e5d5 h7h6 d5e6 h6h5 g8g1 f2e2 e6f7 e2f2 f7g7 f2f8");

        var searchResult = engine.BestMove(new($"go depth {Engine.DefaultMaxDepth}"));

        engine.AdjustPosition("position startpos moves" +
            " e2e4 c7c5 g1f3 d7d6 d2d4 c5d4 f3d4 g8f6 b1c3 a7a6 f2f3 e7e5 d4b3 c8e6 c1e3 h7h5 c3d5 e6d5 e4d5 b8d7" +
            " f1e2 g7g6 d1d2 f8g7 c2c4 d8c7 e1c1 e8c8 h1e1 d7b6 d2c2 h5h4 a2a3 h4h3 g2h3 h8h3 e2f1 h3h5 h2h3 c8b8" +
            " c2c3 h5f5 b3d2 d8c8 c1b1 b6d5 c4d5 c7c3 b2c3 f6d5 d2e4 d5c3 e4c3 c8c3 b1b2 c3c6 b2b3 b8c8 a3a4 f5f6" +
            " h3h4 c6c7 f1h3 c8b8 h3g4 c7c6 e3g5 f6e6 g4e6 f7e6 g5e7 d6d5 e1g1 g7h6 g1g6 h6e3 a4a5 e3d4 f3f4 b7b6" +
            " a5b6 c6b6 b3a4 b6c6 e7b4 b8b7 f4e5 d4e5 d1e1 e5c7 e1e6 c6c4 e6a6 c4h4 a6e6 h4h1 a4b5 d5d4 g6g7 h1c1" +
            " g7d7 b7c8 d7d4 c1a1 e6e8 c8b7 d4d7 a1c1 b4a3 c1c2 b5a4 b7c6 d7e7 c2d2 a3c1 d2c2 c1g5 c2c5 g5e3 c5d5" +
            " e8a8 d5d3 a8a6 c6d5 e7d7 d5e4 d7c7 d3e3 a6a8 e4d5 a8d8 d5e6 a4b5 e3b3 b5c4 b3b1 d8e8 e6f6 c4d3 b1d1" +
            " d3c2 d1a1 c7d7 f6f5 d7f7 f5g6 e8e7 g6g5 c2d3 a1a2 d3d4 a2a4 d4e5 a4a5 e5d6 a5a2 f7g7 g5f4 g7f7 f4g4" +
            " f7g7 g4f4 g7f7 f4g4 e7b7 g4g5 d6e5 g5g4 f7f4 g4g5 b7g7 g5h5 f4f7 a2e2 e5d5 h5h6 g7h7 h6g5 h7g7 g5h5" +
            " d5d4 h5h6 f7a7 h6h5 d4d5 h5h6 d5d6 h6h5 a7f7 h5h6 d6d5 h6h5 f7a7 h5h6 d5d4 h6h5 g7g8 h5h6 d4d5 e2b2" +
            " d5d6 b2f2 a7a8 h6h7 d6d5 h7h6 d5e5 h6h7 e5d5 h7h6 d5e6 h6h5 g8g1 f2e2 e6f7 e2f2 f7g7 f2f8" +
            $" {searchResult.BestMove.UCIString()} {searchResult.Moves[1].UCIString()}");

        searchResult = engine.BestMove(new($"go depth {Engine.DefaultMaxDepth}"));

        Assert.NotZero(searchResult.BestMove);
    }

    [TestCase("position fen r1bq1rk1/pp1nppbp/2p2np1/8/2QPP3/2N2N2/PP2BPPP/R1B1K2R w KQ - 0 1" +
        " moves c1f4 d7b6 c4b4 c8e6 e1g1 a7a5 b4c5 b6d7 c5a3 d8b6 e4e5 f6d5 c3d5 e6d5 f1d1 a8d8 f4d2 d8a8" +
        " a3e7 f8d8 d2c3 a5a4 d1d2 g7h6 d2c2 h6g7 c2c1 c6c5 c1d1 a8c8 e7d6 d5c6 d4c5 d7c5 d6e7 d8e8 e7h4" +
        " c6f3 e2f3 g7e5 c3e5 e8e5 h4d4 b6f6 d4b4 g8g7 d1d2 c8e8 a1f1 b7b6 d2d1 h7h6 b4c4 e5e7 b2b4 a4b3" +
        " a2b3 c5e4 b3b4 e4c3 d1d3 c3e4 b4b5 f6b2 c4d4 b2d4 d3d4 e7e5 f3e4 e5e4 d4e4 e8e4 f1b1 g7f6 f2f3" +
        " e4e5 g1f2 h6h5 b1b4 f6f5 h2h4 e5d5 f2e3 d5e5 e3f2 f7f6 g2g4 f5e6 f3f4 e5c5 f4f5 e6f7 f5g6 f7g6" +
        " g4h5 g6h5 f2e3 f6f5 e3d3 c5d5 d3e3 h5g6 b4b2 g6f6 b2b4 f6e5 e3e2 f5f4 h4h5 e5f5 h5h6 d5d6 b4c4" +
        " d6h6 c4c6 h6c6 b5c6 f5e6 e2d3 b6b5 d3e4 b5b4 c6c7 e6d7 c7c8q d7c8 e4f4 b4b3 f4e4 b3b2 e4f3" +
        " b2b1q f3f2 c8c7 f2e2 b1a2 e2d1 a2f2 d1c1 f2e2 c1b1 e2d3 b1a2 d3d2 a2b3 c7c6 b3c4 d2e3 c4b4 e3d3" +
        " b4a4 d3e3", Description = "FEN 8/8/2k5/8/K7/4q3/8/8 w - - 0 1")]

    [TestCase("position fen r2qk1nr/1ppb2bp/p1np1pp1/4p3/B2PP3/2P2N2/PP3PPP/RNBQR1K1 w kq - 0 1 moves c1e3" +
        " b7b5 a4c2 g8h6 h2h3 e8g8 b1d2 h6f7 d1e2 e5d4 c3d4 d8e7 a1c1 a8e8 c2b3 e7d8 e2d3 c6e7 c1c2 d6d5" +
        " c2c5 d7c6 e4d5 c6d5 a2a4 d5b3 d3b3 c7c6 b3d3 f7e5 d3c2 e5d7 c2b3 f8f7 c5c2 d7b6 a4b5 a6b5 e3f4" +
        " e7d5 e1e8 d8e8 f4d6 e8d7 b3a3 b6c8 d6c5 d7b7 d2e4 f7d7 a3b3 g8h8 c2e2 d7d8 e4c3 c8b6 h3h4 f6f5" +
        " f3e5 d5c3 b2c3 b6d5 c3c4 d5f4 e2a2 g7e5 d4e5 f4d3 a2d2 b7a6 c4b5 a6a1 b3d1 a1d1 d2d1 d8d5 e5e6" +
        " h8g7 b5c6 d5c5 d1d3 g7f6 e6e7 f6e7 d3d7 e7e6 d7h7 c5c6 h7a7 e6e5 a7e7 e5f6 e7b7 c6c2 g2g3 c2a2" +
        " g1g2 a2c2 g2f3 c2c4 f3e3 c4c3 e3f4 c3c4 f4f3 c4c3 f3e2 c3c2 e2e1 f6e6 e1f1 e6f6 f1g2 f6e6 b7a7" +
        " e6f6 g2f3 c2b2 a7c7 b2a2 f3e3 a2a3 e3d4 a3a2 c7c6 f6g7 d4e3 a2a3 e3f4 a3a4 f4f3 g7f7 h4h5 g6h5" +
        " c6h6 a4a2 h6h5 f7f6 f3e3 a2b2 h5h7 f6e6 e3f3 e6d5 h7e7 d5d6 e7e8 b2a2 f3e3 a2b2 f2f4 b2c2 e8e5" +
        " c2c5 e5c5 d6c5 e3f3 c5d6 g3g4 f5g4 f3g4 d6c6 f4f5 c6d7 g4g5 d7e7 g5g6 e7f8 g6f6 f8e8 f6g7 e8d7" +
        " f5f6 d7e6 f6f7 e6f5 f7f8q f5g5 f8c5 g5g4 g7f7 g4f4 c5b4 f4e3 b4c3 e3e4 c3b4 e4e3 f7f6 e3f3 b4d2" +
        " f3g4 f6f7 g4f3 d2d3 f3f2 f7f8 f2g1 d3e3 g1g2 e3e2 g2g3 f8e7 g3h3 e7d7 h3h4 e2g2 h4h5 d7e6 h5h6 e6d7",
        Description = "FEN 8/3K4/7k/8/8/8/6Q1/8 b - - 0 1")]
    public void DepthOverflow(string positionCommand)
    {
        var engine = GetEngine();

        engine.AdjustPosition(positionCommand);

        var result = engine.BestMove(new("go wtime 3600000 btime 3600000"));
        Assert.Less(result.DepthReached, Configuration.EngineSettings.MaxDepth);
    }

    [TestCase("4r3/5P1k/5K2/5N2/8/8/8/8 w - - 0 1", new[] { "f7e8b" },
        Description = "Position by Alex Brunetti, https://www.talkchess.com/forum3/viewtopic.php?f=2&t=31150&start=28")]
    public void BishopUnderpromotion(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
    }

    /// <summary>
    /// Unfortunately or not, it doesn't crash IDDFS, so the test is mostly useless, but let's have it in place just in case
    /// </summary>
    [Test]
    public void MaxDepthCrash()
    {
        var engine = GetEngine();

        engine.AdjustPosition("position startpos moves e2e4 c7c5 g1f3 g7g6 d2d4 f8g7 d4d5 d7d6 f1e2 c8g4 f3d2 g4e2 d1e2 g8f6 b1c3 f6d7 d2f3 d8b6 e1g1 b8a6 a2a4 a6c7 c3b5 c7b5 a4b5 h7h6 c2c4 e8g8 c1d2 a7a5 h2h3 g7b2 d2a5 a8a5 e2b2 f8a8 e4e5 a5a1 f1a1 a8a1 b2a1 d6e5 f3e5 d7e5 a1e5 b6d6 e5d6 e7d6 f2f4 f7f5 g1h2 g8f7 h2g3 f7f6 h3h4 g6g5 h4h5 f6g7 b5b6 g7f6 g3f3 g5g4 f3e3 f6f7 e3d2 f7e7 d2e2 e7f6 e2d3 f6g7 d3c2 g7f6 c2b3 f6f7 b3c3 f7f6 c3c2 f6e7 c2b2 e7f7 b2b1 f7f6 b1a1 f6e7 a1a2 e7f6 a2b1 f6e7 b1a1 e7f7 a1b2 f7f6 b2a2 f6e7 g2g3 e7f6 a2b2 f6e7 b2c3 e7f6 c3d3");

        engine.BestMove(new("go wtime 10000 btime 10000 winc 80 binc 80"));

        engine.AdjustPosition("position startpos moves e2e4 c7c5 g1f3 g7g6 d2d4 f8g7 d4d5 d7d6 f1e2 c8g4 f3d2 g4e2 d1e2 g8f6 b1c3 f6d7 d2f3 d8b6 e1g1 b8a6 a2a4 a6c7 c3b5 c7b5 a4b5 h7h6 c2c4 e8g8 c1d2 a7a5 h2h3 g7b2 d2a5 a8a5 e2b2 f8a8 e4e5 a5a1 f1a1 a8a1 b2a1 d6e5 f3e5 d7e5 a1e5 b6d6 e5d6 e7d6 f2f4 f7f5 g1h2 g8f7 h2g3 f7f6 h3h4 g6g5 h4h5 f6g7 b5b6 g7f6 g3f3 g5g4 f3e3 f6f7 e3d2 f7e7 d2e2 e7f6 e2d3 f6g7 d3c2 g7f6 c2b3 f6f7 b3c3 f7f6 c3c2 f6e7 c2b2 e7f7 b2b1 f7f6 b1a1 f6e7 a1a2 e7f6 a2b1 f6e7 b1a1 e7f7 a1b2 f7f6 b2a2 f6e7 g2g3 e7f6 a2b2 f6e7 b2c3 e7f6 c3d3 f6f7 d3c3");

        Assert.DoesNotThrow(() => engine.BestMove(new("go wtime 100000 btime 100000 winc 80 binc 80")));
    }

    [TestCase("8/8/4k3/3n1n2/5P2/8/3K4/8 b - - 0 12", null, new[] { "d5f4" },
        Description = "NN vs P, where knights can't take the pawn")]
    [TestCase("8/5R2/1n2RK2/8/8/7k/4r3/8 b - - 0 1", null, new[] { "e2e6" },
        Description = "RR vs RB, where if the side with the bishop exchanges the rooks, they lose")]
    public void PawnlessEndgames(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
    }

    [Explicit]
    [Category(Categories.LongRunning)]
    [TestCase("8/5pk1/6n1/3N4/4Q3/6P1/1q3PK1/8 b - - 0 1", 25)]
    [TestCase("8/p3k3/1b3p2/5Kp1/1p5p/1P2P2P/P3BP2/8 w - - 0 1", 20)]
    [TestCase("6k1/4Np2/8/5PP1/2n3K1/8/8/8 b - - 0 1", 20)]
    [TestCase("8/4Np1k/8/5PPK/2n5/8/8/8 b - - 0 1", 20)]
    [TestCase("1R6/8/8/5p2/5k2/r7/5K2/8 w - - 0 1", 20)]
    [TestCase("8/8/5k2/5p2/1R6/r7/6K1/8 w - - 0 1", 20)]
    [TestCase("8/8/5n2/7k/7p/5P2/6P1/4BK2 w - - 0 1", 20)]
    [TestCase("3B4/8/8/7k/7p/5P2/4n1PK/8 w - - 0 1", 20)]
    public void PVTableOverflow(string fen, int depth)
    {
        var engine = GetEngine();
        engine.AdjustPosition($"position fen {fen}");

        var result = engine.BestMove(new($"go depth {depth}"));

        Assert.AreEqual(depth, result.Depth);
    }

    [Test]
    public void HighSeldepthAtDepth2()
    {
        var engine = GetEngine();

        engine.AdjustPosition("position fen 8/4kpN1/8/4p1PK/1b2P3/5P2/8/8 b - - 60 109");
        var result = engine.BestMove(new("go wtime 6000 btime 6000 winc 3000 binc 3000"));
        Assert.Less(result.DepthReached, 3 * result.Depth, $"depth {result.Depth}, seldepth {result.DepthReached}");

        // It used to happen at the second repetition, info depth 2 seldepth 127
        engine.AdjustPosition("position fen 8/4kpN1/8/4p1PK/1b2P3/5P2/8/8 b - - 60 109");
        result = engine.BestMove(new("go wtime 6000 btime 6000 winc 3000 binc 3000"));
        Assert.Less(result.DepthReached, 3 * result.Depth, $"depth {result.Depth}, seldepth {result.DepthReached}");

        engine.AdjustPosition("position fen 8/4kpN1/8/4p1PK/1b2P3/5P2/8/8 b - - 60 109");
        result = engine.BestMove(new("go wtime 6000 btime 6000 winc 3000 binc 3000"));
        Assert.Less(result.DepthReached, 3 * result.Depth, $"depth {result.Depth}, seldepth {result.DepthReached}");
    }

    [Test]
    public void HighSeldepthAtDepth2_FixedDepth()
    {
        var engine = GetEngine();

        engine.AdjustPosition("position fen 8/4kpN1/8/4p1PK/1b2P3/5P2/8/8 b - - 60 109");
        var result = engine.BestMove(new("go wtime 6000 btime 6000 winc 3000 binc 3000"));
        Assert.Less(result.DepthReached, 3 * result.Depth, $"depth {result.Depth}, seldepth {result.DepthReached}");

        // It used to happen at the second repetition, info depth 2 seldepth 127
        engine.AdjustPosition("position fen 8/4kpN1/8/4p1PK/1b2P3/5P2/8/8 b - - 60 109");
        result = engine.BestMove(new("go depth 3"));
        Assert.Less(result.DepthReached, 16, $"depth {result.Depth}, seldepth {result.DepthReached}");

        engine.AdjustPosition("position fen 8/4kpN1/8/4p1PK/1b2P3/5P2/8/8 b - - 60 109");
        result = engine.BestMove(new("go depth 3"));
        Assert.Less(result.DepthReached, 16, $"depth {result.Depth}, seldepth {result.DepthReached}");
    }
}
