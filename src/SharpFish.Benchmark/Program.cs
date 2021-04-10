using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Reflection;

//SharpFish.Benchmark.BitBoard_Struct_ReadonlyStruct_Class_Record.SizeTest();

#if DEBUG
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args, new DebugInProcessConfig());
#else
            BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
#endif
