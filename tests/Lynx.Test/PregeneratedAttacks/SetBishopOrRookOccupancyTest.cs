using Lynx.Model;
using System;
using Xunit;

namespace Lynx.Test.PregeneratedAttacks
{
    public class SetBishopOrRookOccupancyTest
    {
        [Fact]
        public void SetBishopOccupancy()
        {
            // Arrange
            var occupancyMask = AttackGenerator.MaskBishopOccupancy((int)BoardSquare.d4);
            var maxIndex = (int)Math.Pow(2, occupancyMask.CountBits()) - 1;

            // Act
            var occupancy = AttackGenerator.SetBishopOrRookOccupancy(0, occupancyMask);
            // Assert
            Assert.True(occupancy.Empty);

            // Act
            occupancy = AttackGenerator.SetBishopOrRookOccupancy(maxIndex, occupancyMask);
            // Assert
            Assert.Equal(occupancyMask.Board, occupancy.Board);
        }

        [Theory]
        [InlineData(BoardSquare.a8)]
        [InlineData(BoardSquare.a7)]
        public void SetRookOccupancy(BoardSquare rookSquare)
        {
            // Arrange
            var occupancyMask = AttackGenerator.MaskRookOccupancy((int)rookSquare);
            var maxIndex = (int)Math.Pow(2, occupancyMask.CountBits()) - 1;

            // Act - empty board
            var occupancy = AttackGenerator.SetBishopOrRookOccupancy(0, occupancyMask);
            // Assert
            Assert.True(occupancy.Empty);

            // Act - top rank occupied
            var index = (int)Math.Pow(2, 6) - 1;
            occupancy = AttackGenerator.SetBishopOrRookOccupancy(index, occupancyMask);
            // Assert
            Assert.Equal(0b01111110UL << 8 * ((int)rookSquare / 8), occupancy.Board);

            // Act - max occupancy
            occupancy = AttackGenerator.SetBishopOrRookOccupancy(maxIndex, occupancyMask);
            // Assert
            Assert.Equal(occupancyMask.Board, occupancy.Board);
        }
    }
}
