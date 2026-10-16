using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test.BestMove;

public class QuiescenceTest : BaseTest
{
    [TestCase("r2qkb1r/ppp2ppp/2n2n2/1B1p1b2/3P4/2N2N2/PPP2PPP/R1BQ1RK1 b kq - 0 1", 1, 12,
        null,
        new[] { "f5c2", "f5d3", "f5h3", "f8c5", "f8a3" },
        Description = "Avoid trading pawn for minor piece or sacrificing pieces for nothing")]
    [TestCase("r2qkb1r/1pp2ppp/p1n2n2/1B1p1b2/3P4/2N2N2/PPP2PPP/R1BQ1RK1 w kq - 0 2", 1, 12,
        new[] { "b5c6", "b5a4", "f1e1", "f3h4", "d1e1", "b5d3" },
        new[] { "b5c1", "c3d5" },
        Description = "Originally, it captured in c1")]
    [TestCase("r1bq1b1r/ppppk2p/2n1pp2/3n2B1/3P4/P4N2/1PP2PPP/RN1QKB1R w KQ - 0 1", 1, 12,
        new[] { "g5h4", "g5e3", "g5d2", "g5c1", "c2c4" },
        new[] { "a3a4" },
        Description = "Avoid allowing pieces to be captured")]
    [TestCase("2kr3q/pbppppp1/1p1P3r/4bB2/1n2n1Q1/8/PPPPNBPP/R4RK1 b Q - 0 1", 3, 12,
        new[] { "e5h2" },
        Description = "Mate in 6 with quiescence, https://gameknot.com/chess-puzzle.pl?pz=257112",
        Ignore = "Fails after fixing LMR implementation")]
#pragma warning disable RCS1163, IDE0060 // Unused parameter.
    public void Quiescence(string fen, int depth, int minQuiescenceSearchDepth, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
#pragma warning restore RCS1163, IDE0060 // Unused parameter.
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth);
    }

    /// <summary>
    /// 8   R . . . . . k .
    /// 7   . . . . . p p p
    /// 6   . . . . . . . .
    /// 5   . . . . p . . .
    /// 4   . . . . . . . .
    /// 3   . . . . . . . .
    /// 2   P . . . r . . .
    /// 1   . . . . . . K .
    ///     a b c d e f g h
    /// Checkmate by Ra8, but black has a (pseudo-legal) capture: Rxa2, which doesn't get the king out of check.
    /// In check, QSearch looks for a final position if there isn't any valid capture, regardless of the (pseudo-legal) captures that exist
    /// </summary>
    [Test]
    public void QuiescenceSearch_DetectsCheckmate_WhenCapturesExistButNoneGetsTheKingOutOfCheck()
    {
        var engine = GetEngine("R5k1/5ppp/8/4p3/8/8/P3r3/6K1 b - - 0 1");

        var score = engine.QuiescenceSearch(0, EvaluationConstants.MinEval, EvaluationConstants.MaxEval, pvNode: true, CancellationToken.None);

        Assert.AreEqual(Position.EvaluateFinalPosition(0, isInCheck: true), score);
    }

    /// <summary>
    /// Same as <see cref="QuiescenceSearch_DetectsCheckmate_WhenCapturesExistButNoneGetsTheKingOutOfCheck"/>, but the rook can block the check (Re8), so it isn't a final position
    /// </summary>
    [Test]
    public void QuiescenceSearch_DoesNotDetectCheckmate_WhenCapturesExistButAnotherMoveGetsTheKingOutOfCheck()
    {
        var engine = GetEngine("R5k1/5ppp/8/8/8/8/P3r3/6K1 b - - 0 1");

        var score = engine.QuiescenceSearch(0, EvaluationConstants.MinEval, EvaluationConstants.MaxEval, pvNode: true, CancellationToken.None);

        Assert.Less(Math.Abs(score), EvaluationConstants.PositiveCheckmateDetectionLimit);
    }

    /// <summary>
    /// Checkmate by Ra8, and black doesn't have any capture at all
    /// </summary>
    [Test]
    public void QuiescenceSearch_DetectsCheckmate_WhenThereAreNoCaptures()
    {
        var engine = GetEngine("R5k1/5ppp/8/8/8/8/8/6K1 b - - 0 1");

        var score = engine.QuiescenceSearch(0, EvaluationConstants.MinEval, EvaluationConstants.MaxEval, pvNode: true, CancellationToken.None);

        Assert.AreEqual(Position.EvaluateFinalPosition(0, isInCheck: true), score);
    }

    /// <summary>
    /// Ra8 gives check, black doesn't have any capture, but the king can move to h7, so it's not a final position
    /// </summary>
    [Test]
    public void QuiescenceSearch_DoesNotDetectCheckmate_WhenThereAreNoCapturesButTheKingCanMove()
    {
        var engine = GetEngine("R5k1/5pp1/7p/8/8/8/8/6K1 b - - 0 1");

        var score = engine.QuiescenceSearch(0, EvaluationConstants.MinEval, EvaluationConstants.MaxEval, pvNode: true, CancellationToken.None);

        Assert.Less(Math.Abs(score), EvaluationConstants.PositiveCheckmateDetectionLimit);
    }

    /// <summary>
    /// In check, and one of the captures (Rxa8) gets the king out of check
    /// </summary>
    [Test]
    public void QuiescenceSearch_SearchesCaptureThatGetsTheKingOutOfCheck()
    {
        var engine = GetEngine("R5k1/5ppp/8/8/8/8/r7/6K1 b - - 0 1");

        var score = engine.QuiescenceSearch(0, EvaluationConstants.MinEval, EvaluationConstants.MaxEval, pvNode: true, CancellationToken.None);

        Assert.Less(Math.Abs(score), EvaluationConstants.PositiveCheckmateDetectionLimit);
        Assert.Greater(score, 0, "Black wins a rook");
    }

    /// <summary>
    /// 8   . . . . . . . k
    /// 7   . . . . . Q . .
    /// 6   . . . . . . K .
    ///     a b c d e f g h
    /// Stalemate: black isn't in check, doesn't have any captures, and the king has no legal moves.
    /// Final positions are looked for when not in check only in sparse positions, where stalemates are
    /// </summary>
    [Test]
    public void QuiescenceSearch_DetectsStalemate_WhenThereAreNoCapturesAndFewPieces()
    {
        var engine = GetEngine("7k/5Q2/6K1/8/8/8/8/8 b - - 0 1");

        var score = engine.QuiescenceSearch(0, EvaluationConstants.MinEval, EvaluationConstants.MaxEval, pvNode: true, CancellationToken.None);

        Assert.AreEqual(Position.EvaluateFinalPosition(0, isInCheck: false), score);
    }

    /// <summary>
    /// 8   . . . . . . . k
    /// 7   p . . . . Q . .
    /// 6   . . . . . . K .
    ///     a b c d e f g h
    /// Same as <see cref="QuiescenceSearch_DetectsStalemate_WhenThereAreNoCapturesAndFewPieces"/>, but black has a pawn that can move (a6), so it isn't a final position
    /// </summary>
    [Test]
    public void QuiescenceSearch_DoesNotDetectStalemate_WhenThereIsALegalMove()
    {
        var engine = GetEngine("7k/p4Q2/6K1/8/8/8/8/8 b - - 0 1");

        var score = engine.QuiescenceSearch(0, EvaluationConstants.MinEval, EvaluationConstants.MaxEval, pvNode: true, CancellationToken.None);

        Assert.AreNotEqual(Position.EvaluateFinalPosition(0, isInCheck: false), score);
    }

    [TestCase("7k/8/5NQ1/8/8/KN6/8/1r6 b - - 0 1", 1, new[] { "b1b3" })]
    [TestCase("5Rbk/8/5N2/6Q1/8/1r6/8/KN6 b - - 0 1", 1, new[] { "b3b1" })]
    public void DetectDrawWhenNoCaptures(string fen, int depth, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString = null)
    {
        TestBestMove(fen, allowedUCIMoveString, excludedUCIMoveString, depth);
    }
}
