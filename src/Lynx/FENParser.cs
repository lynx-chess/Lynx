﻿using Lynx.Model;
using NLog;
using System.Runtime.CompilerServices;

using ParseResult = (bool Success, ulong[] PieceBitBoards, ulong[] OccupancyBitBoards, Lynx.Model.Side Side, byte Castle, Lynx.Model.BoardSquare EnPassant,
            int HalfMoveClock, int FullMoveCounter);

namespace Lynx;

public static class FENParser
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static ParseResult ParseFEN(ReadOnlySpan<char> fen)
    {
        fen = fen.Trim();

        var pieceBitBoards = new BitBoard[12];
        var occupancyBitBoards = new BitBoard[3];

        bool success;
        Side side = Side.Both;
        byte castle = 0;
        int halfMoveClock = 0, fullMoveCounter = 1;
        BoardSquare enPassant = BoardSquare.noSquare;

        try
        {
            success = ParseBoard(fen, pieceBitBoards, occupancyBitBoards);

            var unparsedStringAsSpan = fen[fen.IndexOf(' ')..];
            Span<Range> parts = stackalloc Range[5];
            var partsLength = unparsedStringAsSpan.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

            if (partsLength < 3)
            {
                throw new($"Error parsing second half (after board) of fen {fen}");
            }

            side = ParseSide(unparsedStringAsSpan[parts[0]]);

            castle = ParseCastlingRights(unparsedStringAsSpan[parts[1]]);

            (enPassant, success) = ParseEnPassant(unparsedStringAsSpan[parts[2]], pieceBitBoards, side);

            if (partsLength < 4 || !int.TryParse(unparsedStringAsSpan[parts[3]], out halfMoveClock))
            {
                _logger.Debug("No half move clock detected");
            }

            if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
            {
                _logger.Debug("No full move counter detected");
            }
        }
        catch (Exception e)
        {
            _logger.Error("Error parsing FEN string {0}", fen.ToString());
            _logger.Error(e.Message);
            success = false;
        }

        return (success, pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock, fullMoveCounter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ParseBoard(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
    {
        bool success = true;
        var rankIndex = 0;
        var end = fen.IndexOf('/');

        while (success && end != -1)
        {
            var match = fen[..end];

            ParseBoardSection(pieceBitBoards, ref success, rankIndex, match);
            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            fen = fen[(end + 1)..];
            end = fen.IndexOf('/');
            ++rankIndex;
        }

        ParseBoardSection(pieceBitBoards, ref success, rankIndex, fen[..fen.IndexOf(' ')]);
        PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

        return success;

        static void ParseBoardSection(ulong[] pieceBitBoards, ref bool success, int rankIndex, ReadOnlySpan<char> boardfenSection)
        {
            int fileIndex = 0;

            foreach (var ch in boardfenSection)
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
                    _logger.Error("Unrecognized character in FEN: {0} (within {1})", ch, boardfenSection.ToString());
                    success = false;
                    break;
                }
            }
        }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Side ParseSide(ReadOnlySpan<char> side)
    {
        return side[0] switch
        {
            'w' or 'W' => Side.White,
            'b' or 'B' => Side.Black,
            _ => throw new($"Unrecognized side: {side}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ParseCastlingRights(ReadOnlySpan<char> castling)
    {
        byte castle = 0;

        for (int i = 0; i < castling.Length; ++i)
        {
            castle |= castling[i] switch
            {
                'K' => (byte)CastlingRights.WK,
                'Q' => (byte)CastlingRights.WQ,
                'k' => (byte)CastlingRights.BK,
                'q' => (byte)CastlingRights.BQ,
                '-' => castle,
                _ => throw new($"Unrecognized castling char: {castling[i]}")
            };
        }

        return castle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (BoardSquare EnPassant, bool Success) ParseEnPassant(ReadOnlySpan<char> enPassantSpan, BitBoard[] PieceBitBoards, Side side)
    {
        bool success = true;
        BoardSquare enPassant = BoardSquare.noSquare;

        if (Enum.TryParse(enPassantSpan, ignoreCase: true, out BoardSquare result))
        {
            enPassant = result;

            var rank = 1 + ((int)enPassant >> 3);
            if (rank != 3 && rank != 6)
            {
                success = false;
                _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
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
                _logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantSpan.ToString(), side, pawnSquare);
            }
        }
        else if (enPassantSpan[0] != '-')
        {
            success = false;
            _logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
        }

        return (enPassant, success);
    }
}
