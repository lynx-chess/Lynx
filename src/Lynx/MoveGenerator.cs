using Lynx.Model;
using NLog;
using System;
using System.Collections.Generic;

namespace Lynx
{
    public static class MoveGenerator
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private const int TRUE = 1;
        /// <summary>
        /// Checks are not considered
        /// </summary>
        private static readonly IReadOnlyDictionary<int, Func<int, BitBoard, ulong>> PieceAttacksDictionary = new Dictionary<int, Func<int, BitBoard, ulong>>
        {
            [(int)Piece.P] = (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White, origin].Board,
            [(int)Piece.p] = (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black, origin].Board,

            [(int)Piece.K] = (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,
            [(int)Piece.k] = (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,

            [(int)Piece.N] = (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,
            [(int)Piece.n] = (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,

            [(int)Piece.B] = (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,
            [(int)Piece.b] = (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,

            [(int)Piece.R] = (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,
            [(int)Piece.r] = (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,

            // TODO try to improve performance by re-using bishop and rook attacks
            [(int)Piece.Q] = (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
            [(int)Piece.q] = (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
        };

        public static List<Move> GenerateAllMoves(Position position, bool capturesOnly = false)
        {
            var moves = new List<Move>(150);

            if (position.Side == Side.Both)
            {
                return moves;
            }

            var offset = Utils.PieceOffset(position.Side);

            moves.AddRange(GeneratePawnMoves(position, offset, capturesOnly));
            moves.AddRange(GenerateCastlingMoves(position, offset));
            moves.AddRange(GeneratePieceMoves((int)Piece.K + offset, position, capturesOnly));
            moves.AddRange(GeneratePieceMoves((int)Piece.N + offset, position, capturesOnly));
            moves.AddRange(GeneratePieceMoves((int)Piece.B + offset, position, capturesOnly));
            moves.AddRange(GeneratePieceMoves((int)Piece.R + offset, position, capturesOnly));
            moves.AddRange(GeneratePieceMoves((int)Piece.Q + offset, position, capturesOnly));

            return moves;
        }

        internal static IEnumerable<Move> GeneratePawnMoves(Position position, int offset, bool capturesOnly = false)
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
                if (sourceRank == 1 || sourceRank == 8)
                {
                    _logger.Warn($"There's a non-promoted {position.Side} pawn in rank {sourceRank}");
                    continue;
                }

                // Pawn pushes
                var singlePushSquare = sourceSquare + pawnPush;
                if (!position.OccupancyBitBoards[2].GetBit(singlePushSquare))
                {
                    // Single pawn push
                    var targetRank = (singlePushSquare / 8) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Promotion
                    {
                        yield return new Move(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.Q + offset);
                        yield return new Move(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.R + offset);
                        yield return new Move(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.N + offset);
                        yield return new Move(sourceSquare, singlePushSquare, piece, promotedPiece: (int)Piece.B + offset);
                    }
                    else if (!capturesOnly)
                    {
                        yield return new Move(sourceSquare, singlePushSquare, piece);
                    }

                    // Double pawn push
                    // Inside of the if because singlePush square cannot be occupied either
                    if (!capturesOnly)
                    {
                        var doublePushSquare = sourceSquare + (2 * pawnPush);
                        if (!position.OccupancyBitBoards[2].GetBit(doublePushSquare)
                            && ((sourceRank == 2 && position.Side == Side.Black) || (sourceRank == 7 && position.Side == Side.White)))
                        {
                            yield return new Move(sourceSquare, doublePushSquare, piece, isDoublePawnPush: TRUE);
                        }
                    }
                }

                var attacks = Attacks.PawnAttacks[(int)position.Side, sourceSquare];

                // En passant
                if (position.EnPassant != BoardSquare.noSquare && attacks.GetBit(position.EnPassant))
                // We assume that position.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush) == true
                {
                    yield return new Move(sourceSquare, (int)position.EnPassant, piece, isCapture: TRUE, isEnPassant: TRUE);
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
                        yield return new Move(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.Q + offset, isCapture: TRUE);
                        yield return new Move(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.R + offset, isCapture: TRUE);
                        yield return new Move(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.N + offset, isCapture: TRUE);
                        yield return new Move(sourceSquare, targetSquare, piece, promotedPiece: (int)Piece.B + offset, isCapture: TRUE);
                    }
                    else
                    {
                        yield return new Move(sourceSquare, targetSquare, piece, isCapture: TRUE);
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
        internal static IEnumerable<Move> GenerateCastlingMoves(Position position, int offset)
        {
            var piece = (int)Piece.K + offset;
            var oppositeSide = (Side)Utils.OppositeSide(position.Side);

            int sourceSquare = position.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

            // Castles
            if (position.Castle != default)
            {
                if (position.Side == Side.White)
                {
                    if (((position.Castle & (int)CastlingRights.WK) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g1)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.e1, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f1, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g1, position, oppositeSide))
                    {
                        yield return new Move(sourceSquare, Constants.WhiteShortCastleKingSquare, piece, isShortCastle: TRUE);
                    }

                    if (((position.Castle & (int)CastlingRights.WQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c1)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b1)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.e1, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d1, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c1, position, oppositeSide))
                    {
                        yield return new Move(sourceSquare, Constants.WhiteLongCastleKingSquare, piece, isLongCastle: TRUE);
                    }
                }
                else
                {
                    if (((position.Castle & (int)CastlingRights.BK) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.f8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.g8)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.e8, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.f8, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.g8, position, oppositeSide))
                    {
                        yield return new Move(sourceSquare, Constants.BlackShortCastleKingSquare, piece, isShortCastle: TRUE);
                    }

                    if (((position.Castle & (int)CastlingRights.BQ) != default)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.d8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.c8)
                        && !position.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquare.b8)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.e8, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.d8, position, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquare.c8, position, oppositeSide))
                    {
                        yield return new Move(sourceSquare, Constants.BlackLongCastleKingSquare, piece, isLongCastle: TRUE);
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
        internal static IEnumerable<Move> GeneratePieceMoves(int piece, Position position, bool capturesOnly = false)
        {
            var bitboard = position.PieceBitBoards[piece].Board;
            int sourceSquare, targetSquare;

            while (bitboard != default)
            {
                sourceSquare = BitBoard.GetLS1BIndex(bitboard);
                bitboard = BitBoard.ResetLS1B(bitboard);

                ulong attacks = PieceAttacksDictionary[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both])
                    & ~position.OccupancyBitBoards[(int)position.Side].Board;

                while (attacks != default)
                {
                    targetSquare = BitBoard.GetLS1BIndex(attacks);
                    attacks = BitBoard.ResetLS1B(attacks);

                    if (position.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare))
                    {
                        yield return new Move(sourceSquare, targetSquare, piece, isCapture: TRUE);
                    }
                    else if (!capturesOnly)
                    {
                        yield return new Move(sourceSquare, targetSquare, piece);
                    }
                }
            }
        }

        internal static IEnumerable<Move> GenerateKingMoves(Position position, bool capturesOnly = false)
        {
            var offset = Utils.PieceOffset(position.Side);

            return GeneratePieceMoves((int)Piece.K + offset, position, capturesOnly);
        }

        internal static IEnumerable<Move> GenerateKnightMoves(Position position, bool capturesOnly = false)
        {
            var offset = Utils.PieceOffset(position.Side);

            return GeneratePieceMoves((int)Piece.N + offset, position, capturesOnly);
        }

        internal static IEnumerable<Move> GenerateBishopMoves(Position position, bool capturesOnly = false)
        {
            var offset = Utils.PieceOffset(position.Side);

            return GeneratePieceMoves((int)Piece.B + offset, position, capturesOnly);
        }

        internal static IEnumerable<Move> GenerateRookMoves(Position position, bool capturesOnly = false)
        {
            var offset = Utils.PieceOffset(position.Side);

            return GeneratePieceMoves((int)Piece.R + offset, position, capturesOnly);
        }

        internal static IEnumerable<Move> GenerateQueenMoves(Position position, bool capturesOnly = false)
        {
            var offset = Utils.PieceOffset(position.Side);

            return GeneratePieceMoves((int)Piece.Q + offset, position, capturesOnly);
        }
    }
}
