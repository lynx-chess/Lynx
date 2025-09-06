/*
 *
 *  |               Method |                  fen |      Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 |  Gen 1 | Allocated |
 *  |--------------------- |--------------------- |----------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|
 *  |         SingleThread | r2q1r(...)- 0 9 [68] |  8.650 us | 0.1549 us | 0.1293 us |  1.00 |    0.00 | 2.3346 |      - |      5 KB |
 *  |      ParallelForEach | r2q1r(...)- 0 9 [68] | 23.517 us | 0.2692 us | 0.2248 us |  2.72 |    0.04 | 1.2207 | 0.6104 |      8 KB |
 *  |              WhenAll | r2q1r(...)- 0 9 [68] | 16.825 us | 0.1360 us | 0.1272 us |  1.95 |    0.03 | 2.8076 |      - |      6 KB |
 *  |    SingleThreadArray | r2q1r(...)- 0 9 [68] | 10.622 us | 0.1570 us | 0.1468 us |  1.22 |    0.02 | 2.7771 |      - |      6 KB |
 *  | ParallelForEachArray | r2q1r(...)- 0 9 [68] | 28.377 us | 0.2868 us | 0.2682 us |  3.29 |    0.06 | 1.4954 | 0.7324 |      9 KB |
 *  |         WhenAllArray | r2q1r(...)- 0 9 [68] | 25.536 us | 0.0787 us | 0.0698 us |  2.95 |    0.04 | 3.7537 |      - |      8 KB |
 *  |                      |                      |           |           |           |       |         |        |        |           |
 *  |         SingleThread | r3k2r(...)- 0 1 [68] |  8.494 us | 0.1677 us | 0.1487 us |  1.00 |    0.00 | 2.2888 |      - |      5 KB |
 *  |      ParallelForEach | r3k2r(...)- 0 1 [68] | 23.596 us | 0.2795 us | 0.2615 us |  2.78 |    0.04 | 1.2207 | 0.6104 |      8 KB |
 *  |              WhenAll | r3k2r(...)- 0 1 [68] | 16.958 us | 0.1025 us | 0.0909 us |  2.00 |    0.04 | 2.7771 |      - |      6 KB |
 *  |    SingleThreadArray | r3k2r(...)- 0 1 [68] | 10.695 us | 0.1484 us | 0.1315 us |  1.26 |    0.03 | 2.7313 |      - |      6 KB |
 *  | ParallelForEachArray | r3k2r(...)- 0 1 [68] | 29.879 us | 0.2631 us | 0.2461 us |  3.52 |    0.07 | 1.4648 | 0.7324 |      9 KB |
 *  |         WhenAllArray | r3k2r(...)- 0 1 [68] | 26.096 us | 0.4990 us | 0.8608 us |  3.12 |    0.12 | 3.7231 |      - |      8 KB |
 *  |                      |                      |           |           |           |       |         |        |        |           |
 *  |         SingleThread | r3k2r(...)- 0 1 [68] | 10.186 us | 0.1554 us | 0.1378 us |  1.00 |    0.00 | 2.2888 |      - |      5 KB |
 *  |      ParallelForEach | r3k2r(...)- 0 1 [68] | 23.685 us | 0.1207 us | 0.1008 us |  2.33 |    0.04 | 1.2207 | 0.6104 |      8 KB |
 *  |              WhenAll | r3k2r(...)- 0 1 [68] | 16.570 us | 0.0919 us | 0.0860 us |  1.63 |    0.02 | 2.7771 |      - |      6 KB |
 *  |    SingleThreadArray | r3k2r(...)- 0 1 [68] | 12.884 us | 0.1372 us | 0.1283 us |  1.27 |    0.02 | 2.7618 |      - |      6 KB |
 *  | ParallelForEachArray | r3k2r(...)- 0 1 [68] | 30.797 us | 0.4458 us | 0.4170 us |  3.03 |    0.05 | 1.4648 | 0.7324 |      9 KB |
 *  |         WhenAllArray | r3k2r(...)- 0 1 [68] | 28.268 us | 0.1514 us | 0.1416 us |  2.77 |    0.04 | 3.7537 |      - |      8 KB |
 *  |                      |                      |           |           |           |       |         |        |        |           |
 *  |         SingleThread | rnbqk(...)6 0 1 [67] |  9.058 us | 0.1410 us | 0.1250 us |  1.00 |    0.00 | 2.2888 |      - |      5 KB |
 *  |      ParallelForEach | rnbqk(...)6 0 1 [67] | 23.341 us | 0.2234 us | 0.2090 us |  2.58 |    0.04 | 1.2512 | 0.6104 |      8 KB |
 *  |              WhenAll | rnbqk(...)6 0 1 [67] | 16.515 us | 0.1038 us | 0.0971 us |  1.82 |    0.03 | 2.7771 |      - |      6 KB |
 *  |    SingleThreadArray | rnbqk(...)6 0 1 [67] | 11.260 us | 0.1537 us | 0.1283 us |  1.24 |    0.02 | 2.7161 |      - |      6 KB |
 *  | ParallelForEachArray | rnbqk(...)6 0 1 [67] | 29.332 us | 0.3204 us | 0.2997 us |  3.24 |    0.05 | 1.4648 | 0.7324 |      9 KB |
 *  |         WhenAllArray | rnbqk(...)6 0 1 [67] | 26.143 us | 0.1039 us | 0.0972 us |  2.89 |    0.04 | 3.7231 |      - |      8 KB |
 *  |                      |                      |           |           |           |       |         |        |        |           |
 *  |         SingleThread | rnbqk(...)- 0 1 [56] |  7.046 us | 0.0691 us | 0.0647 us |  1.00 |    0.00 | 2.1439 |      - |      4 KB |
 *  |      ParallelForEach | rnbqk(...)- 0 1 [56] | 18.182 us | 0.1416 us | 0.1324 us |  2.58 |    0.03 | 1.0986 | 0.5493 |      7 KB |
 *  |              WhenAll | rnbqk(...)- 0 1 [56] | 14.317 us | 0.2094 us | 0.1959 us |  2.03 |    0.04 | 2.6245 |      - |      5 KB |
 *  |    SingleThreadArray | rnbqk(...)- 0 1 [56] |  7.986 us | 0.1455 us | 0.1361 us |  1.13 |    0.02 | 2.3956 |      - |      5 KB |
 *  | ParallelForEachArray | rnbqk(...)- 0 1 [56] | 20.664 us | 0.1411 us | 0.1251 us |  2.93 |    0.04 | 1.2512 | 0.6104 |      8 KB |
 *  |         WhenAllArray | rnbqk(...)- 0 1 [56] | 19.929 us | 0.0522 us | 0.0435 us |  2.83 |    0.03 | 3.2959 |      - |      7 KB |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Lynx.Benchmark;

public class MoveGeneratorParallel_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data =>
    [
        Constants.InitialPositionFEN,
        Constants.TrickyTestPositionFEN,
        Constants.TrickyTestPositionReversedFEN,
        Constants.CmkTestPositionFEN,
        Constants.KillerTestPositionFEN
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public IOrderedEnumerable<Move> SingleThread(string fen)
    {
        return CustomMoveGenerator.GenerateAllMoves_SingleThread(new Position(fen));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public IOrderedEnumerable<Move> ParallelForEach(string fen)
    {
        return CustomMoveGenerator.GenerateAllMoves_ParallelForEach(new Position(fen));
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public IOrderedEnumerable<Move> WhenAll(string fen)
    {
        return CustomMoveGenerator.GenerateAllMoves_WhenAll(new Position(fen)).Result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Move[] SingleThreadArray(string fen)
    {
        return [.. CustomMoveGenerator.GenerateAllMoves_SingleThread(new Position(fen))];
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Move[] ParallelForEachArray(string fen)
    {
        return [.. CustomMoveGenerator.GenerateAllMoves_ParallelForEach(new Position(fen))];
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Move[] WhenAllArray(string fen)
    {
        return [.. CustomMoveGenerator.GenerateAllMoves_WhenAll(new Position(fen)).Result];
    }
}
file static class CustomMoveGenerator
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<Move> GenerateAllMoves_SingleThread(Position position, int[,]? killerMoves = null, int? plies = null, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        var moves = new List<Move>(150);
        moves.AddRange(GeneratePawnMoves(position, offset, capturesOnly));
        moves.AddRange(GenerateCastlingMoves(position, offset));
        moves.AddRange(GeneratePieceMoves((int)Piece.K + offset, position, capturesOnly));
        moves.AddRange(GeneratePieceMoves((int)Piece.N + offset, position, capturesOnly));
        moves.AddRange(GeneratePieceMoves((int)Piece.B + offset, position, capturesOnly));
        moves.AddRange(GeneratePieceMoves((int)Piece.R + offset, position, capturesOnly));
        moves.AddRange(GeneratePieceMoves((int)Piece.Q + offset, position, capturesOnly));

        return moves.OrderByDescending(move => move.OldScore(position, killerMoves, plies));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<IOrderedEnumerable<Move>> GenerateAllMoves_WhenAll(Position position, int[,]? killerMoves = null, int? plies = null, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        var taskList = new List<Task<IEnumerable<Move>>>
                {
                    Task.Run(() => GeneratePawnMoves(position, offset, capturesOnly)),
                    Task.Run(() => GenerateCastlingMoves(position, offset)),
                    Task.Run(() => GeneratePieceMoves((int)Piece.K + offset, position, capturesOnly)),
                    Task.Run(() => GeneratePieceMoves((int)Piece.N + offset, position, capturesOnly)),
                    Task.Run(() => GeneratePieceMoves((int)Piece.B + offset, position, capturesOnly)),
                    Task.Run(() => GeneratePieceMoves((int)Piece.R + offset, position, capturesOnly)),
                    Task.Run(() => GeneratePieceMoves((int)Piece.Q + offset, position, capturesOnly)),
                };

        var result = await Task.WhenAll(taskList);

        return result.SelectMany(i => i).OrderByDescending(move => move.OldScore(in position, killerMoves, plies));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<Move> GenerateAllMoves_ParallelForEach(Position position, int[,]? killerMoves = null, int? plies = null, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        var concurrentBag = new ConcurrentBag<Move>();
        var actionList = new Func<int, Position, bool, IEnumerable<Move>>[]
        {
                    (offset, position, capturesOnly) => GeneratePawnMoves(position, offset, capturesOnly),
                    (offset, position, _) => GenerateCastlingMoves(position, offset),
                    (offset, position, capturesOnly) => GeneratePieceMoves((int)Piece.K + offset, position, capturesOnly),
                    (offset, position, capturesOnly) => GeneratePieceMoves((int)Piece.N + offset, position, capturesOnly),
                    (offset, position, capturesOnly) => GeneratePieceMoves((int)Piece.B + offset, position, capturesOnly),
                    (offset, position, capturesOnly) => GeneratePieceMoves((int)Piece.R + offset, position, capturesOnly),
                    (offset, position, capturesOnly) => GeneratePieceMoves((int)Piece.Q + offset, position, capturesOnly),
        };

        Parallel
            .ForEach(actionList, action =>
        {
            foreach (var move in action.Invoke(offset, position, capturesOnly))
            {
                concurrentBag.Add(move);
            }
        });

        return concurrentBag.OrderByDescending(move => move.OldScore(in position, killerMoves, plies));
    }

    #region Other stuff

    private static readonly Func<int, BitBoard, ulong>[] _pieceLocalAttacks =
    [
#pragma warning disable IDE0350 // Use implicitly typed lambda
        (int origin, BitBoard _) => LocalAttacks.PawnAttacks[(int)Side.White][origin],
        (int origin, BitBoard _) => LocalAttacks.KnightLocalAttacks[origin],
        LocalAttacks.BishopLocalAttacks,
        LocalAttacks.RookLocalAttacks,
        LocalAttacks.QueenLocalAttacks,
        (int origin, BitBoard _) => LocalAttacks.KingLocalAttacks[origin],

        (int origin, BitBoard _) => LocalAttacks.PawnAttacks[(int)Side.Black][origin],
        (int origin, BitBoard _) => LocalAttacks.KnightLocalAttacks[origin],
        LocalAttacks.BishopLocalAttacks,
        LocalAttacks.RookLocalAttacks,
        LocalAttacks.QueenLocalAttacks,
        (int origin, BitBoard _) => LocalAttacks.KingLocalAttacks[origin],
#pragma warning restore IDE0350 // Use implicitly typed lambda
    ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GeneratePawnMoves(Position position, int offset, bool capturesOnly = false)
    {
        int sourceSquare, targetSquare;

        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = position.PieceBitBoards[piece];

        while (bitboard != default)
        {
            sourceSquare = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            var sourceRank = (sourceSquare / 8) + 1;
            if (sourceRank == 1 || sourceRank == 8)
            {
                _logger.Warn("There's a non-promoted {0} pawn in rank {1}", position.Side, sourceRank);
                continue;
            }

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare / 8) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    yield return MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    yield return MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    yield return MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    yield return MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
                }
                else if (!capturesOnly)
                {
                    yield return MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
                }

                // Double pawn push
                // Inside of the if because singlePush square cannot be occupied either
                if (!capturesOnly)
                {
                    var doublePushSquare = sourceSquare + (2 * pawnPush);
                    if (!position.OccupancyBitBoards[2].GetBit(doublePushSquare)
                        && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White)))
                    {
                        yield return MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece);
                    }
                }
            }

            var attacks = LocalAttacks.PawnAttacks[(int)position.Side][sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                yield return MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece);
            }

            // Captures
            ulong attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();

                var targetRank = (targetSquare / 8) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    yield return MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    yield return MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset);
                    yield return MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset);
                    yield return MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset);
                }
                else
                {
                    yield return MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, position.Board[targetSquare]);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateCastlingMoves(Position position, int offset)
    {
        var piece = (int)Piece.K + offset;
        var oppositeSide = (Side)Utils.OppositeSide(position.Side);

        int sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

        // Castles
        if (position.Castle != default)
        {
            if (position.Side == Side.White)
            {
                if (((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.e1, position, oppositeSide)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.f1, position, oppositeSide)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.g1, position, oppositeSide))
                {
                    yield return MoveExtensions.EncodeShortCastle(sourceSquare, Constants.WhiteKingKingsideCastlingSquare, piece);
                }

                if (((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.e1, position, oppositeSide)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                {
                    yield return MoveExtensions.EncodeLongCastle(sourceSquare, Constants.WhiteKingQueensideCastlingSquare, piece);
                }
            }
            else
            {
                if (((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.e8, position, oppositeSide)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.f8, position, oppositeSide)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.g8, position, oppositeSide))
                {
                    yield return MoveExtensions.EncodeShortCastle(sourceSquare, Constants.BlackKingKingsideCastlingSquare, piece);
                }

                if (((position.Castle & (int)CastlingRights.BQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.e8, position, oppositeSide)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                    && !LocalAttacks.IsSquareAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                {
                    yield return MoveExtensions.EncodeLongCastle(sourceSquare, Constants.BlackKingQueensideCastlingSquare, piece);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GeneratePieceMoves(int piece, Position position, bool capturesOnly = false)
    {
        var bitboard = position.PieceBitBoards[piece];
        int sourceSquare, targetSquare;

        while (bitboard != default)
        {
            sourceSquare = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            ulong LocalAttacks = _pieceLocalAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                & ~position.OccupancyBitBoards[(int)position.Side];

            while (LocalAttacks != default)
            {
                targetSquare = LocalAttacks.GetLS1BIndex();
                LocalAttacks.ResetLS1B();

                if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                {
                    yield return MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, 1);
                }
                else if (!capturesOnly)
                {
                    yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateKingMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMoves((int)Piece.K + offset, position, capturesOnly);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateKnightMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMoves((int)Piece.N + offset, position, capturesOnly);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateBishopMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMoves((int)Piece.B + offset, position, capturesOnly);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateRookMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMoves((int)Piece.R + offset, position, capturesOnly);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateQueenMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMoves((int)Piece.Q + offset, position, capturesOnly);
    }

    #endregion
}

file static class LocalAttacks
{
    private static readonly BitBoard[] _bishopOccupancyMasks;
    private static readonly BitBoard[] _rookOccupancyMasks;

    /// <summary>
    /// [64 (Squares), 512 (Occupancies)]
    /// Use <see cref="BishopLocalAttacks(int, BitBoard)"/>
    /// </summary>
    private static readonly BitBoard[][] _bishopLocalAttacks;

    /// <summary>
    /// [64 (Squares), 4096 (Occupancies)]
    /// Use <see cref="RookLocalAttacks(int, BitBoard)"/>
    /// </summary>
    private static readonly BitBoard[][] _rookLocalAttacks;

    private static readonly ulong[] _pextLocalAttacks;
    private static readonly ulong[] _pextBishopOffset;
    private static readonly ulong[] _pextRookOffset;

    /// <summary>
    /// [2 (B|W), 64 (Squares)]
    /// </summary>
    public static BitBoard[][] PawnAttacks { get; }
    public static BitBoard[] KnightLocalAttacks { get; }
    public static BitBoard[] KingLocalAttacks { get; }

    static LocalAttacks()
    {
        KingLocalAttacks = AttackGenerator.InitializeKingAttacks();
        PawnAttacks = AttackGenerator.InitializePawnAttacks();
        KnightLocalAttacks = AttackGenerator.InitializeKnightAttacks();

        (_bishopOccupancyMasks, _bishopLocalAttacks) = AttackGenerator.InitializeBishopMagicAttacks();
        (_rookOccupancyMasks, _rookLocalAttacks) = AttackGenerator.InitializeRookMagicAttacks();

        if (Bmi2.X64.IsSupported)
        {
            _pextLocalAttacks = new ulong[5248 + 102400];
            _pextBishopOffset = new ulong[64];
            _pextRookOffset = new ulong[64];

            InitializeBishopAndRookPextLocalAttacks();
        }
        else
        {
            _pextLocalAttacks = [];
            _pextBishopOffset = [];
            _pextRookOffset = [];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard BishopLocalAttacks(int squareIndex, BitBoard occupancy)
    {
        return Bmi2.X64.IsSupported
            ? _pextLocalAttacks[_pextBishopOffset[squareIndex] + Bmi2.X64.ParallelBitExtract(occupancy, _bishopOccupancyMasks[squareIndex])]
            : MagicNumbersBishopLocalAttacks(squareIndex, occupancy);
    }

    /// <summary>
    /// Get Bishop LocalAttacks assuming current board occupancy
    /// </summary>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard MagicNumbersBishopLocalAttacks(int squareIndex, BitBoard occupancy)
    {
        var occ = occupancy & _bishopOccupancyMasks[squareIndex];
        occ *= Constants.BishopMagicNumbers[squareIndex];
        occ >>= (64 - Constants.BishopRelevantOccupancyBits[squareIndex]);

        return _bishopLocalAttacks[squareIndex][occ];
    }

    /// <summary>
    /// Get Rook LocalAttacks assuming current board occupancy
    /// </summary>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard RookLocalAttacks(int squareIndex, BitBoard occupancy)
    {
        return Bmi2.IsSupported
            ? _pextLocalAttacks[_pextRookOffset[squareIndex] + Bmi2.X64.ParallelBitExtract(occupancy, _rookOccupancyMasks[squareIndex])]
            : MagicNumbersRookLocalAttacks(squareIndex, occupancy);
    }

    public static BitBoard MagicNumbersRookLocalAttacks(int squareIndex, BitBoard occupancy)
    {
        var occ = occupancy & _rookOccupancyMasks[squareIndex];
        occ *= Constants.RookMagicNumbers[squareIndex];
        occ >>= (64 - Constants.RookRelevantOccupancyBits[squareIndex]);

        return _rookLocalAttacks[squareIndex][occ];
    }

    /// <summary>
    /// Get Queen LocalAttacks assuming current board occupancy
    /// Use <see cref="QueenLocalAttacks(BitBoard, BitBoard)"/> if rook and bishop LocalAttacks are already calculated
    /// </summary>
    /// <param name="occupancy">Occupancy of <see cref="Side.Both"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard QueenLocalAttacks(int squareIndex, BitBoard occupancy)
    {
        return QueenLocalAttacks(
            RookLocalAttacks(squareIndex, occupancy),
            BishopLocalAttacks(squareIndex, occupancy));
    }

    /// <summary>
    /// Get Queen LocalAttacks having rook and bishop LocalAttacks pre-calculated
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard QueenLocalAttacks(BitBoard rookLocalAttacks, BitBoard bishopLocalAttacks)
    {
        return rookLocalAttacks | bishopLocalAttacks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquareAttackedBySide(int squaredIndex, Position position, Side sideToMove) =>
        IsSquareAttacked(squaredIndex, sideToMove, position.PieceBitBoards, position.OccupancyBitBoards);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquareAttacked(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);
        var bothSidesOccupancy = occupancy[(int)Side.Both];

        // I tried to order them from most to least likely - not tested
        return
            IsSquareAttackedByPawns(squareIndex, sideToMoveInt, piecePosition[offset])
            || IsSquareAttackedByKing(squareIndex, piecePosition[(int)Piece.K + offset])
            || IsSquareAttackedByKnights(squareIndex, piecePosition[(int)Piece.N + offset])
            || IsSquareAttackedByBishops(squareIndex, piecePosition[(int)Piece.B + offset], bothSidesOccupancy, out var bishopLocalAttacks)
            || IsSquareAttackedByRooks(squareIndex, piecePosition[(int)Piece.R + offset], bothSidesOccupancy, out var rookLocalAttacks)
            || IsSquareAttackedByQueens(bishopLocalAttacks, rookLocalAttacks, piecePosition[(int)Piece.Q + offset]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSquareInCheck(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
    {
        Utils.Assert(sideToMove != Side.Both);

        var sideToMoveInt = (int)sideToMove;
        var offset = Utils.PieceOffset(sideToMoveInt);
        var bothSidesOccupancy = occupancy[(int)Side.Both];

        // I tried to order them from most to least likely- not tested
        return
            IsSquareAttackedByRooks(squareIndex, piecePosition[(int)Piece.R + offset], bothSidesOccupancy, out var rookLocalAttacks)
            || IsSquareAttackedByBishops(squareIndex, piecePosition[(int)Piece.B + offset], bothSidesOccupancy, out var bishopLocalAttacks)
            || IsSquareAttackedByQueens(bishopLocalAttacks, rookLocalAttacks, piecePosition[(int)Piece.Q + offset])
            || IsSquareAttackedByKnights(squareIndex, piecePosition[(int)Piece.N + offset])
            || IsSquareAttackedByPawns(squareIndex, sideToMoveInt, piecePosition[offset]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByPawns(int squareIndex, int sideToMove, BitBoard pawnBitBoard)
    {
        var oppositeColorIndex = sideToMove ^ 1;

        return (PawnAttacks[oppositeColorIndex][squareIndex] & pawnBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKnights(int squareIndex, BitBoard knightBitBoard)
    {
        return (KnightLocalAttacks[squareIndex] & knightBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByKing(int squareIndex, BitBoard kingBitBoard)
    {
        return (KingLocalAttacks[squareIndex] & kingBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByBishops(int squareIndex, BitBoard bishopBitBoard, BitBoard occupancy, out BitBoard bishopLocalAttacks)
    {
        bishopLocalAttacks = BishopLocalAttacks(squareIndex, occupancy);
        return (bishopLocalAttacks & bishopBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByRooks(int squareIndex, BitBoard rookBitBoard, BitBoard occupancy, out BitBoard rookLocalAttacks)
    {
        rookLocalAttacks = RookLocalAttacks(squareIndex, occupancy);
        return (rookLocalAttacks & rookBitBoard) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSquareAttackedByQueens(BitBoard bishopLocalAttacks, BitBoard rookLocalAttacks, BitBoard queenBitBoard)
    {
        var queenLocalAttacks = QueenLocalAttacks(rookLocalAttacks, bishopLocalAttacks);
        return (queenLocalAttacks & queenBitBoard) != default;
    }

    /// <summary>
    /// Taken from Leorik (https://github.com/lithander/Leorik/blob/master/Leorik.Core/Slider/Pext.cs)
    /// Based on https://www.chessprogramming.org/BMI2#PEXT_Bitboards
    /// </summary>
    private static void InitializeBishopAndRookPextLocalAttacks()
    {
        ulong index = 0;

        // Bishop-LocalAttacks
        for (int square = 0; square < 64; square++)
        {
            _pextBishopOffset[square] = index;
            ulong bishopMask = _bishopOccupancyMasks[square];

            ulong patterns = 1UL << BitOperations.PopCount(bishopMask);

            for (ulong i = 0; i < patterns; i++)
            {
                ulong occupation = Bmi2.X64.ParallelBitDeposit(i, bishopMask);
                _pextLocalAttacks[index++] = MagicNumbersBishopLocalAttacks(square, occupation);
            }
        }

        // Rook-LocalAttacks
        for (int square = 0; square < 64; square++)
        {
            _pextRookOffset[square] = index;
            ulong rookMask = _rookOccupancyMasks[square];
            ulong patterns = 1UL << BitOperations.PopCount(rookMask);

            for (ulong i = 0; i < patterns; i++)
            {
                ulong occupation = Bmi2.X64.ParallelBitDeposit(i, rookMask);
                _pextLocalAttacks[index++] = MagicNumbersRookLocalAttacks(square, occupation);
            }
        }
    }
}
