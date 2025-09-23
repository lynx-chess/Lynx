using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class MoveGenerator_SpanUnsafeAdd_Benchmark : BaseBenchmark
{
    private readonly Position[] _positions;

    public MoveGenerator_SpanUnsafeAdd_Benchmark()
    {
        _positions = [.. Engine._benchmarkFens.Select(fen => new Position(fen))];
    }

    [Benchmark(Baseline = true)]
    public int GenerateAllMoves()
    {
        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        var total = 0;
        foreach (var position in _positions)
        {
            Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
            var moves = MoveGenerator.GenerateAllMoves(position, ref evaluationContext, movePool);

            total += moves.Length;
        }

        return total;
    }
}
