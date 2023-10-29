using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.BestMove;
public class PonderHitTest : BaseTest
{
    [Test]
    public async Task PonderHit()
    {
        var engine = GetEngine(Constants.InitialPositionFEN);

        var result = await engine.BestMove(new("go depth 10"));

        engine.AdjustPosition($"position startpos moves {result.Moves[0].UCIString()} {result.Moves[1].UCIString()}".AsSpan());

        result = await engine.BestMove(new("go wtime 1000 btime 1000"));

        Assert.AreNotEqual(0, result.BestMove);
        Assert.GreaterOrEqual(result.Depth, 8);
    }
}
