using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

#pragma warning disable RCS1098 // Constant values should be placed on right side of comparisons.
public class GeneratePawnMovesTest
{
    private static IEnumerable<Move> GeneratePawnMoves(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        using var evaluationContext = new EvaluationContext();
        return MoveGenerator.GenerateAllMoves(position, evaluationContext, moves).ToArray().Where(m => m.Piece() % (int)Piece.p == 0);
    }

    private static IEnumerable<Move> GeneratePawnCaptures(Position position)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        using var evaluationContext = new EvaluationContext();
        return MoveGenerator.GenerateAllCaptures(position, evaluationContext, moves).ToArray().Where(m => m.Piece() % (int)Piece.p == 0);
    }

    [Test]
    public void QuietMoves()
    {
        var position = new Position(Constants.InitialPositionFEN);

        var whiteMoves = GeneratePawnMoves(position);

        for (int square = (int)BoardSquare.a2; square <= (int)BoardSquare.h2; ++square)
        {
            Assert.True(1 == whiteMoves.Count(m =>
                m.SourceSquare() == square
                && m.TargetSquare() == square - 8
                && m.CapturedPiece() == (int)Piece.None));

            Assert.True(1 == whiteMoves.Count(m =>
                 m.SourceSquare() == square
                && m.TargetSquare() == square - 16
                && m.CapturedPiece() == (int)Piece.None));
        }

        Assert.AreEqual(whiteMoves.Count(), ReferenceMoveGenerator.GeneratePawnMovesForReference(position, 0).Count());

        position = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1");
        var blackMoves = GeneratePawnMoves(position);

        for (int square = (int)BoardSquare.a7; square <= (int)BoardSquare.h7; ++square)
        {
            Assert.True(1 == blackMoves.Count(m =>
                m.SourceSquare() == square
                && m.TargetSquare() == square + 8
                && m.CapturedPiece() == (int)Piece.None));

            Assert.True(1 == blackMoves.Count(m =>
                m.SourceSquare() == square
                && m.TargetSquare() == square + 16
                && m.CapturedPiece() == (int)Piece.None));
        }

        Assert.AreEqual(blackMoves.Count(), ReferenceMoveGenerator.GeneratePawnMovesForReference(position, 6).Count());
    }

    [TestCase("K1k5/8/8/8/8/p1p1p1p1/P1P1P1P1/8 w - - 0 1", 0)]
    [TestCase("K2k4/8/8/8/8/R1R1R1R1/P1P1P1P1/8 w - - 0 1", 0)]
    [TestCase("K1k5/8/8/8/p1p1p1p1/8/P1P1P1P1/8 w - - 0 1", 4)]
    [TestCase("K2k4/8/8/8/R1R1R1R1/8/P1P1P1P1/8 w - - 0 1", 4)]
    [TestCase("8/p1p1p1p1/P1P1P1P1/8/8/8/8/K1k5 b - - 0 1", 0)]
    [TestCase("8/p1p1p1p1/r1r1r1r1/8/8/8/8/2k2K2 b - - 0 1", 0)]
    [TestCase("8/p1p1p1p1/8/P1P1P1P1/8/8/8/K1k5 b - - 0 1", 4)]
    [TestCase("8/p1p1p1p1/8/r1r1r1r1/8/8/8/2k2K2 b - - 0 1", 4)]
    public void QuietMoves_NoDoublePush(string fen, int expectedMoves)
    {
        var position = new Position(fen);
        var moves = GeneratePawnMoves(position);
        Assert.AreEqual(expectedMoves, moves.Count());
    }

    [Test]
    public void PawnCaptures()
    {
        var position = new Position("K1k5/8/8/8/8/1n6/PPP5/8 w - - 0 1");

        var whiteMoves = GeneratePawnMoves(position).ToList();

        Assert.AreEqual(6, whiteMoves.Count);
        Assert.True(1 == whiteMoves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a2
            && m.TargetSquare() == (int)BoardSquare.b3
            && m.CapturedPiece() != (int)Piece.None));

        Assert.True(1 == whiteMoves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.c2
            && m.TargetSquare() == (int)BoardSquare.b3
            && m.CapturedPiece() != (int)Piece.None));

        whiteMoves = [.. GeneratePawnCaptures(position)];

        Assert.True(1 == whiteMoves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a2
            && m.TargetSquare() == (int)BoardSquare.b3
            && m.CapturedPiece() != (int)Piece.None));

        Assert.True(1 == whiteMoves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.c2
            && m.TargetSquare() == (int)BoardSquare.b3
            && m.CapturedPiece() != (int)Piece.None));

        position = new Position("8/ppp/1B6/8/8/8/8/K1k5 b - - 0 1");

        var blackMoves = GeneratePawnMoves(position).ToList();
        Assert.AreEqual(6, blackMoves.Count);
        Assert.True(1 == blackMoves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a7
            && m.TargetSquare() == (int)BoardSquare.b6
            && m.CapturedPiece() != (int)Piece.None));

        Assert.True(1 == blackMoves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.c7
            && m.TargetSquare() == (int)BoardSquare.b6
             && m.CapturedPiece() != (int)Piece.None));

        blackMoves = [.. GeneratePawnCaptures(position)];

        Assert.True(1 == blackMoves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.a7
            && m.TargetSquare() == (int)BoardSquare.b6
            && m.CapturedPiece() != (int)Piece.None));

        Assert.True(1 == blackMoves.Count(m =>
            m.SourceSquare() == (int)BoardSquare.c7
            && m.TargetSquare() == (int)BoardSquare.b6
             && m.CapturedPiece() != (int)Piece.None));
    }

    [Test]
    public void PromotionsWithoutCapturing()
    {
        var position = new Position("8/P6P/8/8/K1k5/8/p6p/8 w - - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var whiteMoves = GeneratePawnMoves(position).ToList();

        Assert.AreEqual(8, whiteMoves.Count);
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset && m.CapturedPiece() == (int)Piece.None));
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset && m.CapturedPiece() == (int)Piece.None));
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset && m.CapturedPiece() == (int)Piece.None));
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset && m.CapturedPiece() == (int)Piece.None));
        Assert.AreEqual(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.a8));
        Assert.AreEqual(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.h8));

        position = new Position("8/P6P/8/8/K1k5/8/p6p/8 b - - 0 1");
        offset = Utils.PieceOffset(position.Side);
        var blackMoves = GeneratePawnMoves(position).ToList();

        Assert.AreEqual(8, blackMoves.Count);
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset));
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset));
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset));
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset));
        Assert.AreEqual(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.a1));
        Assert.AreEqual(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.h1));
    }

    [Test]
    public void PromotionsCapturing()
    {
        var position = new Position("BqB2NqB/P6P/8/K1k5/8/8/p6p/bQb2bNb w - - 0 1");
        var offset = Utils.PieceOffset(position.Side);
        var whiteMoves = GeneratePawnMoves(position).ToList();

        Assert.AreEqual(8, whiteMoves.Count);
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.b8));
        Assert.AreEqual(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.g8));

        whiteMoves = [.. GeneratePawnCaptures(position)];

        Assert.AreEqual(8, whiteMoves.Count);
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.b8));
        Assert.AreEqual(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.g8));

        position = new Position("BqB2BqB/P6P/8/8/K1k5/8/p6p/bQb2bQb b - - 0 1");
        offset = Utils.PieceOffset(position.Side);
        var blackMoves = GeneratePawnMoves(position).ToList();

        Assert.AreEqual(8, blackMoves.Count);
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.b1));
        Assert.AreEqual(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.g1));

        blackMoves = [.. GeneratePawnCaptures(position)];

        Assert.AreEqual(8, blackMoves.Count);
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset && m.CapturedPiece() != (int)Piece.None));
        Assert.AreEqual(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.b1));
        Assert.AreEqual(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.g1));
    }

    [TestCase("7k/8/p7/Pp6/8/Q7/8/7K w - b6 0 1")]
    [TestCase("7k/8/q7/8/pP6/P7/8/7K b - b3 0 1")]
    public void EnPassant(string fen)
    {
        var position = new Position(fen);
        var moves = GeneratePawnMoves(position);
        Assert.AreEqual(1, moves.Count());
        Assert.AreEqual(1, moves.Count(m => m.IsEnPassant() && m.CapturedPiece() != (int)Piece.None));

        moves = GeneratePawnCaptures(position);
        Assert.AreEqual(1, moves.Count());
        Assert.AreEqual(1, moves.Count(m => m.IsEnPassant() && m.CapturedPiece() != (int)Piece.None));
    }

    [TestCase("7k/8/8/PpP5/8/Q7/8/7K w - b6 0 1")]
    [TestCase("7k/8/q7/8/pPp5/8/8/7K b - b3 0 1")]
    public void DoubleEnPassant(string fen)
    {
        var position = new Position(fen);
        var moves = GeneratePawnMoves(position);
        Assert.AreEqual(2, moves.Count(m => m.IsEnPassant() && m.CapturedPiece() != (int)Piece.None));

        moves = GeneratePawnCaptures(position);
        Assert.AreEqual(2, moves.Count(m => m.IsEnPassant() && m.CapturedPiece() != (int)Piece.None));
    }

    [TestCase("K1k5/8/8/p1p1p1p1/PpPpPpPp/1P1P1P1P/8/8 w - - 0 1", Description = "Blocked position")]
    [TestCase("K1k5/8/8/p1p1p1p1/PpPpPpPp/1P1P1P1P/8/8 b - - 0 1", Description = "Blocked position")]
    [TestCase("K1k5/8/8/N7/P7/1p6/8 w - - 0 1", Description = "Backwards/inverse capture")]
    [TestCase("K1k5/8/8/1P6/p7/n7/8/8 b - - 0 1", Description = "Backwards/inverse capture")]
    public void ShouldNotGenerateMoves(string fen)
    {
        var position = new Position(fen);
        Assert.IsEmpty(GeneratePawnMoves(position));
        Assert.IsEmpty(GeneratePawnCaptures(position));
    }

#pragma warning restore RCS1098 // Constant values should be placed on right side of comparisons.
}
