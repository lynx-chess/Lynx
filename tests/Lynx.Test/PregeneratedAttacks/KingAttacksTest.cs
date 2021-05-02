using Xunit;
using BS = Lynx.Model.BoardSquare;

namespace Lynx.Test.PregeneratedAttacks
{
    public class KingAttacksTest
    {
        [Theory]
        [InlineData(BS.a8, new[] { BS.b8, BS.a7, BS.b7 })]
        [InlineData(BS.h8, new[] { BS.g8, BS.g7, BS.h7 })]
        [InlineData(BS.a1, new[] { BS.a2, BS.b2, BS.b1 })]
        [InlineData(BS.h1, new[] { BS.g2, BS.h2, BS.g1 })]

        [InlineData(BS.b8, new[] { BS.a8, BS.c8, BS.a7, BS.b7, BS.c7 })]
        [InlineData(BS.g8, new[] { BS.f8, BS.h8, BS.f7, BS.g7, BS.h7 })]
        [InlineData(BS.b1, new[] { BS.a1, BS.c1, BS.a2, BS.b2, BS.c2 })]
        [InlineData(BS.g1, new[] { BS.f1, BS.h1, BS.f2, BS.g2, BS.h2 })]

        [InlineData(BS.a7, new[] { BS.a8, BS.b8, BS.b7, BS.a6, BS.b6 })]
        [InlineData(BS.h7, new[] { BS.g8, BS.h8, BS.g7, BS.g6, BS.h6 })]
        [InlineData(BS.a2, new[] { BS.a3, BS.b3, BS.b2, BS.a1, BS.b1 })]
        [InlineData(BS.h2, new[] { BS.g3, BS.h3, BS.g2, BS.g1, BS.h1 })]

        [InlineData(BS.e4, new[] { BS.d5, BS.e5, BS.f5, BS.d4, BS.f4, BS.d3, BS.e3, BS.f3 })]
        [InlineData(BS.e5, new[] { BS.d6, BS.e6, BS.f6, BS.d5, BS.f5, BS.d4, BS.e4, BS.f4 })]
        [InlineData(BS.d4, new[] { BS.c5, BS.d5, BS.e5, BS.c4, BS.e4, BS.c3, BS.d3, BS.e3 })]
        [InlineData(BS.d5, new[] { BS.c6, BS.d6, BS.e6, BS.c5, BS.e5, BS.c4, BS.d4, BS.e4 })]
        public void MaskKingAttacks(BS kingSquare, BS[] attackedSquares)
        {
            var attacks = AttackGenerator.MaskKingAttacks((int)kingSquare);
            ValidateAttacks(attackedSquares, attacks);

            attacks = Attacks.KingAttacks[(int)kingSquare];
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
