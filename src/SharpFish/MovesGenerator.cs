using SharpFish.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFish
{
    public static class MovesGenerator
    {
        //private static readonly IReadOnlyDictionary<int, Func<int, BitBoard, ulong>> PieceAttacksDictionary = new Dictionary<int, Func<int, BitBoard, ulong>>
        //{
        //    [(int)Piece.P] = (int origin, BitBoard _) => Attacks.PawnAttacks[1, origin].Board,
        //    [(int)Piece.p] = (int origin, BitBoard _) => Attacks.PawnAttacks[0, origin].Board,

        //    [(int)Piece.K] = (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,
        //    [(int)Piece.k] = (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,

        //    [(int)Piece.N] = (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,
        //    [(int)Piece.n] = (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,

        //    [(int)Piece.B] = (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,
        //    [(int)Piece.b] = (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,

        //    [(int)Piece.R] = (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,
        //    [(int)Piece.r] = (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,

        //    // TODO try to improve performance by re-using bishop and rook attacks
        //    [(int)Piece.Q] = (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
        //    [(int)Piece.q] = (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
        //};

        public static List<Move> GenerateAllMoves(Game game)
        {
            var moves = new List<Move>();

            if (game.Side == Side.Both)
            {
                return moves;
            }

            var offset = Utils.PieceOffset(game.Side);

            moves.AddRange(GeneratePawnMoves(game, offset));

            return moves;
        }

        internal static IEnumerable<Move> GeneratePawnMoves(Game game, int offset)
        {
            int sourceSquare, targetSquare;

            var piece = (int)Piece.P + offset;
            var pawnPush = +8 - ((int)game.Side * 16);          // game.Side == Side.White ? -8 : +8
            int oppositeSide = Utils.OppositeSide(game.Side);   // game.Side == Side.White ? (int)Side.Black : (int)Side.White
            var bitboard = game.PieceBitBoards[piece].Board;

            while (bitboard != default)
            {
                sourceSquare = BitBoard.GetLS1BIndex(bitboard);
                bitboard = BitBoard.ResetLS1B(bitboard);

                var sourceRank = (sourceSquare / 8) + 1;
                if (sourceRank == 1 || sourceRank == 8)
                {
                    Logger.Warn($"There's a non-promoted {game.Side} pawn in rank {sourceRank}");
                    continue;
                }

                // Pawn pushes
                var singlePush = sourceSquare + pawnPush;
                if (!game.OccupancyBitBoards[2].GetBit(singlePush))
                {
                    var targetRank = (singlePush / 8) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Promotion
                    {
                        yield return new Move(piece, sourceSquare, singlePush, MoveType.QueenPromotion);
                        yield return new Move(piece, sourceSquare, singlePush, MoveType.RookPromotion);
                        yield return new Move(piece, sourceSquare, singlePush, MoveType.KnightPromotion);
                        yield return new Move(piece, sourceSquare, singlePush, MoveType.BishopPromotion);
                    }
                    else
                    {
                        yield return new Move(piece, sourceSquare, singlePush, MoveType.Quiet);
                    }

                    // Inside of the if because singlePush square cannot be occupied either
                    var doublePush = sourceSquare + (2 * pawnPush);
                    if (!game.OccupancyBitBoards[2].GetBit(doublePush)
                        && ((sourceRank == 2 && game.Side == Side.Black) || (sourceRank == 7 && game.Side == Side.White)))
                    {
                        yield return new Move(piece, sourceSquare, doublePush, MoveType.Quiet);
                    }
                }

                var attacks = Attacks.PawnAttacks[(int)game.Side, sourceSquare];

                // En passant
                if (attacks.GetBit(game.EnPassant)) /*&& game.OccupancyBitBoards[oppositeOccupancy].GetBit(targetSquare + singlePush)*/
                {
                    yield return new Move(piece, sourceSquare, (int)game.EnPassant, MoveType.EnPassant);
                }

                // Captures
                ulong attackedSquares = attacks.Board & game.OccupancyBitBoards[oppositeSide].Board;
                while (attackedSquares != default)
                {
                    targetSquare = BitBoard.GetLS1BIndex(attackedSquares);
                    attackedSquares = BitBoard.ResetLS1B(attackedSquares);

                    var targetRank = (targetSquare / 8) + 1;
                    if (targetRank == 1 || targetRank == 8)  // Capture with promotion
                    {
                        yield return new Move(piece, sourceSquare, targetSquare, MoveType.QueenPromotion);
                        yield return new Move(piece, sourceSquare, targetSquare, MoveType.RookPromotion);
                        yield return new Move(piece, sourceSquare, targetSquare, MoveType.KnightPromotion);
                        yield return new Move(piece, sourceSquare, targetSquare, MoveType.BishopPromotion);
                    }
                    else
                    {
                        yield return new Move(piece, sourceSquare, targetSquare, MoveType.Capture);
                    }
                }
            }
        }

        /// <summary>
        /// Obvious moves that put the king in check have been discarded, but the rest still need to be discarded
        /// see FEN position "8/8/8/2bbb3/2bKb3/2bbb3/8/8 w - - 0 1", where 4 legal moves (corners) are found
        /// </summary>
        /// <param name="game"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal static IEnumerable<Move> GenerateCastleMoves(Game game, int offset)
        {
            var piece = (int)Piece.K + offset;
            var oppositeSide = (Side)Utils.OppositeSide(game.Side);

            int sourceSquare = game.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one

            // Castles
            if (game.Castle != default)
            {
                if (game.Side == Side.White)
                {
                    if (((game.Castle & (int)CastlingRights.WK) != default)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.f1)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.g1)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.e1, game, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.f1, game, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.g1, game, oppositeSide))
                    {
                        yield return new Move(piece, sourceSquare, (int)BoardSquares.g1, MoveType.ShortCastle);
                    }

                    if (((game.Castle & (int)CastlingRights.WQ) != default)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.d1)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.c1)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b1)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.e1, game, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.d1, game, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.c1, game, oppositeSide))
                    {
                        yield return new Move(piece, sourceSquare, (int)BoardSquares.c1, MoveType.LongCastle);
                    }
                }
                else
                {
                    if (((game.Castle & (int)CastlingRights.BK) != default)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.f8)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.g8)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.e8, game, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.f8, game, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.g8, game, oppositeSide))
                    {
                        yield return new Move(piece, sourceSquare, (int)BoardSquares.g8, MoveType.ShortCastle);
                    }

                    if (((game.Castle & (int)CastlingRights.BQ) != default)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.d8)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.c8)
                        && !game.OccupancyBitBoards[(int)Side.Both].GetBit(BoardSquares.b8)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.e8, game, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.d8, game, oppositeSide)
                        && !Attacks.IsSquaredAttackedBySide((int)BoardSquares.c8, game, oppositeSide))
                    {
                        yield return new Move(piece, sourceSquare, (int)BoardSquares.c8, MoveType.LongCastle);
                    }
                }
            }
        }

        internal static IEnumerable<Move> GenerateKingMoves(Game game, int offset)
        {
            var piece = (int)Piece.K + offset;
            var oppositeSide = (Side)Utils.OppositeSide(game.Side);

            int sourceSquare = game.PieceBitBoards[piece].GetLS1BIndex(); // There's for sure only one
            int targetSquare;

            // Captures
            ulong attacks = Attacks.KingAttacks[sourceSquare].Board;
            while (attacks != default)
            {
                targetSquare = BitBoard.GetLS1BIndex(attacks);
                attacks = BitBoard.ResetLS1B(attacks);

                if (!game.OccupancyBitBoards[(int)game.Side].GetBit(targetSquare)
                    && !Attacks.IsSquaredAttackedBySide(targetSquare, game, oppositeSide))
                {
                    var moveType = game.OccupancyBitBoards[(int)Side.Both].GetBit(targetSquare)
                        ? MoveType.Capture
                        : MoveType.Quiet;

                    yield return new Move(piece, sourceSquare, targetSquare, moveType);
                }
            }
        }
    }
}
