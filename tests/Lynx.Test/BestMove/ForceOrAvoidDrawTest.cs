using Lynx.Model;
using Moq;
using NUnit.Framework;
using System.Text;
using System.Threading.Channels;

namespace Lynx.Test.BestMove;

public class ForceOrAvoidDrawTest : BaseTest
{
    [TestCase("8/8/4NQ2/7k/2P4p/1q2P2P/5P2/6K1 b - - 5 52", new[] { "b3b1", "b3d1" },
        Description = "Force stalemate - https://lichess.org/sM5ekwnW/black#103, Having issues with null pruning implemeneted")]
    [TestCase("8/8/4NQ2/7k/2P4p/4P2P/5PK1/3q4 b - - 7 53", new[] { "d1h1", "d1g1", "d1f1" },
        Description = "Force stalemate - https://lichess.org/sM5ekwnW/black#105")]
    public async Task ForceStaleMate(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
    {
        var result = await TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth: 5);
        Assert.AreEqual(0, result.Evaluation, "No drawn position detected");
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(6)]
    [TestCase(7)]
    [TestCase(8)]
    [TestCase(9)]
    public async Task AvoidThreefoldRepetitionWhenWinningPosition(int depth)
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
                MoveExtensions.Encode((int)BoardSquare.d6, (int)BoardSquare.c7, (int)Piece.q),
                MoveExtensions.Encode((int)BoardSquare.h5, (int)BoardSquare.h4, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.c7, (int)BoardSquare.d6, (int)Piece.q),
                MoveExtensions.Encode((int)BoardSquare.h4, (int)BoardSquare.h5, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.d6, (int)BoardSquare.c7, (int)Piece.q),
                MoveExtensions.Encode((int)BoardSquare.h5, (int)BoardSquare.h4, (int)Piece.K)
            };

        Move movesThatAllowsRepetition = MoveExtensions.Encode((int)BoardSquare.c7, (int)BoardSquare.d6, (int)Piece.q);

        var sb = new StringBuilder($"position fen {fen} moves");
        foreach (var move in repeatedMoves)
        {
            sb.Append(' ').Append(move.UCIString());
            engine.AdjustPosition(sb.ToString());
        }

        Assert.AreEqual(repeatedMoves.Count, engine.Game.MoveHistory.Count);

        // Act
        var searchResult = await engine.BestMove(new($"go depth {depth}"));
        var bestMoveFound = searchResult.BestMove;

        // Assert
        Assert.AreNotEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No threefold repetition avoided");
    }

    [Test]
    public async Task ForceThreefoldRepetitionWhenLosingPosition()
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
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h6, (int)BoardSquare.h7, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h7, (int)BoardSquare.h6, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h6, (int)BoardSquare.h7, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
            };

        Move movesThatAllowsRepetition = MoveExtensions.Encode((int)BoardSquare.h7, (int)BoardSquare.h6, (int)Piece.k);

        var sb = new StringBuilder($"position fen {fen} moves");
        foreach (var move in repeatedMoves)
        {
            sb.Append(' ');
            sb.Append(move.UCIString());
            engine.AdjustPosition(sb.ToString());
        }

        Assert.AreEqual(repeatedMoves.Count, engine.Game.MoveHistory.Count);

        // Act
        var searchResult = await engine.BestMove();
        var bestMoveFound = searchResult.BestMove;

        // Assert
        Assert.AreEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No threefold repetition forced");
        Assert.AreEqual(0, searchResult.Evaluation, "No drawn position detected");
    }

    [Test]
    public async Task Avoid50MovesRuleRepetitionWhenWinningPosition()
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

        var nonCaptureOrPawnMoveMoves = new List<Move>
            {
                MoveExtensions.Encode((int)BoardSquare.d6, (int)BoardSquare.c7, (int)Piece.q),
                MoveExtensions.Encode((int)BoardSquare.h5, (int)BoardSquare.h4, (int)Piece.K),
                MoveExtensions.Encode((int)BoardSquare.c7, (int)BoardSquare.d6, (int)Piece.q),
                MoveExtensions.Encode((int)BoardSquare.h4, (int)BoardSquare.h5, (int)Piece.K)
            };

        Move movesThatAllowsRepetition = MoveExtensions.Encode((int)BoardSquare.c7, (int)BoardSquare.d6, (int)Piece.q);

        var sb = new StringBuilder($"position fen {fen} moves");
        for (int i = 0; i < 98; ++i)
        {
            var move = nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count];
            sb.Append(' ').Append(move.UCIString());
            engine.AdjustPosition(sb.ToString());
        }

        Assert.AreEqual(98, engine.Game.MoveHistory.Count);

        engine.Game.PositionHashHistory.Clear(); // Make sure we don't take account threefold repetition

        // Act
        var searchResult = await engine.BestMove();
        var bestMoveFound = searchResult.BestMove;

        // Assert
        Assert.AreNotEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No 50 moves rule avoided");
    }

    [Test]
    public async Task Force50MovesRuleRepetitionWhenLosingPosition()
    {
        // Arrange

        const string fen = "8/7B/8/7k/5KR1/8/4R3/8 w - - 0 1";

        var mock = new Mock<ChannelWriter<string>>();

        mock
            .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var engine = new Engine(mock.Object);
        engine.SetGame(new Game(fen));

        var nonCaptureOrPawnMoveMoves = new List<Move>
            {
                MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.f2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h5, (int)BoardSquare.h6, (int)Piece.k),
                MoveExtensions.Encode((int)BoardSquare.f2, (int)BoardSquare.e2, (int)Piece.R),
                MoveExtensions.Encode((int)BoardSquare.h6, (int)BoardSquare.h5, (int)Piece.k)
            };

        Move movesThatAllowsRepetition = MoveExtensions.Encode((int)BoardSquare.h6, (int)BoardSquare.h5, (int)Piece.k);

        var sb = new StringBuilder($"position fen {fen} moves");
        for (int i = 0; i < 98; ++i)
        {
            var move = nonCaptureOrPawnMoveMoves[i % nonCaptureOrPawnMoveMoves.Count];
            sb.Append(' ').Append(move.UCIString());
            engine.AdjustPosition(sb.ToString());
        }

        sb.Append(' ').Append(nonCaptureOrPawnMoveMoves[2].UCIString());
        engine.AdjustPosition(sb.ToString());

        Assert.AreEqual(99, engine.Game.MoveHistory.Count);
        engine.Game.PositionHashHistory.Clear(); // Make sure we don't take account threefold repetition

        // Act
        var searchResult = await engine.BestMove();
        var bestMoveFound = searchResult.BestMove;

        // Assert
        Assert.AreEqual(movesThatAllowsRepetition.UCIString(), bestMoveFound.UCIString(), "No 50 moves rule forced");
        Assert.AreEqual(0, searchResult.Evaluation, "No drawn position detected");
    }

    /// <summary>
    /// If a checkmate is delivered in move 50 (ply 100), the result of the game is checkmate
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task CheckmateHasPrecedenceOver50MovesRule()
    {
        // Source: https://github.com/PGG106/Alexandria/issues/213
        const string mateIn1Fen = "4Q3/8/1p4pk/1PbB1p1p/7P/p3P1PK/P3qP2/8 w - - 99 88";

        var result = await TestBestMove(mateIn1Fen, new[] { "e8h8" }, Array.Empty<string>(), depth: 1);
        Assert.AreEqual(1, result.Mate);
    }
}
