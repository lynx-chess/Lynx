using BenchmarkDotNet.Attributes;

namespace Lynx.Benchmark;

[MarkdownExporterAttribute.GitHub]
[HtmlExporter]
[MemoryDiagnoser]
//[NativeMemoryProfiler]
public class BaseBenchmark;
