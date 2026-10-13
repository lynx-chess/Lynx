using NUnit.Framework;
using Lynx.Model;

namespace Lynx.Test;

/// <summary>
/// http://www.rocechess.ch/perftsuite.zip, from http://www.rocechess.ch/perft.html
/// </summary>
[TestFixture(Category = Categories.Perft, Explicit = true)]
public class PerftTestSuite
{
    [TestCaseSource(typeof(PerftTestSuiteCases), nameof(PerftTestSuiteCases.Cases))]
    public void Suite(string fen, int depth, long expectedNumberOfNodes)
    {
        Validate(fen, depth, expectedNumberOfNodes);
    }

    private static void Validate(string fen, int depth, long expectedNumberOfNodes)
    {
        Assert.AreEqual(expectedNumberOfNodes, Perft.PerftRecursiveImpl(new Position(fen), depth, default));
    }
}
