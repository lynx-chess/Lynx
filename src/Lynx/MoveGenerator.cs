using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx;

public static class MoveGenerator
{
#if DEBUG
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

    private const int TRUE = 1;

    public static readonly int WhiteShortCastle = MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K);
    public static readonly int WhiteLongCastle = MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K);
    public static readonly int BlackShortCastle = MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k);
    public static readonly int BlackLongCastle = MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.k);

    /// <summary>
    /// Indexed by <see cref="Piece"/>.
    /// Checks are not considered
    /// </summary>
    private static readonly Func<int, BitBoard, BitBoard>[] _pieceAttacks =
    [
        (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White][origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin],

        (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black][origin],
        (int origin, BitBoard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, BitBoard _) => Attacks.KingAttacks[origin],
    ];

    internal static int Init() => TRUE;

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>
    /// </summary>
    /// <param name="capturesOnly">Filters out all moves but captures</param>
    [Obsolete("dev and test only")]
    internal static Move[] GenerateAllMoves(Position position, bool capturesOnly = false)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];

        return (capturesOnly
            ? GenerateAllCaptures(position, moves)
            : GenerateAllMoves(position, moves)).ToArray();
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllMoves(Position position, Span<Move> movePool)
    {
        Debug.Assert(position.Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset(position.Side);

        GenerateAllPawnMoves(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.K + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position);

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllMoves_DoubleChecks(Position position, Span<Move> movePool)
    {
        Debug.Assert(position.Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset(position.Side);
        var oppositeSide = Utils.OppositeSide(position.Side);
        var sameSideKing = position.PieceBitBoards[(int)Piece.K + offset];

        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.K + offset, position);

        if (position._attacks[oppositeSide] == 0
            || (sameSideKing & position._doubleAttacksBySide[oppositeSide]) == 0)
        {
            GenerateAllPawnMoves(ref localIndex, movePool, position, offset);
            GenerateCastlingMoves(ref localIndex, movePool, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position);
            GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position);
        }

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all psuedo-legal captures from <paramref name="position"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllCaptures(Position position, Span<Move> movePool)
    {
        Debug.Assert(position.Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset(position.Side);

        GeneratePawnCapturesAndPromotions(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.K + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset, position);

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all psuedo-legal captures from <paramref name="position"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllCaptures_DoubleChecks(Position position, Span<Move> movePool)
    {
        Debug.Assert(position.Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset(position.Side);
        var oppositeSide = Utils.OppositeSide(position.Side);
        var sameSideKing = position.PieceBitBoards[(int)Piece.K + offset];

        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.K + offset, position);

        if (position._attacks[oppositeSide] == 0
            || (sameSideKing & position._doubleAttacksBySide[oppositeSide]) == 0)
        {
            GeneratePawnCapturesAndPromotions(ref localIndex, movePool, position, offset);
            GenerateCastlingMoves(ref localIndex, movePool, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset, position);
            GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset, position);
        }

        return movePool[..localIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateAllPawnMoves(ref int localIndex, Span<Move> movePool, Position position, int offset)
    {
        int sourceSquare, targetSquare;

        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = position.PieceBitBoards[piece];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out sourceSquare);

            var sourceRank = (sourceSquare >> 3) + 1;

            Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!position.OccupancyBitBoards[(int)Side.Both].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    movePool[localIndex] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    movePool[localIndex + 1] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    movePool[localIndex + 2] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    movePool[localIndex + 3] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);

                    localIndex += 4;
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                    // Double pawn push
                    // Inside of the single pawn push if because singlePush square cannot be occupied either
                    if ((sourceRank == 2)        // position.Side == Side.Black is always true, otherwise targetRank would be 1
                        || (sourceRank == 7))    // position.Side == Side.White is always true, otherwise targetRank would be 8
                    {
                        var doublePushSquare = singlePushSquare + pawnPush;

                        if (!position.OccupancyBitBoards[(int)Side.Both].GetBit(doublePushSquare))
                        {
                            movePool[localIndex++] = MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece);
                        }
                    }
                }
            }

            var attacks = Attacks.PawnAttacks[(int)position.Side][sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                movePool[localIndex++] = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece, capturedPiece: (int)Piece.p - offset);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                attackedSquares = attackedSquares.WithoutLS1B(out targetSquare);
                var capturedPiece = position.Board[targetSquare];

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    movePool[localIndex] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, capturedPiece);
                    movePool[localIndex + 1] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, capturedPiece);
                    movePool[localIndex + 2] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, capturedPiece);
                    movePool[localIndex + 3] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, capturedPiece);

                    localIndex += 4;
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePawnCapturesAndPromotions(ref int localIndex, Span<Move> movePool, Position position, int offset)
    {
        int sourceSquare, targetSquare;

        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = position.PieceBitBoards[piece];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out sourceSquare);

            var sourceRank = (sourceSquare >> 3) + 1;

            Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!position.OccupancyBitBoards[(int)Side.Both].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    movePool[localIndex] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    movePool[localIndex + 1] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    movePool[localIndex + 2] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    movePool[localIndex + 3] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);

                    localIndex += 4;
                }
            }

            var attacks = Attacks.PawnAttacks[(int)position.Side][sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                movePool[localIndex++] = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece, capturedPiece: (int)Piece.p - offset);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                attackedSquares = attackedSquares.WithoutLS1B(out targetSquare);
                var capturedPiece = position.Board[targetSquare];

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    movePool[localIndex] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, capturedPiece: capturedPiece);
                    movePool[localIndex + 1] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, capturedPiece: capturedPiece);
                    movePool[localIndex + 2] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, capturedPiece: capturedPiece);
                    movePool[localIndex + 3] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, capturedPiece: capturedPiece);

                    localIndex += 4;
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece: capturedPiece);
                }
            }
        }
    }

    /// <summary>
    /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
    /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateCastlingMoves(ref int localIndex, Span<Move> movePool, Position position)
    {
        if (position.Castle != default)
        {
            ulong occupancy = position.OccupancyBitBoards[(int)Side.Both];

            if (position.Side == Side.White)
            {
                bool ise1Attacked = position.IsSquareAttacked(Constants.WhiteKingSourceSquare, Side.Black);

                if (!ise1Attacked
                    && (position.Castle & (int)CastlingRights.WK) != default
                    && (occupancy & Constants.WhiteShortCastleFreeSquares) == 0
                    && !position.IsSquareAttacked((int)BoardSquare.f1, Side.Black)
                    && !position.IsSquareAttacked((int)BoardSquare.g1, Side.Black))
                {
                    movePool[localIndex++] = WhiteShortCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K),
                        $"Wrong hardcoded white short castle move, expected {WhiteShortCastle}, got {MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K)}");
                }

                if (!ise1Attacked
                    && (position.Castle & (int)CastlingRights.WQ) != default
                    && (occupancy & Constants.WhiteLongCastleFreeSquares) == 0
                    && !position.IsSquareAttacked((int)BoardSquare.d1, Side.Black)
                    && !position.IsSquareAttacked((int)BoardSquare.c1, Side.Black))
                {
                    movePool[localIndex++] = WhiteLongCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K),
                        $"Wrong hardcoded white long castle move, expected {WhiteLongCastle}, got {MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K)}");
                }
            }
            else
            {
                bool ise8Attacked = position.IsSquareAttacked(Constants.BlackKingSourceSquare, Side.White);

                if ((!ise8Attacked
                    && (position.Castle & (int)CastlingRights.BK) != default)
                    && (occupancy & Constants.BlackShortCastleFreeSquares) == 0
                    && !position.IsSquareAttacked((int)BoardSquare.f8, Side.White)
                    && !position.IsSquareAttacked((int)BoardSquare.g8, Side.White))
                {
                    movePool[localIndex++] = BlackShortCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k),
                        $"Wrong hardcoded black short castle move, expected {BlackShortCastle}, got {MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k)}");
                }

                if (!ise8Attacked
                    && (position.Castle & (int)CastlingRights.BQ) != default
                    && (occupancy & Constants.BlackLongCastleFreeSquares) == 0
                    && !position.IsSquareAttacked((int)BoardSquare.d8, Side.White)
                    && !position.IsSquareAttacked((int)BoardSquare.c8, Side.White))
                {
                    movePool[localIndex++] = BlackLongCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.k),
                        $"Wrong hardcoded black long castle move, expected {BlackLongCastle}, got {MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.k)}");
                }
            }
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateAllPieceMoves(ref int localIndex, Span<Move> movePool, int piece, Position position)
    {
        var bitboard = position.PieceBitBoards[piece];
        int sourceSquare, targetSquare;

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out sourceSquare);

            var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                & ~position.OccupancyBitBoards[(int)position.Side];

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out targetSquare);

                if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                {
                    var capturedPiece = position.Board[targetSquare];
                    movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece: capturedPiece);
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePieceCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position)
    {
        var bitboard = position.PieceBitBoards[piece];
        var oppositeSide = Utils.OppositeSide(position.Side);
        int sourceSquare, targetSquare;

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out sourceSquare);

            var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                & position.OccupancyBitBoards[oppositeSide];

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out targetSquare);
                var capturedPiece = position.Board[targetSquare];
                movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
            }
        }
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanGenerateAtLeastAValidMove(Position position)
    {
        Debug.Assert(position.Side != Side.Both);

        var offset = Utils.PieceOffset(position.Side);

#if DEBUG
        try
        {
#endif
            return IsAnyPawnMoveValid(position, offset)
                || IsAnyPieceMoveValid((int)Piece.K + offset, position)
                || IsAnyPieceMoveValid((int)Piece.Q + offset, position)
                || IsAnyPieceMoveValid((int)Piece.B + offset, position)
                || IsAnyPieceMoveValid((int)Piece.N + offset, position)
                || IsAnyPieceMoveValid((int)Piece.R + offset, position)
                || IsAnyCastlingMoveValid(position);
#if DEBUG
        }
        catch (Exception e)
        {
            _logger.Error(e, $"Error in {nameof(CanGenerateAtLeastAValidMove)}");
            return false;
        }
#endif
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
            bitboard = bitboard.WithoutLS1B(out sourceSquare);

            var sourceRank = (sourceSquare >> 3) + 1;

            Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!position.OccupancyBitBoards[(int)Side.Both].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    if (IsValidMove(position, MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset))
                        || IsValidMove(position, MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset))
                        || IsValidMove(position, MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset))
                        || IsValidMove(position, MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset)))
                    {
                        return true;
                    }
                }
                else
                {
                    if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, singlePushSquare, piece)))
                    {
                        return true;
                    }

                    // Double pawn push
                    // Inside of the if because singlePush square cannot be occupied either
                    var doublePushSquare = singlePushSquare + pawnPush;
                    if (!position.OccupancyBitBoards[(int)Side.Both].GetBit(doublePushSquare)
                        && (sourceRank == 2         // position.Side == Side.Black is always true, otherwise targetRank would be 1
                            || sourceRank == 7)     // position.Side == Side.White is always true, otherwise targetRank would be 8
                        && IsValidMove(position, MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece)))
                    {
                        return true;
                    }
                }
            }

            var attacks = Attacks.PawnAttacks[(int)position.Side][sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant)
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                && IsValidMove(position, MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece)))
            {
                return true;
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                attackedSquares = attackedSquares.WithoutLS1B(out targetSquare);
                var capturedPiece = position.Board[targetSquare];

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    if (IsValidMove(position, MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, capturedPiece))
                        || IsValidMove(position, MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, capturedPiece))
                        || IsValidMove(position, MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, capturedPiece))
                        || IsValidMove(position, MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, capturedPiece)))
                    {
                        return true;
                    }
                }
                else if (IsValidMove(position, MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyCastlingMoveValid(Position position)
    {
        if (position.Castle != default)
        {
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];

            if (position.Side == Side.White)
            {
                bool ise1Attacked = position.IsSquareAttacked(Constants.WhiteKingSourceSquare, Side.Black);

                if (!ise1Attacked
                    && (position.Castle & (int)CastlingRights.WK) != default
                    && (occupancy & Constants.WhiteShortCastleFreeSquares) == 0
                    && !position.IsSquareAttacked((int)BoardSquare.f1, Side.Black)
                    && !position.IsSquareAttacked((int)BoardSquare.g1, Side.Black)
                    && IsValidMove(position, WhiteShortCastle))
                {
                    return true;
                }

                if (!ise1Attacked
                    && (position.Castle & (int)CastlingRights.WQ) != default
                    && (occupancy & Constants.WhiteLongCastleFreeSquares) == 0
                    && !position.IsSquareAttacked((int)BoardSquare.d1, Side.Black)
                    && !position.IsSquareAttacked((int)BoardSquare.c1, Side.Black)
                    && IsValidMove(position, WhiteLongCastle))
                {
                    return true;
                }
            }
            else
            {
                bool ise8Attacked = position.IsSquareAttacked(Constants.BlackKingSourceSquare, Side.White);

                if (!ise8Attacked
                    && (position.Castle & (int)CastlingRights.BK) != default
                    && (occupancy & Constants.BlackShortCastleFreeSquares) == 0
                    && !position.IsSquareAttacked((int)BoardSquare.f8, Side.White)
                    && !position.IsSquareAttacked((int)BoardSquare.g8, Side.White)
                    && IsValidMove(position, BlackShortCastle))
                {
                    return true;
                }

                if ((!ise8Attacked
                    && (position.Castle & (int)CastlingRights.BQ) != default)
                    && (occupancy & Constants.BlackLongCastleFreeSquares) == 0
                    && !position.IsSquareAttacked((int)BoardSquare.d8, Side.White)
                    && !position.IsSquareAttacked((int)BoardSquare.c8, Side.White)
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyPieceMoveValid(int piece, Position position)
    {
        var bitboard = position.PieceBitBoards[piece];
        int sourceSquare, targetSquare;

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out sourceSquare);

            var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                & ~position.OccupancyBitBoards[(int)position.Side];

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out targetSquare);

                if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                {
                    if (IsValidMove(position, MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, position.Board[targetSquare])))
                    {
                        return true;
                    }
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
#if DEBUG
        // After introducing Position.Board, captured piece will always be populared here
        if (move.IsCapture())
        {
            Debug.Assert(move.CapturedPiece() != (int)Piece.None);
        }
        else
        {
            Debug.Assert(
                move.CapturedPiece() == (int)Piece.None
                || move.CapturedPiece() == 0);  // In case of CanGenerateAnyValidMoves() / IsValidMove() scenarios, when we can't pre-populate with Piece.None since otherwise the | captured piece of MoveExtensions.EncodeCapturedPiece() wouldn't work
        }
#endif

        var gameState = position.MakeMove(move);

        bool result = position.WasProduceByAValidMove();
        position.UnmakeMove(move, gameState);

        return result;
    }
}
