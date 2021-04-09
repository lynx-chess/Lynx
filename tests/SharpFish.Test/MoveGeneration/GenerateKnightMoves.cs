using SharpFish.Model;
using System.Linq;
using Xunit;

namespace SharpFish.Test.MoveGeneration
{
    public class GenerateKnightMoves
    {
        [Theory]
        [InlineData(Constants.EmptyBoardFEN, 0)]
        [InlineData(Constants.InitialPositionFEN, 4)]
        [InlineData("8/8/8/8/8/P1P2P1P/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 0)]
        [InlineData("rnbqkbnr/pppppppp/p1p2p1p/8/8/8/8/8 b KQkq - 0 1", 0)]
        [InlineData("8/8/2P1P3/1P3P2/3N4/1P3P2/2P1P3/8 w - - 0 1", 0)]
        [InlineData("8/8/2p1p3/1p3p2/3N4/1p3p2/2p1p3/8 w - - 0 1", 8)]
        [InlineData("8/8/2p1p3/1p3p2/3n4/1p3p2/2p1p3/8 b - - 0 1", 0)]
        [InlineData("8/8/2P1P3/1P3P2/3n4/1P3P2/2P1P3/8 b - - 0 1", 8)]
        public void KnightMoves_Count(string fen, int expectedMoves)
        {
            var game = new Game(fen);
            var offset = Utils.PieceOffset(game.Side);
            var moves = MovesGenerator.GeneratePieceMoves((int)Piece.N + offset, game);

            Assert.Equal(expectedMoves, moves.Count());

            Assert.Equal(moves, MovesGenerator.GenerateKnightMoves(game));
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
        public void KnightMoves_White()
        {
            var game = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.N + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game);

            Assert.Equal(11, moves.Count(m => m.Piece == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.c3
                && m.TargetSquare == (int)BoardSquares.a4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.c3
                && m.TargetSquare == (int)BoardSquares.b5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.c3
                && m.TargetSquare == (int)BoardSquares.b1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.c3
                && m.TargetSquare == (int)BoardSquares.d1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.c4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.c6));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.g4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.d3));

            Assert.Equal(1, moves.Count(m =>
               m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.d7));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.f7));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.g6));
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
        public void KnightMoves_Black()
        {
            var game = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.N + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game);

            Assert.Equal(10, moves.Count(m => m.Piece == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.b6
                && m.TargetSquare == (int)BoardSquares.c8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.b6
                && m.TargetSquare == (int)BoardSquares.d5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.b6
                && m.TargetSquare == (int)BoardSquares.c4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.b6
                && m.TargetSquare == (int)BoardSquares.a4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f6
                && m.TargetSquare == (int)BoardSquares.g8));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f6
                && m.TargetSquare == (int)BoardSquares.h7));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f6
                && m.TargetSquare == (int)BoardSquares.h5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f6
                && m.TargetSquare == (int)BoardSquares.g4));

            Assert.Equal(1, moves.Count(m =>
               m.SourceSquare == (int)BoardSquares.f6
                && m.TargetSquare == (int)BoardSquares.e4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f6
                && m.TargetSquare == (int)BoardSquares.d5));
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
        public void KnightMoves_CapturesOnly_White()
        {
            var game = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.N + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game, capturesOnly: true);

            Assert.Equal(3, moves.Count(m => m.Piece == piece && m.MoveType == MoveType.Capture));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.d7));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.f7));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.e5
                && m.TargetSquare == (int)BoardSquares.g6));
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
        public void KnightMoves_CapturesOnly_Black()
        {
            var game = new Game("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1");
            var offset = Utils.PieceOffset(game.Side);
            var piece = (int)Piece.N + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, game, capturesOnly: true);

            Assert.Equal(3, moves.Count(m => m.Piece == piece && m.MoveType == MoveType.Capture));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.b6
                && m.TargetSquare == (int)BoardSquares.d5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f6
                && m.TargetSquare == (int)BoardSquares.e4));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare == (int)BoardSquares.f6
                && m.TargetSquare == (int)BoardSquares.d5));
        }
    }
}
