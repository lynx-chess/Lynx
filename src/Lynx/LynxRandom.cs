using System.Numerics;
using System.Runtime.CompilerServices;

namespace Lynx;

#pragma warning disable CA5394 // Do not use insecure randomness

public class LynxRandom : Random
{
    private readonly XoshiroImpl _impl;

    public LynxRandom()
    {
        _impl = new XoshiroImpl(this);
    }

    public LynxRandom(int seed) : base(seed)
    {
        _impl = new XoshiroImpl(this);
    }

    /// <summary>
    /// Based on dotnet/runtime implementation,
    /// https://github.com/dotnet/runtime/blob/508fef51e841aa16ffed1aae32bf4793a2cea363/src/libraries/System.Private.CoreLib/src/System/Random.Xoshiro256StarStarImpl.cs
    /// </summary>
    public ulong NextUInt64() => _impl.NextUInt64();

    /// <summary>
    /// Based on dotnet/runtime implementation,
    /// https://github.com/dotnet/runtime/blob/a7f96cb070ffb8adf266b2e09d26759d7f978a60/src/libraries/System.Private.CoreLib/src/System/Random.Xoshiro256StarStarImpl.cs
    /// <see cref="NextUInt64"/> is subsequently based on the algorithm from http://prng.di.unimi.it/xoshiro256starstar.c
    /// Written in 2018 by David Blackman and Sebastiano Vigna (vigna@acm.org)
    /// To the extent possible under law, the author has dedicated all copyright
    /// and related and neighboring rights to this software to the public domain
    /// worldwide. This software is distributed without any warranty.
    /// See <http://creativecommons.org/publicdomain/zero/1.0/>.
    /// </summary>
    internal sealed class XoshiroImpl
    {
        private ulong _s0, _s1, _s2, _s3;

        public XoshiroImpl(Random random)
        {
            do
            {
                _s0 = InternalNextUInt64();
                _s1 = InternalNextUInt64();
                _s2 = InternalNextUInt64();
                _s3 = InternalNextUInt64();
            }
            while ((_s0 | _s1 | _s2 | _s3) == 0); // at least one value must be non-zero

            // 'Naive' version of what we're trying to achieve
            ulong InternalNextUInt64()
            {
                Span<byte> arr = stackalloc byte[8];
                random.NextBytes(arr);

                return BitConverter.ToUInt64(arr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong NextUInt64()
        {
            ulong s0 = _s0, s1 = _s1, s2 = _s2, s3 = _s3;

            ulong result = BitOperations.RotateLeft(s1 * 5, 7) * 9;
            ulong t = s1 << 17;

            s2 ^= s0;
            s3 ^= s1;
            s1 ^= s2;
            s0 ^= s3;

            s2 ^= t;
            s3 = BitOperations.RotateLeft(s3, 45);

            _s0 = s0;
            _s1 = s1;
            _s2 = s2;
            _s3 = s3;

            return result;
        }
    }
}

#pragma warning restore CA5394 // Do not use insecure randomness
