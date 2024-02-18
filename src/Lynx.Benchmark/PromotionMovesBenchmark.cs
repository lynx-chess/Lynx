/*
 * Some other runs also favor the increment at the end approach, so will take my chance
 *
 * BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 * AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 * .NET SDK 8.0.201
 *   [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 * | Method               | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 * |--------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 * | Naive                | 2.784 ns | 0.0244 ns | 0.0228 ns |  1.00 |    0.00 |         - |          NA |
 * | Increment_At_The_End | 2.796 ns | 0.0205 ns | 0.0192 ns |  1.00 |    0.01 |         - |          NA |
 * | ArrayCopyToSpan      | 4.043 ns | 0.0271 ns | 0.0240 ns |  1.45 |    0.02 |         - |          NA |
 *
 *
 * BenchmarkDotNet v0.13.12, Windows 10 (10.0.20348.2227) (Hyper-V)
 * AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 * .NET SDK 8.0.201
 *   [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *
 * | Method               | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 * |--------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 * | Naive                | 2.760 ns | 0.0076 ns | 0.0068 ns |  1.00 |         - |          NA |
 * | Increment_At_The_End | 2.755 ns | 0.0147 ns | 0.0138 ns |  1.00 |         - |          NA |
 * | ArrayCopyToSpan      | 3.674 ns | 0.0086 ns | 0.0081 ns |  1.33 |         - |          NA |
 *
 *
 * BenchmarkDotNet v0.13.12, macOS Monterey 12.7.3 (21H1015) [Darwin 21.6.0]
 * Intel Core i7-8700B CPU 3.20GHz (Max: 3.19GHz) (Coffee Lake), 1 CPU, 4 logical and 4 physical cores
 * .NET SDK 8.0.201
 *   [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *
 * | Method               | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 * |--------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 * | Naive                | 3.898 ns | 0.0582 ns | 0.0545 ns |  1.00 |    0.00 |         - |          NA |
 * | Increment_At_The_End | 3.885 ns | 0.0490 ns | 0.0434 ns |  1.00 |    0.02 |         - |          NA |
 * | ArrayCopyToSpan      | 4.465 ns | 0.0503 ns | 0.0471 ns |  1.15 |    0.02 |         - |          NA |
 *
 *
 * BenchmarkDotNet v0.13.12, macOS Sonoma 14.2.1 (23C71) [Darwin 23.2.0]
 * Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 * .NET SDK 8.0.201
 *   [Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
 *   DefaultJob : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
 *
 * | Method               | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 * |--------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 * | Naive                | 2.107 ns | 0.0058 ns | 0.0045 ns |  1.00 |         - |          NA |
 * | Increment_At_The_End | 2.042 ns | 0.0064 ns | 0.0057 ns |  0.97 |         - |          NA |
 * | ArrayCopyToSpan      | 2.525 ns | 0.0035 ns | 0.0030 ns |  1.20 |         - |          NA |
 *
*/

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using static Lynx.PregeneratedMoves;

namespace Lynx.Benchmark;
public class PromotionMovesBenchmark : BaseBenchmark
{
    private Memory<Move> _movePool;

    [GlobalSetup]
    public void Setup()
    {
        _movePool = new Memory<Move>(new Move[Constants.MaxNumberOfPossibleMovesInAPosition]);
    }

    [Benchmark(Baseline = true)]
    public int Naive()
    {
        int localIndex = 0;
        return PregeneratedWhitePromotions_Naive(_movePool.Span, ref localIndex, (int)BoardSquare.e8);
    }

    [Benchmark]
    public int Increment_At_The_End()
    {
        int localIndex = 0;
        return PregeneratedWhitePromotions_Increment_At_The_End(_movePool.Span, ref localIndex, (int)BoardSquare.e8);
    }

    [Benchmark]
    public int ArrayCopyToSpan()
    {
        int localIndex = 0;
        return PregeneratedWhitePromotions_ArrayCopyToSpan(_movePool.Span, ref localIndex, (int)BoardSquare.e8);
    }

    public int PregeneratedWhitePromotions_Naive(Span<Move> movePool, ref int localIndex, int singlePushSquare)
    {
        var whitePromotions = WhitePromotions[singlePushSquare];

        movePool[localIndex++] = whitePromotions[0];
        movePool[localIndex++] = whitePromotions[1];
        movePool[localIndex++] = whitePromotions[2];
        movePool[localIndex++] = whitePromotions[3];

        return localIndex;
    }

    public int PregeneratedWhitePromotions_Increment_At_The_End(Span<Move> movePool, ref int localIndex, int singlePushSquare)
    {
        var whitePromotions = WhitePromotions[singlePushSquare];

        movePool[localIndex] = whitePromotions[0];
        movePool[localIndex + 1] = whitePromotions[1];
        movePool[localIndex + 2] = whitePromotions[2];
        movePool[localIndex + 3] = whitePromotions[3];

        localIndex += 4;

        return localIndex;
    }

    public int PregeneratedWhitePromotions_ArrayCopyToSpan(Span<Move> movePool, ref int localIndex, int singlePushSquare)
    {
        WhitePromotions[singlePushSquare].CopyTo(movePool[localIndex..]);
        localIndex += 4;
        return localIndex;
    }
}
