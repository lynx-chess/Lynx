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
        set
        {
            EngineSettings.TranspositionTableSize = value;
            if (value == 0)
            {
                EngineSettings.TranspositionTableEnabled = false;
            }
        }
    }
}

public sealed class GeneralSettings
{
    public bool EnableLogging { get; set; } = false;
}

public class TaperedEvaluationTerm
{
    public int MG { get; set; }

    public int EG { get; set; }

    internal TaperedEvaluationTerm(int singleValue) : this(singleValue, singleValue)
    {
    }

    public TaperedEvaluationTerm(int mg, int eg)
    {
        MG = mg;
        EG = eg;
    }

    public override string ToString()
    {
        return $"{{\"MG\":{MG},\"EG\":{EG}}}";
    }
}

public class TaperedEvaluationTermByRank
{
    private readonly List<TaperedEvaluationTerm> _evaluationTermsIndexedByPiece;

    public TaperedEvaluationTerm Rank0 { get; set; }
    public TaperedEvaluationTerm Rank1 { get; set; }
    public TaperedEvaluationTerm Rank2 { get; set; }
    public TaperedEvaluationTerm Rank3 { get; set; }
    public TaperedEvaluationTerm Rank4 { get; set; }
    public TaperedEvaluationTerm Rank5 { get; set; }
    public TaperedEvaluationTerm Rank6 { get; set; }
    public TaperedEvaluationTerm Rank7 { get; set; }

    public TaperedEvaluationTermByRank(
        TaperedEvaluationTerm rank0, TaperedEvaluationTerm rank1, TaperedEvaluationTerm rank2,
        TaperedEvaluationTerm rank3, TaperedEvaluationTerm rank4, TaperedEvaluationTerm rank5,
        TaperedEvaluationTerm rank6, TaperedEvaluationTerm rank7)
    {
        Rank0 = rank0;
        Rank1 = rank1;
        Rank2 = rank2;
        Rank3 = rank3;
        Rank4 = rank4;
        Rank5 = rank5;
        Rank6 = rank6;
        Rank7 = rank7;

        _evaluationTermsIndexedByPiece = [rank0, rank1, rank2, rank3, rank4, rank5, rank6, rank7];
    }

    public TaperedEvaluationTerm this[int i]
    {
        get { return _evaluationTermsIndexedByPiece[i]; }
    }

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

public sealed class EngineSettings
{
    public int DefaultMaxDepth { get; set; } = 5;

    #region MovesToGo provided

    /// <summary>
    /// Coefficient applied to ensure more time is allocated to moves when there are over <see cref="KeyMovesBeforeMovesToGo"/> moves left
    /// </summary>
    public double CoefficientBeforeKeyMovesBeforeMovesToGo { get; set; } = 1.5;

    public int KeyMovesBeforeMovesToGo { get; set; } = 10;

    /// <summary>
    /// Security coefficient applied to ensure there are no timeouts when there are less than <see cref="KeyMovesBeforeMovesToGo"/>  movesleft
    /// </summary>
    public double CoefficientAfterKeyMovesBeforeMovesToGo { get; set; } = 0.95;

    #endregion

    #region No MovesToGo provided

    /// <summary>
    /// Number of total moves to calculate decision time against
    /// </summary>
    public int TotalMovesWhenNoMovesToGoProvided { get; set; } = 100;

    /// <summary>
    /// Number of extra moves to calculate decision time against, when the number of moves exceeds <see cref="TotalMovesWhenNoMovesToGoProvided"/>
    /// </summary>
    public int FixedMovesLeftWhenNoMovesToGoProvidedAndOverTotalMovesWhenNoMovesToGoProvided { get; set; } = 20;

    /// <summary>
    /// Min time to apply <see cref="FirstCoefficientWhenNoMovesToGoProvided"/>
    /// </summary>
    public int FirstTimeLimitWhenNoMovesToGoProvided { get; set; } = 120_000;

    /// <summary>
    /// Coefficient applied to ensure more time is allocated to moves when there's over <see cref="FirstTimeLimitWhenNoMovesToGoProvided"/> ms on the clock
    /// </summary>
    public int FirstCoefficientWhenNoMovesToGoProvided { get; set; } = 3;

    /// <summary>
    /// Min time to apply <see cref="SecondCoefficientWhenNoMovesToGoProvided"/>
    /// </summary>
    public int SecondTimeLimitWhenNoMovesToGoProvided { get; set; } = 30_000;

    /// <summary>
    /// Coefficient applied to ensure more time is allocated to moves when there's over <see cref="SecondTimeLimitWhenNoMovesToGoProvided"/> ms on the clock
    /// </summary>
    public int SecondCoefficientWhenNoMovesToGoProvided { get; set; } = 2;

    #endregion

    /// <summary>
    /// Min. time left in the clock if all decision time is used before <see cref="CoefficientSecurityTime"/> is used over that decision time
    /// </summary>
    public int MinSecurityTime { get; set; } = 1_000;

    /// <summary>
    /// Coefficient applied to devision tim if the time left in the clock after spending it is less than <see cref="MinSecurityTime"/>
    /// </summary>
    public double CoefficientSecurityTime { get; set; } = 0.9;

    public int MinDepth { get; set; } = 4;

    private int _maxDepth = 128;
    public int MaxDepth { get => _maxDepth; set => _maxDepth = Math.Clamp(value, 1, Constants.AbsoluteMaxDepth); }

    //public int MinMoveTime { get; set; } = 1_000;

    //public int DepthWhenLessThanMinMoveTime { get; set; } = 4;

    public int MinElapsedTimeToConsiderStopSearching { get; set; } = 0;

    /// <summary>
    /// Elapsed time must be greater than <see cref="DecisionTimeFactorToStopSearching"/> * decision time
    /// to continue the search, providing elaped time is greater than <see cref="MinElapsedTimeToConsiderStopSearching"/>
    /// </summary>
    public double DecisionTimeFactorToStopSearching { get; set; } = 0.4;

    /// <summary>
    /// Time increment between last 2 searches
    /// </summary>
    public int MinLastSearchTimeToConsiderStopSearching { get; set; } = 1_000;

    /// <summary>
    /// Remaining time must be greater than <see cref="LastSearchTimeFactorToStopSearching"/> * last search time
    /// to continue the search, providing last search time is greater than <see cref="MinLastSearchTimeToConsiderStopSearching"/>
    /// </summary>
    public double LastSearchTimeFactorToStopSearching { get; set; } = 1.0;

    public int LMR_MinFullDepthSearchedMoves { get; set; } = 4;

    public int LMR_MaxDepth { get; set; } = 3;

    public int LMR_DepthReduction { get; set; } = 1;

    public int NMP_DepthReduction { get; set; } = 3;

    public int AspirationWindowAlpha { get; set; } = 50;

    public int AspirationWindowBeta { get; set; } = 50;

    #region Evaluation

    public TaperedEvaluationTerm DoubledPawnPenalty { get; set; } = new(-3, -11);

    public TaperedEvaluationTerm IsolatedPawnPenalty { get; set; } = new(-13, -10);

    public TaperedEvaluationTerm OpenFileRookBonus { get; set; } = new(43, 22);

    public TaperedEvaluationTerm SemiOpenFileRookBonus { get; set; } = new(18, 16);

    public TaperedEvaluationTerm BishopMobilityBonus { get; set; } = new(8, 7);

    public TaperedEvaluationTerm QueenMobilityBonus { get; set; } = new(2, 7);

    public TaperedEvaluationTerm SemiOpenFileKingPenalty { get; set; } = new(-29, 19);

    public TaperedEvaluationTerm OpenFileKingPenalty { get; set; } = new(-81, 3);

    public TaperedEvaluationTerm KingShieldBonus { get; set; } = new(15, -5);

    public TaperedEvaluationTerm BishopPairBonus { get; set; } = new(22, 65);

    public TaperedEvaluationTermByRank PassedPawnBonus { get; set; } = new(
        new(0),
        new(-2, 5),
        new(-13, 10),
        new(-12, 32),
        new(13, 62),
        new(38, 132),
        new(53, 191),
        new(200));

    #endregion

    public bool TranspositionTableEnabled { get; set; } = true;

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

    /// <summary>
    /// Depth for bench command
    /// </summary>
    public int BenchDepth { get; set; } = 5;

    public int RFP_MaxDepth { get; set; } = 6;

    public int RFP_DepthScalingFactor { get; set; } = 75;
}

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default, WriteIndented = true)] // https://github.com/dotnet/runtime/issues/78602#issuecomment-1322004254
[JsonSerializable(typeof(EngineSettings))]
[JsonSerializable(typeof(TaperedEvaluationTerm))]
[JsonSerializable(typeof(TaperedEvaluationTermByRank))]
internal partial class EngineSettingsJsonSerializerContext : JsonSerializerContext
{
}
