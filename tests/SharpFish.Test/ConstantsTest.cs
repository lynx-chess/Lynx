using SharpFish.Model;
using System;
using Xunit;

namespace SharpFish.Test
{
    public class ConstantsTest
    {
        [Fact]
        public void BishopRelevantOccupancyBits()
        {
            for (int square = 0; square < 64; ++square)
            {
                var mask = AttacksGenerator.MaskBishopOccupancy(square);

                Assert.Equal(Constants.BishopRelevantOccupancyBits[square], mask.CountBits());
            }
        }

        [Fact]
        public void RookRelevantOccupancyBits()
        {
            for (int square = 0; square < 64; ++square)
            {
                var mask = AttacksGenerator.MaskRookOccupancy(square);

                Assert.Equal(Constants.RookRelevantOccupancyBits[square], mask.CountBits());
            }
        }

        [Fact]
        public void AsciiPieces()
        {
            foreach (var value in Enum.GetValues<Piece>())
            {
                Assert.Equal(value.ToString(), Constants.AsciiPieces[(int)value].ToString());
            }
        }

        [Fact]
        public void PiecesByChar()
        {
            foreach (var value in Enum.GetValues<Piece>())
            {
                Assert.Equal(value, Constants.PiecesByChar[value.ToString()[0]]);
            }
        }
    }
}
