using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lynx.Model
{
    public readonly struct Move
    {
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

        internal Move(int n) { EncodedMove = n; }

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

        public readonly int SourceSquare() => EncodedMove & 0x3F;

        public readonly int TargetSquare() => (EncodedMove & 0xFC0) >> 6;

        public readonly int Piece() => (EncodedMove & 0xF000) >> 12;

        public readonly int PromotedPiece() => (EncodedMove & 0xF0000) >> 16;

        public readonly bool IsCapture() => (EncodedMove & 0x10_0000) >> 20 != default;

        public readonly bool IsDoublePawnPush() => (EncodedMove & 0x20_0000) >> 21 != default;

        public readonly bool IsEnPassant() => (EncodedMove & 0x40_0000) >> 22 != default;

        public readonly bool IsShortCastle() => (EncodedMove & 0x80_0000) >> 23 != default;

        public readonly bool IsLongCastle() => (EncodedMove & 0x100_0000) >> 24 != default;

        public readonly bool IsCastle() => (EncodedMove & 0x180_0000) >> 23 != default;

        public readonly void Print()
        {
            Console.WriteLine(
                IsCastle() == default
                ?
                    Constants.AsciiPieces[Piece()] +
                    Constants.Coordinates[SourceSquare()] +
                    (IsCapture() == default ? "" : "x") +
                    Constants.Coordinates[TargetSquare()] +
                    (PromotedPiece() == default ? "" : $"={Constants.AsciiPieces[PromotedPiece()]}") +
                    (IsEnPassant() == default ? "" : "e.p.")
                : (IsShortCastle() ? "O-O" : "O-O-O"));
        }


        public string UCIString()
        {
            return
                Constants.Coordinates[SourceSquare()] +
                Constants.Coordinates[TargetSquare()] +
                (PromotedPiece() == default ? "" : $"{Constants.AsciiPieces[PromotedPiece()].ToString().ToLowerInvariant()}") +
                (IsEnPassant() == default ? "" : "e.p.");
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

                sb.Append($"{i + 1,-3}")
                  .Append($"{Constants.AsciiPieces[move.Piece()],-3}")
                  .Append($"{Constants.Coordinates[move.SourceSquare()],-4}")
                  .Append($"{isCapture(move.IsCapture()),-2}")
                  .Append($"{Constants.Coordinates[move.TargetSquare()],-4}")
                  .Append($"{bts(move.IsDoublePawnPush()),-4}")
                  .Append($"{bts(move.IsEnPassant()),-3}")
                  .Append($"{bts(move.IsShortCastle()),-4}")
                  .Append($"{bts(move.IsLongCastle()),-4}")
                  .Append(Environment.NewLine);
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
