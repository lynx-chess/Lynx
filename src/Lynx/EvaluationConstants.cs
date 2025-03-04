#pragma warning disable IDE1006 // Naming Styles

namespace Lynx;

public static class EvaluationConstants
{
    /// <summary>
    /// 20_000 games, 20+0.2, 8moves_v3.epd, no draw or win adj.
    /// Retained (W,D,L) = (432747, 1652733, 434200) positions.
    /// </summary>
    public const int EvalNormalizationCoefficient = 99;

    public static ReadOnlySpan<double> As => [-3.65736087, 46.66362338, -38.24834086, 94.32750834];

    public static ReadOnlySpan<double> Bs => [-0.59179904, 16.00808254, -30.40319388, 61.53258225];

    public static ReadOnlySpan<int> GamePhaseByPiece =>
    [
        0, 1, 1, 2, 4, 0,
        0, 1, 1, 2, 4, 0
    ];

    public const int MaxPhase = 24;

    /// <summary>
    /// 2 x <see cref="Constants.AbsoluteMaxDepth"/> x <see cref="Constants.MaxNumberOfPossibleMovesInAPosition"/>
    /// </summary>
    public static readonly int[][][] LMRReductions = new int[2][][];

    /// <summary>
    /// [0, 4, 136, 276, 424, 580, 744, 916, 1096, 1284, 1480, 1684, 1896, 1896, 1896, 1896, ...]
    /// </summary>
    public static readonly int[] HistoryBonus = new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin];

    public const int LMRScaleFactor = 100;

    static EvaluationConstants()
    {
        var quietReductions = LMRReductions[0] = new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin][];
        var noisyReductions = LMRReductions[1] = new int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin][];

        for (int searchDepth = 1; searchDepth < Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin; ++searchDepth)    // Depth > 0 or we'd be in QSearch
        {
            quietReductions[searchDepth] = new int[Constants.MaxNumberOfPossibleMovesInAPosition];
            noisyReductions[searchDepth] = new int[Constants.MaxNumberOfPossibleMovesInAPosition];

            quietReductions[searchDepth][0] = 0;
            noisyReductions[searchDepth][0] = 0;

            for (int movesSearchedCount = 1; movesSearchedCount < Constants.MaxNumberOfPossibleMovesInAPosition; ++movesSearchedCount) // movesSearchedCount > 0 or we wouldn't be applying LMR
            {
                quietReductions[searchDepth][movesSearchedCount] = Convert.ToInt32(Math.Round(
                    LMRScaleFactor *
                    (Configuration.EngineSettings.LMR_Base_Quiet + (Math.Log(movesSearchedCount) * Math.Log(searchDepth) / Configuration.EngineSettings.LMR_Divisor_Quiet))));

                noisyReductions[searchDepth][movesSearchedCount] = Convert.ToInt32(Math.Round(
                    LMRScaleFactor *
                    (Configuration.EngineSettings.LMR_Base_Noisy + (Math.Log(movesSearchedCount) * Math.Log(searchDepth) / Configuration.EngineSettings.LMR_Divisor_Noisy))));
            }

            HistoryBonus[searchDepth] = Math.Min(
                Configuration.EngineSettings.History_MaxMoveRawBonus,
                (4 * searchDepth * searchDepth) + (120 * searchDepth) - 120);   // Sirius, originally from Berserk
        }

        quietReductions[0] = new int[Constants.MaxNumberOfPossibleMovesInAPosition];
        noisyReductions[0] = new int[Constants.MaxNumberOfPossibleMovesInAPosition];
    }

    /// <summary>
    /// MVV LVA [attacker,victim] 12x11
    /// Original based on
    /// https://github.com/maksimKorzh/chess_programming/blob/master/src/bbc/move_ordering_intro/bbc.c#L2406
    ///             (Victims)   Pawn Knight Bishop  Rook   Queen  King
    /// (Attackers)
    ///       Pawn              105    205    305    405    505    0
    ///     Knight              104    204    304    404    504    0
    ///     Bishop              103    203    303    403    503    0
    ///       Rook              102    202    302    402    502    0
    ///      Queen              101    201    301    401    501    0
    ///       King              100    200    300    400    500    0
    /// </summary>
    public static readonly int[][] MostValueableVictimLeastValuableAttacker =
    [         //    P     N     B     R      Q  K    p    n      b    r      q          k
        /* P */ [   0,    0,    0,    0,     0, 0,  1500, 4000, 4500, 5500, 11500 ], // 0],
        /* N */ [   0,    0,    0,    0,     0, 0,  1400, 3900, 4400, 5400, 11400 ], // 0],
        /* B */ [   0,    0,    0,    0,     0, 0,  1300, 3800, 4300, 5300, 11300 ], // 0],
        /* R */ [   0,    0,    0,    0,     0, 0,  1200, 3700, 4200, 5200, 11200 ], // 0],
        /* Q */ [   0,    0,    0,    0,     0, 0,  1100, 3600, 4100, 5100, 11100 ], // 0],
        /* K */ [   0,    0,    0,    0,     0, 0,  1000, 3500, 4001, 5000, 11000 ], // 0],
        /* p */ [1500, 4000, 4500, 5500, 11500, 0,     0,    0,    0,    0,     0 ], // 0],
        /* n */ [1400, 3900, 4400, 5400, 11400, 0,     0,    0,    0,    0,     0 ], // 0],
        /* b */ [1300, 3800, 4300, 5300, 11300, 0,     0,    0,    0,    0,     0 ], // 0],
        /* r */ [1200, 3700, 4200, 5200, 11200, 0,     0,    0,    0,    0,     0 ], // 0],
        /* q */ [1100, 3600, 4100, 5100, 11100, 0,     0,    0,    0,    0,     0 ], // 0],
        /* k */ [1000, 3500, 4001, 5000, 11000, 0,     0,    0,    0,    0,     0 ], // 0]
    ];

    public static ReadOnlySpan<int> MVV_PieceValues =>
    [
        1000, 3500, 4000, 5000, 11000, 0,
        1000, 3500, 4000, 5000, 11000, 0,
        0
    ];

    /// <summary>
    /// Base absolute checkmate evaluation value. Actual absolute evaluations are lower than this one
    /// </summary>
    public const int CheckMateBaseEvaluation = 29_000;

    /// <summary>
    /// Max eval, including checkmate values
    /// </summary>
    public const int MaxEval = 32_000;  // CheckMateBaseEvaluation + (Constants.AbsoluteMaxDepth + 45) * 10;

    /// <summary>
    /// Min eval, including checkmate values
    /// </summary>
    public const int MinEval = -32_000;    // -CheckMateBaseEvaluation - (Constants.AbsoluteMaxDepth + 45) * 10;

    /// <summary>
    /// Minimum evaluation for a position to be White checkmate
    /// </summary>
    public const int PositiveCheckmateDetectionLimit = 26_000; // CheckMateBaseEvaluation - (Constants.AbsoluteMaxDepth + 45) * 10;

    /// <summary>
    /// Maximum evaluation for a position to be Black checkmate
    /// </summary>
    public const int NegativeCheckmateDetectionLimit = -26_000; // -CheckMateBaseEvaluation + (Constants.AbsoluteMaxDepth + 45) * 10;

    public const int MaxMate = 1500; // Utils.CalculateMateInX(PositiveCheckmateDetectionLimit + 1);

    public const int MinMate = -1500; // Utils.CalculateMateInX(NegativeCheckmateDetectionLimit -1);

    /// <summary>
    /// Max static eval. It doesn't include checkmate values and it's below <see cref="PositiveCheckmateDetectionLimit"/>
    /// </summary>
    public const int MaxStaticEval = PositiveCheckmateDetectionLimit - 1;

    /// <summary>
    /// Min static eval. It doesn't include checkmate values and it's above <see cref="NegativeCheckmateDetectionLimit"/>
    /// </summary>
    public const int MinStaticEval = NegativeCheckmateDetectionLimit + 1;

    /// <summary>
    /// Outside of the evaluation ranges (higher than any sensible evaluation, lower than <see cref="PositiveCheckmateDetectionLimit"/>)
    /// </summary>
    public const int NoHashEntry = 25_000;

    /// <summary>
    /// Evaluation to be returned when there's one single legal move
    /// </summary>
    public const int SingleMoveScore = 666;

    #region Move ordering

    public const int TTMoveScoreValue = 2_097_152;

    public const int QueenPromotionWithCaptureBaseValue = GoodCaptureMoveBaseScoreValue + PromotionMoveScoreValue;

    public const int GoodCaptureMoveBaseScoreValue = 1_048_576;

    public const int FirstKillerMoveValue = 524_288;

    public const int SecondKillerMoveValue = 262_144;

    public const int ThirdKillerMoveValue = 131_072;

    public const int CounterMoveValue = 65_536;

    // Revisit bad capture pruning in NegaMax.cs if order changes and promos aren't the lowest before bad captures
    public const int PromotionMoveScoreValue = 32_768;

    public const int BadCaptureMoveBaseScoreValue = 16_384;

    //public const int MaxHistoryMoveValue => Configuration.EngineSettings.MaxHistoryMoveValue;

    /// <summary>
    /// Negative offset to ensure history move scores don't reach other move ordering values
    /// </summary>
    public const int BaseMoveScore = int.MinValue / 2;

    #endregion

    public const int ContinuationHistoryPlyCount = 1;
}

#pragma warning restore IDE1006 // Naming Styles
