using Lynx.Model;
using NUnit.Framework;
using static Lynx.EvaluationConstants;

namespace Lynx.Test;
public class TranspositionTableTests
{
    [TestCase(100_000, 1, 100_000)]
    [TestCase(100_000, 3, 100_000)]
    [TestCase(100_000, 5, 100_000)]
    [TestCase(PositiveCheckmateDetectionLimit - 1, 5, PositiveCheckmateDetectionLimit - 1)]
    [TestCase(-100_000, 1, -100_000)]
    [TestCase(-100_000, 3, -100_000)]
    [TestCase(-100_000, 5, -100_000)]
    [TestCase(NegativeCheckmateDetectionLimit + 1, 5, NegativeCheckmateDetectionLimit + 1)]

    [TestCase(CheckMateBaseEvaluation - 5 * DepthCheckmateFactor, 2, CheckMateBaseEvaluation - 7 * DepthCheckmateFactor)]
    [TestCase(CheckMateBaseEvaluation - 2 * DepthCheckmateFactor, 4, CheckMateBaseEvaluation - 6 * DepthCheckmateFactor)]

    [TestCase(-CheckMateBaseEvaluation + 5 * DepthCheckmateFactor, 2, -CheckMateBaseEvaluation + 7 * DepthCheckmateFactor)]
    [TestCase(-CheckMateBaseEvaluation + 2 * DepthCheckmateFactor, 4, -CheckMateBaseEvaluation + 6 * DepthCheckmateFactor)]
    public void RecalculateMateScores(int evaluation, int depth, int expectedEvaluation)
    {
        Assert.AreEqual(expectedEvaluation, TranspositionTableExtensions.RecalculateMateScores(evaluation, depth));
    }

    [TestCase(+19, NodeType.Alpha, +20, +30, 20)]
    [TestCase(+21, NodeType.Alpha, +20, +30, NoHashEntry)]
    [TestCase(+29, NodeType.Beta, +20, +30, NoHashEntry)]
    [TestCase(+31, NodeType.Beta, +20, +30, 30)]
    public void RecordHash_ProbeHash(int recordedEval, NodeType recordNodeType, int probeAlpha, int probeBeta, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTableElement[Configuration.Hash];

        transpositionTable.RecordHash(position, maxDepth: 5, ply: 3, move: 1234, eval: recordedEval, nodeType: recordNodeType);

        Assert.AreEqual(expectedProbeEval, transpositionTable.ProbeHash(position, maxDepth: 5, depth: 3, alpha: probeAlpha, beta: probeBeta));
    }

    [TestCase(CheckMateBaseEvaluation - 8 * DepthCheckmateFactor)]
    [TestCase(-CheckMateBaseEvaluation + 3 * DepthCheckmateFactor)]
    public void RecordHash_ProbeHash_CheckmateSameDepth(int recordedEval)
    {
        const int sharedDepth = 5;
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTableElement[Configuration.Hash];

        transpositionTable.RecordHash(position, maxDepth: 10, ply: sharedDepth, move: 1234, eval: recordedEval, nodeType: NodeType.Exact);

        Assert.AreEqual(recordedEval, transpositionTable.ProbeHash(position, maxDepth: 7, depth: sharedDepth, alpha: 50, beta: 100));
    }

    [TestCase(CheckMateBaseEvaluation - 8 * DepthCheckmateFactor, 5, 4, CheckMateBaseEvaluation - 7 * DepthCheckmateFactor)]
    [TestCase(CheckMateBaseEvaluation - 8 * DepthCheckmateFactor, 5, 6, CheckMateBaseEvaluation - 9 * DepthCheckmateFactor)]
    [TestCase(-CheckMateBaseEvaluation + 8 * DepthCheckmateFactor, 5, 4, -CheckMateBaseEvaluation + 7 * DepthCheckmateFactor)]
    [TestCase(-CheckMateBaseEvaluation + 8 * DepthCheckmateFactor, 5, 6, -CheckMateBaseEvaluation + 9 * DepthCheckmateFactor)]
    public void RecordHash_ProbeHash_CheckmateDifferentDepth(int recordedEval, int recordedDeph, int probeDepth, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTableElement[Configuration.Hash];

        transpositionTable.RecordHash(position, maxDepth: 10, ply: recordedDeph, move: 1234, eval: recordedEval, nodeType: NodeType.Exact);

        Assert.AreEqual(expectedProbeEval, transpositionTable.ProbeHash(position, maxDepth: 7, depth: probeDepth, alpha: 50, beta: 100));
    }
}
