using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class GenerateKingMovesTest
{
    private static IEnumerable<Move> GenerateKingMoves(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        return position.GenerateAllMoves(moves).ToArray().Where(m => m.Piece() == (int)Piece.K || m.Piece() == (int)Piece.k);
    }

    private static IEnumerable<Move> GenerateKingCaptures(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        return position.GenerateAllCaptures(moves).ToArray().Where(m => m.Piece() == (int)Piece.K || m.Piece() == (int)Piece.k);
    }

    [TestCase(Constants.InitialPositionFEN, 0)]
    [TestCase("8/8/8/2PPP3/2PKP3/2P1P3/8/8 w - - 0 1", 1)]
    [TestCase("8/8/8/2PPP3/2PKP3/3PP3/8/8 w - - 0 1", 1)]
    [TestCase("8/8/8/2PPP3/2PKP3/2PP4/8/8 w - - 0 1", 1)]
    [TestCase("8/8/8/2PPP3/2PKP3/3P4/8/8 w - - 0 1", 2)]
    [TestCase("8/8/8/2PPP3/2PKP3/8/8/8 w - - 0 1", 3)]
    [TestCase("8/8/8/2PPP3/2PK4/8/8/8 w - - 0 1", 4)]
    [TestCase("8/8/8/2PPP3/3K4/8/8/8 w - - 0 1", 5)]
    [TestCase("8/8/8/2P1P3/3K4/8/8/8 w - - 0 1", 6)]
    [TestCase("8/8/8/4P3/3K4/8/8/8 w - - 0 1", 7)]
    [TestCase("8/8/8/8/3K4/8/8/8 w - - 0 1", 8)]
    public void KingMoves_Count(string fen, int expectedMoves)
    {
        var position = new Position(fen);
        var moves = GenerateKingMoves(position);

        Assert.AreEqual(moves, ReferenceMoveGenerator.GenerateKingMoves(position));
        Assert.AreEqual(expectedMoves, moves.Count());
    }

    /// <summary>
    /// 8   . . . . . . . .
    /// 7   . . . . . . . .
    /// 6   . . . . . . k .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . N . . . . .
    /// 2   n K r . . . . .
    /// 1   b p . . . . . .
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   -- | --
    ///     FEN:        8/8/6k1/8/8/2N5/nKr5/bp6 w - - 0 1
    /// </summary>
    [Test]
    public void KingMoves_White()
    {
        var position = new Position("8/8/6k1/8/8/2N5/nKr5/bp6 w - - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.K + offset;
        var moves = GenerateKingMoves(position);

        Assert.AreEqual(7, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.b3));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.a3));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.a2));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.a1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.b1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.c1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.c2));
    }

    /// <summary>
    /// 8   . . . . . . . .
    /// 7   . . . . . . n Q
    /// 6   . . . . . . k .
    /// 5   . . . . . P B .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . . .
    /// 1   K . . . . . . .
    ///     a b c d e f g h
    ///     Side:       Black
    ///     Enpassant:  no
    ///     Castling:   -- | --
    ///     FEN:        8/6nQ/6k1/5PB1/8/8/8/K7 b - - 0 1
    /// </summary>
    [Test]
    public void KingMoves_Black()
    {
        var position = new Position("8/6nQ/6k1/5PB1/8/8/8/K7 b - - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.K + offset;
        var moves = GenerateKingMoves(position);

        Assert.AreEqual(7, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.h7));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.h6));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.h5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.g5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.f5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.f6));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.f7));
    }

    /// <summary>
    /// 8   . . . . . . . .
    /// 7   . . . . . . . .
    /// 6   . . . . . . k .
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   . . N . . . . .
    /// 2   n K r . . . . .
    /// 1   b p . . . . . .
    ///     a b c d e f g h
    ///     Side:       White
    ///     Enpassant:  no
    ///     Castling:   -- | --
    ///     FEN:        8/8/6k1/8/8/2N5/nKr5/bp6 w - - 0 1
    /// </summary>
    [Test]
    public void KingMoves_CapturesOnly_White()
    {
        var position = new Position("8/8/6k1/8/8/2N5/nKr5/bp6 w - - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.K + offset;
        var moves = GenerateKingCaptures(position);

        Assert.AreEqual(4, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.a2));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.a1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.b1));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.b2
            && m.TargetSquare() == (int)BoardSquare.c2));
    }

    /// <summary>
    /// 8   . . . . . . . .
    /// 7   . . . . . . n Q
    /// 6   . . . . . . k .
    /// 5   . . . . . P B .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   . . . . . . . .
    /// 1   K . . . . . . .
    ///     a b c d e f g h
    ///     Side:       Black
    ///     Enpassant:  no
    ///     Castling:   -- | --
    ///     FEN:        8/6nQ/6k1/5PB1/8/8/8/K7 b - - 0 1
    /// </summary>
    [Test]
    public void KingMoves_CapturesOnly_Black()
    {
        var position = new Position("8/6nQ/6k1/5PB1/8/8/8/K7 b - - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var piece = (int)Piece.K + offset;
        var moves = GenerateKingCaptures(position);

        Assert.AreEqual(3, moves.Count(m => m.Piece() == piece));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.h7));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.g5));

        Assert.AreEqual(1, moves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.g6
            && m.TargetSquare() == (int)BoardSquare.f5));
    }
}
