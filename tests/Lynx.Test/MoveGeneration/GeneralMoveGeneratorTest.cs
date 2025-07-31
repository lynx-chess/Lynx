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
        var enPassantMove = MoveGenerator.GenerateAllMoves(originalPosition, moves).ToArray().Single(m => m.IsEnPassant());
        var positionAfterEnPassant = new Position(originalPosition);
        positionAfterEnPassant.MakeMove(enPassantMove);

        moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        foreach (var move in MoveGenerator.GenerateAllMoves(positionAfterEnPassant, moves))
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

    [Test]
    public void PositionWithOver256PseudolegalMoves()
    {
        // 265 pseudolegal moves at the time of writing this
        var position = new Position("QQQQQQBk/Q6B/Q6Q/Q6Q/Q6Q/Q6Q/Q6Q/KQQQQQQQ w - - 0 1");

        Span<Move> moveSpan = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        var allMoves = MoveGenerator.GenerateAllMoves(position, moveSpan);

        Assert.LessOrEqual(allMoves.Length, Constants.MaxNumberOfPseudolegalMovesInAPosition);
    }
}
