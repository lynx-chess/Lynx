using SharpFish.Model;
using System;
using Xunit;

namespace SharpFish.Test
{
    public class BitBoardTest
    {
        [Fact]
        public void GetBit()
        {
            foreach (var square in Enum.GetValues<BoardSquares>())
            {
                var bitBoard = new BitBoard(1UL << (int)square);

                Assert.True(bitBoard.GetBit(square));
            }
        }

        [Fact]
        public void SetBit()
        {
            foreach (var square in Enum.GetValues<BoardSquares>())
            {
                var bitBoard = new BitBoard();

                bitBoard.SetBit(square);
                Assert.True(bitBoard.GetBit(square));

                // Making sure that setting it again doesn't flipt it
                bitBoard.SetBit(square);
                Assert.True(bitBoard.GetBit(square));
            }
        }

        [Fact]
        public void PopBit()
        {
            foreach (var square in Enum.GetValues<BoardSquares>())
            {
                var bitBoard = new BitBoard();
                bitBoard.SetBit(square);

                Assert.True(bitBoard.GetBit(square));

                bitBoard.PopBit(square);
                Assert.False(bitBoard.GetBit(square));

                // Making sure that popping it again doesn't flipt it
                bitBoard.PopBit(square);
                Assert.False(bitBoard.GetBit(square));
            }
        }

        [Fact]
        public void Empty()
        {
            var bitBoard = new BitBoard();
            Assert.True(bitBoard.Empty());

            bitBoard.SetBit(BoardSquares.e4);
            Assert.False(bitBoard.Empty());
        }

        [Fact]
        public void IsSinglePopulated()
        {
            var bitBoard = new BitBoard();
            Assert.False(bitBoard.IsSinglePopulated());

            bitBoard.SetBit(BoardSquares.e4);
            Assert.True(bitBoard.IsSinglePopulated());

            bitBoard.SetBit(BoardSquares.e5);
            Assert.False(bitBoard.IsSinglePopulated());
        }
    }
}
