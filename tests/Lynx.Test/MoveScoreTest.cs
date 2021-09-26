using Lynx.Model;
using System.Linq;
using Xunit;

namespace Lynx.Test
{
    public class MoveScoreTest
    {
        /// <summary>
        /// 'Tricky position'
        /// 8   r . . . k . . r
        /// 7   p . p p q p b .
        /// 6   b n . . p n p .
        /// 5   . . . P N . . .
        /// 4   . p . . P . . .
        /// 3   . . N . . Q . p
        /// 2   P P P B B P P P
        /// 1   R . . . K . . R
        ///     a b c d e f g h
        /// This tests indirectly <see cref="EvaluationConstants.MostValueableVictimLeastValuableAttacker"/>
        /// </summary>
        [Theory]
        [InlineData(Constants.TrickyTestPositionFEN)]
        public void MoveScore(string fen)
        {
            var position = new Position(fen);

            var allMoves = position.AllPossibleMoves();

            Assert.Equal("e2a6", allMoves[0].UCIString());     // BxB
            Assert.Equal("f3f6", allMoves[1].UCIString());     // QxN
            Assert.Equal("d5e6", allMoves[2].UCIString());     // PxP
            Assert.Equal("g2h3", allMoves[3].UCIString());     // PxP
            Assert.Equal("e5d7", allMoves[4].UCIString());     // NxP
            Assert.Equal("e5f7", allMoves[5].UCIString());     // NxP
            Assert.Equal("e5g6", allMoves[6].UCIString());     // NxP
            Assert.Equal("f3h3", allMoves[7].UCIString());     // QxP

            foreach (var move in allMoves.Where(move => !move.IsCapture() && !move.IsCastle()))
            {
                Assert.Equal(0, move.Score(position));
            }
        }

        /// <summary>
        /// Only one capture, en passant, both sides
        /// 8   r n b q k b n r
        /// 7   p p p . p p p p
        /// 6   . . . . . . . .
        /// 5   . . . p P . . .
        /// 4   . . . . . . . .
        /// 3   . . . . . . . .
        /// 2   P P P P . P P P
        /// 1   R N B Q K B N R
        ///     a b c d e f g h
        /// </summary>
        /// <param name="fen"></param>
        [Theory]
        [InlineData("rnbqkbnr/ppp1pppp/8/3pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 1", "e5d6")]
        [InlineData("rnbqkbnr/ppp1pppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", "d4e3")]
        public void MoveScoreEnPassant(string fen, string moveWithHighestScore)
        {
            var position = new Position(fen);

            var allMoves = position.AllPossibleMoves();

            Assert.Equal(moveWithHighestScore, allMoves[0].UCIString());
            Assert.Equal(Move.CaptureBaseScore + EvaluationConstants.MostValueableVictimLeastValuableAttacker[0,0], allMoves[0].Score(position));
        }
    }
}
