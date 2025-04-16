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

    #region Time management

    /// <summary>
    /// Time overhead to take into account engine-gui communication process overhead
    /// </summary>
    public int EngineGuiCommunicationTimeOverhead { get; set; } = 50;

    /// <summary>
    /// Min milliseconds left after substracting <see cref="EngineGuiCommunicationTimeOverhead"/>
    /// from wtime/btime or movetime. This min value is used to avoid 0 or negative time left.
    /// Resulting milliseconds left are later used to calculate hard and soft time bounds
    /// </summary>
    public int MinSearchTime { get; set; } = 50;

    public double HardTimeBoundMultiplier { get; set; } = 0.52;

    public double SoftTimeBoundMultiplier { get; set; } = 1;

    public double SoftTimeBaseIncrementMultiplier { get; set; } = 0.8;

    [SPSA<double>(1, 3, 0.1)]
    public double NodeTmBase { get; set; } = 2.56;

    [SPSA<double>(0.5, 2.5, 0.1)]
    public double NodeTmScale { get; set; } = 1.66;

    //[SPSA<int>(1, 15, 1)]
    public int ScoreStabiity_MinDepth { get; set; } = 7;

    public int SoftTimeBoundLimitOnMate { get; set; } = 1_000;

    public int PonderHitMinTimeToContinueSearch { get; set; } = 100;

    public int PonderHitMinDepthToStopSearch { get; set; } = 15;

    #endregion

    #region Search

    //[SPSA<int>(3, 10, 0.5)]
    public int LMR_MinDepth { get; set; } = 3;

    //[SPSA<int>(1, 10, 0.5)]
    public int LMR_MinFullDepthSearchedMoves_PV { get; set; } = 5;

    //[SPSA<int>(1, 10, 0.5)]
    public int LMR_MinFullDepthSearchedMoves_NonPV { get; set; } = 2;

    [SPSA<double>(0.1, 2, 0.2)]
    public double LMR_Base_Quiet { get; set; } = 0.85;

    [SPSA<double>(0.1, 2, 0.2)]
    public double LMR_Base_Noisy { get; set; } = 0.52;

    [SPSA<double>(1, 5, 0.2)]
    public double LMR_Divisor_Quiet { get; set; } = 2.70;

    [SPSA<double>(1, 5, 0.2)]
    public double LMR_Divisor_Noisy { get; set; } = 2.67;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Improving { get; set; } = 75;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Cutnode { get; set; } = 141;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_TTPV { get; set; } = 82;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_TTCapture { get; set; } = 100;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_PVNode { get; set; } = 60;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_InCheck { get; set; } = 79;

    /// <summary>
    /// Needs to be re-scaled dividing by <see cref="EvaluationConstants.LMRScaleFactor"/>
    /// </summary>
    [SPSA<int>(25, 300, 30)]
    public int LMR_Quiet { get; set; } = 100;

    /// <summary>
    /// Tuned from ~<see cref="History_MaxMoveValue"/> / 2
    /// </summary>
    [SPSA<int>(1, 8192, 512)]
    public int LMR_History_Divisor_Quiet { get; set; } = 3107;

    /// <summary>
    /// Tuned from ~<see cref="History_MaxMoveValue"/> / 2 * (3 / 4)
    /// </summary>
    [SPSA<int>(1, 8192, 512)]
    public int LMR_History_Divisor_Noisy { get; set; } = 3451;

    [SPSA<int>(20, 100, 8)]
    public int LMR_DeeperBase { get; set; } = 38;

    //[SPSA<int>(1, 10, 1)]
    public int LMR_DeeperDepthMultiplier { get; set; } = 2;

    //[SPSA<int>(1, 10, 0.5)]
    public int NMP_MinDepth { get; set; } = 3;

    //[SPSA<int>(1, 5, 0.5)]
    public int NMP_BaseDepthReduction { get; set; } = 2;

#pragma warning disable CA1805 // Do not initialize unnecessarily
    //[SPSA<int>(0, 10, 0.5)]
    public int NMP_DepthIncrement { get; set; } = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily

    //[SPSA<int>(1, 10, 0.5)]
    public int NMP_DepthDivisor { get; set; } = 3;

    [SPSA<int>(50, 350, 15)]
    public int NMP_StaticEvalBetaDivisor { get; set; } = 113;

    //[SPSA<int>(1, 10, 0.5)]
    public int NMP_StaticEvalBetaMaxReduction { get; set; } = 3;

    [SPSA<int>(5, 30, 1)]
    public int AspirationWindow_Base { get; set; } = 10;

    //[SPSA<int>(5, 30, 1)]
    //public int AspirationWindow_Delta { get; set; } = 13;

    //[SPSA<int>(1, 20, 1)]
    public int AspirationWindow_MinDepth { get; set; } = 8;

    //[SPSA<int>(1, 10, 0.5)]
    public int RFP_MaxDepth { get; set; } = 7;

    //[SPSA<int>(1, 300, 15)]
    //public int RFP_DepthScalingFactor { get; set; } = 55;

    //[SPSA<int>(1, 10, 0.5)]
    public int Razoring_MaxDepth { get; set; } = 2;

    [SPSA<int>(1, 300, 15)]
    public int Razoring_Depth1Bonus { get; set; } = 104;

    [SPSA<int>(1, 300, 15)]
    public int Razoring_NotDepth1Bonus { get; set; } = 190;

    //[SPSA<int>(1, 10, 0.5)]
    public int IIR_MinDepth { get; set; } = 4;

    //[SPSA<int>(0, 10, 0.5)]
    public int LMP_BaseMovesToTry { get; set; } = 1;

    //[SPSA<int>(0, 10, 0.5)]
    public int LMP_MovesDepthMultiplier { get; set; } = 3;

    public int History_MaxMoveValue { get; set; } = 8_192;

    /// <summary>
    /// 1896: constant from depth 12
    /// </summary>
    public int History_MaxMoveRawBonus { get; set; } = 1_896;

    public int CounterMoves_MinDepth { get; set; } = 3;

    [SPSA<int>(0, 200, 10)]
    public int History_BestScoreBetaMargin { get; set; } = 86;

    //[SPSA<int>(0, 6, 0.5)]
    public int SEE_BadCaptureReduction { get; set; } = 2;

    //[SPSA<int>(1, 10, 0.5)]
    public int FP_MaxDepth { get; set; } = 7;

    [SPSA<int>(1, 200, 10)]
    public int FP_DepthScalingFactor { get; set; } = 105;

    [SPSA<int>(0, 500, 25)]
    public int FP_Margin { get; set; } = 108;

    //[SPSA<int>(0, 10, 0.5)]
    public int HistoryPrunning_MaxDepth { get; set; } = 5;

    [SPSA<int>(-8192, 0, 512)]
    public int HistoryPrunning_Margin { get; set; } = -643;

    //[SPSA<int>(0, 10, 0.5)]
    public int TTHit_NoCutoffExtension_MaxDepth { get; set; } = 6;

    //[SPSA<int>(0, 6, 0.5)]
#pragma warning disable CA1805 // Do not initialize unnecessarily
    public int TTReplacement_DepthOffset { get; set; } = 0;
#pragma warning restore CA1805 // Do not initialize unnecessarily

    //[SPSA<int>(0, 10, 0.5)]
    public int TTReplacement_TTPVDepthOffset { get; set; } = 2;

    [SPSA<int>(-100, -10, 10)]
    public int PVS_SEE_Threshold_Quiet { get; set; } = -42;

    [SPSA<int>(-150, -50, 10)]
    public int PVS_SEE_Threshold_Noisy { get; set; } = -117;

    /// <summary>
    /// Initial value same as <see cref="History_MaxMoveValue"/>
    /// </summary>
    public int CorrHistory_MaxValue { get; set; } = 8_192;

    /// <summary>
    /// Initial value same as <see cref="History_MaxMoveRawBonus"/>
    /// </summary>
    public int CorrHistory_MaxRawBonus { get; set; } = 1_896;

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
