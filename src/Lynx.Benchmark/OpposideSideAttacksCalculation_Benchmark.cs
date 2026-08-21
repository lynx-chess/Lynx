using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class OpposideSideAttacksCalculation_Benchmark : BaseBenchmark
{
    public static Position[] Data => [.. Engine._benchmarkFens.Select(fen => new Position(fen))];

    [Benchmark(Baseline = true)]
    public int GenerateAllMoves_NoAttacks()
    {
        var result = 0;

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        foreach (var position in Data)
        {
            const Bitboard oppositeSideAttacks = 0UL;
            result += MoveGenerator.GenerateAllMoves(position, oppositeSideAttacks, movePool).Length;
        }

        return result;
    }

    [Benchmark]
    public int GenerateAllMoves_EvaluationContextCalculateThreats()
    {
        var result = 0;

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        foreach (var position in Data)
        {
            position.CalculateThreats(ref evaluationContext);
            var oppositeSideAttacks = evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];

            result += MoveGenerator.GenerateAllMoves(position, oppositeSideAttacks, movePool).Length;
        }

        return result;
    }

    [Benchmark]
    public int GenerateAllMoves_CalculateOnlyOppositeSideAttacks()
    {
        var result = 0;

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        foreach (var position in Data)
        {
            var oppositeSideAttacks = position.OppositeSideAttacks();
            result += MoveGenerator.GenerateAllMoves(position, oppositeSideAttacks, movePool).Length;
        }

        return result;
    }

    [Benchmark]
    public int GenerateAllCaptures_NoAttacks()
    {
        var result = 0;

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        foreach (var position in Data)
        {
            const Bitboard oppositeSideAttacks = 0UL;
            result += MoveGenerator.GenerateAllCaptures(position, oppositeSideAttacks, movePool).Length;
        }

        return result;
    }

    [Benchmark]
    public int GenerateAllCaptures_EvaluationContextCalculateThreats()
    {
        var result = 0;

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        foreach (var position in Data)
        {
            position.CalculateThreats(ref evaluationContext);
            var oppositeSideAttacks = evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];

            result += MoveGenerator.GenerateAllCaptures(position, oppositeSideAttacks, movePool).Length;
        }

        return result;
    }

    [Benchmark]
    public int GenerateAllCaptures_CalculateOnlyOppositeSideAttacks()
    {
        var result = 0;

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        foreach (var position in Data)
        {
            var oppositeSideAttacks = position.OppositeSideAttacks();
            result += MoveGenerator.GenerateAllCaptures(position, oppositeSideAttacks, movePool).Length;
        }

        return result;
    }

    [Benchmark]
    public int CanGenerateAtLeastAValidMove_NoAttacks()
    {
        var result = 0;

        foreach (var position in Data)
        {
            const Bitboard oppositeSideAttacks = 0UL;
            result += MoveGenerator.CanGenerateAtLeastAValidMove(position, oppositeSideAttacks) ? 1 : 0;
        }

        return result;
    }

    [Benchmark]
    public int CanGenerateAtLeastAValidMove_EvaluationContextCalculateThreats()
    {
        var result = 0;

        Span<Move> movePool = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        foreach (var position in Data)
        {
            position.CalculateThreats(ref evaluationContext);
            var oppositeSideAttacks = evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];

            result += MoveGenerator.CanGenerateAtLeastAValidMove(position, oppositeSideAttacks) ? 1 : 0;
        }

        return result;
    }

    [Benchmark]
    public int CanGenerateAtLeastAValidMove_CalculateOnlyOppositeSideAttacks()
    {
        var result = 0;

        foreach (var position in Data)
        {
            var oppositeSideAttacks = position.OppositeSideAttacks();
            result += MoveGenerator.CanGenerateAtLeastAValidMove(position, oppositeSideAttacks) ? 1 : 0;
        }

        return result;
    }

    // GenerateAllMoves
    // GenerateAllCaptures
    // IsAnyValidMove
}
