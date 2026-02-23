/*
 *
 *
 *  BenchmarkDotNet v0.15.8, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763 2.66GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *
 *  | Method                                | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |-------------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | RawIndexing_Baseline                  | 2.738 us | 0.0054 us | 0.0050 us |  1.00 |         - |          NA |
 *  | RawIndexing_BaseOffsetOverload        | 2.734 us | 0.0028 us | 0.0026 us |  1.00 |         - |          NA |
 *  | IncrementalPattern_Baseline           | 1.498 us | 0.0015 us | 0.0013 us |  0.55 |         - |          NA |
 *  | IncrementalPattern_BaseOffsetOverload | 1.483 us | 0.0011 us | 0.0009 us |  0.54 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.32370/24H2/2024Update/HudsonValley) (Hyper-V)
 *  AMD EPYC 7763 2.44GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
 *
 *  | Method                                | Mean     | Error     | StdDev    | Ratio | Allocated | Alloc Ratio |
 *  |-------------------------------------- |---------:|----------:|----------:|------:|----------:|------------:|
 *  | RawIndexing_Baseline                  | 2.595 us | 0.0196 us | 0.0183 us |  1.00 |         - |          NA |
 *  | RawIndexing_BaseOffsetOverload        | 2.597 us | 0.0125 us | 0.0111 us |  1.00 |         - |          NA |
 *  | IncrementalPattern_Baseline           | 1.431 us | 0.0167 us | 0.0156 us |  0.55 |         - |          NA |
 *  | IncrementalPattern_BaseOffsetOverload | 1.403 us | 0.0150 us | 0.0133 us |  0.54 |         - |          NA |
 *
 *
 *  BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
 *  Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
 *  .NET SDK 10.0.103
 *    [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *    DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
 *
 *  | Method                                | Mean     | Error     | StdDev    | Median   | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |-------------------------------------- |---------:|----------:|----------:|---------:|------:|--------:|----------:|------------:|
 *  | RawIndexing_Baseline                  | 2.236 us | 0.0770 us | 0.2221 us | 2.194 us |  1.01 |    0.14 |         - |          NA |
 *  | RawIndexing_BaseOffsetOverload        | 2.188 us | 0.0963 us | 0.2793 us | 2.070 us |  0.99 |    0.16 |         - |          NA |
 *  | IncrementalPattern_Baseline           | 1.201 us | 0.0835 us | 0.2396 us | 1.113 us |  0.54 |    0.12 |         - |          NA |
 *  | IncrementalPattern_BaseOffsetOverload | 1.110 us | 0.0574 us | 0.1676 us | 1.040 us |  0.50 |    0.09 |         - |          NA |
 *
*/

using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class PsqtBaseOffset_Benchmark : BaseBenchmark
{
    private const int BucketCount = 23;
    private const int PieceCount = 12;
    private const int SquareCount = 64;
    private const int FriendBucketStride = PieceCount * SquareCount * BucketCount;
    private const int EnemyBucketStride = PieceCount * SquareCount;
    private const int PieceStride = SquareCount;

    private readonly int[] _packedPsqt = GC.AllocateArray<int>(BucketCount * BucketCount * PieceCount * SquareCount, pinned: true);
    private readonly int[] _friendBucket = new int[1024];
    private readonly int[] _enemyBucket = new int[1024];
    private readonly int[] _piece = new int[1024];
    private readonly int[] _square = new int[1024];

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1337);

        for (int i = 0; i < _packedPsqt.Length; ++i)
        {
            _packedPsqt[i] = random.Next();
        }

        for (int i = 0; i < _friendBucket.Length; ++i)
        {
            _friendBucket[i] = random.Next(BucketCount);
            _enemyBucket[i] = random.Next(BucketCount);
            _piece[i] = random.Next(PieceCount);
            _square[i] = random.Next(SquareCount);
        }
    }

    [Benchmark(Baseline = true)]
    public int RawIndexing_Baseline()
    {
        int sum = 0;

        for (int i = 0; i < _friendBucket.Length; ++i)
        {
            var value = ReadBaseline(_friendBucket[i], _enemyBucket[i], _piece[i], _square[i]);
            sum ^= value;
        }

        return sum;
    }

    [Benchmark]
    public int RawIndexing_BaseOffsetOverload()
    {
        int sum = 0;

        for (int i = 0; i < _friendBucket.Length; ++i)
        {
            var baseOffset = BaseOffset(_friendBucket[i], _enemyBucket[i]);
            var value = ReadWithBaseOffset(baseOffset, _piece[i], _square[i]);
            sum ^= value;
        }

        return sum;
    }

    [Benchmark]
    public int IncrementalPattern_Baseline()
    {
        int sum = 0;

        for (int i = 0; i < _friendBucket.Length - 3; i += 4)
        {
            var sameSideBucket = _friendBucket[i];
            var oppositeSideBucket = _enemyBucket[i];

            sum += ReadBaseline(sameSideBucket, oppositeSideBucket, _piece[i], _square[i]);
            sum -= ReadBaseline(sameSideBucket, oppositeSideBucket, _piece[i + 1], _square[i + 1]);
            sum += ReadBaseline(oppositeSideBucket, sameSideBucket, _piece[i + 2], _square[i + 2]);
            sum -= ReadBaseline(oppositeSideBucket, sameSideBucket, _piece[i + 3], _square[i + 3]);
        }

        return sum;
    }

    [Benchmark]
    public int IncrementalPattern_BaseOffsetOverload()
    {
        int sum = 0;

        for (int i = 0; i < _friendBucket.Length - 3; i += 4)
        {
            var sameSideBucket = _friendBucket[i];
            var oppositeSideBucket = _enemyBucket[i];

            var sameSideBaseOffset = BaseOffset(sameSideBucket, oppositeSideBucket);
            var oppositeSideBaseOffset = BaseOffset(oppositeSideBucket, sameSideBucket);

            sum += ReadWithBaseOffset(sameSideBaseOffset, _piece[i], _square[i]);
            sum -= ReadWithBaseOffset(sameSideBaseOffset, _piece[i + 1], _square[i + 1]);
            sum += ReadWithBaseOffset(oppositeSideBaseOffset, _piece[i + 2], _square[i + 2]);
            sum -= ReadWithBaseOffset(oppositeSideBaseOffset, _piece[i + 3], _square[i + 3]);
        }

        return sum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int BaseOffset(int friendBucket, int enemyBucket) => (friendBucket * FriendBucketStride) + (enemyBucket * EnemyBucketStride);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ReadBaseline(int friendBucket, int enemyBucket, int piece, int square)
    {
        var index = (friendBucket * FriendBucketStride)
            + (enemyBucket * EnemyBucketStride)
            + (piece * PieceStride)
            + square;

        return _packedPsqt[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ReadWithBaseOffset(int baseOffset, int piece, int square)
    {
        var index = baseOffset
            + (piece * PieceStride)
            + square;

        return _packedPsqt[index];
    }
}
