/*
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using NLog;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class MoveGenerator_CapturesSeparated : BaseBenchmark
{
    private static Move[] _movePool = new Move[Constants.MaxNumberOfPossibleMovesInAPosition];

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
    public Move[] OldMoveGenerator_GenerateAll(string fen)
    {
        return OldMoveGenerator.GenerateAllMoves(new Position(fen), _movePool);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Move[] OldMoveGenerator_GenerateCaptures(string fen)
    {
        return OldMoveGenerator.GenerateAllMoves(new Position(fen), _movePool, capturesOnly: true);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Move[] NewMoveGenerator_GenerateAll(string fen)
    {
        return NewMoveGenerator.GenerateAllMoves(new Position(fen), _movePool);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public Move[] NewMoveGenerator_GenerateCaptures(string fen)
    {
        return NewMoveGenerator.GenerateAllCaptures(new Position(fen), _movePool);
    }
}

file static class OldMoveGenerator
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private const int TRUE = 1;

    /// <summary>
    /// Indexed by <see cref="Piece"/>.
    /// Checks are not considered
    /// </summary>
    private static readonly Func<int, BitBoard, BitBoard>[] _pieceAttacks =
    [
        (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White, origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin],

        (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black, origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin],
    ];

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="capturesOnly">Filters out all moves but captures</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move[] GenerateAllMoves(Position position, Move[]? movePool = null, bool capturesOnly = false)
    {
#if DEBUG
        if (position.Side == Side.Both)
        {
            return Array.Empty<Move>();
        }
#endif

        movePool ??= new Move[Constants.MaxNumberOfPossibleMovesInAPosition];
        int localIndex = 0;

        var offset = Utils.PieceOffset(position.Side);

        GeneratePawnMoves(ref localIndex, movePool, position, offset, capturesOnly);
        GenerateCastlingMoves(ref localIndex, movePool, position, offset);
        GeneratePieceMoves(ref localIndex, movePool, (int)Piece.K + offset, position, capturesOnly);
        GeneratePieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position, capturesOnly);
        GeneratePieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position, capturesOnly);
        GeneratePieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position, capturesOnly);
        GeneratePieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position, capturesOnly);

        return movePool[..localIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePawnMoves(ref int localIndex, Move[] movePool, Position position, int offset, bool capturesOnly = false)
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

            var sourceRank = (sourceSquare >> 3) + 1;

#if DEBUG
            if (sourceRank == 1 || sourceRank == 8)
            {
                _logger.Warn("There's a non-promoted {0} pawn in rank {1}", position.Side, sourceRank);
                continue;
            }
#endif

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
                }
                else if (!capturesOnly)
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
                }

                // Double pawn push
                // Inside of the if because singlePush square cannot be occupied either
                if (!capturesOnly)
                {
                    var doublePushSquare = sourceSquare + (2 * pawnPush);
                    if (!position.OccupancyBitBoards[2].GetBit(doublePushSquare)
                        && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White)))
                    {
                        movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE);
                    }
                }
            }

            var attacks = Attacks.PawnAttacks[(int)position.Side, sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE);
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                }
            }
        }
    }

    /// <summary>
    /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
    /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
    /// </summary>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateCastlingMoves(ref int localIndex, Move[] movePool, Position position, int offset)
    {
        var piece = (int)Piece.K + offset;
        var oppositeSide = (Side)Utils.OppositeSide(position.Side);

        int sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

        // Castles
        if (position.Castle != default)
        {
            if (position.Side == Side.White)
            {
                bool ise1Attacked = Attacks.IsSquareAttackedBySide((int)BoardSquare.e1, position, oppositeSide);
                if (((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.f1, position, oppositeSide)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.g1, position, oppositeSide))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE);
                }
            }
            else
            {
                bool ise8Attacked = Attacks.IsSquareAttackedBySide((int)BoardSquare.e8, position, oppositeSide);
                if (((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.f8, position, oppositeSide)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.g8, position, oppositeSide))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((position.Castle & (int)CastlingRights.BQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE);
                }
            }
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    /// <param name="position"></param>
    /// <param name="capturesOnly"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePieceMoves(ref int localIndex, Move[] movePool, int piece, Position position, bool capturesOnly = false)
    {
        var bitboard = position.PieceBitBoards[piece];
        int sourceSquare, targetSquare;

        while (bitboard != default)
        {
            sourceSquare = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                & ~position.OccupancyBitBoards[(int)position.Side];

            while (attacks != default)
            {
                targetSquare = attacks.GetLS1BIndex();
                attacks.ResetLS1B();

                if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                }
                else if (!capturesOnly)
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece);
                }
            }
        }
    }
}

file static class NewMoveGenerator
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private const int TRUE = 1;

    /// <summary>
    /// Indexed by <see cref="Piece"/>.
    /// Checks are not considered
    /// </summary>
    private static readonly Func<int, BitBoard, BitBoard>[] _pieceAttacks =
    [
        (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White, origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin],

        (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black, origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin],
    ];

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("Use override that accepts Move[] when possible")]
    public static Move[] GenerateAllMoves(Position position) =>
        GenerateAllMoves(position, new Move[Constants.MaxNumberOfPossibleMovesInAPosition]);

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="movePool">Filters out all moves but captures</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move[] GenerateAllMoves(Position position, Move[] movePool)
    {
#if DEBUG
        if (position.Side == Side.Both)
        {
            return [];
        }
#endif

        int localIndex = 0;

        var offset = Utils.PieceOffset(position.Side);

        GenerateAllPawnMoves(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position, offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.K + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position);

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("Use override that accepts Move[] when possible")]
    public static Move[] GenerateAllCaptures(Position position) =>
        GenerateAllCaptures(position, new Move[Constants.MaxNumberOfPossibleMovesInAPosition]);

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="movePool"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move[] GenerateAllCaptures(Position position, Move[] movePool)
    {
#if DEBUG
        if (position.Side == Side.Both)
        {
            return [];
        }
#endif

        int localIndex = 0;

        var offset = Utils.PieceOffset(position.Side);

        GeneratePawnCaptures(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position, offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.K + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset, position);

        return movePool[..localIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateAllPawnMoves(ref int localIndex, Move[] movePool, Position position, int offset)
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

            var sourceRank = (sourceSquare >> 3) + 1;

#if DEBUG
            if (sourceRank == 1 || sourceRank == 8)
            {
                _logger.Warn("There's a non-promoted {0} pawn in rank {1}", position.Side, sourceRank);
                continue;
            }
#endif

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
                }

                // Double pawn push
                // Inside of the if because singlePush square cannot be occupied either
                var doublePushSquare = sourceSquare + (2 * pawnPush);
                if (!position.OccupancyBitBoards[2].GetBit(doublePushSquare)
                    && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White)))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE);
                }
            }

            var attacks = Attacks.PawnAttacks[(int)position.Side, sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE);
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePawnCaptures(ref int localIndex, Move[] movePool, Position position, int offset)
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

            var sourceRank = (sourceSquare >> 3) + 1;

#if DEBUG
            if (sourceRank == 1 || sourceRank == 8)
            {
                _logger.Warn("There's a non-promoted {0} pawn in rank {1}", position.Side, sourceRank);
                continue;
            }
#endif

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
                }
            }

            var attacks = Attacks.PawnAttacks[(int)position.Side, sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE);
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                }
            }
        }
    }

    /// <summary>
    /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
    /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
    /// </summary>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateCastlingMoves(ref int localIndex, Move[] movePool, Position position, int offset)
    {
        var piece = (int)Piece.K + offset;
        var oppositeSide = (Side)Utils.OppositeSide(position.Side);

        int sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

        // Castles
        if (position.Castle != default)
        {
            if (position.Side == Side.White)
            {
                bool ise1Attacked = Attacks.IsSquareAttackedBySide((int)BoardSquare.e1, position, oppositeSide);
                if (((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.f1, position, oppositeSide)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.g1, position, oppositeSide))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE);
                }
            }
            else
            {
                bool ise8Attacked = Attacks.IsSquareAttackedBySide((int)BoardSquare.e8, position, oppositeSide);
                if (((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.f8, position, oppositeSide)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.g8, position, oppositeSide))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((position.Castle & (int)CastlingRights.BQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE);
                }
            }
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen moves
    /// </summary>
    /// <param name="movePool"></param>
    /// <param name="piece"><see cref="Piece"/></param>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateAllPieceMoves(ref int localIndex, Move[] movePool, int piece, Position position)
    {
        var bitboard = position.PieceBitBoards[piece];
        int sourceSquare, targetSquare;

        while (bitboard != default)
        {
            sourceSquare = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                & ~position.OccupancyBitBoards[(int)position.Side];

            while (attacks != default)
            {
                targetSquare = attacks.GetLS1BIndex();
                attacks.ResetLS1B();

                if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece);
                }
            }
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen capture moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePieceCaptures(ref int localIndex, Move[] movePool, int piece, Position position)
    {
        var bitboard = position.PieceBitBoards[piece];
        int sourceSquare, targetSquare;

        while (bitboard != default)
        {
            sourceSquare = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                & ~position.OccupancyBitBoards[(int)position.Side];

            while (attacks != default)
            {
                targetSquare = attacks.GetLS1BIndex();
                attacks.ResetLS1B();

                if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                }
            }
        }
    }
}
