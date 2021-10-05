using Lynx.Model;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lynx.NUnit.Test
{
    public class EngineBestMoveTest : BaseTest
    {
        [TestCase("8/8/1p2k3/3bn3/3K4/8/7r/1q6 b - - 0 1", new[] { "b1d3" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 1")]
        public void Mate_in_1(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
        }

        [TestCase("8/pN3R2/1b2k1K1/n4R2/pp1p4/3B1P1n/3B1PNP/3r3Q w - -", new[] { "d2f4" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=114463")]
        [TestCase("KQ4R1/8/8/8/4N3/8/5p2/6bk w - -", new[] { "b8b2" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=1")]
        [TestCase("8/8/8/8/8/3n1N2/8/3Q1K1k w - -", new[] { "d1b1" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=1669")]
        [TestCase("RNBKRNRQ/PPPPPPPP/8/pppppppp/rnbqkbnr/8/8/8 b - -", new[] { "g4h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 2, https://gameknot.com/chess-puzzle.pl?pz=1630")]
        public void Mate_in_2(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
        }

        [TestCase("4rqk1/3R1prR/p1p5/1p2PQp1/5p2/1P6/P1B2PP1/6K1 w - -", new[] { "f5h3" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=111285")]
        [TestCase("1b6/2p2N1K/1p2Bp1p/3Pp2R/4kp1p/1N6/p1P1PPb1/r1R4r w - -", new[] { "h7h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=251481")]
        [TestCase("5B2/3Q4/8/7p/6N1/6k1/8/5K2 w - -", new[] { "f8h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=248898")]
        [TestCase("nb5B/1N1Q4/6RK/3pPP2/RrrkN3/2pP3b/qpP1PP2/3n4 w - -", new[] { "g6g3" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 3, https://gameknot.com/chess-puzzle.pl?pz=228148")]
        public void Mate_in_3(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
        }

        [TestCase("6k1/1R6/5K2/3p1N2/1P3n2/8/8/3r4 w - -", new[] { "f5h6" },
            Category = "LongRunning", Explicit = true, Description = "Mate in 4, https://gameknot.com/chess-puzzle.pl?pz=260253",
            Ignore = "Not good enough yet")]
        public void Mate_in_4(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
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

        [TestCase("3rk2r/ppq1pp2/2p1n1pp/7n/4P3/2P1BQP1/P1P2PBP/R3R1K1 w k - 0 18", null, new[] { "e3a7" },
            Category = "LongRunning", Explicit = true, Description = "At depth 3 White takes the pawn",
            Ignore = "Not good enough yet")]
        [TestCase("r1bqk2r/ppp2ppp/2n1p3/8/Q1pP4/2b2NP1/P3PPBP/1RB2RK1 b kq - 1 10", null, new[] { "c3d4" },
            Category = "LongRunning", Explicit = true, Description = "It failed at depth 6 in https://lichess.org/nZVw6G5D/black#19",
            Ignore = "Not good enough yet")]
        [TestCase("r1bqkb1r/ppp2ppp/2n1p3/3pP3/3Pn3/5P2/PPP1N1PP/R1BQKBNR b KQkq - 0 1", null, new[] { "f8b4" },
            Category = "LongRunning", Explicit = true, Description = "It failed at depth 5 in https://lichess.org/rtTsj9Sr/black",
            Ignore = "Not good enough yet")]

        [TestCase("6k1/1R6/5Kn1/3p1N2/1P6/8/8/3r4 b - - 10 37", new[] { "g6f8" }, new[] { "g6f4" },
            Category = "LongRunning", Explicit = true, Description = "Avoid mate in 4 https://gameknot.com/chess-puzzle.pl?pz=260253",
            Ignore = "Not good enough yet")]
        public void Regression(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
        }

        [TestCase("8/8/4NQ2/7k/2P4p/1q2P2P/5P2/6K1 b - - 5 52", new[] { "b3b1", "b3d1" },
    Category = "LongRunning", Explicit = true, Description = "Force stalemate - https://lichess.org/sM5ekwnW/black#103")]
        [TestCase("8/8/4NQ2/7k/2P4p/4P2P/5PK1/3q4 b - - 7 53", new[] { "d1h1", "d1g1", "d1f1" },
    Category = "LongRunning", Explicit = true, Description = "Force stalemate - https://lichess.org/sM5ekwnW/black#105")]
        public void ForceStaleMate(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
        }

        [Test]
        public void AvodTripleRepetitionWhenWinningPosition()
        {
            // Arrange

            // https://gameknot.com/chess-puzzle.pl?pz=247493
            const string fen = "r6k/p3b1pp/2pq4/Qp2n1NK/4P1P1/P3Br1P/1P2RP2/8 b - - 0 1";

            var mock = new Mock<ChannelWriter<string>>();

            mock
                .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            var engine = new Engine(mock.Object);
            engine.SetGame(new Game(fen));

            var repeatedMoves = new List<Move>
            {
                new ((int)BoardSquare.d6, (int)BoardSquare.c7, (int)Piece.q),
                new ((int)BoardSquare.h5, (int)BoardSquare.h4, (int)Piece.K),
                new ((int)BoardSquare.c7, (int)BoardSquare.d6, (int)Piece.q),
                new ((int)BoardSquare.h4, (int)BoardSquare.h5, (int)Piece.K),
                new ((int)BoardSquare.d6, (int)BoardSquare.c7, (int)Piece.q),
                new ((int)BoardSquare.h5, (int)BoardSquare.h4, (int)Piece.K)
            };

            Move movesThatAllowsRepetition = new((int)BoardSquare.c7, (int)BoardSquare.d6, (int)Piece.q);

            var sb = new StringBuilder($"position fen {fen} moves");
            foreach(var move in repeatedMoves)
            {
                sb.Append($" {move.UCIString()}");
                engine.AdjustPosition(sb.ToString());
            }

            Assert.AreEqual(repeatedMoves.Count, engine.Game.MoveHistory.Count);

            // Act
            var searchResult = engine.BestMove();
            var bestMoveFound = searchResult.BestMove;

            // Assert
            Assert.AreNotEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No triple repetition avoided");
            Assert.Less(searchResult.Evaluation, Position.CheckMateEvaluation - (20 * Position.DepthFactor), "Mate not detected");
        }

        [Test]
        public void ForceTripleRepetitionWhenLosingPosition()
        {
            // Arrange

            const string fen = "7B/8/7k/8/5KR1/8/5R2/K7 w - - 0 1";

            var mock = new Mock<ChannelWriter<string>>();

            mock
                .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            var engine = new Engine(mock.Object);
            engine.SetGame(new Game(fen));

            var repeatedMoves = new List<Move>
            {
                new ((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                new ((int)BoardSquare.h6, (int)BoardSquare.h7, (int)Piece.k),
                new ((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                new ((int)BoardSquare.h7, (int)BoardSquare.h6, (int)Piece.k),
                new ((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                new ((int)BoardSquare.h6, (int)BoardSquare.h7, (int)Piece.k),
                new ((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
            };

            Move movesThatAllowsRepetition = new((int)BoardSquare.h7, (int)BoardSquare.h6, (int)Piece.k);

            var sb = new StringBuilder($"position fen {fen} moves");
            foreach (var move in repeatedMoves)
            {
                sb.Append($" {move.UCIString()}");
                engine.AdjustPosition(sb.ToString());
            }

            Assert.AreEqual(repeatedMoves.Count, engine.Game.MoveHistory.Count);

            // Act
            var searchResult = engine.BestMove();
            var bestMoveFound = searchResult.BestMove;

            // Assert
            Assert.AreEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No triple repetition forced");
            Assert.AreEqual(0, searchResult.Evaluation, "No drawn position detected");
        }

        //    [TestCase("4n3/bp2k2p/p2p2pP/P1nP1pP1/N1P2P2/2BB4/3K4/8 w - - 9 42", new[] { "a5c5" },
        //Category = "LongRunning", Explicit = true, Description = "Foresee bishop sacrifice - https://lichess.org/training/5OHnu")]
        //    [TestCase("4n3/1p2k2p/p2p2pP/P1bP1pP1/2P2P2/2BB4/3K4/8 w - - 0 43", new[] { "c3f5" },
        //Category = "LongRunning", Explicit = true, Description = "Bishop sacrifice - https://lichess.org/VaY6zfHI/white#84")]
        //    public void Sacrifices(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        //    {
        //        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
        //    }
    }
}
