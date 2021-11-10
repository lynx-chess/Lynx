using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test
{
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
                if (value > 0)
                {
                    Assert.AreEqual(value.ToString(), Constants.AsciiPieces[(int)value].ToString());
                }
            }
        }

        [Test]
        public void PiecesByChar()
        {
            foreach (var value in Enum.GetValues<Piece>())
            {
                if (value > 0)
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
            }

            Assert.AreEqual((int)BoardSquare.d4, Constants.EnPassantCaptureSquares[(int)BoardSquare.d3]);
            for (int square = (int)BoardSquare.a3; square <= (int)BoardSquare.h3; ++square)
            {
                Assert.AreEqual(square - 8, Constants.EnPassantCaptureSquares[square]);
            }
        }
    }
}
