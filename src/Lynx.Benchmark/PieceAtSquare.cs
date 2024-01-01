/*
 * Locgical results
 *
* BenchmarkDotNet v0.13.11, Windows 10 (10.0.19045.3803/22H2/2022Update)
* Intel Core i7-5500U CPU 2.40GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
* .NET SDK 8.0.100
*   [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
*   DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
*
*
* | Method          | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
* |---------------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
* | KnowingColor    | 5.033 us | 0.1004 us | 0.2611 us |  1.00 |    0.00 | 0.2823 |     601 B |        1.00 |
* | KnowingColor_2  | 4.837 us | 0.0966 us | 0.2613 us |  0.96 |    0.08 | 0.2823 |     601 B |        1.00 |
* | NotKnowingColor | 4.814 us | 0.0952 us | 0.2685 us |  0.96 |    0.08 | 0.2823 |     601 B |        1.00 |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class PieceAtSquare : BaseBenchmark
{
    [Benchmark(Baseline = true)]
    public int KnowingColor()
    {
        int result = 0;

        var position = new Position(Constants.TrickyTestPositionFEN);

        for (int i = 0; i < position.PieceBitBoards.Length; i++)
        {
            ulong bb = position.PieceBitBoards[i];

            while (bb == default)
            {
                var square = bb.GetLS1BIndex();
                bb.ResetLS1B();

                result = result + PieceAt_KnowingColor(position, square);
            }
        }

        return result;
    }

    [Benchmark]
    public int KnowingColor_Unrolled()
    {
        int result = 0;

        var position = new Position(Constants.TrickyTestPositionFEN);

        for (int i = 0; i < position.PieceBitBoards.Length; i += 4)
        {
            ulong bb1 = position.PieceBitBoards[i];
            ulong bb2 = position.PieceBitBoards[i + 1];
            ulong bb3 = position.PieceBitBoards[i + 2];
            ulong bb4 = position.PieceBitBoards[i + 3];

            while (bb1 == default)
            {
                var square = bb1.GetLS1BIndex();
                bb1.ResetLS1B();

                result = result + PieceAt_KnowingColor(position, square);
            }

            while (bb2 == default)
            {
                var square = bb2.GetLS1BIndex();
                bb2.ResetLS1B();

                result = result + PieceAt_KnowingColor(position, square);
            }

            while (bb3 == default)
            {
                var square = bb3.GetLS1BIndex();
                bb3.ResetLS1B();

                result = result + PieceAt_KnowingColor(position, square);
            }

            while (bb4 == default)
            {
                var square = bb4.GetLS1BIndex();
                bb4.ResetLS1B();

                result = result + PieceAt_KnowingColor(position, square);
            }
        }

        return result;
    }

    [Benchmark]
    public int NotKnowingColor()
    {
        int result = 0;

        var position = new Position(Constants.TrickyTestPositionFEN);

        for (int i = 0; i < position.PieceBitBoards.Length; i++)
        {
            ulong bb = position.PieceBitBoards[i];

            while (bb == default)
            {
                var square = bb.GetLS1BIndex();
                bb.ResetLS1B();

                result = result + PieceAt(position, square);
            }
        }

        return result;
    }

    [Benchmark]
    public int NotKnowingColor_Unrolled()
    {
        int result = 0;

        var position = new Position(Constants.TrickyTestPositionFEN);

        for (int i = 0; i < position.PieceBitBoards.Length; i += 4)
        {
            ulong bb1 = position.PieceBitBoards[i];
            ulong bb2 = position.PieceBitBoards[i + 1];
            ulong bb3 = position.PieceBitBoards[i + 2];
            ulong bb4 = position.PieceBitBoards[i + 3];

            while (bb1 == default)
            {
                var square = bb1.GetLS1BIndex();
                bb1.ResetLS1B();

                result = result + PieceAt(position, square);
            }

            while (bb2 == default)
            {
                var square = bb2.GetLS1BIndex();
                bb2.ResetLS1B();

                result = result + PieceAt(position, square);
            }

            while (bb3 == default)
            {
                var square = bb3.GetLS1BIndex();
                bb3.ResetLS1B();

                result = result + PieceAt(position, square);
            }

            while (bb4 == default)
            {
                var square = bb4.GetLS1BIndex();
                bb4.ResetLS1B();

                result = result + PieceAt(position, square);
            }
        }

        return result;
    }

    private static int PieceAt_KnowingColor(Position position, int targetSquare)
    {
        int targetPiece = (int)Piece.P;    // Important to initialize to P or p, due to en-passant captures

        var offset = Utils.PieceOffset(position.Side);
        var oppositePawnIndex = (int)Piece.p - offset;

        var limit = (int)Piece.k - offset;
        for (int pieceIndex = oppositePawnIndex; pieceIndex < limit; ++pieceIndex)
        {
            if (position.PieceBitBoards[pieceIndex].GetBit(targetSquare))
            {
                targetPiece = pieceIndex;
                break;
            }
        }

        return targetPiece;
    }

    /// <summary>
    /// Based on Stormphrax
    /// </summary>
    /// <param name="position"></param>
    /// <param name="targetSquare"></param>
    /// <returns></returns>
    private static int PieceAt(Position position, int targetSquare)
    {
        var bit = BitBoardExtensions.SquareBit(targetSquare);

        Side color;

        if ((position.OccupancyBitBoards[(int)Side.Black] & bit) != default)
        {
            color = Side.Black;
        }
        else if ((position.OccupancyBitBoards[(int)Side.White] & bit) != default)
        {
            color = Side.White;
        }
        else
        {
            return (int)Piece.None;
        }

        var offset = Utils.PieceOffset(color);

        for (int pieceIndex = offset; pieceIndex < 6 + offset; ++pieceIndex)
        {
            if (!(position.PieceBitBoards[pieceIndex] & bit).Empty())
            {
                return pieceIndex;
            }
        }

        System.Diagnostics.Debug.Fail($"Bit set in {position.Side} occupancy bitboard, but not piece found");

        return (int)Piece.None;
    }
}
