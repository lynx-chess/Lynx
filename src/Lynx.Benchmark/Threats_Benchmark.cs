/*
 *  BenchmarkDotNet v0.14.0, Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *
 *  | Method    | Mean      | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |---------- |----------:|----------:|----------:|------:|----------:|------------:|
 *  | Original  |  9.741 us | 0.0314 us | 0.0294 us |  1.00 |      80 B |        1.00 |
 *  | Optimized | 10.074 us | 0.0362 us | 0.0321 us |  1.03 |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.4946) (Hyper-V)
 *  Unknown processor
 *  .NET SDK 9.0.305
 *
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *  | Method    | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Original  | 8.737 us | 0.1108 us | 0.0925 us |  1.00 |    0.01 |      80 B |        1.00 |
 *  | Optimized | 9.147 us | 0.1755 us | 0.2219 us |  1.05 |    0.03 |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Sequoia 15.6.1 (24G90) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), Arm64 RyuJIT AdvSIMD
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), Arm64 RyuJIT AdvSIMD
 *  | Method    | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 *  | Original  | 8.735 us | 0.2465 us | 0.7190 us |  1.01 |    0.12 |      80 B |        1.00 |
 *  | Optimized | 9.125 us | 0.2991 us | 0.8631 us |  1.05 |    0.13 |      80 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.14.0, macOS Ventura 13.7.6 (22H625) [Darwin 22.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *    DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
 *
 *  | Method    | Mean     | Error    | StdDev   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |---------- |---------:|---------:|---------:|------:|--------:|----------:|------------:|
 *  | Original  | 17.90 us | 0.434 us | 1.230 us |  1.00 |    0.10 |      80 B |        1.00 |
 *  | Optimized | 18.65 us | 0.571 us | 1.600 us |  1.05 |    0.11 |      80 B |        1.00 |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

using static Lynx.EvaluationParams;

namespace Lynx.Benchmark;

public class Threats_Benchmark : BaseBenchmark
{
    private readonly Position_Threats_Benchmark[] _positions;

    public Threats_Benchmark()
    {
        _positions = [.. Engine._benchmarkFens.Select(fen => new Position_Threats_Benchmark(fen))];
    }

    [Benchmark(Baseline = true)]
    public int Original()
    {
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        var total = 0;

        foreach (var position in _positions)
        {
            evaluationContext.Reset();
            position.CalculateThreats(ref evaluationContext);

            total += position.Threats_Original(evaluationContext, (int)Side.White)
                - position.Threats_Original(evaluationContext, (int)Side.Black);
        }

        return total;
    }

    [Benchmark]
    public int Optimized()
    {
        Span<Bitboard> buffer = stackalloc Bitboard[EvaluationContext.RequiredBufferSize];
        var evaluationContext = new EvaluationContext(buffer);

        var total = 0;

        foreach (var position in _positions)
        {
            evaluationContext.Reset();
            position.CalculateThreats(ref evaluationContext);

            total += position.Threats_Optimized(evaluationContext, (int)Side.White)
                - position.Threats_Optimized(evaluationContext, (int)Side.Black);
        }

        return total;
    }
}

class Position_Threats_Benchmark
{
    private readonly ulong[] _pieceBitboards;
    private readonly ulong[] _occupancyBitboards;
    private readonly int[] _board;

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

    private static readonly int[][] _defendedThreatsBonus =
    [
        [],
        KnightThreatsBonus_Defended,
        BishopThreatsBonus_Defended,
        RookThreatsBonus_Defended,
        QueenThreatsBonus_Defended,
        KingThreatsBonus_Defended
    ];

    private static readonly int[][] _undefendedThreatsBonus =
    [
        [],
        KnightThreatsBonus,
        BishopThreatsBonus,
        RookThreatsBonus,
        QueenThreatsBonus,
        KingThreatsBonus
    ];

    /// <summary>
    /// Beware, half move counter isn't take into account
    /// Use alternative constructor instead and set it externally if relevant
    /// </summary>
    public Position_Threats_Benchmark(string fen) : this(FENParser.ParseFEN(fen))
    {
    }

    public Position_Threats_Benchmark(ParseFENResult parsedFEN)
    {
        _pieceBitboards = parsedFEN.PieceBitboards;
        _occupancyBitboards = parsedFEN.OccupancyBitboards;
        _board = parsedFEN.Board;
    }

    public void CalculateThreats(ref EvaluationContext evaluationContext)
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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Threats_Original(EvaluationContext evaluationContext, int oppositeSide)
    {
        var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
        var oppositeSidePieces = _occupancyBitboards[oppositeSide];
        int packedBonus = 0;

        var attacks = evaluationContext.Attacks;
        var board = _board;
        var defendedThreatsBonus = _defendedThreatsBonus;
        var undefendedThreatsBonus = _undefendedThreatsBonus;

        var defendedSquares = attacks[(int)Piece.P + oppositeSideOffset];

        for (int i = (int)Piece.N; i <= (int)Piece.K; ++i)
        {
            var threats = attacks[6 + i - oppositeSideOffset] & oppositeSidePieces;

            var defended = threats & defendedSquares;
            while (defended != 0)
            {
                defended = defended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                packedBonus += defendedThreatsBonus[i][attackedPiece - oppositeSideOffset];
            }

            var undefended = threats & ~defendedSquares;
            while (undefended != 0)
            {
                undefended = undefended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                packedBonus += undefendedThreatsBonus[i][attackedPiece - oppositeSideOffset];
            }
        }

        return packedBonus;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Threats_Optimized(EvaluationContext evaluationContext, int oppositeSide)
    {
        var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
        var oppositeSidePieces = _occupancyBitboards[oppositeSide];
        int packedBonus = 0;

        var attacks = evaluationContext.Attacks;
        var board = _board;
        var defendedThreatsBonus = _defendedThreatsBonus;
        var undefendedThreatsBonus = _undefendedThreatsBonus;

        var defendedSquares = attacks[(int)Piece.P + oppositeSideOffset];

        for (int i = (int)Piece.N; i <= (int)Piece.K; ++i)
        {
            var threats = attacks[6 + i - oppositeSideOffset] & oppositeSidePieces;

            var defended = threats & defendedSquares;
            var undefended = threats & ~defendedSquares;

            var thisDefendedThreatsBonus = defendedThreatsBonus[i];
            var thisUndefendedThreatsBonus = undefendedThreatsBonus[i];

            while (defended != 0)
            {
                defended = defended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                packedBonus += thisDefendedThreatsBonus[attackedPiece - oppositeSideOffset];
            }

            while (undefended != 0)
            {
                undefended = undefended.WithoutLS1B(out var square);
                var attackedPiece = board[square];

                packedBonus += thisUndefendedThreatsBonus[attackedPiece - oppositeSideOffset];
            }
        }

        return packedBonus;
    }
}
