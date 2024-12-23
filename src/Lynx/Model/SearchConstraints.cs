﻿namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public readonly struct SearchConstraints
{
    public const int DefaultHardLimitTimeBound  = int.MaxValue;
    public const int DefaultSoftLimitTimeBound  = int.MaxValue;

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