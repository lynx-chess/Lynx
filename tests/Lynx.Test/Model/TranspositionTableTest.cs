using Lynx.Model;
using NUnit.Framework;
using System.Numerics;
using static Lynx.EvaluationConstants;

namespace Lynx.Test.Model;
public class TranspositionTableTests
{
    [TestCase(2)]
    [TestCase(4)]
    [TestCase(8)]
    [TestCase(16)]
    [TestCase(32)]
    [TestCase(64)]
    [TestCase(128)]
    [TestCase(256)]
    [TestCase(512)]
    [TestCase(1024)]
    public void TranspositionTableLength(int sizeMb)
    {
        var ttLength = TranspositionTableExtensions.CalculateLength(sizeMb);
        var mask = ttLength - 1;
        Assert.AreEqual(0, ttLength % 2);
        Assert.IsTrue(BitOperations.IsPow2(ttLength));

        // Length: 100....000
        var lengthHexString = ttLength.ToString("X");
        Assert.AreNotEqual('0', lengthHexString[0]);
        for (int i = 1; i < lengthHexString.Length; i++)
        {
            Assert.AreEqual('0', lengthHexString[i]);
        }

        // Mask: 111....11
        Assert.AreEqual(1, Convert.ToString(mask, 2).AsEnumerable().Distinct().Count());

        if (sizeMb <= 16)
        {
            for (int i = 0; i < ttLength; ++i)
            {
                Verify(ttLength, mask, i);
            }
        }
        else
        {
            for (int i = 0; i < ttLength; i += Random.Shared.Next(100))
            {
                Verify(ttLength, mask, i);
            }
        }

        static void Verify(int length, int mask, int i)
        {
            Assert.AreEqual(i % length, i & mask, $"Error in {i}: {i} %{length} should be {i} & 0x{mask:X}");
        }
    }

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

        transpositionTable.RecordHash(position, depth: 5, ply: 3, score: recordedEval, nodeType: recordNodeType, move: 1234);

        Assert.AreEqual(expectedProbeEval, transpositionTable.ProbeHash(position, depth: 5, ply: 3, alpha: probeAlpha, beta: probeBeta).Score);
    }

    [TestCase(CheckMateBaseEvaluation - (8 * CheckmateDepthFactor))]
    [TestCase(-CheckMateBaseEvaluation + (3 * CheckmateDepthFactor))]
    public void RecordHash_ProbeHash_CheckmateSameDepth(int recordedEval)
    {
        const int sharedDepth = 5;
        var position = new Position(Constants.InitialPositionFEN);
        var ttLength = TranspositionTableExtensions.CalculateLength(Configuration.EngineSettings.TranspositionTableSize);
        var transpositionTable = new TranspositionTableElement[ttLength];

        transpositionTable.RecordHash(position, depth: 10, ply: sharedDepth, score: recordedEval, nodeType: NodeType.Exact, move: 1234);

        Assert.AreEqual(recordedEval, transpositionTable.ProbeHash(position, depth: 7, ply: sharedDepth, alpha: 50, beta: 100).Score);
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

        transpositionTable.RecordHash(position, depth: 10, ply: recordedDeph, score: recordedEval, nodeType: NodeType.Exact, move: 1234);

        Assert.AreEqual(expectedProbeEval, transpositionTable.ProbeHash(position, depth: 7, ply: probeDepth, alpha: 50, beta: 100).Score);
    }
}