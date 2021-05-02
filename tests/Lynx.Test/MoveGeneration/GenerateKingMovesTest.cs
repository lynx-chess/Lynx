using Lynx.Model;
using System.Linq;
using Xunit;

namespace Lynx.Test.MoveGeneration
{
    public class GenerateKingMovesTest
    {
        [Theory]
        [InlineData(Constants.InitialPositionFEN, 0)]
        [InlineData("8/8/8/2PPP3/2PKP3/2P1P3/8/8 w - - 0 1", 1)]
        [InlineData("8/8/8/2PPP3/2PKP3/3PP3/8/8 w - - 0 1", 1)]
        [InlineData("8/8/8/2PPP3/2PKP3/2PP4/8/8 w - - 0 1", 1)]
        [InlineData("8/8/8/2PPP3/2PKP3/3P4/8/8 w - - 0 1", 2)]
        [InlineData("8/8/8/2PPP3/2PKP3/8/8/8 w - - 0 1", 3)]
        [InlineData("8/8/8/2PPP3/2PK4/8/8/8 w - - 0 1", 4)]
        [InlineData("8/8/8/2PPP3/3K4/8/8/8 w - - 0 1", 5)]
        [InlineData("8/8/8/2P1P3/3K4/8/8/8 w - - 0 1", 6)]
        [InlineData("8/8/8/4P3/3K4/8/8/8 w - - 0 1", 7)]
        [InlineData("8/8/8/8/3K4/8/8/8 w - - 0 1", 8)]
        public void KingMoves_Count(string fen, int expectedMoves)
        {
            var position = new Position(fen);
            var offset = Utils.PieceOffset(position.Side);
            var moves = MovesGenerator.GeneratePieceMoves((int)Piece.K + offset, position);

            Assert.Equal(expectedMoves, moves.Count());

            Assert.Equal(moves, MovesGenerator.GenerateKingMoves(position));
        }

        /// <summary>
        /// 8   . . . . . . . .
        /// 7   . . . . . . n Q
        /// 6   . . . . . . k .
        /// 5   . . . . . P B .
        /// 4   . . . . . . . .
        /// 3   . . N . . . . .
        /// 2   n K r . . . . .
        /// 1   b p . . . . . .
        ///     a b c d e f g h
        ///     Side:       White
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void KingMoves_White()
        {
            var position = new Position("8/6nQ/6k1/5PB1/8/2N5/nKr5/bp6 w - - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var piece = (int)Piece.K + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, position);

            Assert.Equal(7, moves.Count(m => m.Piece() == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.b3));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.a3));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.a2));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.a1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.b1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.c1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.c2));
        }

        /// <summary>
        /// 8   . . . . . . . .
        /// 7   . . . . . . n Q
        /// 6   . . . . . . k .
        /// 5   . . . . . P B .
        /// 4   . . . . . . . .
        /// 3   . . N . . . . .
        /// 2   n K r . . . . .
        /// 1   b p . . . . . .
        ///     a b c d e f g h
        ///     Side:       Black
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void KingMoves_Black()
        {
            var position = new Position("8/6nQ/6k1/5PB1/8/2N5/nKr5/bp6 b - - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var piece = (int)Piece.K + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, position);

            Assert.Equal(7, moves.Count(m => m.Piece() == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.h7));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.h6));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.h5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.g5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.f5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.f6));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.f7));
        }

        /// <summary>
        /// 8   . . . . . . . .
        /// 7   . . . . . . n Q
        /// 6   . . . . . . k .
        /// 5   . . . . . P B .
        /// 4   . . . . . . . .
        /// 3   . . N . . . . .
        /// 2   n K r . . . . .
        /// 1   b p . . . . . .
        ///     a b c d e f g h
        ///     Side:       White
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void KingMoves_CapturesOnly_White()
        {
            var position = new Position("8/6nQ/6k1/5PB1/8/2N5/nKr5/bp6 w - - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var piece = (int)Piece.K + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, position, capturesOnly: true);

            Assert.Equal(4, moves.Count(m => m.Piece() == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.a2));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.a1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.b1));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.b2
                && m.TargetSquare() == (int)BoardSquares.c2));
        }

        /// <summary>
        /// 8   . . . . . . . .
        /// 7   . . . . . . n Q
        /// 6   . . . . . . k .
        /// 5   . . . . . P B .
        /// 4   . . . . . . . .
        /// 3   . . N . . . . .
        /// 2   n K r . . . . .
        /// 1   b p . . . . . .
        ///     a b c d e f g h
        ///     Side:       Black
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void KingMoves_CapturesOnly_Black()
        {
            var position = new Position("8/6nQ/6k1/5PB1/8/2N5/nKr5/bp6 b - - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var piece = (int)Piece.K + offset;
            var moves = MovesGenerator.GeneratePieceMoves(piece, position, capturesOnly: true);

            Assert.Equal(3, moves.Count(m => m.Piece() == piece));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.h7));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.g5));

            Assert.Equal(1, moves.Count(m =>
                m.SourceSquare() == (int)BoardSquares.g6
                && m.TargetSquare() == (int)BoardSquares.f5));
        }

        //[Theory]
        //[InlineData(Constants.InitialPositionFEN, 0)]
        //[InlineData("8/8/8/2ppp3/2pKp3/2p1p3/8/8 w - - 0 1", 5)]
        //[InlineData("8/8/8/2ppp3/2pKp3/3pp3/8/8 w - - 0 1", 4)]
        //[InlineData("8/8/8/2ppp3/2pKp3/2pp4/8/8 w - - 0 1", 4)]
        //[InlineData("8/8/8/2ppp3/2pKp3/3p4/8/8 w - - 0 1", 3)]
        //[InlineData("8/8/8/2ppp3/2pKp3/8/8/8 w - - 0 1", 3)]
        //[InlineData("8/8/8/2ppp3/2pK4/8/8/8 w - - 0 1", 3)]
        //[InlineData("8/8/8/2ppp3/3K4/8/8/8 w - - 0 1", 3)]
        //[InlineData("8/8/8/2p1p3/3K4/8/8/8 w - - 0 1", 2)]
        //[InlineData("8/8/8/4p3/3K4/8/8/8 w - - 0 1", 1)]
        //[InlineData("8/8/8/8/3K4/8/8/8 w - - 0 1", 0)]
        //[InlineData("8/8/8/2nnn3/2nKn3/2nnn3/8/8 w - - 0 1", 0)]
        //[InlineData("8/8/8/2qqq3/2qKq3/2qqq3/8/8 w - - 0 1", 0)]
        //[InlineData("8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", 0)]
        //public void LegalMoves(string fen, int expectedMoves)
        //{
        //    var position = new Position(fen);
        //    var offset = Utils.PieceOffset(position.Side);
        //    var piece = (int)Piece.K + offset;
        //    var moves = MovesGenerator.GeneratePieceMoves(piece, position);

        //    Assert.Equal(expectedMoves, moves.Count(m => m.Piece() == piece));
        //}
    }
}
