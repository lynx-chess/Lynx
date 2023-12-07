using Lynx.Model;
using NUnit.Framework;
using System.Numerics;
using static Lynx.EvaluationConstants;

namespace Lynx.Test;
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
        var (length, mask) = TranspositionTableExtensions.CalculateLength(sizeMb);
        Assert.AreEqual(length - 1, mask);
        Assert.AreEqual(0, length % 2);
        Assert.IsTrue(BitOperations.IsPow2(length));

        // Length: 100....000
        var lengthHexString = length.ToString("X");
        Assert.AreNotEqual('0', lengthHexString[0]);
        for (int i = 1; i < lengthHexString.Length; i++)
        {
            Assert.AreEqual('0', lengthHexString[i]);
        }

        // Mask: 111....11
        Assert.AreEqual(1, Convert.ToString(mask, 2).AsEnumerable().Distinct().Count());

        if (sizeMb <= 16)
        {
            for (int i = 0; i < length; ++i)
            {
                Verify(length, mask, i);
            }
        }
        else
        {
            for (int i = 0; i < length; i += Random.Shared.Next(100))
            {
                Verify(length, mask, i);
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

    [TestCase(CheckMateBaseEvaluation - 5 * CheckmateDepthFactor, 2, CheckMateBaseEvaluation - 7 * CheckmateDepthFactor)]
    [TestCase(CheckMateBaseEvaluation - 2 * CheckmateDepthFactor, 4, CheckMateBaseEvaluation - 6 * CheckmateDepthFactor)]

    [TestCase(-CheckMateBaseEvaluation + 5 * CheckmateDepthFactor, 2, -CheckMateBaseEvaluation + 7 * CheckmateDepthFactor)]
    [TestCase(-CheckMateBaseEvaluation + 2 * CheckmateDepthFactor, 4, -CheckMateBaseEvaluation + 6 * CheckmateDepthFactor)]
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
        var (mask, length) = TranspositionTableExtensions.CalculateLength(Configuration.EngineSettings.TranspositionTableSize);
        var transpositionTable = new TranspositionTableElement[length];

        transpositionTable.RecordHash(mask, position, depth: 5, ply: 3, eval: recordedEval, nodeType: recordNodeType, age: default, move: 1234);

        Assert.AreEqual(expectedProbeEval, transpositionTable.ProbeHash(mask, position, depth: 5, ply: 3, alpha: probeAlpha, beta: probeBeta).Evaluation);
    }

    [TestCase(CheckMateBaseEvaluation - 8 * CheckmateDepthFactor)]
    [TestCase(-CheckMateBaseEvaluation + 3 * CheckmateDepthFactor)]
    public void RecordHash_ProbeHash_CheckmateSameDepth(int recordedEval)
    {
        const int sharedDepth = 5;
        var position = new Position(Constants.InitialPositionFEN);
        var (mask, length) = TranspositionTableExtensions.CalculateLength(Configuration.EngineSettings.TranspositionTableSize);
        var transpositionTable = new TranspositionTableElement[length];

        transpositionTable.RecordHash(mask, position, depth: 10, ply: sharedDepth, eval: recordedEval, nodeType: NodeType.Exact, age: default, move: 1234);

        Assert.AreEqual(recordedEval, transpositionTable.ProbeHash(mask, position, depth: 7, ply: sharedDepth, alpha: 50, beta: 100).Evaluation);
    }

    [TestCase(CheckMateBaseEvaluation - 8 * CheckmateDepthFactor, 5, 4, CheckMateBaseEvaluation - 7 * CheckmateDepthFactor)]
    [TestCase(CheckMateBaseEvaluation - 8 * CheckmateDepthFactor, 5, 6, CheckMateBaseEvaluation - 9 * CheckmateDepthFactor)]
    [TestCase(-CheckMateBaseEvaluation + 8 * CheckmateDepthFactor, 5, 4, -CheckMateBaseEvaluation + 7 * CheckmateDepthFactor)]
    [TestCase(-CheckMateBaseEvaluation + 8 * CheckmateDepthFactor, 5, 6, -CheckMateBaseEvaluation + 9 * CheckmateDepthFactor)]
    public void RecordHash_ProbeHash_CheckmateDifferentDepth(int recordedEval, int recordedDeph, int probeDepth, int expectedProbeEval)
    {
        var position = new Position(Constants.InitialPositionFEN);
        var (mask, length) = TranspositionTableExtensions.CalculateLength(Configuration.EngineSettings.TranspositionTableSize);
        var transpositionTable = new TranspositionTableElement[length];

        transpositionTable.RecordHash(mask, position, depth: 10, ply: recordedDeph, eval: recordedEval, nodeType: NodeType.Exact, age: default, move: 1234);

        Assert.AreEqual(expectedProbeEval, transpositionTable.ProbeHash(mask, position, depth: 7, ply: probeDepth, alpha: 50, beta: 100).Evaluation);
    }
}