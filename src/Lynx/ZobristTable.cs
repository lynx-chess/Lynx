using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx;

/// <summary>
/// White pawns in file 1 are used to encode en-passant pawns
/// Black pawns in file 8 are used to encode castling rights and side
/// </summary>
public static class ZobristTable
{
    private static readonly long[,] _table = Initialize();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long PieceHash(int boardSquare, int piece) => _table[boardSquare, piece];

    /// <summary>
    /// Uses <see cref="Piece.P"/> and squares <see cref="BoardSquare.a1"/>-<see cref="BoardSquare.h1"/>
    /// </summary>
    /// <param name="enPassantSquare"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long EnPassantHash(int enPassantSquare)
    {
        if (enPassantSquare == (int)BoardSquare.noSquare)
        {
            return default;
        }

#if DEBUG
        if (!Constants.EnPassantCaptureSquares.ContainsKey(enPassantSquare))
        {
            throw new ArgumentException($"{Constants.Coordinates[enPassantSquare]} is not a valid en-passant square");
        }
#endif

        var file = enPassantSquare & 0x03;  // enPassantSquare % 8

        return _table[file, (int)Piece.P];
    }

    /// <summary>
    /// Uses <see cref="Piece.p"/> and <see cref="BoardSquare.h8"/>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SideHash()
    {
        return _table[(int)BoardSquare.h8, (int)Piece.p];
    }

    internal static readonly long WK_Hash = _table[(int)BoardSquare.a8, (int)Piece.p];
    internal static readonly long WQ_Hash = _table[(int)BoardSquare.b8, (int)Piece.p];
    internal static readonly long BK_Hash = _table[(int)BoardSquare.c8, (int)Piece.p];
    internal static readonly long BQ_Hash = _table[(int)BoardSquare.d8, (int)Piece.p];

    private static readonly Dictionary<byte, long> _castleHashDictionary = new()
    {
        [0] = 0,                                // -    | -
        [(byte)CastlingRights.WK] = WK_Hash,    // K    | -
        [(byte)CastlingRights.WQ] = WQ_Hash,    // Q    | -
        [(byte)CastlingRights.BK] = BK_Hash,    // -    | k
        [(byte)CastlingRights.BQ] = BQ_Hash,    // -    | q

        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ] = WK_Hash ^ WQ_Hash,    // KQ   | -
        [(byte)CastlingRights.WK | (byte)CastlingRights.BK] = WK_Hash ^ BK_Hash,    // K    | k
        [(byte)CastlingRights.WK | (byte)CastlingRights.BQ] = WK_Hash ^ BQ_Hash,    // K    | q
        [(byte)CastlingRights.WQ | (byte)CastlingRights.BK] = WQ_Hash ^ BK_Hash,    // Q    | k
        [(byte)CastlingRights.WQ | (byte)CastlingRights.BQ] = WQ_Hash ^ BQ_Hash,    // Q    | q
        [(byte)CastlingRights.BK | (byte)CastlingRights.BQ] = BK_Hash ^ BQ_Hash,    // -    | kq

        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK] = WK_Hash ^ WQ_Hash ^ BK_Hash,    // KQ   | k
        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BQ] = WK_Hash ^ WQ_Hash ^ BQ_Hash,    // KQ   | q
        [(byte)CastlingRights.WK | (byte)CastlingRights.BK | (byte)CastlingRights.BQ] = WK_Hash ^ BK_Hash ^ BQ_Hash,    // K    | kq
        [(byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ] = WQ_Hash ^ BK_Hash ^ BQ_Hash,    // Q    | kq

        [(byte)CastlingRights.WK | (byte)CastlingRights.WQ | (byte)CastlingRights.BK | (byte)CastlingRights.BQ] =       // KQ   | kq
            WK_Hash ^ WQ_Hash ^ BK_Hash ^ BQ_Hash
    };

    /// <summary>
    /// Uses <see cref="Piece.p"/> and
    /// <see cref="BoardSquare.a8"/> for <see cref="CastlingRights.WK"/>, <see cref="BoardSquare.b8"/> for <see cref="CastlingRights.WQ"/>
    /// <see cref="BoardSquare.c8"/> for <see cref="CastlingRights.BK"/>, <see cref="BoardSquare.d8"/> for <see cref="CastlingRights.BQ"/>
    /// </summary>
    /// <param name="castle"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long CastleHash(byte castle)
    {
        System.Diagnostics.Debug.Assert(_castleHashDictionary[castle] == CalculateCastleHash(castle));

        return _castleHashDictionary[castle];
    }

    internal static long CalculateCastleHash(byte castle)
    {
        long combinedHash = 0;

        if ((castle & (int)CastlingRights.WK) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.a8, (int)Piece.p];        // a8
        }

        if ((castle & (int)CastlingRights.WQ) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.b8, (int)Piece.p];        // b8
        }

        if ((castle & (int)CastlingRights.BK) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.c8, (int)Piece.p];        // c8
        }

        if ((castle & (int)CastlingRights.BQ) != default)
        {
            combinedHash ^= _table[(int)BoardSquare.d8, (int)Piece.p];        // d8
        }

        return combinedHash;
    }

    /// <summary>
    /// Calculates from scratch the hash of a position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long PositionHash(Position position)
    {
        long positionHash = 0;

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                if (position.PieceBitBoards[pieceIndex].GetBit(squareIndex))
                {
                    positionHash ^= PieceHash(squareIndex, pieceIndex);
                }
            }
        }

        positionHash ^= EnPassantHash((int)position.EnPassant)
            ^ SideHash()
            ^ CastleHash(position.Castle);

        return positionHash;
    }

    /// <summary>
    /// Initializes Zobrist table (long[64, 12])
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long[,] Initialize()
    {
        var zobristTable = new long[64, 12];
        var randomInstance = new Random(int.MaxValue);

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                zobristTable[squareIndex, pieceIndex] = randomInstance.NextInt64();
            }
        }

        return zobristTable;
    }
}
