namespace Lynx;

public static class Configuration
{
    private static int _isDebug = 0;
#pragma warning disable IDE1006 // Naming Styles
    private static int _UCI_AnalyseMode = 0;
#pragma warning restore IDE1006 // Naming Styles
    private static int _ponder = 0;
    private static int _hash = 0;

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
        get => _hash;
        set => Interlocked.Exchange(ref _hash, value);
    }

    public static EngineSettings EngineSettings { get; set; } = new EngineSettings();

    public static GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();
}

public sealed class GeneralSettings
{
    public bool DisableLogging { get; set; } = false;
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
    /// Min. time left in the clock if all decision time is used befire <see cref="CoefficientSecurityTime"/> is used over that decision time
    /// </summary>
    public int MinSecurityTime { get; set; } = 1_000;

    /// <summary>
    /// Coefficient applied to devision tim if the time left in the clock after spending it is less than <see cref="MinSecurityTime"/>
    /// </summary>
    public double CoefficientSecurityTime { get; set; } = 0.9;

    public int MinDepth { get; set; } = 4;

    public int MaxDepth { get; set; } = 32;

    public int MinMoveTime { get; set; } = 1_000;

    public int DepthWhenLessThanMinMoveTime { get; set; } = 4;

    public int MinElapsedTimeToConsiderStopSearching { get; set; } = 1_000;

    public double DecisionTimePercentageToStopSearching { get; set; } = 0.5;

    public int LMR_FullDepthMoves { get; set; } = 4;

    public int LMR_ReductionLimit { get; set; } = 3;

    public int LMR_DepthReduction { get; set; } = 1;

    public int NullMovePruning_R { get; set; } = 3;

    public int AspirationWindowAlpha { get; set; } = 50;

    public int AspirationWindowBeta { get; set; } = 50;

    public int IsolatedPawnPenalty { get; set; } = 10;

    public int DoubledPawnPenalty { get; set; } = 10;

    public int[] PassedPawnBonus { get; set; } = new[] { 0, 10, 30, 50, 75, 100, 150, 200 };

    public int SemiOpenFileRookBonus { get; set; } = 10;

    public int OpenFileRookBonus { get; set; } = 15;

    public int SemiOpenFileKingPenalty { get; set; } = 10;

    public int OpenFileKingPenalty { get; set; } = 15;

    public int KingShieldBonus { get; set; } = 5;

    public int BishopMobilityBonus { get; set; } = 1;
}
