using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hezium.Memory;

/// <summary>
/// Represents a one-dimensional, zero-based array.
/// </summary>
/// <typeparam name="T">The type of elements in the array.</typeparam>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.CreateBigArray))]
[DebuggerDisplay("IsEmpty = {IsEmpty}, Length = {(long)Length}")]
public sealed partial class BigArray<T> : IEnumerable<T>
{
    /// <summary>
    /// Returns a reference to the element at index 0 of the array, or a null reference if the array is empty.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetPinnableReference()
    {
        if (_length == 0)
        {
            return ref Unsafe.NullRef<T>();
        }

        return ref MemoryExtensions.GetBigArrayDataReference(this);
    }

    internal readonly Array _storage;
    internal readonly nint _length;

    static BigArray()
    {
        if (Unsafe.SizeOf<T>() > 65535)
        {
            throw new NotSupportedException($"Type {typeof(T)} is too large to be used with BigArray.");
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigArray{T}"/> class with the specified length.
    /// </summary>
    /// <param name="length">The number of elements in the array.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="length"/> is negative or greater than <see cref="MaxLength"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigArray(nint length)
    {
        if ((nuint)length > (nuint)MaxLength) ThrowHelpers.ThrowOutOfRange(nameof(length));

        if (length <= Array.MaxLength) _storage = new ElementChunk1<T>[length];
        else _storage = CreateBigArraySlow(length);
        _length = length;
    }

    private BigArray(Array storage, nint length)
    {
        _storage = storage;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static BigArray<T> Allocate(nint length, bool pinned, bool uninitialized)
    {
        if ((nuint)length > (nuint)MaxLength) ThrowHelpers.ThrowOutOfRange(nameof(length));

        Array storage = length <= Array.MaxLength
            ? uninitialized
                ? GC.AllocateUninitializedArray<ElementChunk1<T>>((int)length, pinned)
                : GC.AllocateArray<ElementChunk1<T>>((int)length, pinned)
            : CreateBigArraySlow(length, pinned, uninitialized);

        return new BigArray<T>(storage, length);
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="BigArray{T}"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="BigArray{T}"/>.</returns>
    public override string ToString()
    {
        return $"{nameof(BigArray<>)}<{typeof(T)}>[{_length}]";
    }

    /// <summary>
    /// Gets an empty <see cref="BigArray{T}"/>.
    /// </summary>
    public static BigArray<T> Empty { get; } = new(0);

    /// <summary>
    /// Gets the number of elements in the array.
    /// </summary>
    public nint Length => _length;

    /// <summary>
    /// Gets a value that indicates whether the array is empty.
    /// </summary>
    public bool IsEmpty => _length == 0;

    /// <summary>
    /// Gets a reference to the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>A reference to the element at <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is outside the bounds of the array.</exception>
    public ref T this[nint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((nuint)index >= (nuint)_length) ThrowHelpers.ThrowOutOfRange(nameof(index));

            return ref Unsafe.Add(ref MemoryExtensions.GetBigArrayDataReference(this), index);
        }
    }

    /// <summary>
    /// Gets the maximum supported length for a <see cref="BigArray{T}"/>.
    /// </summary>
    public static nint MaxLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return nint.Size == 4 ? Array.MaxLength : GetChunkLength() * (nint)Array.MaxLength;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the array.
    /// </summary>
    /// <returns>An enumerator for the array.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Creates a <see cref="BigSpan{T}"/> over the entire array.
    /// </summary>
    /// <returns>A <see cref="BigSpan{T}"/> over the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigSpan<T> AsBigSpan()
    {
        return new BigSpan<T>(ref MemoryExtensions.GetBigArrayDataReference(this), _length);
    }

    /// <summary>
    /// Creates a <see cref="BigSpan{T}"/> over a range of the array that starts at the specified index.
    /// </summary>
    /// <param name="start">The zero-based index at which the span starts.</param>
    /// <returns>A <see cref="BigSpan{T}"/> over the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="start"/> is outside the bounds of the array.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigSpan<T> AsBigSpan(nint start)
    {
        return AsBigSpan().Slice(start);
    }

    /// <summary>
    /// Creates a <see cref="BigSpan{T}"/> over a range of the array that starts at the specified index and has the specified length.
    /// </summary>
    /// <param name="start">The zero-based index at which the span starts.</param>
    /// <param name="length">The number of elements in the span.</param>
    /// <returns>A <see cref="BigSpan{T}"/> over the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of the array.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigSpan<T> AsBigSpan(nint start, nint length)
    {
        return AsBigSpan().Slice(start, length);
    }

    /// <summary>
    /// Creates a <see cref="BigMemory{T}"/> over the entire array.
    /// </summary>
    /// <returns>A <see cref="BigMemory{T}"/> over the array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigMemory<T> AsBigMemory()
    {
        return new BigMemory<T>(_storage, 0, _length);
    }

    /// <summary>
    /// Creates a <see cref="BigMemory{T}"/> over a range of the array that starts at the specified index.
    /// </summary>
    /// <param name="start">The zero-based index at which the memory starts.</param>
    /// <returns>A <see cref="BigMemory{T}"/> over the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="start"/> is outside the bounds of the array.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigMemory<T> AsBigMemory(nint start)
    {
        return AsBigMemory().Slice(start);
    }

    /// <summary>
    /// Creates a <see cref="BigMemory{T}"/> over a range of the array that starts at the specified index and has the specified length.
    /// </summary>
    /// <param name="start">The zero-based index at which the memory starts.</param>
    /// <param name="length">The number of elements in the memory.</param>
    /// <returns>A <see cref="BigMemory{T}"/> over the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of the array.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigMemory<T> AsBigMemory(nint start, nint length)
    {
        return AsBigMemory().Slice(start, length);
    }

    /// <summary>
    /// Creates a <see cref="Span{T}"/> over a range of the array that starts at the specified index and has the specified length.
    /// </summary>
    /// <param name="start">The zero-based index at which the span starts.</param>
    /// <param name="length">The number of elements in the span.</param>
    /// <returns>A <see cref="Span{T}"/> over the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of the array.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(nint start, int length)
    {
        if ((nuint)start > (nuint)_length || (nuint)length > (nuint)(_length - start)) ThrowHelpers.ThrowOutOfRange();

        return MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryExtensions.GetBigArrayDataReference(this), start), length);
    }
}
