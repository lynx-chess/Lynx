using SharpFish.Model;
using System.Linq;
using Xunit;

namespace SharpFish.Test.MoveGeneration
{
    public class GenerateRookMovesTest
    {
        [Theory]
        [InlineData(Constants.EmptyBoardFEN, 0)]
        [InlineData(Constants.InitialPositionFEN, 0)]
        [InlineData("8/8/2n1n3/3R4/2n1n3/8/8/8 w - - 0 1", 14)]
        [InlineData("8/8/2N1N3/3r4/2N1N3/8/8/8 b - - 0 1", 14)]
        [InlineData("8/8/2n1n3/3R4/2n1n3/8/8/3q4 w - - 0 1", 14)]
        [InlineData("8/8/2N1N3/3r4/2N1N3/8/8/3Q4 b - - 0 1", 14)]
        [InlineData("8/8/2n1n3/3R4/2n1n3/8/8/3Q4 w - - 0 1", 13)]
        [InlineData("8/8/2N1N3/3r4/2N1N3/8/8/3q4 b - - 0 1", 13)]
        public void RookMoves_Count(string fen, int expectedMoves)
        {
            var position = new Position(fen);
            var offset = Utils.PieceOffset(position.Side);
            var moves = MovesGenerator.GeneratePieceMoves((int)Piece.R + offset, position);

            Assert.Equal(expectedMoves, moves.Count());

            Assert.Equal(moves, MovesGenerator.GenerateRookMoves(position));
        }

        /// <summary>
        /// 8   r . . . k . . r
        /// 7   p . p p q p b .
        /// 6   b n . . p n p .
        /// 5   . . . P N . . .
        /// 4   . p . . P . . .
        /// 3   . . N . . Q . P
        /// 2   . P P B B P P .
        /// 1   R . . . K . . R
        ///     a b c d e f g h
        ///     Side:       White
        ///     Enpassant:  no
        ///     Castling:   KQ | kq
        /// </summary>
        [Fact]
        public void RookMoves_White()
        {
            var position = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1P/1PPBBPP1/R3K2R w KQkq - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var piece = (int)Piece.R + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, position);

            Assert.Equal(11, moves.Count(m => m.Piece() == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a1
                && m.TargetSquare() == (int)BoardSquares.a2));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a1
                && m.TargetSquare() == (int)BoardSquares.a3));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a1
                && m.TargetSquare() == (int)BoardSquares.a4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a1
                && m.TargetSquare() == (int)BoardSquares.a5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a1
                && m.TargetSquare() == (int)BoardSquares.a6));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a1
                && m.TargetSquare() == (int)BoardSquares.b1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a1
                && m.TargetSquare() == (int)BoardSquares.c1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a1
                && m.TargetSquare() == (int)BoardSquares.d1));

            Assert.Equal(1, moves.Count(m =>
               m.SourceSquare() == (int)BoardSquares.h1
                && m.TargetSquare() == (int)BoardSquares.h2));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h1
                && m.TargetSquare() == (int)BoardSquares.g1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h1
                && m.TargetSquare() == (int)BoardSquares.f1));
        }

        /// <summary>
        /// 8   r . . . k . . r
        /// 7   p . p p q p b .
        /// 6   b n . . p n p .
        /// 5   . . . P N . . .
        /// 4   . p . . P . . .
        /// 3   . . N . . Q . P
        /// 2   . P P B B P P .
        /// 1   R . . . K . . R
        ///     a b c d e f g h
        ///     Side:       Black
        ///     Enpassant:  no
        ///     Castling:   KQ | kq
        /// </summary>
        [Fact]
        public void RookMoves_Black()
        {
            var position = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1P/1PPBBPP1/R3K2R b KQkq - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var piece = (int)Piece.R + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, position);

            Assert.Equal(10, moves.Count(m => m.Piece() == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a8
                && m.TargetSquare() == (int)BoardSquares.b8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a8
                && m.TargetSquare() == (int)BoardSquares.c8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a8
                && m.TargetSquare() == (int)BoardSquares.d8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h8
                && m.TargetSquare() == (int)BoardSquares.g8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h8
                && m.TargetSquare() == (int)BoardSquares.f8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h8
                && m.TargetSquare() == (int)BoardSquares.h7));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h8
                && m.TargetSquare() == (int)BoardSquares.h6));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h8
                && m.TargetSquare() == (int)BoardSquares.h5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h8
                && m.TargetSquare() == (int)BoardSquares.h4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h8
                && m.TargetSquare() == (int)BoardSquares.h3));
        }

        /// <summary>
        /// 8   r . . . k . . r
        /// 7   p . p p q p b .
        /// 6   b n . . p n p .
        /// 5   . . . P N . . .
        /// 4   . p . . P . . .
        /// 3   . . N . . Q . P
        /// 2   . P P B B P P .
        /// 1   R . . . K . . R
        ///     a b c d e f g h
        ///     Side:       White
        ///     Enpassant:  no
        ///     Castling:   KQ | kq
        /// </summary>
        [Fact]
        public void RookMoves_CapturesOnly_White()
        {
            var position = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1P/1PPBBPP1/R3K2R w KQkq - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var piece = (int)Piece.R + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, position, capturesOnly: true);

            Assert.Equal(1, moves.Count(m => m.Piece() == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.a1
                && m.TargetSquare() == (int)BoardSquares.a6
                && m.IsCapture() != default));
        }

        /// <summary>
        /// 8   r . . . k . . r
        /// 7   p . p p q p b .
        /// 6   b n . . p n p .
        /// 5   . . . P N . . .
        /// 4   . p . . P . . .
        /// 3   . . N . . Q . P
        /// 2   . P P B B P P .
        /// 1   R . . . K . . R
        ///     a b c d e f g h
        ///     Side:       Black
        ///     Enpassant:  no
        ///     Castling:   KQ | kq
        /// </summary>
        [Fact]
        public void RookMoves_CapturesOnly_Black()
        {
            var position = new Position("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1P/1PPBBPP1/R3K2R b KQkq - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var piece = (int)Piece.R + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, position, capturesOnly: true);

            Assert.Equal(1, moves.Count(m => m.Piece() == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.h8
                && m.TargetSquare() == (int)BoardSquares.h3));
        }
    }
}
