namespace SharpFish.Model
{
    public struct MoveStruct
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

        internal MoveStruct(int n) { EncodedMove = n; }

        public MoveStruct(
            int sourceSquare, int targetSquare, int piece, int promotedPiece = default,
            int isCapture = default, int isDoublePawnPush = default, int enPassant = default, int isCastle = default)
        {
            EncodedMove = sourceSquare | (targetSquare << 6) | (piece << 12) | (promotedPiece << 16)
                | (isCapture << 20)
                | (isDoublePawnPush << 21)
                | (enPassant << 22)
                | (isCastle << 23);
        }

        public int SourceSquare() => EncodedMove & 0x3F;

        public int TargetSquare() => (EncodedMove & 0xFC0) >> 6;

        public int Piece() => (EncodedMove & 0xF000) >> 12;

        public int PromotedPiece() => (EncodedMove & 0xF0000) >> 16;

        public int IsCapture() => (EncodedMove & 0x40_0000) >> 20;

        public int IsDoublePawnPush() => (EncodedMove & 0x40_0000) >> 21;

        public int IsEnPassant() => (EncodedMove & 0x40_0000) >> 22;

        public int IsCastle() => (EncodedMove & 0x80_0000) >> 23;

        /// <summary>
        /// Assumues <see cref="IsCastle"/> == 1
        /// </summary>
        /// <returns></returns>
        public bool IsShortCastle()
        {
            var targetSquare = TargetSquare();

            return targetSquare == (int)BoardSquares.g1 || targetSquare == (int)BoardSquares.g8;
        }

        /// <summary>
        /// Assumues <see cref="IsCastle"/> == 1
        /// </summary>
        /// <returns></returns>
        public bool IsLongCastle()
        {
            var targetSquare = TargetSquare();

            return targetSquare == (int)BoardSquares.c1 || targetSquare == (int)BoardSquares.c8;
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
    public record Move(int Piece, int SourceSquare, int TargetSquare, MoveType MoveType = MoveType.Unknown)
    {
        /// <summary>
        /// Encodes a move in the 24 LSB of an int
        /// </summary>
        /// <param name="sourceSquare"><see cref="BoardSquares"/></param>
        /// <param name="targetSquare"><see cref="BoardSquares"/></param>
        /// <param name="piece"><see cref="Piece"/></param>
        /// <param name="promotedPiece"></param>
        /// <param name="isCapture">0|1</param>
        /// <param name="isDoublePawnPush">0|1</param>
        /// <param name="enPassant">0|1</param>
        /// <param name="isCastle">0|1</param>
        /// <returns></returns>
        public static int Encode(
            int sourceSquare, int targetSquare, int piece, int promotedPiece = default,
            int isCapture = default, int isDoublePawnPush = default, int enPassant = default, int isCastle = default)
        {
            return sourceSquare | (targetSquare << 6) | (piece << 12) | (promotedPiece << 16)
                | (isCapture << 20)
                | (isDoublePawnPush << 21)
                | (enPassant << 22)
                | (isCastle << 23);
        }

        //public static int SourceSquare(int move) => (move & 0x3F);

        //public static int TargetSquare(int move) => (move & 0xFC0) >> 6;

        //public static int Piece(int move) => (move & 0xF000) >> 12;

        public static int PromotedPiece(int move) => (move & 0xF0000) >> 16;

        public static int IsCapture(int move) => (move & 0x40_0000) >> 20;

        public static int IsDoublePawnPush(int move) => (move & 0x40_0000) >> 21;

        public static int IsEnPassant(int move) => (move & 0x40_0000) >> 22;

        public static int IsCastle(int move) => (move & 0x80_0000) >> 23;

        //public string Print(int move)
        //{
        //    if (IsCastle(move) == default)
        //    {
        //        return $"{Constants.AsciiPieces[Piece(move)]}" +
        //            $"{Constants.Coordinates[SourceSquare(move)]}" +
        //        $"{(IsCapture(move) == default ? "" : "x")}" +
        //        $"{Constants.Coordinates[TargetSquare(move)]}" +
        //        $"{(PromotedPiece(move) == default ? "" : $"={Constants.AsciiPieces[PromotedPiece(move)]}")}" +
        //        $"{(IsEnPassant(move) == default ? "" : "e.p.")}";
        //    }
        //    else
        //    {
        //        if (TargetSquare(move) == (int)BoardSquares.c1 || TargetSquare(move) == (int)BoardSquares.c8)
        //        {
        //            return "O-O";
        //        }
        //        else
        //        {
        //            return "O-O-O";
        //        }
        //    }
        //}
    }

    public enum MoveType
    {
        Unknown,
        Quiet,
        Capture,
        EnPassant,
        DoublePush,
        ShortCastle,
        LongCastle,
        QueenPromotion,
        RookPromotion,
        KnightPromotion,
        BishopPromotion
    }
}
