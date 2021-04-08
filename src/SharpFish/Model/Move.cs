namespace SharpFish.Model
{
    public record Move(int Piece, int SourceSquare, int TargetSquare, MoveType MoveType)
    {
        public override string ToString()
        {
            return $"{Constants.AsciiPieces[Piece]}{Constants.Coordinates[SourceSquare]}{Constants.Coordinates[TargetSquare]} ({MoveType})";
        }
    }

    public enum MoveType
    {
        Quiet,
        Capture,
        EnPassant,
        ShortCastle,
        LongCastle,
        QueenPromotion,
        RookPromotion,
        KnightPromotion,
        BishopPromotion
    }
}
