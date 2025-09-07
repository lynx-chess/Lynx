using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class GenerateQueenMovesTest
{
    private static IEnumerable<Move> GenerateQueenMoves(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];

        var evaluationContext = new EvaluationContext(attacks, attacksBySide);
        evaluationContext.EnsureThreatsAreCalculated(position);

        return MoveGenerator.GenerateAllMoves(position, ref evaluationContext, moves).ToArray().Where(m => m.Piece() == (int)Piece.Q || m.Piece() == (int)Piece.q);
    }

    private static IEnumerable<Move> GenerateQueenCaptures(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];

        var evaluationContext = new EvaluationContext(attacks, attacksBySide);
        evaluationContext.EnsureThreatsAreCalculated(position);

        return MoveGenerator.GenerateAllCaptures(position, ref evaluationContext, moves).ToArray().Where(m => m.Piece() == (int)Piece.Q || m.Piece() == (int)Piece.q);
    }

    [TestCase(Constants.InitialPositionFEN, 0)]
    [TestCase("1k6/8/8/8/8/8/PP3PPP/RNBQKBNR w KQ - 0 1", 14)]
    [TestCase("rnbqkbnr/pp3ppp/8/8/8/8/8/1K6 b kq - 0 1", 14)]
    [TestCase("1k6/pppppppp/8/8/8/8/PP3PPP/RNBQKBNR w KQ - 0 1", 13)]
    [TestCase("rnbqkbnr/pp3ppp/8/8/8/8/PPPPPPPP/1K6 b kq - 0 1", 13)]
    [TestCase("K1k5/8/8/8/3Q4/8/8/8 w - - 0 1", 27)]
    [TestCase("K1k5/8/8/8/3q4/8/8/8 b - - 0 1", 27)]
    [TestCase("K1k5/8/1P1P1P2/8/1P1Q1P2/8/1P1P1P2/8 w - - 0 1", 8)]
    [TestCase("K1k5/8/1p1p1p2/8/1p1q1p2/8/1p1p1p2/8 b - - 0 1", 8)]
    [TestCase("K1k5/8/1p1p1p2/8/1p1Q1p2/8/1p1p1p2/8 w - - 0 1", 16)]
    [TestCase("K1k5/8/1P1P1P2/8/1P1q1P2/8/1P1P1P2/8 b - - 0 1", 16)]
    public void QueenMoves_Count(string fen, int expectedMoves)
    {
        var position = new Position(fen);
        var moves = GenerateQueenMoves(position);

        Assert.AreEqual(expectedMoves, moves.Count());

        Assert.AreEqual(moves, ReferenceMoveGenerator.GenerateQueenMoves(position));
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
    public void QueenMoves_White()
    {
        var position = new Position(Constants.TrickyTestPositionFEN);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.Q + offset;
        var moves = GenerateQueenMoves(position);

        Assert.AreEqual(9, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f3
            && m.TargetSquare() == (int)BoardSquare.e3));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f3
            && m.TargetSquare() == (int)BoardSquare.d3));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f3
            && m.TargetSquare() == (int)BoardSquare.f4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f3
            && m.TargetSquare() == (int)BoardSquare.f5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f3
            && m.TargetSquare() == (int)BoardSquare.f6));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f3
            && m.TargetSquare() == (int)BoardSquare.g4));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f3
            && m.TargetSquare() == (int)BoardSquare.h5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f3
            && m.TargetSquare() == (int)BoardSquare.g3));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.f3
            && m.TargetSquare() == (int)BoardSquare.h3));
    }

    /// <summary>
    /// 8   r . . . k R . r
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
    public void QueenMoves_Black()
    {
        var position = new Position("r3kR1r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.Q + offset;
        var moves = GenerateQueenMoves(position);

        Assert.AreEqual(4, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e7
            && m.TargetSquare() == (int)BoardSquare.d8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e7
            && m.TargetSquare() == (int)BoardSquare.f8));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e7
            && m.TargetSquare() == (int)BoardSquare.d6));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.e7
            && m.TargetSquare() == (int)BoardSquare.c5));
    }

    [TestCase("K1k5/8/1p1p1p2/8/1p1Q1p2/8/1p1p1p2/8 w - - 0 1", 8)]
    [TestCase("K1k5/8/1P1P1P2/8/1P1q1P2/8/1P1P1P2/8 b - - 0 1", 8)]
    [TestCase("n1n1n3/8/p1Q1p3/8/p1p1p1p1/8/4p1Q1/K1k4n w - - 0 1", 12)]
    [TestCase("N1N1N3/8/P1q1P3/8/P1P1P1P1/8/4P1q1/K1k4N b - - 0 1", 12)]
    public void QueenMoves_CapturesOnly(string fen, int expectedCaptures)
    {
        var position = new Position(fen);
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.Q + offset;
        var moves = GenerateQueenCaptures(position);

        Assert.AreEqual(expectedCaptures, moves.Count(m => m.Piece() == piece && m.CapturedPiece() != (int)Piece.None));
    }
}
