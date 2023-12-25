using Lynx.UCI.Commands.GUI;
using Moq;
using NUnit.Framework;
using System.Threading.Channels;

namespace Lynx.Test.Commands;

/// <summary>
/// https://github.com/lynx-chess/Lynx/issues/31
/// </summary>
public class StopCommandTest
{
    [TestCase(Constants.TrickyTestPositionFEN)]
    public async Task StopCommandShouldNotModifyPositionOrAddMoveToMoveHistory(string initialPositionFEN)
    {
        // Arrange
        var engine = new Engine(new Mock<ChannelWriter<string>>().Object);
        engine.NewGame();
        engine.AdjustPosition($"position fen {initialPositionFEN}");

        // A command that guarantees that the search doesn't finish before the end of the test
        var goCommand = new GoCommand($"go depth {Configuration.EngineSettings.MaxDepth}");

        var resultTask = Task.Run(() => engine.BestMove(goCommand));
        // Wait 2s so that there's some best move available
        Thread.Sleep(2000);

        // Act
        engine.StopSearching();

        // Assert
        Assert.AreNotEqual(default, (await resultTask).BestMove);

        Assert.AreEqual(initialPositionFEN, engine.Game.CurrentPosition.FEN());
        Assert.IsEmpty(engine.Game.MoveHistory);
    }
}
