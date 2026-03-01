using Lynx.Model;
using NUnit.Framework;
using BS = Lynx.Model.BoardSquare;

namespace Lynx.Test.PregeneratedAttacks;

public class KnightAttacksTest
{
    [TestCase(BS.a8, new[] { BS.c7, BS.b6 })]
    [TestCase(BS.h8, new[] { BS.f7, BS.g6 })]
    [TestCase(BS.a1, new[] { BS.b3, BS.c2 })]
    [TestCase(BS.h1, new[] { BS.g3, BS.f2 })]

    [TestCase(BS.b8, new[] { BS.a6, BS.c6, BS.d7 })]
    [TestCase(BS.g8, new[] { BS.e7, BS.f6, BS.h6 })]
    [TestCase(BS.b1, new[] { BS.a3, BS.c3, BS.d2 })]
    [TestCase(BS.g1, new[] { BS.e2, BS.f3, BS.h3 })]

    [TestCase(BS.a7, new[] { BS.c8, BS.c6, BS.b5 })]
    [TestCase(BS.h7, new[] { BS.f8, BS.f6, BS.g5 })]
    [TestCase(BS.a2, new[] { BS.b4, BS.c3, BS.c1 })]
    [TestCase(BS.h2, new[] { BS.f1, BS.f3, BS.g4 })]

    [TestCase(BS.d8, new[] { BS.b7, BS.c6, BS.e6, BS.f7 })]
    [TestCase(BS.a5, new[] { BS.b7, BS.c6, BS.c4, BS.b3 })]
    [TestCase(BS.h5, new[] { BS.g7, BS.f6, BS.f4, BS.g3 })]
    [TestCase(BS.d1, new[] { BS.b2, BS.c3, BS.e3, BS.f2 })]

    [TestCase(BS.b6, new[] { BS.a8, BS.c8, BS.d7, BS.d5, BS.c4, BS.a4 })]
    [TestCase(BS.g6, new[] { BS.h8, BS.f8, BS.e7, BS.e5, BS.f4, BS.h4 })]
    [TestCase(BS.b3, new[] { BS.a5, BS.c5, BS.d4, BS.d2, BS.c1, BS.a1 })]
    [TestCase(BS.g3, new[] { BS.f5, BS.h5, BS.e4, BS.e2, BS.f1, BS.h1 })]

    [TestCase(BS.e4, new[] { BS.d6, BS.f6, BS.c5, BS.g5, BS.c3, BS.g3, BS.d2, BS.f2 })]
    [TestCase(BS.e5, new[] { BS.d7, BS.f7, BS.c6, BS.g6, BS.c4, BS.g4, BS.d3, BS.f3 })]
    [TestCase(BS.d4, new[] { BS.c6, BS.e6, BS.b5, BS.f5, BS.b3, BS.f3, BS.c2, BS.e2 })]
    [TestCase(BS.d5, new[] { BS.c7, BS.e7, BS.b6, BS.f6, BS.b4, BS.f4, BS.c3, BS.e3 })]
    public void MaskKnightAttacks(BS knightSquare, BS[] attackedSquares)
    {
        var attacks = AttackGenerator.MaskKnightAttacks((int)knightSquare);
        ValidateAttacks(attackedSquares, attacks);

        attacks = Attacks.KnightAttacks[(int)knightSquare];
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
}
