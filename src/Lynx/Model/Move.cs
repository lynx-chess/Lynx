using NLog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

public enum SpecialMoveType
{
    None = 0,
    EnPassant = 1,
    Castle = 2,
    Promotion = 3,
}

/// <summary>
///  Binary move bits     Hexadecimal
/// 0000 0000 0011 1111     0x3F            Source square (0-63)
/// 0000 1111 1100 0000     0xFC0           Target square (0-63)
/// 0011 0000 0000 0000     0x3000          Promoted piece (1-4)
/// 0100 0000 0000 0000     0x40000         En-passant flag
/// 1000 0000 0000 0000     0x80000         Castle flag
/// 1100 0000 0000 0000     0xC000          Promotion flag
/// --------------------------------------------------------------------------------------------
/// Total: 16 bits -> fits in a short
/// </summary>
public static class MoveExtensions
{
    private const int TargetSquareOffset = 6;
    private const int PromotedPieceOffset = 12;
    private const int SpecialMoveFlagOffset = 14;

    private const int SpecialMoveMask = 0xC000;
    private const int PromotedPieceMask = 0x3000;
    private const int IsPromotionMask = 0xC000;
    private const int SourceSquareMask = 0x3F;
    private const int TargetSquareMask = 0xFC0;

    private const int UCIMask = 0b1111_1111_1111_1111;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Move to represent a null move that fits in 12x64 arrays
    /// </summary>
    public static readonly Move NullMove = Encode((int)BoardSquare.e1, (int)BoardSquare.e1);

    /// <summary>
    /// Encodes non-capturing moves
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move Encode(int sourceSquare, int targetSquare)
    {
        return sourceSquare
            | (targetSquare << TargetSquareOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeEnPassant(int sourceSquare, int targetSquare)
    {
        return sourceSquare
            | (targetSquare << TargetSquareOffset)
            | (int)SpecialMoveType.EnPassant << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeCastle(int sourceSquare, int targetSquare)
    {
        if (targetSquare == CastlingData.DefaultValues)
        {
            return -1;
        }

        return sourceSquare
            | (targetSquare << TargetSquareOffset)
            | (int)SpecialMoveType.Castle << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodePromotion(int sourceSquare, int targetSquare, int promotedPiece)
    {
        return sourceSquare
            | (targetSquare << TargetSquareOffset)
            | ((promotedPiece - 1) << PromotedPieceOffset)
            | (int)SpecialMoveType.Promotion << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodePromotionFromPawnMove(Move pawnMove, int promotedPiece) =>
        pawnMove
            | ((promotedPiece - 1) << PromotedPieceOffset)
            | (int)SpecialMoveType.Promotion << SpecialMoveFlagOffset;

    /// <summary>
    /// Returns the move from <paramref name="moveList"/> indicated by <paramref name="UCIString"/>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public static bool TryParseFromUCIString(ReadOnlySpan<char> UCIString, ReadOnlySpan<Move> moveList, int side, [NotNullWhen(true)] out Move? move)
    {
        Utils.Assert(UCIString.Length == 4 || UCIString.Length == 5);

        var sourceSquare = (UCIString[0] - 'a') + ((8 - (UCIString[1] - '0')) * 8);
        var targetSquare = (UCIString[2] - 'a') + ((8 - (UCIString[3] - '0')) * 8);

        for (int i = 0; i < moveList.Length; ++i)
        {
            Move candidateMove = moveList[i];

            if (candidateMove.SourceSquare() == sourceSquare && candidateMove.TargetSquare() == targetSquare)
            {
                if (UCIString.Length == 4)
                {
                    Debug.Assert(candidateMove.PromotedPiece(side) == default);

                    move = candidateMove;
                    return true;
                }

                var promotedPiece = (int)Enum.Parse<Piece>(UCIString[4].ToString());
                var candidatePromotedPiece = candidateMove.PromotedPiece(side);

                if (candidatePromotedPiece == promotedPiece
                    || candidatePromotedPiece == promotedPiece - 6)
                {
                    move = candidateMove;
                    return true;
                }

                Debug.Assert(moveList.Length >= 4, "Assert fail", "There will be at least 4 moves that match sourceSquare and targetSquare when there is a promotion");
#pragma warning disable MA0031 // Optimize Enumerable.Count() usage
                Debug.Assert(moveList.ToArray().Count(m => m.PromotedPiece(side) != default) % 4 == 0,
                    "Assert fail", "There should be 0 or a multiple of 4 that are a promotion");
                Debug.Assert(moveList.ToArray().Count(m => m.SourceSquare() == sourceSquare && m.TargetSquare() == targetSquare && m.PromotedPiece(side) != default) == 4,
                    "Assert fail", "There will be 4 (and always 4) moves that match sourceSquare and targetSquare when there is a promotion");
#pragma warning restore MA0031 // Optimize Enumerable.Count() usage
            }
        }

        _logger.Warn("Unable to link last move string {0} to a valid move in the current position. That move may have already been played", UCIString.ToString());
        move = null;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PromotedPiece(this Move move, int side) =>
        move.IsPromotion()
            ? ((move & PromotedPieceMask) >> PromotedPieceOffset) + 1 + Utils.PieceOffset(side)
            : 0;    // None?   

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPromotion(this Move move) => (move & IsPromotionMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.Promotion;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SourceSquare(this Move move) => move & SourceSquareMask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TargetSquare(this Move move) => (move & TargetSquareMask) >> TargetSquareOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Piece(this Move move, int[] board) => board[move.SourceSquare()];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Piece(this Move move, int[] board, int sourceSquare) => board[sourceSquare];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CapturedPiece(this Move move, int[] board) => board[move.TargetSquare()];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CapturedPiece(this Move move, int[] board, int targetSquare) => board[targetSquare];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpecialMoveType SpecialMoveFlag(this Move move) => (SpecialMoveType)((move & SpecialMoveMask) >> SpecialMoveFlagOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnPassant(this Move move) => (move & SpecialMoveMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.EnPassant;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShortCastle(this Move move) =>
        (move & SpecialMoveMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.Castle
        && move.TargetSquare() > move.SourceSquare();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShortCastle(this Move move, int sourceSquare, int targetSquare) =>
        (move & SpecialMoveMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.Castle
        && targetSquare > sourceSquare;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLongCastle(this Move move) =>
        (move & SpecialMoveMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.Castle
        && move.TargetSquare() < move.SourceSquare();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLongCastle(this Move move, int sourceSquare, int targetSquare) =>
        (move & SpecialMoveMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.Castle
        && targetSquare < sourceSquare;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCastle(this Move move) => (move & SpecialMoveMask) >> SpecialMoveFlagOffset >= (int)SpecialMoveType.Castle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDoublePawnPush(this Move move, int[] board)
    {
        var sourceSquare = move.SourceSquare();

        return
            (board[sourceSquare] == (int)Model.Piece.P
                    && move.TargetSquare() == sourceSquare - 16)
                || (board[sourceSquare] == (int)Model.Piece.p
                    && move.TargetSquare() == sourceSquare + 16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDoublePawnPush(this Move move, int[] board, int sourceSquare, int targetSquare)
    {
        return
            (board[sourceSquare] == (int)Model.Piece.P
                    && targetSquare == sourceSquare - 16)
                || (board[sourceSquare] == (int)Model.Piece.p
                    && targetSquare == sourceSquare + 16);
    }

    /// <summary>
    /// EPD representation of a valid move in a position
    /// </summary>
    /// <param name="move">A valid move for the given position</param>
    public static string ToEPDString(this Move move, Position position)
    {
        var piece = move.Piece(position.Board);
        var capturedPiece = move.CapturedPiece(position.Board);

        if (move.IsShortCastle())
        {
            return "O-O";
        }

        if (move.IsLongCastle())
        {
            return "O-O-O";
        }

#pragma warning disable S3358, MA0075 // Ternary operators should not be nested, culture-sensitive string
        return (piece == (int)Model.Piece.P || piece == (int)Model.Piece.p
            ? (capturedPiece != (int)Model.Piece.None
                ? global::Lynx.Constants.FileString[global::Lynx.Constants.File(move.SourceSquare())]  // exd5
                : "")    // d5
            : (char.ToUpperInvariant(global::Lynx.Constants.AsciiPieces[piece]))
                + DisambiguateMove(move, position))
            + (capturedPiece == (int)Model.Piece.None ? "" : "x")
            + Constants.Coordinates[move.TargetSquare()]
            + (move.PromotedPiece((int)position.Side) == default ? "" : $"={char.ToUpperInvariant(Constants.AsciiPieces[move.PromotedPiece((int)position.Side)])}");
#pragma warning restore S3358, MA0075 // Ternary operators should not be nested, culture-sensitive string
    }

    private static readonly string[] _uciStrings = InitUCIStrings();

    private static string[] InitUCIStrings()
    {
        var result = new string[ushort.MaxValue + 1];

        for (int source = 0; source < 64; source++)
        {
            for (int target = 0; target < 64; target++)
            {
                int baseIndex = source
                    | (target << TargetSquareOffset);

                var baseStr = string.Concat(Constants.Coordinates[source], Constants.Coordinates[target]);
                result[baseIndex] = baseStr;

                for (int promotedPiece = (int)Model.Piece.N; promotedPiece < (int)Model.Piece.k; promotedPiece++)
                {
                    var promotionMove = baseIndex
                        | ((promotedPiece - 1) << PromotedPieceOffset)
                        | (int)SpecialMoveType.Promotion << SpecialMoveFlagOffset;    // Not needed, since UCIMask ignores the first two bits

                    result[promotionMove] = $"{baseStr}{Constants.AsciiPiecesLowercase[promotedPiece]}";
                }

                var enPassantMove = baseIndex | ((int)SpecialMoveType.EnPassant << SpecialMoveFlagOffset);
                result[baseIndex | enPassantMove] = baseStr;

                var castlingMove = baseIndex | ((int)SpecialMoveType.Castle << SpecialMoveFlagOffset);
                result[baseIndex | castlingMove] = baseStr;
            }
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string UCIString(this Move move)
    {
        // We can't just cast to ShortMove (aka short), since moves with the highest bit of the second half set to 1
        // would be interpreted as negative numbers, and therefore throwing an IndexOutOfRangeException when widened back to int for array indexing.
        // By masking with 0xFFFF, we ensure that the move is always interpreted as an unsigned number.
        return _uciStrings[move & UCIMask];
    }

    /// <summary>
    /// First file letter, then rank number and finally the whole square.
    /// At least according to https://chess.stackexchange.com/a/1819
    /// </summary>
    private static string DisambiguateMove(Move move, Position position)
    {
        var piece = move.Piece(position.Board);
        var targetSquare = move.TargetSquare();

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, moves).ToArray();

#pragma warning disable MA0029 // Combine LINQ methods
        var movesWithSameSimpleRepresentation = pseudoLegalMoves
            .Where(m => m != move && m.Piece(position.Board) == piece && m.TargetSquare() == targetSquare)
            .Where(m =>
            {
                // If any illegal moves exist with the same simple representation there's no need to disambiguate
                var gameState = position.MakeMove(m);
                var isLegal = position.WasProduceByAValidMove();
                position.UnmakeMove(m, gameState);

                return isLegal;
            })
            .ToArray();
#pragma warning restore MA0029 // Combine LINQ methods

        if (movesWithSameSimpleRepresentation.Length == 0)
        {
            return string.Empty;
        }

        int sourceSquare = move.SourceSquare();
        var moveFile = Constants.File(sourceSquare);

        var files = movesWithSameSimpleRepresentation.Select(m => Constants.File(m.SourceSquare()));

        if (files.Any(f => f == moveFile))
        {
            var moveRank = Constants.Rank(sourceSquare);

            var ranks = movesWithSameSimpleRepresentation.Select(m => Constants.Rank(m.SourceSquare()));

            if (ranks.Any(r => r == moveRank))
            {
                return Constants.Coordinates[sourceSquare];
            }

            return (moveRank + 1).ToString();
        }

        return Constants.FileString[moveFile].ToString();
    }
}
