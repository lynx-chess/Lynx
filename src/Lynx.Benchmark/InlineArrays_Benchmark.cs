/*
 *
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class InlineArrays_Benchmark : BaseBenchmark
{
    private readonly Move[] _arrayMovePool = new Move[Constants.MaxNumberOfPossibleMovesInAPosition];

    public static IEnumerable<string> Data => new[] {
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        Constants.TrickyTestPositionReversedFEN,
        Constants.CmkTestPositionFEN,
        Constants.ComplexPositionFEN,
        Constants.KillerTestPositionFEN,
        Constants.TTPositionFEN
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int Span(string fen)
    {
        Span<Move> moveSpan = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        var allMoves = MoveGenerator.GenerateAllMoves(new Position(fen), moveSpan);

        return allMoves.Length;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int Array(string fen)
    {
        var allMoves = MoveGenerator.GenerateAllMoves(new Position(fen), _arrayMovePool);

        return allMoves.Length;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int InlineArray(string fen)
    {
        var moveArray = new MoveArray();
        var allMoves = MoveGenerator.GenerateAllMoves(new Position(fen), ref moveArray);

        return allMoves.Length;
    }
}
