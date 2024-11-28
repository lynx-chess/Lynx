using Lynx.Model;
using Lynx.UCI.Commands.GUI;
using NLog;
using System.Diagnostics;
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
                    ? MovesDivisor(ExpectedMovesLeft(game.PositionHashHistoryLength()))
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SoftLimit(SearchConstraints searchConstraints, ulong bestMoveNodeCount, ulong totalNodeCount)
    {
        Debug.Assert(totalNodeCount > 0);
        Debug.Assert(totalNodeCount >= bestMoveNodeCount);

        double nodeTmBase = Configuration.EngineSettings.NodeTmBase;
        double nodeTmScale = Configuration.EngineSettings.NodeTmScale;

        double scale = 1.0;

        // Node time management: scale soft limit time bound by the proportion of nodes spent
        //   searching the best move at root level vs the total nodes searched.
        // The more time spent in best move -> the more sure we are about our previous results,
        //   so the less time we spent in the search.
        // i.e. with nodeTmBase = 2 and nodeTmScale = 1
        // - bestMoveFraction = 0.50 -> scale = 1.0 x (2 - 0.5) = 1.5
        // - bestMoveFraction = 0.25 -> scale = 1.0 x (2 - 0.25) = 1.75
        // - bestMoveFraction = 1.00 -> scale = 1.0 x (2 - 1.00) = 1
        double bestMoveFraction = (double)bestMoveNodeCount / totalNodeCount;
        var nodeTmFactor = nodeTmBase - (bestMoveFraction * nodeTmScale);
        scale *= nodeTmFactor;

        return (int)Math.Round(searchConstraints.SoftLimitTimeBound * scale);
    }

    /// <summary>
    /// Offset applied after <see cref="ExpectedMovesLeft(int)"/>
    /// Moves 0 - 20: 64 64 63 63 61 61 60 60 58 58 57 57 55 55 54 54 52 52 52 51
    /// Moves 19-29: 51 49 49 48 48 48 46 46 46 45 45 45 43 43 43 43 42 42 42 42
    /// Moves 39-49: 40 40 40 40 40 40 39 39 39 39 39 39 39 37 37 37 37 37 37 37
    /// Moves 59-69: 37 37 37 37 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36
    /// Moves 79-89: 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36
    /// Moves 99-109: 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36
    /// Moves 119-129: 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36 36
    /// Moves 139-149: 36 36 36 36 36 36 36 36 36 36 36 36 36 37 37 37 37 37 37 37
    /// Moves 159-169: 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37
    /// Moves 179-189: 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37
    /// Moves 199-209: 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 37 39
    /// Moves 219-229: 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39
    /// Moves 239-249: 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39
    /// Moves 259-269: 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39
    /// Moves 279-289: 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39
    /// Moves 299-309: 39 39 39 39 39 39 39 39 39 39 39 39 39 39 39 40 40 40 40 40
    /// Moves 319-329: 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40
    /// Moves 339-349: 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40
    /// Moves 359-369: 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40
    /// Moves 379-389: 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40
    /// Moves 399-409: 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40
    /// Moves 419-429: 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40
    /// Moves 439-449: 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40
    /// Moves 459-469: 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40 40
    /// Moves 479-489: 40 40 40 40 40 40 40 40 40 40 40 40 40 42 42 42 42 42 42 42
    /// </summary>
    /// <param name="expectedMovesLeft"></param>
    /// <returns></returns>
    private static int MovesDivisor(int expectedMovesLeft)
    {
        return expectedMovesLeft * 3 / 2;
    }

    /// <summary>
    /// Straight from expositor's author paper, https://expositor.dev/pdf/movetime.pdf
    /// 10 moves / 20 plies per row:
    /// Moves 0 - 20:   43 43 42 42 41 41 40 40 39 39 38 38 37 37 36 36 35 35 35 34
    /// Moves 19-29:    34 33 33 32 32 32 31 31 31 30 30 30 29 29 29 29 28 28 28 28
    /// Moves 39-49:    27 27 27 27 27 27 26 26 26 26 26 26 26 25 25 25 25 25 25 25
    /// Moves 59-69:    25 25 25 25 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24
    /// Moves 79-89:    24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24
    /// Moves 99-109:   24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24
    /// Moves 119-129:  24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24 24
    /// Moves 139-149:  24 24 24 24 24 24 24 24 24 24 24 24 24 25 25 25 25 25 25 25
    /// Moves 159-169:  25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25
    /// Moves 179-189:  25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25
    /// Moves 199-209:  25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 25 26
    /// Moves 219-229:  26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26
    /// Moves 239-249:  26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26
    /// Moves 259-269:  26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26
    /// Moves 279-289:  26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 26
    /// Moves 299-309:  26 26 26 26 26 26 26 26 26 26 26 26 26 26 26 27 27 27 27 27
    /// </summary>
    private static int ExpectedMovesLeft(int plies_played)
    {
        double p = plies_played;

        return (int)Math.Round(
            (59.3 + ((72830.0 - (p * 2330.0)) / ((p * p) + (p * 10.0) + 2644.0)))   // Plies remaining
            / 2.0); // Full moves remaining
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SoftLimit(SearchConstraints searchConstraints, ulong bestMoveNodeCount, ulong totalNodeCount)
    {
        Debug.Assert(totalNodeCount > 0);
        Debug.Assert(totalNodeCount >= bestMoveNodeCount);

        double nodeTmBase = Configuration.EngineSettings.NodeTmBase;
        double nodeTmScale = Configuration.EngineSettings.NodeTmScale;

        double scale = 1.0;

        // Node time management: scale soft limit time bound by the proportion of nodes spent
        // searching the best move at root level vs the total nodes searched
        // More time spent in best move -> less time for the rest of the search
        // i.e. with nodeTmBase = 2 and nodeTmScale = 1
        // - bestMoveFraction = 0.50 -> scale = 1.0 x (2 - 0.5) = 1.5
        // - bestMoveFraction = 0.25 -> scale = 1.0 x (2 - 0.25) = 1.75
        // - bestMoveFraction = 1.00 -> scale = 1.0 x (2 - 1.00) = 1
        double bestMoveFraction = (double)bestMoveNodeCount / totalNodeCount;
        var nodeTmFactor = nodeTmBase - (bestMoveFraction * nodeTmScale);
        scale *= nodeTmFactor;

        return (int)Math.Round(searchConstraints.SoftLimitTimeBound * scale);
    }
}
