using System.Runtime.InteropServices;

namespace Hezium.Memory;

/// <summary>
/// Provides a builder for creating instances of <see cref="BigArray{T}"/>.
/// </summary>
public static class CollectionBuilders
{
    /// <summary>
    /// Creates a <see cref="BigArray{T}"/> from a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="values">The span of values to create the array from.</param>
    /// <returns>A <see cref="BigArray{T}"/> containing the elements from the span.</returns>
    public static BigArray<T> CreateBigArray<T>(ReadOnlySpan<T> values)
    {
        var array = new BigArray<T>(values.Length);
        values.CopyTo(array.AsSpan(0, values.Length));
        return array;
    }

    /// <summary>
    /// Creates a <see cref="BigSpan{T}"/> from a <see cref="Span{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="values">The span of values to create the BigSpan from.</param>
    /// <returns>A <see cref="BigSpan{T}"/> containing the elements from the span.</returns>
    public static BigSpan<T> CreateBigSpan<T>(ReadOnlySpan<T> values)
    {
        return new BigSpan<T>(ref MemoryMarshal.GetReference(values), values.Length);
    }

    /// <summary>
    /// Creates a <see cref="BigReadOnlySpan{T}"/> from a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="values">The span of values to create the BigReadOnlySpan from.</param>
    /// <returns>A <see cref="BigReadOnlySpan{T}"/> containing the elements from the span.</returns>
    public static BigReadOnlySpan<T> CreateBigReadOnlySpan<T>(ReadOnlySpan<T> values)
    {
        return new BigReadOnlySpan<T>(ref MemoryMarshal.GetReference(values), values.Length);
    }
}
