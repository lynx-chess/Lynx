using Lynx.UCI.Commands.GUI;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
        Assert.AreEqual(90, goCommand.Nodes);
        Assert.True(goCommand.Ponder);
        _ = Assert.Throws<NotSupportedException>(() => _ = GoCommand.Mate);
        Assert.IsEmpty(goCommand.SearchMoves);
    }

    [TestCase("go infinite")]
    [TestCase("go depth 5 searchmoves")]
    public void ParseGoCommandWithoutSearchMoves(string goCommandString)
    {
        var goCommand = new GoCommand(goCommandString);

        Assert.IsEmpty(goCommand.SearchMoves);
    }

    [TestCase("go searchmoves e2e4 d2d4", new[] { "e2e4", "d2d4" })]
    [TestCase("go infinite searchmoves e2e4 d2d4", new[] { "e2e4", "d2d4" })]
    [TestCase("go   searchmoves   e2e4    d2d4   infinite", new[] { "e2e4", "d2d4" })]
    [TestCase("go wtime 10 btime 20 searchmoves g1f3 depth 7", new[] { "g1f3" })]
    [TestCase("go SEARCHMOVES a7a8q a7a8N b1c3 movetime 5", new[] { "a7a8q", "a7a8N", "b1c3" })]
    [TestCase("go searchmoves e2e4 e2e4", new[] { "e2e4", "e2e4" })]
    [TestCase("go searchmoves e2e4 z9z9 d2d4", new[] { "e2e4" })]       // The list ends at the first token that isn't a move
    public void ParseSearchMoves(string goCommandString, string[] expectedSearchMoves)
    {
        var goCommand = new GoCommand(goCommandString);

        CollectionAssert.AreEqual(expectedSearchMoves, goCommand.SearchMoves);
    }

    [Test]
    public void SearchMovesDontAffectOtherArguments()
    {
        var goCommand = new GoCommand("go searchmoves e2e4 d2d4 wtime 10 btime 20 winc 30 binc 40 depth 6 nodes 90");

        Assert.AreEqual(10, goCommand.WhiteTime);
        Assert.AreEqual(20, goCommand.BlackTime);
        Assert.AreEqual(30, goCommand.WhiteIncrement);
        Assert.AreEqual(40, goCommand.BlackIncrement);
        Assert.AreEqual(6, goCommand.Depth);
        Assert.AreEqual(90, goCommand.Nodes);
        CollectionAssert.AreEqual(new[] { "e2e4", "d2d4" }, goCommand.SearchMoves);
    }

    [TestCase("e2e4", true)]
    [TestCase("a7a8q", true)]
    [TestCase("a7a8N", true)]
    [TestCase("h1h8", true)]
    [TestCase("e2e4k", false)]
    [TestCase("e2e", false)]
    [TestCase("e2e44", false)]
    [TestCase("i2e4", false)]
    [TestCase("e0e4", false)]
    [TestCase("e2e9", false)]
    [TestCase("E2E4", false)]
    [TestCase("depth", false)]
    [TestCase("wtime", false)]
    [TestCase("mate", false)]
    [TestCase("nodes", false)]
    [TestCase("", false)]
    public void IsValidUCIMove(string token, bool expected)
    {
        Assert.AreEqual(expected, GoCommand.IsValidUCIMove(token));
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
