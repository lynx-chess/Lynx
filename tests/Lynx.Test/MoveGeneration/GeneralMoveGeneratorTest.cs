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
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var enPassantMove = MoveGenerator.GenerateAllMoves(originalPosition, moves).ToArray().Single(m => m.IsEnPassant());
        var positionAferEnPassant = new Position(originalPosition, enPassantMove);

        moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        foreach (var move in MoveGenerator.GenerateAllMoves(positionAferEnPassant, moves))
        {
            if (new Position(positionAferEnPassant, move).IsValid())
            {
                Assert.AreNotEqual(Piece.n, (Piece)move.Piece());
                Assert.AreEqual(Piece.k, (Piece)move.Piece());
            }
        }
    }
}
