using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.Model;
public class BoardSquareTest : BaseTest
{
    [TestCase(BoardSquare.a1, BoardSquare.a3)]
    [TestCase(BoardSquare.a1, BoardSquare.a5)]
    [TestCase(BoardSquare.a1, BoardSquare.a7)]
    [TestCase(BoardSquare.a1, BoardSquare.c1)]
    [TestCase(BoardSquare.a1, BoardSquare.e1)]
    [TestCase(BoardSquare.a1, BoardSquare.g1)]
    [TestCase(BoardSquare.a1, BoardSquare.b2)]
    [TestCase(BoardSquare.a1, BoardSquare.c3)]
    [TestCase(BoardSquare.a1, BoardSquare.d4)]
    [TestCase(BoardSquare.a1, BoardSquare.e5)]
    [TestCase(BoardSquare.a1, BoardSquare.f6)]
    [TestCase(BoardSquare.a1, BoardSquare.g7)]
    [TestCase(BoardSquare.a1, BoardSquare.h8)]
    public void SameColor(BoardSquare square1, BoardSquare square2)
    {
        Assert.True(BoardSquareExtensions.SameColor((int)square1, (int)square2));
        Assert.False(BoardSquareExtensions.DifferentColor((int)square1, (int)square2));
    }

    [TestCase(BoardSquare.a1, BoardSquare.a2)]
    [TestCase(BoardSquare.a1, BoardSquare.a4)]
    [TestCase(BoardSquare.a1, BoardSquare.a6)]
    [TestCase(BoardSquare.a1, BoardSquare.a8)]
    [TestCase(BoardSquare.a1, BoardSquare.b1)]
    [TestCase(BoardSquare.a1, BoardSquare.d1)]
    [TestCase(BoardSquare.a1, BoardSquare.f1)]
    [TestCase(BoardSquare.a1, BoardSquare.h1)]
    [TestCase(BoardSquare.a1, BoardSquare.b3)]
    [TestCase(BoardSquare.a1, BoardSquare.c4)]
    [TestCase(BoardSquare.a1, BoardSquare.d5)]
    [TestCase(BoardSquare.a1, BoardSquare.e6)]
    [TestCase(BoardSquare.a1, BoardSquare.f7)]
    [TestCase(BoardSquare.a1, BoardSquare.g8)]
    [TestCase(BoardSquare.a1, BoardSquare.h7)]
    public void DifferentColor(BoardSquare square1, BoardSquare square2)
    {
        Assert.True(BoardSquareExtensions.DifferentColor((int)square1, (int)square2));
        Assert.False(BoardSquareExtensions.SameColor((int)square1, (int)square2));
    }
}
