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
/// 0000 0001 0000 0000 0000 0000 0000 0000     0x100_0000      Capture flag (1)
/// 0000 1110 0000 0000 0000 0000 0000 0000     0xE00_0000      SpecialMoveFlagOffset: Double pawn push, en-passant, short castle or long castle (1-5)
/// Total: 28 bits -> fits an int
/// By casting it to ShortMove, a unique int16 (short) move is achieved, since
/// source and target square and promoted piece can only represent a move in a given position
/// </summary>
public static class MoveExtensions
{
    private const int SourceSquareOffset = 4;
    private const int TargetSquareOffset = 10;
    private const int PieceOffset = 16;
    private const int CapturedPieceOffset = 20;
    private const int IsCaptureOffset = 24;
    private const int SpecialMoveFlagOffset = 25;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move Encode(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeDoublePawnPush(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (int)SpecialMoveType.DoublePawnPush << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeEnPassant(int sourceSquare, int targetSquare, int piece, int capturedPiece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (capturedPiece << CapturedPieceOffset)
            | (1 << IsCaptureOffset)
            | (int)SpecialMoveType.EnPassant << SpecialMoveFlagOffset;
    }

    /// <summary>
    ///  Override when captured piece (aka side) isn't provided
    /// </summary>
    /// <param name="sourceSquare"></param>
    /// <param name="targetSquare"></param>
    /// <param name="piece"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeEnPassant(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (1 << IsCaptureOffset)
            | (int)SpecialMoveType.EnPassant << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeShortCastle(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (int)SpecialMoveType.ShortCastle << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeLongCastle(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (int)SpecialMoveType.LongCastle << SpecialMoveFlagOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeCapture(int sourceSquare, int targetSquare, int piece, int capturedPiece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (capturedPiece << CapturedPieceOffset)
            | (1 << IsCaptureOffset);
    }

    /// <summary>
    /// Override when captured piece isn't provided
    /// </summary>
    /// <param name="sourceSquare"></param>
    /// <param name="targetSquare"></param>
    /// <param name="piece"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeCapture(int sourceSquare, int targetSquare, int piece)
    {
        return (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (1 << IsCaptureOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodePromotion(int sourceSquare, int targetSquare, int piece, int promotedPiece)
    {
        return promotedPiece
            | (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodePromotion(int sourceSquare, int targetSquare, int piece, int promotedPiece, int capturedPiece)
    {
        return promotedPiece
            | (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (capturedPiece << CapturedPieceOffset)
            | (1 << IsCaptureOffset);
    }

    /// <summary>
    /// Override when captured piece isn't provided
    /// </summary>
    /// <param name="sourceSquare"></param>
    /// <param name="targetSquare"></param>
    /// <param name="piece"></param>
    /// <param name="promotedPiece"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodePromotionWithCapture(int sourceSquare, int targetSquare, int piece, int promotedPiece)
    {
        return promotedPiece
            | (sourceSquare << SourceSquareOffset)
            | (targetSquare << TargetSquareOffset)
            | (piece << PieceOffset)
            | (1 << IsCaptureOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Move EncodeCapturedPiece(int move, int capturedPiece) => move | (capturedPiece << 20);

    /// <summary>
    /// Returns the move from <paramref name="moveList"/> indicated by <paramref name="UCIString"/>
    /// </summary>
    /// <param name="UCIString"></param>
    /// <param name="moveList"></param>
    /// <param name="move"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <returns></returns>
    [Obsolete("Just intended for testing purposes")]
    public static bool TryParseFromUCIString(ReadOnlySpan<char> UCIString, Move[] moveList, [NotNullWhen(true)] out Move? move)
    {
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

            Utils.Assert(candidateMoves.Count() == 4);
            Utils.Assert(candidateMoves.Count(predicate) == 1);

            return true;
        }
    }

    /// <summary>
    /// Returns the move from <paramref name="moveList"/> indicated by <paramref name="UCIString"/>
    /// </summary>
    /// <param name="UCIString"></param>
    /// <param name="moveList"></param>
    /// <param name="move"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <returns></returns>
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

                    Debug.Assert(moveList.Length >= 4, "There will be at least 4 moves that match sourceSquare and targetSquare when there is a promotion");
                    Debug.Assert(moveList.ToArray().Count(m => m.PromotedPiece() != default) == 4 || moveList.ToArray().Count(m => m.PromotedPiece() != default) == 8, "There will be either 4 or 8 moves that are a promotion");
                    Debug.Assert(moveList.ToArray().Count(m => m.SourceSquare() == sourceSquare && m.TargetSquare() == targetSquare && m.PromotedPiece() != default) == 4, "There will be 4 (and always 4) moves that match sourceSquare and targetSquare when there is a promotion");
                }
            }
        }

        _logger.Warn("Unable to link last move string {0} to a valid move in the current position. That move may have already been played", UCIString.ToString());
        move = null;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PromotedPiece(this Move move) => move & 0xF;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPromotion(this Move move) => (move & 0xF) != default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SourceSquare(this Move move) => (move & 0x3F0) >> SourceSquareOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TargetSquare(this Move move) => (move & 0xFC00) >> TargetSquareOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Piece(this Move move) => (move & 0xF_0000) >> PieceOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CapturedPiece(this Move move) => (move & 0xF0_0000) >> CapturedPieceOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCapture(this Move move) => (move & 0x100_0000) >> IsCaptureOffset != default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SpecialMoveType SpecialMoveFlag(this Move move) => (SpecialMoveType)((move & 0xE00_0000) >> SpecialMoveFlagOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDoublePawnPush(this Move move) => (move & 0xE00_0000) >> SpecialMoveFlagOffset == (int)SpecialMoveType.DoublePawnPush;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnPassant(this Move move) => (move & 0xE00_0000) >> SpecialMoveFlagOffset == (int)SpecialMoveType.EnPassant;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShortCastle(this Move move) => (move & 0xE00_0000) >> SpecialMoveFlagOffset == (int)SpecialMoveType.ShortCastle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLongCastle(this Move move) => (move & 0xE00_0000) >> SpecialMoveFlagOffset == (int)SpecialMoveType.LongCastle;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCastle(this Move move) => (move & 0xE00_0000) >> SpecialMoveFlagOffset >= (int)SpecialMoveType.ShortCastle;

    /// <summary>
    /// Typical format when humans write moves
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToMoveString(this Move move)
    {
#pragma warning disable S3358 // Ternary operators should not be nested
        return move.SpecialMoveFlag() switch
        {
            SpecialMoveType.ShortCastle => "0-0",
            SpecialMoveType.LongCastle => "0-0-O",
            _ =>
                Constants.AsciiPieces[move.Piece()] +
                Constants.Coordinates[move.SourceSquare()] +
                (move.IsCapture() ? "x" : "") +
                Constants.Coordinates[move.TargetSquare()] +
                (move.PromotedPiece() == default ? "" : $"={Constants.AsciiPieces[move.PromotedPiece()]}") +
                (move.IsEnPassant() ? "e.p." : "")
        };
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    [Obsolete(
        "Consider using the override that accepts a position for fully compliant EPD/PGN string representation of the move. " +
        "This method be removed/renamed in future versions")]
    public static string ToEPDString(this Move move)
    {
        var piece = move.Piece();

#pragma warning disable S3358 // Ternary operators should not be nested
        return move.SpecialMoveFlag() switch
        {
            SpecialMoveType.ShortCastle => "0-0",
            SpecialMoveType.LongCastle => "0-0-O",
            _ =>
                (piece == (int)Model.Piece.P || piece == (int)Model.Piece.p
                    ? (move.IsCapture()
                        ? Constants.Coordinates[move.SourceSquare()][..^1]  // exd5
                        : "")    // d5
                    : char.ToUpperInvariant(Constants.AsciiPieces[move.Piece()]))
                + (move.IsCapture() == default ? "" : "x")
                + Constants.Coordinates[move.TargetSquare()]
                + (move.PromotedPiece() == default ? "" : $"={char.ToUpperInvariant(Constants.AsciiPieces[move.PromotedPiece()])}")
                + (move.IsEnPassant() == default ? "" : "e.p.")
        };
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    /// <summary>
    /// EPD representation of a valid move in a position
    /// </summary>
    /// <param name="move">A valid move for the given position</param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static string ToEPDString(this Move move, Position position)
    {
        var piece = move.Piece();

#pragma warning disable S3358 // Ternary operators should not be nested
        return move.SpecialMoveFlag() switch
        {
            SpecialMoveType.ShortCastle => "0-0",
            SpecialMoveType.LongCastle => "0-0-O",
            _ =>
                (piece == (int)Model.Piece.P || piece == (int)Model.Piece.p
                    ? (move.IsCapture()
                        ? global::Lynx.Constants.Coordinates[move.SourceSquare()][..^1]  // exd5
                        : "")    // d5
                    : char.ToUpperInvariant(global::Lynx.Constants.AsciiPieces[move.Piece()]))
                + DisambiguateMove(move, position)
                + (move.IsCapture() == default ? "" : "x")
                + Constants.Coordinates[move.TargetSquare()]
                + (move.PromotedPiece() == default ? "" : $"={char.ToUpperInvariant(Constants.AsciiPieces[move.PromotedPiece()])}")
                + (move.IsEnPassant() == default ? "" : "e.p.")
        };
#pragma warning restore S3358 // Ternary operators should not be nested

        // https://chess.stackexchange.com/a/1819
        static string DisambiguateMove(Move move, Position position)
        {
            var piece = move.Piece();
            var targetSquare = move.TargetSquare();

            Span<Move> moves = stackalloc Move[Constants.MaxNumberOfPossibleMovesInAPosition];
            var pseudoLegalMoves = MoveGenerator.GenerateAllMoves(position, moves).ToArray();

            var movesWithSameSimpleRepresentation = pseudoLegalMoves
                .Where(m => m != move && m.Piece() == piece && m.TargetSquare() == targetSquare)
                .Where(m =>
                {
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

                if (ranks.Any(f => f == moveRank))
                {
                    return Constants.Coordinates[sourceSquare];
                }

                return (moveRank + 1).ToString();
            }

            return Constants.FileString[moveFile].ToString();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string UCIString(this Move move)
    {
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
}
