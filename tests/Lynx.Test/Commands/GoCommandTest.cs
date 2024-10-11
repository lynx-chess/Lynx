using Lynx.UCI.Commands.GUI;
using NUnit.Framework;

namespace Lynx.Test.Commands;

public class GoCommandTest
{
    [Test]
    public void ParseGoCommand()
    {
        const string goCommandString = "go  infinite   wtime    10 btime     20  winc      30 binc   40  movestogo   50   movetime 70 mate 80 nodes 90   depth 60 ";

        var goCommand = new GoCommand(goCommandString);
        Assert.AreEqual(10, goCommand.WhiteTime);
        Assert.AreEqual(20, goCommand.BlackTime);
        Assert.AreEqual(30, goCommand.WhiteIncrement);
        Assert.AreEqual(40, goCommand.BlackIncrement);
        Assert.AreEqual(50, goCommand.MovesToGo);
        Assert.AreEqual(60, goCommand.Depth);
        Assert.AreEqual(70, goCommand.MoveTime);
        _ = Assert.Throws<NotImplementedException>(() => _ = GoCommand.Mate);
        _ = Assert.Throws<NotImplementedException>(() => _ = GoCommand.Nodes);
        _ = Assert.Throws<NotImplementedException>(() => _ = GoCommand.SearchMoves);
    }
}
