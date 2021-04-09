using SharpFish.Model;
using System.Linq;
using Xunit;

namespace SharpFish.Test.MoveGeneration
{
    public class GenerateQueenMoves
    {
        [Theory]
        [InlineData(Constants.EmptyBoardFEN, 0)]
        [InlineData(Constants.InitialPositionFEN, 0)]
        [InlineData("8/8/8/8/8/8/PP3PPP/RNBQKBNR w KQkq - 0 1", 14)]
        [InlineData("rnbqkbnr/pp3ppp/8/8/8/8/8/8 b KQkq - 0 1", 14)]
        [InlineData("8/pppppppp/8/8/8/8/PP3PPP/RNBQKBNR w KQkq - 0 1", 13)]
        [InlineData("rnbqkbnr/pp3ppp/8/8/8/8/PPPPPPPP/8 b KQkq - 0 1", 13)]
        [InlineData("8/8/8/8/3Q4/8/8/8 w - - 0 1", 27)]
        [InlineData("8/8/8/8/3q4/8/8/8 b - - 0 1", 27)]
        [InlineData("8/8/1P1P1P2/8/1P1Q1P2/8/1P1P1P2/8 w - - 0 1", 8)]
        [InlineData("8/8/1p1p1p2/8/1p1q1p2/8/1p1p1p2/8 b - - 0 1", 8)]
        [InlineData("8/8/1p1p1p2/8/1p1Q1p2/8/1p1p1p2/8 w - - 0 1", 16)]
        [InlineData("8/8/1P1P1P2/8/1P1q1P2/8/1P1P1P2/8 b - - 0 1", 16)]
        public void QueenMoves_Count(string fen, int expectedMoves)
        {
            var game = new Game(fen);
            var offset = Utils.PieceOffset(game.Side);
            var moves = MovesGenerator.GeneratePieceMoves((int)Piece.Q + offset, game);

            Assert.Equal(expectedMoves, moves.Count());

            Assert.Equal(moves, MovesGenerator.GenerateQueenMoves(game));
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
        [Fact]
        public void QueenMoves_White()
        {
            var game = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.Q + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game);

            Assert.Equal(9, moves.Count(m => m.Piece == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f3
                && m.TargetSquare == (int)BoardSquares.e3));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f3
                && m.TargetSquare == (int)BoardSquares.d3));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f3
                && m.TargetSquare == (int)BoardSquares.f4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f3
                && m.TargetSquare == (int)BoardSquares.f5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f3
                && m.TargetSquare == (int)BoardSquares.f6));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f3
                && m.TargetSquare == (int)BoardSquares.g4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f3
                && m.TargetSquare == (int)BoardSquares.h5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f3
                && m.TargetSquare == (int)BoardSquares.g3));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f3
                && m.TargetSquare == (int)BoardSquares.h3));
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
        [Fact]
        public void QueenMoves_Black()
        {
            var game = new Game("r3kR1r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.Q + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game);

            Assert.Equal(4, moves.Count(m => m.Piece == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e7
                && m.TargetSquare == (int)BoardSquares.d8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e7
                && m.TargetSquare == (int)BoardSquares.f8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e7
                && m.TargetSquare == (int)BoardSquares.d6));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e7
                && m.TargetSquare == (int)BoardSquares.c5));
        }

        [Theory]
        [InlineData("8/8/1p1p1p2/8/1p1Q1p2/8/1p1p1p2/8 w - - 0 1", 8)]
        [InlineData("8/8/1P1P1P2/8/1P1q1P2/8/1P1P1P2/8 b - - 0 1", 8)]
        [InlineData("p1p1p3/8/p1Q1p3/8/p1p1p1p1/8/4p1Q1/7p w - - 0 1", 12)]
        [InlineData("P1P1P3/8/P1q1P3/8/P1P1P1P1/8/4P1q1/7P b - - 0 1", 12)]
        public void QueenMoves_CapturesOnly(string fen, int expectedCaptures)
        {
            var game = new Game(fen);
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.Q + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game, capturesOnly: true);

            Assert.Equal(expectedCaptures, moves.Count(m => m.Piece == piece && m.MoveType == MoveType.Capture));
        }
    }
}
