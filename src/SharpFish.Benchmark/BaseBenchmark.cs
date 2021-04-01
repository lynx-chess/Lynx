using BenchmarkDotNet.Attributes;

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
