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
        public int Depth { get; set; } = 3;
    }
}
