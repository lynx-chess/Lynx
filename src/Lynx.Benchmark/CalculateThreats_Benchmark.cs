/*
 *  BenchmarkDotNet v0.15.3, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method    | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Original  | 7.147 us | 0.0225 us | 0.0176 us |  1.00 |    0.00 |      80 B |        1.00 |
 *  | Reference | 7.836 us | 0.1542 us | 0.2741 us |  1.10 |    0.04 |      80 B |        1.00 |
 *  | UnsafeAdd | 7.926 us | 0.0928 us | 0.0775 us |  1.11 |    0.01 |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.15.3, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method    | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | Original  | 9.364 us | 0.0378 us | 0.0315 us |  1.00 |      80 B |        1.00 |
 *  | Reference | 9.209 us | 0.0570 us | 0.0534 us |  0.98 |      80 B |        1.00 |
 *  | UnsafeAdd | 8.297 us | 0.0474 us | 0.0420 us |  0.89 |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.15.3, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), Arm64 RyuJIT armv8.0-a
 *
 *  | Method    | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | Original  | 7.970 us | 0.3532 us | 1.0414 us |  1.02 |    0.18 | 0.0076 |      80 B |        1.00 |
 *  | Reference | 8.546 us | 0.3173 us | 0.9154 us |  1.09 |    0.18 |      - |      80 B |        1.00 |
 *  | UnsafeAdd | 8.658 us | 0.2544 us | 0.7298 us |  1.10 |    0.17 |      - |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.15.3, macOS Ventura 13.7.6 (22H625) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method    | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
 *  | Original  | 19.18 us | 0.647 us | 1.896 us |  1.01 |    0.14 |      80 B |        1.00 |
 *  | Reference | 17.19 us | 0.559 us | 1.631 us |  0.90 |    0.12 |      80 B |        1.00 |
 *  | UnsafeAdd | 26.23 us | 0.776 us | 2.240 us |  1.38 |    0.18 |      80 B |        1.00 |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx.Benchmark;

public class CalculateThreats_Benchmark : BaseBenchmark
{
    private readonly Position_CalculateThreats_Benchmark[] _positions;

    public CalculateThreats_Benchmark()
    {
        _positions = [.. Engine._benchmarkFens.Select(fen => new Position_CalculateThreats_Benchmark(fen))];
    }

    [Benchmark(Baseline = true)]
    public int Original()
    {
        Span<BitBoard> attacks = stackalloc BitBoard[Enum.GetValues<Piece>().Length];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        foreach (var position in _positions)
        {
            position.CalculateThreats_Original(ref evaluationContext);
        }

        return attacks[0].CountBits() + attacksBySide[0].CountBits();
    }

    [Benchmark]
    public int Reference()
    {
        Span<BitBoard> attacks = stackalloc BitBoard[Enum.GetValues<Piece>().Length];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        foreach (var position in _positions)
        {

            position.CalculateThreats_Reference(ref evaluationContext);
        }

        return attacks[0].CountBits() + attacksBySide[0].CountBits();
    }

    [Benchmark]
    public int UnsafeAdd()
    {
        Span<BitBoard> attacks = stackalloc BitBoard[Enum.GetValues<Piece>().Length];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        foreach (var position in _positions)
        {
            position.CalculateThreats_UnsafeAdd(ref evaluationContext);
        }

        return attacks[0].CountBits() + attacksBySide[0].CountBits();
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CalculateThreats_UnsafeAdd(ref EvaluationContext evaluationContext)
    {
        var occupancy = _occupancyBitBoards[(int)Side.Both];

        ref var attacksRef = ref MemoryMarshal.GetReference(evaluationContext.Attacks);
        ref var attacksBySideRef = ref MemoryMarshal.GetReference(evaluationContext.AttacksBySide);

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref Unsafe.Add(ref attacksRef, pieceIndex);
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            Unsafe.Add(ref attacksBySideRef, (int)Side.White) |= existingAttacks;
        }

        for (int pieceIndex = (int)Piece.p; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var board = _pieceBitBoards[pieceIndex];
            var attacks = MoveGenerator._pieceAttacks[pieceIndex];

            ref var existingAttacks = ref Unsafe.Add(ref attacksRef, pieceIndex);
            while (board != 0)
            {
                board = board.WithoutLS1B(out var square);
                existingAttacks |= attacks(square, occupancy);
            }

            Unsafe.Add(ref attacksBySideRef, (int)Side.Black) |= existingAttacks;
        }
    }
}
