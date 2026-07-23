/*
 *  BenchmarkDotNet v0.15.3, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method    | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Original  | 8.683 us | 0.1707 us | 0.2032 us |  1.00 |    0.03 |      80 B |        1.00 |
 *  | Reference | 8.049 us | 0.0385 us | 0.0360 us |  0.93 |    0.02 |      80 B |        1.00 |
 *  | UnsafeAdd | 8.266 us | 0.0413 us | 0.0366 us |  0.95 |    0.02 |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.15.3, Windows 11 (10.0.26100.4946/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method    | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | Original  | 8.636 us | 0.0974 us | 0.0911 us |  1.00 |      80 B |        1.00 |
 *  | Reference | 8.077 us | 0.0825 us | 0.0772 us |  0.94 |      80 B |        1.00 |
 *  | UnsafeAdd | 7.993 us | 0.0886 us | 0.0785 us |  0.93 |      80 B |        1.00 |
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
 *  | Original  | 6.047 us | 0.1202 us | 0.2613 us |  1.00 |    0.06 | 0.0076 |      80 B |        1.00 |
 *  | Reference | 5.721 us | 0.1087 us | 0.0908 us |  0.95 |    0.04 | 0.0076 |      80 B |        1.00 |
 *  | UnsafeAdd | 6.167 us | 0.1234 us | 0.3293 us |  1.02 |    0.07 | 0.0076 |      80 B |        1.00 |
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
 *  | Original  | 17.61 us | 0.342 us | 0.407 us |  1.00 |    0.03 |      80 B |        1.00 |
 *  | Reference | 17.40 us | 0.317 us | 0.297 us |  0.99 |    0.03 |      80 B |        1.00 |
 *  | UnsafeAdd | 17.04 us | 0.333 us | 0.397 us |  0.97 |    0.03 |      80 B |        1.00 |
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
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        foreach (var position in _positions)
        {
            position.CalculateThreats_Original(ref evaluationContext);
        }

        return buffer[0].CountBits() + buffer[EvaluationContext.RequiredBufferSize - 2].CountBits();
    }

    [Benchmark]
    public int Reference()
    {
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        foreach (var position in _positions)
        {
            position.CalculateThreats_Reference(ref evaluationContext);
        }

        return buffer[0].CountBits() + buffer[EvaluationContext.RequiredBufferSize - 2].CountBits();
    }

    [Benchmark]
    public int UnsafeAdd()
    {
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        foreach (var position in _positions)
        {
            position.CalculateThreats_UnsafeAdd(ref evaluationContext);
        }

        return buffer[0].CountBits() + buffer[EvaluationContext.RequiredBufferSize - 2].CountBits();
    }
}

class Position_CalculateThreats_Benchmark
{
    private readonly ulong[] _pieceBitboards;
    private readonly ulong[] _occupancyBitboards;

#pragma warning disable RCS1085 // Use auto-implemented property

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public Bitboard[] PieceBitboards => _pieceBitboards;

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public Bitboard[] OccupancyBitboards => _occupancyBitboards;

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
        _pieceBitboards = parsedFEN.PieceBitboards;
        _occupancyBitboards = parsedFEN.OccupancyBitboards;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public EvaluationContext CalculateThreats_Original(ref EvaluationContext evaluationContext)
    {
        var occupancy = OccupancyBitboards[(int)Side.Both];

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = PieceBitboards[pieceIndex];
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
            var board = PieceBitboards[pieceIndex];
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
        var occupancy = _occupancyBitboards[(int)Side.Both];
        ref var attacksByWhite = ref evaluationContext.AttacksBySide[(int)Side.White];
        ref var attacksByBlack = ref evaluationContext.AttacksBySide[(int)Side.Black];

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = _pieceBitboards[pieceIndex];
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
            var board = _pieceBitboards[pieceIndex];
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
        var occupancy = _occupancyBitboards[(int)Side.Both];

        ref var attacksRef = ref MemoryMarshal.GetReference(evaluationContext.Attacks);
        ref var attacksBySideRef = ref MemoryMarshal.GetReference(evaluationContext.AttacksBySide);

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.K; ++pieceIndex)
        {
            var board = _pieceBitboards[pieceIndex];
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
            var board = _pieceBitboards[pieceIndex];
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
