using Lynx.Model;
using System;
using Xunit;

namespace Lynx.Test
{
    public class BitBoardTest
    {
        [Fact]
        public void GetBit()
        {
            foreach (var square in Enum.GetValues<BoardSquare>())
            {
                var bitBoard = new BitBoard(1UL << (int)square);

                Assert.True(bitBoard.GetBit(square));
            }
        }

        [Fact]
        public void SetBit()
        {
            foreach (var square in Enum.GetValues<BoardSquare>())
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
            foreach (var square in Enum.GetValues<BoardSquare>())
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
            Assert.True(bitBoard.Empty);

            bitBoard.SetBit(BoardSquare.e4);
            Assert.False(bitBoard.Empty);
        }

        [Fact]
        public void IsSinglePopulated()
        {
            var bitBoard = new BitBoard();
            Assert.False(bitBoard.IsSinglePopulated());

            bitBoard.SetBit(BoardSquare.e4);
            Assert.True(bitBoard.IsSinglePopulated());

            bitBoard.SetBit(BoardSquare.e5);
            Assert.False(bitBoard.IsSinglePopulated());
        }

        [Fact]
        public void CountBits()
        {
            var bitBoard = new BitBoard();
            Assert.Equal(0, bitBoard.CountBits());

            bitBoard.SetBit(BoardSquare.e4);
            Assert.Equal(1, bitBoard.CountBits());

            bitBoard.SetBit(BoardSquare.e4);
            Assert.Equal(1, bitBoard.CountBits());

            bitBoard.SetBit(BoardSquare.d4);
            Assert.Equal(2, bitBoard.CountBits());

            bitBoard.PopBit(BoardSquare.d4);
            Assert.Equal(1, bitBoard.CountBits());
        }

        [Fact]
        public void CountBits_ulong()
        {
            var bitBoard = new BitBoard();
            Assert.Equal(0, BitBoard.CountBits(bitBoard.Board));

            bitBoard.SetBit(BoardSquare.e4);
            Assert.Equal(1, BitBoard.CountBits(bitBoard.Board));

            bitBoard.SetBit(BoardSquare.e4);
            Assert.Equal(1, BitBoard.CountBits(bitBoard.Board));

            bitBoard.SetBit(BoardSquare.d4);
            Assert.Equal(2, BitBoard.CountBits(bitBoard.Board));

            bitBoard.PopBit(BoardSquare.d4);
            Assert.Equal(1, BitBoard.CountBits(bitBoard.Board));
        }

        [Fact]
        public void ResetLS1B()
        {
            // Arrange
            BitBoard bitBoard = new(new[] { BoardSquare.d5, BoardSquare.e4 });

            // Act
            bitBoard.ResetLS1B();

            // Assert
            Assert.True(bitBoard.GetBit(BoardSquare.e4));
            Assert.False(bitBoard.GetBit(BoardSquare.d5));
        }

        [Fact]
        public void ResetLS1B_ulong()
        {
            // Arrange
            BitBoard bitBoard = new(new[] { BoardSquare.d5, BoardSquare.e4 });

            // Act
            var result = new BitBoard(BitBoard.ResetLS1B(bitBoard.Board));

            // Assert
            Assert.True(bitBoard.GetBit(BoardSquare.e4));
            Assert.True(bitBoard.GetBit(BoardSquare.d5));

            Assert.True(result.GetBit(BoardSquare.e4));
            Assert.False(result.GetBit(BoardSquare.d5));
        }

        [Theory]
        [InlineData(new BoardSquare[] { }, -1)]
        [InlineData(new BoardSquare[] { BoardSquare.e4 }, (int)BoardSquare.e4)]
        [InlineData(new BoardSquare[] { BoardSquare.a8 }, (int)BoardSquare.a8)]
        [InlineData(new BoardSquare[] { BoardSquare.h1 }, (int)BoardSquare.h1)]
        [InlineData(new BoardSquare[] { BoardSquare.a8, BoardSquare.h1 }, (int)BoardSquare.a8)]
        [InlineData(new BoardSquare[] { BoardSquare.d5, BoardSquare.e4 }, (int)BoardSquare.d5)]
        [InlineData(new BoardSquare[] { BoardSquare.e4, BoardSquare.f4 }, (int)BoardSquare.e4)]
        public void GetLS1BIndex(BoardSquare[] occupiedSquares, int expectedLS1B)
        {
            var bitboard = new BitBoard(occupiedSquares);
            Assert.Equal(expectedLS1B, bitboard.GetLS1BIndex());
            Assert.Equal(expectedLS1B, BitBoard.GetLS1BIndex(bitboard.Board));

            if (expectedLS1B != -1)
            {
                Assert.Equal(((BoardSquare)expectedLS1B).ToString(), Constants.Coordinates[expectedLS1B]);
            }
        }
    }
}
