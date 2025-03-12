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
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move GenerateMove(Position position, Move move)
    {
        var offset = Utils.PieceOffset(position.Side);

        // move could be a ShortMove if it comes from TT
        var sourceSquare = move.SourceSquare();
        var targetSquare = move.TargetSquare();
        var promotedPiece = move.PromotedPiece();

        if (promotedPiece != default)
        {
            var pawn = position.Board[sourceSquare];
            if (pawn == (int)Piece.P + offset)
            {
                return GeneratePawnPromotion(sourceSquare, targetSquare, promotedPiece, position, offset);
            }

            return default;
        }

        var piece = position.Board[sourceSquare];
        if (piece == (int)Piece.None
            || piece - offset < (int)Piece.P
            || piece - offset > (int)Piece.K)
        {
            return default;
        }

        if (piece != (int)Piece.K + offset)
        {
            return (piece == (int)Piece.P + offset)
                ? GenerateNonPromotionPawnMove(sourceSquare, targetSquare, piece, position, offset)
                : GeneratePieceMove(sourceSquare, targetSquare, piece, position);
        }

        // Account for potential castling moves
        if (sourceSquare == Constants.WhiteKingSourceSquare)
        {
            if (targetSquare == Constants.WhiteShortCastleKingSquare)
            {
                if (position.Castle != default
                    && position.Side == Side.White
                    && ((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !position.IsSquareAttackedBySide(Constants.WhiteKingSourceSquare, Side.Black)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.f1, Side.Black)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.g1, Side.Black))
                {
                    return WhiteShortCastle;
                }

                return default;
            }

            if (targetSquare == Constants.WhiteLongCastleKingSquare)
            {
                if (position.Castle != default
                    && position.Side == Side.White
                    && ((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !position.IsSquareAttackedBySide(Constants.WhiteKingSourceSquare, Side.Black)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.d1, Side.Black)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.c1, Side.Black))
                {
                    return WhiteLongCastle;
                }

                return default;
            }
        }
        else if (sourceSquare == Constants.BlackKingSourceSquare)
        {
            if (targetSquare == Constants.BlackShortCastleKingSquare)
            {
                if (position.Castle != default
                    && position.Side == Side.Black
                    && ((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !position.IsSquareAttackedBySide(Constants.BlackKingSourceSquare, Side.White)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.f8, Side.White)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.g8, Side.White))
                {
                    return BlackShortCastle;
                }

                return default;
            }

            if (targetSquare == Constants.BlackLongCastleKingSquare)
            {
                if (position.Castle != default
                        && position.Side == Side.Black
                        && ((position.Castle & (int)CastlingRights.BQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                        && !position.IsSquareAttackedBySide(Constants.BlackKingSourceSquare, Side.White)
                        && !position.IsSquareAttackedBySide((int)BoardSquare.d8, Side.White)
                        && !position.IsSquareAttackedBySide((int)BoardSquare.c8, Side.White))
                {
                    return BlackLongCastle;
                }

                return default;
            }
        }

        return GeneratePieceMove(sourceSquare, targetSquare, piece, position);
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
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
    /// Generates all psuedo-legal captures from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move[] GenerateAllCaptures(Position position, Move[] movePool)
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
    /// Generates all psuedo-legal captures from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
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
            if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
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
                    movePool[localIndex++] = MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece);
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
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, capturedPiece);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, capturedPiece);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, capturedPiece);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, capturedPiece);
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
            if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
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
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, capturedPiece: capturedPiece);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, capturedPiece: capturedPiece);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, capturedPiece: capturedPiece);
                    movePool[localIndex++] = MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, capturedPiece: capturedPiece);
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece: capturedPiece);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Move GenerateNonPromotionPawnMove(int sourceSquare, int targetSquare, int piece, Position position, int offset)
    {
        var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide(position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White

        var sourceRank = (sourceSquare >> 3) + 1;
        Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

        // Pawn pushes
        // Single pawn push
        var singlePushSquare = sourceSquare + pawnPush;
        if (targetSquare == singlePushSquare)
        {
            if (position.Board[targetSquare] == (int)Piece.None)
            {
                return MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
            }
            return default;
        }

        // Double pawn push
        var doublePushSquare = sourceSquare + (2 * pawnPush);
        if (targetSquare == doublePushSquare)
        {
            if (position.Board[targetSquare] == (int)Piece.None
                && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White)))
            {
                return MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
            }

            return default;
        }

        var attacks = Attacks.PawnAttacks[(int)position.Side][sourceSquare];
        // En passant
        Debug.Assert(targetSquare != (int)BoardSquare.noSquare);

        if (targetSquare == (int)position.EnPassant)
        {
            if (attacks.GetBit(position.EnPassant))
            {
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                return MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece, capturedPiece: (int)Piece.p - offset);
            }

            return default;
        }

        // Captures
        var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
        if (attackedSquares.GetBit(targetSquare))
        {
            var capturedPiece = position.Board[targetSquare];

            if (capturedPiece != (int)Piece.None)
            {
                return MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
            }
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Move GeneratePawnPromotion(int sourceSquare, int targetSquare, int promotedPiece, Position position, int offset)
    {
        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8

        var singlePushSquare = sourceSquare + pawnPush;
        if (targetSquare == singlePushSquare)   // Non-capture promotion
        {
            Debug.Assert(promotedPiece - offset >= (int)Piece.N && promotedPiece - offset <= (int)Piece.Q);

            if (position.Board[targetSquare] == (int)Piece.None)
            {
                return MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece);
            }
        }
        else // Capture promotion
        {
            var capturedPiece = position.Board[targetSquare];

            if (capturedPiece != (int)Piece.None)
            {
                return MoveExtensions.EncodePromotion(sourceSquare, targetSquare, piece, promotedPiece, capturedPiece);
            }
        }

        return default;
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
            if (position.Side == Side.White)
            {
                bool ise1Attacked = position.IsSquareAttackedBySide(Constants.WhiteKingSourceSquare, Side.Black);

                if (((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !position.IsSquareAttackedBySide((int)BoardSquare.f1, Side.Black)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.g1, Side.Black))
                {
                    movePool[localIndex++] = WhiteShortCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K),
                        $"Wrong hardcoded white short castle move, expected {WhiteShortCastle}, got {MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K)}");
                }

                if (((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !position.IsSquareAttackedBySide((int)BoardSquare.d1, Side.Black)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.c1, Side.Black))
                {
                    movePool[localIndex++] = WhiteLongCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K),
                        $"Wrong hardcoded white long castle move, expected {WhiteLongCastle}, got {MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K)}");
                }
            }
            else
            {
                bool ise8Attacked = position.IsSquareAttackedBySide(Constants.BlackKingSourceSquare, Side.White);

                if (((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !position.IsSquareAttackedBySide((int)BoardSquare.f8, Side.White)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.g8, Side.White))
                {
                    movePool[localIndex++] = BlackShortCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k),
                        $"Wrong hardcoded black short castle move, expected {BlackShortCastle}, got {MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k)}");
                }

                if (((position.Castle & (int)CastlingRights.BQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !position.IsSquareAttackedBySide((int)BoardSquare.d8, Side.White)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.c8, Side.White))
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Move GeneratePieceMove(int sourceSquare, int targetSquare, int piece, Position position)
    {
        var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
            & ~position.OccupancyBitBoards[(int)position.Side];

        if (attacks.GetBit(targetSquare))
        {
            if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))   // Capture
            {
                var capturedPiece = position.Board[targetSquare];

                if (capturedPiece != (int)Piece.None)
                {
                    return MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
                }
            }
            else
            {
                return MoveExtensions.Encode(sourceSquare, targetSquare, piece);
            }
        }

        return default;
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
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
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
            if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
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
                else if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, singlePushSquare, piece)))
                {
                    return true;
                }

                // Double pawn push
                // Inside of the if because singlePush square cannot be occupied either

                var doublePushSquare = sourceSquare + (2 * pawnPush);
                if (!position.OccupancyBitBoards[2].GetBit(doublePushSquare)
                    && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White))
                    && IsValidMove(position, MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece)))
                {
                    return true;
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

    /// <summary>
    /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
    /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyCastlingMoveValid(Position position)
    {
        if (position.Castle != default)
        {
            if (position.Side == Side.White)
            {
                bool ise1Attacked = position.IsSquareAttackedBySide(Constants.WhiteKingSourceSquare, Side.Black);

                if (((position.Castle & (int)CastlingRights.WK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !position.IsSquareAttackedBySide((int)BoardSquare.f1, Side.Black)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.g1, Side.Black)
                    && IsValidMove(position, WhiteShortCastle))
                {
                    return true;
                }

                if (((position.Castle & (int)CastlingRights.WQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !position.IsSquareAttackedBySide((int)BoardSquare.d1, Side.Black)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.c1, Side.Black)
                    && IsValidMove(position, WhiteLongCastle))
                {
                    return true;
                }
            }
            else
            {
                bool ise8Attacked = position.IsSquareAttackedBySide(Constants.BlackKingSourceSquare, Side.White);

                if (((position.Castle & (int)CastlingRights.BK) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !position.IsSquareAttackedBySide((int)BoardSquare.f8, Side.White)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.g8, Side.White)
                    && IsValidMove(position, BlackShortCastle))
                {
                    return true;
                }

                if (((position.Castle & (int)CastlingRights.BQ) != default)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !position.IsSquareAttackedBySide((int)BoardSquare.d8, Side.White)
                    && !position.IsSquareAttackedBySide((int)BoardSquare.c8, Side.White)
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
