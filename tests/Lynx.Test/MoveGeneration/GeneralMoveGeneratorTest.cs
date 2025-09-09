using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class GeneralMoveGeneratorTest
{
    /// <summary>
    /// http://www.talkchess.com/forum3/viewtopic.php?f=7&t=78241&sid=c0f623952408bbd4a891bd36adcc132d&start=10#p907063
    /// </summary>
    [Test]
    public void DiscoveredCheckAfterEnPassantCapture()
    {
        var originalPosition = new Position("8/8/8/k1pP3R/8/8/8/n4K2 w - c6 0 1");

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        var enPassantMove = originalPosition.GenerateAllMoves(ref evaluationContext, moves).ToArray().Single(m => m.IsEnPassant());
        var positionAfterEnPassant = new Position(originalPosition);
        positionAfterEnPassant.MakeMove(enPassantMove);

        moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        foreach (var move in positionAfterEnPassant.GenerateAllMoves(ref evaluationContext, moves))
        {
            var newPosition = new Position(positionAfterEnPassant);
            newPosition.MakeMove(move);
            if (newPosition.IsValid())
            {
                Assert.AreNotEqual(Piece.n, (Piece)move.Piece());
                Assert.AreEqual(Piece.k, (Piece)move.Piece());
            }
        }
    }

    [TestCase("QQQQQQBk/Q6B/Q6Q/Q6Q/Q6Q/Q6Q/Q6Q/KQQQQQQQ w - - 0 1")]   // 265 pseudolegal moves at the time of writing this
    public void PositionWithMoreThan256PseudolegalMoves(string fen)
    {
        // 265 pseudolegal moves at the time of writing this
        var position = new Position(fen);

        Span<Move> moveSpan = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        var allMoves = position.GenerateAllMoves(ref evaluationContext, moveSpan);

        Assert.LessOrEqual(allMoves.Length, Constants.MaxNumberOfPseudolegalMovesInAPosition);
    }
}
