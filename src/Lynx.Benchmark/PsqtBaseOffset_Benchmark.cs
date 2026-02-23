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
