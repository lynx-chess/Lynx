using SharpFish.Model;
using System;
using Xunit;

namespace SharpFish.Test.Attacks
{
    public class PawnAttacksTest
    {
        [Theory]
        [InlineData(BoardSquares.a1, true, 1UL << (int)BoardSquares.b2)]
        [InlineData(BoardSquares.a1, false, 0UL)]
        [InlineData(BoardSquares.a8, true, 0UL)]
        [InlineData(BoardSquares.a8, false, 1UL << (int)BoardSquares.b7)]

        [InlineData(BoardSquares.h1, true, 1UL << (int)BoardSquares.g2)]
        [InlineData(BoardSquares.h1, false, 0UL)]
        [InlineData(BoardSquares.h8, true, 0UL)]
        [InlineData(BoardSquares.h8, false, 1UL << (int)BoardSquares.g7)]

        [InlineData(BoardSquares.b6, true, 0b101UL << (int)BoardSquares.a7)]
        [InlineData(BoardSquares.b6, false, 0b101UL << (int)BoardSquares.a5)]
        [InlineData(BoardSquares.e3, true, 0b101UL << (int)BoardSquares.d4)]
        [InlineData(BoardSquares.e3, false, 0b101UL << (int)BoardSquares.d2)]
        public void MaskPawnAttacks(BoardSquares square, bool isWhite, ulong expectedResult)
        {
            // Act
            var attacks = AttacksGenerator.MaskPawnAttacks((int)square, isWhite);

            // Assert
            Assert.Equal(expectedResult, attacks.Board);
        }

        [Fact]
        public void InitializePawnAttacks()
        {
            // Act
            var result = AttacksGenerator.InitializePawnAttacks();

            // Assert
            foreach (var square in Enum.GetValues<BoardSquares>())
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
                Assert.Equal(expectedWhiteResult, result[1, intSquare].Board);

                var blackOffset = aFile
                    ? 9     // a7 -> b8 (1)
                    : 7;    // c7 -> b6 (101)

                var expectedBlackResult = attackDiagram << (intSquare + blackOffset);
                Assert.Equal(expectedBlackResult, result[0, intSquare].Board);
            }

            for (int square = 0; square < 8; ++square)
            {
                Assert.Equal(0UL, result[1, square].Board);
                Assert.Equal(0UL, result[0, 63 - square].Board);
            }
        }
    }
}
