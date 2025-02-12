using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

#pragma warning disable S4136

namespace Lynx.Model;

public struct BitBoard
{
    private ulong _value;

    public BitBoard(ulong value)
    {
        _value = value;
    }

    public static implicit operator ulong(BitBoard b) => b._value;
    //public static explicit operator BitBoard(ulong b) => new BitBoard(b);

    public static BitBoard operator |(BitBoard a, BitBoard b) => new(a._value | b._value);
    public static BitBoard operator &(BitBoard a, BitBoard b) => new(a._value & b._value);
    public static BitBoard operator ^(BitBoard a, BitBoard b) => new(a._value ^ b._value);
    public static BitBoard operator >>(BitBoard a, int b) => new(a._value >> b);
    public static BitBoard operator <<(BitBoard a, int b) => new(a._value << b);
    public static BitBoard operator ~(BitBoard a) => new(~a._value);

    public BitBoard(params BoardSquare[] occupiedSquares)
    {
        foreach (var square in occupiedSquares)
        {
            SetBit(square);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Empty() => _value == default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool NotEmpty() => _value != default;

    public void Initialize(params BoardSquare[] occupiedSquares)
    {
#pragma warning disable S3353 // Unchanged local variables should be "const" - FP https://community.sonarsource.com/t/fp-s3353-value-modified-in-ref-extension-method/132389
        _value = 0;
#pragma warning restore S3353 // Unchanged local variables should be "const"

        foreach (var square in occupiedSquares)
        {
            SetBit(square);
        }
    }

    internal void Clear() => _value = default;

#pragma warning disable S106, S2228 // Standard outputs should not be used directly to log anything
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

        Console.WriteLine($"\n    Bitboard: {_value} (0x{_value:X})");
        Console.WriteLine(separator);
    }
#pragma warning restore S106,S2228 // Standard outputs should not be used directly to log anything

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool GetBit(int squareIndex)
    {
        return (_value & (1UL << squareIndex)) != default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(int square)
    {
        _value |= (1UL << square);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopBit(int square)
    {
        _value &= ~(1UL << square);
    }

    /// <summary>
    /// https://www.chessprogramming.org/Population_Count#Single_Populated_Bitboards
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsSinglePopulated()
    {
        return _value != default && WithoutLS1B() == default;
    }

    /// <summary>
    /// https://github.com/SebLague/Chess-Challenge/blob/4ef9025ebf5f3386e416ce8244bbdf3fc488f95b/Chess-Challenge/src/Framework/Chess/Move%20Generation/Bitboards/BitBoardUtility.cs#L32
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToggleBit(int squareIndex)
    {
        _value ^= 1ul << squareIndex;
    }

    /// <summary>
    /// https://github.com/SebLague/Chess-Challenge/blob/4ef9025ebf5f3386e416ce8244bbdf3fc488f95b/Chess-Challenge/src/Framework/Chess/Move%20Generation/Bitboards/BitBoardUtility.cs#L37
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToggleBits(int squareA, int squareB)
    {
        _value ^= (1ul << squareA | 1ul << squareB);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BitBoard LSB()
    {
        if (System.Runtime.Intrinsics.X86.Bmi1.IsSupported)
        {
            return new(System.Runtime.Intrinsics.X86.Bmi1.X64.ExtractLowestSetBit(_value));
        }

        return new(_value & (~_value + 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BitBoard ShiftUp()
    {
        return new(_value >> 8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BitBoard ShiftDown()
    {
        return new(_value << 8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BitBoard ShiftLeft()
    {
        return new((_value >> 1) & Constants.NotHFile);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BitBoard ShiftRight()
    {
        return new((_value << 1) & Constants.NotAFile);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BitBoard ShiftUpRight()
    {
        return ShiftUp().ShiftRight();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BitBoard ShiftUpLeft()
    {
        return ShiftUp().ShiftLeft();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BitBoard ShiftDownRight()
    {
        return ShiftDown().ShiftRight();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BitBoard ShiftDownLeft()
    {
        return ShiftDown().ShiftLeft();
    }

    /// <summary>
    /// Assumes that <paramref name="board"/> != default
    /// https://www.chessprogramming.org/General_Setwise_Operations#Separation.
    /// Cannot use (Board & -Board) - 1 due to limitation applying unary - to ulong.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetLS1BIndex()
    {
        Utils.Assert(_value != default);

        return BitOperations.TrailingZeroCount(_value);
    }

    /// <summary>
    /// https://www.chessprogramming.org/General_Setwise_Operations#LS1BReset
    /// </summary>
    /// <returns>Bitboard</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ulong WithoutLS1B()
    {
        return _value & (_value - 1);
    }

    /// <summary>
    /// https://www.chessprogramming.org/General_Setwise_Operations#LS1BReset
    /// </summary>
    /// <returns>Bitboard</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetLS1B()
    {
        _value &= (_value - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int CountBits()
    {
        return BitOperations.PopCount(_value);
    }

    public readonly bool Contains(int boardSquare)
    {
        var bit = SquareBit(boardSquare);

        return (_value & bit) != default;
    }

    public readonly bool DoesNotContain(int boardSquare)
    {
        var bit = SquareBit(boardSquare);

        return (_value & bit) == default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool GetBit(BoardSquare square) => GetBit((int)square);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBit(BoardSquare square) => SetBit((int)square);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PopBit(BoardSquare square) => PopBit((int)square);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SquareIndex(int rank, int file)
    {
        return (rank * 8) + file;
    }

    /// <summary>
    /// Extracts the bit that represents each square on a bitboard
    /// </summary>
    public static BitBoard SquareBit(int boardSquare)
    {
        return new(1UL << boardSquare);
    }
}

#pragma warning restore S4136
