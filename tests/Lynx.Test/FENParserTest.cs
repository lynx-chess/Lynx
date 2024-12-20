using Lynx.Model;
using NUnit.Framework;
using CR = Lynx.Model.CastlingRights;

namespace Lynx.Test;

public class FENParserTest
{
    [Test]
    public void PieceBitBoards_InitialPosition()
    {
        // Arrange

        // Make sure a previous Fen doesn't change anything
        const string previuosFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        _ = FENParser.ParseFEN(previuosFen);

        const string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        // Act
        var result = FENParser.ParseFEN(fen);
        BitBoard[] pieceBitBoards = result.PieceBitBoards;

        // Assert

        var whitePawns = pieceBitBoards[(int)Piece.P];
        Assert.AreEqual(0b1111_1111UL << (6 * 8), whitePawns);
        var blackPawns = pieceBitBoards[(int)Piece.p];
        Assert.AreEqual(0b1111_1111UL << (1 * 8), blackPawns);

        var whiteRooks = pieceBitBoards[(int)Piece.R];
        Assert.AreEqual(0b1000_0001UL << (7 * 8), whiteRooks);
        var blackRooks = pieceBitBoards[(int)Piece.r];
        Assert.AreEqual(0b1000_0001UL << (0 * 8), blackRooks);

        var whiteKnights = pieceBitBoards[(int)Piece.N];
        Assert.AreEqual(0b0100_0010UL << (7 * 8), whiteKnights);
        var blackKnights = pieceBitBoards[(int)Piece.n];
        Assert.AreEqual(0b0100_0010UL << (0 * 8), blackKnights);

        var whiteBishops = pieceBitBoards[(int)Piece.B];
        Assert.AreEqual(0b0010_0100UL << (7 * 8), whiteBishops);
        var blackBishops = pieceBitBoards[(int)Piece.b];
        Assert.AreEqual(0b0010_0100UL << (0 * 8), blackBishops);

        var whiteQueen = pieceBitBoards[(int)Piece.Q];
        Assert.AreEqual(0b0000_1000UL << (int)BoardSquare.a1, whiteQueen);
        var blackQueen = pieceBitBoards[(int)Piece.q];
        Assert.AreEqual(0b0000_1000UL << (0 * 8), blackQueen);

        var whiteKing = pieceBitBoards[(int)Piece.K];
        Assert.AreEqual(0b0001_0000UL << (7 * 8), whiteKing);
        var blackKing = pieceBitBoards[(int)Piece.k];
        Assert.AreEqual(0b0001_0000UL << (0 * 8), blackKing);
    }

    [Test]
    public void PieceBitBoards()
    {
        // Arrange
        const string fen = Constants.TrickyTestPositionFEN;

        // Act
        var result = FENParser.ParseFEN(fen);
        BitBoard[] pieceBitBoards = result.PieceBitBoards;

        // Assert

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

    [TestCase("8/8/8/8/8/8/8/8 w - - 0 1")]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 ")]
    [TestCase(Constants.TrickyTestPositionFEN)]
    [TestCase(Constants.KillerTestPositionFEN)]
    [TestCase(Constants.CmkTestPositionFEN)]
    public void OccupancyBitBoards(string fen)
    {
        // Arrange
        // Make sure a previous Fen doesn't change anything
        const string previuosFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        _ = FENParser.ParseFEN(previuosFen);

        // Act
        var result = FENParser.ParseFEN(fen);
        BitBoard[] pieceBitBoards = result.PieceBitBoards;
        BitBoard[] occupancyBitBoards = result.OccupancyBitBoards;

        // Assert

        var expectedWhiteOccupancy = 0UL;
        expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.P];
        expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.N];
        expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.B];
        expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.R];
        expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.Q];
        expectedWhiteOccupancy |= pieceBitBoards[(int)Piece.K];

        Assert.AreEqual(expectedWhiteOccupancy, occupancyBitBoards[(int)Side.White]);

        var expectedBlackOccupancy = 0UL;
        expectedBlackOccupancy |= pieceBitBoards[(int)Piece.p];
        expectedBlackOccupancy |= pieceBitBoards[(int)Piece.n];
        expectedBlackOccupancy |= pieceBitBoards[(int)Piece.b];
        expectedBlackOccupancy |= pieceBitBoards[(int)Piece.r];
        expectedBlackOccupancy |= pieceBitBoards[(int)Piece.q];
        expectedBlackOccupancy |= pieceBitBoards[(int)Piece.k];

        Assert.AreEqual(expectedBlackOccupancy, occupancyBitBoards[(int)Side.Black]);

        var expectedCombinedOccupancy = expectedWhiteOccupancy | expectedBlackOccupancy;

        Assert.AreEqual(expectedCombinedOccupancy, occupancyBitBoards[(int)Side.Both]);
    }

    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", Side.White)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR W KQkq - 0 1", Side.White)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", Side.Black)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR B KQkq - 0 1", Side.Black)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR A KQkq - 0 1", Side.Both)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR a KQkq - 0 1", Side.Both)]
    public void SideToMove(string fen, Side expectedSide)
    {
        if (expectedSide != Side.Both)
        {
            Assert.AreEqual(expectedSide, FENParser.ParseFEN(fen).Side);
        }
        else
        {
            Assert.Throws<Exception>(() => FENParser.ParseFEN(fen));
        }
    }

    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BK | (int)CR.BQ)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Qkq - 0 1", (int)CR.WQ | (int)CR.BK | (int)CR.BQ)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kkq - 0 1", (int)CR.WK | (int)CR.BK | (int)CR.BQ)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQq - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BQ)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQk - 0 1", (int)CR.WK | (int)CR.WQ | (int)CR.BK)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1", (int)CR.WK | (int)CR.WQ)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w kq - 0 1", (int)CR.BK | (int)CR.BQ)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Qq - 0 1", (int)CR.WQ | (int)CR.BQ)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kk - 0 1", (int)CR.WK | (int)CR.BK)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1", 0)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w A - 0 1", -1)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KA - 0 1", -1)]
    public void CastlingRights(string fen, int expectedCastleResult)
    {
        // Arrange
        // Make sure a previous Fen doesn't change anything
        const string previuosFen = "8/8/8/8/8/8/8/8 w KQkq - 0 1";
        Assert.DoesNotThrow(() => FENParser.ParseFEN(previuosFen));

        if (expectedCastleResult >= 0)
        {
            // Act
            var result = FENParser.ParseFEN(fen);

            //Assert
            Assert.AreEqual(expectedCastleResult, result.Castle);
        }
        else
        {
            // Act and Assert
            Assert.Throws<Exception>(() => FENParser.ParseFEN(fen));
        }
    }

    [TestCase("rnbqkbnr/pppp1ppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b Qq e3 0 1", BoardSquare.e3)]
    [TestCase("rnbqkbnr/ppppp1pp/8/8/4pP2/8/PPPPP1PP/RNBQKBNR b Qq f3 0 1", BoardSquare.f3)]
    [TestCase("rnbqkbnr/pp1ppppp/8/1Pp5/8/8/P1PPPPPP/RNBQKBNR w KQkq c6 0 1", BoardSquare.c6)]
    [TestCase("rnbqkbnr/ppp1pppp/8/2Pp4/8/8/PP1PPPPP/RNBQKBNR w KQkq d6 0 1", BoardSquare.d6)]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", BoardSquare.noSquare)]
    public void EnPassant(string fen, BoardSquare expectedEnPassantSquare)
    {
        var result = FENParser.ParseFEN(fen);

        Assert.AreEqual(expectedEnPassantSquare, result.EnPassant);
    }

    [TestCase("8/8/8/8/8/8/8/8 w KQkq e6 0 1")]
    [TestCase("8/8/8/8/8/8/8/8 w KQkq e7 0 1")]
    [TestCase("rnbqkbnr/pppp1ppp/8/8/3pP3/8/PPPP1PPP/RNBQKBNR b Qq a3 0 1")]  // e3 could be
    [TestCase("rnbqkbnr/ppppp1pp/8/8/4pP2/8/PPPPP1PP/RNBQKBNR b Qq b3 0 1")]  // f3 could be
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq i1 0 1")]
    [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq i3 0 1")]
    [TestCase("8/8/Q7/P7/1p6/8/8/8 w - b3 0 1", Description = "Wrong EnPassant square")]
    [TestCase("8/8/8/1P6/p7/q7/8/8 b - b6 0 1", Description = "Wrong EnPassant square")]
    public void EnPassant_Error(string fen)
    {
        Assert.Throws<LynxException>(() => FENParser.ParseFEN(fen));
    }

    [TestCase("8/8/8/8/8/8/8/8 w KQkq - 0 1", 0)]
    [TestCase("8/8/8/8/8/8/8/8 w KQkq - 1 1", 1)]
    [TestCase("8/8/8/8/8/8/8/8 w KQkq - 51 1", 51)]
    public void HalfMoveClock(string fen, int expectedHalfMoves)
    {
        var result = FENParser.ParseFEN(fen);

        Assert.AreEqual(expectedHalfMoves, result.HalfMoveClock);
    }

    //[TestCase("8/8/8/8/8/8/8/8 w KQkq - 0 1", 1)]
    //[TestCase("8/8/8/8/8/8/8/8 w KQkq - 0 50", 50)]
    //[TestCase("8/8/8/8/8/8/8/8 w KQkq - 0 100", 100)]
    //public void FullMoveCounter(string fen, int expectedFullMoveCounter)
    //{
    //    var result = FENParser.ParseFEN(fen);

    //    Assert.AreEqual(expectedFullMoveCounter, result.FullMoveCounter);
    //}
}