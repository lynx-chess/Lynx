using Lynx.Model;
using System.Diagnostics;
using System.Threading.Channels;

namespace Lynx;

/// <summary>
/// <see cref="https://www.chessprogramming.org/Perft"/>
/// Calculates the leaf nodes (number of positions) reached during the test of the move generator at a given depth
/// </summary>
public static class Perft
{
    public static (long Nodes, double ElapsedMilliseconds) Results(Position position, int depth)
    {
        var sw = new Stopwatch();
        sw.Start();
        var nodes = ResultsImpl(position, depth, 0);
        sw.Stop();

        return (nodes, CalculateElapsedMilliseconds(sw));
    }

    public static (long Nodes, double ElapsedMilliseconds) Divide(Position position, int depth)
    {
        var sw = new Stopwatch();
        sw.Start();
        var nodes = DivideImpl(position, depth, 0);
        sw.Stop();

        return (nodes, CalculateElapsedMilliseconds(sw));
    }

    /// <summary>
    /// Proper implementation, used by <see cref="DivideImpl(Position, int, long)"/> as well
    /// </summary>
    /// <param name="position"></param>
    /// <param name="depth"></param>
    /// <param name="nodes"></param>
    /// <returns></returns>
    internal static long ResultsImpl(Position position, int depth, long nodes)
    {
        if (depth != 0)
        {
            foreach (var move in MoveGenerator.GenerateAllMoves(position))
            {
                var state = position.MakeMove(move);

                if (position.WasProduceByAValidMove())
                {
                    nodes = ResultsImpl(position, depth - 1, nodes);
                }
                position.UnmakeMove(move, state);
            }

            return nodes;
        }

        return ++nodes;
    }

    private static long DivideImpl(Position position, int depth, long nodes)
    {
        if (depth != 0)
        {
            foreach (var move in MoveGenerator.GenerateAllMoves(position))
            {
                var state = position.MakeMove(move);

                if (position.WasProduceByAValidMove())
                {
                    var cummulativeNodes = nodes;

                    nodes = ResultsImpl(position, depth - 1, nodes);

                    var oldNodes = nodes - cummulativeNodes;
                    Console.WriteLine($"{move.UCIString()}\t\t{oldNodes:N0}");
                }

                position.UnmakeMove(move, state);
            }

            return nodes;
        }

        return ++nodes;
    }

    #region Legacy

    /// <summary>
    /// Proper implementation, used by <see cref="DivideImplUsingPositionConstructor(Position, int, long)"/> as well
    /// </summary>
    /// <param name="position"></param>
    /// <param name="depth"></param>
    /// <param name="nodes"></param>
    /// <returns></returns>
    private static long ResultsImplUsingPositionConstructor(Position position, int depth, long nodes)
    {
        if (depth != 0)
        {
            foreach (var move in MoveGenerator.GenerateAllMoves(position))
            {
                var newPosition = new Position(position, move);

                if (newPosition.WasProduceByAValidMove())
                {
                    nodes = ResultsImplUsingPositionConstructor(newPosition, depth - 1, nodes);
                }
            }

            return nodes;
        }

        return ++nodes;
    }

    private static long DivideImplUsingPositionConstructor(Position position, int depth, long nodes)
    {
        if (depth != 0)
        {
            foreach (var move in MoveGenerator.GenerateAllMoves(position))
            {
                var newPosition = new Position(position, move);

                if (newPosition.WasProduceByAValidMove())
                {
                    var cummulativeNodes = nodes;

                    nodes = ResultsImplUsingPositionConstructor(newPosition, depth - 1, nodes);

                    var oldNodes = nodes - cummulativeNodes;
                    Console.WriteLine($"{move.UCIString()}\t\t{oldNodes:N0}");
                }
            }

            return nodes;
        }

        return ++nodes;
    }

    #endregion

    public static void PrintPerftResult(int depth, (long Nodes, double ElapsedMilliseconds) peftResult, Action<string> write)
    {
        var timeStr = TimeToString(peftResult.ElapsedMilliseconds);

        write(
            $"Depth:\t{depth}" + Environment.NewLine +
            $"Nodes:\t{peftResult.Nodes:N0}" + Environment.NewLine +
            $"Time:\t{timeStr}" + Environment.NewLine);
    }

    public static async ValueTask PrintPerftResult(int depth, (long Nodes, double ElapsedMilliseconds) peftResult, Func<string, ValueTask> write)
    {
        var timeStr = TimeToString(peftResult.ElapsedMilliseconds);

        await write(
            $"Depth:\t{depth}" + Environment.NewLine +
            $"Nodes:\t{peftResult.Nodes:N0}" + Environment.NewLine +
            $"Time:\t{timeStr}" + Environment.NewLine);
    }

    /// <summary>
    /// http://geekswithblogs.net/BlackRabbitCoder/archive/2012/01/12/c.net-little-pitfalls-stopwatch-ticks-are-not-timespan-ticks.aspx
    /// </summary>
    /// <param name="stopwatch"></param>
    /// <returns></returns>
    private static double CalculateElapsedMilliseconds(Stopwatch stopwatch)
    {
        return 1000 * stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
    }

    private static string TimeToString(double milliseconds)
    {
        return milliseconds switch
        {
            < 1 => $"{milliseconds:F} ms",
            < 1_000 => $"{Math.Round(milliseconds)} ms",
            < 60_000 => $"{0.001 * milliseconds:F} s",
            < 3_600_000 => $"{Math.Floor(milliseconds / 60_000)} min {Math.Round(0.001 * (milliseconds % 60_000))} s",
            _ => $"{Math.Floor(milliseconds / 3_600_000)} h {Math.Round((milliseconds % 3_600_000) / 60_000)} min"
        };
    }
}
