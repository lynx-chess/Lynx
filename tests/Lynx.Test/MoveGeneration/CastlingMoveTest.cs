using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;
public class CastlingMoveTest
{
    [Test]
    public void WhiteShortCastling()
    {
        Assert.AreEqual(MoveGenerator.WhiteShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K));

        Assert.AreEqual(MoveGenerator.WhiteShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.White)));
    }

    [Test]
    public void WhiteLongCastling()
    {
        Assert.AreEqual(MoveGenerator.WhiteLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K));

        Assert.AreEqual(MoveGenerator.WhiteLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.White)));
    }

    [Test]
    public void BlackShortCastling()
    {
        Assert.AreEqual(MoveGenerator.BlackShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k));

        Assert.AreEqual(MoveGenerator.BlackShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.Black)));
    }

    [Test]
    public void BlackLongCastling()
    {
        Assert.AreEqual(MoveGenerator.BlackLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.k));

        Assert.AreEqual(MoveGenerator.BlackLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.Black)));
    }
}
