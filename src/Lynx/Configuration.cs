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

    public double SPSA_OB_R_end { get; set; } = 0.02;

    #region Time management

    public double HardTimeBoundMultiplier { get; set; } = 0.52;

    public double SoftTimeBoundMultiplier { get; set; } = 1;

    public double SoftTimeBaseIncrementMultiplier { get; set; } = 0.8;

    [SPSA<double>(1, 3, 0.1)]
    public double NodeTmBase { get; set; } = 2.4;

    [SPSA<double>(0.5, 2.5, 0.1)]
    public double NodeTmScale { get; set; } = 1.65;

    //[SPSA<int>(1, 15, 1)]
    public int ScoreStabiity_MinDepth { get; set; } = 7;

    #endregion

    #region Search

    //[SPSA<int>(3, 10, 0.5)]
    public int LMR_MinDepth { get; set; } = 3;

    //[SPSA<int>(1, 10, 0.5)]
    public int LMR_MinFullDepthSearchedMoves_PV { get; set; } = 5;

    //[SPSA<int>(1, 10, 0.5)]
    public int LMR_MinFullDepthSearchedMoves_NonPV { get; set; } = 2;

    /// <summary>
    /// Value originally from Stormphrax, who apparently took it from Viridithas
    /// </summary>
    [SPSA<double>(0.1, 2, 0.1)]
    public double LMR_Base { get; set; } = 0.60;

    /// <summary>
    /// Value originally from Akimbo
    /// </summary>
    [SPSA<double>(1, 5, 0.1)]
    public double LMR_Divisor { get; set; } = 3.21;

    //[SPSA<int>(1, 10, 0.5)]
    public int NMP_MinDepth { get; set; } = 3;

    //[SPSA<int>(1, 5, 0.5)]
    public int NMP_BaseDepthReduction { get; set; } = 2;

    //[SPSA<int>(0, 10, 0.5)]
    public int NMP_DepthIncrement { get; set; } = 0;

    //[SPSA<int>(1, 10, 0.5)]
    public int NMP_DepthDivisor { get; set; } = 3;

    [SPSA<int>(50, 350, 15)]
    public int NMP_StaticEvalBetaDivisor { get; set; } = 111;

    //[SPSA<int>(1, 10, 0.5)]
    public int NMP_StaticEvalBetaMaxReduction { get; set; } = 3;

    [SPSA<int>(5, 30, 1)]
    public int AspirationWindow_Base { get; set; } = 11;

    //[SPSA<int>(5, 30, 1)]
    //public int AspirationWindow_Delta { get; set; } = 13;

    //[SPSA<int>(1, 20, 1)]
    public int AspirationWindow_MinDepth { get; set; } = 8;

    //[SPSA<int>(1, 10, 0.5)]
    public int RFP_MaxDepth { get; set; } = 7;

    [SPSA<int>(1, 300, 15)]
    public int RFP_DepthScalingFactor { get; set; } = 55;

    //[SPSA<int>(1, 10, 0.5)]
    public int Razoring_MaxDepth { get; set; } = 2;

    [SPSA<int>(1, 300, 15)]
    public int Razoring_Depth1Bonus { get; set; } = 87;

    [SPSA<int>(1, 300, 15)]
    public int Razoring_NotDepth1Bonus { get; set; } = 220;

    //[SPSA<int>(1, 10, 0.5)]
    public int IIR_MinDepth { get; set; } = 4;

    //[SPSA<int>(1, 10, 0.5)]
    public int LMP_MaxDepth { get; set; } = 8;

    //[SPSA<int>(0, 10, 0.5)]
    public int LMP_BaseMovesToTry { get; set; } = 1;

    //[SPSA<int>(0, 10, 0.5)]
    public int LMP_MovesDepthMultiplier { get; set; } = 3;

    public int History_MaxMoveValue { get; set; } = 8_192;

    /// <summary>
    /// 1896: constant from depth 12
    /// </summary>
    public int History_MaxMoveRawBonus { get; set; } = 1_896;

    //[SPSA<int>(0, 200, 10)]
    public int History_BestScoreBetaMargin { get; set; } = 60;

    //[SPSA<int>(0, 6, 0.5)]
    public int SEE_BadCaptureReduction { get; set; } = 2;

    //[SPSA<int>(1, 10, 0.5)]
    public int FP_MaxDepth { get; set; } = 7;

    [SPSA<int>(1, 200, 10)]
    public int FP_DepthScalingFactor { get; set; } = 87;

    [SPSA<int>(0, 500, 25)]
    public int FP_Margin { get; set; } = 167;

    //[SPSA<int>(0, 10, 0.5)]
    public int HistoryPrunning_MaxDepth { get; set; } = 5;

    [SPSA<int>(-8192, 0, 512)]
    public int HistoryPrunning_Margin { get; set; } = -1345;

    //[SPSA<int>(0, 10, 0.5)]
    public int TTHit_NoCutoffExtension_MaxDepth { get; set; } = 6;

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
