using Lynx.UCI.Commands.GUI;
using NUnit.Framework;

namespace Lynx.Test.Commands;

public class GoCommandTest
{
    [TestCase]
    public void ParseGoCommand()
    {
        const string goCommandString = "go  infinite   wtime    10 btime     20  winc      30 binc   40  movestogo   50   movetime 70 mate 80 nodes 90   depth 60 ponder  ";

        var goCommand = new GoCommand(goCommandString);

        Assert.AreEqual(10, goCommand.WhiteTime);
        Assert.AreEqual(20, goCommand.BlackTime);
        Assert.AreEqual(30, goCommand.WhiteIncrement);
        Assert.AreEqual(40, goCommand.BlackIncrement);
        Assert.AreEqual(50, goCommand.MovesToGo);
        Assert.AreEqual(60, goCommand.Depth);
        Assert.AreEqual(70, goCommand.MoveTime);
        Assert.True(goCommand.Ponder);
        _ = Assert.Throws<NotSupportedException>(() => _ = GoCommand.Mate);
        _ = Assert.Throws<NotSupportedException>(() => _ = GoCommand.Nodes);
        _ = Assert.Throws<NotSupportedException>(() => _ = GoCommand.SearchMoves);
    }

    [TestCase("go wtime 10 btime 20 winc 30 binc 40 ponder")]
    [TestCase("go ponder wtime 10 btime 20 winc 30 binc 40")]
    [TestCase("go wtime 10 btime 20 ponder winc 30 binc 40")]
    [TestCase("go wtime 10 btime 20 winc 30 ponder binc 40")]
    [TestCase("go btime 20 wtime 10 winc 30 binc 40 ponder")]
    [TestCase("go winc 30 btime 20 wtime 10 binc 40 ponder")]
    [TestCase("go binc 40 winc 30 btime 20 wtime 10 ponder")]
    [TestCase("go ponder binc 40 winc 30 btime 20 wtime 10")]
    public void ParseGoCommandUnordered(string goCommandString)
    {
        var goCommand = new GoCommand(goCommandString);

        Assert.AreEqual(10, goCommand.WhiteTime);
        Assert.AreEqual(20, goCommand.BlackTime);
        Assert.AreEqual(30, goCommand.WhiteIncrement);
        Assert.AreEqual(40, goCommand.BlackIncrement);

        Assert.True(goCommand.Ponder);
    }
}
