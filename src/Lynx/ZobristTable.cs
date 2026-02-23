using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    private static readonly ulong[] _table;
    private static readonly ulong[] _50mrTable = GC.AllocateArray<ulong>(Constants.MaxNumberMovesInAGame, pinned: true);

#pragma warning disable IDE1006 // Naming Styles
    private static readonly ulong _WK_Hash;
    private static readonly ulong _WQ_Hash;
    private static readonly ulong _BK_Hash;
    private static readonly ulong _BQ_Hash;
    private static readonly ulong[] _castleHashes = GC.AllocateArray<ulong>(16, pinned: true);
#pragma warning restore IDE1006 // Naming Styles

#pragma warning disable CA1810 // Initialize reference type static fields inline

    static ZobristTable()
    {
        _table = InitializeZobristTable();
        Initialize50mrTable();

        _WK_Hash = PieceHash((int)BoardSquare.a8, (int)Piece.p);
        _WQ_Hash = PieceHash((int)BoardSquare.b8, (int)Piece.p);
        _BK_Hash = PieceHash((int)BoardSquare.c8, (int)Piece.p);
        _BQ_Hash = PieceHash((int)BoardSquare.d8, (int)Piece.p);

        _castleHashes[(int)CastlingRights.None] = 0;

        _castleHashes[(int)CastlingRights.WK] = _WK_Hash;
        _castleHashes[(int)CastlingRights.WQ] = _WQ_Hash;
        _castleHashes[(int)CastlingRights.BK] = _BK_Hash;
        _castleHashes[(int)CastlingRights.BQ] = _BQ_Hash;

        _castleHashes[(int)(CastlingRights.WK | CastlingRights.WQ)] = _WK_Hash ^ _WQ_Hash;
        _castleHashes[(int)(CastlingRights.WK | CastlingRights.BK)] = _WK_Hash ^ _BK_Hash;
        _castleHashes[(int)(CastlingRights.WK | CastlingRights.BQ)] = _WK_Hash ^ _BQ_Hash;
        _castleHashes[(int)(CastlingRights.WQ | CastlingRights.BK)] = _WQ_Hash ^ _BK_Hash;
        _castleHashes[(int)(CastlingRights.WQ | CastlingRights.BQ)] = _WQ_Hash ^ _BQ_Hash;
        _castleHashes[(int)(CastlingRights.BK | CastlingRights.BQ)] = _BK_Hash ^ _BQ_Hash;

        _castleHashes[(int)(CastlingRights.WK | CastlingRights.WQ | CastlingRights.BK)] = _WK_Hash ^ _WQ_Hash ^ _BK_Hash;
        _castleHashes[(int)(CastlingRights.WK | CastlingRights.WQ | CastlingRights.BQ)] = _WK_Hash ^ _WQ_Hash ^ _BQ_Hash;
        _castleHashes[(int)(CastlingRights.WK | CastlingRights.BK | CastlingRights.BQ)] = _WK_Hash ^ _BK_Hash ^ _BQ_Hash;
        _castleHashes[(int)(CastlingRights.WQ | CastlingRights.BK | CastlingRights.BQ)] = _WQ_Hash ^ _BK_Hash ^ _BQ_Hash;

        _castleHashes[(int)(CastlingRights.WK | CastlingRights.WQ | CastlingRights.BK | CastlingRights.BQ)] = _WK_Hash ^ _WQ_Hash ^ _BK_Hash ^ _BQ_Hash;
    }

#pragma warning restore CA1810 // Initialize reference type static fields inline

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong HalfMovesWithoutCaptureOrPawnMoveHash(int counter)
        => _50mrTable[counter];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PieceHash(int boardSquare, int piece)
        => Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_table), (boardSquare * 12) + piece);

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

        return PieceHash(file, (int)Piece.P);
    }

    /// <summary>
    /// Uses <see cref="Piece.p"/> and <see cref="BoardSquare.h8"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SideHash()
    {
        return PieceHash((int)BoardSquare.h8, (int)Piece.p);
    }

    /// <summary>
    /// Uses <see cref="Piece.p"/> and <see cref="BoardSquare.h8"/>.
    /// Differentiates white and black sides
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong SideHash(ulong side)
    {
        return side * PieceHash((int)BoardSquare.h8, (int)Piece.p);
    }

    /// <summary>
    /// Uses <see cref="Piece.p"/> and
    /// <see cref="BoardSquare.a8"/> for <see cref="CastlingRights.WK"/>, <see cref="BoardSquare.b8"/> for <see cref="CastlingRights.WQ"/>
    /// <see cref="BoardSquare.c8"/> for <see cref="CastlingRights.BK"/>, <see cref="BoardSquare.d8"/> for <see cref="CastlingRights.BQ"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong CastleHash(byte castle) => _castleHashes[castle];

    /// <summary>
    /// Calculates from scratch the hash of a position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PositionHash(Position position)
    {
        ulong positionHash = 0;

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitboards[pieceIndex];

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

        var whitePawns = position.PieceBitboards[(int)Piece.P];
        while (whitePawns != default)
        {
            whitePawns = whitePawns.WithoutLS1B(out var pieceSquareIndex);

            pawnKingHash ^= PieceHash(pieceSquareIndex, (int)Piece.P);
        }

        var blackPawns = position.PieceBitboards[(int)Piece.p];
        while (blackPawns != default)
        {
            blackPawns = blackPawns.WithoutLS1B(out var pieceSquareIndex);

            pawnKingHash ^= PieceHash(pieceSquareIndex, (int)Piece.p);
        }

        var whiteKing = position.PieceBitboards[(int)Piece.K].GetLS1BIndex();
        pawnKingHash ^= PieceHash(whiteKing, (int)Piece.K);

        var blackKing = position.PieceBitboards[(int)Piece.k].GetLS1BIndex();
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
            var bitboard = position.PieceBitboards[pieceIndex];

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
            var whiteBitboard = position.PieceBitboards[pieceIndex];
            while (whiteBitboard != default)
            {
                whiteBitboard = whiteBitboard.WithoutLS1B(out var pieceSquareIndex);

                minorHash ^= PieceHash(pieceSquareIndex, pieceIndex);
            }

            var blackBitboard = position.PieceBitboards[pieceIndex + 6];
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
            var whiteBitboard = position.PieceBitboards[pieceIndex];
            while (whiteBitboard != default)
            {
                whiteBitboard = whiteBitboard.WithoutLS1B(out var pieceSquareIndex);
                majorHash ^= PieceHash(pieceSquareIndex, pieceIndex);
            }

            var blackBitboard = position.PieceBitboards[pieceIndex + 6];
            while (blackBitboard != default)
            {
                blackBitboard = blackBitboard.WithoutLS1B(out var pieceSquareIndex);
                majorHash ^= PieceHash(pieceSquareIndex, pieceIndex + 6);
            }
        }

        return majorHash;
    }

    private static ulong[] InitializeZobristTable() => InitializeZobristTable(_random);

    /// <summary>
    /// Initializes Zobrist table (long[64 x 12])
    /// </summary>
    internal static ulong[] InitializeZobristTable(LynxRandom random)
    {
        var zobristTable = GC.AllocateArray<ulong>(64 * 12, pinned: true);

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                zobristTable[(squareIndex * 12) + pieceIndex] = random.NextUInt64();
            }
        }

        return zobristTable;
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
