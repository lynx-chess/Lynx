using Xunit;
using BS = Lynx.Model.BoardSquares;

namespace Lynx.Test.PregeneratedAttacks
{
    public class KnightAttacksTest
    {
        [Theory]
        [InlineData(BS.a8, new[] { BS.c7, BS.b6 })]
        [InlineData(BS.h8, new[] { BS.f7, BS.g6 })]
        [InlineData(BS.a1, new[] { BS.b3, BS.c2 })]
        [InlineData(BS.h1, new[] { BS.g3, BS.f2 })]

        [InlineData(BS.b8, new[] { BS.a6, BS.c6, BS.d7 })]
        [InlineData(BS.g8, new[] { BS.e7, BS.f6, BS.h6 })]
        [InlineData(BS.b1, new[] { BS.a3, BS.c3, BS.d2 })]
        [InlineData(BS.g1, new[] { BS.e2, BS.f3, BS.h3 })]

        [InlineData(BS.a7, new[] { BS.c8, BS.c6, BS.b5 })]
        [InlineData(BS.h7, new[] { BS.f8, BS.f6, BS.g5 })]
        [InlineData(BS.a2, new[] { BS.b4, BS.c3, BS.c1 })]
        [InlineData(BS.h2, new[] { BS.f1, BS.f3, BS.g4 })]

        [InlineData(BS.d8, new[] { BS.b7, BS.c6, BS.e6, BS.f7 })]
        [InlineData(BS.a5, new[] { BS.b7, BS.c6, BS.c4, BS.b3 })]
        [InlineData(BS.h5, new[] { BS.g7, BS.f6, BS.f4, BS.g3 })]
        [InlineData(BS.d1, new[] { BS.b2, BS.c3, BS.e3, BS.f2 })]

        [InlineData(BS.b6, new[] { BS.a8, BS.c8, BS.d7, BS.d5, BS.c4, BS.a4 })]
        [InlineData(BS.g6, new[] { BS.h8, BS.f8, BS.e7, BS.e5, BS.f4, BS.h4 })]
        [InlineData(BS.b3, new[] { BS.a5, BS.c5, BS.d4, BS.d2, BS.c1, BS.a1 })]
        [InlineData(BS.g3, new[] { BS.f5, BS.h5, BS.e4, BS.e2, BS.f1, BS.h1 })]

        [InlineData(BS.e4, new[] { BS.d6, BS.f6, BS.c5, BS.g5, BS.c3, BS.g3, BS.d2, BS.f2 })]
        [InlineData(BS.e5, new[] { BS.d7, BS.f7, BS.c6, BS.g6, BS.c4, BS.g4, BS.d3, BS.f3 })]
        [InlineData(BS.d4, new[] { BS.c6, BS.e6, BS.b5, BS.f5, BS.b3, BS.f3, BS.c2, BS.e2 })]
        [InlineData(BS.d5, new[] { BS.c7, BS.e7, BS.b6, BS.f6, BS.b4, BS.f4, BS.c3, BS.e3 })]
        public void MaskKnightAttacks(BS knightSquare, BS[] attackedSquares)
        {
            var attacks = AttacksGenerator.MaskKnightAttacks((int)knightSquare);
            ValidateAttacks(attackedSquares, attacks);

            attacks = Attacks.KnightAttacks[(int)knightSquare];
            ValidateAttacks(attackedSquares, attacks);

            static void ValidateAttacks(BS[] attackedSquares, Model.BitBoard attacks)
            {
                foreach (var attackedSquare in attackedSquares)
                {
                    Assert.True(attacks.GetBit(attackedSquare));
                    attacks.PopBit(attackedSquare);
                }

                Assert.Equal(default, attacks);
            }
        }
    }
}
