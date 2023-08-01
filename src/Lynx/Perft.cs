using Lynx.Model;
using System.Diagnostics;

namespace Lynx;

/// <summary>
/// <see cref="https://www.chessprogramming.org/Perft"/>
/// Calculates the leaf nodes (number of positions) reached during the test of the move generator at a given depth
/// </summary>
public static class Perft
{
    public static long Results(Position position, int depth)
    {
        var sw = new Stopwatch();
        sw.Start();
        var nodes = ResultsImpl(position, depth, 0);
        sw.Stop();

        PrintPerftResult(depth, nodes, sw);

        return nodes;
    }

    public static long Divide(Position position, int depth)
    {
        var sw = new Stopwatch();
        sw.Start();
        var nodes = DivideImpl(position, depth, 0);
        sw.Stop();

        Console.WriteLine();
        PrintPerftResult(depth, nodes, sw);

        return nodes;
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
                var newPosition = new Position(position, move);

                if (newPosition.WasProduceByAValidMove())
                {
                    nodes = ResultsImpl(newPosition, depth - 1, nodes);
                }
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
                var newPosition = new Position(position, move);

                if (newPosition.WasProduceByAValidMove())
                {
                    var cummulativeNodes = nodes;

                    nodes = ResultsImpl(newPosition, depth - 1, nodes);

                    var oldNodes = nodes - cummulativeNodes;
                    Console.WriteLine($"{move.UCIString()}\t\t{oldNodes:N0}");
                }
            }

            return nodes;
        }

        return ++nodes;
    }

    private static void PrintPerftResult(int depth, long nodes, Stopwatch sw)
    {
        var timeStr = TimeToString(sw);

        Console.WriteLine(
            $"Depth:\t{depth}" + Environment.NewLine +
            $"Nodes:\t{nodes:N0}" + Environment.NewLine +
            $"Time:\t{timeStr}" + Environment.NewLine);
    }

    private static string TimeToString(Stopwatch stopwatch)
    {
        // http://geekswithblogs.net/BlackRabbitCoder/archive/2012/01/12/c.net-little-pitfalls-stopwatch-ticks-are-not-timespan-ticks.aspx
        static double CalculateElapsedMilliseconds(Stopwatch stopwatch)
        {
            return 1000 * stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
        }

        var milliseconds = CalculateElapsedMilliseconds(stopwatch);

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
