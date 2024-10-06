using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx;

/// <summary>
/// <see cref="https://www.chessprogramming.org/Perft"/>
/// Calculates the leaf nodes (number of positions) reached during the test of the move generator at a given depth
/// </summary>
public static class Perft
{
    public static (long Nodes, double ElapsedSeconds) Results(Position position, int depth)
    {
        var sw = new Stopwatch();
        sw.Start();
        var nodes = ResultsImpl(position, depth, 0);
        sw.Stop();

        return (nodes, Utils.CalculateElapsedSeconds(sw));
    }

    public static (long Nodes, double ElapsedSeconds) Divide(Position position, int depth, Action<string> write)
    {
        var sw = new Stopwatch();
        sw.Start();
        var nodes = DivideImpl(position, depth, 0, write);
        sw.Stop();

        return (nodes, Utils.CalculateElapsedSeconds(sw));
    }

    /// <summary>
    /// Proper implementation, used by <see cref="DivideImpl(Position, int, long)"/> as well
    /// </summary>
    /// <param name="position"></param>
    /// <param name="depth"></param>
    /// <param name="nodes"></param>
    /// <returns></returns>
    [SkipLocalsInit]
    internal static long ResultsImpl(Position position, int depth, long nodes)
    {
        if (depth != 0)
        {
            Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            foreach (var move in MoveGenerator.GenerateAllMoves(position, moves))
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

        return nodes + 1;
    }

    [SkipLocalsInit]
    private static long DivideImpl(Position position, int depth, long nodes, Action<string> write)
    {
        if (depth != 0)
        {
            Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            foreach (var move in MoveGenerator.GenerateAllMoves(position, moves))
            {
                var state = position.MakeMove(move);

                if (position.WasProduceByAValidMove())
                {
                    var accumulatedNodes = nodes;
                    nodes = ResultsImpl(position, depth - 1, nodes);

                    write($"{move.UCIString()}\t\t{nodes - accumulatedNodes}");
                }

                position.UnmakeMove(move, state);
            }

            write(string.Empty);

            return nodes;
        }

        return nodes + 1;
    }

    public static void PrintPerftResult(int depth, (long Nodes, double ElapsedSeconds) peftResult, Action<string> write)
    {
        var timeStr = TimeToString(peftResult.ElapsedSeconds * 1_000);

        write(
            $"Depth:\t{depth}" + Environment.NewLine +
            $"Nodes:\t{peftResult.Nodes}" + Environment.NewLine +
            $"Time:\t{timeStr}" + Environment.NewLine +
            $"nps:\t{peftResult.Nodes / (peftResult.ElapsedSeconds * 1_000_000):F} Mnps" + Environment.NewLine);
    }

    public static async ValueTask PrintPerftResult(int depth, (long Nodes, double ElapsedSeconds) peftResult, Func<string, ValueTask> write)
    {
        var timeStr = TimeToString(peftResult.ElapsedSeconds * 1_000);

        await write(
            $"Depth:\t{depth}" + Environment.NewLine +
            $"Nodes:\t{peftResult.Nodes}" + Environment.NewLine +
            $"Time:\t{timeStr}" + Environment.NewLine +
            $"nps:\t{peftResult.Nodes / (peftResult.ElapsedSeconds * 1_000_000):F} Mnps" + Environment.NewLine);
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
