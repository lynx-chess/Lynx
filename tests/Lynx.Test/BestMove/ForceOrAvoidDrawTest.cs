using Lynx.Model;
using Moq;
using NUnit.Framework;
using System.Text;
using System.Threading.Channels;

namespace Lynx.Test.BestMove
{
    public class ForceOrAvoidDrawTest : BaseTest
    {
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

        [TestCase("8/8/4NQ2/7k/2P4p/1q2P2P/5P2/6K1 b - - 5 52", new[] { "b3b1", "b3d1" },
            Description = "Force stalemate - https://lichess.org/sM5ekwnW/black#103")]
        [TestCase("8/8/4NQ2/7k/2P4p/4P2P/5PK1/3q4 b - - 7 53", new[] { "d1h1", "d1g1", "d1f1" },
            Description = "Force stalemate - https://lichess.org/sM5ekwnW/black#105")]
        public void ForceStaleMate(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
        {
            TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString);
        }

        [Test]
        public void AvoidThreefoldRepetitionWhenWinningPosition()
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
            foreach (var move in repeatedMoves)
            {
                sb.Append(' ').Append(move.UCIString());
                engine.AdjustPosition(sb.ToString());
            }

            Assert.AreEqual(repeatedMoves.Count, engine.Game.MoveHistory.Count);

            // Act
            var searchResult = engine.BestMove();
            var bestMoveFound = searchResult.BestMove;

            // Assert
            Assert.AreNotEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No threefold repetition avoided");
            Assert.Less(searchResult.Evaluation, EvaluationConstants.CheckMateEvaluation - (20 * Position.DepthFactor), "Mate not detected");
        }

        [Test]
        public void ForceThreefoldRepetitionWhenLosingPosition()
        {
            // Arrange

            const string fen = "7B/8/7k/8/5KR1/8/5R2/8 w - - 0 1";

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
                sb.Append(' ');
                sb.Append(move.UCIString());
                engine.AdjustPosition(sb.ToString());
            }

            Assert.AreEqual(repeatedMoves.Count, engine.Game.MoveHistory.Count);

            // Act
            var searchResult = engine.BestMove();
            var bestMoveFound = searchResult.BestMove;

            // Assert
            Assert.AreEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No threefold repetition forced");
            Assert.AreEqual(0, searchResult.Evaluation, "No drawn position detected");
        }

        [Test]
        public void Avoid50MovesRuleRepetitionWhenWinningPosition()
        {
            // Arrange

            // https://gameknot.com/chess-puzzle.pl?pz=247493
            const string fen = "r6k/p1q1b1pp/2p5/Qp2n1N1/4P1PK/P3Br1P/1P2RP2/8 b - - 0 1";

            var mock = new Mock<ChannelWriter<string>>();

            mock
                .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            var engine = new Engine(mock.Object);
            engine.SetGame(new Game(fen));

            var nonCaptureOrPawnMoveMoves = new List<Move>
            {
                new ((int)BoardSquare.c7, (int)BoardSquare.d6, (int)Piece.q),
                new ((int)BoardSquare.h4, (int)BoardSquare.h5, (int)Piece.K),
                new ((int)BoardSquare.d6, (int)BoardSquare.c7, (int)Piece.q),
                new ((int)BoardSquare.h5, (int)BoardSquare.h4, (int)Piece.K)
            };

            Move movesThatAllowsRepetition = new((int)BoardSquare.c7, (int)BoardSquare.d6, (int)Piece.q);

            var sb = new StringBuilder($"position fen {fen} moves");
            for (int i = 0; i < 48; ++i)
            {
                var move = nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count];
                sb.Append(' ').Append(move.UCIString());
                engine.AdjustPosition(sb.ToString());
            }

            Assert.AreEqual(48, engine.Game.MoveHistory.Count);

            engine.Game.PositionHashHistory.Clear(); // Make sure we don't take account threefold repetition

            // Act
            var searchResult = engine.BestMove();
            var bestMoveFound = searchResult.BestMove;

            // Assert
            Assert.AreNotEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No 50 moves rule avoided");
            Assert.Less(searchResult.Evaluation, EvaluationConstants.CheckMateEvaluation - (20 * Position.DepthFactor), "Mate not detected");
        }

        [Test]
        public void Force50MovesRuleRepetitionWhenLosingPosition()
        {
            // Arrange

            const string fen = "8/7B/7k/8/5KR1/8/5R2/8 w - - 0 1";

            var mock = new Mock<ChannelWriter<string>>();

            mock
                .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            var engine = new Engine(mock.Object);
            engine.SetGame(new Game(fen));

            var nonCaptureOrPawnMoveMoves = new List<Move>
            {
                new ((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                new ((int)BoardSquare.h6, (int)BoardSquare.h5, (int)Piece.k),
                new ((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                new ((int)BoardSquare.h5, (int)BoardSquare.h6, (int)Piece.k)
            };

            Move movesThatAllowsRepetition = new((int)BoardSquare.h6, (int)BoardSquare.h5, (int)Piece.k);

            var sb = new StringBuilder($"position fen {fen} moves");
            for (int i = 0; i < 48; ++i)
            {
                var move = nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count];
                sb.Append(' ').Append(move.UCIString());
                engine.AdjustPosition(sb.ToString());
            }

            sb.Append(' ').Append(nonCaptureOrPawnMoveMoves[0].UCIString());
            engine.AdjustPosition(sb.ToString());

            Assert.AreEqual(49, engine.Game.MoveHistory.Count);
            engine.Game.PositionHashHistory.Clear(); // Make sure we don't take account threefold repetition

            // Act
            var searchResult = engine.BestMove();
            var bestMoveFound = searchResult.BestMove;

            // Assert
            Assert.AreEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No 50 moves rule forced");
            Assert.AreEqual(0, searchResult.Evaluation, "No drawn position detected");
        }
    }
}
