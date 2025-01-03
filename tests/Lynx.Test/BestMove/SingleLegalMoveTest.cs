using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.BestMove;

/// <summary>
/// "If there's a single move, just do it"
/// </summary>
public class SingleLegalMoveTest : BaseTest
{
    // https://lichess.org/MThBKius/black#313
    [TestCase("8/PPPPP1Pk/1PPPPP2/5PPP/2R5/R2RKR2/1R4R1/7R b - - 58 154")]
    [TestCase("Q5k1/1PPPP1P1/1PPPPP2/5PPP/2R5/R2RKR2/1R4R1/7R b - - 0 155")]
    [TestCase("6Q1/1PPPP1Pk/1PPPPP2/5PPP/2R5/R2RKR2/1R4R1/7R b - - 2 156")]
    [TestCase("6k1/1PPPP1P1/1PPPPP2/5PPP/2R5/R2RKRR1/1R6/7R b - - 1 157")]

    // https://lichess.org/BmNRWSXV/black#363
    [TestCase("1k6/3PPPPP/PPPPPPPP/PPPRK3/1RR3RR/R4R2/8/8 b - - 8 181")]
    [TestCase("k7/3PPPPP/PPPPPPPP/PPPRK3/1RR3RR/R7/5R2/8 b - - 10 182")]

    [TestCase("8/8/8/8/8/k7/1R6/K1q5 w - - 0 1")]
    [TestCase("8/8/8/8/8/k7/2B5/K1q5 w - - 0 1")]
    [TestCase("8/8/8/8/8/k1N5/8/K1q5 w - - 0 1")]
    [TestCase("1Q6/8/8/8/8/k7/8/K1q5 w - - 0 1")]

    [TestCase("8/8/8/8/8/K7/1r6/k1Q5 b - - 0 1")]
    [TestCase("8/8/8/8/8/K7/2b5/k1Q5 b - - 0 1")]
    [TestCase("8/8/8/8/8/K1n5/8/k1Q5 b - - 0 1")]
    [TestCase("1q6/8/8/8/8/K7/8/k1Q5 b - - 0 1")]
    public void SingleMove(string fen)
    {
        // Arrange
        const int depth = 61;
        Move? singleMove = null;
        var pos = new Position(fen);
        foreach (var move in pos.GenerateAllMoves())
        {
            var state = pos.MakeMove(move);
            if (pos.IsValid())
            {
                Assert.IsNull(singleMove);
                singleMove = move;
            }

            pos.UnmakeMove(move, state);
        }

        Assert.LessOrEqual(depth, Configuration.EngineSettings.MaxDepth);

        // Act
        var result = SearchBestMove(fen, depth);

        Assert.AreEqual(singleMove, result.BestMove);
        Assert.AreEqual(singleMove, result.Moves.Single());

        if (pos.Side == Side.White)
        {
            Assert.AreEqual(EvaluationConstants.SingleMoveScore, result.Score);
        }
        else
        {
            Assert.AreEqual(-EvaluationConstants.SingleMoveScore, result.Score);
        }

        Assert.AreEqual(0, result.Depth);
        Assert.AreEqual(0, result.DepthReached);
        Assert.AreEqual(0, result.NodesPerSecond);
    }
}
