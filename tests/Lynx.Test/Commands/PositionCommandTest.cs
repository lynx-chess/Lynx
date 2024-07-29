using Lynx.UCI.Commands.GUI;
using Moq;
using NUnit.Framework;
using System.Threading.Channels;
#if DEBUG
using Lynx.Model;
#endif

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

        var resultTask = Task.Run(() => engine.BestMove(new($"go depth {Engine.DefaultMaxDepth}")));

        engine.StopSearching();
        await resultTask;

        // Act
        engine.AdjustPosition($"position fen {Constants.InitialPositionFEN} moves d2d4");

        // Assert
#if DEBUG
        Assert.AreEqual(1, engine.Game.MoveHistory.Count);
#endif

        Assert.Pass();
    }

    [TestCase("position startpos moves d2d4 g8f6 g1f3 d7d5 b1c3 e7e6 g2g3 c7c5 e2e3")]
    [TestCase(" position  startpos   moves d2d4   g8f6     g1f3  d7d5 b1c3  e7e6 g2g3 c7c5  e2e3      ")]
    [TestCase("position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 moves d2d4 g8f6 g1f3 d7d5 b1c3 e7e6 g2g3 c7c5 e2e3")]
    [TestCase(" position    fen         rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1   moves d2d4   g8f6     g1f3  d7d5 b1c3  e7e6 g2g3 c7c5  e2e3      ")]
    public void ParseGame_Spaces(string positionCommand)
    {
        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var parsedGame = PositionCommand.ParseGame(positionCommand, movePool);

#if DEBUG
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
#endif
        Assert.AreEqual("rnbqkb1r/pp3ppp/4pn2/2pp4/3P4/2N1PNP1/PPP2P1P/R1BQKB1R b KQkq - 0 5", parsedGame.CurrentPosition.FEN(parsedGame.HalfMovesWithoutCaptureOrPawnMove, (parsedGame.PositionHashHistory.Count / 2) + (parsedGame.PositionHashHistory.Count % 2)));

        Assert.Pass();
    }

    /// <summary>
    /// 296 moves https://lichess.org/RViT3UWL2yy0
    /// </summary>
    [Test]
    public void ParseGame_Long()
    {
        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var parsedGame = PositionCommand.ParseGame(Constants.LongPositionCommand, movePool);

        Assert.AreNotEqual(Constants.InitialPositionFEN, parsedGame.CurrentPosition);
        Assert.Greater(parsedGame.PositionHashHistory.Count, 500);

#if DEBUG
        Assert.Greater(parsedGame.MoveHistory.Count, 590);
#endif
    }
}
