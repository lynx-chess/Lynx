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

    [Params(1, 10, 100, 1_000, 10_000)]
    public int Size { get; set; }

    private static readonly Position[] Positions =
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
        new Position(Constants.TTPositionFEN)
    ];

    [Benchmark(Baseline = true)]
    public long Span()
    {
        long result = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in Positions)
            {
                result += Span(position);
            }
        }

        return result;

        static int Span(Position position)
        {
            Span<Move> moveSpan = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            var allMoves = MoveGenerator.GenerateAllMoves(position, moveSpan);

            return allMoves.Length;
        }
    }

    [Benchmark]
    public long Array()
    {
        long result = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in Positions)
            {
                result += Array(position);
            }
        }

        return result;

        int Array(Position position)
        {
            var allMoves = MoveGenerator.GenerateAllMoves(position, _arrayMovePool);

            return allMoves.Length;
        }
    }

    [Benchmark]
    public long InlineArray()
    {
        long result = 0;

        for (int i = 0; i < Size; ++i)
        {
            foreach (var position in Positions)
            {
                result += InlineArray(position);
            }
        }

        return result;

        static int InlineArray(Position position)
        {
            var moveArray = new MoveArray();
            var allMoves = MoveGenerator.GenerateAllMoves(position, ref moveArray);

            return allMoves.Length;
        }
    }
}
