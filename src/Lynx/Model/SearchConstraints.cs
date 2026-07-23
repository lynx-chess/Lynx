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

    public const ulong DefaultMaxNodes = 10_000_000_000_000_000_000;

    public const int DefaultMaxDepth = -1;

    public readonly int HardLimitTimeBound;

    public readonly int SoftLimitTimeBound;

    public readonly int MaxDepth;

    public readonly ulong MaxNodes;

    public static readonly SearchConstraints InfiniteSearchConstraint = new(DefaultHardLimitTimeBound, DefaultSoftLimitTimeBound, DefaultMaxDepth, DefaultMaxNodes);

    public SearchConstraints(int hardLimitTimeBound, int softLimitTimeBound, int maxDepth, ulong maxNodes)
    {
        HardLimitTimeBound = hardLimitTimeBound;
        SoftLimitTimeBound = softLimitTimeBound;
        MaxDepth = maxDepth;
        MaxNodes = maxNodes;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields