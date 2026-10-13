using NUnit.Framework;

namespace Lynx.Test;

/// <summary>
/// Runs <see cref="PerftFRCTestSuite"/> positions using <see cref="MovePicker"/> (staged move generation) instead of generating all moves at once.
/// Specially relevant for castling, which in FRC/DFRC is encoded as the king moving to its own rook square.
/// </summary>
[TestFixture(Category = Categories.PerftFRC, Explicit = true)]
public class PerftFRCTestSuiteMovePicker : BaseTest
{
    public PerftFRCTestSuiteMovePicker()
    {
        Configuration.EngineSettings.IsChess960 = true;
    }

    [TestCaseSource(typeof(PerftFRCTestSuiteCases), nameof(PerftFRCTestSuiteCases.Cases))]
    public void FRCSuite(string fen, int depth, long expectedNumberOfNodes)
    {
        // Test cases can run in parallel, so each one needs its own engine and state
        using var engine = GetEngine(fen);

        Assert.AreEqual(expectedNumberOfNodes, new PerftMovePickerWalker(engine).Perft(engine.Game.CurrentPosition, depth, ply: 0));
    }
}
