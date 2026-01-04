using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

public class HistoryAging_Vectorization_Benchmark : BaseBenchmark
{
    /// <summary>
    /// 12 * 64 * 2 * 2 
    /// </summary>
    private const int QuietHistoryLength = 3_072;

    private readonly short[] _quietHistory = GC.AllocateArray<short>(QuietHistoryLength, pinned: true);

    [Benchmark(Baseline = true)]
    public void Naive()
    {
        for (int i = 0; i < QuietHistoryLength; ++i)
        {
            _quietHistory[i] = (short)(_quietHistory[i] * 3 / 4);
        }
    }

    [Benchmark]
    public void TemporaryVariable()
    {
        for (int i = 0; i < QuietHistoryLength; ++i)
        {
            int tmp = _quietHistory[i] * 3;
            _quietHistory[i] = (short)(tmp / 4);
        }
    }

    [Benchmark]
    public void ManuallyVectorized()
    {
        for (int i = 0; i < QuietHistoryLength; i += 4)
        {
            _quietHistory[i] = (short)(_quietHistory[i] * 3 / 4);
            _quietHistory[i + 1] = (short)(_quietHistory[i + 1] * 3 / 4);
            _quietHistory[i + 2] = (short)(_quietHistory[i + 2] * 3 / 4);
            _quietHistory[i + 3] = (short)(_quietHistory[i + 3] * 3 / 4);
        }
    }
}
