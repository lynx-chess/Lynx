using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

/// <summary>
/// Runs <see cref="PerftTestSuite"/> positions using <see cref="MovePicker"/> (staged move generation) instead of generating all moves at once.
/// If the picker misses or repeats any legal move, the node count won't match.
/// To make the TT, killer and counter move stages kick in, the picker is fed with stale moves from previously visited nodes of the same ply.
/// </summary>
[TestFixture(Category = Categories.Perft, Explicit = true)]
public class PerftTestSuiteMovePicker : BaseTest
{
    [TestCaseSource(typeof(PerftTestSuiteCases), nameof(PerftTestSuiteCases.Cases))]
    public void Suite_MovePicker(string fen, int depth, long expectedNumberOfNodes)
    {
        // Test cases can run in parallel, so each one needs its own engine and state
        using var engine = GetEngine(fen);

        Assert.AreEqual(expectedNumberOfNodes, new PerftMovePickerWalker(engine).Perft(engine.Game.CurrentPosition, depth, ply: 0));
    }
}
