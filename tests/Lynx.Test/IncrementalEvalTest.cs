using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;
public class IncrementalEvalTest
{
    /// <summary>
    /// If castling moves are ever refactored, i.e. when adding FRC support, this should break.
    /// That'll mean that incremental eval condition in <see cref="Position.MakeMove(int)"/> needs to change
    /// </summary>
    [Test]
    public void CastlingMovesAreKingMoves()
    {
        var position = new Position("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1");

        ValidateCastlingMoves(position);

        position = new Position("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/R3K2R b KQkq - 0 1");
        ValidateCastlingMoves(position);

        static void ValidateCastlingMoves(Position position)
        {
            Span<Move> moveSpan = stackalloc Move[2];
            var index = 0;

            using var evaluationContext = new EvaluationContext();

            MoveGenerator.GenerateCastlingMoves(ref index, moveSpan, position, evaluationContext);

            foreach (var move in moveSpan[..index])
            {
                Assert.IsTrue(move.IsCastle());
                Assert.AreEqual((int)Piece.K + Utils.PieceOffset(position.Side), move.Piece());
            }
        }
    }
}
