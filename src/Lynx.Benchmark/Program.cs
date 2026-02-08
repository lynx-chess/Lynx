using BenchmarkDotNet.Running;
using System.Reflection;
#if DEBUG
using BenchmarkDotNet.Configs;
#endif

//Lynx.Benchmark.Bitboard_Struct_ReadonlyStruct_Class_Record.SizeTest();

#if DEBUG
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args, new DebugInProcessConfig());
#else
BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
#endif
