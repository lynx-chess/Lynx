﻿using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.MoveGeneration;

public class EncodeDecodeMoveTest
{
    [Test]
    public void SpecialMoveTypeValues()
    {
        foreach (var value in Enum.GetValues(typeof(SpecialMoveType)).Cast<int>())
        {
            Assert.LessOrEqual(value, 0b111, $"Need to change {MoveExtensions.SpecialMoveFlag} mask");
            Assert.LessOrEqual(value, (int)SpecialMoveType.LongCastle, $"Need to change {MoveExtensions.IsCastle}");
        }

        Assert.Greater((int)SpecialMoveType.LongCastle, (int)SpecialMoveType.ShortCastle, $"Need to change {MoveExtensions.IsCastle}");
    }

    [TestCase(BoardSquare.e4, BoardSquare.e5, Piece.P, false)]
    [TestCase(BoardSquare.e4, BoardSquare.d5, Piece.P, true)]
    [TestCase(BoardSquare.e5, BoardSquare.e4, Piece.p, false)]
    [TestCase(BoardSquare.e5, BoardSquare.d4, Piece.p, true)]
    [TestCase(BoardSquare.a1, BoardSquare.a4, Piece.R, false)]
    [TestCase(BoardSquare.a1, BoardSquare.a4, Piece.R, true)]
    [TestCase(BoardSquare.a1, BoardSquare.a4, Piece.r, false)]
    [TestCase(BoardSquare.a1, BoardSquare.a4, Piece.r, true)]
    [TestCase(BoardSquare.a1, BoardSquare.h8, Piece.B, false)]
    [TestCase(BoardSquare.a1, BoardSquare.h8, Piece.B, true)]
    [TestCase(BoardSquare.a1, BoardSquare.h8, Piece.b, false)]
    [TestCase(BoardSquare.a1, BoardSquare.h8, Piece.b, true)]
    [TestCase(BoardSquare.a1, BoardSquare.h8, Piece.Q, false)]
    [TestCase(BoardSquare.a1, BoardSquare.h8, Piece.Q, true)]
    [TestCase(BoardSquare.a1, BoardSquare.h8, Piece.q, false)]
    [TestCase(BoardSquare.a1, BoardSquare.h8, Piece.q, true)]
    [TestCase(BoardSquare.b1, BoardSquare.c3, Piece.N, true)]
    [TestCase(BoardSquare.b1, BoardSquare.c3, Piece.N, false)]
    [TestCase(BoardSquare.b8, BoardSquare.c6, Piece.n, true)]
    [TestCase(BoardSquare.b8, BoardSquare.c6, Piece.n, false)]
    public void SourceSquare_TargetSquare_Piece_Capture(BoardSquare sourceSquare, BoardSquare targetSquare, Piece piece, bool isCapture)
    {
        var move = isCapture
            ? MoveExtensions.EncodeCapture((int)sourceSquare, (int)targetSquare, (int)piece, capturedPiece: 1)
            : MoveExtensions.Encode((int)sourceSquare, (int)targetSquare, (int)piece);

        Assert.AreEqual((int)sourceSquare, move.SourceSquare());
        Assert.AreEqual((int)targetSquare, move.TargetSquare());
        Assert.AreEqual((int)piece, move.Piece());
        Assert.AreEqual(isCapture, move.IsCapture());

        Assert.AreEqual(default(int), move.PromotedPiece());
        Assert.False(move.IsEnPassant());
        Assert.False(move.IsCastle());
        Assert.False(move.IsShortCastle());
        Assert.False(move.IsLongCastle());
        Assert.False(move.IsPromotion());
    }

    [TestCase(BoardSquare.a7, BoardSquare.a8, Piece.Q)]
    [TestCase(BoardSquare.a7, BoardSquare.a8, Piece.R)]
    [TestCase(BoardSquare.a7, BoardSquare.a8, Piece.B)]
    [TestCase(BoardSquare.a7, BoardSquare.a8, Piece.N)]
    [TestCase(BoardSquare.a2, BoardSquare.a1, Piece.q)]
    [TestCase(BoardSquare.a2, BoardSquare.a1, Piece.r)]
    [TestCase(BoardSquare.a2, BoardSquare.a1, Piece.b)]
    [TestCase(BoardSquare.a2, BoardSquare.a1, Piece.n)]
    public void Promotion(BoardSquare sourceSquare, BoardSquare targetSquare, Piece promotedPiece)
    {
        var move = MoveExtensions.EncodePromotion((int)sourceSquare, (int)targetSquare, (int)Piece.P, promotedPiece: (int)promotedPiece);

        Assert.AreEqual((int)sourceSquare, move.SourceSquare());
        Assert.AreEqual((int)targetSquare, move.TargetSquare());
        Assert.AreEqual((int)promotedPiece, move.PromotedPiece());
        Assert.True(move.IsPromotion());
        Assert.False(move.IsCapture());
        Assert.False(move.IsEnPassant());
    }

    [TestCase(BoardSquare.a7, BoardSquare.a8, Piece.Q, Piece.q)]
    [TestCase(BoardSquare.a7, BoardSquare.a8, Piece.R, Piece.q)]
    [TestCase(BoardSquare.a7, BoardSquare.a8, Piece.B, Piece.q)]
    [TestCase(BoardSquare.a7, BoardSquare.a8, Piece.N, Piece.q)]
    [TestCase(BoardSquare.a2, BoardSquare.a1, Piece.q, Piece.Q)]
    [TestCase(BoardSquare.a2, BoardSquare.a1, Piece.r, Piece.Q)]
    [TestCase(BoardSquare.a2, BoardSquare.a1, Piece.b, Piece.Q)]
    [TestCase(BoardSquare.a2, BoardSquare.a1, Piece.n, Piece.Q)]
    public void PromotionWithCapture(BoardSquare sourceSquare, BoardSquare targetSquare, Piece promotedPiece, Piece capturedPiece)
    {
        var move = MoveExtensions.EncodePromotion((int)sourceSquare, (int)targetSquare, (int)Piece.P, promotedPiece: (int)promotedPiece, capturedPiece: (int)capturedPiece);

        Assert.AreEqual((int)sourceSquare, move.SourceSquare());
        Assert.AreEqual((int)targetSquare, move.TargetSquare());
        Assert.AreEqual((int)promotedPiece, move.PromotedPiece());
        Assert.AreEqual((int)capturedPiece, move.CapturedPiece());
        Assert.True(move.IsCapture());
        Assert.True(move.IsPromotion());
        Assert.False(move.IsEnPassant());
    }

    [TestCase(BoardSquare.e5, BoardSquare.f6, false)]
    [TestCase(BoardSquare.e5, BoardSquare.f6, true)]
    [TestCase(BoardSquare.e4, BoardSquare.d3, false)]
    [TestCase(BoardSquare.e4, BoardSquare.d3, true)]
    public void EnPassant(BoardSquare sourceSquare, BoardSquare targetSquare, bool enPassant)
    {
        var move = enPassant
            ? MoveExtensions.EncodeEnPassant((int)sourceSquare, (int)targetSquare, (int)Piece.P)
            : MoveExtensions.Encode((int)sourceSquare, (int)targetSquare, (int)Piece.P);

        Assert.AreEqual((int)sourceSquare, move.SourceSquare());
        Assert.AreEqual((int)targetSquare, move.TargetSquare());
        Assert.AreEqual(enPassant, move.IsEnPassant());
    }

    [TestCase(BoardSquare.e1, BoardSquare.g1, true, false)]
    [TestCase(BoardSquare.e1, BoardSquare.c1, false, true)]
    [TestCase(BoardSquare.e8, BoardSquare.g8, true, false)]
    [TestCase(BoardSquare.e8, BoardSquare.c8, false, true)]
    [TestCase(BoardSquare.e1, BoardSquare.e2, false, false)]
    [TestCase(BoardSquare.e8, BoardSquare.e7, false, false)]
    public void Castling(BoardSquare sourceSquare, BoardSquare targetSquare, bool isShortCastle, bool isLongCastle)
    {
#pragma warning disable S3358 // Ternary operators should not be nested
        var move = isShortCastle
            ? MoveExtensions.EncodeShortCastle((int)sourceSquare, (int)targetSquare, (int)Piece.K)
            : (isLongCastle
                ? MoveExtensions.EncodeLongCastle((int)sourceSquare, (int)targetSquare, (int)Piece.K)
                : MoveExtensions.Encode((int)sourceSquare, (int)targetSquare, (int)Piece.K));
#pragma warning restore S3358 // Ternary operators should not be nested

        Assert.AreEqual((int)sourceSquare, move.SourceSquare());
        Assert.AreEqual((int)targetSquare, move.TargetSquare());

        Assert.AreEqual(isShortCastle, move.IsShortCastle());
        Assert.AreEqual(isLongCastle, move.IsLongCastle());
        Assert.AreEqual(isShortCastle || isLongCastle, move.IsCastle());
        Assert.False(move.IsPromotion());
        Assert.False(move.IsCapture());
        Assert.False(move.IsEnPassant());
    }

    [TestCase(BoardSquare.g2, BoardSquare.g4)]
    [TestCase(BoardSquare.b2, BoardSquare.b4)]
    [TestCase(BoardSquare.b7, BoardSquare.b5)]
    [TestCase(BoardSquare.g7, BoardSquare.g5)]
    public void DoublePawnPush(BoardSquare sourceSquare, BoardSquare targetSquare)
    {
        var move = MoveExtensions.EncodeDoublePawnPush((int)sourceSquare, (int)targetSquare, (int)Piece.P);

        Assert.AreEqual((int)sourceSquare, move.SourceSquare());
        Assert.AreEqual((int)targetSquare, move.TargetSquare());
        Assert.True(move.IsDoublePawnPush());
        Assert.False(move.IsPromotion());
        Assert.False(move.IsCapture());
        Assert.False(move.IsEnPassant());
    }
}
