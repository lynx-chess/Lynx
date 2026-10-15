using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

/// <summary>
/// Runs <see cref="PerftFRCTestSuite"/> positions using <see cref="MovePicker"/> (staged move generation) instead of generating all moves at once.
/// Specially relevant for castling moves, which are KxR in (D)FRC
/// </summary>
[TestFixture(Category = Categories.PerftFRC, Explicit = true)]
public class PerftFRCTestSuiteMovePicker : BaseTest
{
    private TranspositionTable _tt;

    [OneTimeSetUp]
    public void OneTimeSetUp() => _tt = new();

    [OneTimeTearDown]
    public void OneTimeTearDown() => _tt.Release();

    public PerftFRCTestSuiteMovePicker()
    {
        Configuration.EngineSettings.IsChess960 = true;
    }

    [TestCaseSource(typeof(PerftFRCTestSuiteCases), nameof(PerftFRCTestSuiteCases.Cases))]
    public void FRCSuite_MovePicker(string fen, int depth, long expectedNumberOfNodes)
    {
        // Test cases can run in parallel, so each one needs its own engine and state
        using var engine = GetEngine(fen, in _tt);

        Assert.AreEqual(expectedNumberOfNodes, new PerftMovePickerWalker(engine).Perft(engine.Game.CurrentPosition, depth, ply: 0));
    }
}
