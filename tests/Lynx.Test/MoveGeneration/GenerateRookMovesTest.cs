using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class GenerateRookMovesTest
{
    [TestCase(Constants.EmptyBoardFEN, 0)]
    [TestCase(Constants.InitialPositionFEN, 0)]
    [TestCase("8/8/2n1n3/3R4/2n1n3/8/8/8 w - - 0 1", 14)]
    [TestCase("8/8/2N1N3/3r4/2N1N3/8/8/8 b - - 0 1", 14)]
    [TestCase("8/8/2n1n3/3R4/2n1n3/8/8/3q4 w - - 0 1", 14)]
    [TestCase("8/8/2N1N3/3r4/2N1N3/8/8/3Q4 b - - 0 1", 14)]
    [TestCase("8/8/2n1n3/3R4/2n1n3/8/8/3Q4 w - - 0 1", 13)]
    [TestCase("8/8/2N1N3/3r4/2N1N3/8/8/3q4 b - - 0 1", 13)]
    public void RookMoves_Count(string fen, int expectedMoves)
    {
        var position = new Position(fen);
        var offset = Utils.PieceOffset(position.Side);
        var moves = MoveGenerator.GeneratePieceMovesForReference((int)Piece.R + offset, position);

        Assert.AreEqual(expectedMoves, moves.Count());

        Assert.AreEqual(moves, MoveGenerator.GenerateRookMoves(position));
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   p . p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . P
    /// 2   . P P B B P P .
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void RookMoves_White()
    {
        var position = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1P/1PPBBPP1/R3K2R w KQkq - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.R + offset;
        var moves = MoveGenerator.GeneratePieceMovesForReference(piece, position);

        Assert.AreEqual(11, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a1
            && m.TargetSquare() == (int)BoardSquare.a2));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a1
            && m.TargetSquare() == (int)BoardSquare.a3));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a1
            && m.TargetSquare() == (int)BoardSquare.a4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a1
            && m.TargetSquare() == (int)BoardSquare.a5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a1
            && m.TargetSquare() == (int)BoardSquare.a6));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a1
            && m.TargetSquare() == (int)BoardSquare.b1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a1
            && m.TargetSquare() == (int)BoardSquare.c1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a1
            && m.TargetSquare() == (int)BoardSquare.d1));

        Assert.AreEqual(1, moves.Count(m =>
           m.SourceSquare() == (int)BoardSquare.h1
            && m.TargetSquare() == (int)BoardSquare.h2));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h1
            && m.TargetSquare() == (int)BoardSquare.g1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h1
            && m.TargetSquare() == (int)BoardSquare.f1));
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   p . p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . P
    /// 2   . P P B B P P .
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       Black
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void RookMoves_Black()
    {
        var position = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1P/1PPBBPP1/R3K2R b KQkq - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.R + offset;
        var moves = MoveGenerator.GeneratePieceMovesForReference(piece, position);

        Assert.AreEqual(10, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a8
            && m.TargetSquare() == (int)BoardSquare.b8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a8
            && m.TargetSquare() == (int)BoardSquare.c8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a8
            && m.TargetSquare() == (int)BoardSquare.d8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h8
            && m.TargetSquare() == (int)BoardSquare.g8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h8
            && m.TargetSquare() == (int)BoardSquare.f8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h8
            && m.TargetSquare() == (int)BoardSquare.h7));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h8
            && m.TargetSquare() == (int)BoardSquare.h6));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h8
            && m.TargetSquare() == (int)BoardSquare.h5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h8
            && m.TargetSquare() == (int)BoardSquare.h4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h8
            && m.TargetSquare() == (int)BoardSquare.h3));
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   p . p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . P
    /// 2   . P P B B P P .
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void RookMoves_CapturesOnly_White()
    {
        var position = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1P/1PPBBPP1/R3K2R w KQkq - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.R + offset;
        var moves = MoveGenerator.GeneratePieceMovesForReference(piece, position, capturesOnly: true);

        Assert.AreEqual(1, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a1
            && m.TargetSquare() == (int)BoardSquare.a6
            && m.IsCapture() != default));
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   p . p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . P
    /// 2   . P P B B P P .
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       Black
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void RookMoves_CapturesOnly_Black()
    {
        var position = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1P/1PPBBPP1/R3K2R b KQkq - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.R + offset;
        var moves = MoveGenerator.GeneratePieceMovesForReference(piece, position, capturesOnly: true);

        Assert.AreEqual(1, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.h8
            && m.TargetSquare() == (int)BoardSquare.h3));
    }
}
