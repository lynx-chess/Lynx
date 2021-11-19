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

        // A command that guarantees ~5s thinking time
        var goCommand = new GoCommand();
        await goCommand.Parse($"go depth {Configuration.EngineSettings.MaxDepth}");

        var resultTask = Task.Run(() => engine.BestMove());
        // Wait 2s so that there's some best move available
        Thread.Sleep(2000);

        // Act
        engine.StopSearching();

        // Assert
        Assert.AreNotEqual(default, (await resultTask).BestMove.EncodedMove);

        Assert.AreEqual(initialPositionFEN, engine.Game.CurrentPosition.FEN);
        Assert.IsEmpty(engine.Game.MoveHistory);
    }
}
