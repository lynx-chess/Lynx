using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

public partial class Position
{
#if DEBUG
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

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
    internal Move[] GenerateAllMoves(bool capturesOnly = false)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        return (capturesOnly
            ? GenerateAllCaptures(ref evaluationContext, moves)
            : GenerateAllMoves(ref evaluationContext, moves)).ToArray();
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<Move> GenerateAllMoves(ref EvaluationContext evaluationContext, Span<Move> movePool)
    {
        Debug.Assert(Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset(Side);

        GenerateAllPawnMoves(ref localIndex, movePool, offset);
        GenerateCastlingMoves(ref localIndex, movePool, ref evaluationContext);
        GenerateKingMoves(ref localIndex, movePool, (int)Piece.K + offset, ref evaluationContext);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.N + offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.B + offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.R + offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset);

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all psuedo-legal captures from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<Move> GenerateAllCaptures(ref EvaluationContext evaluationContext, Span<Move> movePool)
    {
        Debug.Assert(Side != Side.Both);

        int localIndex = 0;

        var offset = Utils.PieceOffset(Side);

        GeneratePawnCapturesAndPromotions(ref localIndex, movePool, offset);
        GenerateCastlingMoves(ref localIndex, movePool, ref evaluationContext);
        GenerateKingCaptures(ref localIndex, movePool, (int)Piece.K + offset, ref evaluationContext);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset);

        return movePool[..localIndex];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GenerateAllPawnMoves(ref int localIndex, Span<Move> movePool, int offset)
    {
        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)Side * 16);          // Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide(Side);   // Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = _pieceBitBoards[piece];

        var pawnAttacks = Attacks.PawnAttacks[(int)Side];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = (sourceSquare >> 3) + 1;

            Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!occupancy.GetBit(singlePushSquare))
            {
                // Single pawn push
                var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N + offset);

                    movePool[localIndex] = knightPromo + 3;         // Q
                    movePool[localIndex + 1] = knightPromo + 2;     // R
                    movePool[localIndex + 2] = knightPromo;         // N
                    movePool[localIndex + 3] = knightPromo + 1;     // B

                    localIndex += 4;
                }
                else
                {
                    movePool[localIndex++] = singlePawnPush;

                    // Double pawn push
                    // Inside of the single pawn push if because singlePush square cannot be occupied either
                    if ((sourceRank == 2)        // Side == Side.Black is always true, otherwise targetRank would be 1
                        || (sourceRank == 7))    // Side == Side.White is always true, otherwise targetRank would be 8
                    {
                        var doublePushSquare = singlePushSquare + pawnPush;

                        if (!occupancy.GetBit(doublePushSquare))
                        {
                            movePool[localIndex++] = MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece);
                        }
                    }
                }
            }

            var attacks = pawnAttacks[sourceSquare];

            // En passant
            if (EnPassant != BoardSquare.noSquare && attacks.GetBit(EnPassant))
            // We assume that _occupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                movePool[localIndex++] = MoveExtensions.EncodeEnPassant(sourceSquare, (int)EnPassant, piece, capturedPiece: (int)Piece.p - offset);
            }

            // Captures
            var attackedSquares = attacks & _occupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                var capturedPiece = Board[targetSquare];

                var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N + offset);

                    movePool[localIndex] = knightPromo + 3;         // Q
                    movePool[localIndex + 1] = knightPromo + 2;     // R
                    movePool[localIndex + 2] = knightPromo;         // N
                    movePool[localIndex + 3] = knightPromo + 1;     // B;

                    localIndex += 4;
                }
                else
                {
                    movePool[localIndex++] = pawnCapture;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GeneratePawnCapturesAndPromotions(ref int localIndex, Span<Move> movePool, int offset)
    {
        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)Side * 16);          // Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide(Side);   // Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = _pieceBitBoards[piece];

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var oppositeSidePieces = _occupancyBitBoards[oppositeSide];

        var pawnAttacks = Attacks.PawnAttacks[(int)Side];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = (sourceSquare >> 3) + 1;

            Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {Side} pawn in rank {sourceRank})");

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!occupancy.GetBit(singlePushSquare))
            {
                // Single pawn push
                var singlePawnPush = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);

                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.N + offset);

                    movePool[localIndex] = knightPromo + 3;         // Q
                    movePool[localIndex + 1] = knightPromo + 2;     // R
                    movePool[localIndex + 2] = knightPromo;         // N
                    movePool[localIndex + 3] = knightPromo + 1;     // B

                    localIndex += 4;
                }
            }

            var attacks = pawnAttacks[sourceSquare];

            // En passant
            if (EnPassant != BoardSquare.noSquare && attacks.GetBit(EnPassant))
            // We assume that _occupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                movePool[localIndex++] = MoveExtensions.EncodeEnPassant(sourceSquare, (int)EnPassant, piece, capturedPiece: (int)Piece.p - offset);
            }

            // Captures
            var attackedSquares = attacks & oppositeSidePieces;
            while (attackedSquares != default)
            {
                attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                var capturedPiece = Board[targetSquare];

                var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    var knightPromo = MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.N + offset);

                    movePool[localIndex] = knightPromo + 3;         // Q
                    movePool[localIndex + 1] = knightPromo + 2;     // R
                    movePool[localIndex + 2] = knightPromo;         // N
                    movePool[localIndex + 3] = knightPromo + 1;     // B

                    localIndex += 4;
                }
                else
                {
                    movePool[localIndex++] = pawnCapture;
                }
            }
        }
    }

    /// <summary>
    /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
    /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void GenerateCastlingMoves(ref int localIndex, Span<int> movePool, ref EvaluationContext evaluationContext)
    {
        var castlingRights = Castle;

        if (castlingRights != default)
        {
            var occupancy = _occupancyBitBoards[(int)Side.Both];

            if (Side == Side.White)
            {
                if ((castlingRights & (int)CastlingRights.WK) != default
                    && (occupancy & KingsideCastlingFreeSquares[(int)Side.White]) == 0
                    && !AreSquaresAttacked(KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))
                {
                    movePool[localIndex++] = WhiteShortCastle;
                }

                if ((castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & QueensideCastlingFreeSquares[(int)Side.White]) == 0
                    && !AreSquaresAttacked(QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))

                {
                    movePool[localIndex++] = WhiteLongCastle;
                }
            }
            else
            {
                if ((castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !AreSquaresAttacked(KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                {
                    movePool[localIndex++] = BlackShortCastle;
                }

                if ((castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !AreSquaresAttacked(QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                {
                    movePool[localIndex++] = BlackLongCastle;
                }
            }
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GenerateAllPieceMoves(ref int localIndex, Span<Move> movePool, int piece)
    {
        var bitboard = _pieceBitBoards[piece];

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        ulong squaresNotOccupiedByUs = ~_occupancyBitBoards[(int)Side];

        var pieceAttacks = _pieceAttacks[piece];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = pieceAttacks(sourceSquare, occupancy)
                & squaresNotOccupiedByUs;

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (Board[targetSquare] != (int)Piece.None));

                movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: Board[targetSquare]);
            }
        }
    }

    /// <summary>
    /// Generate King moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GenerateKingMoves(ref int localIndex, Span<Move> movePool, int piece, ref EvaluationContext evaluationContext)
    {
        var sourceSquare = _pieceBitBoards[piece].GetLS1BIndex();
        var occupancy = _occupancyBitBoards[(int)Side.Both];

        var attacks = _pieceAttacks[piece](sourceSquare, occupancy)
            & ~_occupancyBitBoards[(int)Side]
            & ~evaluationContext.AttacksBySide[Utils.OppositeSide(Side)];

        while (attacks != default)
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);

            Debug.Assert(occupancy.GetBit(targetSquare) == (Board[targetSquare] != (int)Piece.None));

            movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: Board[targetSquare]);
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen capture moves.
    /// Could also generate King captures, but <see cref="GenerateKingCaptures(ref int, Span{int}, int)"/> is more efficient.
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GeneratePieceCaptures(ref int localIndex, Span<Move> movePool, int piece)
    {
        var bitboard = _pieceBitBoards[piece];
        var oppositeSide = Utils.OppositeSide(Side);

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var oppositeSidePieces = _occupancyBitBoards[oppositeSide];

        var pieceAttacks = _pieceAttacks[piece];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = pieceAttacks(sourceSquare, occupancy)
                & oppositeSidePieces;

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);
                var capturedPiece = Board[targetSquare];
                movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
            }
        }
    }

    /// <summary>
    /// Generate King capture moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GenerateKingCaptures(ref int localIndex, Span<Move> movePool, int piece, ref EvaluationContext evaluationContext)
    {
        var sourceSquare = _pieceBitBoards[piece].GetLS1BIndex();
        var oppositeSide = Utils.OppositeSide(Side);

        var attacks = _pieceAttacks[piece](sourceSquare, _occupancyBitBoards[(int)Side.Both])
            & _occupancyBitBoards[oppositeSide]
            & ~evaluationContext.AttacksBySide[oppositeSide];

        while (attacks != default)
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);

            var capturedPiece = Board[targetSquare];
            movePool[localIndex++] = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);
        }
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanGenerateAtLeastAValidMove(ref EvaluationContext evaluationContext)
    {
        Debug.Assert(Side != Side.Both);

        var offset = Utils.PieceOffset(Side);

#if DEBUG
        try
        {
#endif
            return IsAnyPawnMoveValid(offset)
                || IsAnyKingMoveValid((int)Piece.K + offset, ref evaluationContext)    // in?
                || IsAnyPieceMoveValid((int)Piece.Q + offset)
                || IsAnyPieceMoveValid((int)Piece.B + offset)
                || IsAnyPieceMoveValid((int)Piece.N + offset)
                || IsAnyPieceMoveValid((int)Piece.R + offset)
                || IsAnyCastlingMoveValid(ref evaluationContext);
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
    private bool IsAnyPawnMoveValid(int offset)
    {
        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)Side * 16);          // Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide(Side);   // Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = _pieceBitBoards[piece];

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var oppositeSidePieces = _occupancyBitBoards[oppositeSide];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var sourceRank = (sourceSquare >> 3) + 1;

            Debug.Assert(sourceRank != 1 && sourceRank != 8, $"There's a non-promoted {Side} pawn in rank {sourceRank})");

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
                    if (IsValidMove(MoveExtensions.EncodePromotionFromPawnMove(singlePawnPush, promotedPiece: (int)Piece.Q + offset)))
                    {
                        return true;
                    }
                }
                else
                {
                    if (IsValidMove(singlePawnPush))
                    {
                        return true;
                    }

                    // Double pawn push
                    // Inside of the if because singlePush square cannot be occupied either
                    var doublePushSquare = singlePushSquare + pawnPush;
                    if (!occupancy.GetBit(doublePushSquare)
                        && (sourceRank == 2         // Side == Side.Black is always true, otherwise targetRank would be 1
                            || sourceRank == 7)     // Side == Side.White is always true, otherwise targetRank would be 8
                        && IsValidMove(MoveExtensions.EncodeDoublePawnPush(sourceSquare, doublePushSquare, piece)))
                    {
                        return true;
                    }
                }
            }

            var attacks = Attacks.PawnAttacks[(int)Side][sourceSquare];

            // En passant
            if (EnPassant != BoardSquare.noSquare && attacks.GetBit(EnPassant)
                // We assume that _occupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                && IsValidMove(MoveExtensions.EncodeEnPassant(sourceSquare, (int)EnPassant, piece))) // Could add here capturedPiece: (int)Piece.p - offset
            {
                return true;
            }

            // Captures
            var attackedSquares = attacks & oppositeSidePieces;
            while (attackedSquares != default)
            {
                attackedSquares = attackedSquares.WithoutLS1B(out int targetSquare);
                var capturedPiece = Board[targetSquare];

                var pawnCapture = MoveExtensions.EncodeCapture(sourceSquare, targetSquare, piece, capturedPiece);

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    // If any of the promotions that capture the same piece isn't valid, it means that the pawn move unveils a discovered check, or that the capture doesn't stop an existing check in the 8th rank
                    // Therefore none of the other promotions capturing the same piece will be valid either
                    if (IsValidMove(MoveExtensions.EncodePromotionFromPawnMove(pawnCapture, promotedPiece: (int)Piece.Q + offset)))
                    {
                        return true;
                    }
                }
                else if (IsValidMove(pawnCapture))
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
    public bool IsAnyCastlingMoveValid(ref EvaluationContext evaluationContext)
    {
        var castlingRights = Castle;

        if (castlingRights != default)
        {
            var occupancy = _occupancyBitBoards[(int)Side.Both];

            if (Side == Side.White)
            {
                if ((castlingRights & (int)CastlingRights.WK) != default
                    && (occupancy & KingsideCastlingFreeSquares[(int)Side.White]) == 0
                    && !AreSquaresAttacked(KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                    && IsValidMove(WhiteShortCastle))
                {
                    return true;
                }

                if ((castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & QueensideCastlingFreeSquares[(int)Side.White]) == 0
                    && !AreSquaresAttacked(QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                    && IsValidMove(WhiteLongCastle))
                {
                    return true;
                }
            }
            else
            {
                if ((castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !AreSquaresAttacked(KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
                    && IsValidMove(BlackShortCastle))
                {
                    return true;
                }

                if ((castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !AreSquaresAttacked(QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
                    && IsValidMove(BlackLongCastle))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Also valid for Kings, but less performant than <see cref="IsAnyKingMoveValid(int)"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAnyPieceMoveValid(int piece)
    {
        var bitboard = _pieceBitBoards[piece];

        var occupancy = _occupancyBitBoards[(int)Side.Both];
        var squaresNotOccupiedByUs = ~_occupancyBitBoards[(int)Side];

        var pieceAttacks = _pieceAttacks[piece];

        while (bitboard != default)
        {
            bitboard = bitboard.WithoutLS1B(out int sourceSquare);

            var attacks = pieceAttacks(sourceSquare, occupancy)
                & squaresNotOccupiedByUs;

            while (attacks != default)
            {
                attacks = attacks.WithoutLS1B(out int targetSquare);

                Debug.Assert(occupancy.GetBit(targetSquare) == (Board[targetSquare] != (int)Piece.None));

                if (IsValidMove(MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: Board[targetSquare])))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAnyKingMoveValid(int piece, ref EvaluationContext evaluationContext)
    {
        var sourceSquare = _pieceBitBoards[piece].GetLS1BIndex();
        var occupancy = _occupancyBitBoards[(int)Side.Both];

        var attacks = _pieceAttacks[piece](sourceSquare, occupancy)
            & ~_occupancyBitBoards[(int)Side]
            & ~evaluationContext.AttacksBySide[Utils.OppositeSide(Side)];

        while (attacks != default)
        {
            attacks = attacks.WithoutLS1B(out var targetSquare);

            Debug.Assert(occupancy.GetBit(targetSquare) == (Board[targetSquare] != (int)Piece.None));

            if (IsValidMove(MoveExtensions.Encode(sourceSquare, targetSquare, piece, capturedPiece: Board[targetSquare])))
            {
                return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsValidMove(Move move)
    {
        var gameState = MakeMove(move);

        bool result = WasProduceByAValidMove();
        UnmakeMove(move, gameState);

        return result;
    }
}
