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
    }
}
