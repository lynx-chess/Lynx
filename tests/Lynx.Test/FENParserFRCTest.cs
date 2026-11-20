using Lynx.Model;
using NUnit.Framework;
using CR = Lynx.Model.CastlingRights;

namespace Lynx.Test;

/// <summary>
/// DFRC counterpart of <see cref="FENParserTest.CastlingRights(string, int)"/>
/// </summary>
[TestFixture(Category = Categories.PerftFRC, Explicit = true)]
public class FENParserFRCTest
{
    public FENParserFRCTest()
    {
        Configuration.EngineSettings.IsChess960 = true;
    }

    // Valid rights
    [TestCase("4k3/8/8/8/8/8/8/4K2R w K - 0 1", (int)CR.WK)]
    [TestCase("4k3/8/8/8/8/8/8/4K2R w H - 0 1", (int)CR.WK)]
    [TestCase("r3k3/8/8/8/8/8/8/4K3 w q - 0 1", (int)CR.BQ)]
    [TestCase("r3k3/8/8/8/8/8/8/4K3 w a - 0 1", (int)CR.BQ)]
    [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BK | (int)CR.BQ)]
    [TestCase("r3k2r/8/8/8/8/8/8/R3K2R w HAha - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BK | (int)CR.BQ)]

    // Rights without rook are dropped, the rest are kept
    [TestCase("r3k3/8/8/8/8/8/8/4K3 w Kq - 0 1", (int)CR.BQ, Description = "X-FEN K dropped, q kept")]
    [TestCase("r3k3/8/8/8/8/8/8/4K3 w Ha - 0 1", (int)CR.BQ, Description = "Shredder-FEN H dropped, a kept")]

    // X-FEN: no rook at all
    [TestCase("4k3/8/8/8/8/8/8/4K3 w K - 0 1", 0, Description = "X-FEN K, no rook")]
    [TestCase("4k3/8/8/8/8/8/8/4K3 w Q - 0 1", 0, Description = "X-FEN Q, no rook")]
    [TestCase("4k3/8/8/8/8/8/8/4K3 w k - 0 1", 0, Description = "X-FEN k, no rook")]
    [TestCase("4k3/8/8/8/8/8/8/4K3 w q - 0 1", 0, Description = "X-FEN q, no rook")]

    // X-FEN: rook only on the other side of the king
    [TestCase("4k3/8/8/8/8/8/8/R3K3 w K - 0 1", 0, Description = "X-FEN K, only queenside rook")]
    [TestCase("4k2r/8/8/8/8/8/8/4K3 w q - 0 1", 0, Description = "X-FEN q, only kingside rook")]

    // X-FEN: no rook on the back rank
    [TestCase("4k3/8/8/8/8/8/7R/4K3 w K - 0 1", 0, Description = "X-FEN K, rook on h2 instead of the back rank")]
    [TestCase("4k3/8/8/8/4K3/8/R7/8 w K - 0 1", 0, Description = "X-FEN K, king off the back rank, rook on a2")]
    [TestCase("8/r7/8/4k3/8/8/8/4K3 w q - 0 1", 0, Description = "X-FEN q, king off the back rank, rook on a7")]

    // Shredder-FEN: no rook on the specified file
    [TestCase("4k3/8/8/8/8/8/8/4K3 w H - 0 1", 0, Description = "Shredder-FEN H, no rook")]
    [TestCase("4k3/8/8/8/8/8/8/4K3 w A - 0 1", 0, Description = "Shredder-FEN A, no rook")]
    [TestCase("4k3/8/8/8/8/8/8/4K3 w h - 0 1", 0, Description = "Shredder-FEN h, no rook")]
    [TestCase("4k3/8/8/8/8/8/8/4K3 w a - 0 1", 0, Description = "Shredder-FEN a, no rook")]
    [TestCase("4k3/8/8/8/8/8/8/4K1R1 w H - 0 1", 0, Description = "Shredder-FEN H, rook on g1 instead")]
    [TestCase("4k3/8/8/8/8/8/8/4K2N w H - 0 1", 0, Description = "Shredder-FEN H, knight on h1")]
    [TestCase("4k3/8/8/8/8/8/8/4K2r w H - 0 1", 0, Description = "Shredder-FEN H, opponent rook on h1")]
    [TestCase("4k3/8/8/8/4K3/8/8/7R w H - 0 1", 0, Description = "Shredder-FEN H, king off the back rank")]

    // Invalid castling characters still throw
    [TestCase("4k3/8/8/8/8/8/8/4K3 w E - 0 1", -1, Description = "Shredder-FEN letter matching the king file")]
    [TestCase("4k3/8/8/8/8/8/8/4K3 w Z - 0 1", -1, Description = "Unrecognized castling char")]
    public void CastlingRights(string fen, int expectedCastleResult)
    {
        if (expectedCastleResult >= 0)
        {
            var result = ParseFEN(fen);

            Assert.AreEqual(expectedCastleResult, result.Castle);
        }
        else
        {
            Assert.Throws<LynxException>(() => ParseFEN(fen));
        }
    }

    private static ParseFENResult ParseFEN(string fen) =>
        FENParser.ParseFEN(fen,
            new Bitboard[Position.PieceBitboardsLength],
            new Bitboard[Position.OccupancyBitboardsLength],
            new int[Position.BoardLength]);
}
