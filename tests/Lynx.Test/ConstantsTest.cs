using Lynx.Model;
using NUnit.Framework;
using System.Collections.Frozen;

namespace Lynx.Test;

public class ConstantsTest
{
    [Test]
    public void BishopRelevantOccupancyBits()
    {
        for (int square = 0; square < 64; ++square)
        {
            var mask = AttackGenerator.MaskBishopOccupancy(square);

            Assert.AreEqual(Constants.BishopRelevantOccupancyBits[square], mask.CountBits());
        }
    }

    [Test]
    public void RookRelevantOccupancyBits()
    {
        for (int square = 0; square < 64; ++square)
        {
            var mask = AttackGenerator.MaskRookOccupancy(square);

            Assert.AreEqual(Constants.RookRelevantOccupancyBits[square], mask.CountBits());
        }
    }

    [Test]
    public void AsciiPieces()
    {
        foreach (var value in Enum.GetValues<Piece>())
        {
            if (value > 0 && value != Piece.None)
            {
                Assert.AreEqual(value.ToString(), Constants.AsciiPieces[(int)value].ToString());
            }
        }
    }

    [Test]
    public void PiecesByString()
    {
        foreach (var value in Enum.GetValues<Piece>())
        {
            if (value > 0 && value != Piece.None)
            {
                Assert.AreEqual(value, Constants.PiecesByChar[value.ToString()[0]]);
            }
        }
    }

    [Test]
    public void EnPassantCaptureSquares()
    {
        Assert.AreEqual((int)BoardSquare.e5, Constants.EnPassantCaptureSquares[(int)BoardSquare.e6]);
        for (int square = (int)BoardSquare.a6; square <= (int)BoardSquare.h6; ++square)
        {
            Assert.AreEqual(square + 8, Constants.EnPassantCaptureSquares[square]);
            Assert.AreEqual(EnPassantCaptureSquaresDictionary[square], Constants.EnPassantCaptureSquares[square]);
        }

        Assert.AreEqual((int)BoardSquare.d4, Constants.EnPassantCaptureSquares[(int)BoardSquare.d3]);
        for (int square = (int)BoardSquare.a3; square <= (int)BoardSquare.h3; ++square)
        {
            Assert.AreEqual(square - 8, Constants.EnPassantCaptureSquares[square]);
            Assert.AreEqual(EnPassantCaptureSquaresDictionary[square], Constants.EnPassantCaptureSquares[square]);
        }
    }

    private static readonly FrozenDictionary<int, int> EnPassantCaptureSquaresDictionary = new Dictionary<int, int>(16)
    {
        [(int)BoardSquare.a6] = (int)BoardSquare.a6 + 8,
        [(int)BoardSquare.b6] = (int)BoardSquare.b6 + 8,
        [(int)BoardSquare.c6] = (int)BoardSquare.c6 + 8,
        [(int)BoardSquare.d6] = (int)BoardSquare.d6 + 8,
        [(int)BoardSquare.e6] = (int)BoardSquare.e6 + 8,
        [(int)BoardSquare.f6] = (int)BoardSquare.f6 + 8,
        [(int)BoardSquare.g6] = (int)BoardSquare.g6 + 8,
        [(int)BoardSquare.h6] = (int)BoardSquare.h6 + 8,

        [(int)BoardSquare.a3] = (int)BoardSquare.a3 - 8,
        [(int)BoardSquare.b3] = (int)BoardSquare.b3 - 8,
        [(int)BoardSquare.c3] = (int)BoardSquare.c3 - 8,
        [(int)BoardSquare.d3] = (int)BoardSquare.d3 - 8,
        [(int)BoardSquare.e3] = (int)BoardSquare.e3 - 8,
        [(int)BoardSquare.f3] = (int)BoardSquare.f3 - 8,
        [(int)BoardSquare.g3] = (int)BoardSquare.g3 - 8,
        [(int)BoardSquare.h3] = (int)BoardSquare.h3 - 8,
    }.ToFrozenDictionary();
}
