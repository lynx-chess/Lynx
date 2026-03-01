using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.Model;

public class BitboardTest
{
    [Test]
    public void GetBit()
    {
        foreach (var square in Enum.GetValues<BoardSquare>())
        {
            var bitboard = 1UL << (int)square;

            Assert.True(bitboard.GetBit(square));
        }
    }

    [Test]
    public void SetBit()
    {
        foreach (var square in Enum.GetValues<BoardSquare>())
        {
            var bitboard = new Bitboard();

            bitboard.SetBit(square);
            Assert.True(bitboard.GetBit(square));

            // Making sure that setting it again doesn't flipt it
            bitboard.SetBit(square);
            Assert.True(bitboard.GetBit(square));
        }
    }

    [Test]
    public void PopBit()
    {
        foreach (var square in Enum.GetValues<BoardSquare>())
        {
            var bitboard = new Bitboard();
            bitboard.SetBit(square);

            Assert.True(bitboard.GetBit(square));

            bitboard.PopBit(square);
            Assert.False(bitboard.GetBit(square));

            // Making sure that popping it again doesn't flipt it
            bitboard.PopBit(square);
            Assert.False(bitboard.GetBit(square));
        }
    }

    [Test]
    public void Empty()
    {
        var bitboard = new Bitboard();
        Assert.True(bitboard.Empty());

        bitboard.SetBit(BoardSquare.e4);
        Assert.False(bitboard.Empty());
    }

    [Test]
    public void IsSinglePopulated()
    {
        var bitboard = new Bitboard();
        Assert.False(bitboard.IsSinglePopulated());

        bitboard.SetBit(BoardSquare.e4);
        Assert.True(bitboard.IsSinglePopulated());

        bitboard.SetBit(BoardSquare.e5);
        Assert.False(bitboard.IsSinglePopulated());
    }

    [Test]
    public void CountBits()
    {
        var bitboard = new Bitboard();
        Assert.AreEqual(0, bitboard.CountBits());

        bitboard.SetBit(BoardSquare.e4);
        Assert.AreEqual(1, bitboard.CountBits());

        bitboard.SetBit(BoardSquare.e4);
        Assert.AreEqual(1, bitboard.CountBits());

        bitboard.SetBit(BoardSquare.d4);
        Assert.AreEqual(2, bitboard.CountBits());

        bitboard.PopBit(BoardSquare.d4);
        Assert.AreEqual(1, bitboard.CountBits());
    }

    [Test]
    public void ResetLS1B()
    {
        // Arrange
        Bitboard bitboard = BitboardExtensions.Initialize(BoardSquare.d5, BoardSquare.e4);

        // Act
        bitboard.ResetLS1B();

        // Assert
        Assert.True(bitboard.GetBit(BoardSquare.e4));
        Assert.False(bitboard.GetBit(BoardSquare.d5));
    }

    [Test]
    public void ResetLS1BNonSideEffect()
    {
        // Arrange
        Bitboard bitboard = BitboardExtensions.Initialize(BoardSquare.d5, BoardSquare.e4);

        // Act
        var result = bitboard.WithoutLS1B();

        // Assert
        Assert.True(bitboard.GetBit(BoardSquare.e4));
        Assert.True(bitboard.GetBit(BoardSquare.d5));

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
        var bitboard = BitboardExtensions.Initialize(occupiedSquares);
        Assert.AreEqual(expectedLS1B, bitboard.GetLS1BIndex());

        if (expectedLS1B != -1)
        {
            Assert.AreEqual(((BoardSquare)expectedLS1B).ToString(), Constants.Coordinates[expectedLS1B]);
        }
    }

    [Test]
    public void ShiftUp()
    {
        Bitboard bb = 0;
        bb.SetBit(BoardSquare.a8);
        bb.SetBit(BoardSquare.b8);
        bb.SetBit(BoardSquare.c8);
        bb.SetBit(BoardSquare.d8);
        bb.SetBit(BoardSquare.e8);
        bb.SetBit(BoardSquare.f8);
        bb.SetBit(BoardSquare.g8);
        bb.SetBit(BoardSquare.h8);

        Assert.Zero(bb.ShiftUp());

        Bitboard another = 0;
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
        Bitboard bb = 0;
        bb.SetBit(BoardSquare.a1);
        bb.SetBit(BoardSquare.b1);
        bb.SetBit(BoardSquare.c1);
        bb.SetBit(BoardSquare.d1);
        bb.SetBit(BoardSquare.e1);
        bb.SetBit(BoardSquare.f1);
        bb.SetBit(BoardSquare.g1);
        bb.SetBit(BoardSquare.h1);

        Assert.Zero(bb.ShiftDown());

        Bitboard another = 0;
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
        Bitboard bb = 0;
        bb.SetBit(BoardSquare.a8);
        bb.SetBit(BoardSquare.a7);
        bb.SetBit(BoardSquare.a6);
        bb.SetBit(BoardSquare.a5);
        bb.SetBit(BoardSquare.a4);
        bb.SetBit(BoardSquare.a3);
        bb.SetBit(BoardSquare.a2);
        bb.SetBit(BoardSquare.a1);

        Assert.Zero(bb.ShiftLeft());

        Bitboard another = 0;
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
        Bitboard bb = 0;
        bb.SetBit(BoardSquare.h8);
        bb.SetBit(BoardSquare.h7);
        bb.SetBit(BoardSquare.h6);
        bb.SetBit(BoardSquare.h5);
        bb.SetBit(BoardSquare.h4);
        bb.SetBit(BoardSquare.h3);
        bb.SetBit(BoardSquare.h2);
        bb.SetBit(BoardSquare.h1);

        Assert.Zero(bb.ShiftRight());

        Bitboard another = 0;
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
        Bitboard defaultbb = 0;

        Bitboard bb = 0;
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
        Bitboard defaultbb = 0;

        Bitboard bb = 0;
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
        Bitboard defaultbb = 0;

        Bitboard bb = 0;
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
        Bitboard defaultbb = 0;

        Bitboard bb = 0;
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

    [TestCase(BoardSquare.a1, BoardSquare.h1)]
    [TestCase(BoardSquare.h1, BoardSquare.a1)]
    [TestCase(BoardSquare.a4, BoardSquare.a4)]
    public void MaskBetweenTwoSquaresSameRankInclusive(BoardSquare start, BoardSquare end)
    {
        var result = BitboardExtensions.MaskBetweenTwoSquaresSameRankInclusive((int)start, (int)end);

        Assert.True(result.GetBit((int)start));
        Assert.True(result.GetBit((int)end));

        Assert.AreEqual(Math.Abs(end - start) + 1, result.CountBits());
    }

    [TestCase(BoardSquare.a1, BoardSquare.h1)]
    [TestCase(BoardSquare.h1, BoardSquare.a1)]
    [TestCase(BoardSquare.a4, BoardSquare.a4)]
    public void MaskBetweenTwoSquaresSameRankExclusive(BoardSquare start, BoardSquare end)
    {
        var result = BitboardExtensions.MaskBetweenTwoSquaresSameRankExclusive((int)start, (int)end);

        Assert.False(result.GetBit((int)start));
        Assert.False(result.GetBit((int)end));

        Assert.AreEqual(Math.Max(0, Math.Abs(end - start) - 1), result.CountBits());
    }
}
