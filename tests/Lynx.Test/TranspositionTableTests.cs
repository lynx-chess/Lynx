using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;
public class TranspositionTableTests
{
    [TestCase(100_000, 1, 100_000)]
    [TestCase(100_000, 3, 100_000)]
    [TestCase(100_000, 5, 100_000)]
    [TestCase(EvaluationConstants.PositiveCheckmateDetectionLimit - 1, 5, EvaluationConstants.PositiveCheckmateDetectionLimit - 1)]
    [TestCase(-100_000, 1, -100_000)]
    [TestCase(-100_000, 3, -100_000)]
    [TestCase(-100_000, 5, -100_000)]
    [TestCase(EvaluationConstants.NegativeCheckmateDetectionLimit + 1, 5, EvaluationConstants.NegativeCheckmateDetectionLimit + 1)]

    [TestCase(EvaluationConstants.CheckMateBaseEvaluation - 5 * EvaluationConstants.DepthCheckmateFactor, 2, EvaluationConstants.CheckMateBaseEvaluation - 7 * EvaluationConstants.DepthCheckmateFactor)]
    [TestCase(EvaluationConstants.CheckMateBaseEvaluation - 2 * EvaluationConstants.DepthCheckmateFactor, 4, EvaluationConstants.CheckMateBaseEvaluation - 6 * EvaluationConstants.DepthCheckmateFactor)]

    [TestCase(-EvaluationConstants.CheckMateBaseEvaluation + 5 * EvaluationConstants.DepthCheckmateFactor, 2, -EvaluationConstants.CheckMateBaseEvaluation + 7 * EvaluationConstants.DepthCheckmateFactor)]
    [TestCase(-EvaluationConstants.CheckMateBaseEvaluation + 2 * EvaluationConstants.DepthCheckmateFactor, 4, -EvaluationConstants.CheckMateBaseEvaluation + 6 * EvaluationConstants.DepthCheckmateFactor)]
    public void RecalculateMateScores(int evaluation, int depth, int expectedEvaluation)
    {
        Assert.AreEqual(expectedEvaluation, TranspositionTableExtensions.RecalculateMateScores(evaluation, depth));
    }

    [TestCase(+19, NodeType.Alpha, +20, +30, 20)]
    [TestCase(+21, NodeType.Alpha, +20, +30, EvaluationConstants.NoHashEntry)]
    [TestCase(+29, NodeType.Beta, +20, +30, EvaluationConstants.NoHashEntry)]
    [TestCase(+31, NodeType.Beta, +20, +30, 30)]
    public void RecordHash_ProbeHash(int recordEval, NodeType recordNodeType, int probeAlpha, int probeBeta, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTableElement[Configuration.Hash];

        transpositionTable.RecordHash(position, depth: 3, maxDepth: 5, move: 1234, eval: recordEval, nodeType: recordNodeType);

        Assert.AreEqual(expectedProbeEval, transpositionTable.ProbeHash(position, maxDepth: 5, depth: 3, alpha: probeAlpha, beta: probeBeta));
    }
}
