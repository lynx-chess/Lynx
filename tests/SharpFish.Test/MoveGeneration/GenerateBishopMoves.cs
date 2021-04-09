using SharpFish.Model;
using System.Linq;
using Xunit;

namespace SharpFish.Test.MoveGeneration
{
    public class GenerateBishopMoves
    {
        [Theory]
        [InlineData(Constants.EmptyBoardFEN, 0)]
        [InlineData(Constants.InitialPositionFEN, 0)]
        [InlineData("8/8/8/8/8/8/P1P2P1P/RNBQKBNR w KQkq - 0 1", 14)]
        [InlineData("rnbqkbnr/p1p2p1p/8/8/8/8/8/8 b KQkq - 0 1", 14)]
        [InlineData("8/8/8/3B4/8/8/8/8 w - - 0 1 w - - 0 1", 13)]
        [InlineData("8/8/8/3b4/8/8/8/8 b - - 0 1 b - - 0 1", 13)]
        [InlineData("8/2N1N3/1N1P1N2/2PBP3/1N1P1N2/2N1N3/3N4/8 w - - 0 1", 13)]
        [InlineData("8/2n1n3/1n1p1n2/2pbp3/1n1p1n2/2n1n3/3n4/8 b - - 0 1", 13)]
        public void BishopMoves_Count(string fen, int expectedMoves)
        {
            var game = new Game(fen);
            var offset = Utils.PieceOffset(game.Side);
            var moves = MovesGenerator.GeneratePieceMoves((int)Piece.B + offset, game);

            Assert.Equal(expectedMoves, moves.Count());

            Assert.Equal(moves, MovesGenerator.GenerateBishopMoves(game));
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
        public void BishopMoves_White()
        {
            var game = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.B + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game);

            Assert.Equal(11, moves.Count(m => m.Piece == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.d2
                && m.TargetSquare == (int)BoardSquares.c1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.d2
                && m.TargetSquare == (int)BoardSquares.e3));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.d2
                && m.TargetSquare == (int)BoardSquares.f4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.d2
                && m.TargetSquare == (int)BoardSquares.g5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.d2
                && m.TargetSquare == (int)BoardSquares.h6));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e2
                && m.TargetSquare == (int)BoardSquares.d1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e2
                && m.TargetSquare == (int)BoardSquares.f1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e2
                && m.TargetSquare == (int)BoardSquares.d3));

            Assert.Equal(1, moves.Count(m =>
               m.SourceSquare == (int)BoardSquares.e2
                && m.TargetSquare == (int)BoardSquares.c4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e2
                && m.TargetSquare == (int)BoardSquares.b5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e2
                && m.TargetSquare == (int)BoardSquares.a6));
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
        ///     Side:       Black
        ///     Enpassant:  no
        ///     Castling:   KQ | kq
        /// </summary>
        [Fact]
        public void BishopMoves_Black()
        {
            var game = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.B + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game);

            Assert.Equal(8, moves.Count(m => m.Piece == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.a6
                && m.TargetSquare == (int)BoardSquares.b5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.a6
                && m.TargetSquare == (int)BoardSquares.c4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.a6
                && m.TargetSquare == (int)BoardSquares.d3));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.a6
                && m.TargetSquare == (int)BoardSquares.e2));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.a6
                && m.TargetSquare == (int)BoardSquares.b7));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.a6
                && m.TargetSquare == (int)BoardSquares.c8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.g7
                && m.TargetSquare == (int)BoardSquares.f8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.g7
                && m.TargetSquare == (int)BoardSquares.h6));
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
        public void BishopMoves_CapturesOnly_White()
        {
            var game = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.B + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game, capturesOnly: true);

            Assert.Equal(1, moves.Count(m => m.Piece == piece && m.MoveType == MoveType.Capture));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e2
                && m.TargetSquare == (int)BoardSquares.a6));
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
        ///     Side:       Black
        ///     Enpassant:  no
        ///     Castling:   KQ | kq
        /// </summary>
        [Fact]
        public void BishopMoves_CapturesOnly_Black()
        {
            var game = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.B + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game, capturesOnly: true);

            Assert.Equal(1, moves.Count(m => m.Piece == piece && m.MoveType == MoveType.Capture));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.a6
                && m.TargetSquare == (int)BoardSquares.e2));
        }
    }
}
