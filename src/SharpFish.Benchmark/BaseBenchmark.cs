using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace SharpFish.Benchmark
{
    [MarkdownExporterAttribute.GitHub]
    [HtmlExporter]
    [MemoryDiagnoser]
    //[NativeMemoryProfiler]
    public class BaseBenchmark
    {
    }
}
