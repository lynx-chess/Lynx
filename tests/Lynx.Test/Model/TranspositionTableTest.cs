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

    [TestCase(CheckMateBaseEvaluation - 5, 2, CheckMateBaseEvaluation - 7)]
    [TestCase(CheckMateBaseEvaluation - 2, 4, CheckMateBaseEvaluation - 6)]

    [TestCase(-CheckMateBaseEvaluation + 5, 2, -CheckMateBaseEvaluation + 7)]
    [TestCase(-CheckMateBaseEvaluation + 2, 4, -CheckMateBaseEvaluation + 6)]
    public void RecalculateMateScores(int evaluation, int depth, int expectedEvaluation)
    {
        Assert.AreEqual(expectedEvaluation, TranspositionTable.RecalculateMateScores(evaluation, depth));
    }

    [TestCase(+19, NodeType.Alpha, +19)]
    [TestCase(+31, NodeType.Beta, +31)]
    public void RecordHash_ProbeHash(int recordedEval, NodeType recordNodeType, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTable();

        var staticEval = position.StaticEvaluation().Score;

        transpositionTable.RecordHash(position, staticEval, depth: 5, ply: 3, score: recordedEval, nodeType: recordNodeType, move: 1234);

        var ttEntry = transpositionTable.ProbeHash(position, ply: 3);
        Assert.AreEqual(expectedProbeEval, ttEntry.Score);
        Assert.AreEqual(staticEval, ttEntry.StaticEval);
    }

    [TestCase(CheckMateBaseEvaluation - 8)]
    [TestCase(-CheckMateBaseEvaluation + 3)]
    public void RecordHash_ProbeHash_CheckmateSameDepth(int recordedEval)
    {
        const int sharedDepth = 5;
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTable();

        transpositionTable.RecordHash(position, recordedEval, depth: 10, ply: sharedDepth, score: recordedEval, nodeType: NodeType.Exact, move: 1234);

        var ttEntry = transpositionTable.ProbeHash(position, ply: sharedDepth);
        Assert.AreEqual(recordedEval, ttEntry.Score);
        Assert.AreEqual(recordedEval, ttEntry.StaticEval);
    }

    [TestCase(CheckMateBaseEvaluation - 8, 5, 4, CheckMateBaseEvaluation - 7)]
    [TestCase(CheckMateBaseEvaluation - 8, 5, 6, CheckMateBaseEvaluation - 9)]
    [TestCase(-CheckMateBaseEvaluation + 8 , 5, 4, -CheckMateBaseEvaluation + 7)]
    [TestCase(-CheckMateBaseEvaluation + 8 , 5, 6, -CheckMateBaseEvaluation + 9)]
    public void RecordHash_ProbeHash_CheckmateDifferentDepth(int recordedEval, int recordedDeph, int probeDepth, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTable();

        transpositionTable.RecordHash(position, recordedEval, depth: 10, ply: recordedDeph, score: recordedEval, nodeType: NodeType.Exact, move: 1234);

        var ttEntry = transpositionTable.ProbeHash(position, ply: probeDepth);
        Assert.AreEqual(expectedProbeEval, ttEntry.Score);
        Assert.AreEqual(recordedEval, ttEntry.StaticEval);
    }
}