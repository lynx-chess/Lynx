// Setup based on https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/vectorization-guidelines.md#custom-config

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;
using System.Runtime.Intrinsics;

Job enough = Job.Default
    .WithWarmupCount(1)
    .WithIterationTime(TimeInterval.FromSeconds(0.25))
    .WithMaxIterationCount(20);

IConfig config = DefaultConfig.Instance
    .HideColumns(Column.EnvironmentVariables, Column.RatioSD, Column.Error)
    .AddJob(enough.WithEnvironmentVariable("DOTNET_EnableHWIntrinsic", "0").WithId("Scalar").AsBaseline());

if(OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
{
    config = config.AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig
        (exportGithubMarkdown: true, printInstructionAddresses: false)));
}

if (Vector512.IsHardwareAccelerated)
{
    config = config.AddJob(enough.WithId("Vector512"));
}

if (Vector256.IsHardwareAccelerated)
{
    // DOTNET_EnableAVX2=0 would not only affect Vector256.IsHardwareAccelerated but also disable stuff like BMI1's tzcnt
    config = config.AddJob(enough.WithEnvironmentVariable("DOTNET_PreferredVectorBitWidth", "256").WithId("Vector256"));
}

if (Vector128.IsHardwareAccelerated)
{
    config = config.AddJob(enough.WithEnvironmentVariable("DOTNET_PreferredVectorBitWidth", "128").WithId("Vector128"));
}

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
    .Run(args, config);