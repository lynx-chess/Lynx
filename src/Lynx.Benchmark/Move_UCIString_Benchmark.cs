/*
 * I was honestly expecting bigger gains
 *
 *  BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method         | Mean     | Error   | StdDev  | Ratio | Gen0   | Allocated | Alloc Ratio |
 *  |--------------- |---------:|--------:|--------:|------:|-------:|----------:|------------:|
 *  | NaiveUCIString | 229.7 ns | 2.45 ns | 2.29 ns |  1.00 | 0.0076 |     648 B |        1.00 |
 *  | SpanUCIString  | 225.8 ns | 1.16 ns | 1.03 ns |  0.98 | 0.0072 |     600 B |        0.93 |
 *
 *
 *  BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2159) (Hyper-V)
 *  AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *  | Method         | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |--------------- |---------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
 *  | NaiveUCIString | 180.5 ns | 3.59 ns | 3.00 ns |  1.00 |    0.00 | 0.0386 |     648 B |        1.00 |
 *  | SpanUCIString  | 179.1 ns | 1.92 ns | 1.60 ns |  0.99 |    0.02 | 0.0358 |     600 B |        0.93 |
 *
 *
 *  BenchmarkDotNet v0.13.12, macOS Monterey 12.7.2 (21G1974) [Darwin 21.6.0]
 *  Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 *  .NET SDK 8.0.101
 *    [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *    DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
 *
 *
 *  | Method         | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
 *  |--------------- |---------:|---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
 *  | NaiveUCIString | 339.3 ns |  4.43 ns |  3.70 ns | 339.6 ns |  1.00 |    0.00 | 0.1030 |     648 B |        1.00 |
 *  | SpanUCIString  | 349.6 ns | 10.09 ns | 28.94 ns | 337.8 ns |  0.99 |    0.06 | 0.0954 |     600 B |        0.93 |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Text;

namespace Lynx.Benchmark;
public class Move_UCIString_Benchmark : BaseBenchmark
{
    private static readonly Move[] _moves =
    [
        MoveExtensions.EncodeShortCastle(Constants.InitialWhiteKingSquare, Constants.WhiteKingShortCastleSquare, (int)Piece.K),
        MoveExtensions.EncodeLongCastle(Constants.InitialBlackKingSquare, Constants.BlackKingLongCastleSquare, (int)Piece.k),
        MoveExtensions.Encode((int)BoardSquare.e2, (int)BoardSquare.e4, (int)Piece.P),
        MoveExtensions.EncodePromotion((int)BoardSquare.e7, (int)BoardSquare.e8, (int)Piece.p, promotedPiece: (int)Piece.q),
        MoveExtensions.EncodePromotion((int)BoardSquare.a7, (int)BoardSquare.b8, (int)Piece.p, promotedPiece: (int)Piece.n, capturedPiece: (int)Piece.B),
        MoveExtensions.EncodeCapture((int)BoardSquare.a8, (int)BoardSquare.h1, (int)Piece.B, capturedPiece: (int)Piece.b),
        MoveExtensions.EncodeEnPassant((int)BoardSquare.e5, (int)BoardSquare.d6, (int)Piece.P)
    ];

    [Benchmark(Baseline = true)]
    public StringBuilder NaiveUCIString()
    {
        var sb = new StringBuilder();
        foreach (var move in _moves)
        {
            sb.Append(move.NaiveUCIString());
        }

        return sb;
    }

    [Benchmark]
    public StringBuilder SpanUCIString()
    {
        var sb = new StringBuilder();
        foreach (var move in _moves)
        {
            sb.Append(move.SpanUCIString());
        }

        return sb;
    }
}

file static class MoveHelpers
{
    public static string NaiveUCIString(this Move move)
    {
        return
            Constants.Coordinates[move.SourceSquare()] +
            Constants.Coordinates[move.TargetSquare()] +
            (move.PromotedPiece() == default ? "" : $"{Constants.AsciiPieces[move.PromotedPiece()].ToString().ToLowerInvariant()}");
    }

    public static string SpanUCIString(this Move move)
    {
        Span<char> span = stackalloc char[5];

        var source = Constants.CoordinatesCharArray[move.SourceSquare()];
        span[0] = source[0];
        span[1] = source[1];

        var target = Constants.CoordinatesCharArray[move.TargetSquare()];
        span[2] = target[0];
        span[3] = target[1];

        var promotedPiece = move.PromotedPiece();
        if (promotedPiece != default)
        {
            span[4] = Constants.AsciiPiecesLowercase[promotedPiece];

            return span.ToString();
        }

        return span[..^1].ToString();
    }
}
