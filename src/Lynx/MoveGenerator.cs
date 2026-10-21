using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

public static class MoveGenerator
{
#if DEBUG
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

    private const int TRUE = 1;

    internal static int Init() => TRUE;

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="capturesOnly">Filters out all moves but captures</param>
    [Obsolete("dev and test only")]
    internal static Move[] GenerateAllMoves(Position position, bool capturesOnly = false)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        return (capturesOnly
            ? GenerateAllCaptures(position, moves)
            : GenerateAllMoves(position, moves)).ToArray();
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllMoves(Position position, Span<Move> movePool, Bitboard oppositeSideAttacks = 0)
    {
        Debug.Assert(position.Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset((int)position.Side);

        GenerateAllPawnMoves(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position, oppositeSideAttacks);
        GenerateKingMoves(ref localIndex, movePool, (int)Piece.K + offset, position, oppositeSideAttacks);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position);

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all pseudo-legal quiet moves from <paramref name="position"/>, castling included.
    /// Complements <see cref="GenerateAllCaptures(Position, Span{int}, Bitboard)"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllQuiets(Position position, Span<Move> movePool, Bitboard oppositeSideAttacks)
    {
        Debug.Assert(position.Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset((int)position.Side);

        GeneratePawnQuiets(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position, oppositeSideAttacks);
        GenerateQuietKingMoves(ref localIndex, movePool, (int)Piece.K + offset, position, oppositeSideAttacks);
        GenerateQuietPieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position);
        GenerateQuietPieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position);
        GenerateQuietPieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position);
        GenerateQuietPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position);

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all psuedo-legal captures from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>.
    /// Complements <see cref="GenerateAllQuiets(Position, Span{int}, ulong)"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllCaptures(Position position, Span<Move> movePool, Bitboard oppositeSideAttacks = 0)
    {
        Debug.Assert(position.Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset((int)position.Side);

        GeneratePawnCapturesAndPromotions(ref localIndex, movePool, position, offset);
        GenerateKingCaptures(ref localIndex, movePool, (int)Piece.K + offset, position, oppositeSideAttacks);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset, position);

        return movePool[..localIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateAllPawnMoves(ref int localIndex, Span<Move> movePool, Position position, int offset)
    {
        var occupancy = position.OccupancyBitboards[(int)Side.Both];
        var piece = (int)Piece.P + offset;
        var pawnPush = Utils.PawhPushSquare((int)position.Side);
        int oppositeSide = Utils.OppositeSide((int)position.Side);
        var bitboard = position.PieceBitboards[piece];

        var pawnAttacks = Attacks.PawnAttacks[(int)position.Side];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard.NotEmpty())
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = sourceSquare >> 3;

            Debug.Assert(sourceRank != 0 && sourceRank != 7, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!occupancy.GetBit(singlePushSquare))
            {
                // Single pawn push
                var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare);

                var targetRank = singlePushSquare >> 3;
                if (targetRank == 0 || targetRank == 7)
                {
                    // Promotion
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.Q);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.R);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.B);
                }
                else
                {
                    Unsafe.Add(ref movePoolRef, localIndex++) = singlePawnPush;

                    // Double pawn push
                    // Inside of the single pawn push if because singlePush square cannot be occupied either
                    if ((sourceRank == 1)        // position.Side == Side.Black is always true, otherwise targetRank would be 0
                        || (sourceRank == 6))    // position.Side == Side.White is always true, otherwise targetRank would be 7
                    {
                        var doublePushSquare = singlePushSquare + pawnPush;

                        if (!occupancy.GetBit(doublePushSquare))
                        {
                            Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, doublePushSquare);
                        }
                    }
                }
            }

            var attacks = pawnAttacks[sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit((int)position.EnPassant))
            // We assume that position.OccupancyBitboards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitboards[oppositeSide];
            while (attackedSquares.NotEmpty())
            {
                attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);

                var pawnCapture = MoveExtensions.Encode(sourceSquare, targetSquare);

                var targetRank = targetSquare >> 3;
                if (targetRank == 0 || targetRank == 7)
                {
                    // Capture with promotion
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.Q);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.R);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.B);
                }
                else
                {
                    Unsafe.Add(ref movePoolRef, localIndex++) = pawnCapture;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePawnQuiets(ref int localIndex, Span<Move> movePool, Position position, int offset)
    {
        var occupancy = position.OccupancyBitboards[(int)Side.Both];
        var piece = (int)Piece.P + offset;
        var pawnPush = Utils.PawhPushSquare((int)position.Side);
        var bitboard = position.PieceBitboards[piece];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard.NotEmpty())
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = sourceSquare >> 3;
            Debug.Assert(sourceRank != 0 && sourceRank != 7, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!occupancy.GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = singlePushSquare >> 3;

                if (targetRank != 0 && targetRank != 7)
                {
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, singlePushSquare);

                    // Double pawn push
                    // Inside of the single pawn push if because singlePush square cannot be occupied either
                    if ((sourceRank == 1)        // position.Side == Side.Black is always true, otherwise targetRank would be 0
                        || (sourceRank == 6))    // position.Side == Side.White is always true, otherwise targetRank would be 7
                    {
                        var doublePushSquare = singlePushSquare + pawnPush;

                        if (!occupancy.GetBit(doublePushSquare))
                        {
                            Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, doublePushSquare);
                        }
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePawnCapturesAndPromotions(ref int localIndex, Span<Move> movePool, Position position, int offset)
    {
        var piece = (int)Piece.P + offset;
        var pawnPush = Utils.PawhPushSquare((int)position.Side);
        int oppositeSide = Utils.OppositeSide((int)position.Side);
        var bitboard = position.PieceBitboards[piece];

        var occupancy = position.OccupancyBitboards[(int)Side.Both];
        var oppositeSidePieces = position.OccupancyBitboards[oppositeSide];

        var pawnAttacks = Attacks.PawnAttacks[(int)position.Side];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard.NotEmpty())
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = sourceSquare >> 3;

            Debug.Assert(sourceRank != 0 && sourceRank != 7, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!occupancy.GetBit(singlePushSquare))
            {
                // Single pawn push
                var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare);

                var targetRank = singlePushSquare >> 3;
                if (targetRank == 0 || targetRank == 7)
                {
                    // Promotion
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.Q);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.R);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.B);
                }
            }

            var attacks = pawnAttacks[sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit((int)position.EnPassant))
            // We assume that position.OccupancyBitboards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant);
            }

            // Captures
            var attackedSquares = attacks & oppositeSidePieces;
            while (attackedSquares.NotEmpty())
            {
                attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);

                var pawnCapture = MoveExtensions.Encode(sourceSquare, targetSquare);

                var targetRank = targetSquare >> 3;
                if (targetRank == 0 || targetRank == 7)
                {
                    // Capture with promotion
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.Q);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.R);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N);
                    Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.B);
                }
                else
                {
                    Unsafe.Add(ref movePoolRef, localIndex++) = pawnCapture;
                }
            }
        }
    }

    /// <summary>
    /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
    /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GenerateCastlingMoves(ref int localIndex, Span<Move> movePool, Position position, Bitboard oppositeSideAttacks)
    {
        var castlingRights = position.Castle;

        if (castlingRights != default)
        {
            var occupancy = position.OccupancyBitboards[(int)Side.Both];

            if (position.Side == Side.White)
            {
                if ((castlingRights & (int)CastlingRights.WK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]).Empty()
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, oppositeSideAttacks))
                {
                    movePool[localIndex++] = position.WhiteShortCastle;
                }

                if ((castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]).Empty()
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, oppositeSideAttacks))

                {
                    movePool[localIndex++] = position.WhiteLongCastle;
                }
            }
            else
            {
                if ((castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]).Empty()
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, oppositeSideAttacks))
                {
                    movePool[localIndex++] = position.BlackShortCastle;
                }

                if ((castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]).Empty()
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, oppositeSideAttacks))
                {
                    movePool[localIndex++] = position.BlackLongCastle;
                }
            }
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen moves
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateAllPieceMoves(ref int localIndex, Span<Move> movePool, int piece, Position position)
    {
        var bitboard = position.PieceBitboards[piece];

        var occupancy = position.OccupancyBitboards[(int)Side.Both];
        var squaresNotOccupiedByUs = ~position.OccupancyBitboards[(int)position.Side];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard.NotEmpty())
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = PieceAttacks(piece, sourceSquare, occupancy)
                & squaresNotOccupiedByUs;

            while (attacks.NotEmpty())
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare);
            }
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen quiet moves.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateQuietPieceMoves(ref int localIndex, Span<Move> movePool, int piece, Position position)
    {
        var bitboard = position.PieceBitboards[piece];

        var occupancy = position.OccupancyBitboards[(int)Side.Both];
        var emptySquares = ~occupancy;

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard.NotEmpty())
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = PieceAttacks(piece, sourceSquare, occupancy)
                & emptySquares;

            while (attacks.NotEmpty())
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);
                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare);
            }
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen capture moves.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePieceCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position)
    {
        var bitboard = position.PieceBitboards[piece];
        var oppositeSide = Utils.OppositeSide((int)position.Side);

        var occupancy = position.OccupancyBitboards[(int)Side.Both];
        var oppositeSidePieces = position.OccupancyBitboards[oppositeSide];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard.NotEmpty())
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = PieceAttacks(piece, sourceSquare, occupancy)
                & oppositeSidePieces;

            while (attacks.NotEmpty())
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);
                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateKingMoves(ref int localIndex, Span<Move> movePool, int piece, Position position, Bitboard oppositeSideAttacks)
    {
        var sourceSquare = position.PieceBitboards[piece].GetLS1BIndex();
        var occupancy = position.OccupancyBitboards[(int)Side.Both];

        var attacks = Attacks.KingAttacks[sourceSquare]
            & ~position.OccupancyBitboards[(int)position.Side]
            & ~oppositeSideAttacks;

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (attacks.NotEmpty())
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);

            Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

            Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateQuietKingMoves(ref int localIndex, Span<Move> movePool, int piece, Position position, Bitboard oppositeSideKingAttacks)
    {
        var sourceSquare = position.PieceBitboards[piece].GetLS1BIndex();
        var occupancy = position.OccupancyBitboards[(int)Side.Both];

        var attacks = Attacks.KingAttacks[sourceSquare]
            & ~occupancy
            & ~oppositeSideKingAttacks;

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (attacks.NotEmpty())
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);
            Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare);
        }
    }

    /// <summary>
    /// Generate King capture moves
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateKingCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position, Bitboard oppositeSideAttacks)
    {
        var sourceSquare = position.PieceBitboards[piece].GetLS1BIndex();
        var oppositeSide = Utils.OppositeSide((int)position.Side);

        var attacks = Attacks.KingAttacks[sourceSquare]
            & position.OccupancyBitboards[oppositeSide]
            & ~oppositeSideAttacks;
        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (attacks.NotEmpty())
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);
            Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanGenerateAtLeastAValidMove(Position position, Bitboard oppositeSideAttacks = 0)
    {
        Debug.Assert(position.Side != Side.Both);

        var offset = Utils.PieceOffset((int)position.Side);

#if DEBUG
        try
        {
#endif
            return IsAnyPawnMoveValid(position, offset)
                || IsAnyKingMoveValid((int)Piece.K + offset, position, oppositeSideAttacks)
                || IsAnyPieceMoveValid((int)Piece.Q + offset, position)
                || IsAnyPieceMoveValid((int)Piece.B + offset, position)
                || IsAnyPieceMoveValid((int)Piece.N + offset, position)
                || IsAnyPieceMoveValid((int)Piece.R + offset, position)
                || IsAnyCastlingMoveValid(position, oppositeSideAttacks);
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
        var piece = (int)Piece.P + offset;
        var pawnPush = Utils.PawhPushSquare((int)position.Side);
        int oppositeSide = Utils.OppositeSide((int)position.Side);
        var bitboard = position.PieceBitboards[piece];

        var occupancy = position.OccupancyBitboards[(int)Side.Both];
        var oppositeSidePieces = position.OccupancyBitboards[oppositeSide];

        while (bitboard.NotEmpty())
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = sourceSquare >> 3;

            Debug.Assert(sourceRank != 0 && sourceRank != 7, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!occupancy.GetBit(singlePushSquare))
            {
                // Single pawn push
                var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare);

                var targetRank = singlePushSquare >> 3;
                if (targetRank == 0 || targetRank == 7)  // Promotion
                {
                    // If any of the promotions isn't valid, it means that the pawn move unveils a discovered check, or that the promoted piece doesn't stop an existing check in the 8th rank
                    // Therefore none of the other promotions will be valid either
                    if (IsValidMove(position, MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.Q)))
                    {
                        return true;
                    }
                }
                else
                {
                    if (IsValidMove(position, singlePawnPush))
                    {
                        return true;
                    }

                    // Double pawn push
                    // Inside of the if because singlePush square cannot be occupied either
                    var doublePushSquare = singlePushSquare + pawnPush;
                    if (!occupancy.GetBit(doublePushSquare)
                        && (sourceRank == 1         // position.Side == Side.Black is always true, otherwise targetRank would be 0
                            || sourceRank == 6)     // position.Side == Side.White is always true, otherwise targetRank would be 7
                        && IsValidMove(position, MoveExtensions.Encode(sourceSquare, doublePushSquare)))
                    {
                        return true;
                    }
                }
            }

            var attacks = Attacks.PawnAttacks[(int)position.Side][sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit((int)position.EnPassant)
                // We assume that position.OccupancyBitboards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                && IsValidMove(position, MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant))) // Could add here capturedPiece: (int)Piece.p - offset
            {
                return true;
            }

            // Captures
            var attackedSquares = attacks & oppositeSidePieces;
            while (attackedSquares.NotEmpty())
            {
                attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);

                var pawnCapture = MoveExtensions.Encode(sourceSquare, targetSquare);

                var targetRank = targetSquare >> 3;
                if (targetRank == 0 || targetRank == 7)  // Capture with promotion
                {
                    // If any of the promotions that capture the same piece isn't valid, it means that the pawn move unveils a discovered check, or that the capture doesn't stop an existing check in the 8th rank
                    // Therefore none of the other promotions capturing the same piece will be valid either
                    if (IsValidMove(position, MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.Q)))
                    {
                        return true;
                    }
                }
                else if (IsValidMove(position, pawnCapture))
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
    public static bool IsAnyCastlingMoveValid(Position position, Bitboard oppositeSideAttacks)
    {
        // TODO: move to position?

        var castlingRights = position.Castle;

        if (castlingRights != default)
        {
            var occupancy = position.OccupancyBitboards[(int)Side.Both];

            if (position.Side == Side.White)
            {
                if ((castlingRights & (int)CastlingRights.WK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]).Empty()
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, oppositeSideAttacks)
                    && IsValidMove(position, position.WhiteShortCastle))
                {
                    return true;
                }

                if ((castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]).Empty()
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, oppositeSideAttacks)
                    && IsValidMove(position, position.WhiteLongCastle))
                {
                    return true;
                }
            }
            else
            {
                if ((castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]).Empty()
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, oppositeSideAttacks)
                    && IsValidMove(position, position.BlackShortCastle))
                {
                    return true;
                }

                if ((castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]).Empty()
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, oppositeSideAttacks)
                    && IsValidMove(position, position.BlackLongCastle))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Also valid for Kings, but less performant than <see cref="IsAnyKingMoveValid(int, Position)"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyPieceMoveValid(int piece, Position position)
    {
        var bitboard = position.PieceBitboards[piece];

        var occupancy = position.OccupancyBitboards[(int)Side.Both];
        var squaresNotOccupiedByUs = ~position.OccupancyBitboards[(int)position.Side];

        while (bitboard.NotEmpty())
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = PieceAttacks(piece, sourceSquare, occupancy)
                & squaresNotOccupiedByUs;

            while (attacks.NotEmpty())
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyKingMoveValid(int piece, Position position, Bitboard oppositeSideAttacks)
    {
        var sourceSquare = position.PieceBitboards[piece].GetLS1BIndex();
        var occupancy = position.OccupancyBitboards[(int)Side.Both];

        var attacks = Attacks.KingAttacks[sourceSquare]
            & ~position.OccupancyBitboards[(int)position.Side]
            & ~oppositeSideAttacks;

        while (attacks.NotEmpty())
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);

            Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

            if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare)))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Bitboard PieceAttacks(int piece, int square, Bitboard occupancy)
    {
        Debug.Assert(piece != (int)Piece.None && piece != (int)Piece.Unknown);

        return piece switch
        {
            (int)Piece.P => Attacks.PawnAttacks[(int)Side.White][square],
            (int)Piece.p => Attacks.PawnAttacks[(int)Side.Black][square],
            (int)Piece.N or (int)Piece.n => Attacks.KnightAttacks[square],
            (int)Piece.B or (int)Piece.b => Attacks.BishopAttacks(square, occupancy),
            (int)Piece.R or (int)Piece.r => Attacks.RookAttacks(square, occupancy),
            (int)Piece.Q or (int)Piece.q => Attacks.QueenAttacks(square, occupancy),
            _ => Attacks.KingAttacks[square],
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsValidMove(Position position, Move move)
    {
        var gameState = position.MakeMove(move);

        bool result = position.WasProduceByAValidMove();
        position.UnmakeMove(move, gameState);

        return result;
    }

    /// <summary>
    /// Whether <paramref name="move"/> (i.e. coming from the TT, killers or counter moves, so potentially from a different position)
    /// is a pseudo-legal move in <paramref name="position"/>.
    /// Since moves only contain squares and flags, the piece and the captured piece are taken from the board
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPseudoLegal(Position position, Move move, Bitboard oppositeSideAttacks)
    {
        var side = (int)position.Side;
        var sourceSquare = move.SourceSquare();
        var targetSquare = move.TargetSquare();
        var piece = position.Board[sourceSquare];

        if (move == 0
            || !IsPieceFromSide(piece, side)
            || sourceSquare == targetSquare)
        {
            return false;
        }

        var specialMove = move.SpecialMoveFlag();
        if (specialMove == SpecialMoveType.Castle)
        {
            return IsPseudoLegalCastlingMove(position, move, oppositeSideAttacks);
        }

        var isPawn = piece == (int)Piece.P || piece == (int)Piece.p;

        if (specialMove != SpecialMoveType.None && !isPawn)
        {
            // En passant and promotions are pawn moves
            return false;
        }

        if (specialMove == SpecialMoveType.EnPassant)
        {
            if (!IsPseudoLegalPawnMove(position, move, allowOnlyEnPassant: true))
            {
                return false;
            }

            var pawnPush = Utils.PawhPushSquare(side);
            var capturedSquare = targetSquare - pawnPush;

            return (BoardSquare)targetSquare == position.EnPassant
                && position.Board[targetSquare] == (int)Piece.None
                && IsOpponentPiece(position.Board[capturedSquare], side);
        }

        var targetPiece = position.Board[targetSquare];
        if (targetPiece != (int)Piece.None
            && (IsPieceFromSide(targetPiece, side)
                || targetPiece == (int)Piece.K
                || targetPiece == (int)Piece.k))
        {
            return false;
        }

        return isPawn
            ? IsPseudoLegalPawnMove(position, move, allowOnlyEnPassant: false)
            : IsPseudoLegalPieceMove(position, move);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPseudoLegalPieceMove(Position position, Move move)
    {
        var sourceSquare = move.SourceSquare();
        var targetSquare = move.TargetSquare();
        var piece = position.Board[sourceSquare];
        var occupancy = position.OccupancyBitboards[(int)Side.Both];

        return PieceAttacks(piece, sourceSquare, occupancy).GetBit(targetSquare);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPseudoLegalPawnMove(Position position, Move move, bool allowOnlyEnPassant)
    {
        var side = (int)position.Side;
        var sourceSquare = move.SourceSquare();
        var targetSquare = move.TargetSquare();
        var pawnPush = Utils.PawhPushSquare(side);

        var occupancy = position.OccupancyBitboards[(int)Side.Both];
        var oppositeSide = Utils.OppositeSide(side);
        var oppositeSidePieces = position.OccupancyBitboards[oppositeSide];

        var sourceRank = sourceSquare >> 3;
        var targetRank = targetSquare >> 3;

        // The promoted piece is always valid (N, B, R or Q of the side to move)
        if (move.IsPromotion())
        {
            if (targetRank != 0 && targetRank != 7)
            {
                return false;
            }
        }
        else if (targetRank == 0 || targetRank == 7)
        {
            return false;
        }

        var attackMask = Attacks.PawnAttacks[side][sourceSquare];
        if (attackMask.GetBit(targetSquare))
        {
            if (allowOnlyEnPassant)
            {
                return (BoardSquare)targetSquare == position.EnPassant;
            }

            return oppositeSidePieces.GetBit(targetSquare);
        }

        if (allowOnlyEnPassant)
        {
            return false;
        }

        if (targetSquare == sourceSquare + pawnPush)
        {
            return !occupancy.GetBit(targetSquare);
        }

        // Double pawn pushes can't be promotions
        return targetSquare == sourceSquare + (2 * pawnPush)
            && !move.IsPromotion()
            && (sourceRank == 1 || sourceRank == 6)
            && !occupancy.GetBit(sourceSquare + pawnPush)
            && !occupancy.GetBit(targetSquare);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPseudoLegalCastlingMove(Position position, Move move, Bitboard oppositeSideAttacks)
    {
        var side = position.Side;
        var castlingRights = position.Castle;
        var occupancy = position.OccupancyBitboards[(int)Side.Both];

        if (side == Side.White)
        {
            if (move == position.WhiteShortCastle)
            {
                return (castlingRights & (int)CastlingRights.WK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]).Empty()
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, oppositeSideAttacks);
            }

            if (move == position.WhiteLongCastle)
            {
                return (castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]).Empty()
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, oppositeSideAttacks);
            }
        }
        else
        {
            if (move == position.BlackShortCastle)
            {
                return (castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]).Empty()
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, oppositeSideAttacks);
            }

            if (move == position.BlackLongCastle)
            {
                return (castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]).Empty()
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, oppositeSideAttacks);
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPieceFromSide(int piece, int side)
    {
        var offset = Utils.PieceOffset(side);
        var p = piece - offset;

        return p >= (int)Piece.P && p <= (int)Piece.K;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsOpponentPiece(int piece, int side)
    {
        return piece != (int)Piece.None && !IsPieceFromSide(piece, side);
    }
}
