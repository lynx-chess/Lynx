using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;
public class MoveGeneratorRegressionTest : BaseTest
{
    [Test]
    public void AllMovesAreGenerated()
    {
        var position = new Position("r3k2r/pP1pqpb1/bn2pnp1/2pPN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq c6 0 1");

        var moves = MoveGenerator.GenerateAllMoves(position).ToList();

        Assert.True(moves.Exists(m => m.IsShortCastle()));
        Assert.True(moves.Exists(m => m.IsLongCastle()));
        Assert.True(moves.Exists(m => m.IsEnPassant()));
        Assert.True(moves.Exists(m => m.PromotedPiece() != default));
        Assert.True(moves.Exists(m => m.PromotedPiece() != default && m.CapturedPiece() != (int)Piece.None));
        Assert.True(moves.Exists(m => m.PromotedPiece() != default && m.CapturedPiece() == (int)Piece.None));
        Assert.True(moves.Exists(m => m.IsDoublePawnPush()));

        Span<Move> moveSpan = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        var captures = MoveGenerator.GenerateAllCaptures(position, ref evaluationContext, moveSpan).ToArray().ToList();

        Assert.True(moves.Exists(m => m.IsShortCastle()));
        Assert.True(moves.Exists(m => m.IsLongCastle()));
        Assert.True(captures.Exists(m => m.IsEnPassant()));
        Assert.True(captures.Exists(m => m.PromotedPiece() != default));
        Assert.True(captures.Exists(m => m.PromotedPiece() != default && m.CapturedPiece() != (int)Piece.None));
        Assert.True(captures.Exists(m => m.PromotedPiece() != default && m.CapturedPiece() == (int)Piece.None));
        Assert.False(captures.Exists(m => m.IsDoublePawnPush()));
    }
}
