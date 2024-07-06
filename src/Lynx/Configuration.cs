using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Lynx;

public static class Configuration
{
    public static EngineSettings EngineSettings { get; set; } = new EngineSettings();
    public static GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();

    private static int _isDebug = 0;
#pragma warning disable IDE1006 // Naming Styles
    private static int _UCI_AnalyseMode = 0;
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
    public bool EnableLogging { get; set; } = false;

    public bool EnableTuning { get; set; } = false;
}

public sealed class EngineSettings
{
    private int _maxDepth = 128;
    public int MaxDepth { get => _maxDepth; set => _maxDepth = Math.Clamp(value, 1, Constants.AbsoluteMaxDepth); }

    /// <summary>
    /// Depth for bench command
    /// </summary>
    public int BenchDepth { get; set; } = 10;

    /// <summary>
    /// MB
    /// </summary>
    public int TranspositionTableSize { get; set; } = 256;

    public bool UseOnlineTablebaseInRootPositions { get; set; } = false;

    /// <summary>
    /// Experimental, might misbehave due to tablebase API limits
    /// </summary>
    public bool UseOnlineTablebaseInSearch { get; set; } = false;

    /// <summary>
    /// This can also de used to reduce online probing
    /// </summary>
    public int OnlineTablebaseMaxSupportedPieces { get; set; } = 7;

    public bool ShowWDL { get; set; } = false;

    public bool IsPonder { get; set; } = false;

    public double SPSA_OB_R_end { get; set; } = 0.02;

    #region Time management

    public double HardTimeBoundMultiplier { get; set; } = 0.52;

    public double SoftTimeBoundMultiplier { get; set; } = 1;

    public int DefaultMovesToGo { get; set; } = 45;

    public double SoftTimeBaseIncrementMultiplier { get; set; } = 0.8;

    #endregion

    [SPSAAttribute<int>(3, 10, 0.5)]
    public int LMR_MinDepth { get; set; } = 3;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int LMR_MinFullDepthSearchedMoves { get; set; } = 4;

    /// <summary>
    /// Value originally from Stormphrax, who apparently took it from Viridithas
    /// </summary>
    [SPSAAttribute<double>(0.1, 2, 0.10)]
    public double LMR_Base { get; set; } = 0.91;

    /// <summary>
    /// Value originally from Akimbo
    /// </summary>
    [SPSAAttribute<double>(1, 5, 0.1)]
    public double LMR_Divisor { get; set; } = 3.42;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int NMP_MinDepth { get; set; } = 2;

    [SPSAAttribute<int>(1, 5, 0.5)]
    public int NMP_BaseDepthReduction { get; set; } = 2;

    [SPSAAttribute<int>(0, 10, 0.5)]
    public int NMP_DepthIncrement { get; set; } = 1;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int NMP_DepthDivisor { get; set; } = 4;

    [SPSAAttribute<int>(1, 100, 5)]
    public int AspirationWindow_Delta { get; set; } = 13;

    [SPSAAttribute<int>(1, 20, 1)]
    public int AspirationWindow_MinDepth { get; set; } = 8;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int RFP_MaxDepth { get; set; } = 6;

    [SPSAAttribute<int>(1, 300, 15)]
    public int RFP_DepthScalingFactor { get; set; } = 82;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int Razoring_MaxDepth { get; set; } = 1;

    [SPSAAttribute<int>(1, 300, 15)]
    public int Razoring_Depth1Bonus { get; set; } = 129;

    [SPSAAttribute<int>(1, 300, 15)]
    public int Razoring_NotDepth1Bonus { get; set; } = 178;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int IIR_MinDepth { get; set; } = 3;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int LMP_MaxDepth { get; set; } = 7;

    [SPSAAttribute<int>(0, 10, 0.5)]
    public int LMP_BaseMovesToTry { get; set; } = 0;

    [SPSAAttribute<int>(0, 10, 0.5)]
    public int LMP_MovesDepthMultiplier { get; set; } = 4;

    public int History_MaxMoveValue { get; set; } = 8_192;

    /// <summary>
    /// 1896: constant from depth 12
    /// </summary>
    public int History_MaxMoveRawBonus { get; set; } = 1_896;

    [SPSAAttribute<int>(0, 6, 0.5)]
    public int SEE_BadCaptureReduction { get; set; } = 2;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int FP_MaxDepth { get; set; } = 5;

    [SPSAAttribute<int>(1, 200, 10)]
    public int FP_DepthScalingFactor { get; set; } = 78;

    [SPSAAttribute<int>(0, 500, 25)]
    public int FP_Margin { get; set; } = 129;

    #region Evaluation

    public static readonly TaperedEvaluationTerm IsolatedPawnPenalty = Utils.Pack(-19, -14);

    public static readonly TaperedEvaluationTerm OpenFileRookBonus = Utils.Pack(45, 6);

    public static readonly TaperedEvaluationTerm SemiOpenFileRookBonus = Utils.Pack(15, 8);

    public static readonly TaperedEvaluationTerm QueenMobilityBonus = Utils.Pack(3, 8);

    public static readonly TaperedEvaluationTerm SemiOpenFileKingPenalty = Utils.Pack(-30, 18);

    public static readonly TaperedEvaluationTerm OpenFileKingPenalty = Utils.Pack(-96, 15);

    public static readonly TaperedEvaluationTerm KingShieldBonus = Utils.Pack(23, -11);

    public static readonly TaperedEvaluationTerm BishopPairBonus = Utils.Pack(30, 81);

    public static readonly TaperedEvaluationTerm PieceProtectedByPawnBonus = Utils.Pack(7, 11);

    public static readonly TaperedEvaluationTerm PieceAttackedByPawnPenalty = Utils.Pack(-45, -18);

    public static readonly TaperedEvaluationTermByRank PassedPawnBonus =
    [
        Utils.Pack(0, 0),
        Utils.Pack(2, 11),
        Utils.Pack(-11, 20),
        Utils.Pack(-12, 47),
        Utils.Pack(18, 81),
        Utils.Pack(63, 161),
        Utils.Pack(102, 225),
        Utils.Pack(0, 0)
    ];

    public static readonly TaperedEvaluationTermByCount27 VirtualKingMobilityBonus =
    [
        Utils.Pack(0, 0),
        Utils.Pack(0, 0),
        Utils.Pack(0, 0),
        Utils.Pack(37, -5),
        Utils.Pack(51, -8),
        Utils.Pack(24, 24),
        Utils.Pack(22, 13),
        Utils.Pack(19, 3),
        Utils.Pack(15, 6),
        Utils.Pack(11, 5),
        Utils.Pack(9, 9),
        Utils.Pack(2, 13),
        Utils.Pack(0, 9),
        Utils.Pack(-5, 12),
        Utils.Pack(-15, 15),
        Utils.Pack(-26, 18),
        Utils.Pack(-35, 15),
        Utils.Pack(-46, 12),
        Utils.Pack(-52, 10),
        Utils.Pack(-60, 3),
        Utils.Pack(-51, -5),
        Utils.Pack(-46, -13),
        Utils.Pack(-44, -24),
        Utils.Pack(-39, -34),
        Utils.Pack(-45, -44),
        Utils.Pack(-22, -65),
        Utils.Pack(-62, -72),
        Utils.Pack(-36, -90)
    ];

    public static readonly TaperedEvaluationTermByCount8 KnightMobilityBonus =
    [
        Utils.Pack(0, 0),
        Utils.Pack(25, -4),
        Utils.Pack(34, 5),
        Utils.Pack(40, 5),
        Utils.Pack(44, 11),
        Utils.Pack(42, 20),
        Utils.Pack(41, 22),
        Utils.Pack(43, 24),
        Utils.Pack(55, 17)
    ];

    public static readonly TaperedEvaluationTermByCount14 BishopMobilityBonus =
    [
        Utils.Pack(-198, -154),
        Utils.Pack(0, 0),
        Utils.Pack(12, 2),
        Utils.Pack(21, 41),
        Utils.Pack(36, 57),
        Utils.Pack(42, 72),
        Utils.Pack(58, 92),
        Utils.Pack(67, 102),
        Utils.Pack(76, 114),
        Utils.Pack(76, 121),
        Utils.Pack(82, 126),
        Utils.Pack(85, 125),
        Utils.Pack(87, 126),
        Utils.Pack(119, 118),
        Utils.Pack(0, 0)
    ];

    public static readonly TaperedEvaluationTermByCount14 RookMobilityBonus =
    [
        Utils.Pack(0, 0),
        Utils.Pack(7, 31),
        Utils.Pack(12, 34),
        Utils.Pack(16, 42),
        Utils.Pack(14, 52),
        Utils.Pack(21, 55),
        Utils.Pack(24, 62),
        Utils.Pack(29, 66),
        Utils.Pack(30, 78),
        Utils.Pack(34, 85),
        Utils.Pack(38, 87),
        Utils.Pack(41, 89),
        Utils.Pack(41, 93),
        Utils.Pack(55, 92),
        Utils.Pack(50, 90)
    ];

    #endregion
}

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default, WriteIndented = true)] // https://github.com/dotnet/runtime/issues/78602#issuecomment-1322004254
[JsonSerializable(typeof(EngineSettings))]
[JsonSerializable(typeof(SPSAAttribute<int>))]
[JsonSerializable(typeof(SPSAAttribute<double>))]
[JsonSerializable(typeof(WeatherFactoryOutput<int>))]
[JsonSerializable(typeof(WeatherFactoryOutput<double>))]
internal partial class EngineSettingsJsonSerializerContext : JsonSerializerContext;
