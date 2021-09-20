using Lynx.UCI.Commands.GUI;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Lynx.NUnit.Test
{
    /// <summary>
    /// https://github.com/lynx-chess/Lynx/issues/31
    /// </summary>
    public class PositionCommandTest
    {
        [TestCase]
        public async Task StopCommandShouldNotModifyPositionOrAddMoveToMoveHistory()
        {
            // Arrange
            var engine = new Engine();
            engine.NewGame();
            engine.AdjustPosition($"position fen {Constants.InitialPositionFEN} moves e2e4");

            // A command that guarantees ~5s thinking time
            var goCommand = new GoCommand();
            await goCommand.Parse("go wtime 1 btime 1 winc 5000 binc 5000");

            var resultTask = Task.Run(() => engine.BestMove());

            engine.StopSearching();
            await resultTask;

            // Act
            engine.AdjustPosition($"position fen {Constants.InitialPositionFEN} moves d2d4");

            // Assert
            Assert.AreEqual(1, engine.Game.MoveHistory.Count);
        }
    }
}
