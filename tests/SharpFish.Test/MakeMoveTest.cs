using SharpFish.Model;
using System.Linq;
using Xunit;

namespace SharpFish.Test
{
    public class MakeMoveTest
    {
        #region Captures

        /// <summary>
        /// 8   . . . . k . . .
        /// 7   . . . . . . . .
        /// 6   . . . . . . . .
        /// 5   . . . b . . . .
        /// 4   . . . . N . . .
        /// 3   . . . . . . . .
        /// 2   . . . . . . . .
        /// 1   . . . . K . . .
        ///     a b c d e f g h
        ///     Side:       Black
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void Capture_Black()
        {
            // Arrange
            var position = new Position("4k3/8/8/3b4/4N3/8/8/4K3 b - - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.b].GetBit(BoardSquares.d5));
            Assert.True(position.PieceBitBoards[(int)Piece.N].GetBit(BoardSquares.e4));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.e4));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.d5));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e4));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.d5));

            var moves = MovesGenerator.GenerateAllMoves(position);

            var captureMove = moves.Single(m => m.IsCapture());

            // Act
            var newPosition = new Position(position, captureMove);

            // Assert
            Assert.False(newPosition.PieceBitBoards[(int)Piece.b].GetBit(BoardSquares.d5));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.N].GetBit(BoardSquares.e4));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.b].GetBit(BoardSquares.e4));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.e4));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.d5));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.e4));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.d5));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e4));
        }

        /// <summary>
        /// 8   . . . . k . . .
        /// 7   . . . . . . . .
        /// 6   . . . . . . . .
        /// 5   . . . B . . . .
        /// 4   . . . . n . . .
        /// 3   . . . . . . . .
        /// 2   . . . . . . . .
        /// 1   . . . . K . . .
        ///     a b c d e f g h
        ///     Side:       White
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void Capture_White()
        {
            // Arrange
            var position = new Position("4k3/8/8/3B4/4n3/8/8/4K3 w - - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.B].GetBit(BoardSquares.d5));
            Assert.True(position.PieceBitBoards[(int)Piece.n].GetBit(BoardSquares.e4));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.e4));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.d5));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e4));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.d5));

            var moves = MovesGenerator.GenerateAllMoves(position);
            var captureMove = moves.Single(m => m.IsCapture());

            // Act
            var newPosition = new Position(position, captureMove);

            // Assert
            Assert.False(newPosition.PieceBitBoards[(int)Piece.B].GetBit(BoardSquares.d5));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.n].GetBit(BoardSquares.e4));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.B].GetBit(BoardSquares.e4));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.e4));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.d5));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.e4));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.d5));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e4));
        }

        #endregion

        #region Promotions

        /// <summary>
        /// 8   . . . . k . . .
        /// 7   . P . . . . . .
        /// 6   . . . . . . . .
        /// 5   . . . . . . . .
        /// 4   . . . . . . . .
        /// 3   . . . . . . . .
        /// 2   . . . . . . . .
        /// 1   . . . . K . . .
        ///     a b c d e f g h
        ///     Side:       White
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void Promotion_White()
        {
            // Arrange
            var position = new Position("4k3/1P6/8/8/8/8/8/4K3 w - - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.b7));
            Assert.False(position.PieceBitBoards[(int)Piece.N].GetBit(BoardSquares.b8));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b7));
            Assert.False(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b7));
            Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b8));

            var moves = MovesGenerator.GenerateAllMoves(position);
            var promotionMove = moves.Single(m => m.PromotedPiece() == (int)Piece.N);

            // Act
            var newPosition = new Position(position, promotionMove);

            // Assert
            Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.b7));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.N].GetBit(BoardSquares.b8));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b7));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b8));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b7));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b8));
        }

        /// <summary>
        /// 8   . . . . k . . .
        /// 7   . . . . . . . .
        /// 6   . . . . . . . .
        /// 5   . . . . . . . .
        /// 4   . . . . . . . .
        /// 3   . . . . . . . .
        /// 2   . . . . . . . .
        /// 1   . p . . K . . .
        ///     a b c d e f g h
        ///     Side:       Black
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void Promotion_Black()
        {
            // Arrange
            var position = new Position("4k3/8/8/8/8/8/1p6/4K3 b - - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.b2));
            Assert.False(position.PieceBitBoards[(int)Piece.n].GetBit(BoardSquares.b1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.b2));
            Assert.False(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.b1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b2));
            Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b1));

            var moves = MovesGenerator.GenerateAllMoves(position);
            var promotionMove = moves.Single(m => m.PromotedPiece() == (int)Piece.n);

            // Act
            var newPosition = new Position(position, promotionMove);

            // Assert
            Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.b2));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.n].GetBit(BoardSquares.b1));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.b2));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.b1));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b2));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b1));
        }

        /// <summary>
        /// 8   r k . . . . . .
        /// 7   . P . . . . . .
        /// 6   . . . . . . . .
        /// 5   . . . . . . . .
        /// 4   . . . . . . . .
        /// 3   . . . . . . . .
        /// 2   . . . . . . . .
        /// 1   . . . . K . . .
        ///     a b c d e f g h
        ///     Side:       White
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void PromotionWithCapture_White()
        {
            // Arrange
            var position = new Position("rk6/1P6/8/8/8/8/8/4K3 w - - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.b7));
            Assert.False(position.PieceBitBoards[(int)Piece.N].GetBit(BoardSquares.a8));
            Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.a8));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b7));
            Assert.False(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.a8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.a8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b7));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a8));

            var moves = MovesGenerator.GenerateAllMoves(position);
            var promotionMove = moves.Single(m => m.PromotedPiece() == (int)Piece.N);

            // Act
            var newPosition = new Position(position, promotionMove);

            // Assert
            Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.b7));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.N].GetBit(BoardSquares.a8));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.a8));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b7));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.a8));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.a8));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b7));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a8));
        }

        /// <summary>
        /// 8   . . . . k . . .
        /// 7   . . . . . . . .
        /// 6   . . . . . . . .
        /// 5   . . . . . . . .
        /// 4   . . . . . . . .
        /// 3   . . . . . . . .
        /// 2   . p . . . . . .
        /// 1   R K . . . . . .
        ///     a b c d e f g h
        ///     Side:       White
        ///     Enpassant:  no
        ///     Castling:   -- | --
        /// </summary>
        [Fact]
        public void PromotionWithCapture_Black()
        {
            // Arrange
            var position = new Position("4k3/8/8/8/8/8/1p6/RK6 b - - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.b2));
            Assert.False(position.PieceBitBoards[(int)Piece.n].GetBit(BoardSquares.a1));
            Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.a1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.b2));
            Assert.False(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.a1));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.a1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b2));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a1));

            var moves = MovesGenerator.GenerateAllMoves(position);
            var promotionMove = moves.Single(m => m.PromotedPiece() == (int)Piece.n);

            // Act
            var newPosition = new Position(position, promotionMove);

            // Assert
            Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.b2));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.n].GetBit(BoardSquares.a1));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.a1));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.b2));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.a1));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.a1));

            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b2));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a1));
        }

        #endregion

        #region EnPassant

        [Fact]
        public void EnPassant_White()
        {
            // Arrange
            var position = new Position("4k3/8/8/1Pp5/8/8/8/4K3 w - c6 0 1");

            Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.b5));
            Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.c5));
            Assert.False(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.c6));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b5));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.c5));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b5));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.c5));
            Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.c6));

            var moves = MovesGenerator.GenerateAllMoves(position);
            var enPassant = moves.Single(m => m.IsEnPassant());

            // Act
            var newPosition = new Position(position, enPassant);

            // Assert
            Assert.True(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.c6));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.c5));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.b5));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.c6));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b5));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.c5));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.c6));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.c5));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b5));
        }

        [Fact]
        public void EnPassant_Black()
        {
            // Arrange
            var position = new Position("4k3/8/8/8/1Pp5/8/8/4K3 b - b3 0 1");

            Assert.True(position.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.b4));
            Assert.True(position.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.c4));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b4));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.c4));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b4));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.c4));
            Assert.False(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b3));

            var moves = MovesGenerator.GenerateAllMoves(position);
            var enPassant = moves.Single(m => m.IsEnPassant());

            // Act
            var newPosition = new Position(position, enPassant);

            // Assert
            Assert.True(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.b3));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.p].GetBit(BoardSquares.c4));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.P].GetBit(BoardSquares.b4));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.b3));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.b4));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.c4));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b3));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.c4));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b4));
        }

        [Theory]
        [InlineData("rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1")]
        [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1")]
        [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1")]
        public void DoublePawnPushEnablesEnPassant(string fen)
        {
            var position = new Position(fen);

            foreach (var move in MovesGenerator.GenerateAllMoves(position).Where(m => m.IsDoublePawnPush()))
            {
                var newPosition = new Position(position, move);
                Assert.NotEqual(position.EnPassant, newPosition.EnPassant);
            }
        }

        [Theory]
        [InlineData("r3kb2/1b4PP/p2ppppp/NPp1qn1r/2B2N2/1PPPPPP1/1p4B1/R2QK2R w KQq c6 0 1")]
        [InlineData("r3kb2/1b4PP/p2ppppp/N1p1qn1r/2B2N2/1PPPPPP1/1p4B1/R2QK2R w KQq c6 0 1")]
        [InlineData("r3kb2/1b6/p2pppp1/N1p1qn1r/2B2NPp/1PPPPP2/1p3B2/R2QK2R b KQq g3 0 1")]
        [InlineData("r3kb2/1b6/p2pppp1/N1p1qn1r/2B2NP1/1PPPPP2/1p3B2/R2QK2R b KQq g3 0 1")]
        [InlineData("rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1")]
        [InlineData("4k3/8/8/1Pp5/8/8/8/4K3 w - c6 0 1")]
        public void AnyMoveDisablesEnPassant(string fen)
        {
            var position = new Position(fen);

            Assert.NotEqual(BoardSquares.noSquare, position.EnPassant);

            foreach (var move in MovesGenerator.GenerateAllMoves(position).Where(m => !m.IsDoublePawnPush()))
            {
                var newPosition = new Position(position, move);
                Assert.Equal(BoardSquares.noSquare, newPosition.EnPassant);
            }
        }

        #endregion

        #region Castling

        [Fact]
        public void CastlingRemovesCastlingRights_Short_White()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.K].GetBit(BoardSquares.e1));
            Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.a1));
            Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.h1));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.e1));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.a1));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.h1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.h1));
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var shortCastling = moves.Single(m => m.IsShortCastle());

            // Act
            var newPosition = new Position(position, shortCastling);

            // Assert - position and occupancy after castling
            Assert.True(newPosition.PieceBitBoards[(int)Piece.K].GetBit(Constants.WhiteShortCastleKingSquare));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.R].GetBit(Constants.WhiteShortCastleRookSquare));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.a1));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.K].GetBit(BoardSquares.e1));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.h1));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(Constants.WhiteShortCastleKingSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(Constants.WhiteShortCastleRookSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.a1));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.e1));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.h1));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.WhiteShortCastleKingSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.WhiteShortCastleRookSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a1));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e1));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.h1));

            // Assert - Castling rights
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void CastlingRemovesCastlingRights_Long_White()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.K].GetBit(BoardSquares.e1));
            Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.a1));
            Assert.True(position.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.h1));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.e1));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.a1));
            Assert.True(position.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.h1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a1));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.h1));
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var shortCastling = moves.Single(m => m.IsLongCastle());

            // Act
            var newPosition = new Position(position, shortCastling);

            // Assert - position and occupancy after castling
            Assert.True(newPosition.PieceBitBoards[(int)Piece.K].GetBit(Constants.WhiteLongCastleKingSquare));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.R].GetBit(Constants.WhiteLongCastleRookSquare));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.h1));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.K].GetBit(BoardSquares.e1));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.R].GetBit(BoardSquares.a1));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(Constants.WhiteLongCastleKingSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(Constants.WhiteLongCastleRookSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.h1));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.e1));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.White].GetBit(BoardSquares.a1));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.WhiteLongCastleKingSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.WhiteLongCastleRookSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.h1));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e1));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a1));

            // Assert - Castling rights
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void CastlingRemovesCastlingRights_Short_Black()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.k].GetBit(BoardSquares.e8));
            Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.a8));
            Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.h8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.e8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.a8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.h8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.h8));
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var shortCastling = moves.Single(m => m.IsShortCastle());

            // Act
            var newPosition = new Position(position, shortCastling);

            // Assert - position and occupancy after castling
            Assert.True(newPosition.PieceBitBoards[(int)Piece.k].GetBit(Constants.BlackShortCastleKingSquare));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.r].GetBit(Constants.BlackShortCastleRookSquare));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.a8));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.k].GetBit(BoardSquares.e8));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.h8));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(Constants.BlackShortCastleKingSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(Constants.BlackShortCastleRookSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.a8));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.e8));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.h8));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.BlackShortCastleKingSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.BlackShortCastleRookSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a8));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e8));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.h8));

            // Assert - Castling rights
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
        }

        [Fact]
        public void CastlingRemovesCastlingRights_Long_Black()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
            Assert.True(position.PieceBitBoards[(int)Piece.k].GetBit(BoardSquares.e8));
            Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.a8));
            Assert.True(position.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.h8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.e8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.a8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.h8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a8));
            Assert.True(position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.h8));
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var shortCastling = moves.Single(m => m.IsLongCastle());

            // Act
            var newPosition = new Position(position, shortCastling);

            // Assert - position and occupancy after castling
            Assert.True(newPosition.PieceBitBoards[(int)Piece.k].GetBit(Constants.BlackLongCastleKingSquare));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.r].GetBit(Constants.BlackLongCastleRookSquare));
            Assert.True(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.h8));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.k].GetBit(BoardSquares.e8));
            Assert.False(newPosition.PieceBitBoards[(int)Piece.r].GetBit(BoardSquares.a8));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(Constants.BlackLongCastleKingSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(Constants.BlackLongCastleRookSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.h8));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.e8));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Black].GetBit(BoardSquares.a8));

            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.BlackLongCastleKingSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(Constants.BlackLongCastleRookSquare));
            Assert.True(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.h8));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.e8));
            Assert.False(newPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.a8));

            // Assert - Castling rights
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
        }

        [Fact]
        public void MovingKingRemovesCastlingRights_White()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var quietKingMove = moves.First(m => !m.IsCastle() && m.Piece() == (int)Piece.K + Utils.PieceOffset(position.Side));

            // Act
            var newPosition = new Position(position, quietKingMove);

            // Assert - Castling rights
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void MovingKingRemovesCastlingRights_Black()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var quietKingMove = moves.First(m => !m.IsCastle() && m.Piece() == (int)Piece.K + Utils.PieceOffset(position.Side));

            // Act
            var newPosition = new Position(position, quietKingMove);

            // Assert - Castling rights
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
        }

        [Fact]
        public void MovingRookRemovesCastlingRights_Queenside_White()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookMove = moves.First(m =>
                m.Piece() == (int)Piece.R + Utils.PieceOffset(position.Side)
                && !m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.a8 + (7 * 8 * (int)position.Side));

            // Act
            var newPosition = new Position(position, rookMove);

            // Assert - Castling rights
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void MovingRookRemovesCastlingRights_Kingside_White()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookMove = moves.First(m =>
                m.Piece() == (int)Piece.R + Utils.PieceOffset(position.Side)
                && !m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.h8 + (7 * 8 * (int)position.Side));

            // Act
            var newPosition = new Position(position, rookMove);

            // Assert - Castling rights
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void MovingRookRemovesCastlingRights_Queenside_Black()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookMove = moves.First(m =>
                m.Piece() == (int)Piece.R + Utils.PieceOffset(position.Side)
                && !m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.a8 + (7 * 8 * (int)position.Side));

            // Act
            var newPosition = new Position(position, rookMove);

            // Assert - Castling rights
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void MovingRookRemovesCastlingRights_Kingside_Black()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookMove = moves.First(m =>
                m.Piece() == (int)Piece.R + Utils.PieceOffset(position.Side)
                && !m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.h8 + (7 * 8 * (int)position.Side));

            // Act
            var newPosition = new Position(position, rookMove);

            // Assert - Castling rights
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void CapturingRookRemovesCastlingRights_QueenSide_BlackRook_Queenside()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/3B4/8/8/8/R3K2R w KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookCapture = moves.First(m =>
                m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
                && m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.d5);

            Assert.Equal((int)BoardSquares.a8, rookCapture.TargetSquare());

            // Act
            var newPosition = new Position(position, rookCapture);

            // Assert - Castling rights
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void CapturingRookRemovesCastlingRights_QueenSide_BlackRook_Kingside()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/4B3/8/8/8/R3K2R w KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookCapture = moves.First(m =>
                m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
                && m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.e5);

            Assert.Equal((int)BoardSquares.h8, rookCapture.TargetSquare());

            // Act
            var newPosition = new Position(position, rookCapture);

            // Assert - Castling rights
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void CapturingRookRemovesCastlingRights_QueenSide_WhiteRook_Queenside()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/4b3/8/8/8/R3K2R b KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookCapture = moves.First(m =>
                m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
                && m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.e5);

            Assert.Equal((int)BoardSquares.a1, rookCapture.TargetSquare());

            // Act
            var newPosition = new Position(position, rookCapture);

            // Assert - Castling rights
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        [Fact]
        public void CapturingRookRemovesCastlingRights_QueenSide_WhiteRook_Kingside()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/3b4/8/8/8/R3K2R b KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookCapture = moves.First(m =>
                m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
                && m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.d5);

            Assert.Equal((int)BoardSquares.h1, rookCapture.TargetSquare());

            // Act
            var newPosition = new Position(position, rookCapture);

            // Assert - Castling rights
            Assert.Equal(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        /// <summary>
        /// 8   r . . . k . . r
        /// 7   . r . . . . . .
        /// 6   . . . . . . . .
        /// 5   . . . B . . . .
        /// 4   . . . . . . . .
        /// 3   . . . . . . . .
        /// 2   . . . . . . . .
        /// 1   R . . . K . . R
        ///     a b c d e f g h
        ///     Side:       White
        ///     Enpassant:  no
        ///     Castling:   KQ | kq
        /// </summary>
        [Fact]
        public void CapturingAnotherRookShouldntRemoveCastlingRights_BlackRook()
        {
            // Arrange
            var position = new Position("r3k2r/1r6/8/3B4/8/8/8/R3K2R w KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookCapture = moves.First(m =>
                m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
                && m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.d5);

            // Act
            var newPosition = new Position(position, rookCapture);

            // Assert - Castling rights
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        /// <summary>
        /// 8   r . . . k . . r
        /// 7   . . . . . . . .
        /// 6   . . . . . . . .
        /// 5   . . . b . . . .
        /// 4   . . . . . . . .
        /// 3   . . . . . . . .
        /// 2   . . . . . . R .
        /// 1   R . . . K . . R
        ///     a b c d e f g h
        ///     Side:       Black
        ///     Enpassant:  no
        ///     Castling:   KQ | kq
        /// </summary>
        [Fact]
        public void CapturingAnotherRookShouldntRemoveCastlingRights_WhiteRook()
        {
            // Arrange
            var position = new Position("r3k2r/8/8/3b4/8/8/6R1/R3K2R b KQkq - 0 1");
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, position.Castle & (int)CastlingRights.BQ);

            var moves = MovesGenerator.GenerateAllMoves(position);
            var rookCapture = moves.First(m =>
                m.Piece() == (int)Piece.B + Utils.PieceOffset(position.Side)
                && m.IsCapture()
                && m.SourceSquare() == (int)BoardSquares.d5);

            // Act
            var newPosition = new Position(position, rookCapture);

            // Assert - Castling rights
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.WQ);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BK);
            Assert.NotEqual(default, newPosition.Castle & (int)CastlingRights.BQ);
        }

        /// <summary>
        /// Nc3 should enable castling rights
        /// </summary>
        [Fact]
        public void MovingAPieceOutOfTheCastleWayEnablesCastlingRights()
        {
            Assert.False(true);
        }

        /// <summary>
        /// Ra2 - Ra1 - Nc3 shouldn't enable castling rights
        /// </summary>
        [Fact]
        public void MovingAPieceOutOfTheCastleWayShouldNotEnableCastlingRights_CastlingRightsLostForever()
        {
            Assert.False(true);
        }

        /// <summary>
        /// Nc3b2 should disable castling rights
        /// </summary>
        [Fact]
        public void MovingAPieceIntoTheCastleWayRemovesCastlingRights()
        {
            Assert.False(true);
        }

        #endregion
    }
}
