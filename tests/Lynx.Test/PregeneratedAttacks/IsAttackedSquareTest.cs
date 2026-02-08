using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.PregeneratedAttacks;

public class IsAttackedSquareTest
{
    private const string ValidFEN = "K1k5/8/8/8/8/8/8/8 w - - 0 1";

    [Test]
    public void IsAttackedByBlackPawn()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e4;

        position.PieceBitboards[(int)Piece.p].SetBit(BoardSquare.d3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.p].SetBit(BoardSquare.d5);
        Assert.True(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.p].Clear();
    }

    [Test]
    public void IsAttackedByWhitePawn()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e4;

        position.PieceBitboards[(int)Piece.P].SetBit(BoardSquare.d5);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.P].SetBit(BoardSquare.d3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.True(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.P].Clear();
    }

    [Test]
    public void IsAttackedByBlackKnight()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e4;

        position.PieceBitboards[(int)Piece.n].SetBit(BoardSquare.e7);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.n].SetBit(BoardSquare.f6);
        Assert.True(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.n].Clear();
    }

    [Test]
    public void IsAttackedByWhiteKnight()
    {
        var position = new Position(ValidFEN);

        const int square = (int)BoardSquare.e4;

        position.PieceBitboards[(int)Piece.N].SetBit(BoardSquare.d3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.N].SetBit(BoardSquare.d2);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.True(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.N].Clear();
    }

    [Test]
    public void IsAttackedByBlackBishop()
    {
        var position = new Position(ValidFEN);

        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.b].SetBit(BoardSquare.b7);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.b].SetBit(BoardSquare.b8);
        Assert.True(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.b].Clear();
    }

    [Test]
    public void IsAttackedByWhiteBishop()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.B].SetBit(BoardSquare.a2);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.B].SetBit(BoardSquare.a1);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.True(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.B].Clear();
    }

    [Test]
    public void IsAttackedByBlackRook()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.r].SetBit(BoardSquare.d7);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.r].SetBit(BoardSquare.e8);
        Assert.True(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.r].Clear();
    }

    [Test]
    public void IsAttackedByWhiteRook()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.R].SetBit(BoardSquare.d1);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.R].SetBit(BoardSquare.e1);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.True(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.R].Clear();
    }

    [Test]
    public void IsAttackedByBlackQueen()
    {
        var position = new Position(ValidFEN);
        const int square1 = (int)BoardSquare.e5;
        const int square2 = (int)BoardSquare.a5;

        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.b7);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.d7);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.f7);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.h7);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.b3);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.d3);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.f3);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.h3);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.d1);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.c4);
        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.g4);
        Assert.False(position.IsSquareAttacked(square1, Side.Black));
        Assert.False(position.IsSquareAttacked(square1, Side.White));
        Assert.False(position.IsSquareAttacked(square2, Side.Black));
        Assert.False(position.IsSquareAttacked(square2, Side.White));

        position.PieceBitboards[(int)Piece.q].SetBit(BoardSquare.e1);
        Assert.True(position.IsSquareAttacked(square1, Side.Black));
        Assert.False(position.IsSquareAttacked(square1, Side.White));
        Assert.True(position.IsSquareAttacked(square2, Side.Black));
        Assert.False(position.IsSquareAttacked(square2, Side.White));

        position.PieceBitboards[(int)Piece.q].Clear();
    }

    [Test]
    public void IsAttackedByWhiteQueen()
    {
        var position = new Position(ValidFEN);
        const int square1 = (int)BoardSquare.e5;
        const int square2 = (int)BoardSquare.a5;

        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.b7);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.d7);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.f7);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.h7);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.b3);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.d3);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.f3);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.h3);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.d1);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.c4);
        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.g4);
        Assert.False(position.IsSquareAttacked(square1, Side.Black));
        Assert.False(position.IsSquareAttacked(square1, Side.White));
        Assert.False(position.IsSquareAttacked(square2, Side.Black));
        Assert.False(position.IsSquareAttacked(square2, Side.White));

        position.PieceBitboards[(int)Piece.Q].SetBit(BoardSquare.e1);
        Assert.True(position.IsSquareAttacked(square1, Side.White));
        Assert.False(position.IsSquareAttacked(square1, Side.Black));
        Assert.True(position.IsSquareAttacked(square2, Side.White));
        Assert.False(position.IsSquareAttacked(square2, Side.Black));

        position.PieceBitboards[(int)Piece.Q].Clear();
    }

    [Test]
    public void IsAttackedByBlackKing()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.k] = 0;
        position.PieceBitboards[(int)Piece.k].SetBit(BoardSquare.e3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.k] = 0;
        position.PieceBitboards[(int)Piece.k].SetBit(BoardSquare.e4);
        Assert.True(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.k].Clear();
    }

    [Test]
    public void IsAttackedByWhiteKing()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.K] = 0;
        position.PieceBitboards[(int)Piece.K].SetBit(BoardSquare.e3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.k] = 0;
        position.PieceBitboards[(int)Piece.K].SetBit(BoardSquare.e4);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.True(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.K].Clear();
    }
}
