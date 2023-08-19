using Lynx.Model;
using NLog;
using System.Runtime.CompilerServices;
namespace Lynx;

using static MoveGenerator;

public sealed partial class Engine
{
    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="Game.CurrentPosition"/>, ordered by <see cref="Move.Score(Game.CurrentPosition)"/>
    /// </summary>
    /// <param name="Game.CurrentPosition"></param>
    /// <param name="capturesOnly">Filters out all moves but captures</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<Move> GenerateAllMoves(bool capturesOnly = false)
    {
#if DEBUG
        if (Game.CurrentPosition.Side == Side.Both)
        {
            return new List<Move>();
        }
#endif

        int localIndex = 0;

        var offset = Utils.PieceOffset(Game.CurrentPosition.Side);

        GeneratePawnMoves(ref localIndex, offset, capturesOnly);
        GenerateCastlingMoves(ref localIndex, offset);
        GeneratePieceMoves(ref localIndex, (int)Piece.K + offset, capturesOnly);
        GeneratePieceMoves(ref localIndex, (int)Piece.N + offset, capturesOnly);
        GeneratePieceMoves(ref localIndex, (int)Piece.B + offset, capturesOnly);
        GeneratePieceMoves(ref localIndex, (int)Piece.R + offset, capturesOnly);
        GeneratePieceMoves(ref localIndex, (int)Piece.Q + offset, capturesOnly);

        return Game.MovePool.Take(localIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void GeneratePawnMoves(ref int localIndex, int offset, bool capturesOnly = false)
    {
        int sourceSquare, targetSquare;

        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)Game.CurrentPosition.Side * 16);          // Game.CurrentPosition.Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide(Game.CurrentPosition.Side);   // Game.CurrentPosition.Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = Game.CurrentPosition.PieceBitBoards[piece];

        while (bitboard != default)
        {
            sourceSquare = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            var sourceRank = (sourceSquare >> 3) + 1;

#if DEBUG
            if (sourceRank == 1 || sourceRank == 8)
            {
                _logger.Warn("There's a non-promoted {0} pawn in rank {1}", Game.CurrentPosition.Side, sourceRank);
                continue;
            }
#endif

            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!Game.CurrentPosition.OccupancyBitBoards[2].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
                }
                else if (!capturesOnly)
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, singlePushSquare, piece);
                }

                // Double pawn push
                // Inside of the if because singlePush square cannot be occupied either
                if (!capturesOnly)
                {
                    var doublePushSquare = sourceSquare + (2 * pawnPush);
                    if (!Game.CurrentPosition.OccupancyBitBoards[2].GetBit(doublePushSquare)
                        && ((sourceRank == 2 && Game.CurrentPosition.Side == Side.Black) || (sourceRank == 7 && Game.CurrentPosition.Side == Side.White)))
                    {
                        Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE);
                    }
                }
            }

            var attacks = Attacks.PawnAttacks[(int)Game.CurrentPosition.Side, sourceSquare];

            // En passant
            if (Game.CurrentPosition.EnPassant != BoardSquare.noSquare && attacks.GetBit(Game.CurrentPosition.EnPassant))
            // We assume that Game.CurrentPosition.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
            {
                Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, (int)Game.CurrentPosition.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE);
            }

            // Captures
            var attackedSquares = attacks & Game.CurrentPosition.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE);
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE);
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE);
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE);
                }
                else
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                }
            }
        }
    }

    /// <summary>
    /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
    /// see FEN Game.CurrentPosition "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
    /// </summary>
    /// <param name="Game.CurrentPosition"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void GenerateCastlingMoves(ref int localIndex, int offset)
    {
        var piece = (int)Piece.K + offset;
        var oppositeSide = (Side)Utils.OppositeSide(Game.CurrentPosition.Side);

        int sourceSquare = Game.CurrentPosition.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

        // Castles
        if (Game.CurrentPosition.Castle != default)
        {
            if (Game.CurrentPosition.Side == Side.White)
            {
                bool ise1Attacked = Attacks.IsSquaredAttackedBySide((int)BoardSquare.e1, Game.CurrentPosition, oppositeSide);
                if (((Game.CurrentPosition.Castle & (int)CastlingRights.WK) != default)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f1, Game.CurrentPosition, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g1, Game.CurrentPosition, oppositeSide))
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((Game.CurrentPosition.Castle & (int)CastlingRights.WQ) != default)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d1, Game.CurrentPosition, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c1, Game.CurrentPosition, oppositeSide))
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE);
                }
            }
            else
            {
                bool ise8Attacked = Attacks.IsSquaredAttackedBySide((int)BoardSquare.e8, Game.CurrentPosition, oppositeSide);
                if (((Game.CurrentPosition.Castle & (int)CastlingRights.BK) != default)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f8, Game.CurrentPosition, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g8, Game.CurrentPosition, oppositeSide))
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE);
                }

                if (((Game.CurrentPosition.Castle & (int)CastlingRights.BQ) != default)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d8, Game.CurrentPosition, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c8, Game.CurrentPosition, oppositeSide))
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE);
                }
            }
        }
    }

    /// <summary>
    /// Generate Knight, Bishop, Rook and Queen moves
    /// </summary>
    /// <param name="piece"><see cref="Piece"/></param>
    /// <param name="Game.CurrentPosition"></param>
    /// <param name="capturesOnly"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void GeneratePieceMoves(ref int localIndex, int piece, bool capturesOnly = false)
    {
        var bitboard = Game.CurrentPosition.PieceBitBoards[piece];
        int sourceSquare, targetSquare;

        while (bitboard != default)
        {
            sourceSquare = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            var attacks = PieceAttacks[piece](sourceSquare, Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both])
                & ~Game.CurrentPosition.OccupancyBitBoards[(int)Game.CurrentPosition.Side];

            while (attacks != default)
            {
                targetSquare = attacks.GetLS1BIndex();
                attacks.ResetLS1B();

                if (Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE);
                }
                else if (!capturesOnly)
                {
                    Game.MovePool[localIndex++] = MoveExtensions.Encode(sourceSquare, targetSquare, piece);
                }
            }
        }
    }

    /// <summary>
    /// Generates all psuedo-legal moves from <paramref name="Game.CurrentPosition"/>, ordered by <see cref="Move.Score(Game.CurrentPosition)"/>
    /// </summary>
    /// <param name="Game.CurrentPosition"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsThereAnyValidMove()
    {
#if DEBUG
        if (Game.CurrentPosition.Side == Side.Both)
        {
            return false;
        }
#endif

        var offset = Utils.PieceOffset(Game.CurrentPosition.Side);

        return IsAnyPawnMoveValid(offset)
            || IsAnyPieceMoveValid((int)Piece.K + offset)
            || IsAnyPieceMoveValid((int)Piece.Q + offset)
            || IsAnyPieceMoveValid((int)Piece.B + offset)
            || IsAnyPieceMoveValid((int)Piece.N + offset)
            || IsAnyPieceMoveValid((int)Piece.R + offset)
            || IsAnyCastlingMoveValid(offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAnyPawnMoveValid(int offset)
    {
        int sourceSquare, targetSquare;

        var piece = (int)Piece.P + offset;
        var pawnPush = +8 - ((int)Game.CurrentPosition.Side * 16);          // Game.CurrentPosition.Side == Side.White ? -8 : +8
        int oppositeSide = Utils.OppositeSide(Game.CurrentPosition.Side);   // Game.CurrentPosition.Side == Side.White ? (int)Side.Black : (int)Side.White
        var bitboard = Game.CurrentPosition.PieceBitBoards[piece];

        while (bitboard != default)
        {
            sourceSquare = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            var sourceRank = (sourceSquare >> 3) + 1;

#if DEBUG
            if (sourceRank == 1 || sourceRank == 8)
            {
                _logger.Warn("There's a non-promoted {0} pawn in rank {1}", Game.CurrentPosition.Side, sourceRank);
                continue;
            }
#endif
            // Pawn pushes
            var singlePushSquare = sourceSquare + pawnPush;
            if (!Game.CurrentPosition.OccupancyBitBoards[2].GetBit(singlePushSquare))
            {
                // Single pawn push
                var targetRank = (singlePushSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Promotion
                {
                    if (IsValidMove(MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset))
                        || IsValidMove(MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset))
                        || IsValidMove(MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset))
                        || IsValidMove(MoveExtensions.Encode(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset)))
                    {
                        return true;
                    }
                }
                else if (IsValidMove(MoveExtensions.Encode(sourceSquare, singlePushSquare, piece)))
                {
                    return true;
                }

                // Double pawn push
                // Inside of the if because singlePush square cannot be occupied either

                var doublePushSquare = sourceSquare + (2 * pawnPush);
                if (!Game.CurrentPosition.OccupancyBitBoards[2].GetBit(doublePushSquare)
                    && ((sourceRank == 2 && Game.CurrentPosition.Side == Side.Black) || (sourceRank == 7 && Game.CurrentPosition.Side == Side.White))
                    && IsValidMove(MoveExtensions.Encode(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE)))
                {
                    return true;
                }
            }

            var attacks = Attacks.PawnAttacks[(int)Game.CurrentPosition.Side, sourceSquare];

            // En passant
            if (Game.CurrentPosition.EnPassant != BoardSquare.noSquare && attacks.GetBit(Game.CurrentPosition.EnPassant)
            // We assume that Game.CurrentPosition.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                && IsValidMove(MoveExtensions.Encode(sourceSquare, (int)Game.CurrentPosition.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE)))
            {
                return true;
            }

            // Captures
            var attackedSquares = attacks & Game.CurrentPosition.OccupancyBitBoards[oppositeSide];
            while (attackedSquares != default)
            {
                targetSquare = attackedSquares.GetLS1BIndex();
                attackedSquares.ResetLS1B();

                var targetRank = (targetSquare >> 3) + 1;
                if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                {
                    if (IsValidMove(MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE))
                        || IsValidMove(MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE))
                        || IsValidMove(MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE))
                        || IsValidMove(MoveExtensions.Encode(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE)))
                    {
                        return true;
                    }
                }
                else if (IsValidMove(MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
    /// see FEN Game.CurrentPosition "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
    /// </summary>
    /// <param name="Game.CurrentPosition"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAnyCastlingMoveValid(int offset)
    {
        var piece = (int)Piece.K + offset;
        var oppositeSide = (Side)Utils.OppositeSide(Game.CurrentPosition.Side);

        int sourceSquare = Game.CurrentPosition.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

        // Castles
        if (Game.CurrentPosition.Castle != default)
        {
            if (Game.CurrentPosition.Side == Side.White)
            {
                bool ise1Attacked = Attacks.IsSquaredAttackedBySide((int)BoardSquare.e1, Game.CurrentPosition, oppositeSide);
                if (((Game.CurrentPosition.Castle & (int)CastlingRights.WK) != default)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                    && !ise1Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f1, Game.CurrentPosition, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g1, Game.CurrentPosition, oppositeSide)
                    && IsValidMove(MoveExtensions.Encode(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE)))
                {
                    return true;
                }

                if (((Game.CurrentPosition.Castle & (int)CastlingRights.WQ) != default)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                    && !ise1Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d1, Game.CurrentPosition, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c1, Game.CurrentPosition, oppositeSide)
                    && IsValidMove(MoveExtensions.Encode(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE)))
                {
                    return true;
                }
            }
            else
            {
                bool ise8Attacked = Attacks.IsSquaredAttackedBySide((int)BoardSquare.e8, Game.CurrentPosition, oppositeSide);
                if (((Game.CurrentPosition.Castle & (int)CastlingRights.BK) != default)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                    && !ise8Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f8, Game.CurrentPosition, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g8, Game.CurrentPosition, oppositeSide)
                    && IsValidMove(MoveExtensions.Encode(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE)))
                {
                    return true;
                }

                if (((Game.CurrentPosition.Castle & (int)CastlingRights.BQ) != default)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                    && !Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                    && !ise8Attacked
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d8, Game.CurrentPosition, oppositeSide)
                    && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c8, Game.CurrentPosition, oppositeSide)
                    && IsValidMove(MoveExtensions.Encode(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE)))
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
    /// <param name="Game.CurrentPosition"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAnyPieceMoveValid(int piece)
    {
        var bitboard = Game.CurrentPosition.PieceBitBoards[piece];
        int sourceSquare, targetSquare;

        while (bitboard != default)
        {
            sourceSquare = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            var attacks = PieceAttacks[piece](sourceSquare, Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both])
                & ~Game.CurrentPosition.OccupancyBitBoards[(int)Game.CurrentPosition.Side];

            while (attacks != default)
            {
                targetSquare = attacks.GetLS1BIndex();
                attacks.ResetLS1B();

                if (Game.CurrentPosition.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare)
                    && IsValidMove(MoveExtensions.Encode(sourceSquare, targetSquare, piece, isCapture: TRUE)))
                {
                    return true;
                }
                else if (IsValidMove(MoveExtensions.Encode(sourceSquare, targetSquare, piece)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsValidMove(Move move)
    {
        var gameState = Game.CurrentPosition.MakeMove(move);
        bool result = Game.CurrentPosition.WasProduceByAValidMove();
        Game.CurrentPosition.UnmakeMove(move, gameState);

        return result;
    }
}
