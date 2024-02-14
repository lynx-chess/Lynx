using System.Runtime.CompilerServices;

namespace Lynx;
public static class PerformanceUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] AllocatePinnedArray<T>(int length) => GC.AllocateArray<T>(length, pinned: true);
}
