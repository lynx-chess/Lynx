using Lynx.Model;
//using NLog;
using System.Runtime.CompilerServices;

using ParseResult = (ulong[] PieceBitBoards, ulong[] OccupancyBitBoards, Lynx.Model.Side Side, byte Castle, Lynx.Model.BoardSquare EnPassant,
            int HalfMoveClock/*, int FullMoveCounter*/);

namespace Lynx;

public static class FENParser
{
    //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParseResult ParseFEN(ReadOnlySpan<char> fen)
    {
        fen = fen.Trim();

        var pieceBitBoards = new BitBoard[12];
        var occupancyBitBoards = new BitBoard[3];

        bool success;
        Side side = Side.Both;
        byte castle = 0;
        int halfMoveClock = 0/*, fullMoveCounter = 1*/;
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
                //_logger.Debug("No half move clock detected");
            }

            //if (partsLength < 5 || !int.TryParse(unparsedStringAsSpan[parts[4]], out fullMoveCounter))
            //{
            //    //_logger.Debug("No full move counter detected");
            //}
        }
        catch (Exception e)
        {
            //_logger.Error(e, "Error parsing FEN");
            success = false;
            throw;
        }

        return success
            ? (pieceBitBoards, occupancyBitBoards, side, castle, enPassant, halfMoveClock/*, fullMoveCounter*/)
            : throw new AssertException($"Error parsing {fen.ToString()}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ParseBoard(ReadOnlySpan<char> fen, BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
    {
        bool success = true;
        var rankIndex = 0;
        var end = fen.IndexOf('/');

        while (
            end != -1
#if DEBUG
            && success
#endif
            )
        {
            var match = fen[..end];

            ParseBoardSection(pieceBitBoards, rankIndex, match
#if DEBUG
            , ref success
#endif
                );
            PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

            fen = fen[(end + 1)..];
            end = fen.IndexOf('/');
            ++rankIndex;
        }

        ParseBoardSection(pieceBitBoards, rankIndex, fen[..fen.IndexOf(' ')]
#if DEBUG
            , ref success
#endif
            );
        PopulateOccupancies(pieceBitBoards, occupancyBitBoards);

        return success;

        static void ParseBoardSection(BitBoard[] pieceBitBoards, int rankIndex, ReadOnlySpan<char> boardfenSection
#if DEBUG
            , ref bool success
#endif
            )
        {
            int fileIndex = 0;

            foreach (var ch in boardfenSection)
            {
                var piece = ch switch
                {
                    'P' => Piece.P,
                    'N' => Piece.N,
                    'B' => Piece.B,
                    'R' => Piece.R,
                    'Q' => Piece.Q,
                    'K' => Piece.K,

                    'p' => Piece.p,
                    'n' => Piece.n,
                    'b' => Piece.b,
                    'r' => Piece.r,
                    'q' => Piece.q,
                    'k' => Piece.k,

                    _ => Piece.None
                };

                if (piece != Piece.None)
                {
                    pieceBitBoards[(int)piece] = pieceBitBoards[(int)piece].SetBit(BitBoardExtensions.SquareIndex(rankIndex, fileIndex));
                    ++fileIndex;
                }
                else
                {
                    fileIndex += ch - '0';
#if DEBUG
                    if (fileIndex < 1 || fileIndex > 8)
                    {
                        System.Diagnostics.Debug.Fail($"Error parsing char {ch} in fen {boardfenSection.ToString()}");
                        success = false;
                    }
#endif
                }
            }
        }

        static void PopulateOccupancies(BitBoard[] pieceBitBoards, BitBoard[] occupancyBitBoards)
        {
            for (int piece = (int)Piece.P; piece <= (int)Piece.K; ++piece)
            {
                occupancyBitBoards[(int)Side.White] |= pieceBitBoards[piece];
                occupancyBitBoards[(int)Side.Black] |= pieceBitBoards[piece + 6];
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
                //_logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
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
                //_logger.Error("Invalid board: en passant square {0}, but no {1} pawn located in {2}", enPassantSpan.ToString(), side, pawnSquare);
            }
        }
        else if (enPassantSpan[0] != '-')
        {
            success = false;
            //_logger.Error("Invalid en passant square: {0}", enPassantSpan.ToString());
        }

        return (enPassant, success);
    }
}
