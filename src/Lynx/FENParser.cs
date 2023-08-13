using Lynx.Model;
using NLog;
using System.Text.RegularExpressions;

namespace Lynx;

public static partial class FENParser
{
    [GeneratedRegex("(?<=^|\\/)[P|N|B|R|Q|K|p|n|b|r|q|k|\\d]{1,8}", RegexOptions.Compiled)]
    private static partial Regex RanksRegex();

    private static readonly Regex _ranksRegex = RanksRegex();

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static (bool Success, BitBoard[] PieceBitBoards, BitBoard[] OccupancyBitBoards, Side Side, byte Castle, BoardSquare EnPassant,
        int HalfMoveClock, int FullMoveCounter) ParseFEN(string fen)
    {
        fen = fen.Trim();

        var pieceBitBoards = new BitBoard[12] {
                default, default, default, default,
                default, default, default, default,
                default, default, default, default};

        var occupancyBitBoards = new BitBoard[3] { default, default, default };

        bool success;
        Side side = Side.Both;
        byte castle = 0;
        int halfMoveClock = 0, fullMoveCounter = 1;
        BoardSquare enPassant = BoardSquare.noSquare;

        try
        {
            MatchCollection matches;
            (matches, success) = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

            var unparsedString = fen[(matches[^1].Index + matches[^1].Length)..];
            var parts = unparsedString.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 3)
            {
                throw new($"Error parsing second half (after board) of fen {fen}");
            }

            side = ParseSide(parts[0]);

            castle = ParseCastlingRights(parts[1]);

            (enPassant, success) = ParseEnPassant(parts[2], pieceBitBoards, side);

            if (parts.Length < 4 || !int.TryParse(parts[3], out halfMoveClock))
            {
                _logger.Debug("No half move clock detected");
            }

            if (parts.Length < 5 || !int.TryParse(parts[4], out fullMoveCounter))
            {
                _logger.Debug("No full move counter detected");
            }
        }
        catch (Exception e)
        {
            _logger.Error("Error parsing FEN string {0}", fen);
            _logger.Error(e.Message);
            success = false;
        }

        return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
    }

    private static (MatchCollection Matches, bool Success) ParseBoard(string fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
    {
        bool success = true;

        var rankIndex = 0;
        var matches = _ranksRegex.Matches(fen);

        if (matches.Count != 8)
        {
            return (matches, false);
        }

        foreach (var match in matches)
        {
            var fileIndex = 0;
            foreach (var ch in ((Group)match).Value)
            {
                if (Constants.PiecesByChar.TryGetValue(ch, out Piece piece))
                {
                    pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                    ++fileIndex;
                }
                else if (int.TryParse($"{ch}", out int emptySquares))
                {
                    fileIndex += emptySquares;
                }
                else
                {
                    _logger.Error("Unrecognized character in FEN: {0} (within {1})", ch, ((Group)match).Value);
                    success = false;
                    break;
                }
            }

            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            ++rankIndex;
        }

        return (matches, success);

        static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            var limit = (int)Piece.K;
            for (int piece = (int)Piece.P; piece <= limit; ++piece)
            {
                occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
            }

            limit = (int)Piece.k;
            for (int piece = (int)Piece.p; piece <= limit; ++piece)
            {
                occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece];
            }

            occupancyBitBoards[(int)Side.Both] = occupancyBitBoards[(int)Side.White] | occupancyBitBoards[(int)Side.Black];
        }
    }

    private static Side ParseSide(string sideString)
    {
#pragma warning disable S3358 // Ternary operators should not be nested
        bool isWhite = sideString.Equals("w", StringComparison.OrdinalIgnoreCase);

        return isWhite || sideString.Equals("b", StringComparison.OrdinalIgnoreCase)
            ? isWhite ? Side.White : Side.Black
            : throw new($"Unrecognized side: {sideString}");
#pragma warning restore S3358 // Ternary operators should not be nested
    }

    private static byte ParseCastlingRights(string castleString)
    {
        byte castle = 0;

        foreach (var ch in castleString)
        {
            castle |= ch switch
            {
                'K' => (byte)CastlingRights.WK,
                'Q' => (byte)CastlingRights.WQ,
                'k' => (byte)CastlingRights.BK,
                'q' => (byte)CastlingRights.BQ,
                '-' => castle,
                _ => throw new($"Unrecognized castling char: {ch}")
            };
        }

        return castle;
    }

    private static (BoardSquare EnPassant, bool Success) ParseEnPassant(string enPassantString, BitBoard[] PieceBitBoards, Side side)
    {
        bool success = true;
        BoardSquare enPassant = BoardSquare.noSquare;

        if (Enum.TryParse(enPassantString, ignoreCase: true, out BoardSquare result))
        {
            enPassant = result;

            var rank = 1 + (int)enPassant / 8;
            if (rank != 3 && rank != 6)
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantString);
            }

            // Check that there's an actual pawn to be captured
            var pawnOffset = side == Side.White
                ? +8
                : -8;

            var pawnSquare = (int)enPassant + pawnOffset;

            var pawnBitBoard = side == Side.White
                ? PieceBitBoards[(int)Piece.p]
                : PieceBitBoards[(int)Piece.P];

            if (!pawnBitBoard.GetBit(pawnSquare))
            {
                success = false;
                _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantString, side, pawnSquare);
            }
        }
        else if (enPassantString != "-")
        {
            success = false;
            _logger.Error("Invalid en passant square: {0}", enPassantString);
        }

        return (enPassant, success);
    }
}
