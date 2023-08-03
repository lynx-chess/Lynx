using NLog;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lynx.Model;

/// <summary>
/// <para>int Value:</para>
/// <para>
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
/// </para>
/// </summary>
/// </summary>
public static class MoveExtensions
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
    public static Move Encode(
        int sourceSquare, int targetSquare, int piece, int promotedPiece = default,
        int isCapture = default, int isDoublePawnPush = default, int isEnPassant = default,
        int isShortCastle = default, int isLongCastle = default)
    {
        return sourceSquare | (targetSquare << 6) | (piece << 12) | (promotedPiece << 16)
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
    /// <param name="move"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <returns></returns>
    public static bool TryParseFromUCIString(string UCIString, IEnumerable<Move> moveList, [NotNullWhen(true)] out Move? move)
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
                _logger.Warn("Unable to link last move string {0} to a valid move in the current position. That move may have already been played", UCIString);
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
                _logger.Warn("Unable to link move {0} to a valid move in the current position. That move may have already been played", UCIString);
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
    public static bool TryParseFromUCIString(string UCIString, ref Span<Move> moveList, [NotNullWhen(true)] out Move? move)
    {
        Utils.Assert(UCIString.Length == 4 || UCIString.Length == 5);

        var sourceSquare = (UCIString[0] - 'a') + ((8 - (UCIString[1] - '0')) * 8);
        var targetSquare = (UCIString[2] - 'a') + ((8 - (UCIString[3] - '0')) * 8);

        var candidateMoves = moveList.ToImmutableArray().Where(move => move.SourceSquare() == sourceSquare && move.TargetSquare() == targetSquare);

        if (UCIString.Length == 4)
        {
            move = candidateMoves.FirstOrDefault();

            if (move.Equals(default(Move)))
            {
                _logger.Warn("Unable to link last move string {0} to a valid move in the current position. That move may have already been played", UCIString);
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
                _logger.Warn("Unable to link move {0} to a valid move in the current position. That move may have already been played", UCIString);
                move = null;
                return false;
            }

            Utils.Assert(candidateMoves.Count() == 4);
            Utils.Assert(candidateMoves.Count(predicate) == 1);

            return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SourceSquare(this Move move) => move & 0x3F;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TargetSquare(this Move move) => (move & 0xFC0) >> 6;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Piece(this Move move) => (move & 0xF000) >> 12;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PromotedPiece(this Move move) => (move & 0xF0000) >> 16;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCapture(this Move move) => (move & 0x10_0000) >> 20 != default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDoublePawnPush(this Move move) => (move & 0x20_0000) >> 21 != default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnPassant(this Move move) => (move & 0x40_0000) >> 22 != default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShortCastle(this Move move) => (move & 0x80_0000) >> 23 != default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLongCastle(this Move move) => (move & 0x100_0000) >> 24 != default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCastle(this Move move) => (move & 0x180_0000) >> 23 != default;

    /// <summary>
    /// Returns the score evaluation of a move taking into account <see cref="EvaluationConstants.MostValueableVictimLeastValuableAttacker"/>
    /// </summary>
    /// <param name="position">The position that precedes a move</param>
    /// <param name="killerMoves"></param>
    /// <param name="plies"></param>
    /// <param name="historyMoves"></param>
    /// <returns>The higher the score is, the more valuable is the captured piece and the less valuable is the piece that makes the such capture</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Score(this Move move, in Position position, int[,]? killerMoves = null, int? plies = null, int[,]? historyMoves = null)
    {
        int score = 0;

        if (move.IsCapture())
        {
            var sourcePiece = move.Piece();
            int targetPiece = (int)Model.Piece.P;    // Important to initialize to P or p, due to en-passant captures

            var targetSquare = move.TargetSquare();
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

            score += EvaluationConstants.CaptureMoveBaseScoreValue + EvaluationConstants.MostValueableVictimLeastValuableAttacker[sourcePiece, targetPiece];
        }
        else
        {
            if (killerMoves is not null && plies is not null)
            {
                // 1st killer move
                if (killerMoves[0, plies.Value] == move)
                {
                    return EvaluationConstants.FirstKillerMoveValue;
                }

                // 2nd killer move
                else if (killerMoves[1, plies.Value] == move)
                {
                    return EvaluationConstants.SecondKillerMoveValue;
                }

                // History move
                else if (historyMoves is not null)
                {
                    return historyMoves[move.Piece(), move.TargetSquare()];
                }
            }
        }

        return score;
    }

    public static string ToMoveString(this Move move)
    {
#pragma warning disable S3358 // Ternary operators should not be nested
        return move.IsCastle() == default
            ?
                Constants.AsciiPieces[move.Piece()] +
                Constants.Coordinates[move.SourceSquare()] +
                (move.IsCapture() == default ? "" : "x") +
                Constants.Coordinates[move.TargetSquare()] +
                (move.PromotedPiece() == default ? "" : $"={Constants.AsciiPieces[move.PromotedPiece()]}") +
                (move.IsEnPassant() == default ? "" : "e.p.")
            : (move.IsShortCastle() ? "O-O" : "O-O-O");
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    /// <summary>
    /// Typical format when humans write moves
    /// </summary>
    /// <returns></returns>
    public static string ToEPDString(this Move move)
    {
        var piece = move.Piece();
#pragma warning disable S3358 // Ternary operators should not be nested
        return move.IsCastle() == default
            ?
                (piece == (int)Model.Piece.P || piece == (int)Model.Piece.p
                    ? (move.IsCapture()
                        ? Constants.Coordinates[move.SourceSquare()][..^1]  // exd5
                        : "")    // d5
                    : char.ToUpperInvariant(Constants.AsciiPieces[move.Piece()])) +
                (move.IsCapture() == default ? "" : "x") +
                Constants.Coordinates[move.TargetSquare()] +
                (move.PromotedPiece() == default ? "" : $"={char.ToUpperInvariant(Constants.AsciiPieces[move.PromotedPiece()])}") +
                (move.IsEnPassant() == default ? "" : "e.p.")
            : (move.IsShortCastle() ? "O-O" : "O-O-O");
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    public static void Print(this Move move)
    {
        Console.WriteLine(move.ToMoveString());
    }

    public static string UCIString(this Move move)
    {
        return
            Constants.Coordinates[move.SourceSquare()] +
            Constants.Coordinates[move.TargetSquare()] +
            (move.PromotedPiece() == default ? "" : $"{Constants.AsciiPieces[move.PromotedPiece()].ToString().ToLowerInvariant()}");
    }

    public static void PrintMoveList(this IEnumerable<Move> moves)
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
