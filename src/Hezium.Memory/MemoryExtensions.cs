using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hezium.Memory;

/// <summary>
/// Provides extension methods for <see cref="BigArray{T}"/>, <see cref="BigSpan{T}"/>, <see cref="BigReadOnlySpan{T}"/>, and <see cref="MemoryMarshal"/>.
/// </summary>
public static class MemoryExtensions
{
    extension(MemoryMarshal)
    {
        /// <summary>
        /// Creates a <see cref="BigSpan{T}"/> from a reference to the first element and a specified length.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="BigSpan{T}"/>.</typeparam>
        /// <param name="first">A reference to the first element of the span.</param>
        /// <param name="length">The number of elements in the span.</param>
        /// <returns>A <see cref="BigSpan{T}"/> that represents the specified range of elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is negative.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BigSpan<T> CreateBigSpan<T>(ref T first, nint length)
        {
            if (length < 0) ThrowHelpers.ThrowOutOfRange(nameof(length));
            return new BigSpan<T>(ref first, length);
        }

        /// <summary>
        /// Gets a reference to the first element of a <see cref="BigSpan{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="BigSpan{T}"/>.</typeparam>
        /// <param name="span">The <see cref="BigSpan{T}"/> to get the reference from.</param>
        /// <returns>A reference to the first element of the <see cref="BigSpan{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetReference<T>(BigSpan<T> span)
        {
            return ref span._first;
        }

        /// <summary>
        /// Gets a read-only reference to the first element of a <see cref="BigReadOnlySpan{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="BigReadOnlySpan{T}"/>.</typeparam>
        /// <param name="span">The <see cref="BigReadOnlySpan{T}"/> to get the reference from.</param>
        /// <returns>A read-only reference to the first element of the <see cref="BigReadOnlySpan{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref readonly T GetReference<T>(BigReadOnlySpan<T> span)
        {
            return ref span._first;
        }

        /// <summary>
        /// Returns a reference to the 0th element of <see cref="BigArray{T}"/>. If the array is empty, returns a reference to where the 0th element would have been stored. Such a reference may be used for pinning but must never be dereferenced.
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="BigArray{T}"/>.</typeparam>
        /// <param name="array">The <see cref="BigArray{T}"/> to get the reference from.</param>
        /// <returns>A reference to the first element of the <see cref="BigArray{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetBigArrayDataReference<T>(BigArray<T> array)
        {
            return ref Unsafe.As<byte, T>(ref MemoryMarshal.GetArrayDataReference(array._storage));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BigReadOnlySpan<T> AsReadOnlySpan<T>(BigSpan<T> span)
    {
        return new BigReadOnlySpan<T>(ref span._first, span._length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Span<T> CreateSpan<T>(BigSpan<T> span, int length)
    {
        return MemoryMarshal.CreateSpan(ref span._first, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReadOnlySpan<T> CreateReadOnlySpan<T>(BigReadOnlySpan<T> span, int length)
    {
        return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in span._first), length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetChunkLength(nint remaining)
    {
        return remaining > Settings.ArrayMaxLength ? Settings.ArrayMaxLength : (int)remaining;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BigSpan<T> SliceUnchecked<T>(BigSpan<T> span, nint start, nint length)
    {
        return new BigSpan<T>(ref Unsafe.Add(ref span._first, start), length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BigReadOnlySpan<T> SliceUnchecked<T>(BigReadOnlySpan<T> span, nint start, nint length)
    {
        return new BigReadOnlySpan<T>(ref Unsafe.Add(ref Unsafe.AsRef(in span._first), start), length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BigSpan<T> SliceUnchecked<T>(BigSpan<T> span, nint start)
    {
        return SliceUnchecked(span, start, span._length - start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BigReadOnlySpan<T> SliceUnchecked<T>(BigReadOnlySpan<T> span, nint start)
    {
        return SliceUnchecked(span, start, span._length - start);
    }

    private static bool Overlaps<T>(BigReadOnlySpan<T> source, BigSpan<T> destination, out nint elementOffset)
    {
        ref T sourceReference = ref Unsafe.AsRef(in source._first);
        nint byteOffset = Unsafe.ByteOffset(ref sourceReference, ref destination._first);
        nint elementSize = Unsafe.SizeOf<T>();
        if (byteOffset % elementSize != 0)
        {
            elementOffset = 0;
            return false;
        }

        elementOffset = byteOffset / elementSize;
        return elementOffset < source._length && elementOffset > -destination._length;
    }

    private static void CopyToCore<T>(BigReadOnlySpan<T> source, BigSpan<T> destination)
    {
        if (source._length == 0) return;

        if (Overlaps(source, destination, out nint elementOffset) && elementOffset > 0)
        {
            nint remaining = source._length;
            while (remaining > 0)
            {
                int chunkLength = GetChunkLength(remaining);
                nint chunkStart = remaining - chunkLength;
                CreateReadOnlySpan(SliceUnchecked(source, chunkStart, chunkLength), chunkLength).CopyTo(CreateSpan(SliceUnchecked(destination, chunkStart, chunkLength), chunkLength));
                remaining = chunkStart;
            }

            return;
        }

        while (source._length > 0)
        {
            int chunkLength = GetChunkLength(source._length);
            CreateReadOnlySpan(source, chunkLength).CopyTo(CreateSpan(destination, chunkLength));
            source = SliceUnchecked(source, chunkLength);
            destination = SliceUnchecked(destination, chunkLength);
        }
    }

    private static void CopyToCore<T>(BigReadOnlySpan<T> source, Span<T> destination)
    {
        if (source._length == 0) return;
        CreateReadOnlySpan(source, (int)source._length).CopyTo(destination);
    }

    private static T[] ToArrayCore<T>(BigReadOnlySpan<T> span)
    {
        if ((nuint)span._length > (nuint)Array.MaxLength) ThrowHelpers.ThrowOutOfRange(nameof(span));
        T[] result = new T[(int)span._length];
        CopyToCore(span, result);
        return result;
    }

    private static BigArray<T> ToBigArrayCore<T>(BigReadOnlySpan<T> span)
    {
        BigArray<T> result = new(span._length);
        CopyToCore(span, result.AsBigSpan());
        return result;
    }

    private static bool SequenceEqualCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> other, IEqualityComparer<T>? comparer)
    {
        if (span._length != other._length) return false;

        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            if (!CreateReadOnlySpan(span, chunkLength).SequenceEqual(CreateReadOnlySpan(other, chunkLength), comparer))
            {
                return false;
            }

            span = SliceUnchecked(span, chunkLength);
            other = SliceUnchecked(other, chunkLength);
        }

        return true;
    }

    private static int SequenceCompareToCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> other, IComparer<T>? comparer)
    {
        nint remaining = Math.Min(span._length, other._length);
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            int result = CreateReadOnlySpan(span, chunkLength).SequenceCompareTo(CreateReadOnlySpan(other, chunkLength), comparer);
            if (result != 0) return result;

            remaining -= chunkLength;
            span = SliceUnchecked(span, chunkLength);
            other = SliceUnchecked(other, chunkLength);
        }

        return span._length.CompareTo(other._length);
    }

    private static nint IndexOfCore<T>(BigReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer)
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOf(value, comparer);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint LastIndexOfCore<T>(BigReadOnlySpan<T> span, T value, IEqualityComparer<T>? comparer)
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOf(value, comparer);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint BinarySearchCore<T, TComparable>(BigReadOnlySpan<T> span, TComparable comparable)
        where TComparable : IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(comparable);

        if (span._length <= int.MaxValue)
        {
            return CreateReadOnlySpan(span, (int)span._length).BinarySearch(comparable);
        }

        nint low = 0;
        nint high = span._length - 1;
        ref T reference = ref Unsafe.AsRef(in span._first);
        while (low <= high)
        {
            nint index = low + ((high - low) >> 1);
            int comparison = comparable.CompareTo(Unsafe.Add(ref reference, index));
            if (comparison == 0) return index;
            if (comparison > 0) low = index + 1;
            else high = index - 1;
        }

        return ~low;
    }

    private static nint BinarySearchCore<T, TComparer>(BigReadOnlySpan<T> span, T value, TComparer comparer)
        where TComparer : IComparer<T>
    {
        ArgumentNullException.ThrowIfNull(comparer);

        if (span._length <= int.MaxValue)
        {
            return CreateReadOnlySpan(span, (int)span._length).BinarySearch(value, comparer);
        }

        nint low = 0;
        nint high = span._length - 1;
        ref T reference = ref Unsafe.AsRef(in span._first);
        while (low <= high)
        {
            nint index = low + ((high - low) >> 1);
            int comparison = comparer.Compare(value, Unsafe.Add(ref reference, index));
            if (comparison == 0) return index;
            if (comparison > 0) low = index + 1;
            else high = index - 1;
        }

        return ~low;
    }

    private static nint IndexOfAnyCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> values)
    {
        if (values._length == 0) return -1;

        nint offset = 0;
        if (values._length <= int.MaxValue)
        {
            ReadOnlySpan<T> valueSpan = CreateReadOnlySpan(values, (int)values._length);
            while (span._length > 0)
            {
                int chunkLength = GetChunkLength(span._length);
                int index = CreateReadOnlySpan(span, chunkLength).IndexOfAny(valueSpan);
                if (index >= 0) return offset + index;

                offset += chunkLength;
                span = SliceUnchecked(span, chunkLength);
            }

            return -1;
        }

        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            ReadOnlySpan<T> chunk = CreateReadOnlySpan(span, chunkLength);
            BigReadOnlySpan<T> remainingValues = values;
            int bestIndex = -1;
            while (remainingValues._length > 0)
            {
                int valueChunkLength = GetChunkLength(remainingValues._length);
                int index = chunk.IndexOfAny(CreateReadOnlySpan(remainingValues, valueChunkLength));
                if (index == 0) return offset;
                if ((uint)index < (uint)bestIndex || bestIndex < 0) bestIndex = index;

                remainingValues = SliceUnchecked(remainingValues, valueChunkLength);
            }

            if (bestIndex >= 0) return offset + bestIndex;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint IndexOfAnyCore<T>(BigReadOnlySpan<T> span, T value0, T value1)
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAny(value0, value1);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint IndexOfAnyCore<T>(BigReadOnlySpan<T> span, T value0, T value1, T value2)
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAny(value0, value1, value2);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint IndexOfAnyCore<T>(BigReadOnlySpan<T> span, SearchValues<T> values)
        where T : IEquatable<T>
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAny(values);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint LastIndexOfAnyCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> values)
    {
        if (values._length == 0) return -1;

        nint remaining = span._length;
        if (values._length <= int.MaxValue)
        {
            ReadOnlySpan<T> valueSpan = CreateReadOnlySpan(values, (int)values._length);
            while (remaining > 0)
            {
                int chunkLength = GetChunkLength(remaining);
                nint chunkStart = remaining - chunkLength;
                int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAny(valueSpan);
                if (index >= 0) return chunkStart + index;

                remaining = chunkStart;
            }

            return -1;
        }

        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            ReadOnlySpan<T> chunk = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength);
            BigReadOnlySpan<T> remainingValues = values;
            int bestIndex = -1;
            while (remainingValues._length > 0)
            {
                int valueChunkLength = GetChunkLength(remainingValues._length);
                int index = chunk.LastIndexOfAny(CreateReadOnlySpan(remainingValues, valueChunkLength));
                if (index == chunkLength - 1) return chunkStart + index;
                if (index > bestIndex) bestIndex = index;

                remainingValues = SliceUnchecked(remainingValues, valueChunkLength);
            }

            if (bestIndex >= 0) return chunkStart + bestIndex;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint LastIndexOfAnyCore<T>(BigReadOnlySpan<T> span, SearchValues<T> values)
        where T : IEquatable<T>
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAny(values);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint LastIndexOfAnyCore<T>(BigReadOnlySpan<T> span, T value0, T value1)
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAny(value0, value1);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint LastIndexOfAnyCore<T>(BigReadOnlySpan<T> span, T value0, T value1, T value2)
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAny(value0, value1, value2);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint IndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, T value)
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAnyExcept(value);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint IndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> values)
    {
        nint offset = 0;
        if (values._length <= int.MaxValue)
        {
            ReadOnlySpan<T> valueSpan = CreateReadOnlySpan(values, (int)values._length);
            while (span._length > 0)
            {
                int chunkLength = GetChunkLength(span._length);
                int index = CreateReadOnlySpan(span, chunkLength).IndexOfAnyExcept(valueSpan);
                if (index >= 0) return offset + index;

                offset += chunkLength;
                span = SliceUnchecked(span, chunkLength);
            }

            return -1;
        }

        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            ReadOnlySpan<T> chunk = CreateReadOnlySpan(span, chunkLength);
            for (int i = 0; i < chunk.Length; i++)
            {
                if (IndexOfCore(values, chunk[i], null) < 0) return offset + i;
            }

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint IndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, T value0, T value1)
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAnyExcept(value0, value1);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint IndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, T value0, T value1, T value2)
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAnyExcept(value0, value1, value2);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint IndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, SearchValues<T> values)
        where T : IEquatable<T>
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAnyExcept(values);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint LastIndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, T value)
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAnyExcept(value);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint LastIndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> values)
    {
        nint remaining = span._length;
        if (values._length <= int.MaxValue)
        {
            ReadOnlySpan<T> valueSpan = CreateReadOnlySpan(values, (int)values._length);
            while (remaining > 0)
            {
                int chunkLength = GetChunkLength(remaining);
                nint chunkStart = remaining - chunkLength;
                int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAnyExcept(valueSpan);
                if (index >= 0) return chunkStart + index;

                remaining = chunkStart;
            }

            return -1;
        }

        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            ReadOnlySpan<T> chunk = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength);
            for (int i = chunk.Length - 1; i >= 0; i--)
            {
                if (IndexOfCore(values, chunk[i], null) < 0) return chunkStart + i;
            }

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint LastIndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, T value0, T value1)
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAnyExcept(value0, value1);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint LastIndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, T value0, T value1, T value2)
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAnyExcept(value0, value1, value2);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint LastIndexOfAnyExceptCore<T>(BigReadOnlySpan<T> span, SearchValues<T> values)
        where T : IEquatable<T>
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAnyExcept(values);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint IndexOfAnyInRangeCore<T>(BigReadOnlySpan<T> span, T lowInclusive, T highInclusive)
        where T : IComparable<T>
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAnyInRange(lowInclusive, highInclusive);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint LastIndexOfAnyInRangeCore<T>(BigReadOnlySpan<T> span, T lowInclusive, T highInclusive)
        where T : IComparable<T>
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAnyInRange(lowInclusive, highInclusive);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint IndexOfAnyExceptInRangeCore<T>(BigReadOnlySpan<T> span, T lowInclusive, T highInclusive)
        where T : IComparable<T>
    {
        nint offset = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAnyExceptInRange(lowInclusive, highInclusive);
            if (index >= 0) return offset + index;

            offset += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return -1;
    }

    private static nint LastIndexOfAnyExceptInRangeCore<T>(BigReadOnlySpan<T> span, T lowInclusive, T highInclusive)
        where T : IComparable<T>
    {
        nint remaining = span._length;
        while (remaining > 0)
        {
            int chunkLength = GetChunkLength(remaining);
            nint chunkStart = remaining - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAnyExceptInRange(lowInclusive, highInclusive);
            if (index >= 0) return chunkStart + index;

            remaining = chunkStart;
        }

        return -1;
    }

    private static nint CountTrimStartCore<T>(BigReadOnlySpan<T> span, T trimElement)
    {
        nint trimmed = 0;
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            int index = CreateReadOnlySpan(span, chunkLength).IndexOfAnyExcept(trimElement);
            if (index >= 0) return trimmed + index;

            trimmed += chunkLength;
            span = SliceUnchecked(span, chunkLength);
        }

        return trimmed;
    }

    private static nint CountTrimStartCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> trimElements)
    {
        nint trimmed = 0;
        if (trimElements._length <= int.MaxValue)
        {
            ReadOnlySpan<T> trimElementSpan = CreateReadOnlySpan(trimElements, (int)trimElements._length);
            while (span._length > 0)
            {
                int chunkLength = GetChunkLength(span._length);
                int index = CreateReadOnlySpan(span, chunkLength).IndexOfAnyExcept(trimElementSpan);
                if (index >= 0) return trimmed + index;

                trimmed += chunkLength;
                span = SliceUnchecked(span, chunkLength);
            }

            return trimmed;
        }

        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            ReadOnlySpan<T> chunk = CreateReadOnlySpan(span, chunkLength);
            for (int i = 0; i < chunk.Length; i++)
            {
                if (IndexOfCore(trimElements, chunk[i], null) < 0) return trimmed + i;
            }

            trimmed += chunkLength;

            span = SliceUnchecked(span, chunkLength);
        }

        return trimmed;
    }

    private static nint CountTrimEndCore<T>(BigReadOnlySpan<T> span, T trimElement)
    {
        nint trimmed = 0;
        nint remainingLength = span._length;
        while (remainingLength > 0)
        {
            int chunkLength = GetChunkLength(remainingLength);
            nint chunkStart = remainingLength - chunkLength;
            int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAnyExcept(trimElement);
            if (index >= 0) return trimmed + (chunkLength - 1 - index);

            trimmed += chunkLength;
            remainingLength = chunkStart;
        }

        return trimmed;
    }

    private static nint CountTrimEndCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> trimElements)
    {
        nint trimmed = 0;
        nint remainingLength = span._length;
        if (trimElements._length <= int.MaxValue)
        {
            ReadOnlySpan<T> trimElementSpan = CreateReadOnlySpan(trimElements, (int)trimElements._length);
            while (remainingLength > 0)
            {
                int chunkLength = GetChunkLength(remainingLength);
                nint chunkStart = remainingLength - chunkLength;
                int index = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength).LastIndexOfAnyExcept(trimElementSpan);
                if (index >= 0) return trimmed + (chunkLength - 1 - index);

                trimmed += chunkLength;
                remainingLength = chunkStart;
            }

            return trimmed;
        }

        while (remainingLength > 0)
        {
            int chunkLength = GetChunkLength(remainingLength);
            nint chunkStart = remainingLength - chunkLength;
            ReadOnlySpan<T> chunk = CreateReadOnlySpan(SliceUnchecked(span, chunkStart, chunkLength), chunkLength);
            for (int i = chunk.Length - 1; i >= 0; i--)
            {
                if (IndexOfCore(trimElements, chunk[i], null) < 0) return trimmed + (chunk.Length - 1 - i);
            }

            trimmed += chunkLength;

            remainingLength = chunkStart;
        }

        return trimmed;
    }

    private static BigReadOnlySpan<T> TrimStartCore<T>(BigReadOnlySpan<T> span, T trimElement)
    {
        return SliceUnchecked(span, CountTrimStartCore(span, trimElement));
    }

    private static BigReadOnlySpan<T> TrimStartCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> trimElements)
    {
        return SliceUnchecked(span, CountTrimStartCore(span, trimElements));
    }

    private static BigReadOnlySpan<T> TrimEndCore<T>(BigReadOnlySpan<T> span, T trimElement)
    {
        return SliceUnchecked(span, 0, span._length - CountTrimEndCore(span, trimElement));
    }

    private static BigReadOnlySpan<T> TrimEndCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> trimElements)
    {
        return SliceUnchecked(span, 0, span._length - CountTrimEndCore(span, trimElements));
    }

    private static BigReadOnlySpan<T> TrimCore<T>(BigReadOnlySpan<T> span, T trimElement)
    {
        nint start = CountTrimStartCore(span, trimElement);
        nint end = CountTrimEndCore(SliceUnchecked(span, start), trimElement);
        return SliceUnchecked(span, start, span._length - start - end);
    }

    private static BigReadOnlySpan<T> TrimCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> trimElements)
    {
        nint start = CountTrimStartCore(span, trimElements);
        nint end = CountTrimEndCore(SliceUnchecked(span, start), trimElements);
        return SliceUnchecked(span, start, span._length - start - end);
    }

    private static void SortCore<T>(BigSpan<T> span)
    {
        if (span._length <= 1) return;
        if (span._length <= int.MaxValue)
        {
            CreateSpan(span, (int)span._length).Sort();
            return;
        }

        SortChunks(span);
        SortMergedRuns(span, Comparer<T>.Default);
    }

    private static void SortCore<T>(BigSpan<T> span, IComparer<T>? comparer)
    {
        if (span._length <= 1) return;
        comparer ??= Comparer<T>.Default;

        if (span._length <= int.MaxValue)
        {
            CreateSpan(span, (int)span._length).Sort(comparer);
            return;
        }

        SortChunks(span, comparer);
        SortMergedRuns(span, comparer);
    }

    private static void SortCore<T>(BigSpan<T> span, Comparison<T> comparison)
    {
        if (span._length <= 1) return;
        if (span._length <= int.MaxValue)
        {
            CreateSpan(span, (int)span._length).Sort(comparison);
            return;
        }

        SortChunks(span, comparison);
        SortMergedRuns(span, comparison);
    }

    private static void SortChunks<T>(BigSpan<T> span)
    {
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            CreateSpan(span, chunkLength).Sort();
            span = SliceUnchecked(span, chunkLength);
        }
    }

    private static void SortChunks<T>(BigSpan<T> span, IComparer<T> comparer)
    {
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            CreateSpan(span, chunkLength).Sort(comparer);
            span = SliceUnchecked(span, chunkLength);
        }
    }

    private static void SortChunks<T>(BigSpan<T> span, Comparison<T> comparison)
    {
        while (span._length > 0)
        {
            int chunkLength = GetChunkLength(span._length);
            CreateSpan(span, chunkLength).Sort(comparison);
            span = SliceUnchecked(span, chunkLength);
        }
    }

    private static void SortMergedRuns<T>(BigSpan<T> span, IComparer<T> comparer)
    {
        BigArray<T> buffer = new(span._length);
        BigSpan<T> source = span;
        BigSpan<T> destination = buffer.AsBigSpan();
        bool sourceIsScratch = false;

        for (nint width = Settings.ArrayMaxLength; width < span._length;)
        {
            MergePass(source, destination, width, comparer);
            BigSpan<T> previousSource = source;
            source = destination;
            destination = previousSource;
            sourceIsScratch = !sourceIsScratch;

            if (width > span._length / 2) break;
            width *= 2;
        }

        if (sourceIsScratch)
        {
            CopyToCore(AsReadOnlySpan(source), span);
        }

        GC.KeepAlive(buffer);
    }

    private static void SortMergedRuns<T>(BigSpan<T> span, Comparison<T> comparison)
    {
        BigArray<T> buffer = new(span._length);
        BigSpan<T> source = span;
        BigSpan<T> destination = buffer.AsBigSpan();
        bool sourceIsScratch = false;

        for (nint width = Settings.ArrayMaxLength; width < span._length;)
        {
            MergePass(source, destination, width, comparison);
            BigSpan<T> previousSource = source;
            source = destination;
            destination = previousSource;
            sourceIsScratch = !sourceIsScratch;

            if (width > span._length / 2) break;
            width *= 2;
        }

        if (sourceIsScratch)
        {
            CopyToCore(AsReadOnlySpan(source), span);
        }

        GC.KeepAlive(buffer);
    }

    private static void MergePass<T>(BigSpan<T> source, BigSpan<T> destination, nint width, IComparer<T> comparer)
    {
        nint start = 0;
        while (start < source._length)
        {
            nint remaining = source._length - start;
            nint leftLength = remaining < width ? remaining : width;
            remaining -= leftLength;
            nint rightLength = remaining < width ? remaining : width;
            nint length = leftLength + rightLength;

            if (rightLength == 0)
            {
                CopyToCore(AsReadOnlySpan(SliceUnchecked(source, start, leftLength)), SliceUnchecked(destination, start, leftLength));
            }
            else
            {
                MergeRuns(
                    AsReadOnlySpan(SliceUnchecked(source, start, leftLength)),
                    AsReadOnlySpan(SliceUnchecked(source, start + leftLength, rightLength)),
                    SliceUnchecked(destination, start, length),
                    comparer);
            }

            start += length;
        }
    }

    private static void MergePass<T>(BigSpan<T> source, BigSpan<T> destination, nint width, Comparison<T> comparison)
    {
        nint start = 0;
        while (start < source._length)
        {
            nint remaining = source._length - start;
            nint leftLength = remaining < width ? remaining : width;
            remaining -= leftLength;
            nint rightLength = remaining < width ? remaining : width;
            nint length = leftLength + rightLength;

            if (rightLength == 0)
            {
                CopyToCore(AsReadOnlySpan(SliceUnchecked(source, start, leftLength)), SliceUnchecked(destination, start, leftLength));
            }
            else
            {
                MergeRuns(
                    AsReadOnlySpan(SliceUnchecked(source, start, leftLength)),
                    AsReadOnlySpan(SliceUnchecked(source, start + leftLength, rightLength)),
                    SliceUnchecked(destination, start, length),
                    comparison);
            }

            start += length;
        }
    }

    private static void MergeRuns<T>(BigReadOnlySpan<T> left, BigReadOnlySpan<T> right, BigSpan<T> destination, IComparer<T> comparer)
    {
        nint leftIndex = 0;
        nint rightIndex = 0;
        nint destinationIndex = 0;
        ref T leftReference = ref Unsafe.AsRef(in left._first);
        ref T rightReference = ref Unsafe.AsRef(in right._first);
        ref T destinationReference = ref destination._first;

        while (leftIndex < left._length && rightIndex < right._length)
        {
            ref T leftValue = ref Unsafe.Add(ref leftReference, leftIndex);
            ref T rightValue = ref Unsafe.Add(ref rightReference, rightIndex);
            if (comparer.Compare(leftValue, rightValue) <= 0)
            {
                Unsafe.Add(ref destinationReference, destinationIndex) = leftValue;
                leftIndex++;
            }
            else
            {
                Unsafe.Add(ref destinationReference, destinationIndex) = rightValue;
                rightIndex++;
            }

            destinationIndex++;
        }

        if (leftIndex < left._length)
        {
            CopyToCore(SliceUnchecked(left, leftIndex), SliceUnchecked(destination, destinationIndex, left._length - leftIndex));
        }
        else if (rightIndex < right._length)
        {
            CopyToCore(SliceUnchecked(right, rightIndex), SliceUnchecked(destination, destinationIndex, right._length - rightIndex));
        }
    }

    private static void MergeRuns<T>(BigReadOnlySpan<T> left, BigReadOnlySpan<T> right, BigSpan<T> destination, Comparison<T> comparison)
    {
        nint leftIndex = 0;
        nint rightIndex = 0;
        nint destinationIndex = 0;
        ref T leftReference = ref Unsafe.AsRef(in left._first);
        ref T rightReference = ref Unsafe.AsRef(in right._first);
        ref T destinationReference = ref destination._first;

        while (leftIndex < left._length && rightIndex < right._length)
        {
            ref T leftValue = ref Unsafe.Add(ref leftReference, leftIndex);
            ref T rightValue = ref Unsafe.Add(ref rightReference, rightIndex);
            if (comparison(leftValue, rightValue) <= 0)
            {
                Unsafe.Add(ref destinationReference, destinationIndex) = leftValue;
                leftIndex++;
            }
            else
            {
                Unsafe.Add(ref destinationReference, destinationIndex) = rightValue;
                rightIndex++;
            }

            destinationIndex++;
        }

        if (leftIndex < left._length)
        {
            CopyToCore(SliceUnchecked(left, leftIndex), SliceUnchecked(destination, destinationIndex, left._length - leftIndex));
        }
        else if (rightIndex < right._length)
        {
            CopyToCore(SliceUnchecked(right, rightIndex), SliceUnchecked(destination, destinationIndex, right._length - rightIndex));
        }
    }

    private static bool StartsWithCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> value, IEqualityComparer<T>? comparer)
    {
        return value._length <= span._length && SequenceEqualCore(SliceUnchecked(span, 0, value._length), value, comparer);
    }

    private static bool EndsWithCore<T>(BigReadOnlySpan<T> span, BigReadOnlySpan<T> value, IEqualityComparer<T>? comparer)
    {
        return value._length <= span._length && SequenceEqualCore(SliceUnchecked(span, span._length - value._length, value._length), value, comparer);
    }

    extension<T>(BigArray<T> array)
    {
        /// <summary>
        /// Sets all elements in the array to the default value of <typeparamref name="T"/>.
        /// </summary>
        public void Clear()
        {
            array.AsBigSpan().Clear();
        }

        /// <summary>
        /// Fills the array with the specified value.
        /// </summary>
        /// <param name="value">The value to assign to each element.</param>
        public void Fill(T value)
        {
            array.AsBigSpan().Fill(value);
        }

        /// <summary>
        /// Copies the array to a destination <see cref="BigArray{T}"/>.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is too small.</exception>
        public void CopyTo(BigArray<T> destination)
        {
            array.AsBigSpan().CopyTo(destination.AsBigSpan());
        }

        /// <summary>
        /// Copies the array to a destination <see cref="BigArray{T}"/> starting at the specified destination index.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="destinationIndex">The zero-based destination index at which copying begins.</param>
        /// <exception cref="ArgumentException">Thrown when the destination range is too small.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destinationIndex"/> is outside the bounds of <paramref name="destination"/>.</exception>
        public void CopyTo(BigArray<T> destination, nint destinationIndex)
        {
            array.AsBigSpan().CopyTo(destination.AsBigSpan(destinationIndex));
        }

        /// <summary>
        /// Attempts to copy the array to a destination <see cref="BigArray{T}"/>.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
        public bool TryCopyTo(BigArray<T> destination)
        {
            return array.AsBigSpan().TryCopyTo(destination.AsBigSpan());
        }

        /// <summary>
        /// Copies the array to a new single-dimensional array.
        /// </summary>
        /// <returns>A new array containing the copied elements.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the array is too large to fit in a single managed array.</exception>
        public T[] ToArray()
        {
            return array.AsBigSpan().ToArray();
        }

        /// <summary>
        /// Searches for the specified value and returns the index of its first occurrence.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>The index of the first occurrence of <paramref name="value"/>, or -1 if it is not found.</returns>
        public nint IndexOf(T value, IEqualityComparer<T>? comparer = null)
        {
            return array.AsBigSpan().IndexOf(value, comparer);
        }

        /// <summary>
        /// Searches for the specified value and returns the index of its last occurrence.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>The index of the last occurrence of <paramref name="value"/>, or -1 if it is not found.</returns>
        public nint LastIndexOf(T value, IEqualityComparer<T>? comparer = null)
        {
            return array.AsBigSpan().LastIndexOf(value, comparer);
        }

        /// <summary>
        /// Determines whether the array contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is found; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T value, IEqualityComparer<T>? comparer = null)
        {
            return array.AsBigSpan().Contains(value, comparer);
        }

        /// <summary>
        /// Searches the sorted array for a value using the specified comparable object.
        /// </summary>
        /// <typeparam name="TComparable">The type that compares the target value with array elements.</typeparam>
        /// <param name="comparable">The comparable object to use when searching.</param>
        /// <returns>The index of the matching value, or the bitwise complement of the insertion index if no match is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparable"/> is <see langword="null"/>.</exception>
        public nint BinarySearch<TComparable>(TComparable comparable)
            where TComparable : IComparable<T>
        {
            return array.AsBigSpan().BinarySearch(comparable);
        }

        /// <summary>
        /// Searches the sorted array for a value using the specified comparer.
        /// </summary>
        /// <typeparam name="TComparer">The type of comparer to use.</typeparam>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use when searching.</param>
        /// <returns>The index of <paramref name="value"/>, or the bitwise complement of the insertion index if no match is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparer"/> is <see langword="null"/>.</exception>
        public nint BinarySearch<TComparer>(T value, TComparer comparer)
            where TComparer : IComparer<T>
        {
            return array.AsBigSpan().BinarySearch(value, comparer);
        }
    }

    extension<T>(BigSpan<T> span)
    {
        /// <summary>
        /// Sets all elements in the span to the default value of <typeparamref name="T"/>.
        /// </summary>
        public void Clear()
        {
            nint remaining = span._length;
            while (remaining > 0)
            {
                int chunkLength = GetChunkLength(remaining);
                CreateSpan(span, chunkLength).Clear();
                remaining -= chunkLength;
                span = SliceUnchecked(span, chunkLength);
            }
        }

        /// <summary>
        /// Fills the span with the specified value.
        /// </summary>
        /// <param name="value">The value to assign to each element.</param>
        public void Fill(T value)
        {
            nint remaining = span._length;
            while (remaining > 0)
            {
                int chunkLength = GetChunkLength(remaining);
                CreateSpan(span, chunkLength).Fill(value);
                remaining -= chunkLength;
                span = SliceUnchecked(span, chunkLength);
            }
        }

        /// <summary>
        /// Copies the elements of the current span to a destination <see cref="BigSpan{T}"/>.
        /// </summary>
        /// <param name="destination">The destination span.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is too small.</exception>
        public void CopyTo(BigSpan<T> destination)
        {
            if ((nuint)span._length > (nuint)destination._length) ThrowHelpers.ThrowOutOfRange(nameof(destination));
            CopyToCore(AsReadOnlySpan(span), destination);
        }

        /// <summary>
        /// Copies the elements of the current span to a destination <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">The destination span.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is too small.</exception>
        public void CopyTo(Span<T> destination)
        {
            if ((nuint)span._length > (nuint)destination.Length) ThrowHelpers.ThrowOutOfRange(nameof(destination));
            CopyToCore(AsReadOnlySpan(span), destination);
        }

        /// <summary>
        /// Attempts to copy the current span to a destination <see cref="BigSpan{T}"/>.
        /// </summary>
        /// <param name="destination">The destination span.</param>
        /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
        public bool TryCopyTo(BigSpan<T> destination)
        {
            if ((nuint)span._length > (nuint)destination._length) return false;
            CopyToCore(AsReadOnlySpan(span), destination);
            return true;
        }

        /// <summary>
        /// Attempts to copy the current span to a destination <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">The destination span.</param>
        /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
        public bool TryCopyTo(Span<T> destination)
        {
            if ((nuint)span._length > (nuint)destination.Length) return false;
            CopyToCore(AsReadOnlySpan(span), destination);
            return true;
        }

        /// <summary>
        /// Copies the contents of the span into a new single-dimensional array.
        /// </summary>
        /// <returns>A new array containing the copied elements.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the span is too large to fit in a single managed array.</exception>
        public T[] ToArray()
        {
            return ToArrayCore(AsReadOnlySpan(span));
        }

        /// <summary>
        /// Copies the contents of the span into a new <see cref="BigArray{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="BigArray{T}"/> containing the copied elements.</returns>
        public BigArray<T> ToBigArray()
        {
            return ToBigArrayCore(AsReadOnlySpan(span));
        }

        /// <summary>
        /// Creates a <see cref="Span{T}"/> over a range of the current span.
        /// </summary>
        /// <param name="start">The zero-based index at which the span starts.</param>
        /// <param name="length">The number of elements in the span.</param>
        /// <returns>A <see cref="Span{T}"/> over the specified range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of the span.</exception>
        public Span<T> ToSpan(nint start, int length)
        {
            if ((nuint)start > (nuint)span._length || (nuint)length > (nuint)(span._length - start)) ThrowHelpers.ThrowOutOfRange();

            return CreateSpan(SliceUnchecked(span, start, length), length);
        }

        /// <summary>
        /// Splits the current span using the specified separator.
        /// </summary>
        /// <param name="separator">The separator to split on.</param>
        /// <returns>An enumerator over the split segments.</returns>
        public BigSpanSplitEnumerator<T> Split(T separator)
        {
            return new BigSpanSplitEnumerator<T>(AsReadOnlySpan(span), separator);
        }

        /// <summary>
        /// Splits the current span using any of the specified separators.
        /// </summary>
        /// <param name="separators">The separators to split on.</param>
        /// <returns>An enumerator over the split segments.</returns>
        public BigSpanSplitEnumerator<T> SplitAny(BigReadOnlySpan<T> separators)
        {
            return new BigSpanSplitEnumerator<T>(AsReadOnlySpan(span), separators);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of the specified element from the current span.
        /// </summary>
        /// <param name="trimElement">The element to remove.</param>
        /// <returns>A span with leading and trailing occurrences of <paramref name="trimElement"/> removed.</returns>
        public BigSpan<T> Trim(T trimElement)
        {
            BigReadOnlySpan<T> readOnlySpan = AsReadOnlySpan(span);
            nint start = CountTrimStartCore(readOnlySpan, trimElement);
            nint end = CountTrimEndCore(SliceUnchecked(readOnlySpan, start), trimElement);
            return SliceUnchecked(span, start, span._length - start - end);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of the specified elements from the current span.
        /// </summary>
        /// <param name="trimElements">The elements to remove.</param>
        /// <returns>A span with leading and trailing occurrences of any element in <paramref name="trimElements"/> removed.</returns>
        public BigSpan<T> Trim(BigReadOnlySpan<T> trimElements)
        {
            BigReadOnlySpan<T> readOnlySpan = AsReadOnlySpan(span);
            nint start = CountTrimStartCore(readOnlySpan, trimElements);
            nint end = CountTrimEndCore(SliceUnchecked(readOnlySpan, start), trimElements);
            return SliceUnchecked(span, start, span._length - start - end);
        }

        /// <summary>
        /// Removes all leading occurrences of the specified element from the current span.
        /// </summary>
        /// <param name="trimElement">The element to remove.</param>
        /// <returns>A span with leading occurrences of <paramref name="trimElement"/> removed.</returns>
        public BigSpan<T> TrimStart(T trimElement)
        {
            return SliceUnchecked(span, CountTrimStartCore(AsReadOnlySpan(span), trimElement));
        }

        /// <summary>
        /// Removes all leading occurrences of the specified elements from the current span.
        /// </summary>
        /// <param name="trimElements">The elements to remove.</param>
        /// <returns>A span with leading occurrences of any element in <paramref name="trimElements"/> removed.</returns>
        public BigSpan<T> TrimStart(BigReadOnlySpan<T> trimElements)
        {
            return SliceUnchecked(span, CountTrimStartCore(AsReadOnlySpan(span), trimElements));
        }

        /// <summary>
        /// Removes all trailing occurrences of the specified element from the current span.
        /// </summary>
        /// <param name="trimElement">The element to remove.</param>
        /// <returns>A span with trailing occurrences of <paramref name="trimElement"/> removed.</returns>
        public BigSpan<T> TrimEnd(T trimElement)
        {
            return SliceUnchecked(span, 0, span._length - CountTrimEndCore(AsReadOnlySpan(span), trimElement));
        }

        /// <summary>
        /// Removes all trailing occurrences of the specified elements from the current span.
        /// </summary>
        /// <param name="trimElements">The elements to remove.</param>
        /// <returns>A span with trailing occurrences of any element in <paramref name="trimElements"/> removed.</returns>
        public BigSpan<T> TrimEnd(BigReadOnlySpan<T> trimElements)
        {
            return SliceUnchecked(span, 0, span._length - CountTrimEndCore(AsReadOnlySpan(span), trimElements));
        }

        /// <summary>
        /// Sorts the elements in the current span.
        /// </summary>
        public void Sort()
        {
            SortCore(span);
        }

        /// <summary>
        /// Sorts the elements in the current span using the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default comparer.</param>
        public void Sort(IComparer<T>? comparer)
        {
            SortCore(span, comparer);
        }

        /// <summary>
        /// Sorts the elements in the current span using the specified comparison.
        /// </summary>
        /// <param name="comparison">The comparison to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparison"/> is <see langword="null"/>.</exception>
        public void Sort(Comparison<T> comparison)
        {
            ArgumentNullException.ThrowIfNull(comparison);
            SortCore(span, comparison);
        }

        /// <summary>
        /// Searches for the specified value and returns the index of its first occurrence.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>The index of the first occurrence of <paramref name="value"/>, or -1 if it is not found.</returns>
        public nint IndexOf(T value, IEqualityComparer<T>? comparer = null)
        {
            return IndexOfCore(AsReadOnlySpan(span), value, comparer);
        }

        /// <summary>
        /// Searches for the specified value and returns the index of its last occurrence.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>The index of the last occurrence of <paramref name="value"/>, or -1 if it is not found.</returns>
        public nint LastIndexOf(T value, IEqualityComparer<T>? comparer = null)
        {
            return LastIndexOfCore(AsReadOnlySpan(span), value, comparer);
        }

        /// <summary>
        /// Determines whether the span contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is found; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T value, IEqualityComparer<T>? comparer = null)
        {
            return IndexOfCore(AsReadOnlySpan(span), value, comparer) >= 0;
        }

        /// <summary>
        /// Searches the sorted span for a value using the specified comparable object.
        /// </summary>
        /// <typeparam name="TComparable">The type that compares the target value with span elements.</typeparam>
        /// <param name="comparable">The comparable object to use when searching.</param>
        /// <returns>The index of the matching value, or the bitwise complement of the insertion index if no match is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparable"/> is <see langword="null"/>.</exception>
        public nint BinarySearch<TComparable>(TComparable comparable)
            where TComparable : IComparable<T>
        {
            return BinarySearchCore(AsReadOnlySpan(span), comparable);
        }

        /// <summary>
        /// Searches the sorted span for a value using the specified comparer.
        /// </summary>
        /// <typeparam name="TComparer">The type of comparer to use.</typeparam>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use when searching.</param>
        /// <returns>The index of <paramref name="value"/>, or the bitwise complement of the insertion index if no match is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparer"/> is <see langword="null"/>.</exception>
        public nint BinarySearch<TComparer>(T value, TComparer comparer)
            where TComparer : IComparer<T>
        {
            return BinarySearchCore(AsReadOnlySpan(span), value, comparer);
        }

        /// <summary>
        /// Searches for the first occurrence of any of the specified values.
        /// </summary>
        /// <param name="values">The values to locate.</param>
        /// <returns>The index of the first occurrence of any value in <paramref name="values"/>, or -1 if none are found.</returns>
        public nint IndexOfAny(BigReadOnlySpan<T> values)
        {
            return IndexOfAnyCore(AsReadOnlySpan(span), values);
        }

        /// <summary>
        /// Searches for the first occurrence of either of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <returns>The index of the first occurrence of either value, or -1 if neither is found.</returns>
        public nint IndexOfAny(T value0, T value1)
        {
            return IndexOfAnyCore(AsReadOnlySpan(span), value0, value1);
        }

        /// <summary>
        /// Searches for the first occurrence of any of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <param name="value2">The third value to locate.</param>
        /// <returns>The index of the first occurrence of any value, or -1 if none are found.</returns>
        public nint IndexOfAny(T value0, T value1, T value2)
        {
            return IndexOfAnyCore(AsReadOnlySpan(span), value0, value1, value2);
        }

        /// <summary>
        /// Searches for the last occurrence of any of the specified values.
        /// </summary>
        /// <param name="values">The values to locate.</param>
        /// <returns>The index of the last occurrence of any value in <paramref name="values"/>, or -1 if none are found.</returns>
        public nint LastIndexOfAny(BigReadOnlySpan<T> values)
        {
            return LastIndexOfAnyCore(AsReadOnlySpan(span), values);
        }

        /// <summary>
        /// Searches for the last occurrence of either of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <returns>The index of the last occurrence of either value, or -1 if neither is found.</returns>
        public nint LastIndexOfAny(T value0, T value1)
        {
            return LastIndexOfAnyCore(AsReadOnlySpan(span), value0, value1);
        }

        /// <summary>
        /// Searches for the last occurrence of any of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <param name="value2">The third value to locate.</param>
        /// <returns>The index of the last occurrence of any value, or -1 if none are found.</returns>
        public nint LastIndexOfAny(T value0, T value1, T value2)
        {
            return LastIndexOfAnyCore(AsReadOnlySpan(span), value0, value1, value2);
        }

        /// <summary>
        /// Determines whether the span contains any of the specified values.
        /// </summary>
        /// <param name="values">The values to locate.</param>
        /// <returns><see langword="true"/> if any value in <paramref name="values"/> is found; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAny(BigReadOnlySpan<T> values)
        {
            return IndexOfAnyCore(AsReadOnlySpan(span), values) >= 0;
        }

        /// <summary>
        /// Determines whether the span contains either of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <returns><see langword="true"/> if either value is found; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAny(T value0, T value1)
        {
            return IndexOfAnyCore(AsReadOnlySpan(span), value0, value1) >= 0;
        }

        /// <summary>
        /// Determines whether the span contains any of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <param name="value2">The third value to locate.</param>
        /// <returns><see langword="true"/> if any value is found; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAny(T value0, T value1, T value2)
        {
            return IndexOfAnyCore(AsReadOnlySpan(span), value0, value1, value2) >= 0;
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified value.
        /// </summary>
        /// <param name="value">The value to exclude.</param>
        /// <returns>The index of the first value other than <paramref name="value"/>, or -1 if all values match.</returns>
        public nint IndexOfAnyExcept(T value)
        {
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), value);
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="values">The values to exclude.</param>
        /// <returns>The index of the first value not in <paramref name="values"/>, or -1 if all values are excluded.</returns>
        public nint IndexOfAnyExcept(BigReadOnlySpan<T> values)
        {
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), values);
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <returns>The index of the first value other than the specified values, or -1 if all values are excluded.</returns>
        public nint IndexOfAnyExcept(T value0, T value1)
        {
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), value0, value1);
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <param name="value2">The third value to exclude.</param>
        /// <returns>The index of the first value other than the specified values, or -1 if all values are excluded.</returns>
        public nint IndexOfAnyExcept(T value0, T value1, T value2)
        {
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), value0, value1, value2);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified value.
        /// </summary>
        /// <param name="value">The value to exclude.</param>
        /// <returns>The index of the last value other than <paramref name="value"/>, or -1 if all values match.</returns>
        public nint LastIndexOfAnyExcept(T value)
        {
            return LastIndexOfAnyExceptCore(AsReadOnlySpan(span), value);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="values">The values to exclude.</param>
        /// <returns>The index of the last value not in <paramref name="values"/>, or -1 if all values are excluded.</returns>
        public nint LastIndexOfAnyExcept(BigReadOnlySpan<T> values)
        {
            return LastIndexOfAnyExceptCore(AsReadOnlySpan(span), values);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <returns>The index of the last value other than the specified values, or -1 if all values are excluded.</returns>
        public nint LastIndexOfAnyExcept(T value0, T value1)
        {
            return LastIndexOfAnyExceptCore(AsReadOnlySpan(span), value0, value1);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <param name="value2">The third value to exclude.</param>
        /// <returns>The index of the last value other than the specified values, or -1 if all values are excluded.</returns>
        public nint LastIndexOfAnyExcept(T value0, T value1, T value2)
        {
            return LastIndexOfAnyExceptCore(AsReadOnlySpan(span), value0, value1, value2);
        }

        /// <summary>
        /// Determines whether the span contains any value other than the specified value.
        /// </summary>
        /// <param name="value">The value to exclude.</param>
        /// <returns><see langword="true"/> if any value differs from <paramref name="value"/>; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExcept(T value)
        {
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), value) >= 0;
        }

        /// <summary>
        /// Determines whether the span contains any value other than the specified values.
        /// </summary>
        /// <param name="values">The values to exclude.</param>
        /// <returns><see langword="true"/> if any value is not in <paramref name="values"/>; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExcept(BigReadOnlySpan<T> values)
        {
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), values) >= 0;
        }

        /// <summary>
        /// Determines whether the span contains any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <returns><see langword="true"/> if any value differs from the specified values; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExcept(T value0, T value1)
        {
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), value0, value1) >= 0;
        }

        /// <summary>
        /// Determines whether the span contains any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <param name="value2">The third value to exclude.</param>
        /// <returns><see langword="true"/> if any value differs from the specified values; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExcept(T value0, T value1, T value2)
        {
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), value0, value1, value2) >= 0;
        }

        /// <summary>
        /// Determines whether the current span and another span contain the same elements.
        /// </summary>
        /// <param name="other">The span to compare with the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if the spans contain the same elements; otherwise, <see langword="false"/>.</returns>
        public bool SequenceEqual(BigSpan<T> other, IEqualityComparer<T>? comparer = null)
        {
            return SequenceEqualCore(AsReadOnlySpan(span), AsReadOnlySpan(other), comparer);
        }

        /// <summary>
        /// Determines whether the current span and another read-only span contain the same elements.
        /// </summary>
        /// <param name="other">The span to compare with the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if the spans contain the same elements; otherwise, <see langword="false"/>.</returns>
        public bool SequenceEqual(BigReadOnlySpan<T> other, IEqualityComparer<T>? comparer = null)
        {
            return SequenceEqualCore(AsReadOnlySpan(span), other, comparer);
        }

        /// <summary>
        /// Compares the current span with another span lexicographically.
        /// </summary>
        /// <param name="other">The span to compare with the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default comparer.</param>
        /// <returns>A value that indicates the relative order of the spans.</returns>
        public int SequenceCompareTo(BigSpan<T> other, IComparer<T>? comparer = null)
        {
            return SequenceCompareToCore(AsReadOnlySpan(span), AsReadOnlySpan(other), comparer);
        }

        /// <summary>
        /// Compares the current span with another read-only span lexicographically.
        /// </summary>
        /// <param name="other">The span to compare with the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default comparer.</param>
        /// <returns>A value that indicates the relative order of the spans.</returns>
        public int SequenceCompareTo(BigReadOnlySpan<T> other, IComparer<T>? comparer = null)
        {
            return SequenceCompareToCore(AsReadOnlySpan(span), other, comparer);
        }

        /// <summary>
        /// Determines whether the beginning of the current span matches another span.
        /// </summary>
        /// <param name="value">The span to compare with the start of the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> matches the start of the current span; otherwise, <see langword="false"/>.</returns>
        public bool StartsWith(BigReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
        {
            return StartsWithCore(AsReadOnlySpan(span), value, comparer);
        }

        /// <summary>
        /// Determines whether the end of the current span matches another span.
        /// </summary>
        /// <param name="value">The span to compare with the end of the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> matches the end of the current span; otherwise, <see langword="false"/>.</returns>
        public bool EndsWith(BigReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
        {
            return EndsWithCore(AsReadOnlySpan(span), value, comparer);
        }
    }

    extension<T>(BigReadOnlySpan<T> span)
    {
        /// <summary>
        /// Copies the elements of the current read-only span to a destination <see cref="BigSpan{T}"/>.
        /// </summary>
        /// <param name="destination">The destination span.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is too small.</exception>
        public void CopyTo(BigSpan<T> destination)
        {
            if ((nuint)span._length > (nuint)destination._length) ThrowHelpers.ThrowOutOfRange(nameof(destination));
            CopyToCore(span, destination);
        }

        /// <summary>
        /// Copies the elements of the current read-only span to a destination <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">The destination span.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is too small.</exception>
        public void CopyTo(Span<T> destination)
        {
            if ((nuint)span._length > (nuint)destination.Length) ThrowHelpers.ThrowOutOfRange(nameof(destination));
            CopyToCore(span, destination);
        }

        /// <summary>
        /// Attempts to copy the current read-only span to a destination <see cref="BigSpan{T}"/>.
        /// </summary>
        /// <param name="destination">The destination span.</param>
        /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
        public bool TryCopyTo(BigSpan<T> destination)
        {
            if ((nuint)span._length > (nuint)destination._length) return false;
            CopyToCore(span, destination);
            return true;
        }

        /// <summary>
        /// Attempts to copy the current read-only span to a destination <see cref="Span{T}"/>.
        /// </summary>
        /// <param name="destination">The destination span.</param>
        /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
        public bool TryCopyTo(Span<T> destination)
        {
            if ((nuint)span._length > (nuint)destination.Length) return false;
            CopyToCore(span, destination);
            return true;
        }

        /// <summary>
        /// Copies the contents of the read-only span into a new single-dimensional array.
        /// </summary>
        /// <returns>A new array containing the copied elements.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the span is too large to fit in a single managed array.</exception>
        public T[] ToArray()
        {
            return ToArrayCore(span);
        }

        /// <summary>
        /// Copies the contents of the read-only span into a new <see cref="BigArray{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="BigArray{T}"/> containing the copied elements.</returns>
        public BigArray<T> ToBigArray()
        {
            return ToBigArrayCore(span);
        }

        /// <summary>
        /// Creates a <see cref="ReadOnlySpan{T}"/> over a range of the current read-only span.
        /// </summary>
        /// <param name="start">The zero-based index at which the span starts.</param>
        /// <param name="length">The number of elements in the span.</param>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> over the specified range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of the span.</exception>
        public ReadOnlySpan<T> ToSpan(nint start, int length)
        {
            if ((nuint)start > (nuint)span._length || (nuint)length > (nuint)(span._length - start)) ThrowHelpers.ThrowOutOfRange();

            return CreateReadOnlySpan(SliceUnchecked(span, start, length), length);
        }

        /// <summary>
        /// Splits the current read-only span using the specified separator.
        /// </summary>
        /// <param name="separator">The separator to split on.</param>
        /// <returns>An enumerator over the split segments.</returns>
        public BigSpanSplitEnumerator<T> Split(T separator)
        {
            return new BigSpanSplitEnumerator<T>(span, separator);
        }

        /// <summary>
        /// Splits the current read-only span using any of the specified separators.
        /// </summary>
        /// <param name="separators">The separators to split on.</param>
        /// <returns>An enumerator over the split segments.</returns>
        public BigSpanSplitEnumerator<T> SplitAny(BigReadOnlySpan<T> separators)
        {
            return new BigSpanSplitEnumerator<T>(span, separators);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of the specified element from the current read-only span.
        /// </summary>
        /// <param name="trimElement">The element to remove.</param>
        /// <returns>A read-only span with leading and trailing occurrences of <paramref name="trimElement"/> removed.</returns>
        public BigReadOnlySpan<T> Trim(T trimElement)
        {
            return TrimCore(span, trimElement);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of the specified elements from the current read-only span.
        /// </summary>
        /// <param name="trimElements">The elements to remove.</param>
        /// <returns>A read-only span with leading and trailing occurrences of any element in <paramref name="trimElements"/> removed.</returns>
        public BigReadOnlySpan<T> Trim(BigReadOnlySpan<T> trimElements)
        {
            return TrimCore(span, trimElements);
        }

        /// <summary>
        /// Removes all leading occurrences of the specified element from the current read-only span.
        /// </summary>
        /// <param name="trimElement">The element to remove.</param>
        /// <returns>A read-only span with leading occurrences of <paramref name="trimElement"/> removed.</returns>
        public BigReadOnlySpan<T> TrimStart(T trimElement)
        {
            return TrimStartCore(span, trimElement);
        }

        /// <summary>
        /// Removes all leading occurrences of the specified elements from the current read-only span.
        /// </summary>
        /// <param name="trimElements">The elements to remove.</param>
        /// <returns>A read-only span with leading occurrences of any element in <paramref name="trimElements"/> removed.</returns>
        public BigReadOnlySpan<T> TrimStart(BigReadOnlySpan<T> trimElements)
        {
            return TrimStartCore(span, trimElements);
        }

        /// <summary>
        /// Removes all trailing occurrences of the specified element from the current read-only span.
        /// </summary>
        /// <param name="trimElement">The element to remove.</param>
        /// <returns>A read-only span with trailing occurrences of <paramref name="trimElement"/> removed.</returns>
        public BigReadOnlySpan<T> TrimEnd(T trimElement)
        {
            return TrimEndCore(span, trimElement);
        }

        /// <summary>
        /// Removes all trailing occurrences of the specified elements from the current read-only span.
        /// </summary>
        /// <param name="trimElements">The elements to remove.</param>
        /// <returns>A read-only span with trailing occurrences of any element in <paramref name="trimElements"/> removed.</returns>
        public BigReadOnlySpan<T> TrimEnd(BigReadOnlySpan<T> trimElements)
        {
            return TrimEndCore(span, trimElements);
        }

        /// <summary>
        /// Searches for the specified value and returns the index of its first occurrence.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>The index of the first occurrence of <paramref name="value"/>, or -1 if it is not found.</returns>
        public nint IndexOf(T value, IEqualityComparer<T>? comparer = null)
        {
            return IndexOfCore(span, value, comparer);
        }

        /// <summary>
        /// Searches for the specified value and returns the index of its last occurrence.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns>The index of the last occurrence of <paramref name="value"/>, or -1 if it is not found.</returns>
        public nint LastIndexOf(T value, IEqualityComparer<T>? comparer = null)
        {
            return LastIndexOfCore(span, value, comparer);
        }

        /// <summary>
        /// Determines whether the read-only span contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is found; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T value, IEqualityComparer<T>? comparer = null)
        {
            return IndexOfCore(span, value, comparer) >= 0;
        }

        /// <summary>
        /// Searches the sorted read-only span for a value using the specified comparable object.
        /// </summary>
        /// <typeparam name="TComparable">The type that compares the target value with span elements.</typeparam>
        /// <param name="comparable">The comparable object to use when searching.</param>
        /// <returns>The index of the matching value, or the bitwise complement of the insertion index if no match is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparable"/> is <see langword="null"/>.</exception>
        public nint BinarySearch<TComparable>(TComparable comparable)
            where TComparable : IComparable<T>
        {
            return BinarySearchCore(span, comparable);
        }

        /// <summary>
        /// Searches the sorted read-only span for a value using the specified comparer.
        /// </summary>
        /// <typeparam name="TComparer">The type of comparer to use.</typeparam>
        /// <param name="value">The value to locate.</param>
        /// <param name="comparer">The comparer to use when searching.</param>
        /// <returns>The index of <paramref name="value"/>, or the bitwise complement of the insertion index if no match is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparer"/> is <see langword="null"/>.</exception>
        public nint BinarySearch<TComparer>(T value, TComparer comparer)
            where TComparer : IComparer<T>
        {
            return BinarySearchCore(span, value, comparer);
        }

        /// <summary>
        /// Searches for the first occurrence of any of the specified values.
        /// </summary>
        /// <param name="values">The values to locate.</param>
        /// <returns>The index of the first occurrence of any value in <paramref name="values"/>, or -1 if none are found.</returns>
        public nint IndexOfAny(BigReadOnlySpan<T> values)
        {
            return IndexOfAnyCore(span, values);
        }

        /// <summary>
        /// Searches for the first occurrence of either of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <returns>The index of the first occurrence of either value, or -1 if neither is found.</returns>
        public nint IndexOfAny(T value0, T value1)
        {
            return IndexOfAnyCore(span, value0, value1);
        }

        /// <summary>
        /// Searches for the first occurrence of any of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <param name="value2">The third value to locate.</param>
        /// <returns>The index of the first occurrence of any value, or -1 if none are found.</returns>
        public nint IndexOfAny(T value0, T value1, T value2)
        {
            return IndexOfAnyCore(span, value0, value1, value2);
        }

        /// <summary>
        /// Searches for the last occurrence of any of the specified values.
        /// </summary>
        /// <param name="values">The values to locate.</param>
        /// <returns>The index of the last occurrence of any value in <paramref name="values"/>, or -1 if none are found.</returns>
        public nint LastIndexOfAny(BigReadOnlySpan<T> values)
        {
            return LastIndexOfAnyCore(span, values);
        }

        /// <summary>
        /// Searches for the last occurrence of either of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <returns>The index of the last occurrence of either value, or -1 if neither is found.</returns>
        public nint LastIndexOfAny(T value0, T value1)
        {
            return LastIndexOfAnyCore(span, value0, value1);
        }

        /// <summary>
        /// Searches for the last occurrence of any of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <param name="value2">The third value to locate.</param>
        /// <returns>The index of the last occurrence of any value, or -1 if none are found.</returns>
        public nint LastIndexOfAny(T value0, T value1, T value2)
        {
            return LastIndexOfAnyCore(span, value0, value1, value2);
        }

        /// <summary>
        /// Determines whether the read-only span contains any of the specified values.
        /// </summary>
        /// <param name="values">The values to locate.</param>
        /// <returns><see langword="true"/> if any value in <paramref name="values"/> is found; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAny(BigReadOnlySpan<T> values)
        {
            return IndexOfAnyCore(span, values) >= 0;
        }

        /// <summary>
        /// Determines whether the read-only span contains either of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <returns><see langword="true"/> if either value is found; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAny(T value0, T value1)
        {
            return IndexOfAnyCore(span, value0, value1) >= 0;
        }

        /// <summary>
        /// Determines whether the read-only span contains any of the specified values.
        /// </summary>
        /// <param name="value0">The first value to locate.</param>
        /// <param name="value1">The second value to locate.</param>
        /// <param name="value2">The third value to locate.</param>
        /// <returns><see langword="true"/> if any value is found; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAny(T value0, T value1, T value2)
        {
            return IndexOfAnyCore(span, value0, value1, value2) >= 0;
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified value.
        /// </summary>
        /// <param name="value">The value to exclude.</param>
        /// <returns>The index of the first value other than <paramref name="value"/>, or -1 if all values match.</returns>
        public nint IndexOfAnyExcept(T value)
        {
            return IndexOfAnyExceptCore(span, value);
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="values">The values to exclude.</param>
        /// <returns>The index of the first value not in <paramref name="values"/>, or -1 if all values are excluded.</returns>
        public nint IndexOfAnyExcept(BigReadOnlySpan<T> values)
        {
            return IndexOfAnyExceptCore(span, values);
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <returns>The index of the first value other than the specified values, or -1 if all values are excluded.</returns>
        public nint IndexOfAnyExcept(T value0, T value1)
        {
            return IndexOfAnyExceptCore(span, value0, value1);
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <param name="value2">The third value to exclude.</param>
        /// <returns>The index of the first value other than the specified values, or -1 if all values are excluded.</returns>
        public nint IndexOfAnyExcept(T value0, T value1, T value2)
        {
            return IndexOfAnyExceptCore(span, value0, value1, value2);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified value.
        /// </summary>
        /// <param name="value">The value to exclude.</param>
        /// <returns>The index of the last value other than <paramref name="value"/>, or -1 if all values match.</returns>
        public nint LastIndexOfAnyExcept(T value)
        {
            return LastIndexOfAnyExceptCore(span, value);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="values">The values to exclude.</param>
        /// <returns>The index of the last value not in <paramref name="values"/>, or -1 if all values are excluded.</returns>
        public nint LastIndexOfAnyExcept(BigReadOnlySpan<T> values)
        {
            return LastIndexOfAnyExceptCore(span, values);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <returns>The index of the last value other than the specified values, or -1 if all values are excluded.</returns>
        public nint LastIndexOfAnyExcept(T value0, T value1)
        {
            return LastIndexOfAnyExceptCore(span, value0, value1);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <param name="value2">The third value to exclude.</param>
        /// <returns>The index of the last value other than the specified values, or -1 if all values are excluded.</returns>
        public nint LastIndexOfAnyExcept(T value0, T value1, T value2)
        {
            return LastIndexOfAnyExceptCore(span, value0, value1, value2);
        }

        /// <summary>
        /// Determines whether the read-only span contains any value other than the specified value.
        /// </summary>
        /// <param name="value">The value to exclude.</param>
        /// <returns><see langword="true"/> if any value differs from <paramref name="value"/>; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExcept(T value)
        {
            return IndexOfAnyExceptCore(span, value) >= 0;
        }

        /// <summary>
        /// Determines whether the read-only span contains any value other than the specified values.
        /// </summary>
        /// <param name="values">The values to exclude.</param>
        /// <returns><see langword="true"/> if any value is not in <paramref name="values"/>; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExcept(BigReadOnlySpan<T> values)
        {
            return IndexOfAnyExceptCore(span, values) >= 0;
        }

        /// <summary>
        /// Determines whether the read-only span contains any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <returns><see langword="true"/> if any value differs from the specified values; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExcept(T value0, T value1)
        {
            return IndexOfAnyExceptCore(span, value0, value1) >= 0;
        }

        /// <summary>
        /// Determines whether the read-only span contains any value other than the specified values.
        /// </summary>
        /// <param name="value0">The first value to exclude.</param>
        /// <param name="value1">The second value to exclude.</param>
        /// <param name="value2">The third value to exclude.</param>
        /// <returns><see langword="true"/> if any value differs from the specified values; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExcept(T value0, T value1, T value2)
        {
            return IndexOfAnyExceptCore(span, value0, value1, value2) >= 0;
        }

        /// <summary>
        /// Determines whether the current read-only span and another read-only span contain the same elements.
        /// </summary>
        /// <param name="other">The span to compare with the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if the spans contain the same elements; otherwise, <see langword="false"/>.</returns>
        public bool SequenceEqual(BigReadOnlySpan<T> other, IEqualityComparer<T>? comparer = null)
        {
            return SequenceEqualCore(span, other, comparer);
        }

        /// <summary>
        /// Compares the current read-only span with another read-only span lexicographically.
        /// </summary>
        /// <param name="other">The span to compare with the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default comparer.</param>
        /// <returns>A value that indicates the relative order of the spans.</returns>
        public int SequenceCompareTo(BigReadOnlySpan<T> other, IComparer<T>? comparer = null)
        {
            return SequenceCompareToCore(span, other, comparer);
        }

        /// <summary>
        /// Determines whether the beginning of the current read-only span matches another span.
        /// </summary>
        /// <param name="value">The span to compare with the start of the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> matches the start of the current span; otherwise, <see langword="false"/>.</returns>
        public bool StartsWith(BigReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
        {
            return StartsWithCore(span, value, comparer);
        }

        /// <summary>
        /// Determines whether the end of the current read-only span matches another span.
        /// </summary>
        /// <param name="value">The span to compare with the end of the current span.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default equality comparer.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> matches the end of the current span; otherwise, <see langword="false"/>.</returns>
        public bool EndsWith(BigReadOnlySpan<T> value, IEqualityComparer<T>? comparer = null)
        {
            return EndsWithCore(span, value, comparer);
        }
    }

    extension<T>(BigSpan<T> span) where T : IEquatable<T>
    {
        /// <summary>
        /// Splits the current span using any of the specified precomputed separators.
        /// </summary>
        /// <param name="separators">The precomputed separators to split on.</param>
        /// <returns>An enumerator over the split segments.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="separators"/> is <see langword="null"/>.</exception>
        public BigSpanSearchValuesSplitEnumerator<T> SplitAny(SearchValues<T> separators)
        {
            ArgumentNullException.ThrowIfNull(separators);
            return new BigSpanSearchValuesSplitEnumerator<T>(AsReadOnlySpan(span), separators);
        }

        /// <summary>
        /// Searches for the first occurrence of any of the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to locate.</param>
        /// <returns>The index of the first occurrence of any value in <paramref name="values"/>, or -1 if none are found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public nint IndexOfAny(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return IndexOfAnyCore(AsReadOnlySpan(span), values);
        }

        /// <summary>
        /// Searches for the last occurrence of any of the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to locate.</param>
        /// <returns>The index of the last occurrence of any value in <paramref name="values"/>, or -1 if none are found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public nint LastIndexOfAny(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return LastIndexOfAnyCore(AsReadOnlySpan(span), values);
        }

        /// <summary>
        /// Determines whether the span contains any of the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to locate.</param>
        /// <returns><see langword="true"/> if any value in <paramref name="values"/> is found; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public bool ContainsAny(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return IndexOfAnyCore(AsReadOnlySpan(span), values) >= 0;
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to exclude.</param>
        /// <returns>The index of the first value not in <paramref name="values"/>, or -1 if all values are excluded.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public nint IndexOfAnyExcept(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), values);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to exclude.</param>
        /// <returns>The index of the last value not in <paramref name="values"/>, or -1 if all values are excluded.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public nint LastIndexOfAnyExcept(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return LastIndexOfAnyExceptCore(AsReadOnlySpan(span), values);
        }

        /// <summary>
        /// Determines whether the span contains any value other than the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to exclude.</param>
        /// <returns><see langword="true"/> if any value is not in <paramref name="values"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public bool ContainsAnyExcept(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return IndexOfAnyExceptCore(AsReadOnlySpan(span), values) >= 0;
        }
    }

    extension<T>(BigReadOnlySpan<T> span) where T : IEquatable<T>
    {
        /// <summary>
        /// Splits the current read-only span using any of the specified precomputed separators.
        /// </summary>
        /// <param name="separators">The precomputed separators to split on.</param>
        /// <returns>An enumerator over the split segments.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="separators"/> is <see langword="null"/>.</exception>
        public BigSpanSearchValuesSplitEnumerator<T> SplitAny(SearchValues<T> separators)
        {
            ArgumentNullException.ThrowIfNull(separators);
            return new BigSpanSearchValuesSplitEnumerator<T>(span, separators);
        }

        /// <summary>
        /// Searches for the first occurrence of any of the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to locate.</param>
        /// <returns>The index of the first occurrence of any value in <paramref name="values"/>, or -1 if none are found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public nint IndexOfAny(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return IndexOfAnyCore(span, values);
        }

        /// <summary>
        /// Searches for the last occurrence of any of the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to locate.</param>
        /// <returns>The index of the last occurrence of any value in <paramref name="values"/>, or -1 if none are found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public nint LastIndexOfAny(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return LastIndexOfAnyCore(span, values);
        }

        /// <summary>
        /// Determines whether the read-only span contains any of the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to locate.</param>
        /// <returns><see langword="true"/> if any value in <paramref name="values"/> is found; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public bool ContainsAny(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return IndexOfAnyCore(span, values) >= 0;
        }

        /// <summary>
        /// Searches for the first occurrence of any value other than the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to exclude.</param>
        /// <returns>The index of the first value not in <paramref name="values"/>, or -1 if all values are excluded.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public nint IndexOfAnyExcept(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return IndexOfAnyExceptCore(span, values);
        }

        /// <summary>
        /// Searches for the last occurrence of any value other than the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to exclude.</param>
        /// <returns>The index of the last value not in <paramref name="values"/>, or -1 if all values are excluded.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public nint LastIndexOfAnyExcept(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return LastIndexOfAnyExceptCore(span, values);
        }

        /// <summary>
        /// Determines whether the read-only span contains any value other than the specified precomputed values.
        /// </summary>
        /// <param name="values">The precomputed values to exclude.</param>
        /// <returns><see langword="true"/> if any value is not in <paramref name="values"/>; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <see langword="null"/>.</exception>
        public bool ContainsAnyExcept(SearchValues<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            return IndexOfAnyExceptCore(span, values) >= 0;
        }
    }

    extension<T>(BigSpan<T> span) where T : IComparable<T>
    {
        /// <summary>
        /// Searches for the first occurrence of any value in the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the range to locate.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the range to locate.</param>
        /// <returns>The index of the first value in the specified range, or -1 if no value is found.</returns>
        public nint IndexOfAnyInRange(T lowInclusive, T highInclusive)
        {
            return IndexOfAnyInRangeCore(AsReadOnlySpan(span), lowInclusive, highInclusive);
        }

        /// <summary>
        /// Searches for the last occurrence of any value in the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the range to locate.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the range to locate.</param>
        /// <returns>The index of the last value in the specified range, or -1 if no value is found.</returns>
        public nint LastIndexOfAnyInRange(T lowInclusive, T highInclusive)
        {
            return LastIndexOfAnyInRangeCore(AsReadOnlySpan(span), lowInclusive, highInclusive);
        }

        /// <summary>
        /// Determines whether the span contains any value in the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the range to locate.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the range to locate.</param>
        /// <returns><see langword="true"/> if any value is in the specified range; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyInRange(T lowInclusive, T highInclusive)
        {
            return IndexOfAnyInRangeCore(AsReadOnlySpan(span), lowInclusive, highInclusive) >= 0;
        }

        /// <summary>
        /// Searches for the first occurrence of any value outside the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the excluded range.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the excluded range.</param>
        /// <returns>The index of the first value outside the specified range, or -1 if all values are inside the range.</returns>
        public nint IndexOfAnyExceptInRange(T lowInclusive, T highInclusive)
        {
            return IndexOfAnyExceptInRangeCore(AsReadOnlySpan(span), lowInclusive, highInclusive);
        }

        /// <summary>
        /// Searches for the last occurrence of any value outside the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the excluded range.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the excluded range.</param>
        /// <returns>The index of the last value outside the specified range, or -1 if all values are inside the range.</returns>
        public nint LastIndexOfAnyExceptInRange(T lowInclusive, T highInclusive)
        {
            return LastIndexOfAnyExceptInRangeCore(AsReadOnlySpan(span), lowInclusive, highInclusive);
        }

        /// <summary>
        /// Determines whether the span contains any value outside the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the excluded range.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the excluded range.</param>
        /// <returns><see langword="true"/> if any value is outside the specified range; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExceptInRange(T lowInclusive, T highInclusive)
        {
            return IndexOfAnyExceptInRangeCore(AsReadOnlySpan(span), lowInclusive, highInclusive) >= 0;
        }
    }

    extension<T>(BigReadOnlySpan<T> span) where T : IComparable<T>
    {
        /// <summary>
        /// Searches for the first occurrence of any value in the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the range to locate.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the range to locate.</param>
        /// <returns>The index of the first value in the specified range, or -1 if no value is found.</returns>
        public nint IndexOfAnyInRange(T lowInclusive, T highInclusive)
        {
            return IndexOfAnyInRangeCore(span, lowInclusive, highInclusive);
        }

        /// <summary>
        /// Searches for the last occurrence of any value in the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the range to locate.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the range to locate.</param>
        /// <returns>The index of the last value in the specified range, or -1 if no value is found.</returns>
        public nint LastIndexOfAnyInRange(T lowInclusive, T highInclusive)
        {
            return LastIndexOfAnyInRangeCore(span, lowInclusive, highInclusive);
        }

        /// <summary>
        /// Determines whether the read-only span contains any value in the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the range to locate.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the range to locate.</param>
        /// <returns><see langword="true"/> if any value is in the specified range; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyInRange(T lowInclusive, T highInclusive)
        {
            return IndexOfAnyInRangeCore(span, lowInclusive, highInclusive) >= 0;
        }

        /// <summary>
        /// Searches for the first occurrence of any value outside the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the excluded range.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the excluded range.</param>
        /// <returns>The index of the first value outside the specified range, or -1 if all values are inside the range.</returns>
        public nint IndexOfAnyExceptInRange(T lowInclusive, T highInclusive)
        {
            return IndexOfAnyExceptInRangeCore(span, lowInclusive, highInclusive);
        }

        /// <summary>
        /// Searches for the last occurrence of any value outside the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the excluded range.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the excluded range.</param>
        /// <returns>The index of the last value outside the specified range, or -1 if all values are inside the range.</returns>
        public nint LastIndexOfAnyExceptInRange(T lowInclusive, T highInclusive)
        {
            return LastIndexOfAnyExceptInRangeCore(span, lowInclusive, highInclusive);
        }

        /// <summary>
        /// Determines whether the read-only span contains any value outside the specified inclusive range.
        /// </summary>
        /// <param name="lowInclusive">The lower bound, inclusive, of the excluded range.</param>
        /// <param name="highInclusive">The upper bound, inclusive, of the excluded range.</param>
        /// <returns><see langword="true"/> if any value is outside the specified range; otherwise, <see langword="false"/>.</returns>
        public bool ContainsAnyExceptInRange(T lowInclusive, T highInclusive)
        {
            return IndexOfAnyExceptInRangeCore(span, lowInclusive, highInclusive) >= 0;
        }
    }

    extension(GC)
    {
        /// <summary>
        /// Allocates a zero-initialized <see cref="BigArray{T}"/> with the specified length.
        /// </summary>
        /// <param name="length">The number of elements in the array.</param>
        /// <param name="pinned"><see langword="true"/> to allocate the underlying storage as pinned; otherwise, <see langword="false"/>.</param>
        /// <returns>A zero-initialized <see cref="BigArray{T}"/> with the specified length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is negative or greater than <see cref="BigArray{T}.MaxLength"/>.</exception>
        public static BigArray<T> AllocateBigArray<T>(nint length, bool pinned = false)
        {
            return BigArray<T>.Allocate(length, pinned, uninitialized: false);
        }

        /// <summary>
        /// Allocates a <see cref="BigArray{T}"/> with the specified length, leaving the underlying storage uninitialized when the runtime can do so.
        /// </summary>
        /// <param name="length">The number of elements in the array.</param>
        /// <param name="pinned"><see langword="true"/> to allocate the underlying storage as pinned; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see cref="BigArray{T}"/> with the specified length.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is negative or greater than <see cref="BigArray{T}.MaxLength"/>.</exception>
        public static BigArray<T> AllocateUninitializedBigArray<T>(nint length, bool pinned = false)
        {
            return BigArray<T>.Allocate(length, pinned, uninitialized: true);
        }
    }
}

internal static class ThrowHelpers
{
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void ThrowOutOfRange(string? paramName = null, string? message = null)
    {
        throw new ArgumentOutOfRangeException(paramName, message);
    }
}
