using Lynx.Model;
using Xunit;

namespace Lynx.Test.PregeneratedAttacks
{
    public class IsAttackedSquare
    {
        private readonly Position _position;

        public IsAttackedSquare()
        {
            _position = new Position(Constants.EmptyBoardFEN);
        }

        [Fact]
        public void IsAttackedByBlackPawn()
        {
            const int square = (int)BoardSquare.e4;

            _position.PieceBitBoards[(int)Piece.p].SetBit(BoardSquare.d3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.p].SetBit(BoardSquare.d5);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.p].Clear();
        }

        [Fact]
        public void IsAttackedByWhitePawn()
        {
            const int square = (int)BoardSquare.e4;

            _position.PieceBitBoards[(int)Piece.P].SetBit(BoardSquare.d5);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.P].SetBit(BoardSquare.d3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.P].Clear();
        }

        [Fact]
        public void IsAttackedByBlackKnight()
        {
            const int square = (int)BoardSquare.e4;

            _position.PieceBitBoards[(int)Piece.n].SetBit(BoardSquare.e7);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.n].SetBit(BoardSquare.f6);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.n].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteKnight()
        {
            const int square = (int)BoardSquare.e4;

            _position.PieceBitBoards[(int)Piece.N].SetBit(BoardSquare.d3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.N].SetBit(BoardSquare.d2);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.N].Clear();
        }

        [Fact]
        public void IsAttackedByBlackBishop()
        {
            const int square = (int)BoardSquare.e5;

            _position.PieceBitBoards[(int)Piece.b].SetBit(BoardSquare.b7);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.b].SetBit(BoardSquare.b8);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.b].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteBishop()
        {
            const int square = (int)BoardSquare.e5;

            _position.PieceBitBoards[(int)Piece.B].SetBit(BoardSquare.a2);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.B].SetBit(BoardSquare.a1);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.B].Clear();
        }

        [Fact]
        public void IsAttackedByBlackRook()
        {
            const int square = (int)BoardSquare.e5;

            _position.PieceBitBoards[(int)Piece.r].SetBit(BoardSquare.d7);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.r].SetBit(BoardSquare.e8);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.r].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteRook()
        {
            const int square = (int)BoardSquare.e5;

            _position.PieceBitBoards[(int)Piece.R].SetBit(BoardSquare.d1);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.R].SetBit(BoardSquare.e1);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.R].Clear();
        }

        [Fact]
        public void IsAttackedByBlackQueen()
        {
            const int square1 = (int)BoardSquare.e5;
            const int square2 = (int)BoardSquare.a5;

            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.b7);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.d7);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.f7);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.h7);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.b3);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.d3);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.f3);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.h3);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.d1);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.c4);
            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.g4);
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.e1);
            Assert.True(Attacks.IsSquaredAttacked(square1, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square2, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.q].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteQueen()
        {
            const int square1 = (int)BoardSquare.e5;
            const int square2 = (int)BoardSquare.a5;

            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.b7);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.d7);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.f7);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.h7);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.b3);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.d3);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.f3);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.h3);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.d1);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.c4);
            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.g4);
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.e1);
            Assert.True(Attacks.IsSquaredAttacked(square1, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square1, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square2, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square2, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.Q].Clear();
        }

        [Fact]
        public void IsAttackedByBlackKing()
        {
            const int square = (int)BoardSquare.e5;

            _position.PieceBitBoards[(int)Piece.k].SetBit(BoardSquare.e3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.k].SetBit(BoardSquare.e4);
            Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.k].Clear();
        }

        [Fact]
        public void IsAttackedByWhiteKing()
        {
            const int square = (int)BoardSquare.e5;

            _position.PieceBitBoards[(int)Piece.K].SetBit(BoardSquare.e3);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.False(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.K].SetBit(BoardSquare.e4);
            Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, _position.PieceBitBoards, _position.OccupancyBitBoards));
            Assert.True(Attacks.IsSquaredAttacked(square, Side.White, _position.PieceBitBoards, _position.OccupancyBitBoards));

            _position.PieceBitBoards[(int)Piece.K].Clear();
        }
    }
}
