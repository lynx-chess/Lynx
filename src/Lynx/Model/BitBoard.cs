using System.Numerics;
using System.Runtime.CompilerServices;

#pragma warning disable S4136

namespace Lynx.Model;

public struct BitBoard
{
    public ulong Board { readonly get; private set; }

    public bool Empty => Board == default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard(ulong value) { Board = value; }

    internal BitBoard(params BoardSquare[] occupiedSquares)
    {
        Board = default;

        foreach (var square in occupiedSquares)
        {
            SetBit(square);
        }
    }

    internal void Clear() => Board = default;

    internal readonly void Print()
    {
        const string separator = "____________________________________________________";
        Console.WriteLine(separator);

        for (var rank = 0; rank < 8; ++rank)
        {
            for (var file = 0; file < 8; ++file)
            {
                if (file == 0)
                {
                    Console.Write($"{8 - rank}  ");
                }

                var squareIndex = SquareIndex(rank, file);

                Console.Write($" {(GetBit(squareIndex) ? "1" : "0")}");
            }

            Console.WriteLine();
        }

        Console.Write("\n    a b c d e f g h\n");

        Console.WriteLine($"\n    Bitboard: {Board} (0x{Board:X})");
        Console.WriteLine(separator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool GetBit(int squareIndex)
    {
        return (Board & (1UL << squareIndex)) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int square)
    {
        Board |= (1UL << square);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopBit(int square)
    {
        Board &= ~(1UL << square);
    }

    /// <summary>
    /// https://www.chessprogramming.org/General_Setwise_Operations#Separation.
    /// Cannot use (Board & -Board) - 1 due to limitation applying unary - to ulong.
    /// Assumes <see cref="Board"/> != default
    /// </summary>
    /// <returns>-1 in case of empty board</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetLS1BIndex() => GetLS1BIndex(Board);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetLS1B()
    {
        Board = ResetLS1B(Board);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int CountBits() => CountBits(Board);

    /// <summary>
    /// https://www.chessprogramming.org/Population_Count#Single_Populated_Bitboards
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsSinglePopulated()
    {
        return Board != default && ResetLS1B(Board) == default;
    }

    #region Static methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SquareIndex(int rank, int file)
    {
        return (rank * 8) + file;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SetBit(ulong bitboard, int squareIndex)
    {
        return bitboard | (1UL << squareIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit(ulong bitboard, int squareIndex)
    {
        return (bitboard & (1UL << squareIndex)) != default;
    }

    /// <summary>
    /// Assumes that <paramref name="bitboard"/> != default
    /// </summary>
    /// <param name="bitboard"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLS1BIndex(ulong bitboard)
    {
        Utils.Assert(bitboard != default);

        return BitOperations.TrailingZeroCount(bitboard);
    }

    /// <summary>
    /// https://www.chessprogramming.org/General_Setwise_Operations#LS1BReset
    /// </summary>
    /// <param name="bitboard"></param>
    /// <returns>Bitboard</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ResetLS1B(ulong bitboard)
    {
        return bitboard & (bitboard - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountBits(ulong bitboard)
    {
        return BitOperations.PopCount(bitboard);
    }

    #endregion

    #region Methods accepting BoardSquares

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool GetBit(BoardSquare square) => GetBit((int)square);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(BoardSquare square) => SetBit((int)square);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopBit(BoardSquare square) => PopBit((int)square);

    #endregion
}

#pragma warning restore S4136
