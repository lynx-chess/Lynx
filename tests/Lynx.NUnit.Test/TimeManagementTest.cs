using NUnit.Framework;

namespace Lynx.NUnit.Test
{
    public class TimeManagementTest
    {
        [TestCase(
            300_000,    // 5 min
            0,
            40,         // 40 moves left
            11_250)]    // 11.25s, CoefficientBeforeKeyMovesBeforeMovesToGo * millisecondsLeft) / movesToGo
        [TestCase(
            300_000,    // 5 min
            10_000,     // 10s increment
            40,         // 40 moves left
            20_875)]    // 20.9s, millisecondsIncrement + CoefficientAfterKeyMovesBeforeMovesToGo * (millisecondsLeft - millisecondsIncrement) / movesToGo
        [TestCase(
            20_000,     // 20s
            0,
            5,          // 5 moves left
            3_800)]     // 3.8s, millisecondsIncrement + CoefficientAfterKeyMovesBeforeMovesToGo * (millisecondsLeft - millisecondsIncrement) / movesToGo
        [TestCase(
            20_000,     // 20s
            10_000,     // 10s increment
            5,          // 5 moves left
            11_900)]    // 11.9s, millisecondsIncrement + CoefficientAfterKeyMovesBeforeMovesToGo * (millisecondsLeft - millisecondsIncrement) / movesToGo
        [TestCase(
            12_000,     // 12s
            10_000,     // 10s increment
            5,          // 5 moves left
            10_380)]    // 10.38s, millisecondsIncrement + CoefficientAfterKeyMovesBeforeMovesToGo * (millisecondsLeft - millisecondsIncrement) / movesToGo
        [TestCase(
            10_000,     // 10s
            10_000,     // 10s increment
            5,          // 5 moves left
            9_000)]     // 9s, millisecondsLeft - decisionTime < 1_000 -> decisionTime *= 0.9
        [TestCase(
            10_000,     // 10s
            10_000,     // 10s increment
            50,         // 50 moves left
            9_000)]     // 9s, millisecondsLeft - decisionTime < 1_000 -> decisionTime *= 0.9
        [TestCase(
            1_000,      // 1s
            1_000,      // 1s increment
            10,         // 50 moves left
            900)]       // 0
        [TestCase(
            182_000,    // 3min
            2_000,      // 2s increment
            0,         // 50 moves left
            900)]       // 0.9s, millisecondsLeft - decisionTime  < 1_000 -> decisionTime *= 0.9
        public void CalculateDecisionTime(
            int millisecondsLeft, int millisecondsIncrement, int movesToGo,int expectedTimeToMove)
        {
            var engine = new Engine();
            var timeToMove = engine.CalculateDecisionTime(movesToGo, millisecondsLeft, millisecondsIncrement);

            Assert.AreEqual(expectedTimeToMove, timeToMove);
        }
    }
}
