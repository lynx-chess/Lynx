using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

internal class MasksTest
{
    [TestCase(BoardSquare.a8, "P7/P7/P7/P7/P7/P7/P7/P7 w - - 0 1")]
    [TestCase(BoardSquare.a6, "P7/P7/P7/P7/P7/P7/P7/P7 w - - 0 1")]
    [TestCase(BoardSquare.b5, "1P6/1P6/1P6/1P6/1P6/1P6/1P6/1P6 w - - 0 1")]
    [TestCase(BoardSquare.b1, "1P6/1P6/1P6/1P6/1P6/1P6/1P6/1P6 w - - 0 1")]
    public void FileMasks(BoardSquare square, string fen)
    {
        var position = new Position(fen);

        var expectedBitBoard = position.PieceBitBoards[(int)Piece.P] | position.PieceBitBoards[(int)Piece.p];
        var actualBitBoard = Masks.FileMasks[(int)square];

        Assert.AreEqual(expectedBitBoard, actualBitBoard);
    }

    [TestCase(BoardSquare.a8, "PPPPPPPP/8/8/8/8/8/8/8 w - - 0 1")]
    [TestCase(BoardSquare.c8, "PPPPPPPP/8/8/8/8/8/8/8 w - - 0 1")]
    [TestCase(BoardSquare.b2, "8/8/8/8/8/8/PPPPPPPP/8 w - - 0 1")]
    [TestCase(BoardSquare.h2, "8/8/8/8/8/8/PPPPPPPP/8 w - - 0 1")]
    public void RankMasks(BoardSquare square, string fen)
    {
        var position = new Position(fen);

        var expectedBitBoard = position.PieceBitBoards[(int)Piece.P] | position.PieceBitBoards[(int)Piece.p];
        var actualBitBoard = Masks.RankMasks[(int)square];

        Assert.AreEqual(expectedBitBoard, actualBitBoard);
    }

    [TestCase(BoardSquare.a1, "1p6/1p6/1p6/1p6/1p6/1p6/1p6/1p6 w - - 0 1")]
    [TestCase(BoardSquare.a8, "1p6/1p6/1p6/1p6/1p6/1p6/1p6/1p6 w - - 0 1")]
    [TestCase(BoardSquare.a5, "1p6/1p6/1p6/1p6/1p6/1p6/1p6/1p6 w - - 0 1")]
    [TestCase(BoardSquare.b2, "p1p5/p1p5/p1p5/p1p5/p1p5/p1p5/p1p5/p1p5 w - - 0 1")]
    [TestCase(BoardSquare.b5, "p1p5/p1p5/p1p5/p1p5/p1p5/p1p5/p1p5/p1p5 w - - 0 1")]
    [TestCase(BoardSquare.b7, "p1p5/p1p5/p1p5/p1p5/p1p5/p1p5/p1p5/p1p5 w - - 0 1")]
    public void IsolatedPawnMasks(BoardSquare square, string fen)
    {
        var position = new Position(fen);

        var expectedBitBoard = position.PieceBitBoards[(int)Piece.P] | position.PieceBitBoards[(int)Piece.p];
        var actualBitBoard = Masks.IsolatedPawnMasks[(int)square];

        Assert.AreEqual(expectedBitBoard, actualBitBoard);
    }
    
    [TestCase(BoardSquare.e4, "3ppp2/3ppp2/3ppp2/3ppp2/8/8/8/8 w - - 0 1")]
    [TestCase(BoardSquare.e5, "3ppp2/3ppp2/3ppp2/8/8/8/8/8 w - - 0 1")]
    [TestCase(BoardSquare.e7, "3ppp2/8/8/8/8/8/8/8 w - - 0 1")] // TODO makes sense?
    [TestCase(BoardSquare.a7, "pp6/8/8/8/8/8/8/8 w - - 0 1")] // TODO makes sense?
    [TestCase(BoardSquare.a2, "pp6/pp6/pp6/pp6/pp6/pp6/8/8 w - - 0 1")]
    [TestCase(BoardSquare.h3, "6pp/6pp/6pp/6pp/6pp/8/8/8 w - - 0 1")]
    [TestCase(BoardSquare.h1, "6pp/6pp/6pp/6pp/6pp/6pp/6pp/8 w - - 0 1")] // TODO makes senes
    [TestCase(BoardSquare.h8, "8/8/8/8/8/8/8/8 w - - 0 1")] // TODO makes senes
    public void WhitePassedPawnMasks(BoardSquare square, string fen)
    {
        var position = new Position(fen);

        var expectedBitBoard = position.PieceBitBoards[(int)Piece.P] | position.PieceBitBoards[(int)Piece.p];
        var actualBitBoard = Masks.WhitePassedPawnMasks[(int)square];

        Assert.AreEqual(expectedBitBoard, actualBitBoard);
    }
    
    [TestCase(BoardSquare.e4, "8/8/8/8/8/3ppp2/3ppp2/3ppp2 w - - 0 1")]
    [TestCase(BoardSquare.e5, "8/8/8/8/3ppp2/3ppp2/3ppp2/3ppp2 w - - 0 1")]
    [TestCase(BoardSquare.e2, "8/8/8/8/8/8/8/3ppp2 w - - 0 1")] // TODO makes sense?
    [TestCase(BoardSquare.a7, "8/8/pp6/pp6/pp6/pp6/pp6/pp6 w - - 0 1")]
    [TestCase(BoardSquare.a2, "8/8/8/8/8/8/8/pp6 w - - 0 1")] // TODO makes sense?
    [TestCase(BoardSquare.h3, "8/8/8/8/8/8/6pp/6pp w - - 0 1")]
    [TestCase(BoardSquare.h8, "8/6pp/6pp/6pp/6pp/6pp/6pp/6pp w - - 0 1")] // TODO makes senes
    [TestCase(BoardSquare.h1, "8/8/8/8/8/8/8/8 w - - 0 1")] // TODO makes senes
    public void BlackPassedPawnMasks(BoardSquare square, string fen)
    {
        var position = new Position(fen);

        var expectedBitBoard = position.PieceBitBoards[(int)Piece.P] | position.PieceBitBoards[(int)Piece.p];
        var actualBitBoard = Masks.BlackPassedPawnMasks[(int)square];

        Assert.AreEqual(expectedBitBoard, actualBitBoard);
    }
}
