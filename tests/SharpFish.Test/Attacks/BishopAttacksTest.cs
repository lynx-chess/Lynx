using Xunit;
using BS = SharpFish.Model.BoardSquares;

namespace SharpFish.Test.Attacks
{
    public class BishopAttacksTest
    {
        [Theory]
        [InlineData(BS.a8, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2 })]
        [InlineData(BS.h1, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2 })]
        [InlineData(BS.a1, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7 })]
        [InlineData(BS.h8, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7 })]

        [InlineData(BS.b7, new[] { BS.c6, BS.d5, BS.e4, BS.f3, BS.g2 })]
        [InlineData(BS.g7, new[] { BS.f6, BS.e5, BS.d4, BS.c3, BS.b2 })]
        [InlineData(BS.b2, new[] { BS.c3, BS.d4, BS.e5, BS.f6, BS.g7 })]
        [InlineData(BS.g2, new[] { BS.f3, BS.e4, BS.d5, BS.c6, BS.b7 })]

        [InlineData(BS.c6, new[] { BS.b7, BS.d5, BS.e4, BS.f3, BS.g2, BS.d7, BS.b5 })]
        [InlineData(BS.f6, new[] { BS.g7, BS.e5, BS.d4, BS.c3, BS.b2, BS.e7, BS.g5 })]
        [InlineData(BS.c3, new[] { BS.b2, BS.d4, BS.e5, BS.f6, BS.g7, BS.b4, BS.d2 })]
        [InlineData(BS.f3, new[] { BS.g2, BS.e4, BS.d5, BS.c6, BS.b7, BS.g4, BS.e2 })]

        [InlineData(BS.e4, new[] { BS.b7, BS.c6, BS.d5, BS.f3, BS.g2, BS.c2, BS.d3, BS.f5, BS.g6 })]
        [InlineData(BS.d4, new[] { BS.g7, BS.f6, BS.e5, BS.c3, BS.b2, BS.b6, BS.c5, BS.e3, BS.f2 })]
        public void MaskBishopAttacks(BS bishopSquare, BS[] attackedSquares)
        {
            var attacks = AttacksGenerator.MaskBishopAttacks((int)bishopSquare);

            foreach (var attackedSquare in attackedSquares)
            {
                Assert.True(attacks.GetBit(attackedSquare));
                attacks.PopBit(attackedSquare);
            }

            Assert.Equal(default, attacks);
        }
    }
}
