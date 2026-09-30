using System.Buffers;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hezium.Memory;

/// <summary>
/// Provides a type-safe view over a contiguous region of memory.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.CreateBigSpan))]
[DebuggerDisplay("IsEmpty = {IsEmpty}, Length = {(long)Length}")]
public readonly ref struct BigSpan<T>
{
    internal readonly ref T _first;
    internal readonly nint _length;

    /// <summary>
    /// Gets an empty <see cref="BigSpan{T}"/>.
    /// </summary>
    public static BigSpan<T> Empty => default;

    /// <summary>
    /// Gets the number of elements in the span.
    /// </summary>
    public nint Length => _length;

    /// <summary>
    /// Gets a value that indicates whether the span is empty.
    /// </summary>
    public bool IsEmpty => _length == 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="BigSpan{T}"/> struct that represents a single element.
    /// </summary>
    /// <param name="first">A reference to the first element in the span.</param>
    public BigSpan(ref T first)
    {
        _first = ref first;
        _length = 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigSpan{T}"/> struct that represents a contiguous region of memory.
    /// </summary>
    /// <param name="span">The span of values to represent.</param>
    public BigSpan(Span<T> span)
    {
        _first = ref MemoryMarshal.GetReference(span);
        _length = span.Length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigSpan{T}"/> struct that represents the entire <see cref="BigArray{T}"/>.
    /// </summary>
    /// <param name="array">The <see cref="BigArray{T}"/> to represent.</param>
    public BigSpan(BigArray<T> array)
    {
        _first = ref MemoryExtensions.GetBigArrayDataReference(array);
        _length = array._length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigSpan{T}"/> struct that represents a portion of the <see cref="BigArray{T}"/>.
    /// </summary>
    /// <param name="array">The <see cref="BigArray{T}"/> to represent.</param>
    /// <param name="start">The starting index of the span.</param>
    /// <param name="length">The number of elements in the span.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the start or length is out of range.</exception>
    public BigSpan(BigArray<T> array, nint start, nint length)
    {
        ArgumentNullException.ThrowIfNull(array);
        if ((nuint)start > (nuint)array.Length || (nuint)length > (nuint)(array.Length - start)) ThrowHelpers.ThrowOutOfRange();

        _first = ref Unsafe.Add(ref MemoryExtensions.GetBigArrayDataReference(array), start);
        _length = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigSpan{T}"/> struct that represents a contiguous region of memory.
    /// </summary>
    /// <param name="array">The array to represent.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    public BigSpan(T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        _first = ref MemoryMarshal.GetArrayDataReference(array);
        _length = array.Length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigSpan{T}"/> struct that represents a contiguous region of memory.
    /// </summary>
    /// <param name="array">The array to represent.</param>
    /// <param name="start">The starting index of the span.</param>
    /// <param name="length">The number of elements in the span.</param>
    public BigSpan(T[] array, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(array);
        if ((nuint)start > (nuint)array.Length || (nuint)length > (nuint)(array.Length - start)) ThrowHelpers.ThrowOutOfRange();

        _first = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), start);
        _length = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigSpan{T}"/> struct that represents a span of memory starting at the specified pointer and with the specified length.
    /// </summary>
    /// <param name="pointer">A pointer to the first element in the span.</param>
    /// <param name="length">The number of elements in the span.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the length is negative.</exception>
#pragma warning disable CS8500
    public unsafe BigSpan(T* pointer, nint length)
#pragma warning restore CS8500
    {
        if (length < 0) ThrowHelpers.ThrowOutOfRange(nameof(length), "Length must be non-negative.");
        _first = ref Unsafe.AsRef<T>(pointer);
        _length = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigSpan{T}"/> struct that represents a span of memory starting at the specified reference and with the specified length.
    /// </summary>
    /// <param name="first">A reference to the first element in the span.</param>
    /// <param name="length">The number of elements in the span.</param>
    internal BigSpan(ref T first, nint length)
    {
        _first = ref first;
        _length = length;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="BigSpan{T}"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="BigSpan{T}"/>.</returns>
    public override string ToString()
    {
        return $"{nameof(BigSpan<>)}<{typeof(T)}>[{_length}]";
    }

    /// <summary>
    /// Creates a new <see cref="BigSpan{T}"/> that represents a slice of the current span starting at the specified index.
    /// </summary>
    /// <param name="start">The index at which to start the slice.</param>
    /// <returns>A new <see cref="BigSpan{T}"/> that represents the specified slice of the current span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BigSpan<T> Slice(nint start)
    {
        if ((nuint)start > (nuint)_length) ThrowHelpers.ThrowOutOfRange(nameof(start));

        return new BigSpan<T>(ref Unsafe.Add(ref _first, start), _length - start);
    }

    /// <summary>
    /// Creates a new <see cref="BigSpan{T}"/> that represents a slice of the current span starting at the specified index and with the specified length.
    /// </summary>
    /// <param name="start">The index at which to start the slice.</param>
    /// <param name="length">The number of elements in the slice.</param>
    /// <returns>A new <see cref="BigSpan{T}"/> that represents the specified slice of the current span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BigSpan<T> Slice(nint start, nint length)
    {
        if ((nuint)start > (nuint)_length || (nuint)length > (nuint)(_length - start)) ThrowHelpers.ThrowOutOfRange();

        return new BigSpan<T>(ref Unsafe.Add(ref _first, start), length);
    }

    /// <summary>
    /// Gets a reference to the element at the specified index in the span.
    /// </summary>
    /// <param name="index">The index of the element to get.</param>
    /// <returns>A reference to the element at the specified index.</returns>
    public readonly ref T this[nint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((nuint)index >= (nuint)_length) ThrowHelpers.ThrowOutOfRange(nameof(index));

            return ref Unsafe.Add(ref _first, index);
        }
    }

    /// <summary>
    /// Gets a reference to the element that can be used for pinning.
    /// </summary>
    /// <returns>A reference to the first element of the span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetPinnableReference()
    {
        return ref _first;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="BigSpan{T}"/>.
    /// </summary>
    /// <returns>An enumerator for the <see cref="BigSpan{T}"/>.</returns>
    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    /// <summary>
    /// Defines an implicit conversion from <see cref="BigSpan{T}"/> to <see cref="BigReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">The <see cref="BigSpan{T}"/> to convert.</param>
    public static implicit operator BigReadOnlySpan<T>(BigSpan<T> span)
    {
        return new BigReadOnlySpan<T>(ref span._first, span._length);
    }

    /// <summary>
    /// Defines an implicit conversion from <see cref="Span{T}"/> to <see cref="BigSpan{T}"/>.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to convert.</param>
    public static implicit operator BigSpan<T>(Span<T> span)
    {
        return new BigSpan<T>(ref MemoryMarshal.GetReference(span), span.Length);
    }

#pragma warning disable CS0809
    /// <summary>
    /// This method is not supported and will throw a <see cref="NotSupportedException"/> if called.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>Does not return a value. Always throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown always.</exception>
    [Obsolete("Equals() on BigSpan<T> is not supported. Use the equality operator instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals([NotNullWhen(true)] object? obj) => throw new NotSupportedException();

    /// <summary>
    /// This method is not supported and will throw a <see cref="NotSupportedException"/> if called.
    /// </summary>
    /// <returns>Does not return a value. Always throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown always.</exception>
    [Obsolete("GetHashCode() on BigSpan<T> is not supported.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();
#pragma warning restore CS0809

    /// <summary>
    /// Compare two <see cref="BigSpan{T}"/> instances for equality.
    /// </summary>
    /// <param name="left">The first <see cref="BigSpan{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="BigSpan{T}"/> to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(BigSpan<T> left, BigSpan<T> right)
    {
        return left._length == right._length && Unsafe.AreSame(ref left._first, ref right._first);
    }

    /// <summary>
    /// Compare two <see cref="BigSpan{T}"/> instances for inequality.
    /// </summary>
    /// <param name="left">The first <see cref="BigSpan{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="BigSpan{T}"/> to compare.</param>
    /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(BigSpan<T> left, BigSpan<T> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Enumerates the elements of a <see cref="BigSpan{T}"/>.
    /// </summary>
    public ref struct Enumerator : IEnumerator<T>
    {
        private readonly BigSpan<T> _span;
        private nint _offset;

        internal Enumerator(BigSpan<T> span)
        {
            _span = span;
            _offset = -1;
        }

        /// <summary>
        /// Gets a reference to the current element in the enumerator.
        /// </summary>
        public readonly ref T Current => ref _span[_offset];
        readonly T IEnumerator<T>.Current => Current;
        readonly object? IEnumerator.Current => Current;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_offset < _span._length - 1)
            {
                _offset++;
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public readonly void Dispose() { }

        /// <inheritdoc/>
        public void Reset()
        {
            _offset = -1;
        }
    }
}

/// <summary>
/// Provides a type-safe read-only view over a contiguous region of memory.
/// </summary>
/// <typeparam name="T">The type of elements in the span.</typeparam>
[CollectionBuilder(typeof(CollectionBuilders), nameof(CollectionBuilders.CreateBigReadOnlySpan))]
public readonly ref struct BigReadOnlySpan<T>
{
    internal readonly ref readonly T _first;
    internal readonly nint _length;

    /// <summary>
    /// Gets an empty <see cref="BigReadOnlySpan{T}"/>.
    /// </summary>
    public static BigReadOnlySpan<T> Empty => default;

    /// <summary>
    /// Gets the number of elements in the span.
    /// </summary>
    public nint Length => _length;

    /// <summary>
    /// Gets a value that indicates whether the span is empty.
    /// </summary>
    public bool IsEmpty => _length == 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlySpan{T}"/> struct that represents a single element.
    /// </summary>
    /// <param name="first">A reference to the first element in the span.</param>
    public BigReadOnlySpan(ref T first)
    {
        _first = ref first;
        _length = 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlySpan{T}"/> struct that represents a span of memory starting at the specified reference and with the specified length.
    /// </summary>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to represent.</param>
    public BigReadOnlySpan(ReadOnlySpan<T> span)
    {
        _first = ref MemoryMarshal.GetReference(span);
        _length = span.Length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlySpan{T}"/> struct that represents a span of memory starting at the specified pointer and with the specified length.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to represent.</param>
    public BigReadOnlySpan(Span<T> span)
    {
        _first = ref MemoryMarshal.GetReference(span);
        _length = span.Length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlySpan{T}"/> struct that represents the entire <see cref="BigArray{T}"/>.
    /// </summary>
    /// <param name="array">The <see cref="BigArray{T}"/> to represent.</param>
    public BigReadOnlySpan(BigArray<T> array)
    {
        _first = ref MemoryExtensions.GetBigArrayDataReference(array);
        _length = array._length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlySpan{T}"/> struct that represents a span of memory starting at the specified reference and with the specified length.
    /// </summary>
    /// <param name="array">The array to represent.</param>
    /// <param name="start">The starting index of the span within the array.</param>
    /// <param name="length">The number of elements in the span.</param>
    public BigReadOnlySpan(BigArray<T> array, nint start, nint length)
    {
        ArgumentNullException.ThrowIfNull(array);
        if ((nuint)start > (nuint)array.Length || (nuint)length > (nuint)(array.Length - start)) ThrowHelpers.ThrowOutOfRange();

        _first = ref Unsafe.Add(ref MemoryExtensions.GetBigArrayDataReference(array), start);
        _length = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlySpan{T}"/> struct that represents a contiguous region of memory.
    /// </summary>
    /// <param name="array">The array to represent.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    [OverloadResolutionPriority(-1)]
    public BigReadOnlySpan(T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        _first = ref MemoryMarshal.GetArrayDataReference(array);
        _length = array.Length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlySpan{T}"/> struct that represents a span of memory starting at the specified reference and with the specified length.
    /// </summary>
    /// <param name="array">The array to represent.</param>
    /// <param name="start">The starting index of the span within the array.</param>
    /// <param name="length">The number of elements in the span.</param>
    [OverloadResolutionPriority(-1)]
    public BigReadOnlySpan(T[] array, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(array);
        if ((nuint)start > (nuint)array.Length || (nuint)length > (nuint)(array.Length - start)) ThrowHelpers.ThrowOutOfRange();

        _first = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), start);
        _length = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BigReadOnlySpan{T}"/> struct that represents a span of memory starting at the specified pointer and with the specified length.
    /// </summary>
    /// <param name="pointer">A pointer to the first element in the span.</param>
    /// <param name="length">The number of elements in the span.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the length is negative.</exception>
#pragma warning disable CS8500
    public unsafe BigReadOnlySpan(T* pointer, nint length)
#pragma warning restore CS8500
    {
        if (length < 0) ThrowHelpers.ThrowOutOfRange(nameof(length));
        _first = ref Unsafe.AsRef<T>(pointer);
        _length = length;
    }

    internal BigReadOnlySpan(ref T first, nint length)
    {
        _first = ref first;
        _length = length;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="BigReadOnlySpan{T}"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="BigReadOnlySpan{T}"/>.</returns>
    public override string ToString()
    {
        return $"{nameof(BigReadOnlySpan<>)}<{typeof(T)}>[{_length}]";
    }

    /// <summary>
    /// Defines an implicit conversion from <see cref="ReadOnlySpan{T}"/> to <see cref="BigReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to convert.</param>
    public static implicit operator BigReadOnlySpan<T>(ReadOnlySpan<T> span)
    {
        return new BigReadOnlySpan<T>(ref MemoryMarshal.GetReference(span), span.Length);
    }

    /// <summary>
    /// Defines an implicit conversion from <see cref="Span{T}"/> to <see cref="BigReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">The <see cref="Span{T}"/> to convert.</param>
    public static implicit operator BigReadOnlySpan<T>(Span<T> span)
    {
        return new BigReadOnlySpan<T>(ref MemoryMarshal.GetReference(span), span.Length);
    }

#pragma warning disable CS0809
    /// <summary>
    /// This method is not supported and will throw a <see cref="NotSupportedException"/> if called.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>Does not return a value. Always throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown always.</exception>
    [Obsolete("Equals() on BigReadOnlySpan<T> is not supported. Use the equality operator instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals([NotNullWhen(true)] object? obj) => throw new NotSupportedException();

    /// <summary>
    /// This method is not supported and will throw a <see cref="NotSupportedException"/> if called.
    /// </summary>
    /// <returns>Does not return a value. Always throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown always.</exception>
    [Obsolete("GetHashCode() on BigReadOnlySpan<T> is not supported.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();
#pragma warning restore CS0809

    /// <summary>
    /// Compare two <see cref="BigReadOnlySpan{T}"/> instances for equality.
    /// </summary>
    /// <param name="left">The first <see cref="BigReadOnlySpan{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="BigReadOnlySpan{T}"/> to compare.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(BigReadOnlySpan<T> left, BigReadOnlySpan<T> right)
    {
        return left._length == right._length && Unsafe.AreSame(in left._first, in right._first);
    }

    /// <summary>
    /// Compare two <see cref="BigReadOnlySpan{T}"/> instances for inequality.
    /// </summary>
    /// <param name="left">The first <see cref="BigReadOnlySpan{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="BigReadOnlySpan{T}"/> to compare.</param>
    /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(BigReadOnlySpan<T> left, BigReadOnlySpan<T> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Creates a new <see cref="BigReadOnlySpan{T}"/> that represents a slice of the current span starting at the specified index.
    /// </summary>
    /// <param name="start">The index at which to start the slice.</param>
    /// <returns>A new <see cref="BigReadOnlySpan{T}"/> that represents the specified slice.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BigReadOnlySpan<T> Slice(nint start)
    {
        if ((nuint)start > (nuint)_length) ThrowHelpers.ThrowOutOfRange(nameof(start));

        return new BigReadOnlySpan<T>(ref Unsafe.Add(ref Unsafe.AsRef(in _first), start), _length - start);
    }

    /// <summary>
    /// Creates a new <see cref="BigReadOnlySpan{T}"/> that represents a slice of the current span starting at the specified index and with the specified length.
    /// </summary>
    /// <param name="start">The index at which to start the slice.</param>
    /// <param name="length">The number of elements in the slice.</param>
    /// <returns>A new <see cref="BigReadOnlySpan{T}"/> that represents the specified slice.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly BigReadOnlySpan<T> Slice(nint start, nint length)
    {
        if ((nuint)start > (nuint)_length || (nuint)length > (nuint)(_length - start)) ThrowHelpers.ThrowOutOfRange();

        return new BigReadOnlySpan<T>(ref Unsafe.Add(ref Unsafe.AsRef(in _first), start), length);
    }

    /// <summary>
    /// Gets a read-only reference to the element at the specified index in the span.
    /// </summary>
    /// <param name="index">The index of the element to retrieve.</param>
    /// <returns>A read-only reference to the element at the specified index.</returns>
    public readonly ref readonly T this[nint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ((nuint)index >= (nuint)_length) ThrowHelpers.ThrowOutOfRange(nameof(index));

            return ref Unsafe.Add(ref Unsafe.AsRef(in _first), index);
        }
    }

    /// <summary>
    /// Gets a read-only reference to the element that can be used for pinning.
    /// </summary>
    /// <returns>A read-only reference to the first element of the span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T GetPinnableReference()
    {
        return ref _first;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="BigReadOnlySpan{T}"/>.
    /// </summary>
    /// <returns>An enumerator that iterates through the <see cref="BigReadOnlySpan{T}"/>.</returns>
    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    /// <summary>
    /// Enumerates the elements of a <see cref="BigReadOnlySpan{T}"/>.
    /// </summary>
    public ref struct Enumerator : IEnumerator<T>
    {
        private readonly BigReadOnlySpan<T> _span;
        private nint _offset;
        internal Enumerator(BigReadOnlySpan<T> span)
        {
            _span = span;
            _offset = -1;
        }

        /// <inheritdoc/>
        public readonly ref readonly T Current => ref _span[_offset];

        readonly T IEnumerator<T>.Current => Current;
        readonly object? IEnumerator.Current => Current;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_offset < _span._length - 1)
            {
                _offset++;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public readonly void Dispose() { }

        /// <inheritdoc/>
        public void Reset()
        {
            _offset = -1;
        }
    }
}

/// <summary>
/// Enumerates the segments produced by splitting a <see cref="BigReadOnlySpan{T}"/>.
/// </summary>
/// <typeparam name="T">The type of elements in each segment.</typeparam>
public ref struct BigSpanSplitEnumerator<T>
{
    private BigReadOnlySpan<T> _remaining;
    private readonly T _separator;
    private readonly BigReadOnlySpan<T> _separators;
    private readonly byte _mode;
    private bool _finished;

    internal BigSpanSplitEnumerator(BigReadOnlySpan<T> span, T separator)
    {
        _remaining = span;
        _separator = separator;
        _separators = default;
        _mode = 0;
        _finished = false;
        Current = default;
    }

    internal BigSpanSplitEnumerator(BigReadOnlySpan<T> span, BigReadOnlySpan<T> separators)
    {
        _remaining = span;
        _separator = default!;
        _separators = separators;
        _mode = 1;
        _finished = false;
        Current = default;
    }

    /// <summary>
    /// Gets the segment at the current position of the enumerator.
    /// </summary>
    public BigReadOnlySpan<T> Current { get; private set; }

    /// <summary>
    /// Returns this enumerator.
    /// </summary>
    /// <returns>This enumerator.</returns>
    public readonly BigSpanSplitEnumerator<T> GetEnumerator()
    {
        return this;
    }

    /// <summary>
    /// Advances the enumerator to the next segment.
    /// </summary>
    /// <returns><see langword="true"/> if the enumerator was advanced; otherwise, <see langword="false"/>.</returns>
    public bool MoveNext()
    {
        if (_finished) return false;

        nint index = _mode switch
        {
            0 => _remaining.IndexOf(_separator),
            _ => _remaining.IndexOfAny(_separators)
        };

        if (index < 0)
        {
            Current = _remaining;
            _remaining = BigReadOnlySpan<T>.Empty;
            _finished = true;
            return true;
        }

        Current = _remaining.Slice(0, index);
        _remaining = _remaining.Slice(index + 1);
        return true;
    }
}

/// <summary>
/// Enumerates the segments produced by splitting a <see cref="BigReadOnlySpan{T}"/> with precomputed separator values.
/// </summary>
/// <typeparam name="T">The type of elements in each segment.</typeparam>
public ref struct BigSpanSearchValuesSplitEnumerator<T>
    where T : IEquatable<T>
{
    private BigReadOnlySpan<T> _remaining;
    private readonly SearchValues<T> _separators;
    private bool _finished;

    internal BigSpanSearchValuesSplitEnumerator(BigReadOnlySpan<T> span, SearchValues<T> separators)
    {
        _remaining = span;
        _separators = separators;
        _finished = false;
        Current = default;
    }

    /// <summary>
    /// Gets the segment at the current position of the enumerator.
    /// </summary>
    public BigReadOnlySpan<T> Current { get; private set; }

    /// <summary>
    /// Returns this enumerator.
    /// </summary>
    /// <returns>This enumerator.</returns>
    public readonly BigSpanSearchValuesSplitEnumerator<T> GetEnumerator()
    {
        return this;
    }

    /// <summary>
    /// Advances the enumerator to the next segment.
    /// </summary>
    /// <returns><see langword="true"/> if the enumerator was advanced; otherwise, <see langword="false"/>.</returns>
    public bool MoveNext()
    {
        if (_finished) return false;

        nint index = _remaining.IndexOfAny(_separators);
        if (index < 0)
        {
            Current = _remaining;
            _remaining = BigReadOnlySpan<T>.Empty;
            _finished = true;
            return true;
        }

        Current = _remaining.Slice(0, index);
        _remaining = _remaining.Slice(index + 1);
        return true;
    }
}
