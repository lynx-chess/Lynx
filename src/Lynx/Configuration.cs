using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lynx
{
    public static class Configuration
    {
        private static int _isDebug = 0;
        private static int _UCI_AnalyseMode = 0;
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

        public static GameParameters Parameters { get; set; } = new GameParameters();
    }

    public class GameParameters
    {
        public int Depth { get; set; } = 5;

        public int QuiescenceSearchDepth { get; set; } = 8;

        #region MovesToGo provided

        public double CoefficientBeforeKeyMovesBeforeMovesToGo { get; set; } = 1.5;

        public int KeyMovesBeforeMovesToGo { get; set; } = 10;

        public double CoefficientAfterKeyMovesBeforeMovesToGo { get; set; } = 0.95;

        public int TotalMovesWhenNoMovesToGoProvided { get; set; } = 100;

        #endregion

        #region No MovesToGo provided

        public int FirstTimeLimitWhenNoMovesToGoProvided { get; set; } = 120_000;

        public int FirstCoefficientWhenNoMovesToGoProvided { get; set; } = 3;

        public int SecondTimeLimitWhenNoMovesToGoProvided { get; set; } = 30_000;

        public int SecondCoefficientWhenNoMovesToGoProvided { get; set; } = 2;

        #endregion

        public int MinDepth { get; set; } = 5;

        public int MinMoveTime { get; set; } = 1_000;

        public int MinDepthWhenLessThanMinMoveTime { get; set; } = 3;
    }
}
