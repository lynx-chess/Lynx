using SharpFish.Model;
using Xunit;

namespace SharpFish.Test.PregeneratedAttacks
{
    public class IsAttackedSquare
    {
        private readonly Position _game;

        public IsAttackedSquare()
        {
            _game = new Position(Constants.EmptyBoardFEN);
        }

        [Fact]
        public void IsAttackedByBlackPawn()
        {
            const int square = (int)BoardSquares.e4;

            _game.PieceBitBoards[(int)Piece.p].SetBit(BoardSquares.d3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.p].SetBit(BoardSquares.d5);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.p].Clear();
        }

        [Fact]
        public void IsAttackedByWhitePawn()
        {
            const int square = (int)BoardSquares.e4;

            _game.PieceBitBoards[(int)Piece.P].SetBit(BoardSquares.d5);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.P].SetBit(BoardSquares.d3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.P].Clear();
        }

        [Fact]
        public void IsAttackedByBlackKnight()
        {
            const int square = (int)BoardSquares.e4;

            _game.PieceBitBoards[(int)Piece.n].SetBit(BoardSquares.e7);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.n].SetBit(BoardSquares.f6);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.n].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteKnight()
        {
            const int square = (int)BoardSquares.e4;

            _game.PieceBitBoards[(int)Piece.N].SetBit(BoardSquares.d3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.N].SetBit(BoardSquares.d2);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.N].Clear();
        }

        [Fact]
        public void IsAttackedByBlackBishop()
        {
            const int square = (int)BoardSquares.e5;

            _game.PieceBitBoards[(int)Piece.b].SetBit(BoardSquares.b7);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.b].SetBit(BoardSquares.b8);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.b].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteBishop()
        {
            const int square = (int)BoardSquares.e5;

            _game.PieceBitBoards[(int)Piece.B].SetBit(BoardSquares.a2);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.B].SetBit(BoardSquares.a1);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.B].Clear();
        }

        [Fact]
        public void IsAttackedByBlackRook()
        {
            const int square = (int)BoardSquares.e5;

            _game.PieceBitBoards[(int)Piece.r].SetBit(BoardSquares.d7);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.r].SetBit(BoardSquares.e8);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.r].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteRook()
        {
            const int square = (int)BoardSquares.e5;

            _game.PieceBitBoards[(int)Piece.R].SetBit(BoardSquares.d1);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.R].SetBit(BoardSquares.e1);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.R].Clear();
        }

        [Fact]
        public void IsAttackedByBlackQueen()
        {
            const int square1 = (int)BoardSquares.e5;
            const int square2 = (int)BoardSquares.a5;

            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.b7);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.d7);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.f7);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.h7);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.b3);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.d3);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.f3);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.h3);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.d1);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.c4);
            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.g4);
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.q].SetBit(BoardSquares.e1);
            Assert.True(Attacks.IsSquaredAttacked(square1, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square2, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.q].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteQueen()
        {
            const int square1 = (int)BoardSquares.e5;
            const int square2 = (int)BoardSquares.a5;

            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.b7);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.d7);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.f7);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.h7);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.b3);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.d3);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.f3);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.h3);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.d1);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.c4);
            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.g4);
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquares.e1);
            Assert.True(Attacks.IsSquaredAttacked(square1, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square2, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.Q].Clear();
        }

        [Fact]
        public void IsAttackedByBlackKing()
        {
            const int square = (int)BoardSquares.e5;

            _game.PieceBitBoards[(int)Piece.k].SetBit(BoardSquares.e3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.k].SetBit(BoardSquares.e4);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.k].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteKing()
        {
            const int square = (int)BoardSquares.e5;

            _game.PieceBitBoards[(int)Piece.K].SetBit(BoardSquares.e3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.K].SetBit(BoardSquares.e4);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _game.PieceBitBoards, _game.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _game.PieceBitBoards, _game.OccupancyBitBoards));

            _game.PieceBitBoards[(int)Piece.K].Clear();
        }
    }
}
