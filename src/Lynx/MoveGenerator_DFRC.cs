using Lynx.Model;
using System.Diagnostics;

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
    public void GenerateCastlingMoves(ref int localIndex, Span<int> movePool, Position position, ref readonly EvaluationContext evaluationContext)
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
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, in evaluationContext))
                {
                    var whiteShortCastle = MoveExtensions.EncodeShortCastle(whiteKingSourceSquare, Constants.WhiteKingKingsideCastlingSquare, (int)Piece.K);
                    movePool[localIndex++] = whiteShortCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeShortCastle(whiteKingSourceSquare, Constants.WhiteKingKingsideCastlingSquare, (int)Piece.K),
                        $"Wrong hardcoded white short castle move, expected {whiteShortCastle}, got {MoveExtensions.EncodeShortCastle(whiteKingSourceSquare, Constants.WhiteKingKingsideCastlingSquare, (int)Piece.K)}");
                }

                if ((castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, in evaluationContext))

                {
                    var whiteLongCastle = MoveExtensions.EncodeLongCastle(whiteKingSourceSquare, Constants.WhiteKingQueensideCastlingSquare, (int)Piece.K);
                    movePool[localIndex++] = whiteLongCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeLongCastle(whiteKingSourceSquare, Constants.WhiteKingQueensideCastlingSquare, (int)Piece.K),
                        $"Wrong hardcoded white long castle move, expected {whiteLongCastle}, got {MoveExtensions.EncodeLongCastle(whiteKingSourceSquare, Constants.WhiteKingQueensideCastlingSquare, (int)Piece.K)}");
                }
            }
            else
            {
                var blackKingSourceSquare = position.InitialKingSquares[(int)Side.Black];

                if ((castlingRights & (int)CastlingRights.BK) != default
                    && (occupancy & position.KingsideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, in evaluationContext))
                {
                    var blackShortCastle = MoveExtensions.EncodeShortCastle(blackKingSourceSquare, Constants.BlackKingKingsideCastlingSquare, (int)Piece.k);
                    movePool[localIndex++] = blackShortCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeShortCastle(blackKingSourceSquare, Constants.BlackKingKingsideCastlingSquare, (int)Piece.k),
                        $"Wrong hardcoded black short castle move, expected {blackShortCastle}, got {MoveExtensions.EncodeShortCastle(blackKingSourceSquare, Constants.BlackKingKingsideCastlingSquare, (int)Piece.k)}");
                }

                if ((castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, in evaluationContext))
                {
                    var blackLongCastle = MoveExtensions.EncodeLongCastle(blackKingSourceSquare, Constants.BlackKingQueensideCastlingSquare, (int)Piece.k);
                    movePool[localIndex++] = blackLongCastle;

                    Debug.Assert(movePool[localIndex - 1] == MoveExtensions.EncodeLongCastle(blackKingSourceSquare, Constants.BlackKingQueensideCastlingSquare, (int)Piece.k),
                        $"Wrong hardcoded black long castle move, expected {blackLongCastle}, got {MoveExtensions.EncodeLongCastle(blackKingSourceSquare, Constants.BlackKingQueensideCastlingSquare, (int)Piece.k)}");
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool IsAnyCastlingMoveValid(Position position, ref readonly EvaluationContext evaluationContext)
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
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.White], Side.Black, in evaluationContext)
                    && IMoveGenerator.IsValidMove(position, MoveExtensions.EncodeShortCastle(whiteKingSourceSquare, Constants.WhiteKingKingsideCastlingSquare, (int)Piece.K)))
                {
                    return true;
                }

                if (!ise1Attacked
                    && (castlingRights & (int)CastlingRights.WQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.White]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.White], Side.Black, in evaluationContext)
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
                    && !position.AreSquaresAttacked(position.KingsideCastlingNonAttackedSquares[(int)Side.Black], Side.White, in evaluationContext)
                    && IMoveGenerator.IsValidMove(position, MoveExtensions.EncodeShortCastle(blackKingSourceSquare, Constants.BlackKingKingsideCastlingSquare, (int)Piece.k)))
                {
                    return true;
                }

                if (!ise8Attacked
                    && (castlingRights & (int)CastlingRights.BQ) != default
                    && (occupancy & position.QueensideCastlingFreeSquares[(int)Side.Black]) == 0
                    && !position.AreSquaresAttacked(position.QueensideCastlingNonAttackedSquares[(int)Side.Black], Side.White, in evaluationContext)
                    && IMoveGenerator.IsValidMove(position, MoveExtensions.EncodeLongCastle(blackKingSourceSquare, Constants.BlackKingQueensideCastlingSquare, (int)Piece.k)))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
