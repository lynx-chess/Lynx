using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class QuietCaptureMoveGenerationTest
{
    [TestCase(Constants.InitialPositionFEN)]
    [TestCase("r3k2r/pppq1ppp/2npbn2/3Np3/2P1P3/2N1B3/PP3PPP/R2QKB1R w KQkq - 0 1")]
    [TestCase("r2q1rk1/pp1n1ppp/2pbpn2/3p4/3P4/2N1PN2/PPQ1BPPP/R1B2RK1 b - - 0 1")]
    public void PieceMoves_AreEquivalentToQuietPlusCaptures(string fen)
    {
        var position = new Position(fen);
        var offset = Utils.PieceOffset((int)position.Side);

        foreach (var basePiece in new[] { (int)Piece.N, (int)Piece.B, (int)Piece.R, (int)Piece.Q })
        {
            var piece = basePiece + offset;

            var allMoves = GenerateAllPieceMoves(position, piece);
            var splitMoves = GenerateQuietPieceMoves(position, piece)
                .Concat(GeneratePieceCaptures(position, piece))
                .ToArray();

            Assert.That(splitMoves, Is.EquivalentTo(allMoves),
                $"Piece {(Piece)piece} mismatch in position {fen}");
        }
    }

    [TestCase(Constants.InitialPositionFEN)]
    [TestCase("8/8/6k1/8/8/2N5/nKr5/bb6 w - - 0 1")]
    [TestCase("8/6nQ/6k1/5PB1/8/8/8/K7 b - - 0 1")]
    public void KingMoves_AreEquivalentToQuietPlusCaptures(string fen)
    {
        var position = new Position(fen);
        var offset = Utils.PieceOffset((int)position.Side);
        var piece = (int)Piece.K + offset;

        var allMoves = GenerateKingMoves(position, piece);
        var splitMoves = GenerateQuietKingMoves(position, piece)
            .Concat(GenerateKingCaptures(position, piece))
            .ToArray();

        Assert.That(splitMoves, Is.EquivalentTo(allMoves), $"King moves mismatch in position {fen}");
    }

    [TestCase(Constants.InitialPositionFEN)]
    [TestCase("8/P6P/8/8/K1k5/8/p6p/8 w - - 0 1")]
    [TestCase("8/8/8/3pP3/8/8/8/K1k5 w - d6 0 1")]
    [TestCase("8/8/8/8/3Pp3/8/8/K1k5 b - d3 0 1")]
    public void PawnMoves_AreEquivalentToQuietPlusCapturesAndPromotions(string fen)
    {
        var position = new Position(fen);
        var offset = Utils.PieceOffset((int)position.Side);

        var allMoves = GenerateAllPawnMoves(position, offset);
        var splitMoves = GeneratePawnQuiets(position, offset)
            .Concat(GeneratePawnCapturesAndPromotions(position, offset))
            .ToArray();

        Assert.That(splitMoves, Is.EquivalentTo(allMoves), $"Pawn moves mismatch in position {fen}");
    }

    private static Move[] GenerateAllPieceMoves(Position position, int piece)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        int localIndex = 0;
        MoveGenerator.GenerateAllPieceMoves(ref localIndex, moves, piece, position);
        return moves[..localIndex].ToArray();
    }

    private static Move[] GenerateQuietPieceMoves(Position position, int piece)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        int localIndex = 0;
        MoveGenerator.GenerateQuietPieceMoves(ref localIndex, moves, piece, position);
        return moves[..localIndex].ToArray();
    }

    private static Move[] GeneratePieceCaptures(Position position, int piece)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        int localIndex = 0;
        MoveGenerator.GeneratePieceCaptures(ref localIndex, moves, piece, position);
        return moves[..localIndex].ToArray();
    }

    private static Move[] GenerateKingMoves(Position position, int piece)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        int localIndex = 0;
        MoveGenerator.GenerateKingMoves(ref localIndex, moves, piece, position, ref evaluationContext);
        return moves[..localIndex].ToArray();
    }

    private static Move[] GenerateQuietKingMoves(Position position, int piece)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        int localIndex = 0;
        MoveGenerator.GenerateQuietKingMoves(ref localIndex, moves, piece, position, ref evaluationContext);
        return moves[..localIndex].ToArray();
    }

    private static Move[] GenerateKingCaptures(Position position, int piece)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        int localIndex = 0;
        MoveGenerator.GenerateKingCaptures(ref localIndex, moves, piece, position, ref evaluationContext);
        return moves[..localIndex].ToArray();
    }

    private static Move[] GenerateAllPawnMoves(Position position, int offset)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        int localIndex = 0;
        MoveGenerator.GenerateAllPawnMoves(ref localIndex, moves, position, offset);
        return moves[..localIndex].ToArray();
    }

    private static Move[] GeneratePawnQuiets(Position position, int offset)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        int localIndex = 0;
        MoveGenerator.GeneratePawnQuiets(ref localIndex, moves, position, offset);
        return moves[..localIndex].ToArray();
    }

    private static Move[] GeneratePawnCapturesAndPromotions(Position position, int offset)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        int localIndex = 0;
        MoveGenerator.GeneratePawnCapturesAndPromotions(ref localIndex, moves, position, offset);
        return moves[..localIndex].ToArray();
    }
}
