using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.Model;

public class BitBoardTest
{
    [Test]
    public void GetBit()
    {
        foreach (var square in Enum.GetValues<BoardSquare>())
        {
            var bitBoard = 1UL << (int)square;

            Assert.True(bitBoard.GetBit(square));
        }
    }

    [Test]
    public void SetBit()
    {
        foreach (var square in Enum.GetValues<BoardSquare>())
        {
            var bitBoard = new BitBoard();

            bitBoard.SetBit(square);
            Assert.True(bitBoard.GetBit(square));

            // Making sure that setting it again doesn't flipt it
            bitBoard.SetBit(square);
            Assert.True(bitBoard.GetBit(square));
        }
    }

    [Test]
    public void PopBit()
    {
        foreach (var square in Enum.GetValues<BoardSquare>())
        {
            var bitBoard = new BitBoard();
            bitBoard.SetBit(square);

            Assert.True(bitBoard.GetBit(square));

            bitBoard.PopBit(square);
            Assert.False(bitBoard.GetBit(square));

            // Making sure that popping it again doesn't flipt it
            bitBoard.PopBit(square);
            Assert.False(bitBoard.GetBit(square));
        }
    }

    [Test]
    public void Empty()
    {
        var bitBoard = new BitBoard();
        Assert.True(bitBoard.Empty());

        bitBoard.SetBit(BoardSquare.e4);
        Assert.False(bitBoard.Empty());
    }

    [Test]
    public void IsSinglePopulated()
    {
        var bitBoard = new BitBoard();
        Assert.False(bitBoard.IsSinglePopulated());

        bitBoard.SetBit(BoardSquare.e4);
        Assert.True(bitBoard.IsSinglePopulated());

        bitBoard.SetBit(BoardSquare.e5);
        Assert.False(bitBoard.IsSinglePopulated());
    }

    [Test]
    public void CountBits()
    {
        var bitBoard = new BitBoard();
        Assert.AreEqual(0, bitBoard.CountBits());

        bitBoard.SetBit(BoardSquare.e4);
        Assert.AreEqual(1, bitBoard.CountBits());

        bitBoard.SetBit(BoardSquare.e4);
        Assert.AreEqual(1, bitBoard.CountBits());

        bitBoard.SetBit(BoardSquare.d4);
        Assert.AreEqual(2, bitBoard.CountBits());

        bitBoard.PopBit(BoardSquare.d4);
        Assert.AreEqual(1, bitBoard.CountBits());
    }

    [Test]
    public void CountBits_ulong()
    {
        var bitBoard = new BitBoard();
        Assert.AreEqual(0, bitBoard.CountBits());

        bitBoard.SetBit(BoardSquare.e4);
        Assert.AreEqual(1, bitBoard.CountBits());

        bitBoard.SetBit(BoardSquare.e4);
        Assert.AreEqual(1, bitBoard.CountBits());

        bitBoard.SetBit(BoardSquare.d4);
        Assert.AreEqual(2, bitBoard.CountBits());

        bitBoard.PopBit(BoardSquare.d4);
        Assert.AreEqual(1, bitBoard.CountBits());
    }

    [Test]
    public void ResetLS1B()
    {
        // Arrange
        BitBoard bitBoard = BitBoardExtensions.Initialize(BoardSquare.d5, BoardSquare.e4);

        // Act
        bitBoard.ResetLS1B();

        // Assert
        Assert.True(bitBoard.GetBit(BoardSquare.e4));
        Assert.False(bitBoard.GetBit(BoardSquare.d5));
    }

    [Test]
    public void ResetLS1BNonSideEffect()
    {
        // Arrange
        BitBoard bitBoard = BitBoardExtensions.Initialize(BoardSquare.d5, BoardSquare.e4);

        // Act
        var result = bitBoard.WithoutLS1B();

        // Assert
        Assert.True(bitBoard.GetBit(BoardSquare.e4));
        Assert.True(bitBoard.GetBit(BoardSquare.d5));

        Assert.True(result.GetBit(BoardSquare.e4));
        Assert.False(result.GetBit(BoardSquare.d5));
    }

    [TestCase(new BoardSquare[] { }, -1, Ignore = "Not a real case, we always check if beforehand")]
    [TestCase(new BoardSquare[] { BoardSquare.e4 }, (int)BoardSquare.e4)]
    [TestCase(new BoardSquare[] { BoardSquare.a8 }, (int)BoardSquare.a8)]
    [TestCase(new BoardSquare[] { BoardSquare.h1 }, (int)BoardSquare.h1)]
    [TestCase(new BoardSquare[] { BoardSquare.a8, BoardSquare.h1 }, (int)BoardSquare.a8)]
    [TestCase(new BoardSquare[] { BoardSquare.d5, BoardSquare.e4 }, (int)BoardSquare.d5)]
    [TestCase(new BoardSquare[] { BoardSquare.e4, BoardSquare.f4 }, (int)BoardSquare.e4)]
    public void GetLS1BIndex(BoardSquare[] occupiedSquares, int expectedLS1B)
    {
        var bitboard = BitBoardExtensions.Initialize(occupiedSquares);
        Assert.AreEqual(expectedLS1B, bitboard.GetLS1BIndex());

        if (expectedLS1B != -1)
        {
            Assert.AreEqual(((BoardSquare)expectedLS1B).ToString(), Constants.Coordinates[expectedLS1B]);
        }
    }

    [Test]
    public void ShiftUp()
    {
        BitBoard bb = 0;
        bb.SetBit(BoardSquare.a8);
        bb.SetBit(BoardSquare.b8);
        bb.SetBit(BoardSquare.c8);
        bb.SetBit(BoardSquare.d8);
        bb.SetBit(BoardSquare.e8);
        bb.SetBit(BoardSquare.f8);
        bb.SetBit(BoardSquare.g8);
        bb.SetBit(BoardSquare.h8);

        Assert.Zero(bb.ShiftUp());

        BitBoard another = 0;
        another.SetBit(BoardSquare.a7);
        another.SetBit(BoardSquare.a7);
        another.SetBit(BoardSquare.b7);
        another.SetBit(BoardSquare.c7);
        another.SetBit(BoardSquare.d7);
        another.SetBit(BoardSquare.e7);
        another.SetBit(BoardSquare.f7);
        another.SetBit(BoardSquare.g7);
        another.SetBit(BoardSquare.h7);

        Assert.AreEqual(bb, another.ShiftUp());
        Assert.AreEqual(another, bb.ShiftDown());
    }

    [Test]
    public void ShiftDown()
    {
        BitBoard bb = 0;
        bb.SetBit(BoardSquare.a1);
        bb.SetBit(BoardSquare.b1);
        bb.SetBit(BoardSquare.c1);
        bb.SetBit(BoardSquare.d1);
        bb.SetBit(BoardSquare.e1);
        bb.SetBit(BoardSquare.f1);
        bb.SetBit(BoardSquare.g1);
        bb.SetBit(BoardSquare.h1);

        Assert.Zero(bb.ShiftDown());

        BitBoard another = 0;
        another.SetBit(BoardSquare.a2);
        another.SetBit(BoardSquare.a2);
        another.SetBit(BoardSquare.b2);
        another.SetBit(BoardSquare.c2);
        another.SetBit(BoardSquare.d2);
        another.SetBit(BoardSquare.e2);
        another.SetBit(BoardSquare.f2);
        another.SetBit(BoardSquare.g2);
        another.SetBit(BoardSquare.h2);

        Assert.AreEqual(bb, another.ShiftDown());
        Assert.AreEqual(another, bb.ShiftUp());
    }

    [Test]
    public void ShiftLeft()
    {
        BitBoard bb = 0;
        bb.SetBit(BoardSquare.a8);
        bb.SetBit(BoardSquare.a7);
        bb.SetBit(BoardSquare.a6);
        bb.SetBit(BoardSquare.a5);
        bb.SetBit(BoardSquare.a4);
        bb.SetBit(BoardSquare.a3);
        bb.SetBit(BoardSquare.a2);
        bb.SetBit(BoardSquare.a1);

        Assert.Zero(bb.ShiftLeft());

        BitBoard another = 0;
        another.SetBit(BoardSquare.b8);
        another.SetBit(BoardSquare.b7);
        another.SetBit(BoardSquare.b6);
        another.SetBit(BoardSquare.b5);
        another.SetBit(BoardSquare.b4);
        another.SetBit(BoardSquare.b3);
        another.SetBit(BoardSquare.b2);
        another.SetBit(BoardSquare.b1);

        Assert.AreEqual(bb, another.ShiftLeft());
        Assert.AreEqual(another, bb.ShiftRight());
    }

    [Test]
    public void ShiftRight()
    {
        BitBoard bb = 0;
        bb.SetBit(BoardSquare.h8);
        bb.SetBit(BoardSquare.h7);
        bb.SetBit(BoardSquare.h6);
        bb.SetBit(BoardSquare.h5);
        bb.SetBit(BoardSquare.h4);
        bb.SetBit(BoardSquare.h3);
        bb.SetBit(BoardSquare.h2);
        bb.SetBit(BoardSquare.h1);

        Assert.Zero(bb.ShiftRight());

        BitBoard another = 0;
        another.SetBit(BoardSquare.g8);
        another.SetBit(BoardSquare.g7);
        another.SetBit(BoardSquare.g6);
        another.SetBit(BoardSquare.g5);
        another.SetBit(BoardSquare.g4);
        another.SetBit(BoardSquare.g3);
        another.SetBit(BoardSquare.g2);
        another.SetBit(BoardSquare.g1);

        Assert.AreEqual(bb, another.ShiftRight());
        Assert.AreEqual(another, bb.ShiftLeft());
    }

    [Test]
    public void ShiftUpRight()
    {
        BitBoard defaultbb = 0;

        BitBoard bb = 0;
        bb.SetBit(BoardSquare.a1);

        Assert.AreEqual(defaultbb.SetBit(BoardSquare.b2), bb.ShiftUpRight());

        bb = 0;
        bb.SetBit(BoardSquare.h8);
        Assert.Zero(bb.ShiftUpRight());

        bb = 0;
        bb.SetBit(BoardSquare.a8);
        Assert.Zero(bb.ShiftUpRight());

        bb = 0;
        bb.SetBit(BoardSquare.h5);
        Assert.Zero(bb.ShiftUpRight());

        bb = 0;
        bb.SetBit(BoardSquare.h1);
        Assert.Zero(bb.ShiftUpRight());
    }

    [Test]
    public void ShiftUpLeft()
    {
        BitBoard defaultbb = 0;

        BitBoard bb = 0;
        bb.SetBit(BoardSquare.h1);

        Assert.AreEqual(defaultbb.SetBit(BoardSquare.g2), bb.ShiftUpLeft());

        bb = 0;
        bb.SetBit(BoardSquare.a8);
        Assert.Zero(bb.ShiftUpLeft());

        bb = 0;
        bb.SetBit(BoardSquare.h8);
        Assert.Zero(bb.ShiftUpLeft());

        bb = 0;
        bb.SetBit(BoardSquare.a5);
        Assert.Zero(bb.ShiftUpLeft());

        bb = 0;
        bb.SetBit(BoardSquare.a1);
        Assert.Zero(bb.ShiftUpLeft());
    }

    [Test]
    public void ShiftDownLeft()
    {
        BitBoard defaultbb = 0;

        BitBoard bb = 0;
        bb.SetBit(BoardSquare.h8);

        Assert.AreEqual(defaultbb.SetBit(BoardSquare.g7), bb.ShiftDownLeft());

        bb = 0;
        bb.SetBit(BoardSquare.a1);
        Assert.Zero(bb.ShiftDownLeft());

        bb = 0;
        bb.SetBit(BoardSquare.h1);
        Assert.Zero(bb.ShiftDownLeft());

        bb = 0;
        bb.SetBit(BoardSquare.a5);
        Assert.Zero(bb.ShiftDownLeft());

        bb = 0;
        bb.SetBit(BoardSquare.a8);
        Assert.Zero(bb.ShiftDownLeft());
    }

    [Test]
    public void ShiftDownRight()
    {
        BitBoard defaultbb = 0;

        BitBoard bb = 0;
        bb.SetBit(BoardSquare.a8);

        Assert.AreEqual(defaultbb.SetBit(BoardSquare.b7), bb.ShiftDownRight());

        bb = 0;
        bb.SetBit(BoardSquare.h1);
        Assert.Zero(bb.ShiftDownRight());

        bb = 0;
        bb.SetBit(BoardSquare.a1);
        Assert.Zero(bb.ShiftDownRight());

        bb = 0;
        bb.SetBit(BoardSquare.h5);
        Assert.Zero(bb.ShiftDownRight());

        bb = 0;
        bb.SetBit(BoardSquare.h8);
        Assert.Zero(bb.ShiftDownRight());
    }
}
