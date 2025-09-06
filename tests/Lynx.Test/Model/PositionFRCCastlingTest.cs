using Lynx.Model;
using NUnit.Framework;
using System.Runtime.CompilerServices;

namespace Lynx.Test.Model;

public class PositionFRCCastlingTest
{
    public PositionFRCCastlingTest()
    {
        Configuration.EngineSettings.IsChess960 = true;
        UpdateCurrentInstance();
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
            var startingSquare = position.InitialKingSquares[(int)position.Side];

            ulong nonAttackedKingsideMask = ExtractNonAttackedMask(startingSquare, Constants.BlackShortCastleKingSquare);
            Assert.AreEqual(nonAttackedKingsideMask, position.KingsideCastlingNonAttackedSquares[(int)Side.Black]);

            ulong nonAttackedQueensideMask = ExtractNonAttackedMask(startingSquare, Constants.BlackLongCastleKingSquare);
            Assert.AreEqual(nonAttackedQueensideMask, position.QueensideCastlingNonAttackedSquares[(int)Side.Black]);

            ulong whiteNonAttackedKingsideMask = ExtractNonAttackedMask(startingSquare, Constants.BlackShortCastleKingSquare);
            Assert.AreEqual(whiteNonAttackedKingsideMask, position.KingsideCastlingNonAttackedSquares[(int)Side.White]);

            ulong whiteNonAttackedQueensideMask = ExtractNonAttackedMask(startingSquare, Constants.BlackLongCastleKingSquare);
            Assert.AreEqual(whiteNonAttackedQueensideMask, position.QueensideCastlingNonAttackedSquares[(int)Side.White]);

            static ulong ExtractNonAttackedMask(int startSquare, int endSquare)
            {
                var lowerSquare = Math.Min(startSquare, endSquare);
                var squareCount = Math.Abs(startSquare - endSquare);
                var kingStartToEnd = Enumerable.Range(lowerSquare, squareCount + 1);

                Assert.True(startSquare == kingStartToEnd.First() || startSquare == kingStartToEnd.Last());
                Assert.True(endSquare == kingStartToEnd.First() || endSquare == kingStartToEnd.Last());

                ulong nonAttackedMask = 0;
                foreach (var n in kingStartToEnd)
                {
                    nonAttackedMask.SetBit(n);
                }

                return nonAttackedMask;
            }
        }
    }
}
