using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;
internal class GetAndResetLS1BIndex_Benchmark : BaseBenchmark
{
    public static IEnumerable<Position> Data =>
    [
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public int GetLS1BIndex_ResetLS1B(Position position)
    {
        int result = 0;

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            var square = bitboard.GetLS1BIndex();
            bitboard.ResetLS1B();

            result += square;
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int GetLS1BIndexAndPopIt(Position position)
    {
        int result = 0;

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            var square = bitboard.GetLS1BIndexAndPopIt();

            result += square;
        }

        return result;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public int GetLS1BIndexAndToggleIt(Position position)
    {
        int result = 0;

        for (int pieceIndex = (int)Piece.P; pieceIndex <= (int)Piece.k; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            var square = bitboard.GetLS1BIndexAndToggleIt();

            result += square;
        }

        return result;
    }
}

internal static class BitBoardExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLS1BIndex(this BitBoard board)
    {
        Utils.Assert(board != default);

        return BitOperations.TrailingZeroCount(board);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ResetLS1B(this ref BitBoard board)
    {
        board &= (board - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLS1BIndexAndPopIt(this ref BitBoard board)
    {
        var index = GetLS1BIndex(board);
        board.PopBit(index);

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLS1BIndexAndToggleIt(this ref BitBoard board)
    {
        var index = GetLS1BIndex(board);
        board.ToggleBit(index);

        return index;
    }
}
