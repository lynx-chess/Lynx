using SharpFish.Model;
using System;
using Xunit;

namespace SharpFish.Test.Attacks
{
    public class SetBishopOrRookOccupancyTest
    {
        [Fact]
        public void SetBishopOccupancy()
        {
            // Arrange
            var occupancyMask = AttacksGenerator.MaskBishopOccupancy((int)BoardSquares.d4);
            var maxIndex = (int)Math.Pow(2, occupancyMask.CountBits()) - 1;

            // Act
            var occupancy = AttacksGenerator.SetBishopOrRookOccupancy(0, occupancyMask);
            // Assert
            Assert.True(occupancy.Empty);

            // Act
            occupancy = AttacksGenerator.SetBishopOrRookOccupancy(maxIndex, occupancyMask);
            // Assert
            Assert.Equal(occupancyMask.Board, occupancy.Board);
        }

        [Theory]
        [InlineData(BoardSquares.a8)]
        [InlineData(BoardSquares.a7)]
        public void SetRookOccupancy(BoardSquares rookSquare)
        {
            // Arrange
            var occupancyMask = AttacksGenerator.MaskRookOccupancy((int)rookSquare);
            var maxIndex = (int)Math.Pow(2, occupancyMask.CountBits()) - 1;

            // Act - empty board
            var occupancy = AttacksGenerator.SetBishopOrRookOccupancy(0, occupancyMask);
            // Assert
            Assert.True(occupancy.Empty);

            // Act - top rank occupied
            var index = (int)Math.Pow(2, 6) - 1;
            occupancy = AttacksGenerator.SetBishopOrRookOccupancy(index, occupancyMask);
            // Assert
            Assert.Equal(0b01111110UL << 8 * ((int)rookSquare / 8), occupancy.Board);

            // Act - max occupancy
            occupancy = AttacksGenerator.SetBishopOrRookOccupancy(maxIndex, occupancyMask);
            // Assert
            Assert.Equal(occupancyMask.Board, occupancy.Board);
        }
    }
}
