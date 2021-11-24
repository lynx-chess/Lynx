﻿using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.PregeneratedAttacks;

public class IsAttackedSquareTest
{
    [Test]
    public void IsAttackedByBlackPawn()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e4;

        position.PieceBitBoards[(int)Piece.p].SetBit(BoardSquare.d3);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.p].SetBit(BoardSquare.d5);
        Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.p].Clear();
    }

    [Test]
    public void IsAttackedByWhitePawn()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e4;

        position.PieceBitBoards[(int)Piece.P].SetBit(BoardSquare.d5);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.P].SetBit(BoardSquare.d3);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.True(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.P].Clear();
    }

    [Test]
    public void IsAttackedByBlackKnight()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e4;

        position.PieceBitBoards[(int)Piece.n].SetBit(BoardSquare.e7);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.n].SetBit(BoardSquare.f6);
        Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.n].Clear();
    }

    [Test]
    public void IsAttackedByWhiteKnight()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e4;

        position.PieceBitBoards[(int)Piece.N].SetBit(BoardSquare.d3);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.N].SetBit(BoardSquare.d2);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.True(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.N].Clear();
    }

    [Test]
    public void IsAttackedByBlackBishop()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitBoards[(int)Piece.b].SetBit(BoardSquare.b7);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.b].SetBit(BoardSquare.b8);
        Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.b].Clear();
    }

    [Test]
    public void IsAttackedByWhiteBishop()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitBoards[(int)Piece.B].SetBit(BoardSquare.a2);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.B].SetBit(BoardSquare.a1);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.True(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.B].Clear();
    }

    [Test]
    public void IsAttackedByBlackRook()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitBoards[(int)Piece.r].SetBit(BoardSquare.d7);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.r].SetBit(BoardSquare.e8);
        Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.r].Clear();
    }

    [Test]
    public void IsAttackedByWhiteRook()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitBoards[(int)Piece.R].SetBit(BoardSquare.d1);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.R].SetBit(BoardSquare.e1);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.True(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.R].Clear();
    }

    [Test]
    public void IsAttackedByBlackQueen()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square1 = (int)BoardSquare.e5;
        const int square2 = (int)BoardSquare.a5;

        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.b7);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.d7);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.f7);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.h7);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.b3);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.d3);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.f3);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.h3);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.d1);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.c4);
        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.g4);
        Assert.False(Attacks.IsSquaredAttacked(square1, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square1, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square2, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square2, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.q].SetBit(BoardSquare.e1);
        Assert.True(Attacks.IsSquaredAttacked(square1, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square1, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.True(Attacks.IsSquaredAttacked(square2, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square2, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.q].Clear();
    }

    [Test]
    public void IsAttackedByWhiteQueen()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square1 = (int)BoardSquare.e5;
        const int square2 = (int)BoardSquare.a5;

        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.b7);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.d7);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.f7);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.h7);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.b3);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.d3);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.f3);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.h3);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.d1);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.c4);
        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.g4);
        Assert.False(Attacks.IsSquaredAttacked(square1, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square1, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square2, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square2, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.Q].SetBit(BoardSquare.e1);
        Assert.True(Attacks.IsSquaredAttacked(square1, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square1, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.True(Attacks.IsSquaredAttacked(square2, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square2, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.Q].Clear();
    }

    [Test]
    public void IsAttackedByBlackKing()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitBoards[(int)Piece.k].SetBit(BoardSquare.e3);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.k].SetBit(BoardSquare.e4);
        Assert.True(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.k].Clear();
    }

    [Test]
    public void IsAttackedByWhiteKing()
    {
        var position = new Position(Constants.EmptyBoardFEN);
        const int square = (int)BoardSquare.e5;

        position.PieceBitBoards[(int)Piece.K].SetBit(BoardSquare.e3);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.False(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.K].SetBit(BoardSquare.e4);
        Assert.False(Attacks.IsSquaredAttacked(square, Side.Black, position.PieceBitBoards, position.OccupancyBitBoards));
        Assert.True(Attacks.IsSquaredAttacked(square, Side.White, position.PieceBitBoards, position.OccupancyBitBoards));

        position.PieceBitBoards[(int)Piece.K].Clear();
    }
}
