/*
 *
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.4 LTS (Noble Numbat)
 *  AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.400
 *    [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
 *  
 *  | Method                                                         | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------------- |---------:|---------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | GenerateAllMoves_NoAttacks                                     | 88.88 us | 1.338 us | 1.251 us |  1.00 |    0.02 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | GenerateAllMoves_EvaluationContextCalculateThreats             | 98.34 us | 1.488 us | 1.392 us |  1.11 |    0.02 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | GenerateAllMoves_CalculateOnlyOppositeSideAttacks              | 94.60 us | 1.825 us | 1.707 us |  1.06 |    0.02 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  
 *  | GenerateAllCaptures_NoAttacks                                  | 77.84 us | 1.352 us | 1.557 us |  0.88 |    0.02 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | GenerateAllCaptures_EvaluationContextCalculateThreats          | 96.79 us | 1.165 us | 1.089 us |  1.09 |    0.02 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | GenerateAllCaptures_CalculateOnlyOppositeSideAttacks           | 87.79 us | 1.474 us | 1.307 us |  0.99 |    0.02 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  
 *  | CanGenerateAtLeastAValidMove_NoAttacks                         | 77.55 us | 1.013 us | 0.791 us |  0.87 |    0.01 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | CanGenerateAtLeastAValidMove_EvaluationContextCalculateThreats | 93.54 us | 1.580 us | 1.478 us |  1.05 |    0.02 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | CanGenerateAtLeastAValidMove_CalculateOnlyOppositeSideAttacks  | 86.24 us | 1.719 us | 1.911 us |  0.97 |    0.02 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.33296/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.400
 *    [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
 *  
 *  | Method                                                         | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------------- |---------:|---------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
 *  | GenerateAllMoves_NoAttacks                                     | 81.44 us | 0.472 us | 0.442 us |  1.00 |    0.01 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | GenerateAllMoves_EvaluationContextCalculateThreats             | 86.17 us | 0.767 us | 0.680 us |  1.06 |    0.01 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | GenerateAllMoves_CalculateOnlyOppositeSideAttacks              | 83.74 us | 0.867 us | 0.811 us |  1.03 |    0.01 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  
 *  | GenerateAllCaptures_NoAttacks                                  | 71.03 us | 0.588 us | 0.550 us |  0.87 |    0.01 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | GenerateAllCaptures_EvaluationContextCalculateThreats          | 84.96 us | 0.668 us | 0.625 us |  1.04 |    0.01 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | GenerateAllCaptures_CalculateOnlyOppositeSideAttacks           | 75.67 us | 1.331 us | 1.245 us |  0.93 |    0.02 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  
 *  | CanGenerateAtLeastAValidMove_NoAttacks                         | 71.89 us | 0.543 us | 0.454 us |  0.88 |    0.01 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | CanGenerateAtLeastAValidMove_EvaluationContextCalculateThreats | 87.61 us | 1.011 us | 0.946 us |  1.08 |    0.01 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  | CanGenerateAtLeastAValidMove_CalculateOnlyOppositeSideAttacks  | 73.27 us | 0.945 us | 0.884 us |  0.90 |    0.01 | 13.3057 | 3.6621 | 217.62 KB |        1.00 |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.7 (24G720) [Darwin 24.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 10.0.400
 *    [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
 *  
 *  | Method                                                         | Mean     | Error   | StdDev   | Median   | Ratio | RatioSD | Gen0    | Gen1    | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------------- |---------:|--------:|---------:|---------:|------:|--------:|--------:|--------:|----------:|------------:|
 *  | GenerateAllMoves_NoAttacks                                     | 143.8 us | 3.70 us | 10.37 us | 140.7 us |  1.00 |    0.10 | 35.4004 | 10.0098 | 217.62 KB |        1.00 |
 *  | GenerateAllMoves_EvaluationContextCalculateThreats             | 149.5 us | 2.88 us |  6.90 us | 147.7 us |  1.04 |    0.08 | 35.4004 | 10.0098 | 217.62 KB |        1.00 |
 *  | GenerateAllMoves_CalculateOnlyOppositeSideAttacks              | 139.6 us | 2.39 us |  3.86 us | 138.7 us |  0.98 |    0.07 | 35.4004 | 10.0098 | 217.62 KB |        1.00 |
 *  
 *  | GenerateAllCaptures_NoAttacks                                  | 117.8 us | 2.15 us |  1.91 us | 117.6 us |  0.82 |    0.06 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  | GenerateAllCaptures_EvaluationContextCalculateThreats          | 138.7 us | 2.14 us |  1.79 us | 138.0 us |  0.97 |    0.07 | 35.4004 | 10.0098 | 217.62 KB |        1.00 |
 *  | GenerateAllCaptures_CalculateOnlyOppositeSideAttacks           | 131.2 us | 2.48 us |  2.54 us | 130.3 us |  0.92 |    0.06 | 35.4004 | 10.0098 | 217.62 KB |        1.00 |
 *  
 *  | CanGenerateAtLeastAValidMove_NoAttacks                         | 121.0 us | 2.42 us |  5.40 us | 120.3 us |  0.85 |    0.07 | 35.4004 | 10.0098 | 217.62 KB |        1.00 |
 *  | CanGenerateAtLeastAValidMove_EvaluationContextCalculateThreats | 149.3 us | 2.58 us |  5.22 us | 148.1 us |  1.04 |    0.08 | 35.4004 | 10.0098 | 217.62 KB |        1.00 |
 *  | CanGenerateAtLeastAValidMove_CalculateOnlyOppositeSideAttacks  | 132.5 us | 2.61 us |  4.29 us | 131.6 us |  0.93 |    0.07 | 35.4004 | 10.0098 | 217.62 KB |        1.00 |
 *  
 *  
 *  BenchmarkDotNet v0.15.8, macOS Tahoe 26.5.2 (25F84) [Darwin 25.5.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.400
 *    [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a
 *  
 *  | Method                                                         | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0    | Gen1    | Allocated | Alloc Ratio |
 *  |--------------------------------------------------------------- |---------:|---------:|---------:|------:|--------:|--------:|--------:|----------:|------------:|
 *  | GenerateAllMoves_NoAttacks                                     | 57.57 us | 1.406 us | 4.056 us |  1.00 |    0.10 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  | GenerateAllMoves_EvaluationContextCalculateThreats             | 63.03 us | 1.996 us | 5.726 us |  1.10 |    0.12 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  | GenerateAllMoves_CalculateOnlyOppositeSideAttacks              | 57.81 us | 1.355 us | 3.886 us |  1.01 |    0.10 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  
 *  | GenerateAllCaptures_NoAttacks                                  | 52.63 us | 1.256 us | 3.683 us |  0.92 |    0.09 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  | GenerateAllCaptures_EvaluationContextCalculateThreats          | 59.36 us | 1.436 us | 4.143 us |  1.04 |    0.10 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  | GenerateAllCaptures_CalculateOnlyOppositeSideAttacks           | 55.16 us | 1.448 us | 4.270 us |  0.96 |    0.10 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  
 *  | CanGenerateAtLeastAValidMove_NoAttacks                         | 53.55 us | 1.235 us | 3.641 us |  0.93 |    0.09 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  | CanGenerateAtLeastAValidMove_EvaluationContextCalculateThreats | 62.25 us | 1.592 us | 4.491 us |  1.09 |    0.11 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  | CanGenerateAtLeastAValidMove_CalculateOnlyOppositeSideAttacks  | 57.27 us | 1.237 us | 3.528 us |  1.00 |    0.09 | 35.5225 | 10.0098 | 217.62 KB |        1.00 |
 *  
 * 
 */

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
            result += MoveGenerator.GenerateAllMoves(position, movePool).Length;
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

            result += MoveGenerator.GenerateAllMoves(position, movePool, oppositeSideAttacks).Length;
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
            result += MoveGenerator.GenerateAllMoves(position, movePool, oppositeSideAttacks).Length;
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
            result += MoveGenerator.GenerateAllCaptures(position, movePool, oppositeSideAttacks).Length;
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

            result += MoveGenerator.GenerateAllCaptures(position, movePool, oppositeSideAttacks).Length;
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
            result += MoveGenerator.GenerateAllCaptures(position, movePool, oppositeSideAttacks).Length;
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
