namespace SharpFish.Model
{
    public readonly struct Move
    {
        /// <summary>
        ///     Binary move bits            Hexadecimal
        /// 0000 0000 0000 0000 0011 1111       0x3F        Source square (63 bits)
        /// 0000 0000 0000 1111 1100 0000       0xFC0       Target Square (63 bits)
        /// 0000 0000 1111 0000 0000 0000       0xF000      Piece (11 bits)
        /// 0000 1111 0000 0000 0000 0000       0xF0000     Promoted piece (~11 bits)
        /// 0001 0000 0000 0000 0000 0000       0x10_0000   Capture flag
        /// 0010 0000 0000 0000 0000 0000       0x20_0000   Double pawn push flag
        /// 0100 0000 0000 0000 0000 0000       0x40_0000   Enpassant flag
        /// 1000 0000 0000 0000 0000 0000       0x80_0000   Castling flag
        /// Total: 24 bits -> fits an int
        /// Could be reduced to 16 bits -> see https://www.chessprogramming.org/Encoding_Moves
        /// </summary>
        public int EncodedMove { get; }

        internal Move(int n) { EncodedMove = n; }

        public Move(
            int sourceSquare, int targetSquare, int piece, int promotedPiece = default,
            int isCapture = default, int isDoublePawnPush = default, int isEnPassant = default, int isCastle = default)
        {
            EncodedMove = sourceSquare | (targetSquare << 6) | (piece << 12) | (promotedPiece << 16)
                | (isCapture << 20)
                | (isDoublePawnPush << 21)
                | (isEnPassant << 22)
                | (isCastle << 23);
        }

        public readonly int SourceSquare() => EncodedMove & 0x3F;

        public readonly int TargetSquare() => (EncodedMove & 0xFC0) >> 6;

        public readonly int Piece() => (EncodedMove & 0xF000) >> 12;

        public readonly int PromotedPiece() => (EncodedMove & 0xF0000) >> 16;

        public readonly bool IsCapture() => (EncodedMove & 0x10_0000) >> 20 != default;

        public readonly bool IsDoublePawnPush() => (EncodedMove & 0x20_0000) >> 21 != default;

        public readonly bool IsEnPassant() => (EncodedMove & 0x40_0000) >> 22 != default;

        public readonly bool IsCastle() => (EncodedMove & 0x80_0000) >> 23 != default;

        /// <summary>
        /// Assumues <see cref="IsCastle"/> == 1
        /// </summary>
        /// <returns></returns>
        public readonly bool IsShortCastle()
        {
            var targetSquare = TargetSquare();

            return targetSquare == Constants.WhiteShortCastleKingSquare || targetSquare == Constants.BlackShortCastleKingSquare;
        }

        /// <summary>
        /// Assumues <see cref="IsCastle"/> == 1
        /// </summary>
        /// <returns></returns>
        public readonly bool IsLongCastle()
        {
            var targetSquare = TargetSquare();

            return targetSquare == Constants.WhiteLongCastleKingSquare || targetSquare == Constants.BlackLongCastleKingSquare;
        }

        public string Print()
        {
            if (IsCastle() == default)
            {
                return
                    Constants.AsciiPieces[Piece()] +
                    Constants.Coordinates[SourceSquare()] +
                    (IsCapture() == default ? "" : "x") +
                    Constants.Coordinates[TargetSquare()] +
                    (PromotedPiece() == default ? "" : $"={Constants.AsciiPieces[PromotedPiece()]}") +
                    (IsEnPassant() == default ? "" : "e.p.");
            }
            else
            {
                return IsShortCastle() ? "O-O" : "O-O-O";
            }
        }
    }
}
