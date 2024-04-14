using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class GenerateKnightMovesTest
{
    private static IEnumerable<Move> GenerateKnightMoves(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        return MoveGenerator.GenerateAllMoves(position, moves, position.IsInCheck()).ToArray().Where(m => m.Piece() == (int)Piece.N || m.Piece() == (int)Piece.n);
    }

    private static IEnumerable<Move> GenerateKnightCaptures(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        return MoveGenerator.GenerateAllCaptures(position, moves, position.IsInCheck()).ToArray().Where(m => m.Piece() == (int)Piece.N || m.Piece() == (int)Piece.n);
    }

    [TestCase(Constants.InitialPositionFEN, 4)]
    [TestCase("k7/8/8/8/8/P1P2P1P/PPPPPPPP/RNBQKBNR w KQ - 0 1", 0)]
    [TestCase("rnbqkbnr/pppppppp/p1p2p1p/8/8/8/8/8 b KQkq - 0 1", 0)]
    [TestCase("k7/8/2P1P3/1P3P2/3N4/1P3P2/2P1P3/K7 w - - 0 1", 0)]
    [TestCase("k7/8/2p1p3/1p3p2/3N4/1p3p2/2p1p3/K7 w - - 0 1", 8)]
    [TestCase("k7/8/2p1p3/1p3p2/3n4/1p3p2/2p1p3/K7 b - - 0 1", 0)]
    [TestCase("k7/8/2P1P3/1P3P2/3n4/1P3P2/2P1P3/K7 b - - 0 1", 8)]
    public void KnightMoves_Count(string fen, int expectedMoves)
    {
        var position = new Position(fen);
        var moves = GenerateKnightMoves(position);

        Assert.AreEqual(expectedMoves, moves.Count());

        Assert.AreEqual(moves, ReferenceMoveGenerator.GenerateKnightMoves(position));
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   p . p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . p
    /// 2   P P P B B P P P
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void KnightMoves_White()
    {
        var position = new Position(Constants.TrickyTestPositionFEN);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.N + offset;
        var moves = GenerateKnightMoves(position);

        Assert.AreEqual(11, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.c3
            && m.TargetSquare() == (int)BoardSquare.a4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.c3
            && m.TargetSquare() == (int)BoardSquare.b5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.c3
            && m.TargetSquare() == (int)BoardSquare.b1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.c3
            && m.TargetSquare() == (int)BoardSquare.d1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.c4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.c6));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.g4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.d3));

        Assert.AreEqual(1, moves.Count(m =>
           m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.d7));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.f7));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.g6));
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   p . p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . p
    /// 2   P P P B B P P P
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       Black
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void KnightMoves_Black()
    {
        var position = new Position(Constants.TrickyTestPositionReversedFEN);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.N + offset;
        var moves = GenerateKnightMoves(position);

        Assert.AreEqual(10, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b6
            && m.TargetSquare() == (int)BoardSquare.c8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b6
            && m.TargetSquare() == (int)BoardSquare.d5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b6
            && m.TargetSquare() == (int)BoardSquare.c4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b6
            && m.TargetSquare() == (int)BoardSquare.a4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f6
            && m.TargetSquare() == (int)BoardSquare.g8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f6
            && m.TargetSquare() == (int)BoardSquare.h7));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f6
            && m.TargetSquare() == (int)BoardSquare.h5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f6
            && m.TargetSquare() == (int)BoardSquare.g4));

        Assert.AreEqual(1, moves.Count(m =>
           m.SourceSquare() == (int)BoardSquare.f6
            && m.TargetSquare() == (int)BoardSquare.e4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f6
            && m.TargetSquare() == (int)BoardSquare.d5));
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   p . p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . p
    /// 2   P P P B B P P P
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void KnightMoves_CapturesOnly_White()
    {
        var position = new Position(Constants.TrickyTestPositionFEN);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.N + offset;
        var moves = GenerateKnightCaptures(position);

        Assert.AreEqual(3, moves.Count(m => m.Piece() == piece && m.IsCapture()));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.d7));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.f7));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e5
            && m.TargetSquare() == (int)BoardSquare.g6));
    }

    /// <summary>
    /// 8   r . . . k . . r
    /// 7   p . p p q p b .
    /// 6   b n . . p n p .
    /// 5   . . . P N . . .
    /// 4   . p . . P . . .
    /// 3   . . N . . Q . p
    /// 2   P P P B B P P P
    /// 1   R . . . K . . R
    ///     a b c d e f g h
    ///     Side:       Black
    ///     Enpassant:  no
    ///     Castling:   KQ | kq
    /// </summary>
    [Test]
    public void KnightMoves_CapturesOnly_Black()
    {
        var position = new Position(Constants.TrickyTestPositionReversedFEN);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.N + offset;
        var moves = GenerateKnightCaptures(position);

        Assert.AreEqual(3, moves.Count(m => m.Piece() == piece && m.IsCapture()));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b6
            && m.TargetSquare() == (int)BoardSquare.d5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f6
            && m.TargetSquare() == (int)BoardSquare.e4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f6
            && m.TargetSquare() == (int)BoardSquare.d5));
    }
}
