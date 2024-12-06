namespace Lynx;

using static EvaluationConstants;

public static class WDL
{
    /// <summary>
    /// Adjust score so that 100cp == 50% win probability
    /// Based on https://github.com/Ciekce/Stormphrax/blob/main/src/wdl.h
    /// </summary>
    public static int NormalizeScore(int score)
    {
        return (score == 0 || score > PositiveCheckmateDetectionLimit || score < NegativeCheckmateDetectionLimit)

            ? score
            : score * 100 / EvalNormalizationCoefficient;
    }

    /// <summary>
    /// Based on https://github.com/Ciekce/Stormphrax/blob/main/src/wdl.h
    /// </summary>
    public static int UnNormalizeScore(int normalizedScore)
    {
        return (normalizedScore == 0 || normalizedScore > PositiveCheckmateDetectionLimit || normalizedScore < NegativeCheckmateDetectionLimit)
            ? normalizedScore
            : normalizedScore * EvalNormalizationCoefficient / 100;
    }

    /// <summary>
    /// Based on https://github.com/Ciekce/Stormphrax/blob/main/src/wdl.cpp and https://github.com/official-stockfish/Stockfish/blob/master/src/uci.cpp
    /// </summary>
    public static (int WDLWin, int WDLDraw, int WDLLoss) WDLModel(int score, int ply)
    {
        // The model only captures up to 240 plies, so limit the input and then rescale
        double m = Math.Min(240, ply) / 64.0;

        double a = (((((As[0] * m) + As[1]) * m) + As[2]) * m) + As[3];
        double b = (((((Bs[0] * m) + Bs[1]) * m) + Bs[2]) * m) + Bs[3];

        // Transform the eval to centipawns with limited range
        double x = Math.Clamp(score, -4000.0, 4000.0);

        int wdlWin = (int)Math.Round(1000.0 / (1.0 + Math.Exp((a - x) / b)));
        int wdlLoss = (int)Math.Round(1000.0 / (1.0 + Math.Exp((a + x) / b)));
        int wdlDraw = 1000 - wdlWin - wdlLoss;

        return (wdlWin, wdlDraw, wdlLoss);
    }
}
