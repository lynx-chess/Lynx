using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

public class UtilsTest
{
    [TestCase(Side.Black, 6)]
    [TestCase(Side.White, 0)]
    public void PieceOffSetBySide(Side sideToMove, int expectedOffset)
    {
        Assert.AreEqual(expectedOffset, Utils.PieceOffset(sideToMove));
        Assert.AreEqual(expectedOffset, Utils.PieceOffset((int)sideToMove));
    }

    [TestCase(Side.Black, (int)Side.White)]
    [TestCase(Side.White, (int)Side.Black)]
    public void OppositeSide(Side sideToMove, int expectedSide)
    {
        Assert.AreEqual(expectedSide, Utils.OppositeSide(sideToMove));
    }

    [TestCase(Side.White, Constants.WhiteShortCastleRookSquare)]
    [TestCase(Side.Black, Constants.BlackShortCastleRookSquare)]
    public void ShortCastleRookTargetSquare(Side sideToMove, int expectedRookSquare)
    {
        Assert.AreEqual(expectedRookSquare, Utils.ShortCastleRookTargetSquare(sideToMove));
        Assert.AreEqual(expectedRookSquare, Utils.ShortCastleRookTargetSquare((int)sideToMove));
    }

    [TestCase(Side.White, Constants.WhiteLongCastleRookSquare)]
    [TestCase(Side.Black, Constants.BlackLongCastleRookSquare)]
    public void LongCastleRookTargetSquare(Side sideToMove, int expectedRookSquare)
    {
        Assert.AreEqual(expectedRookSquare, Utils.LongCastleRookTargetSquare(sideToMove));
        Assert.AreEqual(expectedRookSquare, Utils.LongCastleRookTargetSquare((int)sideToMove));
    }

    [TestCase(Side.White, (int)BoardSquare.h1)]
    [TestCase(Side.Black, (int)BoardSquare.h8)]
    public void ShortCastleRookSourceSquare(Side sideToMove, int expectedRookSquare)
    {
        Assert.AreEqual(expectedRookSquare, Utils.ShortCastleRookSourceSquare(sideToMove));
        Assert.AreEqual(expectedRookSquare, Utils.ShortCastleRookSourceSquare((int)sideToMove));
    }

    [TestCase(Side.White, (int)BoardSquare.a1)]
    [TestCase(Side.Black, (int)BoardSquare.a8)]
    public void LongCastleRookSourceSquare(Side sideToMove, int expectedRookSquare)
    {
        Assert.AreEqual(expectedRookSquare, Utils.LongCastleRookSourceSquare(sideToMove));
        Assert.AreEqual(expectedRookSquare, Utils.LongCastleRookSourceSquare((int)sideToMove));
    }

    [TestCase("rnbqkbnr/ppp1pppp/3p4/1B6/8/4P3/PPPP1PPP/RNBQK1NR b KQkq - 1 2", true)]
    [TestCase("rnbqkbnr/ppp1pppp/3p4/1B6/8/4P3/PPPP1PPP/RNBQK1NR w KQkq - 1 2", false)]
    [TestCase("rnbqk1nr/pppp1ppp/4p3/8/1b6/3P1N2/PPP1PPPP/RNBQKB1R w KQkq - 2 3", true)]
    [TestCase("rnbqk1nr/pppp1ppp/4p3/8/1b6/3P1N2/PPP1PPPP/RNBQKB1R b KQkq - 2 3", false)]

    [TestCase("rnb1k1nr/pppp1ppp/4p3/8/1b6/3P3P/PPP1PPP1/RNB1KBNR w KQkq - 1 3", true)]
    [TestCase("rnb1k1nr/pppp1ppp/4p3/8/1b6/3P3P/PPP1PPP1/RNB1KBNR b KQkq - 1 3", false)]
    [TestCase("rnb1k1nr/pppp1ppp/4p3/8/1b6/3P3P/PPP1PPP1/RNB1KBNR w KQkq - 1 3", true)]
    [TestCase("rnb1k1nr/pppp1ppp/4p3/8/1b6/3P3P/PPP1PPP1/RNB1KBNR b KQkq - 1 3", false)]
    public void InCheck(string fen, bool positionInCheck)
    {
        var position = new Position(fen);

        Assert.AreEqual(positionInCheck, Utils.InCheck(position));
    }
}
