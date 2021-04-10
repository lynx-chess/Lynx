using SharpFish.Model;
using Xunit;

namespace SharpFish.Test.MoveGeneration
{
    public class EncodeDecodeMoveTest
    {
        [Theory]
        [InlineData(BoardSquares.e4, BoardSquares.e5, Piece.P, false)]
        [InlineData(BoardSquares.e4, BoardSquares.d5, Piece.P, true)]
        [InlineData(BoardSquares.e5, BoardSquares.e4, Piece.p, false)]
        [InlineData(BoardSquares.e5, BoardSquares.d4, Piece.p, true)]
        [InlineData(BoardSquares.a1, BoardSquares.a4, Piece.R, false)]
        [InlineData(BoardSquares.a1, BoardSquares.a4, Piece.R, true)]
        [InlineData(BoardSquares.a1, BoardSquares.a4, Piece.r, false)]
        [InlineData(BoardSquares.a1, BoardSquares.a4, Piece.r, true)]
        [InlineData(BoardSquares.a1, BoardSquares.h8, Piece.B, false)]
        [InlineData(BoardSquares.a1, BoardSquares.h8, Piece.B, true)]
        [InlineData(BoardSquares.a1, BoardSquares.h8, Piece.b, false)]
        [InlineData(BoardSquares.a1, BoardSquares.h8, Piece.b, true)]
        [InlineData(BoardSquares.a1, BoardSquares.h8, Piece.Q, false)]
        [InlineData(BoardSquares.a1, BoardSquares.h8, Piece.Q, true)]
        [InlineData(BoardSquares.a1, BoardSquares.h8, Piece.q, false)]
        [InlineData(BoardSquares.a1, BoardSquares.h8, Piece.q, true)]
        [InlineData(BoardSquares.b1, BoardSquares.c3, Piece.N, true)]
        [InlineData(BoardSquares.b1, BoardSquares.c3, Piece.N, false)]
        [InlineData(BoardSquares.b8, BoardSquares.c6, Piece.n, true)]
        [InlineData(BoardSquares.b8, BoardSquares.c6, Piece.n, false)]
        public void SourceSquare_TargetSquare_Piece_Capture(BoardSquares sourceSquare, BoardSquares targetSquare, Piece piece, bool isCapture)
        {
            var move = new Move((int)sourceSquare, (int)targetSquare, (int)piece, isCapture: isCapture ? 1 : 0);

            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.Equal((int)piece, move.Piece());
            Assert.Equal(isCapture, move.IsCapture());

            Assert.Equal(default, move.PromotedPiece());
            Assert.False(move.IsEnPassant());
            Assert.False(move.IsCastle());
            Assert.False(move.IsShortCastle());
            Assert.False(move.IsLongCastle());
        }

        [Theory]
        [InlineData(BoardSquares.a7, BoardSquares.a8, Piece.Q)]
        [InlineData(BoardSquares.a7, BoardSquares.a8, Piece.R)]
        [InlineData(BoardSquares.a7, BoardSquares.a8, Piece.B)]
        [InlineData(BoardSquares.a7, BoardSquares.a8, Piece.N)]
        [InlineData(BoardSquares.a2, BoardSquares.a1, Piece.q)]
        [InlineData(BoardSquares.a2, BoardSquares.a1, Piece.r)]
        [InlineData(BoardSquares.a2, BoardSquares.a1, Piece.b)]
        [InlineData(BoardSquares.a2, BoardSquares.a1, Piece.n)]
        public void Promotion(BoardSquares sourceSquare, BoardSquares targetSquare, Piece promotedPiece)
        {
            var move = new Move((int)sourceSquare, (int)targetSquare, (int)Piece.P, promotedPiece: (int)promotedPiece);

            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.Equal((int)promotedPiece, move.PromotedPiece());
        }

        [Theory]
        [InlineData(BoardSquares.e5, BoardSquares.f6, false)]
        [InlineData(BoardSquares.e5, BoardSquares.f6, true)]
        [InlineData(BoardSquares.e4, BoardSquares.d3, false)]
        [InlineData(BoardSquares.e4, BoardSquares.d3, true)]
        public void EnPassant(BoardSquares sourceSquare, BoardSquares targetSquare, bool enPassant)
        {
            var move = new Move((int)sourceSquare, (int)targetSquare, (int)Piece.P, isEnPassant: enPassant ? 1 : 0);

            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.Equal(enPassant, move.IsEnPassant());
        }

        [Theory]
        [InlineData(BoardSquares.e1, BoardSquares.g1)]
        [InlineData(BoardSquares.e1, BoardSquares.c1)]
        [InlineData(BoardSquares.e8, BoardSquares.g8)]
        [InlineData(BoardSquares.e8, BoardSquares.c8)]
        public void Castling(BoardSquares sourceSquare, BoardSquares targetSquare)
        {
            var move = new Move((int)sourceSquare, (int)targetSquare, (int)Piece.K, isCastle: 1);

            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.True(move.IsCastle());

            if ((int)targetSquare == Constants.WhiteLongCastleKingSquare || (int)targetSquare == Constants.BlackLongCastleKingSquare)
            {
                Assert.True(move.IsLongCastle());
                Assert.False(move.IsShortCastle());
            }
            else
            {
                Assert.False(move.IsLongCastle());
                Assert.True(move.IsShortCastle());
            }
        }

        [Theory]
        [InlineData(BoardSquares.g2, BoardSquares.g4)]
        [InlineData(BoardSquares.b2, BoardSquares.b4)]
        [InlineData(BoardSquares.b7, BoardSquares.b5)]
        [InlineData(BoardSquares.g7, BoardSquares.g5)]
        public void DoublePawnPush(BoardSquares sourceSquare, BoardSquares targetSquare)
        {
            var move = new Move((int)sourceSquare, (int)targetSquare, (int)Piece.P, isDoublePawnPush: 1);

            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.True(move.IsDoublePawnPush());
        }
    }
}
