using Lynx.Model;
using Xunit;

namespace Lynx.Test.MoveGeneration
{
    public class EncodeDecodeMoveTest
    {
        [Theory]
        [InlineData(BoardSquare.e4, BoardSquare.e5, Piece.P, false)]
        [InlineData(BoardSquare.e4, BoardSquare.d5, Piece.P, true)]
        [InlineData(BoardSquare.e5, BoardSquare.e4, Piece.p, false)]
        [InlineData(BoardSquare.e5, BoardSquare.d4, Piece.p, true)]
        [InlineData(BoardSquare.a1, BoardSquare.a4, Piece.R, false)]
        [InlineData(BoardSquare.a1, BoardSquare.a4, Piece.R, true)]
        [InlineData(BoardSquare.a1, BoardSquare.a4, Piece.r, false)]
        [InlineData(BoardSquare.a1, BoardSquare.a4, Piece.r, true)]
        [InlineData(BoardSquare.a1, BoardSquare.h8, Piece.B, false)]
        [InlineData(BoardSquare.a1, BoardSquare.h8, Piece.B, true)]
        [InlineData(BoardSquare.a1, BoardSquare.h8, Piece.b, false)]
        [InlineData(BoardSquare.a1, BoardSquare.h8, Piece.b, true)]
        [InlineData(BoardSquare.a1, BoardSquare.h8, Piece.Q, false)]
        [InlineData(BoardSquare.a1, BoardSquare.h8, Piece.Q, true)]
        [InlineData(BoardSquare.a1, BoardSquare.h8, Piece.q, false)]
        [InlineData(BoardSquare.a1, BoardSquare.h8, Piece.q, true)]
        [InlineData(BoardSquare.b1, BoardSquare.c3, Piece.N, true)]
        [InlineData(BoardSquare.b1, BoardSquare.c3, Piece.N, false)]
        [InlineData(BoardSquare.b8, BoardSquare.c6, Piece.n, true)]
        [InlineData(BoardSquare.b8, BoardSquare.c6, Piece.n, false)]
        public void SourceSquare_TargetSquare_Piece_Capture(BoardSquare sourceSquare, BoardSquare targetSquare, Piece piece, bool isCapture)
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
        [InlineData(BoardSquare.a7, BoardSquare.a8, Piece.Q)]
        [InlineData(BoardSquare.a7, BoardSquare.a8, Piece.R)]
        [InlineData(BoardSquare.a7, BoardSquare.a8, Piece.B)]
        [InlineData(BoardSquare.a7, BoardSquare.a8, Piece.N)]
        [InlineData(BoardSquare.a2, BoardSquare.a1, Piece.q)]
        [InlineData(BoardSquare.a2, BoardSquare.a1, Piece.r)]
        [InlineData(BoardSquare.a2, BoardSquare.a1, Piece.b)]
        [InlineData(BoardSquare.a2, BoardSquare.a1, Piece.n)]
        public void Promotion(BoardSquare sourceSquare, BoardSquare targetSquare, Piece promotedPiece)
        {
            var move = new Move((int)sourceSquare, (int)targetSquare, (int)Piece.P, promotedPiece: (int)promotedPiece);

            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.Equal((int)promotedPiece, move.PromotedPiece());
        }

        [Theory]
        [InlineData(BoardSquare.e5, BoardSquare.f6, false)]
        [InlineData(BoardSquare.e5, BoardSquare.f6, true)]
        [InlineData(BoardSquare.e4, BoardSquare.d3, false)]
        [InlineData(BoardSquare.e4, BoardSquare.d3, true)]
        public void EnPassant(BoardSquare sourceSquare, BoardSquare targetSquare, bool enPassant)
        {
            var move = new Move((int)sourceSquare, (int)targetSquare, (int)Piece.P, isEnPassant: enPassant ? 1 : 0);

            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.Equal(enPassant, move.IsEnPassant());
        }

        [Theory]
        [InlineData(BoardSquare.e1, BoardSquare.g1, true, false)]
        [InlineData(BoardSquare.e1, BoardSquare.c1, false, true)]
        [InlineData(BoardSquare.e8, BoardSquare.g8, true, false)]
        [InlineData(BoardSquare.e8, BoardSquare.c8, false, true)]
        [InlineData(BoardSquare.e1, BoardSquare.e2, false, false)]
        [InlineData(BoardSquare.e8, BoardSquare.e7, false, false)]
        public void Castling(BoardSquare sourceSquare, BoardSquare targetSquare, bool isShortCastle, bool isLongCastle)
        {
            var move = new Move((int)sourceSquare, (int)targetSquare, (int)Piece.K,
                isShortCastle: isShortCastle ? 1 : 0, isLongCastle: isLongCastle ? 1 : 0);

            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());

            Assert.Equal(isShortCastle, move.IsShortCastle());
            Assert.Equal(isLongCastle, move.IsLongCastle());
            Assert.Equal(isShortCastle || isLongCastle, move.IsCastle());
        }

        [Theory]
        [InlineData(BoardSquare.g2, BoardSquare.g4)]
        [InlineData(BoardSquare.b2, BoardSquare.b4)]
        [InlineData(BoardSquare.b7, BoardSquare.b5)]
        [InlineData(BoardSquare.g7, BoardSquare.g5)]
        public void DoublePawnPush(BoardSquare sourceSquare, BoardSquare targetSquare)
        {
            var move = new Move((int)sourceSquare, (int)targetSquare, (int)Piece.P, isDoublePawnPush: 1);

            Assert.Equal((int)sourceSquare, move.SourceSquare());
            Assert.Equal((int)targetSquare, move.TargetSquare());
            Assert.True(move.IsDoublePawnPush());
        }
    }
}
