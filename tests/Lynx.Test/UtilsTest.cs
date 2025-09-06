using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

public class UtilsTest
{
    [TestCase(Side.Black, 6)]
    [TestCase(Side.White, 0)]
    public void PieceOffSet(Side sideToMove, int expectedOffset)
    {
        Assert.AreEqual(expectedOffset, Utils.PieceOffset(sideToMove));
        Assert.AreEqual(expectedOffset, Utils.PieceOffset((int)sideToMove));
        Assert.AreEqual(expectedOffset, Utils.PieceOffset(sideToMove == Side.White));
    }

    [TestCase(Side.Black, (int)Side.White)]
    [TestCase(Side.White, (int)Side.Black)]
    public void OppositeSide(Side sideToMove, int expectedSide)
    {
        Assert.AreEqual(expectedSide, Utils.OppositeSide(sideToMove));
    }

    [TestCase(Side.White, Constants.WhiteRookKingsideCastlingSquare)]
    [TestCase(Side.Black, Constants.BlackRookKingsideCastlingSquare)]
    public void ShortCastleRookTargetSquare(Side sideToMove, int expectedRookSquare)
    {
        Assert.AreEqual(expectedRookSquare, Utils.ShortCastleRookTargetSquare(sideToMove));
        Assert.AreEqual(expectedRookSquare, Utils.ShortCastleRookTargetSquare((int)sideToMove));
    }

    [TestCase(Side.White, Constants.WhiteRookQueensideCastlingSquare)]
    [TestCase(Side.Black, Constants.BlackRookQueensideCastlingSquare)]
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

    [TestCase(BoardSquare.e4, BoardSquare.d4, 1)]
    [TestCase(BoardSquare.e4, BoardSquare.d5, 1)]
    [TestCase(BoardSquare.e4, BoardSquare.d3, 1)]
    [TestCase(BoardSquare.e4, BoardSquare.e5, 1)]
    [TestCase(BoardSquare.e4, BoardSquare.e3, 1)]
    [TestCase(BoardSquare.e4, BoardSquare.f4, 1)]
    [TestCase(BoardSquare.e4, BoardSquare.f5, 1)]
    [TestCase(BoardSquare.e4, BoardSquare.f3, 1)]

    [TestCase(BoardSquare.e4, BoardSquare.c2, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.c3, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.c4, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.c5, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.c6, 2)]

    [TestCase(BoardSquare.e4, BoardSquare.d2, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.e2, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.f2, 2)]

    [TestCase(BoardSquare.e4, BoardSquare.d6, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.e6, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.f6, 2)]

    [TestCase(BoardSquare.e4, BoardSquare.g2, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.g3, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.g4, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.g5, 2)]
    [TestCase(BoardSquare.e4, BoardSquare.g6, 2)]

    [TestCase(BoardSquare.a1, BoardSquare.a8, 7)]
    [TestCase(BoardSquare.a1, BoardSquare.h1, 7)]
    [TestCase(BoardSquare.a1, BoardSquare.h8, 7)]

    // Reversed

    [TestCase(BoardSquare.d4, BoardSquare.e4, 1)]
    [TestCase(BoardSquare.d5, BoardSquare.e4, 1)]
    [TestCase(BoardSquare.d3, BoardSquare.e4, 1)]
    [TestCase(BoardSquare.e5, BoardSquare.e4, 1)]
    [TestCase(BoardSquare.e3, BoardSquare.e4, 1)]
    [TestCase(BoardSquare.f4, BoardSquare.e4, 1)]
    [TestCase(BoardSquare.f5, BoardSquare.e4, 1)]
    [TestCase(BoardSquare.f3, BoardSquare.e4, 1)]

    [TestCase(BoardSquare.c2, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.c3, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.c4, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.c5, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.c6, BoardSquare.e4, 2)]

    [TestCase(BoardSquare.d2, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.e2, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.f2, BoardSquare.e4, 2)]

    [TestCase(BoardSquare.d6, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.e6, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.f6, BoardSquare.e4, 2)]

    [TestCase(BoardSquare.g2, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.g3, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.g4, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.g5, BoardSquare.e4, 2)]
    [TestCase(BoardSquare.g6, BoardSquare.e4, 2)]

    [TestCase(BoardSquare.a8, BoardSquare.a1, 7)]
    [TestCase(BoardSquare.h1, BoardSquare.a1, 7)]
    [TestCase(BoardSquare.h8, BoardSquare.a1, 7)]
    public void ChebyshevDistance(BoardSquare square1, BoardSquare square2, int expectedDistance)
    {
        Assert.AreEqual(expectedDistance, Constants.ChebyshevDistance[(int)square1][(int)square2]);
    }

    [Test]
    public void CalculateNps()
    {
        Assert.AreEqual(1, Utils.CalculateNps(0, 0));
    }

    [TestCase(Piece.P, false)]
    [TestCase(Piece.N, true)]
    [TestCase(Piece.B, true)]
    [TestCase(Piece.R, false)]
    [TestCase(Piece.Q, false)]
    [TestCase(Piece.K, false)]
    [TestCase(Piece.p, false)]
    [TestCase(Piece.n, true)]
    [TestCase(Piece.b, true)]
    [TestCase(Piece.r, false)]
    [TestCase(Piece.q, false)]
    [TestCase(Piece.k, false)]
    [TestCase(Piece.None, false)]
    public void IsMinorPiece(Piece piece, bool isMinor)
    {
        Assert.AreEqual(isMinor, Utils.IsMinorPiece((int)piece));
    }

    [TestCase(Piece.P, false)]
    [TestCase(Piece.N, false)]
    [TestCase(Piece.B, false)]
    [TestCase(Piece.R, true)]
    [TestCase(Piece.Q, true)]
    [TestCase(Piece.K, false)]
    [TestCase(Piece.p, false)]
    [TestCase(Piece.n, false)]
    [TestCase(Piece.b, false)]
    [TestCase(Piece.r, true)]
    [TestCase(Piece.q, true)]
    [TestCase(Piece.k, false)]
    [TestCase(Piece.None, false)]
    public void IsMajorPiece(Piece piece, bool isMinor)
    {
        Assert.AreEqual(isMinor, Utils.IsMajorPiece((int)piece));
    }
}
