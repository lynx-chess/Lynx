/*
 * Some other runs also favor the increment at the end approach, so will take my chance
 *
 * BenchmarkDotNet v0.13.12, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
 * AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
 * .NET SDK 8.0.201
 *   [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *   DefaultJob : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2
 *
 * | Method               | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 * |--------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 * | Naive                | 2.801 ns | 0.0084 ns | 0.0065 ns |  1.00 |         - |          NA |
 * | Increment_At_The_End | 2.761 ns | 0.0066 ns | 0.0051 ns |  0.99 |         - |          NA |
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
 * | Naive                | 2.744 ns | 0.0071 ns | 0.0066 ns |  1.00 |         - |          NA |
 * | Increment_At_The_End | 2.738 ns | 0.0064 ns | 0.0060 ns |  1.00 |         - |          NA |
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
 * | Naive                | 4.542 ns | 0.3233 ns | 0.9532 ns |  1.00 |    0.00 |         - |          NA |
 * | Increment_At_The_End | 3.830 ns | 0.0625 ns | 0.0554 ns |  0.73 |    0.08 |         - |          NA |
 *
 *
 * BenchmarkDotNet v0.13.12, macOS Sonoma 14.2.1 (23C71) [Darwin 23.2.0]
 * Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 * .NET SDK 8.0.201
 *   [Host]     : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
 *   DefaultJob : .NET 8.0.2 (8.0.224.6711), Arm64 RyuJIT AdvSIMD
 *
 * | Method               | Mean     | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
 * |--------------------- |---------:|----------:|----------:|------:|--------:|----------:|------------:|
 * | Naive                | 2.228 ns | 0.0558 ns | 0.0643 ns |  1.00 |    0.00 |         - |          NA |
 * | Increment_At_The_End | 2.380 ns | 0.0718 ns | 0.0768 ns |  1.07 |    0.04 |         - |          NA |
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
}
