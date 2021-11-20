using NUnit.Framework;

namespace Lynx.Test.BestMove;

public class SacrificesTest : BaseTest
{
    [TestCase("4n3/1p2k2p/p2p2pP/P1bP1pP1/2P2P2/2BB4/3K4/8 w - - 0 43", new[] { "d3f5" },
        Category = "LongRunning", Explicit = true, Description = "Actual Bishop sacrifice - https://lichess.org/VaY6zfHI/white#84")]
    public void Sacrifices(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth: 12);
    }
}
