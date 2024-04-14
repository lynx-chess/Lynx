using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class GenerateBishopMovesTest
{
    private static IEnumerable<Move> GenerateBishopMoves(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        return MoveGenerator.GenerateAllMoves(position, moves, position.IsInCheck()).ToArray().Where(m => m.Piece() == (int)Piece.B || m.Piece() == (int)Piece.b);
    }

    private static IEnumerable<Move> GenerateBishopCaptures(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        return MoveGenerator.GenerateAllCaptures(position, moves, position.IsInCheck()).ToArray().Where(m => m.Piece() == (int)Piece.B || m.Piece() == (int)Piece.b);
    }

    [TestCase(Constants.InitialPositionFEN, 0)]
    [TestCase("k7/8/8/8/8/8/P1P2P1P/RNBQKBNR w KQ - 0 1", 14)]
    [TestCase("rnbqkbnr/p1p2p1p/8/8/8/8/8/8 b KQkq - 0 1", 14)]
    [TestCase("8/k7/8/3B4/8/8/8/K7 w - - 0 1", 13)]
    [TestCase("8/k7/8/3b4/8/8/8/K7 b - - 0 1", 13)]
    [TestCase("8/k1N1N3/1N1P1N2/2PBP3/1N1P1N2/2N1N3/3N4/K7 w - - 0 1", 13)]
    [TestCase("1k6/2n1n3/1n1p1n2/2pbp3/1n1p1n2/2n1n3/3n4/K7 b - - 0 1", 13)]
    public void BishopMoves_Count(string fen, int expectedMoves)
    {
        var position = new Position(fen);
        var moves = GenerateBishopMoves(position);

        Assert.AreEqual(expectedMoves, moves.Count());

        Assert.AreEqual(moves, ReferenceMoveGenerator.GenerateBishopMoves(position));
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
    public void BishopMoves_White()
    {
        var position = new Position(Constants.TrickyTestPositionFEN);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.B + offset;
        var moves = GenerateBishopMoves(position);

        Assert.AreEqual(11, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.d2
            && m.TargetSquare() == (int)BoardSquare.c1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.d2
            && m.TargetSquare() == (int)BoardSquare.e3));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.d2
            && m.TargetSquare() == (int)BoardSquare.f4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.d2
            && m.TargetSquare() == (int)BoardSquare.g5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.d2
            && m.TargetSquare() == (int)BoardSquare.h6));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e2
            && m.TargetSquare() == (int)BoardSquare.d1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e2
            && m.TargetSquare() == (int)BoardSquare.f1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e2
            && m.TargetSquare() == (int)BoardSquare.d3));

        Assert.AreEqual(1, moves.Count(m =>
           m.SourceSquare() == (int)BoardSquare.e2
            && m.TargetSquare() == (int)BoardSquare.c4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e2
            && m.TargetSquare() == (int)BoardSquare.b5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e2
            && m.TargetSquare() == (int)BoardSquare.a6));
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
    public void BishopMoves_Black()
    {
        var position = new Position(Constants.TrickyTestPositionReversedFEN);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.B + offset;
        var moves = GenerateBishopMoves(position);

        Assert.AreEqual(8, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a6
            && m.TargetSquare() == (int)BoardSquare.b5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a6
            && m.TargetSquare() == (int)BoardSquare.c4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a6
            && m.TargetSquare() == (int)BoardSquare.d3));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a6
            && m.TargetSquare() == (int)BoardSquare.e2));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a6
            && m.TargetSquare() == (int)BoardSquare.b7));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a6
            && m.TargetSquare() == (int)BoardSquare.c8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g7
            && m.TargetSquare() == (int)BoardSquare.f8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g7
            && m.TargetSquare() == (int)BoardSquare.h6));
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
    public void BishopMoves_CapturesOnly_White()
    {
        var position = new Position(Constants.TrickyTestPositionFEN);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.B + offset;
        var moves = GenerateBishopCaptures(position);

        Assert.AreEqual(1, moves.Count(m => m.Piece() == piece && m.IsCapture()));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e2
            && m.TargetSquare() == (int)BoardSquare.a6));
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
    public void BishopMoves_CapturesOnly_Black()
    {
        var position = new Position(Constants.TrickyTestPositionReversedFEN);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.B + offset;
        var moves = GenerateBishopCaptures(position);

        Assert.AreEqual(1, moves.Count(m => m.Piece() == piece && m.IsCapture()));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a6
            && m.TargetSquare() == (int)BoardSquare.e2));
    }
}
