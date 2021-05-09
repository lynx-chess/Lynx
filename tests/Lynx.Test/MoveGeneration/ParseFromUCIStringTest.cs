using Lynx.Model;
using Xunit;

namespace Lynx.Test.MoveGeneration
{
    public class ParseFromUCIStringTest
    {
        /// <summary>
        /// 8   r . b . k . . r
        /// 7   p P p p q p b .
        /// 6   b n . . p n p .
        /// 5   . . . P N . . .
        /// 4   . p . . P . . .
        /// 3   . . N . . Q . .
        /// 2   P . P B . P p P
        /// 1   R . . . K B . R
        ///     a b c d e f g h
        /// </summary>
        [Theory]
        [InlineData("b7b8q", BoardSquare.b7, BoardSquare.b8, Piece.Q)]
        [InlineData("b7b8r", BoardSquare.b7, BoardSquare.b8, Piece.R)]
        [InlineData("b7b8n", BoardSquare.b7, BoardSquare.b8, Piece.N)]
        [InlineData("b7b8b", BoardSquare.b7, BoardSquare.b8, Piece.B)]
        [InlineData("b7a8q", BoardSquare.b7, BoardSquare.a8, Piece.Q)]
        [InlineData("b7a8r", BoardSquare.b7, BoardSquare.a8, Piece.R)]
        [InlineData("b7a8n", BoardSquare.b7, BoardSquare.a8, Piece.N)]
        [InlineData("b7a8b", BoardSquare.b7, BoardSquare.a8, Piece.B)]
        [InlineData("b7c8q", BoardSquare.b7, BoardSquare.c8, Piece.Q)]
        [InlineData("b7c8r", BoardSquare.b7, BoardSquare.c8, Piece.R)]
        [InlineData("b7c8n", BoardSquare.b7, BoardSquare.c8, Piece.N)]
        [InlineData("b7c8b", BoardSquare.b7, BoardSquare.c8, Piece.B)]
        [InlineData("e1d1", BoardSquare.e1, BoardSquare.d1, default(Piece))]
        [InlineData("e1c1", BoardSquare.e1, (BoardSquare)Constants.WhiteLongCastleKingSquare, default(Piece))]
        public void ParseFromUCIString_White(string UCIString, BoardSquare sourceSquare, BoardSquare targetSquare, Piece promotedPiece)
        {
            // Arrange
            const string fen = "r1b1k2r/pPppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q2/P1PB1PpP/R3KB1R w KQkq - 0 1";
            var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

            // Act
            var nullableMove = Move.ParseFromUCIString(UCIString, moves);

            // Assert
            Assert.NotNull(nullableMove);
            var move = nullableMove!.Value;
            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.Equal((int)promotedPiece, move.PromotedPiece());
        }

        /// <summary>
        /// 8   r . b . k . . r
        /// 7   p P p p q p b .
        /// 6   b n . . p n p .
        /// 5   . . . P N . . .
        /// 4   . p . . P . . .
        /// 3   . . N . . Q . .
        /// 2   P . P B . P p P
        /// 1   R . . . K B . R
        ///     a b c d e f g h
        /// </summary>
        [Theory]
        [InlineData("g2g1q", BoardSquare.g2, BoardSquare.g1, Piece.q)]
        [InlineData("g2g1r", BoardSquare.g2, BoardSquare.g1, Piece.r)]
        [InlineData("g2g1n", BoardSquare.g2, BoardSquare.g1, Piece.n)]
        [InlineData("g2g1b", BoardSquare.g2, BoardSquare.g1, Piece.b)]
        [InlineData("g2h1q", BoardSquare.g2, BoardSquare.h1, Piece.q)]
        [InlineData("g2h1r", BoardSquare.g2, BoardSquare.h1, Piece.r)]
        [InlineData("g2h1n", BoardSquare.g2, BoardSquare.h1, Piece.n)]
        [InlineData("g2h1b", BoardSquare.g2, BoardSquare.h1, Piece.b)]
        [InlineData("g2f1q", BoardSquare.g2, BoardSquare.f1, Piece.q)]
        [InlineData("g2f1r", BoardSquare.g2, BoardSquare.f1, Piece.r)]
        [InlineData("g2f1n", BoardSquare.g2, BoardSquare.f1, Piece.n)]
        [InlineData("g2f1b", BoardSquare.g2, BoardSquare.f1, Piece.b)]
        [InlineData("e8f8", BoardSquare.e8, BoardSquare.f8, default(Piece))]
        [InlineData("e8g8", BoardSquare.e8, (BoardSquare)Constants.BlackShortCastleKingSquare, default(Piece))]
        public void ParseFromUCIString_Black(string UCIString, BoardSquare sourceSquare, BoardSquare targetSquare, Piece promotedPiece)
        {
            // Arrange
            const string fen = "r1b1k2r/pPppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q2/P1PB1PpP/R3KB1R b KQkq - 0 1";
            var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

            // Act
            var nullableMove = Move.ParseFromUCIString(UCIString, moves);

            // Assert
            Assert.NotNull(nullableMove);
            var move = nullableMove!.Value;
            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.Equal((int)promotedPiece, move.PromotedPiece());
        }

        [Theory]
        [InlineData("e2e5")]
        [InlineData("e7e8q")]
        public void ParseFromUCIString_Error(string UCIString)
        {
            // Arrange
            const string fen = Constants.InitialPositionFEN;
            var moves = MoveGenerator.GenerateAllMoves(new Position(fen));

            // Act
            var move = Move.ParseFromUCIString(UCIString, moves);

            // Assert
            Assert.Null(move);
        }
    }
}
