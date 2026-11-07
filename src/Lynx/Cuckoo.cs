using Lynx.Model;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

/// <summary>
/// Concept based on https://web.archive.org/web/20201107002606/https://marcelk.net/2013-04-06/paper/upcoming-rep-v2.pdf
/// Implementation inspired by Stockfish, Stormphrax and Viridithas
/// </summary>
public static class Cuckoo
{
    private const int TableSize = 8192;
    private const int Mask = TableSize - 1;

    /// <summary>
    /// Number of reversible moves on an empty board. Calculated as the sum of the attacked squares, for each piece and for each square
    /// It's calculated for both colors and then divided by 2 (since from -> to and to -> from moves share hash)
    /// </summary>
    internal const int ExpectedMoveCount = 3668;

    private static readonly ulong[] _keys = GC.AllocateArray<ulong>(TableSize, pinned: true);

    /// <summary>
    /// Reversible moves on an empty board, encoded using <see cref="EncodeMove(int, int)"/>
    /// 0 is used for empty slots
    /// </summary>
    private static readonly ushort[] _moves = GC.AllocateArray<ushort>(TableSize, pinned: true);

    /// <summary>
    /// Non empty <see cref="_moves"/>
    /// </summary>
    internal static int Count { get; }

#pragma warning disable CA1810 // Initialize reference type static fields inline

    static Cuckoo()
    {
        int count = 0;

        for (int offset = (int)Piece.P; offset <= (int)Piece.p; offset += (int)Piece.p)
        {
            for (int pieceType = (int)Piece.N; pieceType <= (int)Piece.K; ++pieceType)
            {
                var piece = pieceType + offset;

                for (int from = 0; from < 64; ++from)
                {
                    var attacks = MoveGenerator.PieceAttacks(piece, from, occupancy: 0);

                    // The key of a move is PieceHash(from, piece) ^ PieceHash(to, piece) ^ SideHash.
                    // Since ^ is commutative, moving from -> to squares and to -> from have the same key, so we only store the squares where to > from.
                    for (int to = from + 1; to < 64; ++to)
                    {
                        if (!attacks.GetBit(to))
                        {
                            continue;
                        }

                        Insert(
                            ZobristTable.PieceHash(from, piece) ^ ZobristTable.PieceHash(to, piece) ^ ZobristTable.SideHash(),
                            EncodeMove(from, to));

                        ++count;
                    }
                }
            }
        }

        Count = count;

        Debug.Assert(Count == ExpectedMoveCount, $"Unexpected number of reversible moves: {count}");
    }

    #pragma warning restore CA1810 // Initialize reference type static fields inline

    private static ushort EncodeMove(int from, int to) => (ushort)(from | (to << 6));

    /// <summary>
    /// Returns true if <paramref name="key"/> is the hash of a reversible piece move, as well as the from-to squares of that move
    /// This doesn't guarantee the ray between the squares is clear
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetMove(ulong key, out int from, out int to)
    {
        ref var keys = ref MemoryMarshal.GetArrayDataReference(_keys);

        var slot = H1(key);
        if (Unsafe.Add(ref keys, slot) != key)
        {
            slot = H2(key);
            if (Unsafe.Add(ref keys, slot) != key)
            {
                from = to = 0;
                return false;
            }
        }

        int move = Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_moves), slot);

        from = move & 0x3F;
        to = move >> 6;

        return move != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int H1(ulong key) => (int)((key >> 32) & Mask);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int H2(ulong key) => (int)((key >> 48) & Mask);

    private static void Insert(ulong key, ushort move)
    {
        var slot = H1(key);

        for (int displacements = 0; displacements <= TableSize; ++displacements)
        {
            (_keys[slot], key) = (key, _keys[slot]);
            (_moves[slot], move) = (move, _moves[slot]);

            if (move == 0)
            {
                return;
            }

            slot = slot == H1(key)
                ? H2(key)
                : H1(key);
        }

        throw new LynxException("Unable to build the Cuckoo table: new Zobrist keys seed or H1 H2 functions might be needed");
    }
}
