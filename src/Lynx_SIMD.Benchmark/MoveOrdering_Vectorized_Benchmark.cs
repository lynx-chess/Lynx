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
 *  | Method    | Job       | Count | Mean         | StdDev     | Ratio | Code Size |
 *  |---------- |---------- |------ |-------------:|-----------:|------:|----------:|
 *  | Overhead  | Scalar    | 3     |     2.860 ns |  0.0603 ns |  1.00 |     175 B |
 *  | Overhead  | Vector128 | 3     |     2.923 ns |  0.1161 ns |  1.02 |     175 B |
 *  | Overhead  | Vector256 | 3     |     2.845 ns |  0.0682 ns |  1.00 |     175 B |
 *  | Overhead  | Vector512 | 3     |     2.783 ns |  0.0202 ns |  0.97 |     175 B |
 *  | FirstPick | Scalar    | 3     |     6.365 ns |  0.1455 ns |  1.00 |     245 B |
 *  | FirstPick | Vector128 | 3     |     3.113 ns |  0.0724 ns |  0.49 |     303 B |
 *  | FirstPick | Vector256 | 3     |     3.477 ns |  0.0686 ns |  0.55 |     323 B |
 *  | FirstPick | Vector512 | 3     |     3.750 ns |  0.0630 ns |  0.59 |     344 B |
 *  | PickAll   | Scalar    | 3     |     9.694 ns |  0.2677 ns |  1.00 |     464 B |
 *  | PickAll   | Vector128 | 3     |    14.663 ns |  0.0910 ns |  1.51 |     559 B |
 *  | PickAll   | Vector256 | 3     |    18.308 ns |  0.8474 ns |  1.89 |     603 B |
 *  | PickAll   | Vector512 | 3     |    23.202 ns |  0.6663 ns |  2.40 |     650 B |
 *  | Overhead  | Scalar    | 8     |     4.119 ns |  0.0428 ns |  1.00 |     175 B |
 *  | Overhead  | Vector128 | 8     |     4.182 ns |  0.0820 ns |  1.02 |     175 B |
 *  | Overhead  | Vector256 | 8     |     4.213 ns |  0.0860 ns |  1.02 |     175 B |
 *  | Overhead  | Vector512 | 8     |     4.115 ns |  0.0879 ns |  1.00 |     175 B |
 *  | FirstPick | Scalar    | 8     |    13.100 ns |  0.0699 ns |  1.00 |     262 B |
 *  | FirstPick | Vector128 | 8     |     8.073 ns |  0.1976 ns |  0.62 |     300 B |
 *  | FirstPick | Vector256 | 8     |     4.674 ns |  0.0806 ns |  0.36 |     323 B |
 *  | FirstPick | Vector512 | 8     |     5.406 ns |  0.1168 ns |  0.41 |     344 B |
 *  | PickAll   | Scalar    | 8     |    42.190 ns |  1.2595 ns |  1.00 |     477 B |
 *  | PickAll   | Vector128 | 8     |    61.877 ns |  1.4244 ns |  1.47 |     621 B |
 *  | PickAll   | Vector256 | 8     |    70.265 ns |  1.3762 ns |  1.67 |     609 B |
 *  | PickAll   | Vector512 | 8     |    94.119 ns |  0.6486 ns |  2.23 |     656 B |
 *  | Overhead  | Scalar    | 16    |     6.295 ns |  0.0821 ns |  1.00 |     175 B |
 *  | Overhead  | Vector128 | 16    |     6.405 ns |  0.1014 ns |  1.02 |     175 B |
 *  | Overhead  | Vector256 | 16    |     6.381 ns |  0.1421 ns |  1.01 |     175 B |
 *  | Overhead  | Vector512 | 16    |     6.341 ns |  0.0995 ns |  1.01 |     175 B |
 *  | FirstPick | Scalar    | 16    |    21.222 ns |  0.5964 ns |  1.00 |     262 B |
 *  | FirstPick | Vector128 | 16    |    13.568 ns |  0.2026 ns |  0.64 |     314 B |
 *  | FirstPick | Vector256 | 16    |    11.484 ns |  0.2187 ns |  0.54 |     320 B |
 *  | FirstPick | Vector512 | 16    |     7.271 ns |  0.1584 ns |  0.34 |     344 B |
 *  | PickAll   | Scalar    | 16    |   186.009 ns |  3.0836 ns |  1.00 |     477 B |
 *  | PickAll   | Vector128 | 16    |   160.800 ns |  1.0548 ns |  0.86 |     597 B |
 *  | PickAll   | Vector256 | 16    |   163.268 ns |  1.1038 ns |  0.88 |     649 B |
 *  | PickAll   | Vector512 | 16    |   208.470 ns |  3.9417 ns |  1.12 |     664 B |
 *  | Overhead  | Scalar    | 32    |    10.506 ns |  0.2833 ns |  1.00 |     175 B |
 *  | Overhead  | Vector128 | 32    |    10.548 ns |  0.2114 ns |  1.00 |     175 B |
 *  | Overhead  | Vector256 | 32    |    10.542 ns |  0.2214 ns |  1.00 |     175 B |
 *  | Overhead  | Vector512 | 32    |    10.545 ns |  0.2087 ns |  1.00 |     175 B |
 *  | FirstPick | Scalar    | 32    |    33.357 ns |  0.2953 ns |  1.00 |     262 B |
 *  | FirstPick | Vector128 | 32    |    20.304 ns |  0.1206 ns |  0.61 |     325 B |
 *  | FirstPick | Vector256 | 32    |    19.062 ns |  0.3630 ns |  0.57 |     334 B |
 *  | FirstPick | Vector512 | 32    |    19.415 ns |  0.0558 ns |  0.58 |     332 B |
 *  | PickAll   | Scalar    | 32    |   647.061 ns |  2.4843 ns |  1.00 |     477 B |
 *  | PickAll   | Vector128 | 32    |   473.195 ns | 13.5992 ns |  0.73 |     596 B |
 *  | PickAll   | Vector256 | 32    |   422.075 ns |  2.2202 ns |  0.65 |     635 B |
 *  | PickAll   | Vector512 | 32    |   483.687 ns |  3.9281 ns |  0.75 |     665 B |
 *  | Overhead  | Scalar    | 48    |    14.489 ns |  0.3544 ns |  1.00 |     175 B |
 *  | Overhead  | Vector128 | 48    |    14.478 ns |  0.3087 ns |  1.00 |     175 B |
 *  | Overhead  | Vector256 | 48    |    14.979 ns |  0.2139 ns |  1.03 |     175 B |
 *  | Overhead  | Vector512 | 48    |    14.343 ns |  0.1281 ns |  0.99 |     175 B |
 *  | FirstPick | Scalar    | 48    |    41.659 ns |  0.2538 ns |  1.00 |     262 B |
 *  | FirstPick | Vector128 | 48    |    27.969 ns |  0.6583 ns |  0.67 |     325 B |
 *  | FirstPick | Vector256 | 48    |    25.214 ns |  0.5595 ns |  0.61 |     334 B |
 *  | FirstPick | Vector512 | 48    |    25.565 ns |  0.3129 ns |  0.61 |     332 B |
 *  | PickAll   | Scalar    | 48    | 1,251.802 ns | 34.5142 ns |  1.00 |     477 B |
 *  | PickAll   | Vector128 | 48    |   731.771 ns | 17.8381 ns |  0.58 |     596 B |
 *  | PickAll   | Vector256 | 48    |   753.099 ns |  5.8170 ns |  0.60 |     638 B |
 *  | PickAll   | Vector512 | 48    |   798.989 ns | 10.1655 ns |  0.64 |     654 B |
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
                if (IndexOfFirstMax(list, start, Count) != IndexOfFirstMaxScalar(list, start, Count))
                {
                    throw new InvalidOperationException($"{Version} and scalar versions differ for list at offset {offset}, start {start}");
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
