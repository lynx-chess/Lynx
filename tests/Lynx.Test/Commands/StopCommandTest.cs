﻿using Lynx.UCI.Commands.GUI;
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
        var engine = new Engine(new Mock<ChannelWriter<object>>().Object);
        engine.NewGame();
        engine.AdjustPosition($"position fen {initialPositionFEN}");

        // A command that guarantees that the search doesn't finish before the end of the test
        var goCommand = new GoCommand($"go depth {Configuration.EngineSettings.MaxDepth}");
        var searchConstraints = TimeManager.CalculateTimeManagement(engine.Game, goCommand);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(10_000);

        var resultTask = Task.Run(() => engine.Search(searchConstraints, isPondering: false, cts.Token, CancellationToken.None));
        // Wait 2s so that there's some best move available
        await Task.Delay(2000);

        // Act
        await cts.CancelAsync();

        // Assert
        var result = await resultTask;
        Assert.NotNull(result);
        Assert.AreNotEqual(default, result?.BestMove);

        Assert.AreEqual(initialPositionFEN, engine.Game.CurrentPosition.FEN());

#if DEBUG
        Assert.IsEmpty(engine.Game.MoveHistory);
#endif
    }
}
