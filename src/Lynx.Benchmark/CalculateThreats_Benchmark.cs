/*
 *  BenchmarkDotNet v0.14.0, Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *
 *  | Method    | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | Original  | 8.832 us | 0.0445 us | 0.0416 us |  1.00 |      80 B |        1.00 |
 *  | Reference | 8.034 us | 0.0358 us | 0.0317 us |  0.91 |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), Arm64 RyuJIT AdvSIMD
 *
 *  | Method    | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | Original  | 7.661 us | 0.3157 us | 0.9058 us | 7.303 us |  1.01 |    0.16 |      - |      80 B |        1.00 |
 *  | Reference | 7.457 us | 0.3365 us | 0.9817 us | 7.208 us |  0.99 |    0.17 | 0.0076 |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.4946) (Hyper-V)
 *  Unknown processor
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *
 *  | Method    | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Original  | 7.227 us | 0.0269 us | 0.0239 us |  1.00 |    0.00 |      80 B |        1.00 |
 *  | Reference | 6.887 us | 0.1372 us | 0.2292 us |  0.95 |    0.03 |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.7.6 (22H625) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *
 *  | Method    | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------- |---------:|---------:|---------:|---------:|------:|--------:|----------:|------------:|
 *  | Original  | 19.30 us | 0.878 us | 2.520 us | 18.50 us |  1.02 |    0.18 |      80 B |        1.00 |
 *  | Reference | 20.60 us | 0.946 us | 2.668 us | 20.72 us |  1.08 |    0.19 |      80 B |        1.00 |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class CalculateThreats_Benchmark : BaseBenchmark
{
    private readonly Position_CalculateThreats_Benchmark[] _positions;

    public CalculateThreats_Benchmark()
    {
        _positions = [.. Engine._benchmarkFens.Select(fen => new Position_CalculateThreats_Benchmark(fen))];
    }

    [Benchmark(Baseline = true)]
    public void Original()
    {
        Span<BitBoard> attacks = stackalloc BitBoard[Enum.GetValues<Piece>().Length];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];

        foreach (var position in _positions)
        {
            var evaluationContext = new EvaluationContext(attacks, attacksBySide);

            position.CalculateThreats_Original(ref evaluationContext);
        }
    }

    [Benchmark]
    public void Reference()
    {
        Span<BitBoard> attacks = stackalloc BitBoard[Enum.GetValues<Piece>().Length];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];

        foreach (var position in _positions)
        {
            var evaluationContext = new EvaluationContext(attacks, attacksBySide);

            position.CalculateThreats_Reference(ref evaluationContext);
        }
    }
}

class Position_CalculateThreats_Benchmark
{
    private readonly ulong[] _pieceBitBoards;
    private readonly ulong[] _occupancyBitBoards;

    #pragma warning disable RCS1085 // Use auto-implemented property

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards => _pieceBitBoards;

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards => _occupancyBitBoards;

#pragma warning restore RCS1085 // Use auto-implemented property

    /// <summary>
    /// Beware, half move counter isn't take into account
    /// Use alternative constructor instead and set it externally if relevant
    /// </summary>
    public Position_CalculateThreats_Benchmark(string fen) : this(FENParser.ParseFEN(fen))
    {
    }

    public Position_CalculateThreats_Benchmark(ParseFENResult parsedFEN)
    {
        _pieceBitBoards = parsedFEN.PieceBitBoards;
        _occupancyBitBoards = parsedFEN.OccupancyBitBoards;
    }

    public EvaluationContext CalculateThreats_Original(ref EvaluationContext evaluationContext)
    {
        var occupancy = OccupancyBitBoards[(int)Side.Both];

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = PieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                evaluationContext.Attacks[pieceIndex] |= attacks(square, occupancy);
            }

            evaluationContext.AttacksBySide[(int)Side.White] |= evaluationContext.Attacks[pieceIndex];
        }

        for (int pieceIndex = (int)Piece.p; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var board = PieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                evaluationContext.Attacks[pieceIndex] |= attacks(square, occupancy);
            }

            evaluationContext.AttacksBySide[(int)Side.Black] |= evaluationContext.Attacks[pieceIndex];
        }

        return evaluationContext;
    }

    public EvaluationContext CalculateThreats_Reference(ref EvaluationContext evaluationContext)
    {
        var occupancy = _occupancyBitBoards[(int)Side.Both];
        ref var attacksByWhite = ref evaluationContext.AttacksBySide[(int)Side.White];
        ref var attacksByBlack = ref evaluationContext.AttacksBySide[(int)Side.Black];

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref evaluationContext.Attacks[pieceIndex];
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            attacksByWhite |= existingAttacks;
        }

        for (int pieceIndex = (int)Piece.p; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref evaluationContext.Attacks[pieceIndex];
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            attacksByBlack |= existingAttacks;
        }

        return evaluationContext;
    }
}
