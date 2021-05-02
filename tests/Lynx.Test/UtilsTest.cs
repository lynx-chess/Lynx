using Lynx.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lynx.Test
{
    public class UtilsTest
    {
        [Theory]
        [InlineData(Side.Black, 6)]
        [InlineData(Side.White, 0)]
        public void PieceOffSetBySide(Side sideToMove, int expectedOffset)
        {
            Assert.Equal(expectedOffset, Utils.PieceOffset(sideToMove));
            Assert.Equal(expectedOffset, Utils.PieceOffset((int)sideToMove));
        }

        [Theory]
        [InlineData(Side.Black, (int)Side.White)]
        [InlineData(Side.White, (int)Side.Black)]
        public void OppositeSide(Side sideToMove, int expectedSide)
        {
            Assert.Equal(expectedSide, Utils.OppositeSide(sideToMove));
        }

        [Theory]
        [InlineData(Side.White, Constants.WhiteShortCastleRookSquare)]
        [InlineData(Side.Black, Constants.BlackShortCastleRookSquare)]
        public void ShortCastleRookTargetSquare(Side sideToMove, int expectedRookSquare)
        {
            Assert.Equal(expectedRookSquare, Utils.ShortCastleRookTargetSquare(sideToMove));
            Assert.Equal(expectedRookSquare, Utils.ShortCastleRookTargetSquare((int)sideToMove));
        }

        [Theory]
        [InlineData(Side.White, Constants.WhiteLongCastleRookSquare)]
        [InlineData(Side.Black, Constants.BlackLongCastleRookSquare)]
        public void LongCastleRookTargetSquare(Side sideToMove, int expectedRookSquare)
        {
            Assert.Equal(expectedRookSquare, Utils.LongCastleRookTargetSquare(sideToMove));
            Assert.Equal(expectedRookSquare, Utils.LongCastleRookTargetSquare((int)sideToMove));
        }

        [Theory]
        [InlineData(Side.White, (int)BoardSquares.h1)]
        [InlineData(Side.Black, (int)BoardSquares.h8)]
        public void ShortCastleRookSourceSquare(Side sideToMove, int expectedRookSquare)
        {
            Assert.Equal(expectedRookSquare, Utils.ShortCastleRookSourceSquare(sideToMove));
            Assert.Equal(expectedRookSquare, Utils.ShortCastleRookSourceSquare((int)sideToMove));
        }

        [Theory]
        [InlineData(Side.White, (int)BoardSquares.a1)]
        [InlineData(Side.Black, (int)BoardSquares.a8)]
        public void LongCastleRookSourceSquare(Side sideToMove, int expectedRookSquare)
        {
            Assert.Equal(expectedRookSquare, Utils.LongCastleRookSourceSquare(sideToMove));
            Assert.Equal(expectedRookSquare, Utils.LongCastleRookSourceSquare((int)sideToMove));
        }
    }
}
