﻿using Lynx.Model;
using Xunit;
using CR = Lynx.Model.CastlingRights;

namespace Lynx.Test
{
    public class FENParserTest
    {
        [Fact]
        public void PieceBitBoards_InitialPosition()
        {
            // Arrange

            // Make sure a previous Fen doesn't change anything
            const string previuosFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            bool success;
            BitBoard[] pieceBitBoards;
            (success, _, _, _, _, _, _, _) = FENParser.ParseFEN(previuosFen);
            Assert.True(success);

            const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            // Act
            (success, pieceBitBoards, _, _, _, _, _, _) = FENParser.ParseFEN(fen);

            // Assert
            Assert.True(success);

            var whitePawns = pieceBitBoards[(int)Piece.P];
            Assert.Equal(0b1111_1111UL << (6 * 8), whitePawns.Board);
            var blackPawns = pieceBitBoards[(int)Piece.p];
            Assert.Equal(0b1111_1111UL << (1 * 8), blackPawns.Board);

            var whiteRooks = pieceBitBoards[(int)Piece.R];
            Assert.Equal(0b1000_0001UL << (7 * 8), whiteRooks.Board);
            var blackRooks = pieceBitBoards[(int)Piece.r];
            Assert.Equal(0b1000_0001UL << (0 * 8), blackRooks.Board);

            var whiteKnights = pieceBitBoards[(int)Piece.N];
            Assert.Equal(0b0100_0010UL << (7 * 8), whiteKnights.Board);
            var blackKnights = pieceBitBoards[(int)Piece.n];
            Assert.Equal(0b0100_0010UL << (0 * 8), blackKnights.Board);

            var whiteBishops = pieceBitBoards[(int)Piece.B];
            Assert.Equal(0b0010_0100UL << (7 * 8), whiteBishops.Board);
            var blackBishops = pieceBitBoards[(int)Piece.b];
            Assert.Equal(0b0010_0100UL << (0 * 8), blackBishops.Board);

            var whiteQueen = pieceBitBoards[(int)Piece.Q];
            Assert.Equal(0b0000_1000UL << (int)BoardSquare.a1, whiteQueen.Board);
            var blackQueen = pieceBitBoards[(int)Piece.q];
            Assert.Equal(0b0000_1000UL << (0 * 8), blackQueen.Board);

            var whiteKing = pieceBitBoards[(int)Piece.K];
            Assert.Equal(0b0001_0000UL << (7 * 8), whiteKing.Board);
            var blackKing = pieceBitBoards[(int)Piece.k];
            Assert.Equal(0b0001_0000UL << (0 * 8), blackKing.Board);
        }

        [Fact]
        public void PieceBitBoards()
        {
            // Arrange
            const string fen = Constants.TrickyTestPositionFEN;

            // Act
            bool success;
            BitBoard[] pieceBitBoards;
            (success, pieceBitBoards, _, _, _, _, _, _) = FENParser.ParseFEN(fen);

            // Assert
            Assert.True(success);

            Assert.True(pieceBitBoards[(int)Piece.p].GetBit(BoardSquare.a7));
            Assert.True(pieceBitBoards[(int)Piece.p].GetBit(BoardSquare.b4));
            Assert.True(pieceBitBoards[(int)Piece.p].GetBit(BoardSquare.c7));
            Assert.True(pieceBitBoards[(int)Piece.p].GetBit(BoardSquare.d7));
            Assert.True(pieceBitBoards[(int)Piece.p].GetBit(BoardSquare.e6));
            Assert.True(pieceBitBoards[(int)Piece.p].GetBit(BoardSquare.f7));
            Assert.True(pieceBitBoards[(int)Piece.p].GetBit(BoardSquare.g6));
            Assert.True(pieceBitBoards[(int)Piece.p].GetBit(BoardSquare.h3));

            Assert.True(pieceBitBoards[(int)Piece.r].GetBit(BoardSquare.a8));
            Assert.True(pieceBitBoards[(int)Piece.r].GetBit(BoardSquare.h8));

            Assert.True(pieceBitBoards[(int)Piece.n].GetBit(BoardSquare.b6));
            Assert.True(pieceBitBoards[(int)Piece.n].GetBit(BoardSquare.f6));

            Assert.True(pieceBitBoards[(int)Piece.b].GetBit(BoardSquare.a6));
            Assert.True(pieceBitBoards[(int)Piece.b].GetBit(BoardSquare.g7));

            Assert.True(pieceBitBoards[(int)Piece.q].GetBit(BoardSquare.e7));
            Assert.True(pieceBitBoards[(int)Piece.k].GetBit(BoardSquare.e8));

            Assert.True(pieceBitBoards[(int)Piece.P].GetBit(BoardSquare.a2));
            Assert.True(pieceBitBoards[(int)Piece.P].GetBit(BoardSquare.b2));
            Assert.True(pieceBitBoards[(int)Piece.P].GetBit(BoardSquare.c2));
            Assert.True(pieceBitBoards[(int)Piece.P].GetBit(BoardSquare.d5));
            Assert.True(pieceBitBoards[(int)Piece.P].GetBit(BoardSquare.e4));
            Assert.True(pieceBitBoards[(int)Piece.P].GetBit(BoardSquare.f2));
            Assert.True(pieceBitBoards[(int)Piece.P].GetBit(BoardSquare.g2));
            Assert.True(pieceBitBoards[(int)Piece.P].GetBit(BoardSquare.h2));

            Assert.True(pieceBitBoards[(int)Piece.R].GetBit(BoardSquare.a1));
            Assert.True(pieceBitBoards[(int)Piece.R].GetBit(BoardSquare.h1));

            Assert.True(pieceBitBoards[(int)Piece.N].GetBit(BoardSquare.c3));
            Assert.True(pieceBitBoards[(int)Piece.N].GetBit(BoardSquare.e5));

            Assert.True(pieceBitBoards[(int)Piece.B].GetBit(BoardSquare.d2));
            Assert.True(pieceBitBoards[(int)Piece.B].GetBit(BoardSquare.e2));

            Assert.True(pieceBitBoards[(int)Piece.Q].GetBit(BoardSquare.f3));
            Assert.True(pieceBitBoards[(int)Piece.K].GetBit(BoardSquare.e1));
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/8/8 w - - 0 1")]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 ")]
        [InlineData(Constants.TrickyTestPositionFEN)]
        [InlineData(Constants.KillerPositionFEN)]
        [InlineData(Constants.CmkPositionFEN)]
        public void OccupancyBitBoards(string fen)
        {
            // Arrange
            // Make sure a previous Fen doesn't change anything
            const string previuosFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            bool success;
            BitBoard[] pieceBitBoards;
            BitBoard[] occupancyBitBoards;
            (success, _, _, _, _, _, _, _) = FENParser.ParseFEN(previuosFen);
            Assert.True(success);

            // Act
            (success, pieceBitBoards, occupancyBitBoards, _, _, _, _, _) = FENParser.ParseFEN(fen);

            // Assert
            Assert.True(success);

            var expectedWhiteOccupancy = 0UL;
            expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.P].Board;
            expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.N].Board;
            expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.B].Board;
            expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.R].Board;
            expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.Q].Board;
            expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.K].Board;

            Assert.Equal(expectedWhiteOccupancy, occupancyBitBoards[(int)Side.White].Board);

            var expectedBlackOccupancy = 0UL;
            expectedBlackOccupancy |= pieceBitBoards[(int)Piece.p].Board;
            expectedBlackOccupancy |= pieceBitBoards[(int)Piece.n].Board;
            expectedBlackOccupancy |= pieceBitBoards[(int)Piece.b].Board;
            expectedBlackOccupancy |= pieceBitBoards[(int)Piece.r].Board;
            expectedBlackOccupancy |= pieceBitBoards[(int)Piece.q].Board;
            expectedBlackOccupancy |= pieceBitBoards[(int)Piece.k].Board;

            Assert.Equal(expectedBlackOccupancy, occupancyBitBoards[(int)Side.Black].Board);

            var expectedCombinedOccupancy = expectedWhiteOccupancy | expectedBlackOccupancy;

            Assert.Equal(expectedCombinedOccupancy, occupancyBitBoards[(int)Side.Both].Board);
        }

        [Theory]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", Side.White)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR W KQkq - 0 1", Side.White)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", Side.Black)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR B KQkq - 0 1", Side.Black)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR A KQkq - 0 1", Side.Both)]
        public void SideToMove(string fen, Side expectedSide)
        {
            bool success;
            Side side;
            (success, _, _, side, _, _, _, _) = FENParser.ParseFEN(fen);

            if (expectedSide != Side.Both)
            {
                Assert.True(success);
                Assert.Equal(expectedSide, side);
            }
            else
            {
                Assert.False(success);
                Assert.Equal(Side.Both, side);
            }
        }

        [Theory]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BK | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Qkq - 0 1", (int)CR.WQ | (int)CR.BK | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kkq - 0 1", (int)CR.WK | (int)CR.BK | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQq - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQk - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BK)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1", (int)CR.WK | (int)CR.WQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w kq - 0 1", (int)CR.BK | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Qq - 0 1", (int)CR.WQ | (int)CR.BQ)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kk - 0 1", (int)CR.WK | (int)CR.BK)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1", 0)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w A - 0 1", -1)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KA - 0 1", -1)]
        public void CastlingRights(string fen, int expectedCastleResult)
        {
            // Arrange
            // Make sure a previous Fen doesn't change anything
            const string previuosFen = "8/8/8/8/8/8/8/8 w KQkq 1234 0 1";
            bool success;
            (success, _, _, _, _, _, _, _) = FENParser.ParseFEN(previuosFen);
            Assert.False(success);

            // Act
            int castleResult;
            (success, _, _, _, castleResult, _, _, _) = FENParser.ParseFEN(fen);

            // Assert
            if (expectedCastleResult >= 0)
            {
                Assert.True(success);
                Assert.Equal(expectedCastleResult, castleResult);
            }
            else
            {
                Assert.False(success);
            }
        }

        [Theory]
        [InlineData("rnbqkbnr/pppp1ppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b Qq e3 0 1", BoardSquare.e3)]
        [InlineData("rnbqkbnr/ppppp1pp/8/8/4pP2/8/PPPPP1PP/RNBQKBNR b Qq f3 0 1", BoardSquare.f3)]
        [InlineData("rnbqkbnr/pp1ppppp/8/1Pp5/8/8/P1PPPPPP/RNBQKBNR w KQkq c6 0 1", BoardSquare.c6)]
        [InlineData("rnbqkbnr/ppp1pppp/8/2Pp4/8/8/PP1PPPPP/RNBQKBNR w KQkq d6 0 1", BoardSquare.d6)]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardSquare.noSquare)]
        public void EnPassant(string fen, BoardSquare expectedEnPassantSquare)
        {
            bool success;
            BoardSquare enPassant;
            (success, _, _, _, _, enPassant, _, _) = FENParser.ParseFEN(fen);

            Assert.True(success);
            Assert.Equal(expectedEnPassantSquare, enPassant);
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq e6 0 1")]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq e7 0 1")]
        [InlineData("rnbqkbnr/pppp1ppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b Qq a3 0 1")]  // e3 could be
        [InlineData("rnbqkbnr/ppppp1pp/8/8/4pP2/8/PPPPP1PP/RNBQKBNR b Qq b3 0 1")]  // f3 could be
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq i1 0 1")]
        [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq i3 0 1")]
        public void EnPassant_Error(string fen)
        {
            bool success;
            (success, _, _, _, _, _, _, _) = FENParser.ParseFEN(fen);

            Assert.False(success);
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 0 1", 0)]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 1 1", 1)]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 51 1", 51)]
        public void HalfMoveClock(string fen, int expectedHalfMoves)
        {
            bool success;
            int halfMoves;
            (success, _, _, _, _, _, halfMoves, _) = FENParser.ParseFEN(fen);

            Assert.True(success);
            Assert.Equal(expectedHalfMoves, halfMoves);
        }

        [Theory]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 0 1", 1)]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 0 50", 50)]
        [InlineData("8/8/8/8/8/8/8/8 w KQkq - 0 100", 100)]
        public void FullMoveCounter(string fen, int expectedFullMoveCounter)
        {
            bool success;
            int fullMoveCounter;
            (success, _, _, _, _, _, _, fullMoveCounter) = FENParser.ParseFEN(fen);

            Assert.True(success);
            Assert.Equal(expectedFullMoveCounter, fullMoveCounter);
        }
    }
}
