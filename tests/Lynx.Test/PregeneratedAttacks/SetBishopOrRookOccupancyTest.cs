using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.PregeneratedAttacks;

public class SetBishopOrRookOccupancyTest
{
    [Test]
    public void SetBishopOccupancy()
    {
        // Arrange
        var occupancyMask = AttackGenerator.MaskBishopOccupancy((int)BoardSquare.d4);
        var maxIndex = (int)Math.Pow(2, occupancyMask.CountBits()) - 1;

        // Act
        var occupancy = AttackGenerator.SetBishopOrRookOccupancy(0, occupancyMask);
        // Assert
        Assert.True(occupancy.Empty());

        // Act
        occupancy = AttackGenerator.SetBishopOrRookOccupancy(maxIndex, occupancyMask);
        // Assert
        Assert.AreEqual(occupancyMask, occupancy);
    }

    [TestCase(BoardSquare.a8)]
    [TestCase(BoardSquare.a7)]
    public void SetRookOccupancy(BoardSquare rookSquare)
    {
        // Arrange
        var occupancyMask = AttackGenerator.MaskRookOccupancy((int)rookSquare);
        var maxIndex = (int)Math.Pow(2, occupancyMask.CountBits()) - 1;

        // Act - empty board
        var occupancy = AttackGenerator.SetBishopOrRookOccupancy(0, occupancyMask);
        // Assert
        Assert.True(occupancy.Empty());

        // Act - top rank occupied
        var index = (int)Math.Pow(2, 6) - 1;
        occupancy = AttackGenerator.SetBishopOrRookOccupancy(index, occupancyMask);
        // Assert
        Assert.AreEqual(0b01111110UL << 8 * ((int)rookSquare / 8), occupancy);

        // Act - max occupancy
        occupancy = AttackGenerator.SetBishopOrRookOccupancy(maxIndex, occupancyMask);
        // Assert
        Assert.AreEqual(occupancyMask, occupancy);
    }
}
