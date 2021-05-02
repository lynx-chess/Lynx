using Lynx.Model;
using System.Linq;
using Xunit;

namespace Lynx.Test.MoveGeneration
{
    public class GeneratePawnMovesTest
    {
        [Fact]
        public void QuietMoves()
        {
            var position = new Position(Constants.InitialPositionFEN);

            var whiteMoves = MoveGenerator.GeneratePawnMoves(position, offset: 0);

            for (int square = (int)BoardSquare.a2; square <= (int)BoardSquare.h2; ++square)
            {
                Assert.Single(whiteMoves.Where(m =>
                    m.SourceSquare() == square
                    && m.TargetSquare() == square - 8
                    && !m.IsCapture()));

                Assert.Single(whiteMoves.Where(m =>
                    m.SourceSquare() == square
                    && m.TargetSquare() == square - 16
                    && !m.IsCapture()));
            }

            position = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1");
            var blackMoves = MoveGenerator.GeneratePawnMoves(position, offset: 6);

            for (int square = (int)BoardSquare.a7; square <= (int)BoardSquare.h7; ++square)
            {
                Assert.Single(blackMoves.Where(m =>
                    m.SourceSquare() == square
                    && m.TargetSquare() == square + 8
                    && !m.IsCapture()));

                Assert.Single(blackMoves.Where(m =>
                    m.SourceSquare() == square
                    && m.TargetSquare() == square + 16
                    && !m.IsCapture()));
            }
        }

        [Theory]
        [InlineData("8/8/8/8/8/p1p1p1p1/P1P1P1P1/8 w - - 0 1", 0)]
        [InlineData("8/8/8/8/8/R1R1R1R1/P1P1P1P1/8 w - - 0 1", 0)]
        [InlineData("8/8/8/8/p1p1p1p1/8/P1P1P1P1/8 w - - 0 1", 4)]
        [InlineData("8/8/8/8/R1R1R1R1/8/P1P1P1P1/8 w - - 0 1", 4)]
        [InlineData("8/p1p1p1p1/P1P1P1P1/8/8/8/8/8 b - - 0 1", 0)]
        [InlineData("8/p1p1p1p1/r1r1r1r1/8/8/8/8/8 b - - 0 1", 0)]
        [InlineData("8/p1p1p1p1/8/P1P1P1P1/8/8/8/8 b - - 0 1", 4)]
        [InlineData("8/p1p1p1p1/8/r1r1r1r1/8/8/8/8 b - - 0 1", 4)]
        public void QuietMoves_NoDoublePush(string fen, int expectedMoves)
        {
            var position = new Position(fen);
            var moves = MoveGenerator.GeneratePawnMoves(position, offset: Utils.PieceOffset(position.Side));
            Assert.Equal(expectedMoves, moves.Count());
        }

        [Fact]
        public void Captures()
        {
            var position = new Position("8/8/8/8/8/1n6/PPP5/8 w - - 0 1");

            var whiteMoves = MoveGenerator.GeneratePawnMoves(position, offset: Utils.PieceOffset(position.Side)).ToList();

            Assert.Equal(6, whiteMoves.Count);
            Assert.Single(whiteMoves.Where(m =>
                m.SourceSquare() == (int)BoardSquare.a2
                && m.TargetSquare() == (int)BoardSquare.b3
                && m.IsCapture()));

            Assert.Single(whiteMoves.Where(m =>
                m.SourceSquare() == (int)BoardSquare.c2
                && m.TargetSquare() == (int)BoardSquare.b3
                && m.IsCapture()));

            position = new Position("8/ppp/1B6/8/8/8/8/8 b - - 0 1");

            var blackMoves = MoveGenerator.GeneratePawnMoves(position, offset: Utils.PieceOffset(position.Side)).ToList();
            Assert.Equal(6, whiteMoves.Count);
            Assert.Single(blackMoves.Where(m =>
                m.SourceSquare() == (int)BoardSquare.a7
                && m.TargetSquare() == (int)BoardSquare.b6
                && m.IsCapture()));

            Assert.Single(blackMoves.Where(m =>
                m.SourceSquare() == (int)BoardSquare.c7
                && m.TargetSquare() == (int)BoardSquare.b6
                 && m.IsCapture()));
        }

        [Fact]
        public void PromotionsWithoutCapturing()
        {
            var position = new Position("8/P6P/8/8/8/8/p6p/8 w - - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var whiteMoves = MoveGenerator.GeneratePawnMoves(position, offset: Utils.PieceOffset(position.Side)).ToList();

            Assert.Equal(8, whiteMoves.Count);
            Assert.Equal(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset && !m.IsCapture()));
            Assert.Equal(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset && !m.IsCapture()));
            Assert.Equal(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset && !m.IsCapture()));
            Assert.Equal(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset && !m.IsCapture()));
            Assert.Equal(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.a8));
            Assert.Equal(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.h8));

            position = new Position("8/P6P/8/8/8/8/p6p/8 b - - 0 1");
            offset = Utils.PieceOffset(position.Side);
            var blackMoves = MoveGenerator.GeneratePawnMoves(position, offset: 6).ToList();

            Assert.Equal(8, blackMoves.Count);
            Assert.Equal(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset));
            Assert.Equal(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset));
            Assert.Equal(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset));
            Assert.Equal(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset));
            Assert.Equal(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.a1));
            Assert.Equal(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.h1));
        }

        [Fact]
        public void PromotionsCapturing()
        {
            var position = new Position("BqB2BqB/P6P/8/8/8/8/p6p/bQb2bQb w - - 0 1");
            var offset = Utils.PieceOffset(position.Side);
            var whiteMoves = MoveGenerator.GeneratePawnMoves(position, offset: Utils.PieceOffset(position.Side)).ToList();

            Assert.Equal(8, whiteMoves.Count);
            Assert.Equal(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset && m.IsCapture()));
            Assert.Equal(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset && m.IsCapture()));
            Assert.Equal(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset && m.IsCapture()));
            Assert.Equal(2, whiteMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset && m.IsCapture()));
            Assert.Equal(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.b8));
            Assert.Equal(4, whiteMoves.Count(m => m.TargetSquare() == (int)BoardSquare.g8));

            position = new Position("BqB2BqB/P6P/8/8/8/8/p6p/bQb2bQb b - - 0 1");
            offset = Utils.PieceOffset(position.Side);
            var blackMoves = MoveGenerator.GeneratePawnMoves(position, offset: Utils.PieceOffset(position.Side)).ToList();

            Assert.Equal(8, blackMoves.Count);
            Assert.Equal(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.B + offset && m.IsCapture()));
            Assert.Equal(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.R + offset && m.IsCapture()));
            Assert.Equal(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.N + offset && m.IsCapture()));
            Assert.Equal(2, blackMoves.Count(m => m.PromotedPiece() == (int)Piece.Q + offset && m.IsCapture()));
            Assert.Equal(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.b1));
            Assert.Equal(4, blackMoves.Count(m => m.TargetSquare() == (int)BoardSquare.g1));
        }

        [Theory]
        [InlineData("8/8/Q7/P7/8/8/8/8 w - b6 0 1")]    // We don't really check if there's a pawn there
        [InlineData("8/8/Q7/P7/1p6/8/8/8 w - b6 0 1")]
        [InlineData("8/8/8/8/p7/q7/8/8 b - b3 0 1")]    // We don't really check if there's a pawn there
        [InlineData("8/8/8/1P6/p7/q7/8/8 b - b3 0 1")]
        public void EnPassant(string fen)
        {
            var position = new Position(fen);
            var moves = MoveGenerator.GeneratePawnMoves(position, offset: Utils.PieceOffset(position.Side));
            Assert.Single(moves);
            Assert.Single(moves.Where(m => m.IsEnPassant() && m.IsCapture()));
        }

        [Theory]
        [InlineData("8/8/8/P1P6/8/8/8/8 w - b6 0 1")]
        [InlineData("8/8/8/8/p1p6/8/8/8 b - b3 0 1")]
        public void DoubleEnPassant(string fen)
        {
            var position = new Position(fen);
            var moves = MoveGenerator.GeneratePawnMoves(position, offset: Utils.PieceOffset(position.Side));
            Assert.Equal(2, moves.Count(m => m.IsEnPassant() && m.IsCapture()));
        }

        [Theory]
        [InlineData("PPPPPPPP/8/8/8/8/8/8/8 w - - 0 1")]    // Last rank
        [InlineData("8/8/8/8/8/8/8/pppppppp b - - 0 1")]    // Last rank
        [InlineData("8/8/8/p1p1p1p1/PpPpPpPp/1P1P1P1P/8/8 w - - 0 1")]  // Blocked position
        [InlineData("8/8/8/p1p1p1p1/PpPpPpPp/1P1P1P1P/8/8 b - - 0 1")]  // Blocked position
        [InlineData("8/8/Q7/P7/1p6/8/8/8 w - b3 0 1")]  // Wrong enpassant square
        [InlineData("8/8/8/1P6/p7/q7/8/8 b - b6 0 1")]  // Wrong enpassant square
        [InlineData("8/8/8/N7/P7/1p6/8/8 w - - 0 1")]   // Backwards/inverse capture
        [InlineData("8/8/8/1P6/p7/n7/8/8 b - - 0 1")]   // Backwards/inverse capture
        public void ShouldNotGenerateMoves(string fen)
        {
            var position = new Position(fen);
            var moves = MoveGenerator.GeneratePawnMoves(position, offset: Utils.PieceOffset(position.Side));
            Assert.Empty(moves);
        }
    }
}
