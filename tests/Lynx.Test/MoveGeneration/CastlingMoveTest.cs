using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class CastlingMoveTest
{
    private static readonly int _whiteShortCastle = MoveExtensions.EncodeShortCastle(Constants.InitialWhiteKingSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K);
    private static readonly int _whiteLongCastle = MoveExtensions.EncodeLongCastle(Constants.InitialWhiteKingSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K);
    private static readonly int _blackShortCastle = MoveExtensions.EncodeShortCastle(Constants.InitialBlackKingSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k);
    private static readonly int _blackLongCastle = MoveExtensions.EncodeLongCastle(Constants.InitialBlackKingSquare, Constants.BlackLongCastleKingSquare, (int)Piece.k);

    [Test]
    public void WhiteShortCastling()
    {
        var position = new Position("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w K - 0 1");

        Span<Move> moveSpan = stackalloc Move[2];
        var index = 0;
        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position);

        var move = moveSpan[0];
        Assert.IsTrue(move.IsCastle());
        Assert.AreEqual(_whiteShortCastle, move);
    }

    [Test]
    public void WhiteLongCastling()
    {
        var position = new Position("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w Q - 0 1");

        Span<Move> moveSpan = stackalloc Move[2];
        var index = 0;
        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position);

        var move = moveSpan[0];
        Assert.IsTrue(move.IsCastle());
        Assert.AreEqual(_whiteLongCastle, move);
    }

    [Test]
    public void BlackShortCastling()
    {
        var position = new Position("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b k - 0 1");

        Span<Move> moveSpan = stackalloc Move[2];
        var index = 0;
        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position);

        var move = moveSpan[0];
        Assert.IsTrue(move.IsCastle());
        Assert.AreEqual(_blackShortCastle, move);
    }

    [Test]
    public void BlackLongCastling()
    {
        var position = new Position("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b q - 0 1");

        Span<Move> moveSpan = stackalloc Move[2];
        var index = 0;
        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position);

        var move = moveSpan[0];
        Assert.IsTrue(move.IsCastle());
        Assert.AreEqual(_blackLongCastle, move);
    }
}
