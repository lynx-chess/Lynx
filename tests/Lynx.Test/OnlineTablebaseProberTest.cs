#pragma warning disable S4144 // Methods should not have identical implementations

using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

[NonParallelizable]
public class OnlineTablebaseProberTest
{
    public OnlineTablebaseProberTest()
    {
        Configuration.EngineSettings.UseOnlineTablebase = true;
    }

    [TestCase("1N6/8/p7/8/4kN2/8/K7/8 w - - 0 1", 115, "f4e2")]     // 115 moves to mate
    [TestCase("8/3B4/8/1R6/5r2/8/3K4/5k2 w - - 1 1", 65, "d2e3")]   // 64 moves to mate, 58 moves to zero
    public void RootSearch_CursedWin(string fen, int distanceToMate, string bestMove)
    {
        var result = OnlineTablebaseProber.RootSearch(new Position(fen), 0, default);
        Assert.AreEqual(distanceToMate, result.DistanceToMate);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [TestCase("1N6/8/p7/8/4kN2/8/K7/8 w - - 0 1", 115, "f4e2")]     // 115 moves to mate
    [TestCase("8/3B4/8/1R6/5r2/8/3K4/5k2 w - - 1 1", 65, "d2e3")]   // 64 moves to mate, 58 moves to zero
    public void EvaluationSearch_CursedWin(string fen, int distanceToMate, string bestMove)
    {
        var result = OnlineTablebaseProber.EvaluationSearch(new Position(fen), 0, default);
        Assert.AreEqual(0, result);
    }

    [TestCase("1N6/8/p7/8/4k3/8/K3N3/8 b - - 1 1", -114, "e4d3")]   // 114 moves to mate
    [TestCase("8/3B4/8/1R6/5r2/4K3/8/5k2 b - - 2 1", -64, "f4c4")] // 63 moves to mate, 57 moves to zero
    public void RootSearch_BlessedLoss(string fen, int distanceToMate, string bestMove)
    {
        var result = OnlineTablebaseProber.RootSearch(new Position(fen), 0, default);
        Assert.AreEqual(distanceToMate, result.DistanceToMate);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [TestCase("1N6/8/p7/8/4k3/8/K3N3/8 b - - 1 1", -114, "e4d3")]   // 114 moves to mate
    [TestCase("8/3B4/8/1R6/5r2/4K3/8/5k2 b - - 2 1", -64, "f4c4")] // 63 moves to mate, 57 moves to zero
    public void EvaluationSearch_BlessedLoss(string fen, int distanceToMate, string bestMove)
    {
        var result = OnlineTablebaseProber.EvaluationSearch(new Position(fen), 0, default);
        Assert.AreEqual(0, result);
    }

    [TestCase("7k/5Q2/3K4/8/8/8/8/8 b - - 0 1")]
    [TestCase("7K/5q2/4k3/8/8/8/8/8 w - - 0 1")]
    public void RootSearch_Stalemate(string fen)
    {
        var result = OnlineTablebaseProber.RootSearch(new Position(fen), 0, default);
        Assert.AreEqual(OnlineTablebaseProber.NoResult, result.DistanceToMate);
        Assert.AreEqual(0, result.BestMove);
    }

    [TestCase("7k/5Q2/3K4/8/8/8/8/8 b - - 0 1")]
    [TestCase("7K/5q2/4k3/8/8/8/8/8 w - - 0 1")]
    public void EvaluationSearch_Stalemate(string fen)
    {
        var result = OnlineTablebaseProber.EvaluationSearch(new Position(fen), 0, default);
        Assert.AreEqual(0, result);
    }

    /// <summary>
    /// For some reason this position isn't detected as no material one
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="distanceToMate"></param>
    [TestCase("3k4/8/1K6/8/8/6N1/4N3/8 w - - 0 1", 0)]  // 2 horses
    [TestCase("3k4/8/1K6/8/8/6N1/4N3/8 b - - 0 1", 0)]  // 2 horses
    [TestCase("3k4/8/1K6/8/8/6n1/4n3/8 w - - 0 1", 0)]  // 2 horses
    [TestCase("3k4/8/1K6/8/8/6n1/4n3/8 b - - 0 1", 0)]  // 2 horses
    [TestCase("8/3n4/8/8/8/2B3K1/8/6k1 b - - 1 1", 0)]  // BK vs NK
    [TestCase("8/3n4/8/8/8/2B3K1/8/6k1 w - - 1 1", 0)]  // BK vs NK
    [TestCase("8/3N4/8/8/8/2b3K1/8/6k1 b - - 1 1", 0)]  // BK vs NK
    [TestCase("8/3N4/8/8/8/2b3K1/8/6k1 w - - 1 1", 0)]  // BK vs NK
    public void RootSearch_ImpossibleCheckmate(string fen, int distanceToMate)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.RootSearch(position, 0, default);
        Assert.AreEqual(distanceToMate, result.DistanceToMate);
        Assert.Contains(result.BestMove, position.AllPossibleMoves().ToList());
    }

    /// <summary>
    /// For some reason this position isn't detected as no material one
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="distanceToMate"></param>
    [TestCase("3k4/8/1K6/8/8/6N1/4N3/8 w - - 0 1", 0)]  // 2 horses
    [TestCase("3k4/8/1K6/8/8/6N1/4N3/8 b - - 0 1", 0)]  // 2 horses
    [TestCase("3k4/8/1K6/8/8/6n1/4n3/8 w - - 0 1", 0)]  // 2 horses
    [TestCase("3k4/8/1K6/8/8/6n1/4n3/8 b - - 0 1", 0)]  // 2 horses
    [TestCase("8/3n4/8/8/8/2B3K1/8/6k1 b - - 1 1", 0)]  // BK vs NK
    [TestCase("8/3n4/8/8/8/2B3K1/8/6k1 w - - 1 1", 0)]  // BK vs NK
    [TestCase("8/3N4/8/8/8/2b3K1/8/6k1 b - - 1 1", 0)]  // BK vs NK
    [TestCase("8/3N4/8/8/8/2b3K1/8/6k1 w - - 1 1", 0)]  // BK vs NK
    public void EvaluationSearch_ImpossibleCheckmate(string fen, int distanceToMate)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, 0, default);
        Assert.AreEqual(0, result);
    }

    [TestCase("8/8/8/8/8/2B3K1/8/6k1 b - - 1 1", 0)]   // B
    [TestCase("8/8/8/8/8/2B3K1/8/6k1 w - - 1 1", 0)]   // B
    [TestCase("8/8/8/8/8/2b3K1/8/6k1 b - - 1 1", 0)]   // b
    [TestCase("8/8/8/8/8/2b3K1/8/6k1 w - - 1 1", 0)]   // b
    [TestCase("8/8/8/8/8/2N3K1/8/6k1 w - - 1 1", 0)]   // N
    [TestCase("8/8/8/8/8/2N3K1/8/6k1 b - - 1 1", 0)]   // N
    [TestCase("8/8/8/8/8/2n3K1/8/6k1 w - - 1 1", 0)]   // n
    [TestCase("8/8/8/8/8/2n3K1/8/6k1 b - - 1 1", 0)]   // n
    public void RootSearch_NoMaterial(string fen, int distanceToMate)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.RootSearch(position, 0, default);
        Assert.AreEqual(distanceToMate, result.DistanceToMate);
        Assert.Contains(result.BestMove, position.AllPossibleMoves().ToList());
    }

    [TestCase("8/8/8/8/8/2B3K1/8/6k1 b - - 1 1", 0)]   // B
    [TestCase("8/8/8/8/8/2B3K1/8/6k1 w - - 1 1", 0)]   // B
    [TestCase("8/8/8/8/8/2b3K1/8/6k1 b - - 1 1", 0)]   // b
    [TestCase("8/8/8/8/8/2b3K1/8/6k1 w - - 1 1", 0)]   // b
    [TestCase("8/8/8/8/8/2N3K1/8/6k1 w - - 1 1", 0)]   // N
    [TestCase("8/8/8/8/8/2N3K1/8/6k1 b - - 1 1", 0)]   // N
    [TestCase("8/8/8/8/8/2n3K1/8/6k1 w - - 1 1", 0)]   // n
    [TestCase("8/8/8/8/8/2n3K1/8/6k1 b - - 1 1", 0)]   // n
    public void EvaluationSearch_NoMaterial(string fen, int distanceToMate)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, 0, default);
        Assert.AreEqual(0, result);
    }

    [TestCase("8/1q6/8/8/8/6K1/1RR5/6k1 b - - 1 1", 0, "b7g7")]
    public void RootSearch_DrawWithCorrectPlay(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.RootSearch(position, 0, default);
        Assert.AreEqual(distanceToMate, result.DistanceToMate);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [TestCase("8/1q6/8/8/8/6K1/1RR5/6k1 b - - 1 1", 0, "b7g7")]
    public void EvaluationSearch_DrawWithCorrectPlay(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, 0, default);
        Assert.AreEqual(0, result);
    }

    [TestCase("8/4P3/8/8/8/2K1P3/k3p3/8 b - - 1 1", 14, "e2e1q")]   // Win or lose
    public void RootSearch_WinWithCorrectPlay(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.RootSearch(position, 0, default);
        Assert.AreEqual(distanceToMate, result.DistanceToMate);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [TestCase("8/4P3/8/8/8/2K1P3/k3p3/8 b - - 1 1", 14, "e2e1q")]   // Win or lose
    public void EvaluationSearch_WinWithCorrectPlay(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, 0, default);
        Assert.Greater(result, -EvaluationConstants.CheckMateEvaluation); // TODO detection limit
        Assert.Less(result, -0.1 * EvaluationConstants.CheckMateEvaluation); // TODO detection limit
    }

    [TestCase("8/3P3K/3p1k2/8/8/8/3p4/8 w - - 1 1", 14, "d7d8q")]   // Win or lose
    public void RootSearch_WinWithCorrectPlay_2(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.RootSearch(position, 0, default);
        Assert.AreEqual(distanceToMate, result.DistanceToMate);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [TestCase("8/3P3K/3p1k2/8/8/8/3p4/8 w - - 1 1", 14, "d7d8q")]   // Win or lose
    public void EvaluationSearch_WinWithCorrectPlay_2(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, 0, default);
        Assert.Greater(result, 0.1 * EvaluationConstants.CheckMateEvaluation); // TODO detection limit
        Assert.Less(result, EvaluationConstants.CheckMateEvaluation); // TODO detection limit
    }

    /// <summary>
    /// In root search mate score will always be the detected one, even if over 50 moves with or without previous <paramref name="halfMovesWithoutCaptureOrPawnMove"/>
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="distanceToMate"></param>
    /// <param name="bestMove"></param>
    /// <param name="halfMovesWithoutCaptureOrPawnMove"></param>
    [TestCase("kN6/8/8/8/8/8/8/B3K3 w - - 0 1", 27, "a1e5", 0)]    // B+N, Mate in 27, DTZ?==DTM=58
    [TestCase("kN6/8/8/8/8/8/8/B3K3 w - - 0 1", 27, "a1e5", 50)]   // B+N, Mate in 27, DTZ?==DTM=58
    [TestCase("Kn6/8/8/8/8/8/8/b3k3 b - - 0 1", 27, "a1e5", 0)]    // B+N, Mate in 27, DTZ?==DTM=58
    [TestCase("Kn6/8/8/8/8/8/8/b3k3 b - - 0 1", 27, "a1e5", 50)]   // B+N, Mate in 27, DTZ?==DTM=58
    public void RootSearch_DrawWinningPositionDueToExistingHalfMovesWithoutCaptureOrPawnMove(string fen, int distanceToMate, string bestMove, int halfMovesWithoutCaptureOrPawnMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.RootSearch(position, halfMovesWithoutCaptureOrPawnMove, default);
        Assert.AreEqual(distanceToMate, result.DistanceToMate);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    /// <summary>
    /// In evaluation search we need to check <paramref name="halfMovesWithoutCaptureOrPawnMove"/> to make sure to apply the 50 moves rule correctly
    /// </summary>
    /// <param name="fen"></param>
    /// <param name="halfMovesWithoutCaptureOrPawnMove"></param>
    /// <param name="expectedEvaluation"></param>
    [TestCase("kN6/8/8/8/8/8/8/B3K3 w - - 0 1", 0, EvaluationConstants.CheckMateEvaluation - 27 * Position.DepthFactor)]    // B+N, Mate in 27, DTZ==DTM=58
    [TestCase("kN6/8/8/8/8/8/8/B3K3 w - - 0 1", 50, 0)]                                                                     // B+N, Mate in 27, DTM=58, DTZ >100
    [TestCase("Kn6/8/8/8/8/8/8/b3k3 b - - 0 1", 0, -EvaluationConstants.CheckMateEvaluation + 27 * Position.DepthFactor)]   // B+N, Mate in 27, DTZ?==DTM=58
    [TestCase("Kn6/8/8/8/8/8/8/b3k3 b - - 0 1", 50, 0)]                                                                     // B+N, Mate in 27, DTZ?==DTM=58, DTZ >100
    public void EvaluationSearch_DrawWinningPositionDueToExistingHalfMovesWithoutCaptureOrPawnMove(string fen, int halfMovesWithoutCaptureOrPawnMove, int expectedEvaluation)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, halfMovesWithoutCaptureOrPawnMove, default);
        Assert.AreEqual(expectedEvaluation, result);
    }
}

#pragma warning restore S4144 // Methods should not have identical implementations
