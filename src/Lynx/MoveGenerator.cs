﻿using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx;

public static class MoveGenerator
{
#if DEBUG
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

    private const int TRUE = 1;

    public const int WhiteShortCastle = 147192768;
    public const int WhiteLongCastle = 281406400;
    public const int BlackShortCastle = 147527744;
    public const int BlackLongCastle = 281741376;

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

    internal static int Init() => TRUE;

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="capturesOnly">Filters out all moves but captures</param>
    /// <returns></returns>
    [Obsolete("dev and test only")]
    internal static Move[] GenerateAllMoves(Position position, bool capturesOnly = false)
    {
        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];

        return (capturesOnly
            ? GenerateAllCaptures(position, moves)
            : GenerateAllMoves(position, moves)).ToArray();
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="movePool"></param>
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
        GenerateCastlingMoves(ref localIndex, movePool, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.K + offset, position, offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position, offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position, offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position, offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position, offset);

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="movePool"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllMoves(Position position, Span<Move> movePool)
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
        GenerateCastlingMoves(ref localIndex, movePool, position);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.K + offset, position, offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.N + offset, position, offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.B + offset, position, offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.R + offset, position, offset);
        GenerateAllPieceMoves(ref localIndex, movePool, (int)Piece.Q + offset, position, offset);

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all psuedo-legal captures from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
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

        GeneratePawnCapturesAndPromotions(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.K + offset, position, offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset, position, offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset, position, offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset, position, offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset, position, offset);

        return movePool[..localIndex];
    }

    /// <summary>
    /// Generates all psuedo-legal captures from <paramref name="position"/>, ordered by <see cref="Move.Score(Position)"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="movePool"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<Move> GenerateAllCaptures(Position position, Span<Move> movePool)
    {
#if DEBUG
        if (position.Side == Side.Both)
        {
            return [];
        }
#endif

        int localIndex = 0;

        var offset = Utils.PieceOffset(position.Side);

        GeneratePawnCapturesAndPromotions(ref localIndex, movePool, position, offset);
        GenerateCastlingMoves(ref localIndex, movePool, position);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.K + offset, position, offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.N + offset, position, offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.B + offset, position, offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.R + offset, position, offset);
        GeneratePieceCaptures(ref localIndex, movePool, (int)Piece.Q + offset, position, offset);

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
                movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE, capturedPiece: (int)Piece.p - offset);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();
                var capturedPiece = FindCapturedPiece(position, offset, targetSquare);

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE, capturedPiece: capturedPiece);
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
                movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE, capturedPiece: (int)Piece.p - offset);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();
                var capturedPiece = FindCapturedPiece(position, offset, targetSquare);

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                }
                else
                {
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE, capturedPiece: capturedPiece);
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
    internal static void GenerateCastlingMoves(ref int localIndex, Span<Move> movePool, Position position)
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
                        $"Wrong hardcoded white short castle move, expected {WhiteShortCastle}, got {MoveExtensions.Encode(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(position.Side), isShortCastle: TRUE)}");
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
                        $"Wrong hardcoded white long castle move, expected {WhiteLongCastle}, got {MoveExtensions.Encode(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(position.Side), isLongCastle: TRUE)}");
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
                        $"Wrong hardcoded black short castle move, expected {BlackShortCastle}, got {MoveExtensions.Encode(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.K + Utils.PieceOffset(position.Side), isShortCastle: TRUE)}");
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
                        $"Wrong hardcoded black long castle move, expected {BlackLongCastle}, got {MoveExtensions.Encode(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.K + Utils.PieceOffset(position.Side), isLongCastle: TRUE)}");
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
    internal static void GenerateAllPieceMoves(ref int localIndex, Span<Move> movePool, int piece, Position position, int offset)
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
                    var capturedPiece = FindCapturedPiece(position, offset, targetSquare);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE, capturedPiece: capturedPiece);
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
    /// <param name="movePool"></param>
    /// <param name="piece"><see cref="Piece"/></param>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void GeneratePieceCaptures(ref int localIndex, Span<Move> movePool, int piece, Position position, int offset)
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
                    var capturedPiece = FindCapturedPiece(position, offset, targetSquare);
                    movePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE, capturedPiece: capturedPiece);
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
            Debug.Fail($"Error in {nameof(CanGenerateAtLeastAValidMove)}", e.StackTrace);
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
                && IsValidMove(position, MoveExtensions.Encode(true, sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE)))
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
                    if (IsValidMove(position, MoveExtensions.Encode(true, sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE))
                        || IsValidMove(position, MoveExtensions.Encode(true, sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE))
                        || IsValidMove(position, MoveExtensions.Encode(true, sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE))
                        || IsValidMove(position, MoveExtensions.Encode(true, sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE)))
                    {
                        return true;
                    }
                }
                else if (IsValidMove(position, MoveExtensions.Encode(true, sourceSquare, targetSquare, piece, isCapture: TRUE)))
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
                    && IsValidMove(position, MoveExtensions.Encode(true, sourceSquare, targetSquare, piece, isCapture: TRUE)))
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
        var gameState = position.MakeMoveCalculatingCapturedPiece(ref move);

#if DEBUG
        if (move.IsCapture())
        {
            Debug.Assert(move.IsCapture());
            Debug.Assert(move.CapturedPiece() != (int)Piece.None);
        }
        else
        {
            Debug.Assert(
                (int)Piece.None == move.CapturedPiece()
                || move.CapturedPiece() == 0);  // In case of CanGenerateAnyValidMoves() / IsValidMove() scenario, when we can't pre-populate with Piece.None since otherwise the | captured piece of MoveExtensions.EncodeCapturedPiece() wouldn't work
        }
#endif

        bool result = position.WasProduceByAValidMove();
        position.UnmakeMove(move, gameState);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindCapturedPiece(Position position, int offset, int targetSquare)
    {
        var start = (int)Piece.p - offset;
        for (int pieceIndex = start; pieceIndex < start + 5; ++pieceIndex)
        {
            if (position.PieceBitBoards[pieceIndex].GetBit(targetSquare))
            {
                return pieceIndex;
            }
        }

        throw new AssertException("No captured piece found");
    }

    #region Only for reference, but unused

    /// <summary>
    /// Sames as <see cref="GeneratePawnMoves(Position, int, bool)"/> but returning them
    /// </summary>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    /// <param name="capturesOnly"></param>
    /// <returns></returns>
    [Obsolete("Mostly for reference")]
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
                yield return MoveExtensions.Encode(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE, capturedPiece: (int)Piece.p - offset);
            }

            // Captures
            var attackedSquares = attacks & position.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();
                var capturedPiece = FindCapturedPiece(position, offset, targetSquare);

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                    yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                    yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                    yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE, capturedPiece: capturedPiece);
                }
                else
                {
                    yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE, capturedPiece: capturedPiece);
                }
            }
        }
    }

    [Obsolete("Mostly for reference")]
    internal static IEnumerable<Move> GenerateKingMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.K + offset, position, offset, capturesOnly);
    }

    [Obsolete("Mostly for reference")]
    internal static IEnumerable<Move> GenerateKnightMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.N + offset, position, offset, capturesOnly);
    }

    [Obsolete("Mostly for reference")]
    internal static IEnumerable<Move> GenerateBishopMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.B + offset, position, offset, capturesOnly);
    }

    [Obsolete("Mostly for reference")]
    internal static IEnumerable<Move> GenerateRookMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.R + offset, position, offset, capturesOnly);
    }

    [Obsolete("Mostly for reference")]
    internal static IEnumerable<Move> GenerateQueenMoves(Position position, bool capturesOnly = false)
    {
        var offset = Utils.PieceOffset(position.Side);

        return GeneratePieceMovesForReference((int)Piece.Q + offset, position, offset, capturesOnly);
    }

    /// <summary>
    /// Same as <see cref="GeneratePieceMoves(int, Position, bool)"/> but returning them
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    /// <param name="position"></param>
    /// <param name="capturesOnly"></param>
    /// <returns></returns>
    [Obsolete("Mostly for reference")]
    private static IEnumerable<Move> GeneratePieceMovesForReference(int piece, Position position, int offset, bool capturesOnly = false)
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
                    var capturedPiece = FindCapturedPiece(position, offset, targetSquare);
                    yield return MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE, capturedPiece: capturedPiece);
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
