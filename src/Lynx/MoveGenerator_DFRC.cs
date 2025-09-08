using Lynx.Model;

namespace Lynx;

public sealed class MoveGenerator_DFRC : IMoveGenerator
{
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static MoveGenerator_DFRC Instance { get; } = new();
#pragma warning restore CA1000 // Do not declare static members on generic types

    private const int TRUE = 1;

    internal static int Init() => TRUE;

    /// <summary>
    /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
    /// https://csharpindepth.com/articles/singleton
    /// </summary>
#pragma warning disable S3253 // Constructor and destructor declarations should not be redundant
    static MoveGenerator_DFRC() { }
#pragma warning restore S3253 // Constructor and destructor declarations should not be redundant

    private MoveGenerator_DFRC() { }

    /// <inheritdoc/>
    public void GenerateCastlingMoves(ref int localIndex, Span<int> movePool, Position position, ref EvaluationContext evaluationContext)
    {
        var castlingRights = position.Castle;

        if (castlingRights != default)
        {
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];

            if (position.Side == Side.White)
            {
                var whiteKingSourceSquare = position.InitialKingSquares[(int)Side.White];

                if ((castlingRights & (int)CastlingRights.WK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))
                {
                    movePool[localIndex++] = MoveExtensions.EncodeShortCastle(whiteKingSourceSquare, position._initialKingsideRookSquares[(int)Side.White], (int)Piece.K);
                }

                if ((castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext))

                {
                    movePool[localIndex++] = MoveExtensions.EncodeLongCastle(whiteKingSourceSquare, position._initialQueensideRookSquares[(int)Side.White], (int)Piece.K);
                }
            }
            else
            {
                var blackKingSourceSquare = position.InitialKingSquares[(int)Side.Black];

                if ((castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                {
                    movePool[localIndex++] = MoveExtensions.EncodeShortCastle(blackKingSourceSquare, position._initialKingsideRookSquares[(int)Side.Black], (int)Piece.k);
                }

                if ((castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext))
                {
                    movePool[localIndex++] = MoveExtensions.EncodeLongCastle(blackKingSourceSquare, position._initialQueensideRookSquares[(int)Side.Black], (int)Piece.k);
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool IsAnyCastlingMoveValid(Position position, ref EvaluationContext evaluationContext)
    {
        var castlingRights = position.Castle;

        if (castlingRights != default)
        {
            var occupancy = position.OccupancyBitBoards[(int)Side.Both];

            if (position.Side == Side.White)
            {
                var whiteKingSourceSquare = position.InitialKingSquares[(int)Side.White];
                bool ise1Attacked = position.IsSquareAttacked(whiteKingSourceSquare, Side.Black);

                if (!ise1Attacked
                    && (castlingRights & (int)CastlingRights.WK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                    && IMoveGenerator.IsValidMove(position, MoveExtensions.EncodeShortCastle(whiteKingSourceSquare, Constants.WhiteKingKingsideCastlingSquare, (int)Piece.K)))
                {
                    return true;
                }

                if (!ise1Attacked
                    && (castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, ref evaluationContext)
                    && IMoveGenerator.IsValidMove(position, MoveExtensions.EncodeLongCastle(whiteKingSourceSquare, Constants.WhiteKingQueensideCastlingSquare, (int)Piece.K)))
                {
                    return true;
                }
            }
            else
            {
                var blackKingSourceSquare = position.InitialKingSquares[(int)Side.Black];

                bool ise8Attacked = position.IsSquareAttacked(blackKingSourceSquare, Side.White);

                if (!ise8Attacked
                    && (castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
                    && IMoveGenerator.IsValidMove(position, MoveExtensions.EncodeShortCastle(blackKingSourceSquare, Constants.BlackKingKingsideCastlingSquare, (int)Piece.k)))
                {
                    return true;
                }

                if (!ise8Attacked
                    && (castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, ref evaluationContext)
                    && IMoveGenerator.IsValidMove(position, MoveExtensions.EncodeLongCastle(blackKingSourceSquare, Constants.BlackKingQueensideCastlingSquare, (int)Piece.k)))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
