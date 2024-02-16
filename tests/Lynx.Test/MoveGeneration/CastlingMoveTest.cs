using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;
public class CastlingMoveTest
{
    [Test]
    public void WhiteShortCastling()
    {
        Assert.AreEqual(PregeneratedMoves.WhiteShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K));

        Assert.AreEqual(PregeneratedMoves.WhiteShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.White)));
    }

    [Test]
    public void WhiteLongCastling()
    {
        Assert.AreEqual(PregeneratedMoves.WhiteLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K));

        Assert.AreEqual(PregeneratedMoves.WhiteLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.White)));
    }

    [Test]
    public void BlackShortCastling()
    {
        Assert.AreEqual(PregeneratedMoves.BlackShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k));

        Assert.AreEqual(PregeneratedMoves.BlackShortCastle,
            MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.Black)));
    }

    [Test]
    public void BlackLongCastling()
    {
        Assert.AreEqual(PregeneratedMoves.BlackLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.k));

        Assert.AreEqual(PregeneratedMoves.BlackLongCastle,
            MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.Black)));
    }
}
