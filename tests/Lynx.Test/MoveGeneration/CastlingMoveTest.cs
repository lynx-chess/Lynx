using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;
public class CastlingMoveTest
{
    [Test]
    public void WhiteShortCastling()
    {
        Assert.AreEqual(IMoveGenerator.WhiteShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K));

        Assert.AreEqual(IMoveGenerator.WhiteShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.White)));
    }

    [Test]
    public void WhiteLongCastling()
    {
        Assert.AreEqual(IMoveGenerator.WhiteLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K));

        Assert.AreEqual(IMoveGenerator.WhiteLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.White)));
    }

    [Test]
    public void BlackShortCastling()
    {
        Assert.AreEqual(IMoveGenerator.BlackShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k));

        Assert.AreEqual(IMoveGenerator.BlackShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.Black)));
    }

    [Test]
    public void BlackLongCastling()
    {
        Assert.AreEqual(IMoveGenerator.BlackLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.k));

        Assert.AreEqual(IMoveGenerator.BlackLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.Black)));
    }
}
