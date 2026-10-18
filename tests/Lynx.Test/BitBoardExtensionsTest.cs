using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

public class BitboardExtensionsTest
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
    public void LSB(Bitboard n, ulong lsb)
    {
        Assert.AreEqual(lsb, n.LSB());

        Assert.AreEqual(lsb, lsb & (~lsb + 1));

        if (System.Runtime.Intrinsics.X86.Bmi1.IsSupported)
        {
            Assert.AreEqual(lsb, System.Runtime.Intrinsics.X86.Bmi1.X64.ExtractLowestSetBit(n));
        }
    }
}
