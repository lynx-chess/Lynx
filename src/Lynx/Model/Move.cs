using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx.Model
{
    public readonly struct Move
    {
        public const int CaptureBaseScore = 100_000;

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Binary move bits            Hexadecimal
        /// 0000 0000 0000 0000 0000 0011 1111      0x3F        Source square (63 bits)
        /// 0000 0000 0000 0000 1111 1100 0000      0xFC0       Target Square (63 bits)
        /// 0000 0000 0000 1111 0000 0000 0000      0xF000      Piece (11 bits)
        /// 0000 0000 1111 0000 0000 0000 0000      0xF0000     Promoted piece (~11 bits)
        /// 0000 0001 0000 0000 0000 0000 0000      0x10_0000   Capture flag
        /// 0000 0010 0000 0000 0000 0000 0000      0x20_0000   Double pawn push flag
        /// 0000 0100 0000 0000 0000 0000 0000      0x40_0000   Enpassant flag
        /// 0000 1000 0000 0000 0000 0000 0000      0x80_0000   Short castling flag
        /// 0001 0000 0000 0000 0000 0000 0000      0x100_0000  Long castling flag
        /// Total: 24 bits -> fits an int
        /// Could be reduced to 16 bits -> see https://www.chessprogramming.org/Encoding_Moves
        /// </summary>
        public int EncodedMove { get; }

        /// <summary>
        /// 'Encode' constractor
        /// </summary>
        /// <param name="sourceSquare"></param>
        /// <param name="targetSquare"></param>
        /// <param name="piece"></param>
        /// <param name="promotedPiece"></param>
        /// <param name="isCapture"></param>
        /// <param name="isDoublePawnPush"></param>
        /// <param name="isEnPassant"></param>
        /// <param name="isShortCastle"></param>
        /// <param name="isLongCastle"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move(
            int sourceSquare, int targetSquare, int piece, int promotedPiece = default,
            int isCapture = default, int isDoublePawnPush = default, int isEnPassant = default,
            int isShortCastle = default, int isLongCastle = default)
        {
            EncodedMove = sourceSquare | (targetSquare << 6) | (piece << 12) | (promotedPiece << 16)
                | (isCapture << 20)
                | (isDoublePawnPush << 21)
                | (isEnPassant << 22)
                | (isShortCastle << 23)
                | (isLongCastle << 24);
        }

        /// <summary>
        /// Returns the move from <paramref name="moveList"/> indicated by <paramref name="UCIString"/>
        /// </summary>
        /// <param name="UCIString"></param>
        /// <param name="moveList"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <returns></returns>
        public static bool TryParseFromUCIString(string UCIString, List<Move> moveList, [NotNullWhen(true)] out Move? move)
        {
            Debug.Assert(UCIString.Length == 4 || UCIString.Length == 5);

            var sourceSquare = (UCIString[0] - 'a') + ((8 - (UCIString[1] - '0')) * 8);
            var targetSquare = (UCIString[2] - 'a') + ((8 - (UCIString[3] - '0')) * 8);

            var candidateMoves = moveList.Where(move => move.SourceSquare() == sourceSquare && move.TargetSquare() == targetSquare);

            if (UCIString.Length == 4)
            {
                move = candidateMoves.FirstOrDefault();

                if (move.Equals(default(Move)))
                {
                    _logger.Warn($"Unable to link last move string {UCIString} to a valid move in the current position. That move may have already been played");
                    move = null;
                    return false;
                }

                Debug.Assert(move.Value.PromotedPiece() == default);
                return true;
            }
            else
            {
                var promotedPiece = (int)Enum.Parse<Piece>(UCIString[4].ToString());

                bool predicate(Move m)
                {
                    var actualPromotedPiece = m.PromotedPiece();

                    return actualPromotedPiece == promotedPiece
                    || actualPromotedPiece == promotedPiece - 6;
                }

                move = candidateMoves.FirstOrDefault(predicate);
                if (move.Equals(default(Move)))
                {
                    _logger.Warn($"Unable to link move {UCIString} to a valid move in the current position. That move may have already been played");
                    move = null;
                    return false;
                }

                Debug.Assert(candidateMoves.Count() == 4);
                Debug.Assert(candidateMoves.Count(predicate) == 1);

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int SourceSquare() => EncodedMove & 0x3F;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int TargetSquare() => (EncodedMove & 0xFC0) >> 6;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Piece() => (EncodedMove & 0xF000) >> 12;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int PromotedPiece() => (EncodedMove & 0xF0000) >> 16;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsCapture() => (EncodedMove & 0x10_0000) >> 20 != default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsDoublePawnPush() => (EncodedMove & 0x20_0000) >> 21 != default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsEnPassant() => (EncodedMove & 0x40_0000) >> 22 != default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsShortCastle() => (EncodedMove & 0x80_0000) >> 23 != default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsLongCastle() => (EncodedMove & 0x100_0000) >> 24 != default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsCastle() => (EncodedMove & 0x180_0000) >> 23 != default;

        /// <summary>
        /// Returns the score evaluation of a move taking into account <see cref="EvaluationConstants.MostValueableVictimLeastValuableAttacker"/>
        /// </summary>
        /// <param name="position">The position that precedes a move</param>
        /// <returns>The higher the score is, the more valuable is the captured piece and the less valuable is the piece that makes the such capture</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int Score(Position position, int[,]? killerMoves = null, int? plies = null)
        {
            int score = 0;

            if (IsCapture())
            {
                var sourcePiece = Piece();
                int targetPiece = (int)Model.Piece.P;    // Important to initialize to P or p, due to en-passant captures

                var targetSquare = TargetSquare();
                var oppositeSide = Utils.OppositeSide(position.Side);
                var oppositeSideOffset = Utils.PieceOffset(oppositeSide);
                var oppositePawnIndex = (int)Model.Piece.P + oppositeSideOffset;

                var limit = (int)Model.Piece.K + oppositeSideOffset;
                for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
                {
                    if (position.PieceBitBoards[pieceIndex].GetBit(targetSquare))
                    {
                        targetPiece = pieceIndex;
                        break;
                    }
                }

                score += CaptureBaseScore + EvaluationConstants.MostValueableVictimLeastValuableAttacker[sourcePiece, targetPiece];
            }
            else
            {
                if (killerMoves is not null && plies is not null)
                {
                    // 1st killer move
                    if (killerMoves[0, plies.Value] == EncodedMove)
                    {
                        return EvaluationConstants.FirstKillerMoveValue;
                    }

                    // 2nd killer move
                    else if (killerMoves[1, plies.Value] == EncodedMove)
                    {
                        return EvaluationConstants.SecondKillerMoveValue;
                    }
                }
            }

            return score;
        }

        public override string ToString()
        {
#pragma warning disable S3358 // Ternary operators should not be nested
            return IsCastle() == default
                ?
                    Constants.AsciiPieces[Piece()] +
                    Constants.Coordinates[SourceSquare()] +
                    (IsCapture() == default ? "" : "x") +
                    Constants.Coordinates[TargetSquare()] +
                    (PromotedPiece() == default ? "" : $"={Constants.AsciiPieces[PromotedPiece()]}") +
                    (IsEnPassant() == default ? "" : "e.p.")
                : (IsShortCastle() ? "O-O" : "O-O-O");
#pragma warning restore S3358 // Ternary operators should not be nested
        }

        public readonly void Print()
        {
            Console.WriteLine(ToString());
        }

        public string UCIString()
        {
            return
                Constants.Coordinates[SourceSquare()] +
                Constants.Coordinates[TargetSquare()] +
                (PromotedPiece() == default ? "" : $"{Constants.AsciiPieces[PromotedPiece()].ToString().ToLowerInvariant()}");
        }

        public static void PrintMoveList(IEnumerable<Move> moves)
        {
            Console.WriteLine($"{"#",-3}{"Pc",-3}{"src",-4}{"x",-2}{"tgt",-4}{"DPP",-4}{"ep",-3}{"O-O",-4}{"O-O-O",-7}\n");

            static string bts(bool b) => b ? "1" : "0";
            static string isCapture(bool c) => c ? "x" : "";

            var sb = new StringBuilder();
            for (int i = 0; i < moves.Count(); ++i)
            {
                var move = moves.ElementAt(i);

                sb.AppendFormat("{0,-3}", i + 1)
                  .AppendFormat("{0,-3}", Constants.AsciiPieces[move.Piece()])
                  .AppendFormat("{0,-4}", Constants.Coordinates[move.SourceSquare()])
                  .AppendFormat("{0,-2}", isCapture(move.IsCapture()))
                  .AppendFormat("{0,-4}", Constants.Coordinates[move.TargetSquare()])
                  .AppendFormat("{0,-4}", bts(move.IsDoublePawnPush()))
                  .AppendFormat("{0,-3}", bts(move.IsEnPassant()))
                  .AppendFormat("{0,-4}", bts(move.IsShortCastle()))
                  .AppendFormat("{0,-4}", bts(move.IsLongCastle()))
                  .Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
