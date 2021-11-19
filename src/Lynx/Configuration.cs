namespace Lynx
{
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
        public int Depth { get; set; } = 5;

        public int QuiescenceSearchDepth { get; set; } = 8;

        #region MovesToGo provided

        public double CoefficientBeforeKeyMovesBeforeMovesToGo { get; set; } = 1.5;

        public int KeyMovesBeforeMovesToGo { get; set; } = 10;

        public double CoefficientAfterKeyMovesBeforeMovesToGo { get; set; } = 0.95;

        #endregion

        #region No MovesToGo provided

        public int TotalMovesWhenNoMovesToGoProvided { get; set; } = 100;

        public int FixedMovesLeftWhenNoMovesToGoProvidedAndOverTotalMovesWhenNoMovesToGoProvided { get; set; } = 20;

        public int FirstTimeLimitWhenNoMovesToGoProvided { get; set; } = 120_000;

        public int FirstCoefficientWhenNoMovesToGoProvided { get; set; } = 3;

        public int SecondTimeLimitWhenNoMovesToGoProvided { get; set; } = 30_000;

        public int SecondCoefficientWhenNoMovesToGoProvided { get; set; } = 2;

        #endregion

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
    }
}
