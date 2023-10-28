using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NUnit.Framework;

namespace Lynx.Test.BestMove;
public class IncreaseDepthWhenInCheckTest : BaseTest
{
    /// <summary>
    /// 8   . r . . . . . .
    /// 7   . . . . . . . .
    /// 6   . . . . . . . .
    /// 5   . . . . . . k P
    /// 4   K . . . . . . .
    /// 3   . . . . . R . .
    /// 2   . q . . . . . .
    /// 1   R . . . . . q .
    ///     a b c d e f g h
    /// </summary>
    [Test]
    public async Task DepthLimit()
    {
        Configuration.EngineSettings.MinDepth = 0;

        var engine = GetEngine("1r6/8/8/6kP/K7/5R1p/1q6/R5q1 w - - 0 2");
        Assert.AreEqual(Side.White, engine.Game.CurrentPosition.Side);

        var searchResult = await engine.BestMove(new GoCommand("go depth 1"));

        // In Quiescence search, which would be triggered after Rxg1+ without
        // the depth increase, Black would capture the pawn and get checkmated
        Assert.AreEqual("g5h4", searchResult.Moves[1].UCIString());
    }
}
