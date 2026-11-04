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

        Assert.True(transpositionTable.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: 3, out var ttEntry));
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

        Assert.True(transpositionTable.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: sharedDepth, out var ttEntry));
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

        Assert.True(transpositionTable.ProbeHash(position, halfMovesWithoutCaptureOrPawnMove: 0, ply: probeDepth, out var ttEntry));
        Assert.AreEqual(expectedProbeEval, ttEntry.Score);
        Assert.AreEqual(recordedEval, ttEntry.StaticEval);
    }

    [TestCase(NodeType.Unknown, 0, 0)]
    [TestCase(NodeType.Exact, 1, 31)]
    [TestCase(NodeType.Alpha, 0, 17)]
    [TestCase(NodeType.Beta, 1, 8)]
    [TestCase(NodeType.Beta, 0, 31)]
    public void AgeTypeWasPvFlags(NodeType nodeType, int wasPv, int age)
    {
        var element = new TranspositionTableElement();

        element.Update(key: 0xABCD, score: -123, staticEval: 456, depth: 17, nodeType, wasPv, move: 789, age);

        Assert.AreEqual(0xABCD, element.Key);
        Assert.AreEqual(-123, element.Score);
        Assert.AreEqual(456, element.StaticEval);
        Assert.AreEqual(17, element.Depth);
        Assert.AreEqual(789, element.Move);
        Assert.AreEqual(nodeType, element.Type);
        Assert.AreEqual(wasPv == 1, element.WasPv);
        Assert.AreEqual(age, element.Age);
    }

    /// <summary>
    /// Every <see cref="Engine"/> keeps its own copy of the <see cref="TranspositionTable"/> struct, and <see cref="Searcher"/> is the only one bumping the age.
    /// </summary>
    [Test]
    public unsafe void BumpAge_IsVisibleThroughCopiesOfTheStruct()
    {
        var tt = new TranspositionTable();
        try
        {
            tt.Clear();

            var engineCopy = tt;
            Assert.AreEqual(0, engineCopy.Age);

            tt.BumpAge();
            tt.BumpAge();
            tt.BumpAge();

            Assert.AreEqual(3, tt.Age);
            Assert.AreEqual(3, engineCopy.Age);

            var position = new Position(Constants.InitialPositionFEN);
            engineCopy.RecordHash(position, halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 1, depth: 5, ply: 0, score: 10, NodeType.Exact, wasPv: false, move: 1234);
            Assert.AreEqual(3, FindOnlyRecordedEntry(tt)->Age);

            engineCopy.SaveStaticEval(new Position(Constants.InitialPositionFEN), halfMovesWithoutCaptureOrPawnMove: 0, staticEval: 2, wasPv: false);
            Assert.AreEqual(3, FindOnlyRecordedEntry(tt)->Age);
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public void BumpAge_CircularBuffer()
    {
        var tt = new TranspositionTable();
        try
        {
            for (int i = 1; i < TranspositionTableElement.MaxAge; ++i)
            {
                tt.BumpAge();
                Assert.AreEqual(i, tt.Age);
            }

            tt.BumpAge();
            Assert.AreEqual(0, tt.Age);
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public void Clear_ResetsAge()
    {
        var tt = new TranspositionTable();
        try
        {
            tt.BumpAge();
            tt.BumpAge();
            Assert.AreEqual(2, tt.Age);

            tt.Clear();

            Assert.AreEqual(0, tt.Age);
        }
        finally
        {
            tt.Release();
        }
    }

    [Test]
    public unsafe void Allocation_IsCacheLineAligned()
    {
        var tt = new TranspositionTable();
        try
        {
            Assert.AreEqual(0UL, (ulong)tt.Get(0) % 64);
        }
        finally
        {
            tt.Release();
        }
    }

    private static unsafe TranspositionTableElement* FindOnlyRecordedEntry(TranspositionTable tt)
    {
        TranspositionTableElement* found = null;

        for (int index = 0; index < (int)tt.Length; ++index)
        {
            var entry = tt.Get(index);
            if (entry->Key != 0)
            {
                Assert.IsTrue(found is null, "More than one entry recorded");
                found = entry;
            }
        }

        Assert.IsTrue(found is not null, "No entry recorded");
        return found!;
    }

    [Test]
    [Explicit]
    [NonParallelizable]
    [Category(Categories.LongRunning)]
    public unsafe void ClearTT()
    {
        Configuration.EngineSettings.TranspositionTableSize = 31;
        Configuration.EngineSettings.Threads = 7;

        var tt = new TranspositionTable();

        Assert.AreNotEqual(0, (int)tt.Length % Configuration.EngineSettings.Threads, "We want to test the edge case where the last thread clears more items than the rest");

        for (int index = 0; index < (int)tt.Length; ++index)
        {
            var ttEntry = tt.Get(index);
            ttEntry->Update(1, 2, 3, 4, NodeType.Exact, 5, 6, 7);
        }

        tt.Clear();

        for (int index = 0; index < (int)tt.Length; ++index)
        {
            var ttEntry = tt.Get(index);

            Assert.AreEqual(0, ttEntry->Age);
            Assert.AreEqual(0, ttEntry->Score);
            Assert.AreEqual(0, ttEntry->StaticEval);
            Assert.AreEqual(0, ttEntry->Depth);
            Assert.AreEqual(NodeType.Unknown, ttEntry->Type);
            Assert.AreEqual(false, ttEntry->WasPv);
            Assert.AreEqual(0, ttEntry->Move);
            Assert.AreEqual(0, ttEntry->Key);
        }
    }

    /// <summary>
    /// Ensures <see cref="SingleArrayTranspositionTable.HashfullPermillApprox"/> works correctly
    /// </summary>
    [Test]
    [Explicit]
    [NonParallelizable]
    public void TranspositionTableSize()
    {
        Configuration.EngineSettings.TranspositionTableSize = 1;
        var tt = new TranspositionTable();

        Assert.GreaterOrEqual(tt.Length, 1000);
    }
}