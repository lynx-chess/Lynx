using SharpFish.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpFish.Test
{
    public class UtilsTest
    {
        [Theory]
        [InlineData(Side.Black, 6)]
        [InlineData(Side.White, 0)]
        public void PieceOffSetBySide(Side sideToMove, int expectedOffset)
        {
            Assert.Equal(expectedOffset, Utils.PieceOffset(sideToMove));
        }

        [Theory]
        [InlineData(Side.Black, (int)Side.White)]
        [InlineData(Side.White, (int)Side.Black)]
        public void OppositeSide(Side sideToMove, int expectedSide)
        {
            Assert.Equal(expectedSide, Utils.OppositeSide(sideToMove));
        }
    }
}
