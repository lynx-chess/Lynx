using System.Text.Json.Serialization;

namespace Lynx;

public static class Configuration
{
    public static EngineSettings EngineSettings { get; set; } = new EngineSettings();
    public static GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();

    private static int _isDebug;
#pragma warning disable IDE1006 // Naming Styles
    private static int _UCI_AnalyseMode;
#pragma warning restore IDE1006 // Naming Styles

    public static bool IsDebug
    {
        get => Interlocked.CompareExchange(ref _isDebug, 1, 1) == 1;
        set
        {
            if (value)
            {
                Interlocked.CompareExchange(ref _isDebug, 1, 0);
            }
            else
            {
                Interlocked.CompareExchange(ref _isDebug, 0, 1);
            }
        }
    }

    public static bool UCI_AnalyseMode
    {
        get => Interlocked.CompareExchange(ref _UCI_AnalyseMode, 1, 1) == 1;
        set
        {
            if (value)
            {
                Interlocked.CompareExchange(ref _UCI_AnalyseMode, 1, 0);
            }
            else
            {
                Interlocked.CompareExchange(ref _UCI_AnalyseMode, 0, 1);
            }
        }
    }

    public static int Hash
    {
        get => EngineSettings.TranspositionTableSize;
        set => EngineSettings.TranspositionTableSize = value;
    }
}

public sealed class GeneralSettings
{
    public bool EnableLogging { get; set; }

    public bool EnableTuning { get; set; }
}

public sealed class EngineSettings
{
    private int _maxDepth = 128;
    public int MaxDepth { get => _maxDepth; set => _maxDepth = Math.Clamp(value, 1, Constants.AbsoluteMaxDepth); }

    /// <summary>
    /// Depth for bench command
    /// </summary>
    public int BenchDepth { get; set; } = 10;

    private int _transpositionTableSize = 256;

    /// <summary>
    /// In MB, clamped to [<see cref="Constants.AbsoluteMinTTSize"/>, <see cref="Constants.AbsoluteMaxTTSize"/>]
    /// </summary>
    public int TranspositionTableSize
    {
        get => _transpositionTableSize;
        set => _transpositionTableSize =
            Math.Clamp(
                value,
                Constants.AbsoluteMinTTSize,
                Constants.AbsoluteMaxTTSize);
    }

    private int _threads = 1;
    public int Threads
    {
        get => _threads;
        set => _threads =
            Math.Clamp(
                value,
                1,
                Constants.MaxThreadCount);
    }

    public bool UseOnlineTablebaseInRootPositions { get; set; }

    /// <summary>
    /// Experimental, might misbehave due to tablebase API limits
    /// </summary>
    public bool UseOnlineTablebaseInSearch { get; set; }

    /// <summary>
    /// This can also de used to reduce online probing
    /// </summary>
    public int OnlineTablebaseMaxSupportedPieces { get; set; } = 7;

    public bool ShowWDL { get; set; }

    public bool IsPonder { get; set; }

    public bool IsChess960 { get; set; }

    /// <summary>
    /// Real NPS aren't calculated until the last search command.
    /// This option enables the report of an NPS estimation by the main thread
    /// </summary>
    public bool EstimateMultithreadedSearchNPS { get; set; }

    public double SPSA_OB_R_end { get; set; } = 0.02;

    /// <summary>
    /// Time overhead to take into account engine-gui communication process overhead
    /// </summary>
    private int _moveOverhead = 50;
    public int MoveOverhead
    {
        get => _moveOverhead;
        set => _moveOverhead =
            Math.Clamp(
                value,
                1,
                Constants.MaxMoveOverhead);
    }

    #region Time management

    /// <summary>
    /// Min milliseconds left after subtracting <see cref="MoveOverhead"/>
    /// from wtime/btime or movetime. This min value is used to avoid 0 or negative time left.
    /// Resulting milliseconds left are later used to calculate hard and soft time bounds
    /// </summary>
    [SPSA<int>(enabled: false)]
    public int MinSearchTime { get; set; } = 50;

    [SPSA<double>(0.25, 0.75, 0.025)]
    public double HardTimeBoundMultiplier { get; set; } = 0.51;

    [SPSA<double>(1.0, 2.0, 0.05)]
    public double MoveDivisor { get; set; } = 1.40;

    [SPSA<double>(enabled: false)]
    public double SoftTimeBoundMultiplier { get; set; } = 1;

    [SPSA<double>(0.5, 1, 0.025)]
    public double SoftTimeBaseIncrementMultiplier { get; set; } = 0.81;

    [SPSA<double>(1.5, 3.5, 0.1)]
    public double NodeTmBase { get; set; } = 2.74;

    [SPSA<double>(0.5, 2.5, 0.1)]
    public double NodeTmScale { get; set; } = 1.80;

    [SPSA<int>(enabled: false)]
    public int ScoreStability_MinDepth { get; set; } = 7;

    [SPSA<int>(enabled: false)]
    public int StopSearchOnMate_MaxSoftTimeBoundLimit { get; set; } = 10_000;

    [SPSA<int>(enabled: false)]
    public int StopSearchOnMate_MinDepth { get; set; } = 20;

    [SPSA<int>(enabled: false)]
    public int PonderHitMinTimeToContinueSearch { get; set; } = 100;

    [SPSA<int>(enabled: false)]
    public int PonderHitMinDepthToStopSearch { get; set; } = 15;

    #endregion

    #region Search

    [SPSA<int>(enabled: false)]
    public int LMR_MinDepth { get; set; } = 3;

    [SPSA<int>(enabled: false)]
    public int LMR_MinFullDepthSearchedMoves_PV { get; set; } = 5;

    [SPSA<int>(enabled: false)]
    public int LMR_MinFullDepthSearchedMoves_NonPV { get; set; } = 2;

    [SPSA<double>(0.1, 2, 0.2)]
    public double LMR_Base_Quiet { get; set; } = 0.20;

    [SPSA<double>(0.1, 2, 0.2)]
    public double LMR_Base_Noisy { get; set; } = 0.21;

    [SPSA<double>(2, 6, 0.2)]
    public double LMR_Divisor_Quiet { get; set; } = 4.31;

    [SPSA<double>(1, 5, 0.2)]
    public double LMR_Divisor_Noisy { get; set; } = 2.25;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Improving { get; set; } = 93;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Cutnode { get; set; } = 109;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_TTPV { get; set; } = 40;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_TTCapture { get; set; } = 202;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_PVNode { get; set; } = 45;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_InCheck { get; set; } = 104;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Quiet { get; set; } = 78;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Corrplexity { get; set; } = 197;

    [SPSA<int>(25, 300, 30)]
    public int LMR_Corrplexity_Delta { get; set; } = 124;

    [SPSA<int>(enabled: false)]
    public int History_MinDepth { get; set; } = 3;

    [SPSA<int>(enabled: false)]
    public int History_MinVisitedMoves { get; set; } = 2;

    /// <summary>
    /// Tuned from ~<see cref="History_MaxMoveValue"/> / 2
    /// </summary>
    [SPSA<int>(1, 8192, 512)]
    public int LMR_History_Divisor_Quiet { get; set; } = 3540;

    /// <summary>
    /// Tuned from ~<see cref="History_MaxMoveValue"/> / 2 * (3 / 4)
    /// </summary>
    [SPSA<int>(4_096, 12_288, 512)]
    public int LMR_History_Divisor_Noisy { get; set; } = 7706;

    [SPSA<int>(20, 100, 8)]
    public int LMR_DeeperBase { get; set; } = 68;

    [SPSA<int>(enabled: false)]
    public int LMR_DeeperDepthMultiplier { get; set; } = 2;

    [SPSA<int>(enabled: false)]
    public int NMP_MinDepth { get; set; } = 3;

    [SPSA<int>(enabled: false)]
    public int NMP_Margin { get; set; } = +30;

    [SPSA<int>(enabled: false)]
    public int NMP_BaseDepthReduction { get; set; } = 2;

#pragma warning disable CA1805 // Do not initialize unnecessarily
    [SPSA<int>(enabled: false)]
    public int NMP_DepthIncrement { get; set; } = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily

    [SPSA<int>(enabled: false)]
    public int NMP_DepthDivisor { get; set; } = 3;

    [SPSA<int>(50, 350, 15)]
    public int NMP_StaticEvalBetaDivisor { get; set; } = 82;

    [SPSA<int>(enabled: false)]
    public int NMP_StaticEvalBetaMaxReduction { get; set; } = 3;

    [SPSA<int>(enabled: false)]
    public int AspirationWindow_Base { get; set; } = 9;

    [SPSA<double>(1, 3, 0.1)]
    public double AspirationWindow_Multiplier { get; set; } = 1.29;

    //[SPSA<int>(5, 30, 1)]
    //public int AspirationWindow_Delta { get; set; } = 13;

    [SPSA<int>(enabled: false)]
    public int AspirationWindow_MinDepth { get; set; } = 8;

    [SPSA<int>(10, 150, 10)]
    public int ImprovingRate { get; set; } = 53;

    [SPSA<int>(enabled: false)]
    public int RFP_MaxDepth { get; set; } = 9;

    [SPSA<int>(50, 150, 10)]
    public int RFP_Improving_Margin { get; set; } = 45;

    [SPSA<int>(50, 150, 10)]
    public int RFP_NotImproving_Margin { get; set; } = 85;

    [SPSA<int>(0, 10, 1)]
    public int RFP_Quadratic { get; set; } = 6;

    /// <summary>
    /// Should be tuned only if improvingRate is ever used for something else
    /// </summary>
    [SPSA<double>(enabled: false)]
    public double RFP_ImprovingFactor { get; set; } = 0.75;

    //[SPSA<int>(1, 300, 15)]
    //public int RFP_DepthScalingFactor { get; set; } = 55;

    [SPSA<int>(enabled: false)]
    public int Razoring_MaxDepth { get; set; } = 2;

    [SPSA<int>(1, 300, 15)]
    public int Razoring_Depth1Bonus { get; set; } = 138;

    [SPSA<int>(1, 300, 15)]
    public int Razoring_NotDepth1Bonus { get; set; } = 205;

    [SPSA<int>(enabled: false)]
    public int IIR_MinDepth { get; set; } = 5;

    [SPSA<int>(enabled: false)]
    public int LMP_BaseMovesToTry { get; set; } = 1;

    [SPSA<int>(enabled: false)]
    public int LMP_MovesDepthMultiplier { get; set; } = 3;

    [SPSA<int>(enabled: false)]
    public int History_MaxMoveValue { get; set; } = 8_192;

    [SPSA<int>(512, 4096, 250)]
    public int History_Bonus_MaxIncrement { get; set; } = 2440;

    [SPSA<int>(1, 500, 35)]
    public int History_Bonus_Constant { get; set; } = 243;

    [SPSA<int>(1, 500, 35)]
    public int History_Bonus_Linear { get; set; } = 178;

    [SPSA<int>(1, 10, 1)]
    public int History_Bonus_Quadratic { get; set; } = 3;

    [SPSA<int>(512, 4096, 250)]
    public int History_Malus_MaxDecrement { get; set; } = 1473;

    [SPSA<int>(1, 500, 35)]
    public int History_Malus_Constant { get; set; } = 220;

    [SPSA<int>(1, 500, 35)]
    public int History_Malus_Linear { get; set; } = 253;

    [SPSA<int>(1, 10, 1)]
    public int History_Malus_Quadratic { get; set; } = 7;

    [SPSA<int>(enabled: false)]
    public int CounterMoves_MinDepth { get; set; } = 3;

    [SPSA<int>(0, 200, 10)]
    public int History_BestScoreBetaMargin { get; set; } = 125;

    [SPSA<int>(0, 40, 4)]
    public int History_EvalDiff_ImprovementMargin { get; set; } = 20;

    [SPSA<int>(0, 10, 1)]
    public int History_EvalDiff_ImprovementCoefficient { get; set; } = 4;

    [SPSA<int>(enabled: false)]
    public int History_EvalDiff_MaxBonus { get; set; } = 100;

    [SPSA<int>(enabled: false)]
    public int SEE_BadCaptureReduction { get; set; } = 2;

    [SPSA<int>(enabled: false)]
    public int FP_MaxDepth { get; set; } = 7;

    [SPSA<int>(1, 200, 10)]
    public int FP_DepthScalingFactor { get; set; } = 76;

    [SPSA<int>(0, 500, 25)]
    public int FP_Margin { get; set; } = 115;

    [SPSA<int>(enabled: false)]
    public int FP_HistoryDivisor { get; set; } = 32;

    [SPSA<int>(enabled: false)]
    public int HistoryPruning_MaxDepth { get; set; } = 5;

    [SPSA<int>(-8192, 0, 512)]
    public int HistoryPruning_Margin { get; set; } = -1506;

    [SPSA<int>(enabled: false)]
    public int TTHit_NoCutoffExtension_MaxDepth { get; set; } = 6;

    [SPSA<int>(enabled: false)]
    public int TTReplacement_DepthOffset { get; set; } = 4;

    [SPSA<int>(enabled: false)]
    public int TTReplacement_TTPVDepthOffset { get; set; } = 2;

    [SPSA<int>(-100, -10, 10)]
    public int PVS_SEE_Threshold_Quiet { get; set; } = -46;

    [SPSA<int>(-150, -50, 10)]
    public int PVS_SEE_Threshold_Noisy { get; set; } = -111;

    /// <summary>
    /// Initial value same as <see cref="History_MaxMoveValue"/>
    /// </summary>
    [SPSA<int>(enabled: false)]
    public int CorrHistory_MaxValue { get; set; } = 8_192;

    /// <summary>
    /// Initial value same as <see cref="History_MaxMoveRawBonus"/>
    /// </summary>
    [SPSA<int>(enabled: false)]
    public int CorrHistory_MaxRawBonus { get; set; } = 1_896;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.CorrHistScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 200, 15)]
    public int CorrHistoryWeight_Pawn { get; set; } = 87;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.CorrHistScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 200, 15)]
    public int CorrHistoryWeight_NonPawnSTM { get; set; } = 74;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.CorrHistScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 200, 15)]
    public int CorrHistoryWeight_NonPawnNoSTM { get; set; } = 115;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.CorrHistScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 200, 15)]
    public int CorrHistoryWeight_Minor { get; set; } = 176;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.CorrHistScaleFactor"/>
    /// </summary>
    [SPSA<int>(50, 250, 15)]
    public int CorrHistoryWeight_Major { get; set; } = 179;

    [SPSA<int>(enabled: false)]
    public int TT_50MR_Start { get; set; } = 20;

    [SPSA<int>(enabled: false)]
    public int TT_50MR_Step { get; set; } = 10;

    [SPSA<int>(enabled: false)]
    public int SE_MinDepth { get; set; } = 7;

    [SPSA<int>(enabled: false)]
    public int SE_TTDepthOffset { get; set; } = 3;

    [SPSA<int>(enabled: false)]
    public int SE_DepthMultiplier { get; set; } = 1;

    [SPSA<int>(0, 50, 5)]
    public int SE_DoubleExtensions_Margin { get; set; } = 1;

    [SPSA<int>(enabled: false)]
    public int SE_DoubleExtensions_Max { get; set; } = 6;

    [SPSA<int>(enabled: false)]
    public int SE_LowDepthExtension { get; set; } = 9;

    [SPSA<int>(0, 20, 2)]
    public int SE_NoPV { get; set; } = 10;

    #endregion
}

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default, WriteIndented = true)] // https://github.com/dotnet/runtime/issues/78602#issuecomment-1322004254
[JsonSerializable(typeof(EngineSettings))]
[JsonSerializable(typeof(SPSAAttribute<int>))]
[JsonSerializable(typeof(SPSAAttribute<double>))]
[JsonSerializable(typeof(WeatherFactoryOutput<int>))]
[JsonSerializable(typeof(WeatherFactoryOutput<double>))]
internal sealed partial class EngineSettingsJsonSerializerContext : JsonSerializerContext;
