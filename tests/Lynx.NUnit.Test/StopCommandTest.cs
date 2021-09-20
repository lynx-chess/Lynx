using Lynx.UCI.Commands.GUI;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Lynx.NUnit.Test
{
    /// <summary>
    /// https://github.com/lynx-chess/Lynx/issues/31
    /// </summary>
    public class StopCommandTest
    {
        [TestCase("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1")]
        public async Task StopCommandShouldNotModifyPositionOrAddMoveToMoveHistory(string initialPositionFEN)
        {
            // Arrange
            var engine = new Engine();
            engine.NewGame();
            engine.AdjustPosition($"position fen {initialPositionFEN}");

            // A command that guarantees ~5s thinking time
            var goCommand = new GoCommand();
            await goCommand.Parse("go wtime 1 btime 1 winc 5000 binc 5000");

            var resultTask = Task.Run(() => engine.BestMove());
            // Wait 2s so that there's some best move available
            Thread.Sleep(2000);

            // Act
            engine.StopSearching();

            // Assert
            Assert.AreNotEqual(default, (await resultTask).BestMove.EncodedMove);

            Assert.AreEqual(initialPositionFEN, engine.Game.CurrentPosition.FEN());
            Assert.IsEmpty(engine.Game.MoveHistory);
        }
    }
}
