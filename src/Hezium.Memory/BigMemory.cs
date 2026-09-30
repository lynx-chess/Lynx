using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hezium.Memory;

/// <summary>
/// Represents a contiguous region of memory.
/// </summary>
/// <typeparam name="T">The type of elements in the memory.</typeparam>
[DebuggerDisplay("IsEmpty = {IsEmpty}, Length = {(long)Length}")]
public readonly struct BigMemory<T> : IEquatable<BigMemory<T>>
{
    internal readonly Array? _storage;
    internal readonly nint _start;
    internal readonly nint _length;

    /// <summary>
    /// Gets an empty <see cref="BigMemory{T}"/>.
    /// </summary>
    public static BigMemory<T> Empty => default;

    /// <summary>
    /// Gets the number of elements in the memory.
    /// </summary>
    public nint Length => _length;

    /// <summary>
    /// Gets a value that indicates whether the memory is empty.
    /// </summary>
    public bool IsEmpty => _length == 0;

    /// <summary>
    /// Gets a span over the memory.
    /// </summary>
    public BigSpan<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_storage is null) return default;
            return new BigSpan<T>(ref Unsafe.Add(ref GetDataReference(_storage), _start), _length);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigMemory{T}"/> struct over the specified array.
    /// </summary>
    /// <param name="array">The array to wrap, or <see langword="null"/> for empty memory.</param>
    [OverloadResolutionPriority(-1)]
    public BigMemory(T[]? array)
    {
        if (array is null)
        {
            _storage = null;
            _start = 0;
            _length = 0;
            return;
        }

        _storage = array;
        _start = 0;
        _length = array.Length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigMemory{T}"/> struct over the specified range of an array.
    /// </summary>
    /// <param name="array">The array to wrap, or <see langword="null"/> when <paramref name="start"/> and <paramref name="length"/> are zero.</param>
    /// <param name="start">The zero-based index at which the memory starts.</param>
    /// <param name="length">The number of elements in the memory.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of <paramref name="array"/>.</exception>
    [OverloadResolutionPriority(-1)]
    public BigMemory(T[]? array, int start, int length)
    {
        nint arrayLength = array?.Length ?? 0;
        if ((nuint)start > (nuint)arrayLength || (nuint)length > (nuint)(arrayLength - start)) ThrowHelpers.ThrowOutOfRange();

        _storage = array;
        _start = start;
        _length = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigMemory{T}"/> struct over the specified <see cref="BigArray{T}"/>.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <see langword="null"/>.</exception>
    public BigMemory(BigArray<T> array)
    {
        ArgumentNullException.ThrowIfNull(array);

        _storage = array._storage;
        _start = 0;
        _length = array._length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigMemory{T}"/> struct over the specified range of a <see cref="BigArray{T}"/>.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="start">The zero-based index at which the memory starts.</param>
    /// <param name="length">The number of elements in the memory.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of <paramref name="array"/>.</exception>
    public BigMemory(BigArray<T> array, nint start, nint length)
    {
        ArgumentNullException.ThrowIfNull(array);
        if ((nuint)start > (nuint)array._length || (nuint)length > (nuint)(array._length - start)) ThrowHelpers.ThrowOutOfRange();

        _storage = array._storage;
        _start = start;
        _length = length;
    }

    [OverloadResolutionPriority(-1)]
    internal BigMemory(Array? storage, nint start, nint length)
    {
        _storage = storage;
        _start = start;
        _length = length;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="BigMemory{T}"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="BigMemory{T}"/>.</returns>
    public override string ToString()
    {
        return $"{nameof(BigMemory<>)}<{typeof(T)}>[{_length}]";
    }

    /// <summary>
    /// Creates a new <see cref="BigMemory{T}"/> that represents a slice of the current memory starting at the specified index.
    /// </summary>
    /// <param name="start">The index at which to start the slice.</param>
    /// <returns>A new <see cref="BigMemory{T}"/> that represents the specified slice.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="start"/> is outside the bounds of the memory.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigMemory<T> Slice(nint start)
    {
        if ((nuint)start > (nuint)_length) ThrowHelpers.ThrowOutOfRange(nameof(start));

        return new BigMemory<T>(_storage, _start + start, _length - start);
    }

    /// <summary>
    /// Creates a new <see cref="BigMemory{T}"/> that represents a slice of the current memory starting at the specified index and with the specified length.
    /// </summary>
    /// <param name="start">The index at which to start the slice.</param>
    /// <param name="length">The number of elements in the slice.</param>
    /// <returns>A new <see cref="BigMemory{T}"/> that represents the specified slice.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of the memory.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigMemory<T> Slice(nint start, nint length)
    {
        if ((nuint)start > (nuint)_length || (nuint)length > (nuint)(_length - start)) ThrowHelpers.ThrowOutOfRange();

        return new BigMemory<T>(_storage, _start + start, length);
    }

    /// <summary>
    /// Copies the contents of the memory into another memory.
    /// </summary>
    /// <param name="destination">The destination memory.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is too small.</exception>
    public void CopyTo(BigMemory<T> destination)
    {
        Span.CopyTo(destination.Span);
    }

    /// <summary>
    /// Attempts to copy the contents of the memory into another memory.
    /// </summary>
    /// <param name="destination">The destination memory.</param>
    /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
    public bool TryCopyTo(BigMemory<T> destination)
    {
        return Span.TryCopyTo(destination.Span);
    }

    /// <summary>
    /// Copies the contents of the memory into a new single-dimensional array.
    /// </summary>
    /// <returns>A new array containing the copied elements.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the memory is too large to fit in a single managed array.</exception>
    public T[] ToArray()
    {
        return Span.ToArray();
    }

    /// <summary>
    /// Copies the contents of the memory into a new <see cref="BigArray{T}"/>.
    /// </summary>
    /// <returns>A new <see cref="BigArray{T}"/> containing the copied elements.</returns>
    public BigArray<T> ToBigArray()
    {
        return Span.ToBigArray();
    }

    /// <summary>
    /// Pins the memory and returns a handle to the pinned region.
    /// </summary>
    /// <returns>A handle to the pinned memory.</returns>
    public MemoryHandle Pin()
    {
        return Pin(_storage, _start);
    }

    internal static unsafe MemoryHandle Pin(Array? storage, nint start)
    {
        if (storage is null) return default;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) throw new ArgumentException("Object contains references.", typeof(T).ToString());

        PinnedGCHandle<Array> pinned = new(storage);
        try
        {
            GCHandle handle = GCHandle.FromIntPtr(PinnedGCHandle<Array>.ToIntPtr(pinned));
            void* pointer = Unsafe.AsPointer(ref Unsafe.Add(ref GetDataReference(storage), start));
            return new MemoryHandle(pointer, handle, pinnable: null);
        }
        catch
        {
            pinned.Dispose();
            throw;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref T GetDataReference(Array storage)
    {
        return ref Unsafe.As<byte, T>(ref MemoryMarshal.GetArrayDataReference(storage));
    }

    /// <inheritdoc/>
    public bool Equals(BigMemory<T> other)
    {
        return ReferenceEquals(_storage, other._storage) && _start == other._start && _length == other._length;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is BigMemory<T> other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(_storage, _start, _length);
    }

    /// <summary>
    /// Defines an implicit conversion from an array to <see cref="BigMemory{T}"/>.
    /// </summary>
    /// <param name="array">The array to convert.</param>
    public static implicit operator BigMemory<T>(T[]? array)
    {
        return new BigMemory<T>(array);
    }

    /// <summary>
    /// Defines an implicit conversion from an array segment to <see cref="BigMemory{T}"/>.
    /// </summary>
    /// <param name="segment">The array segment to convert.</param>
    public static implicit operator BigMemory<T>(ArraySegment<T> segment)
    {
        return new BigMemory<T>(segment.Array, segment.Offset, segment.Count);
    }

    /// <summary>
    /// Defines an implicit conversion from <see cref="BigMemory{T}"/> to <see cref="BigReadOnlyMemory{T}"/>.
    /// </summary>
    /// <param name="memory">The memory to convert.</param>
    public static implicit operator BigReadOnlyMemory<T>(BigMemory<T> memory)
    {
        return new BigReadOnlyMemory<T>(memory._storage, memory._start, memory._length);
    }

    /// <summary>
    /// Compares two <see cref="BigMemory{T}"/> instances for equality.
    /// </summary>
    /// <param name="left">The first <see cref="BigMemory{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="BigMemory{T}"/> to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(BigMemory<T> left, BigMemory<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares two <see cref="BigMemory{T}"/> instances for inequality.
    /// </summary>
    /// <param name="left">The first <see cref="BigMemory{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="BigMemory{T}"/> to compare.</param>
    /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(BigMemory<T> left, BigMemory<T> right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Represents a read-only contiguous region of memory.
/// </summary>
/// <typeparam name="T">The type of elements in the memory.</typeparam>
public readonly struct BigReadOnlyMemory<T> : IEquatable<BigReadOnlyMemory<T>>
{
    internal readonly Array? _storage;
    internal readonly nint _start;
    internal readonly nint _length;

    /// <summary>
    /// Gets an empty <see cref="BigReadOnlyMemory{T}"/>.
    /// </summary>
    public static BigReadOnlyMemory<T> Empty => default;

    /// <summary>
    /// Gets the number of elements in the memory.
    /// </summary>
    public nint Length => _length;

    /// <summary>
    /// Gets a value that indicates whether the memory is empty.
    /// </summary>
    public bool IsEmpty => _length == 0;

    /// <summary>
    /// Gets a read-only span over the memory.
    /// </summary>
    public BigReadOnlySpan<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_storage is null) return default;
            return new BigReadOnlySpan<T>(ref Unsafe.Add(ref BigMemory<T>.GetDataReference(_storage), _start), _length);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlyMemory{T}"/> struct over the specified array.
    /// </summary>
    /// <param name="array">The array to wrap, or <see langword="null"/> for empty memory.</param>
    [OverloadResolutionPriority(-1)]
    public BigReadOnlyMemory(T[]? array)
    {
        if (array is null)
        {
            _storage = null;
            _start = 0;
            _length = 0;
            return;
        }

        _storage = array;
        _start = 0;
        _length = array.Length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlyMemory{T}"/> struct over the specified range of an array.
    /// </summary>
    /// <param name="array">The array to wrap, or <see langword="null"/> when <paramref name="start"/> and <paramref name="length"/> are zero.</param>
    /// <param name="start">The zero-based index at which the memory starts.</param>
    /// <param name="length">The number of elements in the memory.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of <paramref name="array"/>.</exception>
    [OverloadResolutionPriority(-1)]
    public BigReadOnlyMemory(T[]? array, nint start, nint length)
    {
        nint arrayLength = array?.Length ?? 0;
        if ((nuint)start > (nuint)arrayLength || (nuint)length > (nuint)(arrayLength - start)) ThrowHelpers.ThrowOutOfRange();

        _storage = array;
        _start = start;
        _length = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlyMemory{T}"/> struct over the specified <see cref="BigArray{T}"/>.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <see langword="null"/>.</exception>
    public BigReadOnlyMemory(BigArray<T> array)
    {
        ArgumentNullException.ThrowIfNull(array);

        _storage = array._storage;
        _start = 0;
        _length = array._length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlyMemory{T}"/> struct over the specified range of a <see cref="BigArray{T}"/>.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    /// <param name="start">The zero-based index at which the memory starts.</param>
    /// <param name="length">The number of elements in the memory.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of <paramref name="array"/>.</exception>
    public BigReadOnlyMemory(BigArray<T> array, nint start, nint length)
    {
        ArgumentNullException.ThrowIfNull(array);
        if ((nuint)start > (nuint)array._length || (nuint)length > (nuint)(array._length - start)) ThrowHelpers.ThrowOutOfRange();

        _storage = array._storage;
        _start = start;
        _length = length;
    }

    [OverloadResolutionPriority(-1)]
    internal BigReadOnlyMemory(Array? storage, nint start, nint length)
    {
        _storage = storage;
        _start = start;
        _length = length;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="BigReadOnlyMemory{T}"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="BigReadOnlyMemory{T}"/>.</returns>
    public override string ToString()
    {
        return $"{nameof(BigReadOnlyMemory<>)}<{typeof(T)}>[{_length}]";
    }

    /// <summary>
    /// Creates a new <see cref="BigReadOnlyMemory{T}"/> that represents a slice of the current memory starting at the specified index.
    /// </summary>
    /// <param name="start">The index at which to start the slice.</param>
    /// <returns>A new <see cref="BigReadOnlyMemory{T}"/> that represents the specified slice.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="start"/> is outside the bounds of the memory.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigReadOnlyMemory<T> Slice(nint start)
    {
        if ((nuint)start > (nuint)_length) ThrowHelpers.ThrowOutOfRange(nameof(start));

        return new BigReadOnlyMemory<T>(_storage, _start + start, _length - start);
    }

    /// <summary>
    /// Creates a new <see cref="BigReadOnlyMemory{T}"/> that represents a slice of the current memory starting at the specified index and with the specified length.
    /// </summary>
    /// <param name="start">The index at which to start the slice.</param>
    /// <param name="length">The number of elements in the slice.</param>
    /// <returns>A new <see cref="BigReadOnlyMemory{T}"/> that represents the specified slice.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested range is outside the bounds of the memory.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BigReadOnlyMemory<T> Slice(nint start, nint length)
    {
        if ((nuint)start > (nuint)_length || (nuint)length > (nuint)(_length - start)) ThrowHelpers.ThrowOutOfRange();

        return new BigReadOnlyMemory<T>(_storage, _start + start, length);
    }

    /// <summary>
    /// Copies the contents of the memory into another memory.
    /// </summary>
    /// <param name="destination">The destination memory.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is too small.</exception>
    public void CopyTo(BigMemory<T> destination)
    {
        Span.CopyTo(destination.Span);
    }

    /// <summary>
    /// Attempts to copy the contents of the memory into another memory.
    /// </summary>
    /// <param name="destination">The destination memory.</param>
    /// <returns><see langword="true"/> if the copy succeeded; otherwise, <see langword="false"/>.</returns>
    public bool TryCopyTo(BigMemory<T> destination)
    {
        return Span.TryCopyTo(destination.Span);
    }

    /// <summary>
    /// Copies the contents of the memory into a new single-dimensional array.
    /// </summary>
    /// <returns>A new array containing the copied elements.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the memory is too large to fit in a single managed array.</exception>
    public T[] ToArray()
    {
        return Span.ToArray();
    }

    /// <summary>
    /// Copies the contents of the memory into a new <see cref="BigArray{T}"/>.
    /// </summary>
    /// <returns>A new <see cref="BigArray{T}"/> containing the copied elements.</returns>
    public BigArray<T> ToBigArray()
    {
        return Span.ToBigArray();
    }

    /// <summary>
    /// Pins the memory and returns a handle to the pinned region.
    /// </summary>
    /// <returns>A handle to the pinned memory.</returns>
    public MemoryHandle Pin()
    {
        return BigMemory<T>.Pin(_storage, _start);
    }

    /// <inheritdoc/>
    public bool Equals(BigReadOnlyMemory<T> other)
    {
        return ReferenceEquals(_storage, other._storage) && _start == other._start && _length == other._length;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is BigReadOnlyMemory<T> other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(_storage, _start, _length);
    }

    /// <summary>
    /// Defines an implicit conversion from an array to <see cref="BigReadOnlyMemory{T}"/>.
    /// </summary>
    /// <param name="array">The array to convert.</param>
    public static implicit operator BigReadOnlyMemory<T>(T[]? array)
    {
        return new BigReadOnlyMemory<T>(array);
    }

    /// <summary>
    /// Defines an implicit conversion from an array segment to <see cref="BigReadOnlyMemory{T}"/>.
    /// </summary>
    /// <param name="segment">The array segment to convert.</param>
    public static implicit operator BigReadOnlyMemory<T>(ArraySegment<T> segment)
    {
        return new BigReadOnlyMemory<T>(segment.Array, segment.Offset, segment.Count);
    }

    /// <summary>
    /// Compares two <see cref="BigReadOnlyMemory{T}"/> instances for equality.
    /// </summary>
    /// <param name="left">The first <see cref="BigReadOnlyMemory{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="BigReadOnlyMemory{T}"/> to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(BigReadOnlyMemory<T> left, BigReadOnlyMemory<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compares two <see cref="BigReadOnlyMemory{T}"/> instances for inequality.
    /// </summary>
    /// <param name="left">The first <see cref="BigReadOnlyMemory{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="BigReadOnlyMemory{T}"/> to compare.</param>
    /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(BigReadOnlyMemory<T> left, BigReadOnlyMemory<T> right)
    {
        return !(left == right);
    }
}
