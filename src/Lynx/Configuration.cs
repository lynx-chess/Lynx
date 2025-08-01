﻿using System.Text.Json.Serialization;

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
    /// Min milliseconds left after substracting <see cref="MoveOverhead"/>
    /// from wtime/btime or movetime. This min value is used to avoid 0 or negative time left.
    /// Resulting milliseconds left are later used to calculate hard and soft time bounds
    /// </summary>
    [SPSA<int>(enabled: false)]
    public int MinSearchTime { get; set; } = 50;

    [SPSA<double>(enabled: false)]
    public double HardTimeBoundMultiplier { get; set; } = 0.52;

    [SPSA<double>(enabled: false)]
    public double SoftTimeBoundMultiplier { get; set; } = 1;

    [SPSA<double>(enabled: false)]
    public double SoftTimeBaseIncrementMultiplier { get; set; } = 0.8;

    [SPSA<double>(1, 3, 0.1)]
    public double NodeTmBase { get; set; } = 2.56;

    [SPSA<double>(0.5, 2.5, 0.1)]
    public double NodeTmScale { get; set; } = 1.66;

    [SPSA<int>(enabled: false)]
    public int ScoreStabiity_MinDepth { get; set; } = 7;

    [SPSA<int>(enabled: false)]
    public int SoftTimeBoundLimitOnMate { get; set; } = 1_000;

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
    public double LMR_Base_Quiet { get; set; } = 0.38;

    [SPSA<double>(0.1, 2, 0.2)]
    public double LMR_Base_Noisy { get; set; } = 0.23;

    [SPSA<double>(1, 5, 0.2)]
    public double LMR_Divisor_Quiet { get; set; } = 4.02;

    [SPSA<double>(1, 5, 0.2)]
    public double LMR_Divisor_Noisy { get; set; } = 2.37;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Improving { get; set; } = 64;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Cutnode { get; set; } = 78;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_TTPV { get; set; } = 64;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_TTCapture { get; set; } = 114;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_PVNode { get; set; } = 57;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_InCheck { get; set; } = 96;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Quiet { get; set; } = 84;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Corrplexity { get; set; } = 125;

    [SPSA<int>(25, 300, 30)]
    public int LMR_Corrplexity_Delta { get; set; } = 90;

    [SPSA<int>(enabled: false)]
    public int History_MinDepth { get; set; } = 3;

    [SPSA<int>(enabled: false)]
    public int History_MinVisitedMoves { get; set; } = 2;

    /// <summary>
    /// Tuned from ~<see cref="History_MaxMoveValue"/> / 2
    /// </summary>
    [SPSA<int>(1, 8192, 512)]
    public int LMR_History_Divisor_Quiet { get; set; } = 2950;

    /// <summary>
    /// Tuned from ~<see cref="History_MaxMoveValue"/> / 2 * (3 / 4)
    /// </summary>
    [SPSA<int>(1, 8192, 512)]
    public int LMR_History_Divisor_Noisy { get; set; } = 7211;

    [SPSA<int>(20, 100, 8)]
    public int LMR_DeeperBase { get; set; } = 74;

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
    public int NMP_StaticEvalBetaDivisor { get; set; } = 88;

    [SPSA<int>(enabled: false)]
    public int NMP_StaticEvalBetaMaxReduction { get; set; } = 3;

    [SPSA<int>(enabled: false)]
    public int AspirationWindow_Base { get; set; } = 9;

    [SPSA<double>(1, 3, 0.1)]
    public double AspirationWindow_Multiplier { get; set; } = 1.16;

    //[SPSA<int>(5, 30, 1)]
    //public int AspirationWindow_Delta { get; set; } = 13;

    [SPSA<int>(enabled: false)]
    public int AspirationWindow_MinDepth { get; set; } = 8;

    [SPSA<int>(10, 150, 10)]
    public int ImprovingRate { get; set; } = 59;

    [SPSA<int>(enabled: false)]
    public int RFP_MaxDepth { get; set; } = 9;

    [SPSA<int>(50, 150, 10)]
    public int RFP_Improving_Margin { get; set; } = 83;

    [SPSA<int>(50, 150, 10)]
    public int RFP_NotImproving_Margin { get; set; } = 112;

    [SPSA<double>(enabled: false)]
    public double RFP_ImprovingFactor { get; set; } = 0.75;

    //[SPSA<int>(1, 300, 15)]
    //public int RFP_DepthScalingFactor { get; set; } = 55;

    [SPSA<int>(enabled: false)]
    public int Razoring_MaxDepth { get; set; } = 2;

    [SPSA<int>(1, 300, 15)]
    public int Razoring_Depth1Bonus { get; set; } = 111;

    [SPSA<int>(1, 300, 15)]
    public int Razoring_NotDepth1Bonus { get; set; } = 200;

    [SPSA<int>(enabled: false)]
    public int IIR_MinDepth { get; set; } = 4;

    [SPSA<int>(enabled: false)]
    public int LMP_BaseMovesToTry { get; set; } = 1;

    [SPSA<int>(enabled: false)]
    public int LMP_MovesDepthMultiplier { get; set; } = 3;

    [SPSA<int>(enabled: false)]
    public int History_MaxMoveValue { get; set; } = 8_192;

    [SPSA<int>(512, 4096, 250)]
    public int History_Bonus_MaxIncrement { get; set; } = 2106;

    [SPSA<int>(1, 500, 35)]
    public int History_Bonus_Constant { get; set; } = 147;

    [SPSA<int>(1, 500, 35)]
    public int History_Bonus_Linear { get; set; } = 185;

    [SPSA<int>(1, 10, 1)]
    public int History_Bonus_Quadratic { get; set; } = 4;

    [SPSA<int>(512, 4096, 250)]
    public int History_Malus_MaxDecrement { get; set; } = 1744;

    [SPSA<int>(1, 500, 35)]
    public int History_Malus_Constant { get; set; } = 186;

    [SPSA<int>(1, 500, 35)]
    public int History_Malus_Linear { get; set; } = 218;

    [SPSA<int>(1, 10, 1)]
    public int History_Malus_Quadratic { get; set; } = 6;

    [SPSA<int>(enabled: false)]
    public int CounterMoves_MinDepth { get; set; } = 3;

    [SPSA<int>(0, 200, 10)]
    public int History_BestScoreBetaMargin { get; set; } = 136;

    [SPSA<int>(enabled: false)]
    public int SEE_BadCaptureReduction { get; set; } = 2;

    [SPSA<int>(enabled: false)]
    public int FP_MaxDepth { get; set; } = 7;

    [SPSA<int>(1, 200, 10)]
    public int FP_DepthScalingFactor { get; set; } = 81;

    [SPSA<int>(0, 500, 25)]
    public int FP_Margin { get; set; } = 84;

    [SPSA<int>(enabled: false)]
    public int HistoryPrunning_MaxDepth { get; set; } = 5;

    [SPSA<int>(-8192, 0, 512)]
    public int HistoryPrunning_Margin { get; set; } = -3;

    [SPSA<int>(enabled: false)]
    public int TTHit_NoCutoffExtension_MaxDepth { get; set; } = 6;

#pragma warning disable CA1805 // Do not initialize unnecessarily
    [SPSA<int>(enabled: false)]
    public int TTReplacement_DepthOffset { get; set; } = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily

    [SPSA<int>(enabled: false)]
    public int TTReplacement_TTPVDepthOffset { get; set; } = 2;

    [SPSA<int>(-100, -10, 10)]
    public int PVS_SEE_Threshold_Quiet { get; set; } = -36;

    [SPSA<int>(-150, -50, 10)]
    public int PVS_SEE_Threshold_Noisy { get; set; } = -102;

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
    public int CorrHistoryWeight_Pawn { get; set; } = 107;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.CorrHistScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 200, 15)]
    public int CorrHistoryWeight_NonPawnSTM { get; set; } = 49;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.CorrHistScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 200, 15)]
    public int CorrHistoryWeight_NonPawnNoSTM { get; set; } = 117;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.CorrHistScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 200, 15)]
    public int CorrHistoryWeight_Minor { get; set; } = 149;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.CorrHistScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 200, 15)]
    public int CorrHistoryWeight_Major { get; set; } = 150;

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
    public int SE_DoubleExtensions_Margin { get; set; } = 4;

    [SPSA<int>(enabled: false)]
    public int SE_DoubleExtensions_Max { get; set; } = 6;

    [SPSA<int>(enabled: false)]
    public int SE_LowDepthExtension { get; set; } = 9;

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
