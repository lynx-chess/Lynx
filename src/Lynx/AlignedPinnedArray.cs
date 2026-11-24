using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lynx;

internal readonly unsafe struct AlignedPinnedArray<T>
    where T : unmanaged
{
    private const int Alignment = 64;

#pragma warning disable S1450 // Field could be a local - Backs up the pointer
    private readonly T[] _array;
#pragma warning restore S1450

#pragma warning disable CA1051 // Do not declare visible instance fields

    public readonly T* Pointer;

    public readonly int Length;

#pragma warning restore CA1051 // Do not declare visible instance fields

    public AlignedPinnedArray(int length)
    {
        var extraElements = (Alignment + sizeof(T) - 1) / sizeof(T);
        _array = GC.AllocateArray<T>(length + extraElements, pinned: true);

        var start = (nuint)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(_array));
        Pointer = (T*)((start + (Alignment - 1)) & ~(nuint)(Alignment - 1));
        Length = length;
    }

    public void Clear() => new Span<T>(Pointer, Length).Clear();
}
