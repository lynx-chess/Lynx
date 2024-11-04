using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Runtime.CompilerServices;

namespace Lynx;
public static class TimeManager
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static SearchConstraints CalculateTimeManagement(Game game, GoCommand goCommand)
    {
        bool isPondering = goCommand.Ponder;

        int maxDepth = -1;
        int hardLimitTimeBound = SearchConstraints.DefaultHardLimitTimeBound;
        int softLimitTimeBound = int.MaxValue;

        double millisecondsLeft;
        int millisecondsIncrement;
        if (game.CurrentPosition.Side == Side.White)
        {
            millisecondsLeft = goCommand.WhiteTime;
            millisecondsIncrement = goCommand.WhiteIncrement;
        }
        else
        {
            millisecondsLeft = goCommand.BlackTime;
            millisecondsIncrement = goCommand.BlackIncrement;
        }

        // Inspired by Alexandria: time overhead to avoid timing out in the engine-gui communication process
        const int engineGuiCommunicationTimeOverhead = 50;

        if (!isPondering)
        {
            if (goCommand.WhiteTime != 0 || goCommand.BlackTime != 0)  // Cutechess sometimes sends negative wtime/btime
            {
                const int minSearchTime = 50;

                var movesDivisor = goCommand.MovesToGo == 0
                    ? ExpectedMovesLeft(game.PositionHashHistoryLength()) * 3 / 2
                    : goCommand.MovesToGo;

                millisecondsLeft -= engineGuiCommunicationTimeOverhead;
                millisecondsLeft = Math.Clamp(millisecondsLeft, minSearchTime, int.MaxValue); // Avoiding 0/negative values

                hardLimitTimeBound = (int)(millisecondsLeft * Configuration.EngineSettings.HardTimeBoundMultiplier);

                var softLimitBase = (millisecondsLeft / movesDivisor) + (millisecondsIncrement * Configuration.EngineSettings.SoftTimeBaseIncrementMultiplier);
                softLimitTimeBound = Math.Min(hardLimitTimeBound, (int)(softLimitBase * Configuration.EngineSettings.SoftTimeBoundMultiplier));

                _logger.Info("Soft time bound: {0}s", 0.001 * softLimitTimeBound);
                _logger.Info("Hard time bound: {0}s", 0.001 * hardLimitTimeBound);
            }
            else if (goCommand.MoveTime > 0)
            {
                softLimitTimeBound = hardLimitTimeBound = goCommand.MoveTime - engineGuiCommunicationTimeOverhead;
                _logger.Info("Time to move: {0}s", 0.001 * hardLimitTimeBound);
            }
            else if (goCommand.Depth > 0)
            {
                maxDepth = goCommand.Depth > Constants.AbsoluteMaxDepth ? Constants.AbsoluteMaxDepth : goCommand.Depth;
            }
            else if (goCommand.Infinite)
            {
                maxDepth = Configuration.EngineSettings.MaxDepth;
                _logger.Info("Infinite search (depth {0})", maxDepth);
            }
            else
            {
                maxDepth = Engine.DefaultMaxDepth;
                _logger.Warn("Unexpected or unsupported go command");
            }
        }
        else
        {
            maxDepth = Configuration.EngineSettings.MaxDepth;
            _logger.Info("Pondering search (depth {0})", maxDepth);
        }

        return new(hardLimitTimeBound, softLimitTimeBound, maxDepth);
    }

    /// <summary>
    /// Straight from expositor's author paper, https://expositor.dev/pdf/movetime.pdf
    /// </summary>
    /// <param name="plies_played"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ExpectedMovesLeft(int plies_played)
    {
        double p = (double)(plies_played);

        return (int)Math.Round(
            (59.3 + ((72830.0 - (p * 2330.0)) / ((p * p) + (p * 10.0) + 2644.0)))   // Plies remaining
            / 2.0); // Full moves remaining
    }
}
