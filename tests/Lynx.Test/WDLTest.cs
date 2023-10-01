using NUnit.Framework;

namespace Lynx.Test;

[Explicit]
[Category(Categories.Configuration)]
[NonParallelizable]
public class WDLTest
{
    /// <summary>
    /// Enforce that NormalizeToPawnValue corresponds to a 50% win rate at ply 64
    /// </summary>
    [Test]
    public void NormalizeCoefficientAndArrayValues()
    {
        Assert.AreEqual(EvaluationConstants.EvalNormalizationCoefficient, (int)EvaluationConstants.As.Sum());
    }

    [TestCase(500, 617)]
    [TestCase(1000, 1235)]
    [TestCase(0, 0)]
    [TestCase(EvaluationConstants.PositiveCheckmateDetectionLimit + 5, EvaluationConstants.PositiveCheckmateDetectionLimit + 5)]
    [TestCase(EvaluationConstants.NegativeCheckmateDetectionLimit - 5, EvaluationConstants.NegativeCheckmateDetectionLimit - 5)]
    public void NormalizeScore(int score, int expectecNormalizedEval)
    {
        Assert.AreEqual(expectecNormalizedEval, WDL.NormalizeScore(score));
    }

    [TestCase(1000, 810)]
    [TestCase(2000, 1620)]
    [TestCase(0, 0)]
    [TestCase(EvaluationConstants.PositiveCheckmateDetectionLimit + 5, EvaluationConstants.PositiveCheckmateDetectionLimit + 5)]
    [TestCase(EvaluationConstants.NegativeCheckmateDetectionLimit - 5, EvaluationConstants.NegativeCheckmateDetectionLimit - 5)]
    public void UnNormalizeScore(int score, int expectecNormalizedEval)
    {
        Assert.AreEqual(expectecNormalizedEval, WDL.UnNormalizeScore(score));
    }
}
