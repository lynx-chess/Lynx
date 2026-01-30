using System.Runtime.InteropServices;

namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

[StructLayout(LayoutKind.Sequential)]
public readonly struct SearchConstraints
{
    /// <summary>
    /// ~<see cref="int.MaxValue"/> / 200.
    /// Not <see cref="int.MaxValue"/> to avoid overflows in case we're ever attempt to scale it
    /// </summary>
    private const int DefaultTimeBound = 10_000_000;

    public const int DefaultHardLimitTimeBound  = DefaultTimeBound;

    public const int DefaultSoftLimitTimeBound  = DefaultTimeBound;

    public readonly int HardLimitTimeBound;

    public readonly int SoftLimitTimeBound;

    public readonly int MaxDepth;

    public static readonly SearchConstraints InfiniteSearchConstraint = new(DefaultHardLimitTimeBound, DefaultSoftLimitTimeBound, -1);

    public SearchConstraints(int hardLimitTimeBound, int softLimitTimeBound, int maxDepth)
    {
        HardLimitTimeBound = hardLimitTimeBound;
        SoftLimitTimeBound = softLimitTimeBound;
        MaxDepth = maxDepth;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields