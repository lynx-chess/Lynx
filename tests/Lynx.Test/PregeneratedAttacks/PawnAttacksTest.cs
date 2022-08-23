using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.PregeneratedAttacks;

public class PawnAttacksTest
{
    [TestCase(BoardSquare.a1, true, 1UL << (int)BoardSquare.b2)]
    [TestCase(BoardSquare.a1, false, 0UL)]
    [TestCase(BoardSquare.a8, true, 0UL)]
    [TestCase(BoardSquare.a8, false, 1UL << (int)BoardSquare.b7)]

    [TestCase(BoardSquare.h1, true, 1UL << (int)BoardSquare.g2)]
    [TestCase(BoardSquare.h1, false, 0UL)]
    [TestCase(BoardSquare.h8, true, 0UL)]
    [TestCase(BoardSquare.h8, false, 1UL << (int)BoardSquare.g7)]

    [TestCase(BoardSquare.b6, true, 0b101UL << (int)BoardSquare.a7)]
    [TestCase(BoardSquare.b6, false, 0b101UL << (int)BoardSquare.a5)]
    [TestCase(BoardSquare.e3, true, 0b101UL << (int)BoardSquare.d4)]
    [TestCase(BoardSquare.e3, false, 0b101UL << (int)BoardSquare.d2)]
    public void MaskPawnAttacks(BoardSquare square, bool isWhite, ulong expectedResult)
    {
        // Act
        var attacks = AttackGenerator.MaskPawnAttacks((int)square, isWhite);

        // Assert
        Assert.AreEqual(expectedResult, attacks);

        // Act
        attacks = Attacks.PawnAttacks[isWhite ? 1 : 0, (int)square];

        // Assert
        Assert.AreEqual(expectedResult, attacks);
    }

    [Test]
    public void InitializePawnAttacks()
    {
        // Act
        var result = AttackGenerator.InitializePawnAttacks();

        // Assert
        foreach (var square in Enum.GetValues<BoardSquare>())
        {
            var intSquare = (int)square;

            if (intSquare < 8 || intSquare > (63 - 8))
            {
                continue;
            }

            bool aFile = !Constants.NotAFile.GetBit(intSquare);
            bool hFile = !Constants.NotHFile.GetBit(intSquare);

            var attackDiagram = aFile || hFile
                ? 1UL
                : 0b101UL;

            var whiteOffset = aFile
                ? 7     // a7 -> b8 (1)
                : 9;    // c7 -> b8 (101)

            var expectedWhiteResult = attackDiagram << (intSquare - whiteOffset);
            Assert.AreEqual(expectedWhiteResult, result[1, intSquare]);

            var blackOffset = aFile
                ? 9     // a7 -> b8 (1)
                : 7;    // c7 -> b6 (101)

            var expectedBlackResult = attackDiagram << (intSquare + blackOffset);
            Assert.AreEqual(expectedBlackResult, result[0, intSquare]);
        }

        for (int square = 0; square < 8; ++square)
        {
            Assert.AreEqual(0UL, result[1, square]);
            Assert.AreEqual(0UL, result[0, 63 - square]);
        }
    }
}
