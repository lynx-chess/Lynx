using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.Model;
public class MoveToEPDStringTest
{
    [TestCase("d5", (int)BoardSquare.d4, (int)BoardSquare.d5, (int)Piece.P, default, 0)]
    [TestCase("d4", (int)BoardSquare.d2, (int)BoardSquare.d4, (int)Piece.P, 0, 0, 1)]
    [TestCase("d8=Q", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.Q)]
    [TestCase("d8=N", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.N)]
    [TestCase("d8=B", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.B)]
    [TestCase("d8=R", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.R)]
    [TestCase("dxd8=R", (int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, (int)Piece.R, 1)]
    [TestCase("dxe5", (int)BoardSquare.d4, (int)BoardSquare.e5, (int)Piece.P, default, 1)]
    [TestCase("Bxe5", (int)BoardSquare.d4, (int)BoardSquare.e5, (int)Piece.B, default, 1)]
    [TestCase("Nxe5", (int)BoardSquare.d3, (int)BoardSquare.e5, (int)Piece.N, default, 1)]
    [TestCase("dxe7e.p.", (int)BoardSquare.d6, (int)BoardSquare.e7, (int)Piece.P, default, 1, default, 1)]
    public void ToEPDString(string expectedString, int sourceSquare, int targetSquare, int piece,
        int promotedPiece = default,
        int isCapture = default, int isDoublePawnPush = default, int isEnPassant = default,
        int isShortCastle = default, int isLongCastle = default)
    {
        int move = 0;

        if (isShortCastle != default)
        {
            move = MoveExtensions.EncodeShortCastle(sourceSquare, targetSquare, piece);
        }
        else if (isLongCastle != default)
        {
            move = MoveExtensions.EncodeLongCastle(sourceSquare, targetSquare, piece);
        }
        else
        {
            if (isEnPassant != default)
            {
                move = MoveExtensions.EncodeEnPassant(sourceSquare, targetSquare, piece);
            }
            else if (isDoublePawnPush != default)
            {
                move = MoveExtensions.EncodeDoublePawnPush(sourceSquare, targetSquare, piece);
            }
            else if (promotedPiece != default)
            {
                if (isCapture != default)
                {
                    move = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece, capturedPiece: 1);
                }
                else
                {
                    move = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece);
                }
            }
            else if (isCapture != default)
            {
                move = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece: 1);
            }
            else
            {
                move = MoveExtensions.Encode(sourceSquare, targetSquare, piece);
            }
        }

        Assert.AreEqual(expectedString, move.ToEPDString());
    }
}
