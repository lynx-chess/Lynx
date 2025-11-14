using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lynx;

/// <summary>
/// White pawns in file 1 are used to encode en-passant pawns
/// Black pawns in file 8 are used to encode castling rights and side
/// </summary>
public static class ZobristTable
{
    private static readonly LynxRandom _random = new(int.MaxValue);

    /// <summary>
    /// 64x12
    /// </summary>
    private static readonly ulong[] _table = GC.AllocateArray<ulong>(64 * 12, pinned: true);
    private static readonly ulong[] _50mrTable = GC.AllocateArray<ulong>(Constants.MaxNumberMovesInAGame, pinned: true);

#pragma warning disable IDE1006 // Naming Styles
    private static readonly ulong _WK_Hash;
    private static readonly ulong _WQ_Hash;
    private static readonly ulong _BK_Hash;
    private static readonly ulong _BQ_Hash;
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable CA1810 // Initialize reference type static fields inline

    static ZobristTable()
    {
        InitializeZobristTable();
        Initialize50mrTable();

        _WK_Hash = _table[PieceTableIndex((int)BoardSquare.a8, (int)Piece.p)];
        _WQ_Hash = _table[PieceTableIndex((int)BoardSquare.b8, (int)Piece.p)];
        _BK_Hash = _table[PieceTableIndex((int)BoardSquare.c8, (int)Piece.p)];
        _BQ_Hash = _table[PieceTableIndex((int)BoardSquare.d8, (int)Piece.p)];
    }

#pragma warning restore CA1810 // Initialize reference type static fields inline

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong HalfMovesWithoutCaptureOrPawnMoveHash(int counter)
        => _50mrTable[counter];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PieceHash(int boardSquare, int piece)
        => _table[PieceTableIndex(boardSquare, piece)];

    /// <summary>
    /// Uses <see cref="Piece.P"/> and squares <see cref="BoardSquare.a1"/>-<see cref="BoardSquare.h1"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong EnPassantHash(int enPassantSquare)
    {
        if (enPassantSquare == (int)BoardSquare.noSquare)
        {
            return default;
        }

        Debug.Assert(Constants.EnPassantCaptureSquares.Length > enPassantSquare && Constants.EnPassantCaptureSquares[enPassantSquare] != 0,
            $"{Constants.Coordinates[enPassantSquare]} is not a valid en-passant square");

        var file = enPassantSquare & 0x07;  // enPassantSquare % 8

        return _table[PieceTableIndex(file, (int)Piece.P)];
    }

    /// <summary>
    /// Uses <see cref="Piece.p"/> and <see cref="BoardSquare.h8"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SideHash()
    {
        return _table[PieceTableIndex((int)BoardSquare.h8, (int)Piece.p)];
    }

    /// <summary>
    /// Uses <see cref="Piece.p"/> and <see cref="BoardSquare.h8"/>.
    /// Differenciates white and black sides
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong SideHash(ulong side)
    {
        return side * _table[PieceTableIndex((int)BoardSquare.h8, (int)Piece.p)];
    }

    /// <summary>
    /// Uses <see cref="Piece.p"/> and
    /// <see cref="BoardSquare.a8"/> for <see cref="CastlingRights.WK"/>, <see cref="BoardSquare.b8"/> for <see cref="CastlingRights.WQ"/>
    /// <see cref="BoardSquare.c8"/> for <see cref="CastlingRights.BK"/>, <see cref="BoardSquare.d8"/> for <see cref="CastlingRights.BQ"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong CastleHash(byte castle)
    {
        return castle switch
        {
            0 => 0,                                // -    | -

            (byte)CastlingRights.WK => _WK_Hash,    // K    | -
            (byte)CastlingRights.WQ => _WQ_Hash,    // Q    | -
            (byte)CastlingRights.BK => _BK_Hash,    // -    | k
            (byte)CastlingRights.BQ => _BQ_Hash,    // -    | q

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ => _WK_Hash ^ _WQ_Hash,    // KQ   | -
            (byte)CastlingRights.WK | (byte)CastlingRights.BK => _WK_Hash ^ _BK_Hash,    // K    | k
            (byte)CastlingRights.WK | (byte)CastlingRights.BQ => _WK_Hash ^ _BQ_Hash,    // K    | q
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK => _WQ_Hash ^ _BK_Hash,    // Q    | k
            (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => _WQ_Hash ^ _BQ_Hash,    // Q    | q
            (byte)CastlingRights.BK | (byte)CastlingRights.BQ => _BK_Hash ^ _BQ_Hash,    // -    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK => _WK_Hash ^ _WQ_Hash ^ _BK_Hash,    // KQ   | k
            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ => _WK_Hash ^ _WQ_Hash ^ _BQ_Hash,    // KQ   | q
            (byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => _WK_Hash ^ _BK_Hash ^ _BQ_Hash,    // K    | kq
            (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ => _WQ_Hash ^ _BK_Hash ^ _BQ_Hash,    // Q    | kq

            (byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ =>       // KQ   | kq
                _WK_Hash ^ _WQ_Hash ^ _BK_Hash ^ _BQ_Hash,

            _ => throw new LynxException($"Unexpected castle encoded number: {castle}")
        };
    }

    /// <summary>
    /// Calculates from scratch the hash of a position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PositionHash(Position position)
    {
        ulong positionHash = 0;

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                positionHash ^= PieceHash(pieceSquareIndex, pieceIndex);
            }
        }

        positionHash ^= EnPassantHash((int)position.EnPassant)
            ^ SideHash((ulong)position.Side)
            ^ CastleHash(position.Castle);

        return positionHash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PositionHash(Position position, ulong kingPawnHash, ulong nonPawnWhiteHash, ulong nonPawnBlackHash)
    {
        return kingPawnHash
            ^ nonPawnWhiteHash
            ^ nonPawnBlackHash
            ^ PieceHash(position.WhiteKingSquare, (int)Piece.K)     // Removing king hashes, since they're included in both kingPawn and nonPawn ones
            ^ PieceHash(position.BlackKingSquare, (int)Piece.k)
            ^ EnPassantHash((int)position.EnPassant)
            ^ SideHash((ulong)position.Side)
            ^ CastleHash(position.Castle);
    }

    /// <summary>
    /// Calculates from scratch the pawn structure hash of a position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong KingPawnHash(Position position)
    {
        ulong pawnKingHash = 0;

        var whitePawns = position.PieceBitBoards[(int)Piece.P];
        while (whitePawns != default)
        {
            whitePawns = whitePawns.WithoutLS1B(out var pieceSquareIndex);

            pawnKingHash ^= PieceHash(pieceSquareIndex, (int)Piece.P);
        }

        var blackPawns = position.PieceBitBoards[(int)Piece.p];
        while (blackPawns != default)
        {
            blackPawns = blackPawns.WithoutLS1B(out var pieceSquareIndex);

            pawnKingHash ^= PieceHash(pieceSquareIndex, (int)Piece.p);
        }

        var whiteKing = position.PieceBitBoards[(int)Piece.K].GetLS1BIndex();
        pawnKingHash ^= PieceHash(whiteKing, (int)Piece.K);

        var blackKing = position.PieceBitBoards[(int)Piece.k].GetLS1BIndex();
        pawnKingHash ^= PieceHash(blackKing, (int)Piece.k);

        return pawnKingHash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong NonPawnSideHash(Position position, int side)
    {
        ulong nonPawnSideHash = 0;

        var start = 7 - (6 * side);
        var end = 12 - (6 * side);

        for (int pieceIndex = start; pieceIndex < end; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                bitboard = bitboard.WithoutLS1B(out var pieceSquareIndex);

                nonPawnSideHash ^= PieceHash(pieceSquareIndex, pieceIndex);
            }
        }

        return nonPawnSideHash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong MinorHash(Position position)
    {
        ulong minorHash = 0;

        for (int pieceIndex = (int)Piece.N; pieceIndex <= (int)Piece.B; ++pieceIndex)
        {
            var whiteBitboard = position.PieceBitBoards[pieceIndex];
            while (whiteBitboard != default)
            {
                whiteBitboard = whiteBitboard.WithoutLS1B(out var pieceSquareIndex);

                minorHash ^= PieceHash(pieceSquareIndex, pieceIndex);
            }

            var blackBitboard = position.PieceBitBoards[pieceIndex + 6];
            while (blackBitboard != default)
            {
                blackBitboard = blackBitboard.WithoutLS1B(out var pieceSquareIndex);

                minorHash ^= PieceHash(pieceSquareIndex, pieceIndex + 6);
            }
        }

        return minorHash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong MajorHash(Position position)
    {
        ulong majorHash = 0;

        for (int pieceIndex = (int)Piece.R; pieceIndex <= (int)Piece.Q; ++pieceIndex)
        {
            var whiteBitboard = position.PieceBitBoards[pieceIndex];
            while (whiteBitboard != default)
            {
                whiteBitboard = whiteBitboard.WithoutLS1B(out var pieceSquareIndex);
                majorHash ^= PieceHash(pieceSquareIndex, pieceIndex);
            }

            var blackBitboard = position.PieceBitBoards[pieceIndex + 6];
            while (blackBitboard != default)
            {
                blackBitboard = blackBitboard.WithoutLS1B(out var pieceSquareIndex);
                majorHash ^= PieceHash(pieceSquareIndex, pieceIndex + 6);
            }
        }

        return majorHash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PieceTableIndex(int boardSquare, int piece)
        => (boardSquare * 12) + piece;

    private static void InitializeZobristTable() => InitializeZobristTable(_random, _table);

    /// <summary>
    /// Initializes Zobrist table
    /// </summary>
    /// <param name="zobristTable">Array of min size 64 x 12</param>
    internal static void InitializeZobristTable(LynxRandom random, ulong[] zobristTable)
    {
        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                zobristTable[PieceTableIndex(squareIndex, pieceIndex)] = random.NextUInt64();
            }
        }
    }

    private static void Initialize50mrTable()
    {
        var initialMoves = Configuration.EngineSettings.TT_50MR_Start;
        var step = Configuration.EngineSettings.TT_50MR_Step;

        var initialVal = _random.NextUInt64();
        for (int i = 0; i < initialMoves; ++i)
        {
            _50mrTable[i] = initialVal;
        }

        for (int i = initialMoves; i < _50mrTable.Length; i += step)
        {
            var val = _random.NextUInt64();

            for (int j = i; j < i + step && j < _50mrTable.Length; ++j)
            {
                _50mrTable[j] = val;
            }
        }
    }
}
