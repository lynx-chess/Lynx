using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class CastlingMoveTest
{
    private static readonly int _whiteShortCastle = MoveExtensions.EncodeCastle(Constants.InitialWhiteKingSquare, Constants.WhiteKingShortCastleSquare);
    private static readonly int _whiteLongCastle = MoveExtensions.EncodeCastle(Constants.InitialWhiteKingSquare, Constants.WhiteKingLongCastleSquare);
    private static readonly int _blackShortCastle = MoveExtensions.EncodeCastle(Constants.InitialBlackKingSquare, Constants.BlackKingShortCastleSquare);
    private static readonly int _blackLongCastle = MoveExtensions.EncodeCastle(Constants.InitialBlackKingSquare, Constants.BlackKingLongCastleSquare);

    [Test]
    public void WhiteShortCastling()
    {
        var position = new Position("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w K - 0 1");

        Span<Move> moveSpan = stackalloc Move[2];
        var index = 0;

        const Bitboard oppositeSideAttacks = 0UL;

        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position, oppositeSideAttacks);

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

        const Bitboard oppositeSideAttacks = 0UL;

        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position, oppositeSideAttacks);

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

        const Bitboard oppositeSideAttacks = 0UL;

        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position, oppositeSideAttacks);

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

        const Bitboard oppositeSideAttacks = 0UL;

        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position, oppositeSideAttacks);

        var move = moveSpan[0];
        Assert.IsTrue(move.IsCastle());
        Assert.AreEqual(_blackLongCastle, move);
    }
}
