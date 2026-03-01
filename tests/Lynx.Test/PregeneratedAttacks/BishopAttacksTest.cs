using Lynx.Model;
using NUnit.Framework;
using BS = Lynx.Model.BoardSquare;

namespace Lynx.Test.PregeneratedAttacks;

public class BishopAttacksTest
{
    [TestCase(BS.a8, new BS[] { }, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2, BS.h1 })]
    [TestCase(BS.a8, new[] { BS.g2 }, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2 })]
    [TestCase(BS.a8, new[] { BS.b7 }, new[] { BS.b7 })]

    [TestCase(BS.h1, new BS[] { }, new[] { BS.g2, BS.f3, BS.e4, BS.d5, BS.c6, BS.b7, BS.a8 })]
    [TestCase(BS.h1, new[] { BS.c6 }, new[] { BS.g2, BS.f3, BS.e4, BS.d5, BS.c6 })]
    [TestCase(BS.h1, new[] { BS.g2 }, new[] { BS.g2, })]

    [TestCase(BS.a1, new BS[] { }, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7, BS.h8 })]
    [TestCase(BS.a1, new[] { BS.g7 }, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7 })]
    [TestCase(BS.a1, new[] { BS.b2 }, new[] { BS.b2 })]

    [TestCase(BS.h8, new BS[] { }, new[] { BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2, BS.a1 })]
    [TestCase(BS.h8, new[] { BS.b2 }, new[] { BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2 })]
    [TestCase(BS.h8, new[] { BS.g7 }, new[] { BS.g7, })]

    [TestCase(BS.d4, new[] { BS.a7, BS.a7, BS.g7, BS.b2, BS.e3 }, new[] { BS.a7, BS.b6, BS.c5, BS.e3, BS.b2, BS.c3, BS.e5, BS.f6, BS.g7 })]
    public void GenerateBishopAttacksOnTheFly(BS bishopSquare, BS[] occupiedSquares, BS[] attackedSquares)
    {
        // Arrange
        var occupancy = BitboardExtensions.Initialize(occupiedSquares);

        // Act
        var attacks = AttackGenerator.GenerateBishopAttacksOnTheFly((int)bishopSquare, occupancy);

        // Assert
        ValidateAttacks(attackedSquares, attacks);

        static void ValidateAttacks(BS[] attackedSquares, Bitboard attacks)
        {
            foreach (var attackedSquare in attackedSquares)
            {
                Assert.True(attacks.GetBit(attackedSquare));
                attacks.PopBit(attackedSquare);
            }

            Assert.AreEqual(default(Bitboard), attacks);
        }
    }

    /// <summary>
    /// Implicitly tests <see cref="AttackGenerator.InitializeBishopMagicAttacks"/> and <see cref="Constants.BishopMagicNumbers"/>
    /// </summary>
    [TestCase(BS.a8, new BS[] { }, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2, BS.h1 })]
    [TestCase(BS.a8, new[] { BS.g2 }, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2 })]
    [TestCase(BS.a8, new[] { BS.b7 }, new[] { BS.b7 })]

    [TestCase(BS.h1, new BS[] { }, new[] { BS.g2, BS.f3, BS.e4, BS.d5, BS.c6, BS.b7, BS.a8 })]
    [TestCase(BS.h1, new[] { BS.c6 }, new[] { BS.g2, BS.f3, BS.e4, BS.d5, BS.c6 })]
    [TestCase(BS.h1, new[] { BS.g2 }, new[] { BS.g2, })]

    [TestCase(BS.a1, new BS[] { }, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7, BS.h8 })]
    [TestCase(BS.a1, new[] { BS.g7 }, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7 })]
    [TestCase(BS.a1, new[] { BS.b2 }, new[] { BS.b2 })]

    [TestCase(BS.h8, new BS[] { }, new[] { BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2, BS.a1 })]
    [TestCase(BS.h8, new[] { BS.b2 }, new[] { BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2 })]
    [TestCase(BS.h8, new[] { BS.g7 }, new[] { BS.g7, })]

    [TestCase(BS.d4, new[] { BS.a7, BS.a7, BS.g7, BS.b2, BS.e3 }, new[] { BS.a7, BS.b6, BS.c5, BS.e3, BS.b2, BS.c3, BS.e5, BS.f6, BS.g7 })]
    public void GetBishopAttacks(BS bishopSquare, BS[] occupiedSquares, BS[] attackedSquares)
    {
        // Arrange
        var occupancy = BitboardExtensions.Initialize(occupiedSquares);

        // Act
        var attacks = Attacks.BishopAttacks((int)bishopSquare, occupancy);
        Assert.AreEqual(Attacks.MagicNumbersBishopAttacks((int)bishopSquare, occupancy), attacks);

        // Assert
        foreach (var attackedSquare in attackedSquares)
        {
            Assert.True(attacks.GetBit(attackedSquare));
            attacks.PopBit(attackedSquare);
        }

        Assert.AreEqual(default(Bitboard), attacks);
    }
}
