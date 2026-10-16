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

            Assert.True(bitboard.GetBit((int)square));
        }
    }

    [Test]
    public void SetBit()
    {
        foreach (var square in Enum.GetValues<BoardSquare>())
        {
            var bitboard = new Bitboard();

            bitboard.SetBit((int)square);
            Assert.True(bitboard.GetBit((int)square));

            // Making sure that setting it again doesn't flipt it
            bitboard.SetBit((int)square);
            Assert.True(bitboard.GetBit((int)square));
        }
    }

    [Test]
    public void PopBit()
    {
        foreach (var square in Enum.GetValues<BoardSquare>())
        {
            var bitboard = new Bitboard();
            bitboard.SetBit((int)square);

            Assert.True(bitboard.GetBit((int)square));

            bitboard.PopBit((int)square);
            Assert.False(bitboard.GetBit((int)square));

            // Making sure that popping it again doesn't flipt it
            bitboard.PopBit((int)square);
            Assert.False(bitboard.GetBit((int)square));
        }
    }

    [Test]
    public void Empty()
    {
        var bitboard = new Bitboard();
        Assert.True(bitboard.Empty());

        bitboard.SetBit((int)BoardSquare.e4);
        Assert.False(bitboard.Empty());
    }

    [Test]
    public void IsSinglePopulated()
    {
        var bitboard = new Bitboard();
        Assert.False(bitboard.IsSinglePopulated());

        bitboard.SetBit((int)BoardSquare.e4);
        Assert.True(bitboard.IsSinglePopulated());

        bitboard.SetBit((int)BoardSquare.e5);
        Assert.False(bitboard.IsSinglePopulated());
    }

    [Test]
    public void CountBits()
    {
        var bitboard = new Bitboard();
        Assert.AreEqual(0, bitboard.CountBits());

        bitboard.SetBit((int)BoardSquare.e4);
        Assert.AreEqual(1, bitboard.CountBits());

        bitboard.SetBit((int)BoardSquare.e4);
        Assert.AreEqual(1, bitboard.CountBits());

        bitboard.SetBit((int)BoardSquare.d4);
        Assert.AreEqual(2, bitboard.CountBits());

        bitboard.PopBit((int)BoardSquare.d4);
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
        Assert.True(bitboard.GetBit((int)BoardSquare.e4));
        Assert.False(bitboard.GetBit((int)BoardSquare.d5));
    }

    [Test]
    public void ResetLS1BNonSideEffect()
    {
        // Arrange
        Bitboard bitboard = BitboardExtensions.Initialize(BoardSquare.d5, BoardSquare.e4);

        // Act
        var result = bitboard.WithoutLS1B();

        // Assert
        Assert.True(bitboard.GetBit((int)BoardSquare.e4));
        Assert.True(bitboard.GetBit((int)BoardSquare.d5));

        Assert.True(result.GetBit((int)BoardSquare.e4));
        Assert.False(result.GetBit((int)BoardSquare.d5));
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
        bb.SetBit((int)BoardSquare.a8);
        bb.SetBit((int)BoardSquare.b8);
        bb.SetBit((int)BoardSquare.c8);
        bb.SetBit((int)BoardSquare.d8);
        bb.SetBit((int)BoardSquare.e8);
        bb.SetBit((int)BoardSquare.f8);
        bb.SetBit((int)BoardSquare.g8);
        bb.SetBit((int)BoardSquare.h8);

        Assert.Zero(bb.ShiftUp());

        Bitboard another = 0;
        another.SetBit((int)BoardSquare.a7);
        another.SetBit((int)BoardSquare.a7);
        another.SetBit((int)BoardSquare.b7);
        another.SetBit((int)BoardSquare.c7);
        another.SetBit((int)BoardSquare.d7);
        another.SetBit((int)BoardSquare.e7);
        another.SetBit((int)BoardSquare.f7);
        another.SetBit((int)BoardSquare.g7);
        another.SetBit((int)BoardSquare.h7);

        Assert.AreEqual(bb, another.ShiftUp());
        Assert.AreEqual(another, bb.ShiftDown());
    }

    [Test]
    public void ShiftDown()
    {
        Bitboard bb = 0;
        bb.SetBit((int)BoardSquare.a1);
        bb.SetBit((int)BoardSquare.b1);
        bb.SetBit((int)BoardSquare.c1);
        bb.SetBit((int)BoardSquare.d1);
        bb.SetBit((int)BoardSquare.e1);
        bb.SetBit((int)BoardSquare.f1);
        bb.SetBit((int)BoardSquare.g1);
        bb.SetBit((int)BoardSquare.h1);

        Assert.Zero(bb.ShiftDown());

        Bitboard another = 0;
        another.SetBit((int)BoardSquare.a2);
        another.SetBit((int)BoardSquare.a2);
        another.SetBit((int)BoardSquare.b2);
        another.SetBit((int)BoardSquare.c2);
        another.SetBit((int)BoardSquare.d2);
        another.SetBit((int)BoardSquare.e2);
        another.SetBit((int)BoardSquare.f2);
        another.SetBit((int)BoardSquare.g2);
        another.SetBit((int)BoardSquare.h2);

        Assert.AreEqual(bb, another.ShiftDown());
        Assert.AreEqual(another, bb.ShiftUp());
    }

    [Test]
    public void ShiftLeft()
    {
        Bitboard bb = 0;
        bb.SetBit((int)BoardSquare.a8);
        bb.SetBit((int)BoardSquare.a7);
        bb.SetBit((int)BoardSquare.a6);
        bb.SetBit((int)BoardSquare.a5);
        bb.SetBit((int)BoardSquare.a4);
        bb.SetBit((int)BoardSquare.a3);
        bb.SetBit((int)BoardSquare.a2);
        bb.SetBit((int)BoardSquare.a1);

        Assert.Zero(bb.ShiftLeft());

        Bitboard another = 0;
        another.SetBit((int)BoardSquare.b8);
        another.SetBit((int)BoardSquare.b7);
        another.SetBit((int)BoardSquare.b6);
        another.SetBit((int)BoardSquare.b5);
        another.SetBit((int)BoardSquare.b4);
        another.SetBit((int)BoardSquare.b3);
        another.SetBit((int)BoardSquare.b2);
        another.SetBit((int)BoardSquare.b1);

        Assert.AreEqual(bb, another.ShiftLeft());
        Assert.AreEqual(another, bb.ShiftRight());
    }

    [Test]
    public void ShiftRight()
    {
        Bitboard bb = 0;
        bb.SetBit((int)BoardSquare.h8);
        bb.SetBit((int)BoardSquare.h7);
        bb.SetBit((int)BoardSquare.h6);
        bb.SetBit((int)BoardSquare.h5);
        bb.SetBit((int)BoardSquare.h4);
        bb.SetBit((int)BoardSquare.h3);
        bb.SetBit((int)BoardSquare.h2);
        bb.SetBit((int)BoardSquare.h1);

        Assert.Zero(bb.ShiftRight());

        Bitboard another = 0;
        another.SetBit((int)BoardSquare.g8);
        another.SetBit((int)BoardSquare.g7);
        another.SetBit((int)BoardSquare.g6);
        another.SetBit((int)BoardSquare.g5);
        another.SetBit((int)BoardSquare.g4);
        another.SetBit((int)BoardSquare.g3);
        another.SetBit((int)BoardSquare.g2);
        another.SetBit((int)BoardSquare.g1);

        Assert.AreEqual(bb, another.ShiftRight());
        Assert.AreEqual(another, bb.ShiftLeft());
    }

    [Test]
    public void ShiftUpRight()
    {
        Bitboard defaultbb = 0;

        Bitboard bb = 0;
        bb.SetBit((int)BoardSquare.a1);

        Assert.AreEqual(defaultbb.SetBit((int)BoardSquare.b2), bb.ShiftUpRight());

        bb = 0;
        bb.SetBit((int)BoardSquare.h8);
        Assert.Zero(bb.ShiftUpRight());

        bb = 0;
        bb.SetBit((int)BoardSquare.a8);
        Assert.Zero(bb.ShiftUpRight());

        bb = 0;
        bb.SetBit((int)BoardSquare.h5);
        Assert.Zero(bb.ShiftUpRight());

        bb = 0;
        bb.SetBit((int)BoardSquare.h1);
        Assert.Zero(bb.ShiftUpRight());
    }

    [Test]
    public void ShiftUpLeft()
    {
        Bitboard defaultbb = 0;

        Bitboard bb = 0;
        bb.SetBit((int)BoardSquare.h1);

        Assert.AreEqual(defaultbb.SetBit((int)BoardSquare.g2), bb.ShiftUpLeft());

        bb = 0;
        bb.SetBit((int)BoardSquare.a8);
        Assert.Zero(bb.ShiftUpLeft());

        bb = 0;
        bb.SetBit((int)BoardSquare.h8);
        Assert.Zero(bb.ShiftUpLeft());

        bb = 0;
        bb.SetBit((int)BoardSquare.a5);
        Assert.Zero(bb.ShiftUpLeft());

        bb = 0;
        bb.SetBit((int)BoardSquare.a1);
        Assert.Zero(bb.ShiftUpLeft());
    }

    [Test]
    public void ShiftDownLeft()
    {
        Bitboard defaultbb = 0;

        Bitboard bb = 0;
        bb.SetBit((int)BoardSquare.h8);

        Assert.AreEqual(defaultbb.SetBit((int)BoardSquare.g7), bb.ShiftDownLeft());

        bb = 0;
        bb.SetBit((int)BoardSquare.a1);
        Assert.Zero(bb.ShiftDownLeft());

        bb = 0;
        bb.SetBit((int)BoardSquare.h1);
        Assert.Zero(bb.ShiftDownLeft());

        bb = 0;
        bb.SetBit((int)BoardSquare.a5);
        Assert.Zero(bb.ShiftDownLeft());

        bb = 0;
        bb.SetBit((int)BoardSquare.a8);
        Assert.Zero(bb.ShiftDownLeft());
    }

    [Test]
    public void ShiftDownRight()
    {
        Bitboard defaultbb = 0;

        Bitboard bb = 0;
        bb.SetBit((int)BoardSquare.a8);

        Assert.AreEqual(defaultbb.SetBit((int)BoardSquare.b7), bb.ShiftDownRight());

        bb = 0;
        bb.SetBit((int)BoardSquare.h1);
        Assert.Zero(bb.ShiftDownRight());

        bb = 0;
        bb.SetBit((int)BoardSquare.a1);
        Assert.Zero(bb.ShiftDownRight());

        bb = 0;
        bb.SetBit((int)BoardSquare.h5);
        Assert.Zero(bb.ShiftDownRight());

        bb = 0;
        bb.SetBit((int)BoardSquare.h8);
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
