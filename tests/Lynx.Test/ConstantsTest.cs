using Lynx.Model;
using NUnit.Framework;
using System.Buffers;
using System.Collections.Frozen;
using static Lynx.Constants;

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
                Assert.AreEqual(value, PiecesByChar[value.ToString()[0]]);
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
            Assert.AreEqual(_enPassantCaptureSquaresDictionary[square], Constants.EnPassantCaptureSquares[square]);
        }

        Assert.AreEqual((int)BoardSquare.d4, Constants.EnPassantCaptureSquares[(int)BoardSquare.d3]);
        for (int square = (int)BoardSquare.a3; square <= (int)BoardSquare.h3; ++square)
        {
            Assert.AreEqual(square - 8, Constants.EnPassantCaptureSquares[square]);
            Assert.AreEqual(_enPassantCaptureSquaresDictionary[square], Constants.EnPassantCaptureSquares[square]);
        }
    }

    private static readonly FrozenDictionary<int, int> _enPassantCaptureSquaresDictionary = new Dictionary<int, int>(16)
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

    [Test]
    public void HFile()
    {
        Bitboard bb = 0;
        bb.SetBit(BoardSquare.h1);

        Assert.NotZero(bb & Constants.HFile);
        Assert.Zero(bb & NotHFile);
        Assert.Zero(bb & Constants.AFile);

        Assert.AreEqual(NotHFile, ~Constants.HFile);
    }

    [Test]
    public void AFile()
    {
        Bitboard bb = 0;
        bb.SetBit(BoardSquare.a1);

        Assert.NotZero(bb & Constants.AFile);
        Assert.Zero(bb & NotAFile);
        Assert.Zero(bb & Constants.HFile);

        Assert.AreEqual(NotAFile, ~Constants.AFile);
    }

    [Test]
    public void DarkLightSquares()
    {
        for (int i = 0; i < 64; ++i)
        {
            Assert.True(DarkSquaresBitboard.GetBit(i) ^ LightSquaresBitboard.GetBit(i));
        }

        Assert.True(DarkSquaresBitboard.GetBit((int)BoardSquare.a1));
        Assert.True(DarkSquaresBitboard.GetBit((int)BoardSquare.h8));
        Assert.False(DarkSquaresBitboard.GetBit((int)BoardSquare.a8));
        Assert.False(DarkSquaresBitboard.GetBit((int)BoardSquare.h1));

        Assert.AreEqual(1, DarkSquares[(int)BoardSquare.a1]);
        Assert.AreEqual(1, DarkSquares[(int)BoardSquare.h8]);
        Assert.AreEqual(0, DarkSquares[(int)BoardSquare.a8]);
        Assert.AreEqual(0, DarkSquares[(int)BoardSquare.h1]);

        Assert.True(LightSquaresBitboard.GetBit((int)BoardSquare.a8));
        Assert.True(LightSquaresBitboard.GetBit((int)BoardSquare.h1));
        Assert.False(LightSquaresBitboard.GetBit((int)BoardSquare.a1));
        Assert.False(LightSquaresBitboard.GetBit((int)BoardSquare.h8));

        Assert.AreEqual(1, LightSquares[(int)BoardSquare.a8]);
        Assert.AreEqual(1, LightSquares[(int)BoardSquare.h1]);
        Assert.AreEqual(0, LightSquares[(int)BoardSquare.a1]);
        Assert.AreEqual(0, LightSquares[(int)BoardSquare.h8]);
    }

    [Test]
    public void Rank()
    {
        for (int sq = 0; sq < 64; ++sq)
        {
            Assert.AreEqual((sq >> 3) + 1, 8 - Constants.Rank[sq]);
        }
    }

    [Test]
    public void MaxNumberMovesInAGame()
    {
        Assert.Less(Constants.MaxNumberMovesInAGame, 1024 * 1024, "We'd need to customize ArrayPool due to desired array size requirements");
    }

    [Test]
    public void PowerOfTwo()
    {
        Assert.True(int.IsPow2(KingPawnHashSize));
        Assert.True(int.IsPow2(PawnCorrHistoryHashSize));
        Assert.True(int.IsPow2(NonPawnCorrHistoryHashSize));
    }

    [Test]
    public void ManhattanDistance()
    {
        Assert.AreEqual(0, Constants.ManhattanDistance[0][0]);
        Assert.AreEqual(0, Constants.ManhattanDistance[63][63]);

        Assert.AreEqual(7, Constants.ManhattanDistance[0][7]);
        Assert.AreEqual(7, Constants.ManhattanDistance[7][0]);

        Assert.AreEqual(14, Constants.ManhattanDistance[0][63]);
        Assert.AreEqual(14, Constants.ManhattanDistance[63][0]);

        Assert.AreEqual(1, Constants.ManhattanDistance[27][28]);
        Assert.AreEqual(1, Constants.ManhattanDistance[28][27]);

        Assert.AreEqual(1, Constants.ManhattanDistance[27][35]);
        Assert.AreEqual(1, Constants.ManhattanDistance[35][27]);

        Assert.AreEqual(2, Constants.ManhattanDistance[27][36]);
        Assert.AreEqual(2, Constants.ManhattanDistance[36][27]);

        Assert.AreEqual(4, Constants.ManhattanDistance[27][45]);
        Assert.AreEqual(4, Constants.ManhattanDistance[45][27]);

        Assert.AreEqual(6, Constants.ManhattanDistance[27][54]);
        Assert.AreEqual(6, Constants.ManhattanDistance[54][27]);

        Assert.AreEqual(2, Constants.ManhattanDistance[(int)BoardSquare.a8][(int)BoardSquare.b7]);
        Assert.AreEqual(4, Constants.ManhattanDistance[(int)BoardSquare.a8][(int)BoardSquare.c6]);
        Assert.AreEqual(3, Constants.ManhattanDistance[(int)BoardSquare.a8][(int)BoardSquare.b6]);
    }

    /// <summary>
    /// Ensure <see cref="MultiArrayTranspositionTable.HashfullPermillApprox"/> works as expected
    /// </summary>
    [Test]
    public void MaxTTArrayLength()
    {
        Assert.GreaterOrEqual(Constants.MaxTTArrayLength, 1000);
    }
}
