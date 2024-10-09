using Lynx.Model;
using NUnit.Framework;
using static Lynx.EvaluationConstants;

namespace Lynx.Test.Model;
public class TranspositionTableTests
{
    [TestCase(10_000, 1, 10_000)]
    [TestCase(10_000, 5, 10_000)]
    [TestCase(10_000, 3, 10_000)]
    [TestCase(PositiveCheckmateDetectionLimit - 1, 5, PositiveCheckmateDetectionLimit - 1)]
    [TestCase(-10_000, 1, -10_000)]
    [TestCase(-10_000, 3, -10_000)]
    [TestCase(-10_000, 5, -10_000)]
    [TestCase(NegativeCheckmateDetectionLimit + 1, 5, NegativeCheckmateDetectionLimit + 1)]

    [TestCase(CheckMateBaseEvaluation - (5 * CheckmateDepthFactor), 2, CheckMateBaseEvaluation - (7 * CheckmateDepthFactor))]
    [TestCase(CheckMateBaseEvaluation - (2 * CheckmateDepthFactor), 4, CheckMateBaseEvaluation - (6 * CheckmateDepthFactor))]

    [TestCase(-CheckMateBaseEvaluation + (5 * CheckmateDepthFactor), 2, -CheckMateBaseEvaluation + (7 * CheckmateDepthFactor))]
    [TestCase(-CheckMateBaseEvaluation + (2 * CheckmateDepthFactor), 4, -CheckMateBaseEvaluation + (6 * CheckmateDepthFactor))]
    public void RecalculateMateScores(int evaluation, int depth, int expectedEvaluation)
    {
        Assert.AreEqual(expectedEvaluation, TranspositionTableExtensions.RecalculateMateScores(evaluation, depth));
    }

    [TestCase(+19, NodeType.Alpha, +20, +30, +19)]
    [TestCase(+21, NodeType.Alpha, +20, +30, NoHashEntry)]
    [TestCase(+29, NodeType.Beta, +20, +30, NoHashEntry)]
    [TestCase(+31, NodeType.Beta, +20, +30, +31)]
    public void RecordHash_ProbeHash(int recordedEval, NodeType recordNodeType, int probeAlpha, int probeBeta, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var ttLength = TranspositionTableExtensions.CalculateLength(Configuration.EngineSettings.TranspositionTableSize);
        var transpositionTable = new TranspositionTableElement[ttLength];
        var staticEval = position.StaticEvaluation().Score;

        transpositionTable.RecordHash(position, staticEval, depth: 5, ply: 3, score: recordedEval, nodeType: recordNodeType, move: 1234);

        var ttEntry = transpositionTable.ProbeHash(position, depth: 5, ply: 3, alpha: probeAlpha, beta: probeBeta);
        Assert.AreEqual(expectedProbeEval, ttEntry.Score);
        Assert.AreEqual(staticEval, ttEntry.StaticEval);
    }

    [TestCase(CheckMateBaseEvaluation - (8 * CheckmateDepthFactor))]
    [TestCase(-CheckMateBaseEvaluation + (3 * CheckmateDepthFactor))]
    public void RecordHash_ProbeHash_CheckmateSameDepth(int recordedEval)
    {
        const int sharedDepth = 5;
        var position = new Position(Constants.InitialPositionFEN);
        var ttLength = TranspositionTableExtensions.CalculateLength(Configuration.EngineSettings.TranspositionTableSize);
        var transpositionTable = new TranspositionTableElement[ttLength];

        transpositionTable.RecordHash(position, recordedEval, depth: 10, ply: sharedDepth, score: recordedEval, nodeType: NodeType.Exact, move: 1234);

        var ttEntry = transpositionTable.ProbeHash(position, depth: 7, ply: sharedDepth, alpha: 50, beta: 100);
        Assert.AreEqual(recordedEval, ttEntry.Score);
        Assert.AreEqual(recordedEval, ttEntry.StaticEval);
    }

    [TestCase(CheckMateBaseEvaluation - (8 * CheckmateDepthFactor), 5, 4, CheckMateBaseEvaluation - (7 * CheckmateDepthFactor))]
    [TestCase(CheckMateBaseEvaluation - (8 * CheckmateDepthFactor), 5, 6, CheckMateBaseEvaluation - (9 * CheckmateDepthFactor))]
    [TestCase(-CheckMateBaseEvaluation + (8 * CheckmateDepthFactor), 5, 4, -CheckMateBaseEvaluation + (7 * CheckmateDepthFactor))]
    [TestCase(-CheckMateBaseEvaluation + (8 * CheckmateDepthFactor), 5, 6, -CheckMateBaseEvaluation + (9 * CheckmateDepthFactor))]
    public void RecordHash_ProbeHash_CheckmateDifferentDepth(int recordedEval, int recordedDeph, int probeDepth, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var ttLength = TranspositionTableExtensions.CalculateLength(Configuration.EngineSettings.TranspositionTableSize);
        var transpositionTable = new TranspositionTableElement[ttLength];

        transpositionTable.RecordHash(position, recordedEval, depth: 10, ply: recordedDeph, score: recordedEval, nodeType: NodeType.Exact, move: 1234);

        var ttEntry = transpositionTable.ProbeHash(position, depth: 7, ply: probeDepth, alpha: 50, beta: 100);
        Assert.AreEqual(expectedProbeEval, ttEntry.Score);
        Assert.AreEqual(recordedEval, ttEntry.StaticEval);
    }
}