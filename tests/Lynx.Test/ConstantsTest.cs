using Lynx.Model;
using System;
using Xunit;

namespace Lynx.Test
{
    public class ConstantsTest
    {
        [Fact]
        public void BishopRelevantOccupancyBits()
        {
            for (int square = 0; square < 64; ++square)
            {
                var mask = AttackGenerator.MaskBishopOccupancy(square);

                Assert.Equal(Constants.BishopRelevantOccupancyBits[square], mask.CountBits());
            }
        }

        [Fact]
        public void RookRelevantOccupancyBits()
        {
            for (int square = 0; square < 64; ++square)
            {
                var mask = AttackGenerator.MaskRookOccupancy(square);

                Assert.Equal(Constants.RookRelevantOccupancyBits[square], mask.CountBits());
            }
        }

        [Fact]
        public void AsciiPieces()
        {
            foreach (var value in Enum.GetValues<Piece>())
            {
                if (value > 0)
                {
                    Assert.Equal(value.ToString(), Constants.AsciiPieces[(int)value].ToString());
                }
            }
        }

        [Fact]
        public void PiecesByChar()
        {
            foreach (var value in Enum.GetValues<Piece>())
            {
                if (value > 0)
                {
                    Assert.Equal(value, Constants.PiecesByChar[value.ToString()[0]]);
                }
            }
        }

        [Fact]
        public void EnPassantCaptureSquares()
        {
            Assert.Equal((int)BoardSquare.e5, Constants.EnPassantCaptureSquares[(int)BoardSquare.e6]);
            for (int square = (int)BoardSquare.a6; square <= (int)BoardSquare.h6; ++square)
            {
                Assert.Equal(square + 8, Constants.EnPassantCaptureSquares[square]);
            }

            Assert.Equal((int)BoardSquare.d4, Constants.EnPassantCaptureSquares[(int)BoardSquare.d3]);
            for (int square = (int)BoardSquare.a3; square <= (int)BoardSquare.h3; ++square)
            {
                Assert.Equal(square - 8, Constants.EnPassantCaptureSquares[square]);
            }
        }
    }
}
