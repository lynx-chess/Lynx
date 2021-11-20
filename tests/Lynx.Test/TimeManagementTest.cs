using Moq;
using NUnit.Framework;
using System.Threading.Channels;

namespace Lynx.Test;

public class TimeManagementTest
{
    [TestCase(
        2_000,      // 2s, < 10s
        11_111,     // 11s increment
        1,
        9999.90)]   // 10s, 0.9 * increment
    [TestCase(
        2_000,      // 2s, < 10s
        10_000,     // 10s increment
        1,
        9_000)]     // 9s, 0.9 * increment
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
        10,         // 10 moves left
        900)]       // 0.9s, millisecondsLeft - decisionTime  < 1_000 -> decisionTime *= 0.9
    [TestCase(
        182_000,    // 3min (> FirstTimeLimitWhenNoMovesToGoProvided)
        2_000,      // 2s increment
        0,          // No moves to go
        7400,       // 7.4s, millisecondsIncrement + FirstCoefficientWhenNoMovesToGoProvided * (millisecondsLeft - millisecondsIncrement) / TotalMovesWhenNoMovesToGoProvided - (Game.MoveHistory.Count / 2)
        0)]         // Beginning of a 3 + 2 game
    [TestCase(
        182_000,    // 3min (> FirstTimeLimitWhenNoMovesToGoProvided)
        2_000,      // 2s increment
        0,          // No moves to go
        9714,       // 9.7s, millisecondsIncrement + FirstCoefficientWhenNoMovesToGoProvided * (millisecondsLeft - millisecondsIncrement) / TotalMovesWhenNoMovesToGoProvided - (Game.MoveHistory.Count / 2)
        60)]        // ~Endgame of a longer game: 30 moves each
    [TestCase(
        62000,      // 1min (< FirstTimeLimitWhenNoMovesToGoProvided, > SecondTimeLimitWhenNoMovesToGoProvided)
        2_000,      // 2s increment
        0,          // No moves to go
        3200,       // 3.2s, millisecondsIncrement + SecondCoefficientWhenNoMovesToGoProvided * (millisecondsLeft - millisecondsIncrement) / TotalMovesWhenNoMovesToGoProvided - (Game.MoveHistory.Count / 2)
        0)]         // Beginning of 1 + 2 game
    [TestCase(
        62000,      // 1min (< FirstTimeLimitWhenNoMovesToGoProvided, > SecondTimeLimitWhenNoMovesToGoProvided)
        2_000,      // 2s increment
        0,          // No moves to go
        3500,       // 3.5s, millisecondsIncrement + SecondCoefficientWhenNoMovesToGoProvided * (millisecondsLeft - millisecondsIncrement) / TotalMovesWhenNoMovesToGoProvided - (Game.MoveHistory.Count / 2)
        40)]        // ~Middlegame of a 3 + 2 game: 40 moves: 20 moves each
    [TestCase(
        27000,      // 25s (< SecondTimeLimitWhenNoMovesToGoProvided)
        2_000,      // 2s increment
        0,          // No moves to go
        2357,       // 2.4s, millisecondsIncrement + SecondCoefficientWhenNoMovesToGoProvided * (millisecondsLeft - millisecondsIncrement) / TotalMovesWhenNoMovesToGoProvided - (Game.MoveHistory.Count / 2)
        60)]        // ~Endgame of a 3 + 2 game: 60 moves: 30 moves each
    [TestCase(
        31000,      // 31s (< FirstTimeLimitWhenNoMovesToGoProvided, > SecondTimeLimitWhenNoMovesToGoProvided)
        0,          // No increment
        0,          // No moves to go
        885,        // 0.88s, millisecondsIncrement + (millisecondsLeft - millisecondsIncrement) / TotalMovesWhenNoMovesToGoProvided - (Game.MoveHistory.Count / 2)
        60)]        // ~Endgame of a 3 + 2 game: 60 moves: 30 moves each
    [TestCase(
        29000,      // 29s (< SecondTimeLimitWhenNoMovesToGoProvided)
        0,          // No increment
        0,          // No moves to go
        414,        // 0.41s, millisecondsIncrement + (millisecondsLeft - millisecondsIncrement) / TotalMovesWhenNoMovesToGoProvided - (Game.MoveHistory.Count / 2 ))
        60)]        // ~Endgame of a 3 + 2 game: 60 moves: 30 moves each
    [TestCase(
        5710,       // 5.7s
        5000,       // 5s increment
        0,          // No moves to go
        4531.5,     // 0.9 * 5035, 0.9 * (millisecondsIncrement + (millisecondsLeft - millisecondsIncrement) / FixedMovesLeftWhenNoMovesToGoProvidedAndOverTotalMovesWhenNoMovesToGoProvided)
        200)]       // Over default TotalMovesWhenNoMovesToGoProvided (100), https://lichess.org/GxfJjvUu/black
    [TestCase(
        6001,       // 6s
        5000,       // 5s increment
        0,          // No moves to go
        4545,      // 0.9 * 5050, 0.9 * (millisecondsIncrement + (millisecondsLeft - millisecondsIncrement) / FixedMovesLeftWhenNoMovesToGoProvidedAndOverTotalMovesWhenNoMovesToGoProvided)
        200)]       // Over default TotalMovesWhenNoMovesToGoProvided (100), https://lichess.org/GxfJjvUu/black
    [TestCase(
        6001,       // 6s
        5000,       // 5s increment
        0,          // No moves to go
        4545,       // 0.9 * 5050, 0.9 * (millisecondsIncrement + (millisecondsLeft - millisecondsIncrement) / FixedMovesLeftWhenNoMovesToGoProvidedAndOverTotalMovesWhenNoMovesToGoProvided)
        250)]       // Over default TotalMovesWhenNoMovesToGoProvided (100), https://lichess.org/GxfJjvUu/black
    [TestCase(
        1100,       // 1.1s
        1000,       // 1s increment
        0,          // No moves to go
        904.5,      // 0.9, 0.9 * (millisecondsIncrement + (millisecondsLeft - millisecondsIncrement) / FixedMovesLeftWhenNoMovesToGoProvidedAndOverTotalMovesWhenNoMovesToGoProvided)
        210)]       // Over default TotalMovesWhenNoMovesToGoProvided (100)
    [TestCase(
        2100,       // 2.1s
        1000,       // 1s increment
        0,          // No moves to go
        1055,       // 1 + 1.1/20, (millisecondsIncrement + (millisecondsLeft - millisecondsIncrement) / FixedMovesLeftWhenNoMovesToGoProvidedAndOverTotalMovesWhenNoMovesToGoProvided)
        210)]       // Over default TotalMovesWhenNoMovesToGoProvided (100)
    [TestCase(
        21000,      // 21s
        1000,       // 1s increment
        0,          // No moves to go
        2000,       // 2, (millisecondsIncrement + (millisecondsLeft - millisecondsIncrement) / FixedMovesLeftWhenNoMovesToGoProvidedAndOverTotalMovesWhenNoMovesToGoProvided)
        210)]       // Over default TotalMovesWhenNoMovesToGoProvided (100)
    public void CalculateDecisionTime(
        int millisecondsLeft, int millisecondsIncrement, int movesToGo, double expectedTimeToMove, int moveHistoryCount = 0)
    {
        var engine = new Engine(new Mock<ChannelWriter<string>>().Object);
        for (int moveIndex = 0; moveIndex < moveHistoryCount; ++moveIndex)
        {
            engine.Game.MoveHistory.Add(new());
        }
        var timeToMove = engine.CalculateDecisionTime(movesToGo, millisecondsLeft, millisecondsIncrement);

        Assert.True(Equals(expectedTimeToMove, timeToMove), $"{timeToMove} was expected to be {expectedTimeToMove}");
    }
}
