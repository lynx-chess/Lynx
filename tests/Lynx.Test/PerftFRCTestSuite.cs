using NUnit.Framework;
using Lynx.Model;

namespace Lynx.Test;

/// <summary>
/// https://github.com/AndyGrant/Ethereal/blob/master/src/perft/fischer.epd
/// </summary>
[TestFixture(Category = Categories.PerftFRC, Explicit = true)]
public class PerftFRCTestSuite
{
    public PerftFRCTestSuite()
    {
        Configuration.EngineSettings.IsChess960 = true;
    }

    [TestCaseSource(typeof(PerftFRCTestSuiteCases), nameof(PerftFRCTestSuiteCases.Cases))]
    public void FRCSuite(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    private static void Validate(string fen, int depth, long expectedNumberOfNodes)
    {
        Assert.AreEqual(expectedNumberOfNodes, Perft.PerftRecursiveImpl(new Position(fen), depth, default));
    }
}
