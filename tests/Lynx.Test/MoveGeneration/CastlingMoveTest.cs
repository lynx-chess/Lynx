using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class CastlingMoveTest
{
    private static readonly int _whiteShortCastle = MoveExtensions.EncodeShortCastle(Constants.InitialWhiteKingSquare, Constants.WhiteKingShortCastleSquare, (int)Piece.K);
    private static readonly int _whiteLongCastle = MoveExtensions.EncodeLongCastle(Constants.InitialWhiteKingSquare, Constants.WhiteKingLongCastleSquare, (int)Piece.K);
    private static readonly int _blackShortCastle = MoveExtensions.EncodeShortCastle(Constants.InitialBlackKingSquare, Constants.BlackKingShortCastleSquare, (int)Piece.k);
    private static readonly int _blackLongCastle = MoveExtensions.EncodeLongCastle(Constants.InitialBlackKingSquare, Constants.BlackKingLongCastleSquare, (int)Piece.k);

    [Test]
    public void WhiteShortCastling()
    {
        var position = new Position("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w K - 0 1");

        Span<Move> moveSpan = stackalloc Move[2];
        var index = 0;

        Span<BitBoard> buffer = stackalloc BitBoard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position, ref evaluationContext);

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

        Span<BitBoard> buffer = stackalloc BitBoard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position, ref evaluationContext);

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

        Span<BitBoard> buffer = stackalloc BitBoard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position, ref evaluationContext);

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

        Span<BitBoard> buffer = stackalloc BitBoard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position, ref evaluationContext);

        var move = moveSpan[0];
        Assert.IsTrue(move.IsCastle());
        Assert.AreEqual(_blackLongCastle, move);
    }
}
