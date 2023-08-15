using System.Numerics;
using System.Runtime.CompilerServices;

#pragma warning disable S4136

namespace Lynx.Model;

public static class BitBoardExtensions
{
    public static bool Empty(this BitBoard board) => board == default;

    public static BitBoard Initialize(params BoardSquare[] occupiedSquares)
    {
        BitBoard board = default;

        foreach (var square in occupiedSquares)
        {
            board.SetBit(square);
        }

        return board;
    }

    internal static void Clear(this ref BitBoard board) => board = default;

    internal static void Print(this BitBoard board)
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

                Console.Write($" {(board.GetBit(squareIndex) ? "1" : "0")}");
            }

            Console.WriteLine();
        }

        Console.Write("\n    a b c d e f g h\n");

        Console.WriteLine($"\n    Bitboard: {board} (0x{board:X})");
        Console.WriteLine(separator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit(this BitBoard board, int squareIndex)
    {
        return (board & (1UL << squareIndex)) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard SetBit(this ref BitBoard board, int square)
    {
        return board |= (1UL << square);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard PopBit(this ref BitBoard board, int square)
    {
        return board &= ~(1UL << square);
    }

    /// <summary>
    /// https://www.chessprogramming.org/Population_Count#Single_Populated_Bitboards
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSinglePopulated(this BitBoard board)
    {
        return board != default && WithoutLS1B(board) == default;
    }

    /// <summary>
    /// https://github.com/SebLague/Chess-Challenge/blob/4ef9025ebf5f3386e416ce8244bbdf3fc488f95b/Chess-Challenge/src/Framework/Chess/Move%20Generation/Bitboards/BitBoardUtility.cs#L32
    /// </summary>
    /// <param name="bitboard"></param>
    /// <param name="squareIndex"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBit(this ref BitBoard bitboard, int squareIndex)
    {
        bitboard ^= 1ul << squareIndex;
    }

    /// <summary>
    /// https://github.com/SebLague/Chess-Challenge/blob/4ef9025ebf5f3386e416ce8244bbdf3fc488f95b/Chess-Challenge/src/Framework/Chess/Move%20Generation/Bitboards/BitBoardUtility.cs#L37
    /// </summary>
    /// <param name="bitboard"></param>
    /// <param name="squareA"></param>
    /// <param name="squareB"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBits(this ref BitBoard bitboard, int squareA, int squareB)
    {
        bitboard ^= (1ul << squareA | 1ul << squareB);
    }

    #region Static methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SquareIndex(int rank, int file)
    {
        return (rank * 8) + file;
    }

    /// <summary>
    /// Assumes that <paramref name="board"/> != default
    /// https://www.chessprogramming.org/General_Setwise_Operations#Separation.
    /// Cannot use (Board & -Board) - 1 due to limitation applying unary - to ulong.
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLS1BIndex(this BitBoard board)
    {
        Utils.Assert(board != default);

        return BitOperations.TrailingZeroCount(board);
    }

    /// <summary>
    /// https://www.chessprogramming.org/General_Setwise_Operations#LS1BReset
    /// </summary>
    /// <param name="board"></param>
    /// <returns>Bitboard</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard WithoutLS1B(this BitBoard board)
    {
        return board & (board - 1);
    }

    /// <summary>
    /// https://www.chessprogramming.org/General_Setwise_Operations#LS1BReset
    /// </summary>
    /// <param name="board"></param>
    /// <returns>Bitboard</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ResetLS1B(this ref BitBoard board)
    {
        board &= (board - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountBits(this BitBoard board)
    {
        return BitOperations.PopCount(board);
    }

    #endregion

    #region Methods accepting BoardSquares

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit(this BitBoard board, BoardSquare square) => board.GetBit((int)square);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard SetBit(this ref BitBoard board, BoardSquare square) => board.SetBit((int)square);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard PopBit(this ref BitBoard board, BoardSquare square) => board.PopBit((int)square);

    #endregion
}

#pragma warning restore S4136
