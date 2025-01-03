#pragma warning disable S4144   // Methods should not have identical implementations - Logical separations, even if the implementation is the same
#pragma warning disable RCS1163 // Unused parameter -  RootSearch_ and EvaluationSearch_ methods have the same fen input, so they're just duplicated (too lazy to create data input collections)
#pragma warning disable IDE0060 // Remove unused parameter -  RootSearch_ and EvaluationSearch_ methods have the same fen input, so they're just duplicated (too lazy to create data input collections)

using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NUnit.Framework;
using static Lynx.EvaluationConstants;

namespace Lynx.Test;

[NonParallelizable]
public class OnlineTablebaseProberTest
{
    public OnlineTablebaseProberTest()
    {
        Configuration.EngineSettings.UseOnlineTablebaseInRootPositions = true;
        Configuration.EngineSettings.UseOnlineTablebaseInSearch = true;
    }

    [TestCase("1N6/8/p7/8/4kN2/8/K7/8 w - - 0 1", 115, "f4e2")]     // 115 moves to mate
    [TestCase("8/3B4/8/1R6/5r2/8/3K4/5k2 w - - 1 1", 65, "d2e3")]   // 64 moves to mate, 58 moves to zero
    public async Task RootSearch_CursedWin(string fen, int distanceToMate, string bestMove)
    {
        var result = await OnlineTablebaseProber.RootSearch(new Position(fen), [], 0, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
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
    [TestCase("8/6B1/8/8/B7/8/K1pk4/8 b - - 0 1", -67, "c2c1n")] // 67 moves to mate if the underpromotion is played
    public async Task RootSearch_BlessedLoss(string fen, int distanceToMate, string bestMove)
    {
        var result = await OnlineTablebaseProber.RootSearch(new Position(fen), [], 0, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
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
    public async Task RootSearch_Stalemate(string fen)
    {
        var result = await OnlineTablebaseProber.RootSearch(new Position(fen), [], 0, default);
        Assert.AreEqual(OnlineTablebaseProber.NoResult, result.MateScore);
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
    [TestCase("3k4/8/1K6/8/8/6N1/4N3/8 w - - 0 1", 0)]  // 2 horses
    [TestCase("3k4/8/1K6/8/8/6N1/4N3/8 b - - 0 1", 0)]  // 2 horses
    [TestCase("3k4/8/1K6/8/8/6n1/4n3/8 w - - 0 1", 0)]  // 2 horses
    [TestCase("3k4/8/1K6/8/8/6n1/4n3/8 b - - 0 1", 0)]  // 2 horses
    [TestCase("8/3n4/8/8/8/2B3K1/8/6k1 b - - 1 1", 0)]  // BK vs NK
    [TestCase("8/3n4/8/8/8/2B3K1/8/6k1 w - - 1 1", 0)]  // BK vs NK
    [TestCase("8/3N4/8/8/8/2b3K1/8/6k1 b - - 1 1", 0)]  // BK vs NK
    [TestCase("8/3N4/8/8/8/2b3K1/8/6k1 w - - 1 1", 0)]  // BK vs NK
    public async Task RootSearch_ImpossibleCheckmate(string fen, int distanceToMate)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], 0, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.Contains(result.BestMove, MoveGenerator.GenerateAllMoves(position).ToList());
    }

    /// <summary>
    /// For some reason this position isn't detected as no material one
    /// </summary>
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
    public async Task RootSearch_NoMaterial(string fen, int distanceToMate)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], 0, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.Contains(result.BestMove, MoveGenerator.GenerateAllMoves(position).ToList());
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
    public async Task RootSearch_DrawWithCorrectPlay(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], 0, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
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
    public async Task RootSearch_WinWithCorrectPlay(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], 0, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [TestCase("8/4P3/8/8/8/2K1P3/k3p3/8 b - - 1 1", 14, "e2e1q")]   // Win or lose
    public void EvaluationSearch_WinWithCorrectPlay(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, 0, default);
        Assert.Greater(result, PositiveCheckmateDetectionLimit);
        Assert.Less(result, CheckMateBaseEvaluation);
    }

    [TestCase("k7/2QR4/8/8/8/4N3/2r4Q/1K6 b - - 0 1", -49, "c2c1")]   // Loss but after 15 checks
    public async Task RootSearch_LossButHoldingAsLongAsPossible(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], 0, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [TestCase("k7/2QR4/8/8/8/4N3/2r4Q/1K6 b - - 0 1", 49, "c2c1")]   // Loss but after 15 checks
    public void EvaluationSearch_LossButHoldingAsLongAsPossible(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, 0, default);
        Assert.AreEqual(-CheckMateBaseEvaluation + distanceToMate, result);
    }

    [TestCase("8/3P3K/3p1k2/8/8/8/3p4/8 w - - 1 1", 14, "d7d8q")]   // Win or lose
    public async Task RootSearch_WinWithCorrectPlay_2(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], 0, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [TestCase("8/3P3K/3p1k2/8/8/8/3p4/8 w - - 1 1", 14, "d7d8q")]   // Win or lose
    public void EvaluationSearch_WinWithCorrectPlay_2(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, 0, default);
        Assert.Greater(result, PositiveCheckmateDetectionLimit);
        Assert.Less(result, CheckMateBaseEvaluation);
    }

    /// <summary>
    /// In root search mate score will always be the detected one, even if over 50 moves with or without previous <paramref name="halfMovesWithoutCaptureOrPawnMove"/>
    /// </summary>
    [TestCase("kN6/8/8/8/8/8/8/B3K3 w - - 0 1", 27, "a1e5", 0)]    // B+N, Mate in 27, DTZ?==DTM=58
    [TestCase("kN6/8/8/8/8/8/8/B3K3 w - - 0 1", 27, "a1e5", 50)]   // B+N, Mate in 27, DTZ?==DTM=58
    [TestCase("Kn6/8/8/8/8/8/8/b3k3 b - - 0 1", 27, "a1e5", 0)]    // B+N, Mate in 27, DTZ?==DTM=58
    [TestCase("Kn6/8/8/8/8/8/8/b3k3 b - - 0 1", 27, "a1e5", 50)]   // B+N, Mate in 27, DTZ?==DTM=58
    public async Task RootSearch_DrawWinningPositionDueToExistingHalfMovesWithoutCaptureOrPawnMove(string fen, int distanceToMate, string bestMove, int halfMovesWithoutCaptureOrPawnMove)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], halfMovesWithoutCaptureOrPawnMove, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    /// <summary>
    /// In evaluation search we need to check <paramref name="halfMovesWithoutCaptureOrPawnMove"/> to make sure to apply the 50 moves rule correctly
    /// </summary>
    [TestCase("kN6/8/8/8/8/8/8/B3K3 w - - 0 1", 0, CheckMateBaseEvaluation - 27)]    // B+N, Mate in 27, DTZ==DTM=58
    [TestCase("kN6/8/8/8/8/8/8/B3K3 w - - 0 1", 50, 0)]                                                                     // B+N, Mate in 27, DTM=58, DTZ >100
    [TestCase("Kn6/8/8/8/8/8/8/b3k3 b - - 0 1", 0, +CheckMateBaseEvaluation - 27)]   // B+N, Mate in 27, DTZ?==DTM=58
    [TestCase("Kn6/8/8/8/8/8/8/b3k3 b - - 0 1", 50, 0)]                                                                     // B+N, Mate in 27, DTZ?==DTM=58, DTZ >100
    public void EvaluationSearch_DrawWinningPositionDueToExistingHalfMovesWithoutCaptureOrPawnMove(string fen, int halfMovesWithoutCaptureOrPawnMove, int expectedEvaluation)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, halfMovesWithoutCaptureOrPawnMove, default);
        Assert.AreEqual(expectedEvaluation, result);
    }

    /// <summary>
    /// In root search mate score will always be the detected one, even if over 50 moves with or without previous <paramref name="halfMovesWithoutCaptureOrPawnMove"/>
    /// </summary>
    [TestCase("6kN/8/8/8/8/8/8/B3K3 b - - 0 1", -30, "g8f8", 0)]    // B+N, Mate in 30, DTZ?==DTM=60
    [TestCase("6kN/8/8/8/8/8/8/B3K3 b - - 0 1", -30, "g8f8", 42)]   // B+N, Mate in 30, DTZ?==DTM=60
    [TestCase("6Kn/8/8/8/8/8/8/b3k3 w - - 0 1", -30, "g8f8", 0)]    // B+N, Mate in 30, DTZ?==DTM=60
    [TestCase("6Kn/8/8/8/8/8/8/b3k3 w - - 0 1", -30, "g8f8", 42)]   // B+N, Mate in 30, DTZ?==DTM=60
    public async Task RootSearch_DrawLosingPositionDueToExistingHalfMovesWithoutCaptureOrPawnMove(string fen, int distanceToMate, string bestMove, int halfMovesWithoutCaptureOrPawnMove)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], halfMovesWithoutCaptureOrPawnMove, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    /// <summary>
    /// In evaluation search we need to check <paramref name="halfMovesWithoutCaptureOrPawnMove"/> to make sure to apply the 50 moves rule correctly
    /// </summary>
    [TestCase("6kN/8/8/8/8/8/8/B3K3 b - - 0 1", 0, -CheckMateBaseEvaluation + 30)]   // B+N, Mate in 30, DTZ?==DTM=60
    [TestCase("6kN/8/8/8/8/8/8/B3K3 b - - 0 1", 42, 0)]                                                                     // B+N, Mate in 30, DTZ?==DTM=60
    [TestCase("6Kn/8/8/8/8/8/8/b3k3 w - - 0 1", 0, -CheckMateBaseEvaluation + 30)]   // B+N, Mate in 30, DTZ?==DTM=60
    [TestCase("6Kn/8/8/8/8/8/8/b3k3 w - - 0 1", 42, 0)]                                                                     // B+N, Mate in 30, DTZ?==DTM=60
    public void EvaluationSearch_DrawLosingPositionDueToExistingHalfMovesWithoutCaptureOrPawnMove(string fen, int halfMovesWithoutCaptureOrPawnMove, int expectedEvaluation)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, halfMovesWithoutCaptureOrPawnMove, default);
        Assert.AreEqual(expectedEvaluation, result);
    }

    [TestCase("8/1k6/8/6B1/p1P5/2K1Pp2/8/8 w - - 0 1", +49, new[] { "g5h4", "c3d2", "c3d3" })]
    public async Task RootSearch_NoDistanceToMateProvidedWhileWinning(string fen, int distanceToMate, string[] bestMoves)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], default, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.Contains(result.BestMove.UCIString(), bestMoves);
    }

    [TestCase("8/1k6/8/6B1/p1P5/2K1Pp2/8/8 w - - 0 1", +49, new[] { "g5h4", "c3d2", "c3d3" })]
    public void EvaluationSearch_NoDistanceToMateProvidedWhileWinning(string fen, int distanceToMate, string[] bestMoves)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, default, default);
        Assert.AreEqual(+CheckMateBaseEvaluation - distanceToMate, result);
    }

    [TestCase("8/1k6/8/8/p1P4B/2K1Pp2/8/8 b - - 1 1", -49)]
    public async Task RootSearch_NoDistanceToMateProvidedWhileLosing(string fen, int distanceToMate)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], default, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.NotZero(result.BestMove);
    }

    [TestCase("8/1k6/8/8/p1P4B/2K1Pp2/8/8 b - - 1 1", 49)]
    public void EvaluationSearch_NoDistanceToMateProvidedWhileLosing(string fen, int distanceToMate)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, default, default);
        Assert.AreEqual(-CheckMateBaseEvaluation + distanceToMate, result);
    }

    [TestCase("8/8/1p6/8/P1P3k1/P7/P6K/8 b - - 66 1", -49, new[] { "g4f4", "g4f5" })]
    public async Task RootSearch_MaybeLosing(string fen, int distanceToMate, string[] bestMoves)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], default, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.Contains(result.BestMove.UCIString(), bestMoves);
    }

    [TestCase("8/8/1p6/8/P1P3k1/P7/P6K/8 b - - 66 1", 49, new[] { "g4f4", "g4f5" })]
    public void EvaluationSearch_MaybeLosing(string fen, int distanceToMate, string[] bestMoves)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, default, default);
        Assert.AreEqual(-CheckMateBaseEvaluation + distanceToMate, result);
    }

    [TestCase("8/8/1p6/8/P1P2k2/P7/P6K/8 w - - 67 2", +49, new[] { "h2h3" })]
    public async Task RootSearch_MaybeWinning(string fen, int distanceToMate, string[] bestMoves)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], default, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.Contains(result.BestMove.UCIString(), bestMoves);
    }

    [TestCase("8/8/1p6/8/P1P2k2/P7/P6K/8 w - - 67 2", +49, new[] { "h2h3" })]
    public void EvaluationSearch_MaybeWinning(string fen, int distanceToMate, string[] bestMoves)
    {
        var position = new Position(fen);
        var result = OnlineTablebaseProber.EvaluationSearch(position, default, default);
        Assert.AreEqual(CheckMateBaseEvaluation - distanceToMate, result);
    }

    [TestCase("8/2PK4/1k6/8/8/8/8/8 w - - 0 1", 6, "c7c8q")]
    [TestCase("8/8/8/8/8/6K1/4kp2/8 b - - 0 1", 6, "f2f1q")]
    public async Task RootSearch_MoveOrderingWhenWinning(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], default, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [TestCase("8/2PK4/1k6/8/8/8/8/8 b - - 0 1", -8, "b6c5")]
    [TestCase("8/8/8/8/8/6K1/4kp2/8 w - - 0 1", -8, "g3f4")]
    public async Task RootSearch_MoveOrderingWhenLosing(string fen, int distanceToMate, string bestMove)
    {
        var position = new Position(fen);
        var result = await OnlineTablebaseProber.RootSearch(position, [], default, default);
        Assert.AreEqual(distanceToMate, result.MateScore);
        Assert.AreEqual(bestMove, result.BestMove.UCIString());
    }

    [Test]
    public async Task RootSearch_ForceThreefoldRepetitionWhenLosing()
    {
        // Arrange
        Game game = ParseGame();
        var position = game.CurrentPosition;

        // Act
        var result = await OnlineTablebaseProber.RootSearch(position, game.CopyPositionHashHistory(), game.HalfMovesWithoutCaptureOrPawnMove, default);

        // Assert
        Assert.AreEqual(0, result.MateScore);
        Assert.AreEqual("h8g7", result.BestMove.UCIString());

        game.MakeMove(result.BestMove);
        Assert.True(game.IsThreefoldRepetition());

        // Using local method due to async Span limitation
        static Game ParseGame()
        {
            Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            var game = PositionCommand.ParseGame("position fen 4K3/8/8/8/8/8/8/n3k2b w - - 0 1 moves e8d7 e1e2 d7d6 e2e3 d6c5 e3e4 c5d6 e4d4 d6e6 a1c2 e6f6 h1d5 f6f5 c2e1 f5f6 d5b3 f6f5 e1d3 f5f6 d4e4 f6g5 e4e5 g5g6 e5f4 g6h5 f4f5 h5h6 f5g4 h6g6 g4f4 g6h5 f4f5 h5h6 b3d1 h6g7 f5g5 g7f7 d1b3 f7g7 d3f4 g7h7 b3c4 h7g7 c4d3 g7f7 d3c4 f7g7 f4h5 g7h8 h5f4", movePool);
            return game;
        }
    }

    [Test]
    public async Task RootSearch_ForceThreefoldRepetitionWhenBlessedLosing()
    {
        // Arrange
        Game game = ParseGame();
        var position = game.CurrentPosition;

        // Act
        var result = await OnlineTablebaseProber.RootSearch(position, game.CopyPositionHashHistory(), game.HalfMovesWithoutCaptureOrPawnMove, default);

        // Assert
        Assert.AreEqual(0, result.MateScore);
        Assert.AreEqual("h8g7", result.BestMove.UCIString());

        game.MakeMove(result.BestMove);
        Assert.True(game.IsThreefoldRepetition());

        // Using local method due to async Span limitation
        static Game ParseGame()
        {
            Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            var game = PositionCommand.ParseGame("position fen 4K3/8/8/8/8/8/8/n3k2b w - - 40 1 moves e8d7 e1e2 d7d6 e2e3 d6c5 e3e4 c5d6 e4d4 d6e6 a1c2 e6f6 h1d5 f6f5 c2e1 f5f6 d5b3 f6f5 e1d3 f5f6 d4e4 f6g5 e4e5 g5g6 e5f4 g6h5 f4f5 h5h6 f5g4 h6g6 g4f4 g6h5 f4f5 h5h6 b3d1 h6g7 f5g5 g7f7 d1b3 f7g7 d3f4 g7h7 b3c4 h7g7 c4d3 g7f7 d3c4 f7g7 f4h5 g7h8 h5f4", movePool);
            return game;
        }
    }
}

#pragma warning restore S4144   // Methods should not have identical implementations
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Unused parameter.
