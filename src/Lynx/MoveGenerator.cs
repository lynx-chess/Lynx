using Lynx.Model;
using NLog;
using System.Runtime.CompilerServices;

namespace Lynx;

public static class MoveGenerator
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

    private const int TRUE = 1;

    private static int MoveIndex;
    private static Move[] MovePool { get; } = new Move[218];

    public static IEnumerable<Move> GeneratedMoves => MovePool.Take(MoveIndex);

    /// <summary>
    /// Indexed by <see cref="Piece"/>.
    /// Checks are not considered
    /// </summary>
    private static readonly Func<int, BitBoard, ulong>[] _pieceAttacks = new Func<int, BitBoard, ulong>[]
    {
            (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White, origin].Board,
            (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,
            (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,
            // TODO try to improve performance by re-using bishop and rook attacks
            (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
            (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,

            (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black, origin].Board,
            (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,
            (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,
            // TODO try to improve performance by re-using bishop and rook attacks
            (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
            (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,
    };

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="capturesOnly">Filters out all moves but captures</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Move> GenerateAllMoves(Position position, bool capturesOnly = false)
    {
#if DEBUG
        if (position.Side == Side.Both)
        {
            return new List<Move>();
        }
#endif
        var offset = Utils.PieceOffset(position.Side);

        MoveIndex = 0;
        GeneratePawnMoves(position, offset, capturesOnly);
        GenerateCastlingMoves(position, offset);
        GeneratePieceMoves((int)Piece.K + offset, position, capturesOnly);
        GeneratePieceMoves((int)Piece.N + offset, position, capturesOnly);
        GeneratePieceMoves((int)Piece.B + offset, position, capturesOnly);
        GeneratePieceMoves((int)Piece.R + offset, position, capturesOnly);
        GeneratePieceMoves((int)Piece.Q + offset, position, capturesOnly);

        return GeneratedMoves;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePawnMoves(Position position, int offset, bool capturesOnly = false)
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

#if DEBUG
            if (sourceRank == 1 || sourceRank == 8)
            {
                _logger.Warn($"There's a non-promoted {position.Side} pawn in rank {sourceRank}");
                continue;
            }
#endif

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare / 8) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
                }
                else if (!capturesOnly)
                {
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
                }

                // Double pawn push
                // Inside of the if because singlePush square cannot be occupied either
                if (!capturesOnly)
                {
                    var doublePushSquare = sourceSquare + (2 * pawnPush);
                    if (!position.OccupancyBitBoards[2].GetBit(doublePushSquare)
                        && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White)))
                    {
                        MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE);
                    }
                }
            }

            var attacks = Attacks.PawnAttacks[(int)position.Side, sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE);
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
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE);
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE);
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE);
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE);
                }
                else
                {
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
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
    internal static void GenerateCastlingMoves(Position position, int offset)
    {
        var piece = (int)Piece.K + offset;
        var oppositeSide = (Side)Utils.OppositeSide(position.Side);

        int sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

        // Castles
        if (position.Castle != default)
        {
            if (position.Side == Side.White)
            {
                bool ise1Attacked = Attacks.IsSquaredAttackedBySide((int)BoardSquare.e1, position, oppositeSide);
                if (((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f1, position, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g1, position, oppositeSide))
                {
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                {
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE);
                }
            }
            else
            {
                bool ise8Attacked = Attacks.IsSquaredAttackedBySide((int)BoardSquare.e8, position, oppositeSide);
                if (((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f8, position, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g8, position, oppositeSide))
                {
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((position.Castle & (int)CastlingRights.BQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                {
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE);
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
    internal static void GeneratePieceMoves(int piece, Position position, bool capturesOnly = false)
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
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                }
                else if (!capturesOnly)
                {
                    MovePool[MoveIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece);
                }
            }
        }
    }

    #region Only for reference, but unused

    /// <summary>
    /// Sames as <see cref="GeneratePawnMoves(Position, int, bool)"/> but returning them
    /// </summary>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    /// <param name="capturesOnly"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GeneratePawnMovesForReference(Position position, int offset, bool capturesOnly = false)
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

#if DEBUG
            if (sourceRank == 1 || sourceRank == 8)
            {
                _logger.Warn($"There's a non-promoted {position.Side} pawn in rank {sourceRank}");
                continue;
            }
#endif

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

    /// <summary>
    /// Same as <see cref="GenerateCastlingMoves(Position, int)"/> but returning them
    /// </summary>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateCastlingMovesForReference(Position position, int offset)
    {
        var piece = (int)Piece.K + offset;
        var oppositeSide = (Side)Utils.OppositeSide(position.Side);

        int sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

        // Castles
        if (position.Castle != default)
        {
            if (position.Side == Side.White)
            {
                bool ise1Attacked = Attacks.IsSquaredAttackedBySide((int)BoardSquare.e1, position, oppositeSide);
                if (((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f1, position, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g1, position, oppositeSide))
                {
                    yield return MoveExtensions.Encode(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                {
                    yield return MoveExtensions.Encode(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE);
                }
            }
            else
            {
                bool ise8Attacked = Attacks.IsSquaredAttackedBySide((int)BoardSquare.e8, position, oppositeSide);
                if (((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f8, position, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g8, position, oppositeSide))
                {
                    yield return MoveExtensions.Encode(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((position.Castle & (int)CastlingRights.BQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                {
                    yield return MoveExtensions.Encode(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateKingMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.K + offset, position, capturesOnly);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateKnightMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.N + offset, position, capturesOnly);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateBishopMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.B + offset, position, capturesOnly);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateRookMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.R + offset, position, capturesOnly);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GenerateQueenMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.Q + offset, position, capturesOnly);
    }

    /// <summary>
    /// Same as <see cref="GeneratePieceMoves(int, Position, bool)"/> but returning them
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    /// <param name="position"></param>
    /// <param name="capturesOnly"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IEnumerable<Move> GeneratePieceMovesForReference(int piece, Position position, bool capturesOnly = false)
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

    #endregion
}
