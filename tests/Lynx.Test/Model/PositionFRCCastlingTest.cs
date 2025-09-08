using Lynx.Model;
using NUnit.Framework;

using static Lynx.Model.BitBoardExtensions;

namespace Lynx.Test.Model;

[TestFixture(Category = Categories.PerftFRC, Explicit = true)]
public class PositionFRCCastlingTest
{
    public PositionFRCCastlingTest()
    {
        Configuration.EngineSettings.IsChess960 = true;
    }

    /// <summary>
    /// 8   . . . . r . k r
    /// 7   . . . . . . . .
    /// 6   p p p p p p p p
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   P P P P P P P P
    /// 2   . . . . . . . .
    /// 1   R K . . R . . .
    ///     a b c d e f g h
    /// </summary>
    [Test]
    public void NonAttackedSquares()
    {
        var position = new Position("4r1kr/8/pppppppp/8/8/PPPPPPPP/8/RK2R3 b KQkq - 0 1");
        Validate(position);
        Validate(position);

        static void Validate(Position position)
        {
            var whiteKing = position.InitialKingSquares[(int)Side.White];
            var blackKing = position.InitialKingSquares[(int)Side.Black];

            ulong blackNonAttackedKingsideMask = MaskBetweenTwoSquaresSameRankInclusive(blackKing, Constants.BlackKingKingsideCastlingSquare);
            Assert.AreEqual(blackNonAttackedKingsideMask, position.KingsideCastlingNonAttackedSquares[(int)Side.Black]);

            ulong blackNonAttackedQueensideMask = MaskBetweenTwoSquaresSameRankInclusive(blackKing, Constants.BlackKingQueensideCastlingSquare);
            Assert.AreEqual(blackNonAttackedQueensideMask, position.QueensideCastlingNonAttackedSquares[(int)Side.Black]);

            ulong whiteNonAttackedKingsideMask = MaskBetweenTwoSquaresSameRankInclusive(whiteKing, Constants.WhiteKingKingsideCastlingSquare);
            Assert.AreEqual(whiteNonAttackedKingsideMask, position.KingsideCastlingNonAttackedSquares[(int)Side.White]);

            ulong whiteNonAttackedQueensideMask = MaskBetweenTwoSquaresSameRankInclusive(whiteKing, Constants.WhiteKingQueensideCastlingSquare);
            Assert.AreEqual(whiteNonAttackedQueensideMask, position.QueensideCastlingNonAttackedSquares[(int)Side.White]);
        }
    }

    /// <summary>
    /// 8   . . . . r . k r
    /// 7   . . . . . . . .
    /// 6   p p p p p p p p
    /// 5   . . . . . . . .
    /// 4   . . . . . . . .
    /// 3   P P P P P P P P
    /// 2   . . . . . . . .
    /// 1   R K . . R . . .
    ///     a b c d e f g h
    /// </summary>
    [Test]
    public void FreeSquares()
    {
        var position = new Position("4r1kr/8/pppppppp/8/8/PPPPPPPP/8/RK2R3 b KQkq - 0 1");
        Validate(position);

        static void Validate(Position position)
        {
            var whiteKing = position.InitialKingSquares[(int)Side.White];
            var blackKing = position.InitialKingSquares[(int)Side.Black];

            var castlingRook = position.BlackShortCastle.TargetSquare();
            ulong blackFreeKingsideMask = MaskBetweenTwoSquaresSameRankInclusive(blackKing, Constants.BlackKingKingsideCastlingSquare)
                | MaskBetweenTwoSquaresSameRankInclusive(castlingRook, Constants.BlackRookKingsideCastlingSquare);
            blackFreeKingsideMask.PopBit(blackKing);
            blackFreeKingsideMask.PopBit(castlingRook);
            Assert.AreEqual(blackFreeKingsideMask, position.KingsideCastlingFreeSquares[(int)Side.Black]);

            castlingRook = position.BlackLongCastle.TargetSquare();
            ulong blackFreeQueensideMask = MaskBetweenTwoSquaresSameRankInclusive(blackKing, Constants.BlackKingQueensideCastlingSquare)
                | MaskBetweenTwoSquaresSameRankInclusive(castlingRook, Constants.BlackRookQueensideCastlingSquare);
            blackFreeQueensideMask.PopBit(blackKing);
            blackFreeQueensideMask.PopBit(castlingRook);
            Assert.AreEqual(blackFreeQueensideMask, position.QueensideCastlingFreeSquares[(int)Side.Black]);

            castlingRook = position.WhiteShortCastle.TargetSquare();
            ulong whiteFreeKingsideMask = MaskBetweenTwoSquaresSameRankInclusive(whiteKing, Constants.WhiteKingKingsideCastlingSquare)
                | MaskBetweenTwoSquaresSameRankInclusive(castlingRook, Constants.WhiteRookKingsideCastlingSquare);
            whiteFreeKingsideMask.PopBit(whiteKing);
            whiteFreeKingsideMask.PopBit(castlingRook);
            Assert.AreEqual(whiteFreeKingsideMask, position.KingsideCastlingFreeSquares[(int)Side.White]);

            castlingRook = position.WhiteLongCastle.TargetSquare();
            ulong whiteFreeQueensideMask = MaskBetweenTwoSquaresSameRankInclusive(whiteKing, Constants.WhiteKingQueensideCastlingSquare)
                | MaskBetweenTwoSquaresSameRankInclusive(castlingRook, Constants.WhiteRookQueensideCastlingSquare);
            whiteFreeQueensideMask.PopBit(whiteKing);
            whiteFreeQueensideMask.PopBit(castlingRook);
            Assert.AreEqual(whiteFreeQueensideMask, position.QueensideCastlingFreeSquares[(int)Side.White]);
        }
    }
}
