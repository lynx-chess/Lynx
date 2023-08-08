using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class MakeMove : BaseBenchmark
{
    public static IEnumerable<(Position, int)> Data => new[] {
            //(new Position(Constants.InitialPositionFEN), 4),
            (new Position(Constants.TrickyTestPositionFEN), 4)
        };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public long NewPosition((Position Position, int Depth) data) => Perft.ResultsImpl(data.Position, data.Depth, default);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long MakeUnmakeMove((Position Position, int Depth) data) => Perft.ResultsImplUnmakeMove(data.Position, data.Depth, default);
}
