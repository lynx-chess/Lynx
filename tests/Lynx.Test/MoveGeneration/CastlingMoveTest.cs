using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;
public class CastlingMoveTest
{
    [Test]
    public void WhiteShortCastling()
    {
        Assert.AreEqual(MoveGenerator.WhiteShortCastle,
            MoveExtensions.Encode(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.White), isShortCastle: 1));
    }

    [Test]
    public void WhiteLongCastling()
    {
        Assert.AreEqual(MoveGenerator.WhiteLongCastle,
            MoveExtensions.Encode(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.White), isLongCastle: 1));
    }

    [Test]
    public void BlackShortCastling()
    {
        Assert.AreEqual(MoveGenerator.BlackShortCastle,
            MoveExtensions.Encode(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.Black), isShortCastle: 1));
    }

    [Test]
    public void BlackLongCastling()
    {
        Assert.AreEqual(MoveGenerator.BlackLongCastle,
            MoveExtensions.Encode(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(Side.Black), isLongCastle: 1));
    }
}
