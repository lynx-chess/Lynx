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
        Span<BitBoard> attacks = stackalloc BitBoard[Enum.GetValues<Piece>().Length];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        var total = 0;

        foreach (var position in _positions)
        {
            attacks.Clear();
            attacksBySide.Clear();
            position.CalculateThreats(ref evaluationContext);

            total += position.Threats_Original(evaluationContext, (int)Side.White)
                - position.Threats_Original(evaluationContext, (int)Side.Black);
        }

        return total;
    }

    [Benchmark]
    public int Optimized()
    {
        Span<BitBoard> attacks = stackalloc BitBoard[Enum.GetValues<Piece>().Length];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        var total = 0;

        foreach (var position in _positions)
        {
            attacks.Clear();
            attacksBySide.Clear();
            position.CalculateThreats(ref evaluationContext);

            total += position.Threats_Optimized(evaluationContext, (int)Side.White)
                - position.Threats_Optimized(evaluationContext, (int)Side.Black);
        }

        return total;
    }
}

class Position_Threats_Benchmark
{
    private readonly ulong[] _pieceBitBoards;
    private readonly ulong[] _occupancyBitBoards;
    private readonly int[] _board;

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
        _pieceBitBoards = parsedFEN.PieceBitBoards;
        _occupancyBitBoards = parsedFEN.OccupancyBitBoards;
        _board = parsedFEN.Board;
    }

    public void CalculateThreats(ref EvaluationContext evaluationContext)
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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Threats_Original(EvaluationContext evaluationContext, int oppositeSide)
    {
        var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
        var oppositeSidePieces = _occupancyBitBoards[oppositeSide];
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
        var oppositeSidePieces = _occupancyBitBoards[oppositeSide];
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
