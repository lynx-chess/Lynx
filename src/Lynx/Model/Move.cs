using NLog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Lynx.Model;

public enum SpecialMoveType
{
    None = 0,
    DoublePawnPush = 1,
    EnPassant = 2,
    ShortCastle = 3,
    LongCastle = 4
}

/// <summary>
///            Binary move bits               Hexadecimal
/// 0000 0000 0000 0000 0000 0000 0000 1111     0xF             Promoted piece (0-11)
/// 0000 0000 0000 0000 0000 0011 1111 0000     0x3F0           Source square (0-63)
/// 0000 0000 0000 0000 1111 1100 0000 0000     0xFC00          Target Square (0-63)
/// --------------------------------------------------------------------------------------------
/// 0000 0000 0000 1111 0000 0000 0000 0000     0xF_0000        Piece (0-11)
/// 0000 0000 1111 0000 0000 0000 0000 0000     0xF0_0000       Captured piece (0-11)
/// 0000 0111 0000 0000 0000 0000 0000 0000     0x700_0000      SpecialMoveFlagOffset: Double pawn push, en-passant, short castle or long castle (1-5)
/// Total: 27 bits -> fits an int
/// By casting it to ShortMove, a unique int16 (short) move is achieved, since
/// source and target square and promoted piece can only represent a move in a given position
/// </summary>
public static class MoveExtensions
{
    private const int SourceSquareOffset = 4;
    private const int TargetSquareOffset = 10;
    private const int PieceOffset = 16;
    private const int CapturedPieceOffset = 20;
    private const int SpecialMoveFlagOffset = 24;

    private const int SpecialMoveMask = 0x700_0000;
    private const int PromotedPieceMask = 0xF;
    private const int SourceSquareMask = 0x3F0;
    private const int TargetSquareMask = 0xFC00;
    private const int PieceMask = 0xF_0000;
    private const int CapturedPieceMask = 0xF0_0000;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Move to represent a null move that fits in 12x64 arrays
    /// </summary>
    public static readonly Move NullMove = Encode((int)BoardSquare.e1, (int)BoardSquare.e1, (int)Model.Piece.P);

    /// <summary>
    /// Encodes non-capturing moves
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move Encode(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | ((int)Model.Piece.None << CapturedPieceOffset);
    }

    /// <summary>
    /// Encodes capture and non-capturing moves
    /// </summary>
    /// <param name="capturedPiece">Captured piece, or otherwise <see cref="Model.Piece.None"/> if there's not capture</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move Encode(int sourceSquare, int targetSquare, int piece, int capturedPiece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (capturedPiece << CapturedPieceOffset);
    }

    /// <summary>
    /// Encodes capturing move
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeCapture(int sourceSquare, int targetSquare, int piece, int capturedPiece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (capturedPiece << CapturedPieceOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeDoublePawnPush(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | ((int)Model.Piece.None << CapturedPieceOffset)
            | (int)SpecialMoveType.DoublePawnPush << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeEnPassant(int sourceSquare, int targetSquare, int piece, int capturedPiece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (capturedPiece << CapturedPieceOffset)
            | (int)SpecialMoveType.EnPassant << SpecialMoveFlagOffset;
    }

    /// <summary>
    ///  Override when captured piece (aka side) isn't provided (not needed for IsValidMove)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeEnPassant(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (int)SpecialMoveType.EnPassant << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeShortCastle(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | ((int)Model.Piece.None << CapturedPieceOffset)
            | (int)SpecialMoveType.ShortCastle << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeLongCastle(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | ((int)Model.Piece.None << CapturedPieceOffset)
            | (int)SpecialMoveType.LongCastle << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodePromotion(int sourceSquare, int targetSquare, int piece, int promotedPiece)
    {
        return promotedPiece
            | (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | ((int)Model.Piece.None << CapturedPieceOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodePromotion(int sourceSquare, int targetSquare, int piece, int promotedPiece, int capturedPiece)
    {
        return promotedPiece
            | (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (capturedPiece << CapturedPieceOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodePromotionFromPawnMove(Move pawnMove, int promotedPiece)
        => pawnMove | promotedPiece;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeCapturedPiece(Move move, int capturedPiece)
        => move | (capturedPiece << CapturedPieceOffset);

    /// <summary>
    /// Returns the move from <paramref name="moveList"/> indicated by <paramref name="UCIString"/>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="IndexOutOfRangeException"></exception>
    [Obsolete("Just intended for testing purposes")]
    public static bool TryParseFromUCIString(ReadOnlySpan<char> UCIString, Move[] moveList, [NotNullWhen(true)] out Move? move)
    {
#pragma warning disable CA1851 // Possible multiple enumerations of 'IEnumerable' collection

        Utils.Assert(UCIString.Length == 4 || UCIString.Length == 5);

        var sourceSquare = (UCIString[0] - 'a') + ((8 - (UCIString[1] - '0')) * 8);
        var targetSquare = (UCIString[2] - 'a') + ((8 - (UCIString[3] - '0')) * 8);

        var candidateMoves = moveList.Where(move => move.SourceSquare() == sourceSquare && move.TargetSquare() == targetSquare);

        if (UCIString.Length == 4)
        {
            move = candidateMoves.FirstOrDefault();

            if (move.Equals(default(Move)))
            {
                _logger.Warn("Unable to link last move string {0} to a valid move in the current position. That move may have already been played", UCIString.ToString());
                move = null;
                return false;
            }

            Utils.Assert(move.Value.PromotedPiece() == default);
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
                _logger.Warn("Unable to link move {0} to a valid move in the current position. That move may have already been played", UCIString.ToString());
                move = null;
                return false;
            }

            Debug.Assert(candidateMoves.Count() == 4);
            Debug.Assert(candidateMoves.Count(predicate) == 1);

            return true;

#pragma warning restore CA1851 // Possible multiple enumerations of 'IEnumerable' collection
        }
    }

    /// <summary>
    /// Returns the move from <paramref name="moveList"/> indicated by <paramref name="UCIString"/>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public static bool TryParseFromUCIString(ReadOnlySpan<char> UCIString, ReadOnlySpan<Move> moveList, [NotNullWhen(true)] out Move? move)
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
                    Debug.Assert(candidateMove.PromotedPiece() == default);

                    move = candidateMove;
                    return true;
                }
                else
                {
                    var promotedPiece = (int)Enum.Parse<Piece>(UCIString[4].ToString());
                    var candidatePromotedPiece = candidateMove.PromotedPiece();

                    if (candidatePromotedPiece == promotedPiece
                        || candidatePromotedPiece == promotedPiece - 6)
                    {
                        move = candidateMove;
                        return true;
                    }

                    Debug.Assert(moveList.Length >= 4, "Assert fail", "There will be at least 4 moves that match sourceSquare and targetSquare when there is a promotion");
                    Debug.Assert(moveList.ToArray().Count(m => m.PromotedPiece() != default) == 4 || moveList.ToArray().Count(m => m.PromotedPiece() != default) == 8, "Assert fail", "There will be either 4 or 8 moves that are a promotion");
                    Debug.Assert(moveList.ToArray().Count(m => m.SourceSquare() == sourceSquare && m.TargetSquare() == targetSquare && m.PromotedPiece() != default) == 4, "Assert fail", "There will be 4 (and always 4) moves that match sourceSquare and targetSquare when there is a promotion");
                }
            }
        }

        _logger.Warn("Unable to link last move string {0} to a valid move in the current position. That move may have already been played", UCIString.ToString());
        move = null;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PromotedPiece(this Move move) => move & PromotedPieceMask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPromotion(this Move move) => (move & PromotedPieceMask) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SourceSquare(this Move move) => (move & SourceSquareMask) >> SourceSquareOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TargetSquare(this Move move) => (move & TargetSquareMask) >> TargetSquareOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Piece(this Move move) => (move & PieceMask) >> PieceOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CapturedPiece(this Move move) => (move & CapturedPieceMask) >> CapturedPieceOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpecialMoveType SpecialMoveFlag(this Move move) => (SpecialMoveType)((move & SpecialMoveMask) >> SpecialMoveFlagOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDoublePawnPush(this Move move) => (move & SpecialMoveMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.DoublePawnPush;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnPassant(this Move move) => (move & SpecialMoveMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.EnPassant;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShortCastle(this Move move) => (move & SpecialMoveMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.ShortCastle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLongCastle(this Move move) => (move & SpecialMoveMask) >> SpecialMoveFlagOffset == (int)SpecialMoveType.LongCastle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCastle(this Move move) => (move & SpecialMoveMask) >> SpecialMoveFlagOffset >= (int)SpecialMoveType.ShortCastle;

    [Obsolete(
        "Consider using the override that accepts a position for fully compliant EPD/PGN string representation of the move. " +
        "This method be removed/renamed in future versions")]
    internal static string ToEPDString(this Move move)
    {
        var piece = move.Piece();
        var capturedPiece = move.CapturedPiece();

#pragma warning disable S3358 // Ternary operators should not be nested
        return move.SpecialMoveFlag() switch
        {
            SpecialMoveType.ShortCastle => "O-O",
            SpecialMoveType.LongCastle => "O-O-O",
            _ =>
                (piece == (int)Model.Piece.P || piece == (int)Model.Piece.p
                    ? (capturedPiece != (int)Model.Piece.None
                        ? Constants.Coordinates[move.SourceSquare()][..^1]  // exd5
                        : "")    // d5
                    : char.ToUpperInvariant(Constants.AsciiPieces[move.Piece()]))

                + (capturedPiece == (int)Model.Piece.None ? "" : "x")
                + Constants.Coordinates[move.TargetSquare()]
                + (move.PromotedPiece() == default ? "" : $"={char.ToUpperInvariant(Constants.AsciiPieces[move.PromotedPiece()])}")
        };
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    /// <summary>
    /// EPD representation of a valid move in a position
    /// </summary>
    /// <param name="move">A valid move for the given position</param>
    public static string ToEPDString(this Move move, Position position)
    {
        var piece = move.Piece();
        var capturedPiece = move.CapturedPiece();

#pragma warning disable S3358 // Ternary operators should not be nested
        return move.SpecialMoveFlag() switch
        {
            SpecialMoveType.ShortCastle => "O-O",
            SpecialMoveType.LongCastle => "O-O-O",
            _ =>
                (piece == (int)Model.Piece.P || piece == (int)Model.Piece.p
                    ? (capturedPiece != (int)Model.Piece.None
                        ? global::Lynx.Constants.FileString[global::Lynx.Constants.File[move.SourceSquare()]]  // exd5
                        : "")    // d5
                    : (char.ToUpperInvariant(global::Lynx.Constants.AsciiPieces[move.Piece()]))
                        + DisambiguateMove(move, position))
                + (capturedPiece == (int)Model.Piece.None ? "" : "x")
                + Constants.Coordinates[move.TargetSquare()]
                + (move.PromotedPiece() == default ? "" : $"={char.ToUpperInvariant(Constants.AsciiPieces[move.PromotedPiece()])}")
        };
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string UCIString(this Move move)
    {
        // TODO memoize them with dict or even array?
        Span<char> span = stackalloc char[5];

        var source = Constants.CoordinatesCharArray[move.SourceSquare()];
        var target = Constants.CoordinatesCharArray[move.TargetSquare()];

        span[0] = source[0];
        span[1] = source[1];
        span[2] = target[0];
        span[3] = target[1];

        var promotedPiece = move.PromotedPiece();
        if (promotedPiece != default)
        {
            span[4] = Constants.AsciiPiecesLowercase[promotedPiece];

            return span.ToString();
        }

        return span[..^1].ToString();
    }

    public static readonly Dictionary<int, string> UCIStringCache = new(4096);

    /// <summary>
    /// NOT thread-safe
    /// </summary>
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string UCIStringMemoized(this Move move)
    {
        if (UCIStringCache.TryGetValue(move, out var uciString))
        {
            return uciString;
        }

        var str = move.UCIString();
        UCIStringCache[move] = str;

        return str;
    }

    /// <summary>
    /// First file letter, then rank number and finally the whole square.
    /// At least according to https://chess.stackexchange.com/a/1819
    /// </summary>
    private static string DisambiguateMove(Move move, Position position)
    {
        var piece = move.Piece();
        var targetSquare = move.TargetSquare();

        Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPseudolegalMovesInAPosition];

        Span<BitBoard> attacks = stackalloc BitBoard[12];
        Span<BitBoard> attacksBySide = stackalloc BitBoard[2];
        var evaluationContext = new EvaluationContext(attacks, attacksBySide);

        var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, ref evaluationContext, moves).ToArray();

        var movesWithSameSimpleRepresentation = pseudoLegalMoves
            .Where(m => m != move && m.Piece() == piece && m.TargetSquare() == targetSquare)
            .Where(m =>
            {
                // If any illegal moves exist with the same simple representation there's no need to disambiguate
                var gameState = position.MakeMove(m);
                var isLegal = position.WasProduceByAValidMove();
                position.UnmakeMove(m, gameState);

                return isLegal;
            })
            .ToArray();

        if (movesWithSameSimpleRepresentation.Length == 0)
        {
            return string.Empty;
        }

        int sourceSquare = move.SourceSquare();
        var moveFile = Constants.File[sourceSquare];

        var files = movesWithSameSimpleRepresentation.Select(m => Constants.File[m.SourceSquare()]);

        if (files.Any(f => f == moveFile))
        {
            var moveRank = Constants.Rank[sourceSquare];

            var ranks = movesWithSameSimpleRepresentation.Select(m => Constants.Rank[m.SourceSquare()]);

            if (ranks.Any(r => r == moveRank))
            {
                return Constants.Coordinates[sourceSquare];
            }

            return (moveRank + 1).ToString();
        }

        return Constants.FileString[moveFile].ToString();
    }
}
