﻿using System.Runtime.CompilerServices;
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
    private static int _ponder = 0;

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

    public static bool IsPonder
    {
        get => Interlocked.CompareExchange(ref _ponder, 1, 1) == 1;
        set
        {
            if (value)
            {
                Interlocked.CompareExchange(ref _ponder, 1, 0);
            }
            else
            {
                Interlocked.CompareExchange(ref _ponder, 0, 1);
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
    public int BenchDepth { get; set; } = 8;

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

    public double SPSA_OB_R_end { get; set; } = 0.02;

    #region Time management

    public double HardTimeBoundMultiplier { get; set; } = 0.52;

    public double SoftTimeBoundMultiplier { get; set; } = 1;

    public int DefaultMovesToGo { get; set; } = 45;

    public double SoftTimeBaseIncrementMultiplier { get; set; } = 0.8;

    #endregion

    [SPSAAttribute<int>(2, 10, 0.5)]
    public int LMR_MinDepth { get; set; } = 3;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int LMR_MinFullDepthSearchedMoves { get; set; } = 3;

    /// <summary>
    /// Value originally from Stormphrax, who apparently took it from Viridithas
    /// </summary>
    [SPSAAttribute<double>(0.1, 2, 0.10)]
    public double LMR_Base { get; set; } = 0.83;

    /// <summary>
    /// Value originally from Akimbo
    /// </summary>
    [SPSAAttribute<double>(1, 5, 0.1)]
    public double LMR_Divisor { get; set; } = 3.43;

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
    public int RFP_DepthScalingFactor { get; set; } = 97;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int Razoring_MaxDepth { get; set; } = 1;

    [SPSAAttribute<int>(1, 300, 15)]
    public int Razoring_Depth1Bonus { get; set; } = 127;

    [SPSAAttribute<int>(1, 300, 15)]
    public int Razoring_NotDepth1Bonus { get; set; } = 149;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int IIR_MinDepth { get; set; } = 4;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int LMP_MaxDepth { get; set; } = 6;

    [SPSAAttribute<int>(0, 10, 0.5)]
    public int LMP_BaseMovesToTry { get; set; } = 0;

    [SPSAAttribute<int>(0, 10, 0.5)]
    public int LMP_MovesDepthMultiplier { get; set; } = 3;

    public int History_MaxMoveValue { get; set; } = 8_192;

    /// <summary>
    /// 1896: constant from depth 12
    /// </summary>
    public int History_MaxMoveRawBonus { get; set; } = 1_896;

    [SPSAAttribute<int>(0, 6, 0.5)]
    public int SEE_BadCaptureReduction { get; set; } = 1;

    [SPSAAttribute<int>(1, 10, 0.5)]
    public int FP_MaxDepth { get; set; } = 5;

    [SPSAAttribute<int>(1, 200, 10)]
    public int FP_DepthScalingFactor { get; set; } = 62;

    [SPSAAttribute<int>(0, 500, 25)]
    public int FP_Margin { get; set; } = 170;

    #region Evaluation

    public TaperedEvaluationTerm IsolatedPawnPenalty { get; set; } = new(-21, -18);

    public TaperedEvaluationTerm OpenFileRookBonus { get; set; } = new(46, 9);

    public TaperedEvaluationTerm SemiOpenFileRookBonus { get; set; } = new(15, 14);

    public TaperedEvaluationTerm RookMobilityBonus { get; set; } = new(5, 5);

    public TaperedEvaluationTerm QueenMobilityBonus { get; set; } = new(4, 7);

    public TaperedEvaluationTerm SemiOpenFileKingPenalty { get; set; } = new(-39, 21);

    public TaperedEvaluationTerm OpenFileKingPenalty { get; set; } = new(-105, 7);

    public TaperedEvaluationTerm KingShieldBonus { get; set; } = new(16, -6);

    public TaperedEvaluationTerm BishopPairBonus { get; set; } = new(31, 80);

    public TaperedEvaluationTermByRank PassedPawnBonus { get; set; } = new(
        new(0, 0),
        new(2, 12),
        new(-11, 19),
        new(-11, 47),
        new(19, 80),
        new(58, 156),
        new(95, 223),
        new(0, 0));

    public TaperedEvaluationTermByCount BishopMobilityBonus { get; set; } = new(
        new(0, 0),
        new(196, 160),
        new(208, 159),
        new(219, 198),
        new(233, 214),
        new(241, 229),
        new(256, 249),
        new(266, 259),
        new(275, 271),
        new(276, 277),
        new(282, 282),
        new(284, 279),
        new(286, 278),
        new(315, 272),
        new(0, 0));

    #endregion
}

public sealed class TaperedEvaluationTerm
{
    [JsonIgnore]
    public int PackedEvaluation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;

        [Obsolete("Test only")]
        private set;
    }

    public int MG
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Utils.UnpackMG(PackedEvaluation);
        }
        [Obsolete("Test only, will reset internal value")]
        set
        {
            PackedEvaluation = value;
        }
    }

    public int EG
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return Utils.UnpackEG(PackedEvaluation);
        }
        [Obsolete("Test only")]
        set
        {
            PackedEvaluation += (value << 16);
        }
    }

    public TaperedEvaluationTerm(int mg, int eg)
    {
#pragma warning disable CS0618 // Type or member is obsolete - correct usage here, setter wouldn't even be needed
        PackedEvaluation = Utils.Pack((short)mg, (short)eg);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public override string ToString()
    {
        return $"{{\"MG\":{MG},\"EG\":{EG}}}";
    }
}

public sealed class TaperedEvaluationTermByRank
{
    private readonly TaperedEvaluationTerm[] _evaluationTermsIndexedByPiece;

    public TaperedEvaluationTerm Rank0 => _evaluationTermsIndexedByPiece[0];
    public TaperedEvaluationTerm Rank1 => _evaluationTermsIndexedByPiece[1];
    public TaperedEvaluationTerm Rank2 => _evaluationTermsIndexedByPiece[2];
    public TaperedEvaluationTerm Rank3 => _evaluationTermsIndexedByPiece[3];
    public TaperedEvaluationTerm Rank4 => _evaluationTermsIndexedByPiece[4];
    public TaperedEvaluationTerm Rank5 => _evaluationTermsIndexedByPiece[5];
    public TaperedEvaluationTerm Rank6 => _evaluationTermsIndexedByPiece[6];
    public TaperedEvaluationTerm Rank7 => _evaluationTermsIndexedByPiece[7];

    public TaperedEvaluationTermByRank(
        TaperedEvaluationTerm rank0, TaperedEvaluationTerm rank1, TaperedEvaluationTerm rank2,
        TaperedEvaluationTerm rank3, TaperedEvaluationTerm rank4, TaperedEvaluationTerm rank5,
        TaperedEvaluationTerm rank6, TaperedEvaluationTerm rank7)
    {
        _evaluationTermsIndexedByPiece = [rank0, rank1, rank2, rank3, rank4, rank5, rank6, rank7];
    }

    public TaperedEvaluationTerm this[int i] => _evaluationTermsIndexedByPiece[i];

    public override string ToString()
    {
        return "{" +
            $"\"{nameof(Rank0)}\":{Rank0}," +
            $"\"{nameof(Rank1)}\":{Rank1}," +
            $"\"{nameof(Rank2)}\":{Rank2}," +
            $"\"{nameof(Rank3)}\":{Rank3}," +
            $"\"{nameof(Rank4)}\":{Rank4}," +
            $"\"{nameof(Rank5)}\":{Rank5}," +
            $"\"{nameof(Rank6)}\":{Rank6}," +
            $"\"{nameof(Rank7)}\":{Rank7}" +
            "}";
    }
}

/// <summary>
/// 13 for bishop,
/// 14 bor rook
/// </summary>
public sealed class TaperedEvaluationTermByCount
{
    private readonly TaperedEvaluationTerm[] _evaluationTermsIndexedByCount;

    public TaperedEvaluationTerm Count0 => _evaluationTermsIndexedByCount[0];
    public TaperedEvaluationTerm Count1 => _evaluationTermsIndexedByCount[1];
    public TaperedEvaluationTerm Count2 => _evaluationTermsIndexedByCount[2];
    public TaperedEvaluationTerm Count3 => _evaluationTermsIndexedByCount[3];
    public TaperedEvaluationTerm Count4 => _evaluationTermsIndexedByCount[4];
    public TaperedEvaluationTerm Count5 => _evaluationTermsIndexedByCount[5];
    public TaperedEvaluationTerm Count6 => _evaluationTermsIndexedByCount[6];
    public TaperedEvaluationTerm Count7 => _evaluationTermsIndexedByCount[7];
    public TaperedEvaluationTerm Count8 => _evaluationTermsIndexedByCount[8];
    public TaperedEvaluationTerm Count9 => _evaluationTermsIndexedByCount[9];
    public TaperedEvaluationTerm Count10 => _evaluationTermsIndexedByCount[10];
    public TaperedEvaluationTerm Count11 => _evaluationTermsIndexedByCount[11];
    public TaperedEvaluationTerm Count12 => _evaluationTermsIndexedByCount[12];
    public TaperedEvaluationTerm Count13 => _evaluationTermsIndexedByCount[13];
    public TaperedEvaluationTerm Count14 => _evaluationTermsIndexedByCount[14];

    public TaperedEvaluationTermByCount(
        TaperedEvaluationTerm rank0, TaperedEvaluationTerm rank1, TaperedEvaluationTerm rank2,
        TaperedEvaluationTerm rank3, TaperedEvaluationTerm rank4, TaperedEvaluationTerm rank5,
        TaperedEvaluationTerm rank6, TaperedEvaluationTerm rank7, TaperedEvaluationTerm rank8,
        TaperedEvaluationTerm rank9, TaperedEvaluationTerm rank10, TaperedEvaluationTerm rank11,
        TaperedEvaluationTerm rank12, TaperedEvaluationTerm rank13, TaperedEvaluationTerm rank14)
    {
        _evaluationTermsIndexedByCount =
            [rank0, rank1, rank2, rank3, rank4, rank5, rank6, rank7, rank8, rank9, rank10, rank11, rank12, rank13, rank14];
    }

    public TaperedEvaluationTerm this[int i] => _evaluationTermsIndexedByCount[i];

    public override string ToString()
    {
        return "{" +
            $"\"{nameof(Count0)}\":{Count0}," +
            $"\"{nameof(Count1)}\":{Count1}," +
            $"\"{nameof(Count2)}\":{Count2}," +
            $"\"{nameof(Count3)}\":{Count3}," +
            $"\"{nameof(Count4)}\":{Count4}," +
            $"\"{nameof(Count5)}\":{Count5}," +
            $"\"{nameof(Count6)}\":{Count6}," +
            $"\"{nameof(Count7)}\":{Count7}," +
            $"\"{nameof(Count8)}\":{Count8}," +
            $"\"{nameof(Count9)}\":{Count9}," +
            $"\"{nameof(Count10)}\":{Count10}," +
            $"\"{nameof(Count11)}\":{Count11}," +
            $"\"{nameof(Count12)}\":{Count12}," +
            $"\"{nameof(Count13)}\":{Count13}," +
            $"\"{nameof(Count14)}\":{Count14}" +
            "}";
    }
}

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default, WriteIndented = true)] // https://github.com/dotnet/runtime/issues/78602#issuecomment-1322004254
[JsonSerializable(typeof(EngineSettings))]
[JsonSerializable(typeof(TaperedEvaluationTerm))]
[JsonSerializable(typeof(TaperedEvaluationTermByRank))]
[JsonSerializable(typeof(SPSAAttribute<int>))]
[JsonSerializable(typeof(SPSAAttribute<double>))]
internal partial class EngineSettingsJsonSerializerContext : JsonSerializerContext;
