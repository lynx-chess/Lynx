using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;
public class BitBoardExtensionsTest
{
    [TestCase(0b10001UL, 1UL)]
    [TestCase(0b11001UL, 1UL)]
    [TestCase(0b11101UL, 1UL)]
    [TestCase(0b11111UL, 1UL)]
    [TestCase(0b10111UL, 1UL)]
    [TestCase(0b10101UL, 1UL)]
    [TestCase(0b01001UL, 1UL)]
    [TestCase(0b01101UL, 1UL)]
    [TestCase(0b01111UL, 1UL)]
    [TestCase(0b00011UL, 1UL)]
    [TestCase(0b00001UL, 1UL)]

    [TestCase(0b10000UL, 0b10000UL)]
    [TestCase(0b11000UL, 0b1000UL)]
    [TestCase(0b11100UL, 0b100UL)]
    [TestCase(0b11110UL, 0b10UL)]
    [TestCase(0b10110UL, 0b10UL)]
    [TestCase(0b10100UL, 0b100UL)]
    [TestCase(0b01000UL, 0b1000UL)]
    [TestCase(0b01100UL, 0b100UL)]
    [TestCase(0b01110UL, 0b10UL)]
    [TestCase(0b00101UL, 0b1UL)]
    [TestCase(0b00010UL, 0b10UL)]
    [TestCase(0b00001UL, 0b1UL)]
    public void LSB(BitBoard n, ulong lsb)
    {
        Assert.AreEqual(lsb, n.LSB());

        Assert.AreEqual(lsb, lsb & (~lsb + 1));

        if (System.Runtime.Intrinsics.X86.Bmi1.IsSupported)
        {
            Assert.AreEqual(lsb, System.Runtime.Intrinsics.X86.Bmi1.X64.ExtractLowestSetBit(n));
        }
    }

    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.a2, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.b2, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.c2, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.d2, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.e2, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.f2, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.g2, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.h2, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.a7, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.b7, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.c7, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.d7, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.e7, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.f7, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.g7, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.P, BoardSquare.h7, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.a7, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.b7, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.c7, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.d7, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.e7, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.f7, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.g7, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.h7, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.a2, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.b2, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.c2, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.d2, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.e2, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.f2, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.g2, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.p, BoardSquare.h2, false)]

    [TestCase(Constants.InitialPositionFEN, Piece.R, BoardSquare.a1, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.R, BoardSquare.h1, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.r, BoardSquare.a8, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.r, BoardSquare.h8, true)]

    [TestCase(Constants.InitialPositionFEN, Piece.N, BoardSquare.b1, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.N, BoardSquare.g1, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.n, BoardSquare.b8, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.n, BoardSquare.g8, true)]

    [TestCase(Constants.InitialPositionFEN, Piece.B, BoardSquare.c1, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.B, BoardSquare.f1, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.b, BoardSquare.c8, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.b, BoardSquare.f8, true)]

    [TestCase(Constants.InitialPositionFEN, Piece.Q, BoardSquare.d1, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.Q, BoardSquare.e1, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.q, BoardSquare.d8, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.q, BoardSquare.e8, false)]

    [TestCase(Constants.InitialPositionFEN, Piece.K, BoardSquare.e1, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.K, BoardSquare.d1, false)]
    [TestCase(Constants.InitialPositionFEN, Piece.k, BoardSquare.e8, true)]
    [TestCase(Constants.InitialPositionFEN, Piece.k, BoardSquare.d8, false)]
    public void Contains_DoesNotContain(string fen, Piece piece, BoardSquare square, bool shouldContain)
    {
        var position = new Position(fen);
        var bb = position.PieceBitBoards[(int)piece];

        Assert.AreEqual(shouldContain, bb.Contains((int)square));
        Assert.AreEqual(!shouldContain, bb.DoesNotContain((int)square));
    }
}
