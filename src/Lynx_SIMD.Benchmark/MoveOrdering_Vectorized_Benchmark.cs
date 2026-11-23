/*
 *
 *  BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
 *  AMD Ryzen 9 9955HX 2.50GHz, 1 CPU, 32 logical and 16 physical cores
 *  .NET SDK 11.0.100
 *    [Host]    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
 *    Scalar    : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT
 *    Vector128 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
 *    Vector256 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
 *    Vector512 : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
 *
 *  IterationTime=250ms  MaxIterationCount=20  WarmupCount=1
 *
 *  | Method              | Job       | Count | Mean         | StdDev     | Ratio | Code Size |
 *  |-------------------- |---------- |------ |-------------:|-----------:|------:|----------:|
 *  | Overhead            | Scalar    | 3     |     2.951 ns |  0.0297 ns |  1.00 |     175 B |
 *  | Overhead            | Vector128 | 3     |     2.856 ns |  0.0835 ns |  0.97 |     175 B |
 *  | Overhead            | Vector256 | 3     |     2.922 ns |  0.0852 ns |  0.99 |     175 B |
 *  | Overhead            | Vector512 | 3     |     2.921 ns |  0.0737 ns |  0.99 |     175 B |
 *  | FirstPick           | Scalar    | 3     |     6.406 ns |  0.1575 ns |  1.00 |     245 B |
 *  | FirstPick           | Vector128 | 3     |     3.182 ns |  0.0536 ns |  0.50 |     303 B |
 *  | FirstPick           | Vector256 | 3     |     3.677 ns |  0.0606 ns |  0.57 |     323 B |
 *  | FirstPick           | Vector512 | 3     |     3.827 ns |  0.0636 ns |  0.60 |     344 B |
 *  | PickAll             | Scalar    | 3     |     9.908 ns |  0.3209 ns |  1.00 |     464 B |
 *  | PickAll             | Vector128 | 3     |    15.995 ns |  0.3899 ns |  1.62 |     559 B |
 *  | PickAll             | Vector256 | 3     |    18.025 ns |  0.0707 ns |  1.82 |     603 B |
 *  | PickAll             | Vector512 | 3     |    24.005 ns |  0.7709 ns |  2.43 |     650 B |
 *  | FirstPickBranchless | Scalar    | 3     |     6.480 ns |  0.1333 ns |  1.00 |     245 B |
 *  | FirstPickBranchless | Vector128 | 3     |     3.923 ns |  0.0683 ns |  0.61 |     345 B |
 *  | FirstPickBranchless | Vector256 | 3     |     4.042 ns |  0.1211 ns |  0.62 |     368 B |
 *  | FirstPickBranchless | Vector512 | 3     |     4.218 ns |  0.0807 ns |  0.65 |     384 B |
 *  | PickAllBranchless   | Scalar    | 3     |     9.978 ns |  0.1531 ns |  1.00 |     464 B |
 *  | PickAllBranchless   | Vector128 | 3     |    16.038 ns |  0.3548 ns |  1.61 |     442 B |
 *  | PickAllBranchless   | Vector256 | 3     |    18.733 ns |  0.4151 ns |  1.88 |     461 B |
 *  | PickAllBranchless   | Vector512 | 3     |    23.976 ns |  0.5121 ns |  2.40 |     476 B |
 *  | Overhead            | Scalar    | 8     |     4.215 ns |  0.0255 ns |  1.00 |     175 B |
 *  | Overhead            | Vector128 | 8     |     4.245 ns |  0.1160 ns |  1.01 |     175 B |
 *  | Overhead            | Vector256 | 8     |     4.160 ns |  0.0803 ns |  0.99 |     175 B |
 *  | Overhead            | Vector512 | 8     |     4.250 ns |  0.1467 ns |  1.01 |     175 B |
 *  | FirstPick           | Scalar    | 8     |    14.401 ns |  0.4039 ns |  1.00 |     262 B |
 *  | FirstPick           | Vector128 | 8     |     8.413 ns |  0.1216 ns |  0.58 |     300 B |
 *  | FirstPick           | Vector256 | 8     |     4.797 ns |  0.0672 ns |  0.33 |     323 B |
 *  | FirstPick           | Vector512 | 8     |     5.513 ns |  0.1593 ns |  0.38 |     344 B |
 *  | PickAll             | Scalar    | 8     |    42.666 ns |  2.0048 ns |  1.00 |     477 B |
 *  | PickAll             | Vector128 | 8     |    63.050 ns |  1.3938 ns |  1.48 |     621 B |
 *  | PickAll             | Vector256 | 8     |    73.598 ns |  1.2846 ns |  1.73 |     609 B |
 *  | PickAll             | Vector512 | 8     |    97.489 ns |  1.4879 ns |  2.29 |     656 B |
 *  | FirstPickBranchless | Scalar    | 8     |    13.325 ns |  0.1517 ns |  1.00 |     262 B |
 *  | FirstPickBranchless | Vector128 | 8     |     6.334 ns |  0.0688 ns |  0.48 |     358 B |
 *  | FirstPickBranchless | Vector256 | 8     |     5.341 ns |  0.0977 ns |  0.40 |     368 B |
 *  | FirstPickBranchless | Vector512 | 8     |     5.473 ns |  0.1591 ns |  0.41 |     384 B |
 *  | PickAllBranchless   | Scalar    | 8     |    45.287 ns |  2.1014 ns |  1.00 |     477 B |
 *  | PickAllBranchless   | Vector128 | 8     |    63.713 ns |  0.6637 ns |  1.41 |     717 B |
 *  | PickAllBranchless   | Vector256 | 8     |    73.000 ns |  2.4515 ns |  1.62 |     742 B |
 *  | PickAllBranchless   | Vector512 | 8     |    99.774 ns |  1.4953 ns |  2.21 |     778 B |
 *  | Overhead            | Scalar    | 16    |     6.437 ns |  0.1007 ns |  1.00 |     175 B |
 *  | Overhead            | Vector128 | 16    |     6.594 ns |  0.3005 ns |  1.02 |     175 B |
 *  | Overhead            | Vector256 | 16    |     6.311 ns |  0.1918 ns |  0.98 |     175 B |
 *  | Overhead            | Vector512 | 16    |     6.379 ns |  0.0597 ns |  0.99 |     175 B |
 *  | FirstPick           | Scalar    | 16    |    21.008 ns |  0.5702 ns |  1.00 |     262 B |
 *  | FirstPick           | Vector128 | 16    |    13.631 ns |  0.4757 ns |  0.65 |     314 B |
 *  | FirstPick           | Vector256 | 16    |    11.806 ns |  0.2828 ns |  0.56 |     325 B |
 *  | FirstPick           | Vector512 | 16    |     8.700 ns |  0.2683 ns |  0.41 |     344 B |
 *  | PickAll             | Scalar    | 16    |   186.635 ns |  4.8654 ns |  1.00 |     477 B |
 *  | PickAll             | Vector128 | 16    |   165.758 ns |  3.2839 ns |  0.89 |     597 B |
 *  | PickAll             | Vector256 | 16    |   169.482 ns |  3.6712 ns |  0.91 |     649 B |
 *  | PickAll             | Vector512 | 16    |   211.459 ns |  4.4716 ns |  1.13 |     664 B |
 *  | FirstPickBranchless | Scalar    | 16    |    21.110 ns |  0.5044 ns |  1.00 |     262 B |
 *  | FirstPickBranchless | Vector128 | 16    |     8.321 ns |  0.2447 ns |  0.39 |     372 B |
 *  | FirstPickBranchless | Vector256 | 16    |     7.437 ns |  0.1305 ns |  0.35 |     378 B |
 *  | FirstPickBranchless | Vector512 | 16    |     7.824 ns |  0.0877 ns |  0.37 |     384 B |
 *  | PickAllBranchless   | Scalar    | 16    |   188.912 ns |  3.1407 ns |  1.00 |     477 B |
 *  | PickAllBranchless   | Vector128 | 16    |   147.752 ns |  4.1595 ns |  0.78 |     725 B |
 *  | PickAllBranchless   | Vector256 | 16    |   168.659 ns |  5.0390 ns |  0.89 |     748 B |
 *  | PickAllBranchless   | Vector512 | 16    |   219.420 ns |  7.2398 ns |  1.16 |     789 B |
 *  | Overhead            | Scalar    | 32    |    10.667 ns |  0.1524 ns |  1.00 |     175 B |
 *  | Overhead            | Vector128 | 32    |    10.708 ns |  0.1860 ns |  1.00 |     175 B |
 *  | Overhead            | Vector256 | 32    |    10.636 ns |  0.1327 ns |  1.00 |     175 B |
 *  | Overhead            | Vector512 | 32    |    11.829 ns |  0.0834 ns |  1.11 |     175 B |
 *  | FirstPick           | Scalar    | 32    |    34.115 ns |  0.4014 ns |  1.00 |     262 B |
 *  | FirstPick           | Vector128 | 32    |    20.530 ns |  0.4577 ns |  0.60 |     325 B |
 *  | FirstPick           | Vector256 | 32    |    19.548 ns |  0.3159 ns |  0.57 |     334 B |
 *  | FirstPick           | Vector512 | 32    |    18.953 ns |  0.5510 ns |  0.56 |     332 B |
 *  | PickAll             | Scalar    | 32    |   685.143 ns | 17.8025 ns |  1.00 |     477 B |
 *  | PickAll             | Vector128 | 32    |   451.413 ns |  7.4050 ns |  0.66 |     596 B |
 *  | PickAll             | Vector256 | 32    |   438.350 ns |  4.3994 ns |  0.64 |     635 B |
 *  | PickAll             | Vector512 | 32    |   503.614 ns | 16.8384 ns |  0.74 |     665 B |
 *  | FirstPickBranchless | Scalar    | 32    |    33.917 ns |  0.9743 ns |  1.00 |     262 B |
 *  | FirstPickBranchless | Vector128 | 32    |    15.636 ns |  0.2107 ns |  0.46 |     372 B |
 *  | FirstPickBranchless | Vector256 | 32    |    12.819 ns |  0.4296 ns |  0.38 |     392 B |
 *  | FirstPickBranchless | Vector512 | 32    |    12.316 ns |  0.2172 ns |  0.36 |     394 B |
 *  | PickAllBranchless   | Scalar    | 32    |   670.114 ns | 14.9610 ns |  1.00 |     477 B |
 *  | PickAllBranchless   | Vector128 | 32    |   336.579 ns |  6.8992 ns |  0.50 |     713 B |
 *  | PickAllBranchless   | Vector256 | 32    |   349.035 ns |  3.6982 ns |  0.52 |     765 B |
 *  | PickAllBranchless   | Vector512 | 32    |   431.831 ns |  2.5209 ns |  0.64 |     784 B |
 *  | Overhead            | Scalar    | 48    |    14.838 ns |  0.4051 ns |  1.00 |     175 B |
 *  | Overhead            | Vector128 | 48    |    15.142 ns |  0.4727 ns |  1.02 |     175 B |
 *  | Overhead            | Vector256 | 48    |    14.638 ns |  0.5407 ns |  0.99 |     175 B |
 *  | Overhead            | Vector512 | 48    |    15.259 ns |  0.5113 ns |  1.03 |     175 B |
 *  | FirstPick           | Scalar    | 48    |    46.647 ns |  0.8688 ns |  1.00 |     262 B |
 *  | FirstPick           | Vector128 | 48    |    26.740 ns |  0.7840 ns |  0.57 |     325 B |
 *  | FirstPick           | Vector256 | 48    |    26.316 ns |  0.7033 ns |  0.56 |     334 B |
 *  | FirstPick           | Vector512 | 48    |    27.237 ns |  0.8142 ns |  0.58 |     332 B |
 *  | PickAll             | Scalar    | 48    | 1,191.639 ns | 36.3262 ns |  1.00 |     477 B |
 *  | PickAll             | Vector128 | 48    |   733.875 ns | 17.4511 ns |  0.62 |     596 B |
 *  | PickAll             | Vector256 | 48    |   770.037 ns | 13.2101 ns |  0.65 |     638 B |
 *  | PickAll             | Vector512 | 48    |   822.127 ns | 13.3565 ns |  0.69 |     654 B |
 *  | FirstPickBranchless | Scalar    | 48    |    44.035 ns |  1.0253 ns |  1.00 |     262 B |
 *  | FirstPickBranchless | Vector128 | 48    |    22.817 ns |  0.5629 ns |  0.52 |     372 B |
 *  | FirstPickBranchless | Vector256 | 48    |    19.131 ns |  0.1305 ns |  0.43 |     392 B |
 *  | FirstPickBranchless | Vector512 | 48    |    18.743 ns |  0.6416 ns |  0.43 |     394 B |
 *  | PickAllBranchless   | Scalar    | 48    | 1,217.105 ns | 18.5480 ns |  1.00 |     477 B |
 *  | PickAllBranchless   | Vector128 | 48    |   677.501 ns |  7.8539 ns |  0.56 |     706 B |
 *  | PickAllBranchless   | Vector256 | 48    |   569.526 ns | 10.6008 ns |  0.47 |     753 B |
 *  | PickAllBranchless   | Vector512 | 48    |   700.132 ns | 13.9741 ns |  0.58 |     781 B |
 *
 */

using BenchmarkDotNet.Attributes;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Lynx_SIMD.Benchmark;

/// <summary>
/// Scalar, Vector128, Vector256 and Vector512 versions of MovePicker.IndexOfFirstMax, which finds the next move to try.
/// There's one job per version (see Program.cs): each job only allows vectors up to its width and <see cref="IndexOfFirstMax"/> uses the widest one,
/// so the versions run with the same instruction set otherwise.
/// Scalar and Vector128 are copies of the engine ones; Vector256 and Vector512 are the ones tried in perf/vectorized-move-ordering.
/// The *Branchless methods use the vectorized versions that look for the first highest score in up to 64 scores before checking for a match
/// (see MovePicker.ScoresPerMatchMask), instead of stopping at the first vector that contains it; in the Scalar job they run the scalar version too.
/// <para/>
/// Score lists can't repeat: the branch predictor learns the outcomes of the scalar version's branches for thousands of repeated lists
/// (tested on Zen 5), while in the engine they're mispredicted, which is what makes it slower than the vectorized versions.
/// So each invocation uses the next list of a small pool, and then rewrites it as its original scores XOR a new random salt.
/// <see cref="Overhead"/> measures that rewrite alone: subtract it from <see cref="FirstPick"/> and <see cref="PickAll"/>
/// </summary>
public class MoveOrdering_Vectorized_Benchmark : BaseBenchmark
{
    /// <summary>
    /// <see cref="int.MinValue"/> scores after each list, like MovePicker.PadScores writes.
    /// The most any version reads after count is <see cref="Vector512{T}.Count"/> - 1
    /// </summary>
    private const int Padding = 16;

    /// <summary>
    /// Enough for each list to be read again long after being rewritten (no store forwarding stalls), and small enough to stay in L1
    /// </summary>
    private const int Lists = 64;

    /// <summary>
    /// Number of moves in a stage: usually a few captures, or 20-40 quiet moves
    /// </summary>
    [Params(3, 8, 16, 32, 48)]
    public int Count { get; set; }

    private int[] _scores = [];
    private int[] _originalScores = [];
    private int _stride;
    private int _offset;
    private uint _randomState;

    [GlobalSetup]
    public void Setup()
    {
        _stride = Count + Padding;
        _scores = new int[Lists * _stride];
        _originalScores = new int[Lists * _stride];
        _offset = 0;
        _randomState = 2_463_534_242;

        var random = new Random(42);
        for (int offset = 0; offset < _originalScores.Length; offset += _stride)
        {
            for (int i = 0; i < Count; ++i)
            {
                _originalScores[offset + i] = random.Next(-16_384, 16_384);
            }

            Array.Fill(_originalScores, int.MinValue, offset + Count, Padding);
        }

        Array.Copy(_originalScores, _scores, _scores.Length);

        // Sanity checks: the job runs the expected version, and it returns the same as the scalar one
        Console.WriteLine($"// {nameof(IndexOfFirstMax)} version: {Version}");

        for (int offset = 0; offset < _scores.Length; offset += _stride)
        {
            var list = _scores.AsSpan(offset, _stride);

            for (int start = 0; start < Count; ++start)
            {
                var expected = IndexOfFirstMaxScalar(list, start, Count);

                if (IndexOfFirstMax(list, start, Count) != expected)
                {
                    throw new InvalidOperationException($"{Version} and scalar versions differ for list at offset {offset}, start {start}");
                }

                if (IndexOfFirstMaxBranchless(list, start, Count) != expected)
                {
                    throw new InvalidOperationException($"{Version} branchless and scalar versions differ for list at offset {offset}, start {start}");
                }
            }
        }
    }

    private static string Version =>
        Vector512.IsHardwareAccelerated ? "Vector512"
        : Vector256.IsHardwareAccelerated ? "Vector256"
        : Vector128.IsHardwareAccelerated ? "Vector128"
        : "Scalar";

    /// <summary>
    /// Only rewrites the next list, the part of <see cref="FirstPick"/> and <see cref="PickAll"/> that isn't picking
    /// </summary>
    [Benchmark]
    public int Overhead()
    {
        var offset = _offset;
        var first = _scores[offset];

        RewriteAndAdvance(offset);

        return first;
    }

    /// <summary>
    /// First pick of a stage, i.e. the best score of the whole list: what most nodes do before a cutoff
    /// </summary>
    [Benchmark]
    public int FirstPick()
    {
        var offset = _offset;
        var best = IndexOfFirstMax(_scores.AsSpan(offset, _stride), 0, Count);

        RewriteAndAdvance(offset);

        return best;
    }

    /// <summary>
    /// All the picks of a stage, like MovePicker.TrySelectNext does when there's no cutoff:
    /// <see cref="Count"/> calls on a shrinking range, each one followed by swapping the best score to the start of the range
    /// </summary>
    [Benchmark]
    public int PickAll()
    {
        var offset = _offset;
        var count = Count;
        var scores = _scores.AsSpan(offset, _stride);

        var sum = 0;
        for (int start = 0; start < count; ++start)
        {
            var best = IndexOfFirstMax(scores, start, count);

            if (best != start)
            {
                (scores[start], scores[best]) = (scores[best], scores[start]);
            }

            sum += best;
        }

        RewriteAndAdvance(offset);

        return sum;
    }

    /// <summary>
    /// <see cref="FirstPick"/> with <see cref="IndexOfFirstMaxBranchless"/>
    /// </summary>
    [Benchmark]
    public int FirstPickBranchless()
    {
        var offset = _offset;
        var best = IndexOfFirstMaxBranchless(_scores.AsSpan(offset, _stride), 0, Count);

        RewriteAndAdvance(offset);

        return best;
    }

    /// <summary>
    /// <see cref="PickAll"/> with <see cref="IndexOfFirstMaxBranchless"/>
    /// </summary>
    [Benchmark]
    public int PickAllBranchless()
    {
        var offset = _offset;
        var count = Count;
        var scores = _scores.AsSpan(offset, _stride);

        var sum = 0;
        for (int start = 0; start < count; ++start)
        {
            var best = IndexOfFirstMaxBranchless(scores, start, count);

            if (best != start)
            {
                (scores[start], scores[best]) = (scores[best], scores[start]);
            }

            sum += best;
        }

        RewriteAndAdvance(offset);

        return sum;
    }

    /// <summary>
    /// Rewrites the list at <paramref name="offset"/> as its original scores XOR a new random salt (xorshift32), so that its next use is different,
    /// and moves on to the next list.
    /// A plain loop, so that it's the same code in every job
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RewriteAndAdvance(int offset)
    {
        var x = _randomState;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        _randomState = x;

        var salt = (int)(x & 0x7FFF);

        var scores = _scores;
        var originalScores = _originalScores;
        for (int i = offset; i < offset + Count; ++i)
        {
            scores[i] = originalScores[i] ^ salt;
        }

        var next = offset + _stride;
        _offset = next == scores.Length ? 0 : next;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfFirstMax(ReadOnlySpan<int> scores, int start, int count)
    {
        if (Vector512.IsHardwareAccelerated)
        {
            return IndexOfFirstMaxVector512(scores, start, count);
        }

        if (Vector256.IsHardwareAccelerated)
        {
            return IndexOfFirstMaxVector256(scores, start, count);
        }

        if (Vector128.IsHardwareAccelerated)
        {
            return IndexOfFirstMaxVector128(scores, start, count);
        }

        return IndexOfFirstMaxScalar(scores, start, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfFirstMaxVector512(ReadOnlySpan<int> scores, int start, int count)
    {
        ref var scoresRef = ref MemoryMarshal.GetReference(scores);

        var max = Vector512.LoadUnsafe(ref scoresRef, (nuint)start);
        for (int i = start + Vector512<int>.Count; i < count; i += Vector512<int>.Count)
        {
            max = Vector512.Max(max, Vector512.LoadUnsafe(ref scoresRef, (nuint)i));
        }

        var max256 = Vector256.Max(max.GetLower(), max.GetUpper());
        var max128 = Vector128.Max(max256.GetLower(), max256.GetUpper());
        max128 = Vector128.Max(max128, Vector128.Shuffle(max128, Vector128.Create(2, 3, 0, 1)));
        max128 = Vector128.Max(max128, Vector128.Shuffle(max128, Vector128.Create(1, 0, 3, 2)));
        max = Vector512.Create(max128.ToScalar());

        var index = start;
        while (true)
        {
            var matches = Vector512.Equals(Vector512.LoadUnsafe(ref scoresRef, (nuint)index), max).ExtractMostSignificantBits();

            if (matches != 0)
            {
                return index + BitOperations.TrailingZeroCount(matches);
            }

            index += Vector512<int>.Count;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfFirstMaxVector256(ReadOnlySpan<int> scores, int start, int count)
    {
        ref var scoresRef = ref MemoryMarshal.GetReference(scores);

        var max = Vector256.LoadUnsafe(ref scoresRef, (nuint)start);
        for (int i = start + Vector256<int>.Count; i < count; i += Vector256<int>.Count)
        {
            max = Vector256.Max(max, Vector256.LoadUnsafe(ref scoresRef, (nuint)i));
        }

        var max128 = Vector128.Max(max.GetLower(), max.GetUpper());
        max128 = Vector128.Max(max128, Vector128.Shuffle(max128, Vector128.Create(2, 3, 0, 1)));
        max128 = Vector128.Max(max128, Vector128.Shuffle(max128, Vector128.Create(1, 0, 3, 2)));
        max = Vector256.Create(max128, max128);

        var index = start;
        while (true)
        {
            var matches = Vector256.Equals(Vector256.LoadUnsafe(ref scoresRef, (nuint)index), max).ExtractMostSignificantBits();

            if (matches != 0)
            {
                return index + BitOperations.TrailingZeroCount(matches);
            }

            index += Vector256<int>.Count;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfFirstMaxVector128(ReadOnlySpan<int> scores, int start, int count)
    {
        ref var scoresRef = ref MemoryMarshal.GetReference(scores);

        var max = Vector128.LoadUnsafe(ref scoresRef, (nuint)start);
        for (int i = start + Vector128<int>.Count; i < count; i += Vector128<int>.Count)
        {
            max = Vector128.Max(max, Vector128.LoadUnsafe(ref scoresRef, (nuint)i));
        }

        max = Vector128.Max(max, Vector128.Shuffle(max, Vector128.Create(2, 3, 0, 1)));
        max = Vector128.Max(max, Vector128.Shuffle(max, Vector128.Create(1, 0, 3, 2)));

        var index = start;
        while (true)
        {
            var matches = Vector128.Equals(Vector128.LoadUnsafe(ref scoresRef, (nuint)index), max).ExtractMostSignificantBits();

            if (matches != 0)
            {
                return index + BitOperations.TrailingZeroCount(matches);
            }

            index += Vector128<int>.Count;
        }
    }

    private const int ScoresPerMatchMask = 64;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfFirstMaxBranchless(ReadOnlySpan<int> scores, int start, int count)
    {
        if (Vector512.IsHardwareAccelerated)
        {
            return IndexOfFirstMaxVector512Branchless(scores, start, count);
        }

        if (Vector256.IsHardwareAccelerated)
        {
            return IndexOfFirstMaxVector256Branchless(scores, start, count);
        }

        if (Vector128.IsHardwareAccelerated)
        {
            return IndexOfFirstMaxVector128Branchless(scores, start, count);
        }

        return IndexOfFirstMaxScalar(scores, start, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfFirstMaxVector512Branchless(ReadOnlySpan<int> scores, int start, int count)
    {
        ref var scoresRef = ref MemoryMarshal.GetReference(scores);

        var max = Vector512.LoadUnsafe(ref scoresRef, (nuint)start);
        for (int i = start + Vector512<int>.Count; i < count; i += Vector512<int>.Count)
        {
            max = Vector512.Max(max, Vector512.LoadUnsafe(ref scoresRef, (nuint)i));
        }

        var max256 = Vector256.Max(max.GetLower(), max.GetUpper());
        var max128 = Vector128.Max(max256.GetLower(), max256.GetUpper());
        max128 = Vector128.Max(max128, Vector128.Shuffle(max128, Vector128.Create(2, 3, 0, 1)));
        max128 = Vector128.Max(max128, Vector128.Shuffle(max128, Vector128.Create(1, 0, 3, 2)));
        max = Vector512.Create(max128.ToScalar());

        var index = start;
        while (true)
        {
            var end = Math.Min(count, index + ScoresPerMatchMask);

            ulong matches = 0;
            for (int i = index; i < end; i += Vector512<int>.Count)
            {
                matches |= (Vector512.Equals(Vector512.LoadUnsafe(ref scoresRef, (nuint)i), max).ExtractMostSignificantBits()
                    << (i - index));
            }

            if (matches != 0)
            {
                return index + BitOperations.TrailingZeroCount(matches);
            }

            index += ScoresPerMatchMask;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfFirstMaxVector256Branchless(ReadOnlySpan<int> scores, int start, int count)
    {
        ref var scoresRef = ref MemoryMarshal.GetReference(scores);

        var max = Vector256.LoadUnsafe(ref scoresRef, (nuint)start);
        for (int i = start + Vector256<int>.Count; i < count; i += Vector256<int>.Count)
        {
            max = Vector256.Max(max, Vector256.LoadUnsafe(ref scoresRef, (nuint)i));
        }

        var max128 = Vector128.Max(max.GetLower(), max.GetUpper());
        max128 = Vector128.Max(max128, Vector128.Shuffle(max128, Vector128.Create(2, 3, 0, 1)));
        max128 = Vector128.Max(max128, Vector128.Shuffle(max128, Vector128.Create(1, 0, 3, 2)));
        max = Vector256.Create(max128, max128);

        var index = start;
        while (true)
        {
            var end = Math.Min(count, index + ScoresPerMatchMask);

            ulong matches = 0;
            for (int i = index; i < end; i += Vector256<int>.Count)
            {
                matches |= ((ulong)Vector256.Equals(Vector256.LoadUnsafe(ref scoresRef, (nuint)i), max).ExtractMostSignificantBits()
                    << (i - index));
            }

            if (matches != 0)
            {
                return index + BitOperations.TrailingZeroCount(matches);
            }

            index += ScoresPerMatchMask;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfFirstMaxVector128Branchless(ReadOnlySpan<int> scores, int start, int count)
    {
        ref var scoresRef = ref MemoryMarshal.GetReference(scores);

        var max = Vector128.LoadUnsafe(ref scoresRef, (nuint)start);
        for (int i = start + Vector128<int>.Count; i < count; i += Vector128<int>.Count)
        {
            max = Vector128.Max(max, Vector128.LoadUnsafe(ref scoresRef, (nuint)i));
        }

        max = Vector128.Max(max, Vector128.Shuffle(max, Vector128.Create(2, 3, 0, 1)));
        max = Vector128.Max(max, Vector128.Shuffle(max, Vector128.Create(1, 0, 3, 2)));

        var index = start;
        while (true)
        {
            var end = Math.Min(count, index + ScoresPerMatchMask);

            ulong matches = 0;
            for (int i = index; i < end; i += Vector128<int>.Count)
            {
                matches |= ((ulong)Vector128.Equals(Vector128.LoadUnsafe(ref scoresRef, (nuint)i), max).ExtractMostSignificantBits()
                    << (i - index));
            }

            if (matches != 0)
            {
                return index + BitOperations.TrailingZeroCount(matches);
            }

            index += ScoresPerMatchMask;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int IndexOfFirstMaxScalar(ReadOnlySpan<int> scores, int start, int count)
    {
        ref var scoresRef = ref MemoryMarshal.GetReference(scores);

        var bestIndex = start;
        var bestScore = Unsafe.Add(ref scoresRef, start);

        for (int i = start + 1; i < count; ++i)
        {
            var score = Unsafe.Add(ref scoresRef, i);

            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex;
    }
}
