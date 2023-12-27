using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using Moq;
using NUnit.Framework;
using System.Threading.Channels;

namespace Lynx.Test.Commands;

/// <summary>
/// https://github.com/lynx-chess/Lynx/issues/31
/// </summary>
public class PositionCommandTest
{
    [TestCase]
    public async Task PositionCommandShouldNotTakeIntoAccountInternalState()
    {
        // Arrange
        var engine = new Engine(new Mock<ChannelWriter<string>>().Object);
        engine.NewGame();
        engine.AdjustPosition($"position fen {Constants.InitialPositionFEN} moves e2e4");

        // A command that guarantees ~5s thinking time
        var goCommand = new GoCommand("go wtime 1 btime 1 winc 5000 binc 5000");

        var resultTask = Task.Run(() => engine.BestMove(new($"go depth {Engine.DefaultMaxDepth}")));

        engine.StopSearching();
        await resultTask;

        // Act
        engine.AdjustPosition($"position fen {Constants.InitialPositionFEN} moves d2d4");

        // Assert
        Assert.AreEqual(1, engine.Game.MoveHistory.Count);
    }

    [TestCase("position startpos moves d2d4 g8f6 g1f3 d7d5 b1c3 e7e6 g2g3 c7c5 e2e3")]
    [TestCase(" position  startpos   moves d2d4   g8f6     g1f3  d7d5 b1c3  e7e6 g2g3 c7c5  e2e3      ")]
    [TestCase("position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 moves d2d4 g8f6 g1f3 d7d5 b1c3 e7e6 g2g3 c7c5 e2e3")]
    [TestCase(" position    fen         rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1   moves d2d4   g8f6     g1f3  d7d5 b1c3  e7e6 g2g3 c7c5  e2e3      ")]
    public void ParseGame_Spaces(string positionCommand)
    {
        var parsedGame = PositionCommand.ParseGame(positionCommand, new Move[Constants.MaxNumberOfPossibleMovesInAPosition]);

        Assert.AreEqual("d2d4", parsedGame.MoveHistory[0].UCIString());
        Assert.AreEqual("g8f6", parsedGame.MoveHistory[1].UCIString());
        Assert.AreEqual("g1f3", parsedGame.MoveHistory[2].UCIString());
        Assert.AreEqual("d7d5", parsedGame.MoveHistory[3].UCIString());
        Assert.AreEqual("b1c3", parsedGame.MoveHistory[4].UCIString());
        Assert.AreEqual("e7e6", parsedGame.MoveHistory[5].UCIString());
        Assert.AreEqual("g2g3", parsedGame.MoveHistory[6].UCIString());
        Assert.AreEqual("c7c5", parsedGame.MoveHistory[7].UCIString());
        Assert.AreEqual("e2e3", parsedGame.MoveHistory[8].UCIString());

        Assert.AreEqual("rnbqkb1r/pp3ppp/4pn2/2pp4/3P4/2N1PNP1/PPP2P1P/R1BQKB1R b KQkq - 0 5", parsedGame.CurrentPosition.FEN(parsedGame.HalfMovesWithoutCaptureOrPawnMove, (parsedGame.MoveHistory.Count / 2) + (parsedGame.MoveHistory.Count % 2)));
    }

    /// <summary>
    /// 296 moves https://lichess.org/RViT3UWL2yy0
    /// </summary>
    [Test]
    public void ParseGame_Long()
    {
        var parsedGame = PositionCommand.ParseGame(
            "position startpos moves d2d4 g8f6 g1f3 d7d5 b1c3 e7e6 g2g3 c7c5 e2e3 c5c4 f1g2 e6e5 d4e5 f6e4 d1d5 e4c3 d5d8 e8d8 b2c3 b8c6 c1b2 a7a6 e1c1 d8c7 d1e1 a6a5 e3e4 a5a4 a2a3 b7b6 f3d2 b6b5 f2f4 h8g8 d2f3 c8g4 f3g5 f8c5 g5h7 g8h8 h7g5 f7f6 e5f6 g7f6 g5f3 a8b8 f3d4 g4d7 h2h4 h8g8 e1e3 b5b4 a3b4 a4a3 b2a3 c6d4 b4c5 g8g3 e3g3 d4e2 c1d2 e2g3 h1e1 d7c6 a3b4 b8h8 b4a5 c7d7 e1e3 h8g8 d2e1 f6f5 g2h3 d7e7 e4f5 g3e4 e1f1 e7f7 a5b6 g8h8 h3g2 h8h4 b6c7 e4d2 f1g1 h4g4 e3e2 d2f3 g1f1 f3h2 f1g1 h2f3 g1f2 g4g2 f2g2 f3d4 g2f2 d4e2 f2e2 f7f6 e2e3 f6f5 c7e5 f5e6 e3d4 c6f3 d4c4 f3e2 c4d4 e2b5 c3c4 b5e8 d4d3 e8g6 d3c3 e6d7 c3b3 g6e4 c2c3 d7c6 e5d6 c6d7 d6e5 d7c6 b3b4 e4b1 e5d4 b1c2 b4a5 c2f5 a5b4 f5d3 d4e3 d3b1 e3d4 b1e4 d4h8 e4g6 h8g7 g6h7 g7f8 h7e4 f8g7 e4c2 g7f8 c2h7 f8d6 h7d3 d6b8 d3h7 b8e5 h7b1 e5h8 b1e4 h8d4 e4c2 d4e3 c2f5 b4b3 f5g6 b3b4 g6c2 e3c1 c2e4 c1a3 e4g6 a3c1 g6b1 c1e3 b1d3 e3c1 d3b1 c1a3 b1c2 a3b2 c2f5 b2a3 f5d3 b4b3 d3f5 b3a4 f5c2 a4a5 c2f5 a5a4 f5c2 a4a5 c2g6 a5b4 g6c2 a3b2 c2f5 b4b3 c6c5 b2a3 c5c6 b3a4 f5c2 a4a5 c2d3 a5b4 d3f5 a3b2 f5c2 c4c5 c2d3 c3c4 d3f5 b2a3 f5h3 b4c3 h3e6 c3d4 c6c7 a3b2 c7d7 d4c3 e6g4 c3d4 d7e6 d4e3 g4h3 e3f2 h3f5 f2e3 f5h3 b2e5 e6d7 e5d6 h3f5 d6b8 d7c6 b8d6 c6d7 d6b8 d7c6 e3d4 f5e6 b8e5 e6f5 e5d6 f5e6 d6f8 c6c7 f8e7 c7c6 e7d6 c6d7 d6e5 e6h3 d4c3 h3g2 f4f5 g2e4 f5f6 e4g6 c3d4 g6f7 e5d6 d7e6 d6e7 e6d7 d4c3 f7g8 e7d6 g8f7 c3d4 d7e6 d6e5 e6d7 d4d3 d7c6 e5d6 c6d7 d6e5 f7g6 d3e3 g6f7 e3d4 f7g8 d4d3 d7c6 e5d6 c6d7 d3d4 g8f7 d6e7 f7e6 d4c3 e6g8 c3b4 d7c6 e7d6 g8f7 d6e5 f7e6 e5b2 e6f7 b2d4 f7e8 d4c3 c6b7 c3b2 b7c6 b2d4 c6c7 d4e5 c7c6 e5d4 e8f7 d4e5 f7g6 e5b2 g6f7 b2c3 f7h5 c3d4 h5e8 b4b3 e8h5 b3c3 h5e8 c3b2 e8h5 b2c3 h5g6 c3d2 g6f7 d2c3 f7g8 c3b3 g8f7 d4f2 c6d7 b3b4 d7c6 b4c3 f7e6 c3d4 c6d7 f2g3 e6f7 g3e5 d7c6 e5d6 f7e6 d6e7 e6f7 e7d6 f7g8 d6e5 g8e6 f6f7 e6f7 e5f4 f7h5 f4d6 h5g4 d6e5 g4h3 e5d6 c6b7 d6f4 h3f1 f4e5 f1e2 d4d5 e2d1 c5c6 b7c8 c4c5 d1e2 d5d6 e2f3 e5h8 f3d1 h8b2 d1f3 b2e5 f3e4 e5b2 e4h1 b2h8 h1f3 h8e5 f3g4 e5b2 g4h5 b2d4 h5g6 d4b2 g6h5 c6c7 h5f3 b2e5 f3g2 e5b2 g2f3 b2g7 f3h1 g7e5 h1g2 e5d4 g2c6 d4h8 c6f3 h8b2 f3d1 b2e5 d1e2 d6e6 e2b5 e6d6 b5e2 d6e6 e2f3 e6f5 f3b7 f5f6 b7f3 f6e6 f3c6 e6f5 c6g2 f5e6 g2c6 e6f6 c6d7 f6g5 d7h3 g5f6 h3g2 f6f5 g2b7 f5f4 c8d7 f4e3 d7c6 e3d4 b7c8 e5d6 c8b7 d4c4 b7a6 c4d4 a6b7 d6e5 c6d7 d4c4 b7a6 c4d4 a6b7 d4c4 d7c6 e5f4 b7a6 c4d4 a6c8 f4e5 c8b7 e5d6 c6d7 d6e5 d7c6 e5d6 c6d7 d6g3 d7c6 g3f4 b7c8 f4h2 c8b7 h2g3 c6d7 d4c4 d7c6 c4d4 c6d7 g3f4 d7c6 f4h2 c6d7 h2f4 d7c6 d4c4 b7a6 c4b4 a6c8 b4c3 c8a6 c7c8Q a6c8 c3d4 c8b7 d4c4 b7c8 c4d4 c8h3 f4c1 h3f1 c1a3 f1a6 a3b2 a6c8 b2a3 c8h3 d4e5 h3d7 e5d4 d7c8 a3b2 c8d7 d4c4 d7e6 c4d4 e6g4 b2a3 g4c8 a3b2 c8e6 b2c3 e6c8 d4c4 c8a6 c4d4 a6b7 d4c4 b7c8 c3b2 c8h3 b2a1 h3e6 c4d4 e6d5 a1c3 d5g2 d4c4 g2f1 c4d4 f1a6 c3d2 a6f1 d2f4 f1h3 f4h6 h3f1 h6g5 f1h3 d4c4 h3f1 c4d4 f1h3 g5f4 h3f1 f4e5 f1e2 e5d6 e2b5 d6e5 b5f1 e5b8 f1b5 b8d6 b5f1 d6e5 f1a6 e5f4 a6e2 f4e5 e2g4 e5d6 g4e6 d6e5 e6h3 e5f4 h3g2 f4d2 g2d5 d2f4 d5f7 f4e5 f7a2 e5d6 a2g8 d6e7 g8e6 e7d6 e6d7 d4e5 d7h3 e5f6 h3f1", new Move[Constants.MaxNumberOfPossibleMovesInAPosition]);

        Assert.AreNotEqual(Constants.InitialPositionFEN, parsedGame.CurrentPosition);
        Assert.Greater(parsedGame.PositionHashHistory.Count, 500);
        Assert.Greater(parsedGame.MoveHistory.Count, 590);
    }
}
