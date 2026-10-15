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

        position.PieceBitboards[(int)Piece.p].SetBit((int)BoardSquare.d3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.p].SetBit((int)BoardSquare.d5);
        Assert.True(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.p].Clear();
    }

    [Test]
    public void IsAttackedByWhitePawn()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e4;

        position.PieceBitboards[(int)Piece.P].SetBit((int)BoardSquare.d5);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.P].SetBit((int)BoardSquare.d3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.True(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.P].Clear();
    }

    [Test]
    public void IsAttackedByBlackKnight()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e4;

        position.PieceBitboards[(int)Piece.n].SetBit((int)BoardSquare.e7);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.n].SetBit((int)BoardSquare.f6);
        Assert.True(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.n].Clear();
    }

    [Test]
    public void IsAttackedByWhiteKnight()
    {
        var position = new Position(ValidFEN);

        const int square = (int)BoardSquare.e4;

        position.PieceBitboards[(int)Piece.N].SetBit((int)BoardSquare.d3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.N].SetBit((int)BoardSquare.d2);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.True(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.N].Clear();
    }

    [Test]
    public void IsAttackedByBlackBishop()
    {
        var position = new Position(ValidFEN);

        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.b].SetBit((int)BoardSquare.b7);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.b].SetBit((int)BoardSquare.b8);
        Assert.True(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.b].Clear();
    }

    [Test]
    public void IsAttackedByWhiteBishop()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.B].SetBit((int)BoardSquare.a2);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.B].SetBit((int)BoardSquare.a1);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.True(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.B].Clear();
    }

    [Test]
    public void IsAttackedByBlackRook()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.r].SetBit((int)BoardSquare.d7);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.r].SetBit((int)BoardSquare.e8);
        Assert.True(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.r].Clear();
    }

    [Test]
    public void IsAttackedByWhiteRook()
    {
        var position = new Position(ValidFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitboards[(int)Piece.R].SetBit((int)BoardSquare.d1);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.R].SetBit((int)BoardSquare.e1);
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

        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.b7);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.d7);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.f7);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.h7);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.b3);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.d3);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.f3);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.h3);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.d1);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.c4);
        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.g4);
        Assert.False(position.IsSquareAttacked(square1, Side.Black));
        Assert.False(position.IsSquareAttacked(square1, Side.White));
        Assert.False(position.IsSquareAttacked(square2, Side.Black));
        Assert.False(position.IsSquareAttacked(square2, Side.White));

        position.PieceBitboards[(int)Piece.q].SetBit((int)BoardSquare.e1);
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

        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.b7);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.d7);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.f7);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.h7);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.b3);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.d3);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.f3);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.h3);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.d1);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.c4);
        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.g4);
        Assert.False(position.IsSquareAttacked(square1, Side.Black));
        Assert.False(position.IsSquareAttacked(square1, Side.White));
        Assert.False(position.IsSquareAttacked(square2, Side.Black));
        Assert.False(position.IsSquareAttacked(square2, Side.White));

        position.PieceBitboards[(int)Piece.Q].SetBit((int)BoardSquare.e1);
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
        position.PieceBitboards[(int)Piece.k].SetBit((int)BoardSquare.e3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.k] = 0;
        position.PieceBitboards[(int)Piece.k].SetBit((int)BoardSquare.e4);
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
        position.PieceBitboards[(int)Piece.K].SetBit((int)BoardSquare.e3);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.False(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.k] = 0;
        position.PieceBitboards[(int)Piece.K].SetBit((int)BoardSquare.e4);
        Assert.False(position.IsSquareAttacked(square, Side.Black));
        Assert.True(position.IsSquareAttacked(square, Side.White));

        position.PieceBitboards[(int)Piece.K].Clear();
    }
}
