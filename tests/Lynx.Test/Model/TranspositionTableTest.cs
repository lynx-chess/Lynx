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

        transpositionTable.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval, depth: 5, ply: 3, score: recordedEval, nodeType: recordNodeType, false, move: recordNodeType != NodeType.Alpha ? 1234 : null);

        var ttEntry = transpositionTable.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: 3);
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

        transpositionTable.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, recordedEval, depth: 10, ply: sharedDepth, score: recordedEval, nodeType: NodeType.Exact, false, move: 1234);

        var ttEntry = transpositionTable.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: sharedDepth);
        Assert.AreEqual(recordedEval, ttEntry.Score);
        Assert.AreEqual(recordedEval, ttEntry.StaticEval);
    }

    [TestCase(CheckMateBaseEvaluation - 8, 5, 4, CheckMateBaseEvaluation - 7)]
    [TestCase(CheckMateBaseEvaluation - 8, 5, 6, CheckMateBaseEvaluation - 9)]
    [TestCase(-CheckMateBaseEvaluation + 8, 5, 4, -CheckMateBaseEvaluation + 7)]
    [TestCase(-CheckMateBaseEvaluation + 8, 5, 6, -CheckMateBaseEvaluation + 9)]
    public void RecordHash_ProbeHash_CheckmateDifferentDepth(int recordedEval, int recordedDeph, int probeDepth, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var transpositionTable = new TranspositionTable();

        transpositionTable.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, recordedEval, depth: 10, ply: recordedDeph, score: recordedEval, nodeType: NodeType.Exact, false, move: 1234);

        var ttEntry = transpositionTable.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: probeDepth);
        Assert.AreEqual(expectedProbeEval, ttEntry.Score);
        Assert.AreEqual(recordedEval, ttEntry.StaticEval);
    }

    [Test]
    [Explicit]
    [NonParallelizable]
    [Category(Categories.LongRunning)]
    public void ClearTT()
    {
        Configuration.EngineSettings.TranspositionTableSize = 31;
        Configuration.EngineSettings.Threads = 7;

        var tt = new TranspositionTable();

        Assert.AreNotEqual(0, tt.Length % Configuration.EngineSettings.Threads, "We want to test the edge case where the last thread clears more items than the rest");

        for (int index = 0; index < tt.Length; ++index)
        {
            ref var ttEntry = ref tt.Get(index);
            ttEntry.Update(1, 2, 3, 4, NodeType.Exact, 5, 6);
        }

        tt.Clear();

        for (int index = 0; index < tt.Length; ++index)
        {
            var ttEntry = tt.Get(index);

            Assert.AreEqual(0, ttEntry.Score);
            Assert.AreEqual(0, ttEntry.StaticEval);
            Assert.AreEqual(0, ttEntry.Depth);
            Assert.AreEqual(NodeType.Unknown, ttEntry.Type);
            Assert.AreEqual(false, ttEntry.WasPv);
            Assert.AreEqual(0, ttEntry.Move);
            Assert.AreEqual(0, ttEntry.Key);
        }
    }
}