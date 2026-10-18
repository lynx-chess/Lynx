using Lynx.Model;
using NUnit.Framework;
namespace Lynx.Test;

public class RaysBetweenTest
{
    [Test]
    public void RaysBetween_ContainsSquaresStrictlyBetween()
    {
        for (int square1 = 0; square1 < 64; ++square1)
        {
            for (int square2 = 0; square2 < 64; ++square2)
            {
                if (square1 == square2)
                {
                    Assert.AreEqual(0, Attacks.RaysBetween[square1][square2]);
                    continue;
                }

                var file1 = Constants.File(square1);
                var rank1 = Constants.Rank(square1);
                var fileDifference = Constants.File(square2) - file1;
                var rankDifference = Constants.Rank(square2) - rank1;

                Bitboard expected = 0;

                if (fileDifference == 0 || rankDifference == 0 || Math.Abs(fileDifference) == Math.Abs(rankDifference))
                {
                    var fileStep = Math.Sign(fileDifference);
                    var rankStep = Math.Sign(rankDifference);

                    for (int file = file1 + fileStep, rank = rank1 + rankStep;
                        BitboardExtensions.SquareIndex(7 - rank, file) != square2;
                        file += fileStep, rank += rankStep)
                    {
                        expected |= 1UL << BitboardExtensions.SquareIndex(7 - rank, file);
                    }
                }

                Assert.AreEqual(expected, Attacks.RaysBetween[square1][square2], $"RaysBetween[{square1}][{square2}]");
            }
        }
    }
}