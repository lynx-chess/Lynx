using Lynx.Model;
using NUnit.Framework;
using BS = Lynx.Model.BoardSquare;

namespace Lynx.Test.PregeneratedAttacks
{
    public class RookOccupancyTest
    {
        [TestCase(BS.a8, new[] { BS.b8, BS.c8, BS.d8, BS.e8, BS.f8, BS.g8, BS.a2, BS.a3, BS.a4, BS.a5, BS.a6, BS.a7 })]
        [TestCase(BS.a1, new[] { BS.b1, BS.c1, BS.d1, BS.e1, BS.f1, BS.g1, BS.a2, BS.a3, BS.a4, BS.a5, BS.a6, BS.a7 })]
        [TestCase(BS.h8, new[] { BS.b8, BS.c8, BS.d8, BS.e8, BS.f8, BS.g8, BS.h2, BS.h3, BS.h4, BS.h5, BS.h6, BS.h7 })]
        [TestCase(BS.h1, new[] { BS.b1, BS.c1, BS.d1, BS.e1, BS.f1, BS.g1, BS.h2, BS.h3, BS.h4, BS.h5, BS.h6, BS.h7 })]

        [TestCase(BS.c6, new[] { BS.b6, BS.d6, BS.e6, BS.f6, BS.g6, BS.c7, BS.c5, BS.c4, BS.c3, BS.c2 })]
        [TestCase(BS.f6, new[] { BS.b6, BS.c6, BS.d6, BS.e6, BS.g6, BS.f7, BS.f5, BS.f4, BS.f3, BS.f2 })]
        [TestCase(BS.c3, new[] { BS.b3, BS.d3, BS.e3, BS.f3, BS.g3, BS.c7, BS.c6, BS.c5, BS.c4, BS.c2 })]
        [TestCase(BS.f3, new[] { BS.b3, BS.c3, BS.d3, BS.e3, BS.g3, BS.f7, BS.f6, BS.f5, BS.f4, BS.f2 })]

        [TestCase(BS.e4, new[] { BS.b4, BS.c4, BS.d4, BS.f4, BS.g4, BS.e7, BS.e6, BS.e5, BS.e3, BS.e2 })]
        [TestCase(BS.d4, new[] { BS.b4, BS.c4, BS.e4, BS.f4, BS.g4, BS.d7, BS.d6, BS.d5, BS.d3, BS.d2 })]
        public void MaskRookOccupancy(BS rookSquare, BS[] attackedSquares)
        {
            var attacks = AttackGenerator.MaskRookOccupancy((int)rookSquare);

            foreach (var attackedSquare in attackedSquares)
            {
                Assert.True(attacks.GetBit(attackedSquare));
                attacks.PopBit(attackedSquare);
            }

            Assert.AreEqual(default(BitBoard), attacks);
        }
    }
}
