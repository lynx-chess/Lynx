using Lynx.Model;
using System.Diagnostics;

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

    public static (long Nodes, double ElapsedMilliseconds) Divide(Position position, int depth, Action<string> write)
    {
        var sw = new Stopwatch();
        sw.Start();
        var nodes = DivideImpl(position, depth, 0, write);
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
            Move[] moves = new Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            var pseudoLegalMovesStaged = MoveGenerator.GenerateAllMovesStaged(position, moves);
            var enumerator = pseudoLegalMovesStaged.GetEnumerator();

            bool enumeratorHasNext = true;
            var lowerLimit = 0;
            while (enumeratorHasNext)
            {
                enumeratorHasNext = enumerator.MoveNext();
                var pseudoLegalMoves = moves[lowerLimit..enumerator.Current];
                lowerLimit = enumerator.Current;

                foreach (var move in pseudoLegalMoves)
                {
                    var state = position.MakeMoveFast(move, enumeratorHasNext);

                    if (position.WasProduceByAValidMove())
                    {
                        nodes = ResultsImpl(position, depth - 1, nodes);
                    }
                    position.UnmakeMoveFast(move, state);
                }
            }

            return nodes;
        }

        return nodes + 1;
    }

    private static long DivideImpl(Position position, int depth, long nodes, Action<string> write)
    {
        if (depth != 0)
        {
            Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            foreach (var move in MoveGenerator.GenerateAllMoves(position, moves))
            {
                var state = position.MakeMoveFast(move);

                if (position.WasProduceByAValidMove())
                {
                    var accumulatedNodes = nodes;
                    nodes = ResultsImpl(position, depth - 1, nodes);

                    write($"{move.UCIString()}\t\t{nodes - accumulatedNodes}");
                }

                position.UnmakeMoveFast(move, state);
            }

            write(string.Empty);

            return nodes;
        }

        return nodes + 1;
    }

    public static void PrintPerftResult(int depth, (long Nodes, double ElapsedMilliseconds) peftResult, Action<string> write)
    {
        var timeStr = TimeToString(peftResult.ElapsedMilliseconds);

        write(
            $"Depth:\t{depth}" + Environment.NewLine +
            $"Nodes:\t{peftResult.Nodes}" + Environment.NewLine +
            $"Time:\t{timeStr}" + Environment.NewLine +
            $"nps:\t{(Math.Round(peftResult.Nodes / peftResult.ElapsedMilliseconds)) / 1000} Mnps" + Environment.NewLine);
    }

    public static async ValueTask PrintPerftResult(int depth, (long Nodes, double ElapsedMilliseconds) peftResult, Func<string, ValueTask> write)
    {
        var timeStr = TimeToString(peftResult.ElapsedMilliseconds);

        await write(
            $"Depth:\t{depth}" + Environment.NewLine +
            $"Nodes:\t{peftResult.Nodes}" + Environment.NewLine +
            $"Time:\t{timeStr}" + Environment.NewLine +
            $"nps:\t{(Math.Round(peftResult.Nodes / peftResult.ElapsedMilliseconds)) / 1000} Mnps" + Environment.NewLine);
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
