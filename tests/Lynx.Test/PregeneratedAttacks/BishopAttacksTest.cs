using Lynx.Model;
using Xunit;
using BS = Lynx.Model.BoardSquares;

namespace Lynx.Test.PregeneratedAttacks
{
    public class BishopAttacksTest
    {
        [Theory]
        [InlineData(BS.a8, new BS[] { }, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2, BS.h1 })]
        [InlineData(BS.a8, new[] { BS.g2 }, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2 })]
        [InlineData(BS.a8, new[] { BS.b7 }, new[] { BS.b7 })]

        [InlineData(BS.h1, new BS[] { }, new[] { BS.g2, BS.f3, BS.e4, BS.d5, BS.c6, BS.b7, BS.a8 })]
        [InlineData(BS.h1, new[] { BS.c6 }, new[] { BS.g2, BS.f3, BS.e4, BS.d5, BS.c6 })]
        [InlineData(BS.h1, new[] { BS.g2 }, new[] { BS.g2, })]

        [InlineData(BS.a1, new BS[] { }, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7, BS.h8 })]
        [InlineData(BS.a1, new[] { BS.g7 }, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7 })]
        [InlineData(BS.a1, new[] { BS.b2 }, new[] { BS.b2 })]

        [InlineData(BS.h8, new BS[] { }, new[] { BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2, BS.a1 })]
        [InlineData(BS.h8, new[] { BS.b2 }, new[] { BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2 })]
        [InlineData(BS.h8, new[] { BS.g7 }, new[] { BS.g7, })]

        [InlineData(BS.d4, new[] { BS.a7, BS.a7, BS.g7, BS.b2, BS.e3 }, new[] { BS.a7, BS.b6, BS.c5, BS.e3, BS.b2, BS.c3, BS.e5, BS.f6, BS.g7 })]
        public void GenerateBishopAttacksOnTheFly(BS bishopSquare, BS[] occupiedSquares, BS[] attackedSquares)
        {
            // Arrange
            var occupancy = new BitBoard(occupiedSquares);

            // Act
            var attacks = AttacksGenerator.GenerateBishopAttacksOnTheFly((int)bishopSquare, occupancy);

            // Assert
            ValidateAttacks(attackedSquares, attacks);

            static void ValidateAttacks(BS[] attackedSquares, BitBoard attacks)
            {
                foreach (var attackedSquare in attackedSquares)
                {
                    Assert.True(attacks.GetBit(attackedSquare));
                    attacks.PopBit(attackedSquare);
                }

                Assert.Equal(default, attacks);
            }
        }

        /// <summary>
        /// Implicitly tests <see cref="AttacksGenerator.InitializeBishopAttacks"/> and <see cref="Constants.BishopMagicNumbers"/>
        /// </summary>
        /// <param name="bishopSquare"></param>
        /// <param name="occupiedSquares"></param>
        /// <param name="attackedSquares"></param>
        [Theory]
        [InlineData(BS.a8, new BS[] { }, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2, BS.h1 })]
        [InlineData(BS.a8, new[] { BS.g2 }, new[] { BS.b7, BS.c6, BS.d5, BS.e4, BS.f3, BS.g2 })]
        [InlineData(BS.a8, new[] { BS.b7 }, new[] { BS.b7 })]

        [InlineData(BS.h1, new BS[] { }, new[] { BS.g2, BS.f3, BS.e4, BS.d5, BS.c6, BS.b7, BS.a8 })]
        [InlineData(BS.h1, new[] { BS.c6 }, new[] { BS.g2, BS.f3, BS.e4, BS.d5, BS.c6 })]
        [InlineData(BS.h1, new[] { BS.g2 }, new[] { BS.g2, })]

        [InlineData(BS.a1, new BS[] { }, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7, BS.h8 })]
        [InlineData(BS.a1, new[] { BS.g7 }, new[] { BS.b2, BS.c3, BS.d4, BS.e5, BS.f6, BS.g7 })]
        [InlineData(BS.a1, new[] { BS.b2 }, new[] { BS.b2 })]

        [InlineData(BS.h8, new BS[] { }, new[] { BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2, BS.a1 })]
        [InlineData(BS.h8, new[] { BS.b2 }, new[] { BS.g7, BS.f6, BS.e5, BS.d4, BS.c3, BS.b2 })]
        [InlineData(BS.h8, new[] { BS.g7 }, new[] { BS.g7, })]

        [InlineData(BS.d4, new[] { BS.a7, BS.a7, BS.g7, BS.b2, BS.e3 }, new[] { BS.a7, BS.b6, BS.c5, BS.e3, BS.b2, BS.c3, BS.e5, BS.f6, BS.g7 })]
        public void GetBishopAttacks(BS bishopSquare, BS[] occupiedSquares, BS[] attackedSquares)
        {
            // Arrange
            var occupancy = new BitBoard(occupiedSquares);

            // Act
            var attacks = Attacks.BishopAttacks((int)bishopSquare, occupancy);

            // Assert
            foreach (var attackedSquare in attackedSquares)
            {
                Assert.True(attacks.GetBit(attackedSquare));
                attacks.PopBit(attackedSquare);
            }

            Assert.Equal(default, attacks);
        }
    }
}
