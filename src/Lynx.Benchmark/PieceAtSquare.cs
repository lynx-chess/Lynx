/*
 * Pretty much the same
 *
 *  BenchmarkDotNet v0.13.11, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                   | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |------------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
 *  | KnowingColor             | 1.975 us | 0.0057 us | 0.0051 us |  1.00 | 0.0038 |     600 B |        1.00 |
 *  | KnowingColor_Unrolled    | 1.983 us | 0.0044 us | 0.0036 us |  1.00 | 0.0038 |     600 B |        1.00 |
 *  | NotKnowingColor          | 2.017 us | 0.0141 us | 0.0125 us |  1.02 | 0.0038 |     600 B |        1.00 |
 *  | NotKnowingColor_Unrolled | 2.021 us | 0.0201 us | 0.0188 us |  1.02 | 0.0038 |     600 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.13.11, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                   | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |------------------------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
 *  | KnowingColor             | 1.871 us | 0.0044 us | 0.0037 us |  1.00 | 0.0343 |     600 B |        1.00 |
 *  | KnowingColor_Unrolled    | 1.819 us | 0.0038 us | 0.0032 us |  0.97 | 0.0343 |     600 B |        1.00 |
 *  | NotKnowingColor          | 1.819 us | 0.0033 us | 0.0026 us |  0.97 | 0.0343 |     600 B |        1.00 |
 *  | NotKnowingColor_Unrolled | 1.810 us | 0.0026 us | 0.0022 us |  0.97 | 0.0343 |     600 B |        1.00 |
 *
 *
 *  BenchmarkDotNet v0.13.11, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.100
 *    [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
 *
 *  | Method                   | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |------------------------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
 *  | KnowingColor             | 2.178 us | 0.0194 us | 0.0181 us |  1.00 |    0.00 | 0.0954 |     600 B |        1.00 |
 *  | KnowingColor_Unrolled    | 2.225 us | 0.0220 us | 0.0184 us |  1.02 |    0.01 | 0.0954 |     600 B |        1.00 |
 *  | NotKnowingColor          | 2.224 us | 0.0296 us | 0.0277 us |  1.02 |    0.02 | 0.0954 |     600 B |        1.00 |
 *  | NotKnowingColor_Unrolled | 2.210 us | 0.0398 us | 0.0372 us |  1.01 |    0.02 | 0.0954 |     600 B |        1.00 |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

#pragma warning disable RCS1058, IDE0054 // Use compound assignment

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

#pragma warning restore RCS1058, IDE0054 // Use compound assignment
