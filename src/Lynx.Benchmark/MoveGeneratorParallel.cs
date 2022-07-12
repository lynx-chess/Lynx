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
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class MoveGeneratorParallel : BaseBenchmark
{
    public static IEnumerable<string> Data => new[]
    {
            Constants.InitialPositionFEN,
            Constants.TrickyTestPositionFEN,
            Constants.TrickyTestPositionReversedFEN,
            Constants.CmkTestPositionFEN,
            Constants.KillerTestPositionFEN
        };

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
        return CustomMoveGenerator.GenerateAllMoves_SingleThread(new Position(fen)).ToArray();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Move[] ParallelForEachArray(string fen)
    {
        return CustomMoveGenerator.GenerateAllMoves_ParallelForEach(new Position(fen)).ToArray();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Move[] WhenAllArray(string fen)
    {
        return CustomMoveGenerator.GenerateAllMoves_WhenAll(new Position(fen)).Result.ToArray();
    }

    public static class CustomMoveGenerator
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private const int TRUE = 1;

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

            return moves.OrderByDescending(move => move.Score(position, killerMoves, plies));
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

            return result.SelectMany(i => i).OrderByDescending(move => move.Score(position, killerMoves, plies));
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

            return concurrentBag.OrderByDescending(move => move.Score(position, killerMoves, plies));
        }

        #region Other stuff

        private static readonly Func<int, BitBoard, ulong>[] _pieceAttacks = new Func<int, BitBoard, ulong>[]
        {
            (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White, origin].Board,
            (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,
            (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
            (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,

            (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black, origin].Board,
            (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,
            (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
            (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IEnumerable<Move> GeneratePawnMoves(Position position, int offset, bool capturesOnly = false)
        {
            int sourceSquare, targetSquare;

            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = position.PieceBitBoards[piece].Board;

            while (bitboard != default)
            {
                sourceSquare = BitBoard.GetLS1BIndex(bitboard);
                bitboard = BitBoard.ResetLS1B(bitboard);

                var sourceRank = (sourceSquare / 8) + 1;
                if (sourceRank == 1 || sourceRank == 8)
                {
                    _logger.Warn($"There's a non-promoted {position.Side} pawn in rank {sourceRank}");
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
                        yield return MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                        yield return MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                        yield return MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                        yield return MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
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
                            yield return MoveExtensions.Encode(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE);
                        }
                    }
                }

                var attacks = Attacks.PawnAttacks[(int)position.Side, sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                {
                    yield return MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE);
                }

                // Captures
                ulong attackedSquares = attacks.Board & position.OccupancyBitBoards[oppositeSide].Board;
                while (attackedSquares != default)
                {
                    targetSquare = BitBoard.GetLS1BIndex(attackedSquares);
                    attackedSquares = BitBoard.ResetLS1B(attackedSquares);

                    var targetRank = (targetSquare / 8) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                    {
                        yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE);
                        yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE);
                        yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE);
                        yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE);
                    }
                    else
                    {
                        yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
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
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.e1, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f1, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g1, position, oppositeSide))
                    {
                        yield return MoveExtensions.Encode(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE);
                    }

                    if (((position.Castle & (int)CastlingRights.WQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.e1, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                    {
                        yield return MoveExtensions.Encode(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE);
                    }
                }
                else
                {
                    if (((position.Castle & (int)CastlingRights.BK) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.e8, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f8, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g8, position, oppositeSide))
                    {
                        yield return MoveExtensions.Encode(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE);
                    }

                    if (((position.Castle & (int)CastlingRights.BQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.e8, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                    {
                        yield return MoveExtensions.Encode(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IEnumerable<Move> GeneratePieceMoves(int piece, Position position, bool capturesOnly = false)
        {
            var bitboard = position.PieceBitBoards[piece].Board;
            int sourceSquare, targetSquare;

            while (bitboard != default)
            {
                sourceSquare = BitBoard.GetLS1BIndex(bitboard);
                bitboard = BitBoard.ResetLS1B(bitboard);

                ulong attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                    & ~position.OccupancyBitBoards[(int)position.Side].Board;

                while (attacks != default)
                {
                    targetSquare = BitBoard.GetLS1BIndex(attacks);
                    attacks = BitBoard.ResetLS1B(attacks);

                    if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                    {
                        yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
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
}
