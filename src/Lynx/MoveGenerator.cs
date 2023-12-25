using Lynx.Model;
using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx;

public static class MoveGenerator
{
#if DEBUG
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
#endif

    private const int TRUE = 1;

    public const int WhiteShortCastle = 8780736;
    public const int WhiteLongCastle = 17165248;
    public const int BlackShortCastle = 9115712;
    public const int BlackLongCastle = 17500224;

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
    [Obsolete("dev and test only")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move[] GenerateAllMoves(Position position, bool capturesOnly = false) =>
        GenerateAllMoves(position, new Move[Constants.MaxNumberOfPossibleMovesInAPosition], capturesOnly);

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="movePool"></param>
    /// <param name="capturesOnly">Filters out all moves but captures</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move[] GenerateAllMoves(Position position, Move[] movePool, bool capturesOnly = false)
    {
#if DEBUG
        if (position.Side == Side.Both)
        {
            return [];
        }
#endif

        int localIndex = 0;

        var offset = Utils.PieceOffset(position.Side);

        GeneratePawnMoves(ref localIndex, movePool, position, offset, capturesOnly);
        GenerateCastlingMoves(ref localIndex, movePool, position);
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
    /// <param name="movePool"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateCastlingMoves(ref int localIndex, Move[] movePool, Position position)
    {
        if (position.Castle != default)
        {
            if (position.Side == Side.White)
            {
                bool ise1Attacked = Attacks.IsSquareAttackedBySide(Constants.WhiteKingSourceSquare, position, Side.Black);

                if (((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.f1, position, Side.Black)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.g1, position, Side.Black))
                {
                    movePool[localIndex++] = WhiteShortCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.Encode(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(position.Side), isShortCastle: TRUE),
                        "Wrong hardcoded white short castle move");
                }

                if (((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.d1, position, Side.Black)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.c1, position, Side.Black))
                {
                    movePool[localIndex++] = WhiteLongCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.Encode(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(position.Side), isLongCastle: TRUE),
                        "Wrong hardcoded white long castle move");
                }
            }
            else
            {
                bool ise8Attacked = Attacks.IsSquareAttackedBySide(Constants.BlackKingSourceSquare, position, Side.White);

                if (((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.f8, position, Side.White)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.g8, position, Side.White))
                {
                    movePool[localIndex++] = BlackShortCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.Encode(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(position.Side), isShortCastle: TRUE),
                        "Wrong hardcoded black short castle move");
                }

                if (((position.Castle & (int)CastlingRights.BQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.d8, position, Side.White)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.c8, position, Side.White))
                {
                    movePool[localIndex++] = BlackLongCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.Encode(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(position.Side), isLongCastle: TRUE),
                        "Wrong hardcoded black long castle move");
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

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanGenerateAtLeastAValidMove(Position position)
    {
#if DEBUG
        if (position.Side == Side.Both)
        {
            return false;
        }
#endif

        var offset = Utils.PieceOffset(position.Side);

        return IsAnyPawnMoveValid(position, offset)
            || IsAnyPieceMoveValid((int)Piece.K + offset, position)
            || IsAnyPieceMoveValid((int)Piece.Q + offset, position)
            || IsAnyPieceMoveValid((int)Piece.B + offset, position)
            || IsAnyPieceMoveValid((int)Piece.N + offset, position)
            || IsAnyPieceMoveValid((int)Piece.R + offset, position)
            || IsAnyCastlingMoveValid(position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyPawnMoveValid(Position position, int offset)
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
                    if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset))
                        || IsValidMove(position, MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset))
                        || IsValidMove(position, MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset))
                        || IsValidMove(position, MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset)))
                    {
                        return true;
                    }
                }
                else if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, singlePushSquare, piece)))
                {
                    return true;
                }

                // Double pawn push
                // Inside of the if because singlePush square cannot be occupied either

                var doublePushSquare = sourceSquare + (2 * pawnPush);
                if (!position.OccupancyBitBoards[2].GetBit(doublePushSquare)
                    && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White))
                    && IsValidMove(position, MoveExtensions.Encode(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE)))
                {
                    return true;
                }
            }

            var attacks = Attacks.PawnAttacks[(int)position.Side, sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant)
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                && IsValidMove(position, MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE)))
            {
                return true;
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
                    if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE))
                        || IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE))
                        || IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE))
                        || IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE)))
                    {
                        return true;
                    }
                }
                else if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
    /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyCastlingMoveValid(Position position)
    {
        if (position.Castle != default)
        {
            if (position.Side == Side.White)
            {
                bool ise1Attacked = Attacks.IsSquareAttackedBySide(Constants.WhiteKingSourceSquare, position, Side.Black);

                if (((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.f1, position, Side.Black)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.g1, position, Side.Black)
                    && IsValidMove(position, WhiteShortCastle))
                {
                    return true;
                }

                if (((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.d1, position, Side.Black)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.c1, position, Side.Black)
                    && IsValidMove(position, WhiteLongCastle))
                {
                    return true;
                }
            }
            else
            {
                bool ise8Attacked = Attacks.IsSquareAttackedBySide(Constants.BlackKingSourceSquare, position, Side.White);

                if (((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.f8, position, Side.White)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.g8, position, Side.White)
                    && IsValidMove(position, BlackShortCastle))
                {
                    return true;
                }

                if (((position.Castle & (int)CastlingRights.BQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.d8, position, Side.White)
                    && !Attacks.IsSquareAttackedBySide((int)BoardSquare.c8, position, Side.White)
                    && IsValidMove(position, BlackLongCastle))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyPieceMoveValid(int piece, Position position)
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

                if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare)
                    && IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE)))
                {
                    return true;
                }
                else if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidMove(Position position, Move move)
    {
        var gameState = position.MakeMove(move);
        bool result = position.WasProduceByAValidMove();
        position.UnmakeMove(move, gameState);

        return result;
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
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();

                var targetRank = (targetSquare >> 3) + 1;
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
