using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

public static class MoveGenerator
{
    private static GameState _gameState;

#if DEBUG
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

    private const int TRUE = 1;

    internal static int Init() => TRUE;

    /// <summary>
    /// Indexed by <see cref="Piece"/>.
    /// Checks are not considered
    /// </summary>
    public static readonly Func<int, BitBoard, BitBoard>[] _pieceAttacks =
    [
#pragma warning disable IDE0350 // Use implicitly typed lambda
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
#pragma warning restore IDE0350 // Use implicitly typed lambda
    ];

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="capturesOnly">Filters out all moves but captures</param>
    [Obsolete("dev and test only")]
    internal static Move[] GenerateAllMoves(Position position, bool capturesOnly = false)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        return (capturesOnly
            ? GenerateAllCaptures(position, ref evaluationContext, moves)
            : GenerateAllMoves(position, ref evaluationContext, moves)).ToArray();
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllMoves(Position position, ref EvaluationContext evaluationContext, Span<Move> movePool)
    {
        Debug.Assert(position.Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset((int)position.Side);

        GenerateAllPawnMoves(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position, ref evaluationContext);
        GenerateKingMoves(ref localIndex, movePool, (int)Piece.K + offset, position, ref evaluationContext);
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
    public static Span<Move> GenerateAllCaptures(Position position, ref EvaluationContext evaluationContext, Span<Move> movePool)
    {
        Debug.Assert(position.Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset((int)position.Side);

        GeneratePawnCapturesAndPromotions(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position, ref evaluationContext);
        GenerateKingCaptures(ref localIndex, movePool, (int)Piece.K + offset, position, ref evaluationContext);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset, position);

        return movePool[..localIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateAllPawnMoves(ref int localIndex, Span<Move> movePool, Position position, int offset)
    {
        var occupancy = position.OccupancyBitBoards[(int)Side.Both];
        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide((int)position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = position.PieceBitBoards[piece];

        var pawnAttacks = Attacks.PawnAttacks[(int)position.Side];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = (sourceSquare >> 3) + 1;

            Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!occupancy.GetBit(singlePushSquare))
            {
                // Single pawn push
                var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)
                {
                    // Promotion
                    var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N + offset);

                    Unsafe.Add(ref movePoolRef, localIndex) = knightPromo + 3;         // Q
                    Unsafe.Add(ref movePoolRef, localIndex + 1) = knightPromo + 2;     // R
                    Unsafe.Add(ref movePoolRef, localIndex + 2) = knightPromo;         // N
                    Unsafe.Add(ref movePoolRef, localIndex + 3) = knightPromo + 1;     // B

                    localIndex += 4;
                }
                else
                {
                    Unsafe.Add(ref movePoolRef, localIndex++) = singlePawnPush;

                    // Double pawn push
                    // Inside of the single pawn push if because singlePush square cannot be occupied either
                    if ((sourceRank == 2)        // position.Side == Side.Black is always true, otherwise targetRank would be 1
                        || (sourceRank == 7))    // position.Side == Side.White is always true, otherwise targetRank would be 8
                    {
                        var doublePushSquare = singlePushSquare + pawnPush;

                        if (!occupancy.GetBit(doublePushSquare))
                        {
                            Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece);
                        }
                    }
                }
            }

            var attacks = pawnAttacks[sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit((int)position.EnPassant))
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece, capturedPiece: (int)Piece.p - offset);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                var capturedPiece = position.Board[targetSquare];

                var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)
                {
                    // Capture with promotion
                    var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N + offset);

                    Unsafe.Add(ref movePoolRef, localIndex) = knightPromo + 3;
                    Unsafe.Add(ref movePoolRef, localIndex + 1) = knightPromo + 2;
                    Unsafe.Add(ref movePoolRef, localIndex + 2) = knightPromo;
                    Unsafe.Add(ref movePoolRef, localIndex + 3) = knightPromo + 1;

                    localIndex += 4;
                }
                else
                {
                    Unsafe.Add(ref movePoolRef, localIndex++) = pawnCapture;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePawnCapturesAndPromotions(ref int localIndex, Span<Move> movePool, Position position, int offset)
    {
        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide((int)position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = position.PieceBitBoards[piece];

        var occupancy = position.OccupancyBitBoards[(int)Side.Both];
        var oppositeSidePieces = position.OccupancyBitBoards[oppositeSide];

        var pawnAttacks = Attacks.PawnAttacks[(int)position.Side];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = (sourceSquare >> 3) + 1;

            Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!occupancy.GetBit(singlePushSquare))
            {
                // Single pawn push
                var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)
                {
                    // Promotion
                    var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N + offset);

                    Unsafe.Add(ref movePoolRef, localIndex) = knightPromo + 3;         // Q
                    Unsafe.Add(ref movePoolRef, localIndex + 1) = knightPromo + 2;     // R
                    Unsafe.Add(ref movePoolRef, localIndex + 2) = knightPromo;         // N
                    Unsafe.Add(ref movePoolRef, localIndex + 3) = knightPromo + 1;     // B

                    localIndex += 4;
                }
            }

            var attacks = pawnAttacks[sourceSquare];

            // En passant
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit((int)position.EnPassant))
            // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece, capturedPiece: (int)Piece.p - offset);
            }

            // Captures
            var attackedSquares = attacks & oppositeSidePieces;
            while (attackedSquares != default)
            {
                attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                var capturedPiece = position.Board[targetSquare];

                var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)
                {
                    // Capture with promotion
                    var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N + offset);

                    Unsafe.Add(ref movePoolRef, localIndex) = knightPromo + 3;         // Q
                    Unsafe.Add(ref movePoolRef, localIndex + 1) = knightPromo + 2;     // R
                    Unsafe.Add(ref movePoolRef, localIndex + 2) = knightPromo;         // N
                    Unsafe.Add(ref movePoolRef, localIndex + 3) = knightPromo + 1;     // B

                    localIndex += 4;
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
    public static void GenerateCastlingMoves(ref int localIndex, Span<int> movePool, Position position, ref EvaluationContext evaluationContext)
    {
        // TODO: move to position?
        var castlingRights = position.Castle;

        if (castlingRights != default)
        {
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];

            if (position.Side == Side.White)
            {
                if ((castlingRights & (int)CastlingRights.WK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))
                {
                    movePool[localIndex++] = position.WhiteShortCastle;
                }

                if ((castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))

                {
                    movePool[localIndex++] = position.WhiteLongCastle;
                }
            }
            else
            {
                if ((castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                {
                    movePool[localIndex++] = position.BlackShortCastle;
                }

                if ((castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                {
                    movePool[localIndex++] = position.BlackLongCastle;
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

        var occupancy = position.OccupancyBitBoards[(int)Side.Both];
        ulong squaresNotOccupiedByUs = ~position.OccupancyBitBoards[(int)position.Side];

        var pieceAttacks = _pieceAttacks[piece];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = pieceAttacks(sourceSquare, occupancy)
                & squaresNotOccupiedByUs;

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare]);
            }
        }
    }

    /// <summary>
    /// Generate King moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateKingMoves(ref int localIndex, Span<Move> movePool, int piece, Position position, ref EvaluationContext evaluationContext)
    {
        var sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex();
        var occupancy = position.OccupancyBitBoards[(int)Side.Both];

        var attacks = _pieceAttacks[piece](sourceSquare, occupancy)
            & ~position.OccupancyBitBoards[(int)position.Side]
            & ~evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (attacks != default)
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);

            Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

            Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare]);
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen capture moves.
    /// Could also generate King captures, but <see cref="GenerateKingCaptures(ref int, Span{int}, int, Position)"/> is more efficient.
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePieceCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position)
    {
        var bitboard = position.PieceBitBoards[piece];
        var oppositeSide = Utils.OppositeSide((int)position.Side);

        var occupancy = position.OccupancyBitBoards[(int)Side.Both];
        var oppositeSidePieces = position.OccupancyBitBoards[oppositeSide];

        var pieceAttacks = _pieceAttacks[piece];

        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = pieceAttacks(sourceSquare, occupancy)
                & oppositeSidePieces;

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);
                var capturedPiece = position.Board[targetSquare];
                Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
            }
        }
    }

    /// <summary>
    /// Generate King capture moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GenerateKingCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position, ref EvaluationContext evaluationContext)
    {
        var sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex();
        var oppositeSide = Utils.OppositeSide((int)position.Side);

        var attacks = _pieceAttacks[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
            & position.OccupancyBitBoards[oppositeSide]
            & ~evaluationContext.AttacksBySide[oppositeSide];
        ref Move movePoolRef = ref MemoryMarshal.GetReference(movePool);

        while (attacks != default)
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);

            var capturedPiece = position.Board[targetSquare];
            Unsafe.Add(ref movePoolRef, localIndex++) = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
        }
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CanGenerateAtLeastAValidMove(Position position, ref EvaluationContext evaluationContext)
    {
        Debug.Assert(position.Side != Side.Both);

        var offset = Utils.PieceOffset((int)position.Side);

#if DEBUG
        try
        {
#endif
        return IsAnyPawnMoveValid(position, offset)
            || IsAnyKingMoveValid((int)Piece.K + offset, position, ref evaluationContext)    // in?
            || IsAnyPieceMoveValid((int)Piece.Q + offset, position)
            || IsAnyPieceMoveValid((int)Piece.B + offset, position)
            || IsAnyPieceMoveValid((int)Piece.N + offset, position)
            || IsAnyPieceMoveValid((int)Piece.R + offset, position)
            || IsAnyCastlingMoveValid(position, ref evaluationContext);
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
        var pawnPush = +8 - ((int)position.Side * 16);          // position.Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide((int)position.Side);   // position.Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = position.PieceBitBoards[piece];

        var occupancy = position.OccupancyBitBoards[(int)Side.Both];
        var oppositeSidePieces = position.OccupancyBitBoards[oppositeSide];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = (sourceSquare >> 3) + 1;

            Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {position.Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!occupancy.GetBit(singlePushSquare))
            {
                // Single pawn push
                var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    // If any of the promotions isn't valid, it means that the pawn move unveils a discovered check, or that the promoted piece doesn't stop an existing check in the 8th rank
                    // Therefore none of the other promotions will be valid either
                    if (IsValidMove(position, MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.Q + offset)))
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
            if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit((int)position.EnPassant)
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                && IsValidMove(position, MoveExtensions.EncodeEnPassant(sourceSquare, (int)position.EnPassant, piece))) // Could add here capturedPiece: (int)Piece.p - offset
            {
                return true;
            }

            // Captures
            var attackedSquares = attacks & oppositeSidePieces;
            while (attackedSquares != default)
            {
                attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                var capturedPiece = position.Board[targetSquare];

                var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    // If any of the promotions that capture the same piece isn't valid, it means that the pawn move unveils a discovered check, or that the capture doesn't stop an existing check in the 8th rank
                    // Therefore none of the other promotions capturing the same piece will be valid either
                    if (IsValidMove(position, MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.Q + offset)))
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
    public static bool IsAnyCastlingMoveValid(Position position, ref EvaluationContext evaluationContext)
    {
        // TODO: move to position?

        var castlingRights = position.Castle;

        if (castlingRights != default)
        {
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];

            if (position.Side == Side.White)
            {
                if ((castlingRights & (int)CastlingRights.WK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                    && IsValidMove(position, position.WhiteShortCastle))
                {
                    return true;
                }

                if ((castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                    && IsValidMove(position, position.WhiteLongCastle))
                {
                    return true;
                }
            }
            else
            {
                if ((castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
                    && IsValidMove(position, position.BlackShortCastle))
                {
                    return true;
                }

                if ((castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
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
        var bitboard = position.PieceBitBoards[piece];

        var occupancy = position.OccupancyBitBoards[(int)Side.Both];
        var squaresNotOccupiedByUs = ~position.OccupancyBitBoards[(int)position.Side];

        var pieceAttacks = _pieceAttacks[piece];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = pieceAttacks(sourceSquare, occupancy)
                & squaresNotOccupiedByUs;

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

                if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare])))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAnyKingMoveValid(int piece, Position position, ref EvaluationContext evaluationContext)
    {
        var sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex();
        var occupancy = position.OccupancyBitBoards[(int)Side.Both];

        var attacks = _pieceAttacks[piece](sourceSquare, occupancy)
            & ~position.OccupancyBitBoards[(int)position.Side]
            & ~evaluationContext.AttacksBySide[Utils.OppositeSide((int)position.Side)];

        while (attacks != default)
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);

            Debug.Assert(occupancy.GetBit(targetSquare) == (position.Board[targetSquare] != (int)Piece.None));

            if (IsValidMove(position, MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: position.Board[targetSquare])))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsValidMove(Position position, Move move)
    {
        var gameState = position.MakeMove(move, ref _gameState);

        bool result = position.WasProduceByAValidMove();
        position.UnmakeMove(move, gameState);

        return result;
    }
}
