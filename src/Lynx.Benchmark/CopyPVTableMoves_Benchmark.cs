/*
 *  BenchmarkDotNet v0.15.3, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
 *  AMD EPYC 7763 2.45GHz, 1 CPU, 4 logical and 2 physical cores
 *  .NET SDK 9.0.305
 *    [Host]     : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *    DefaultJob : .NET 9.0.9 (9.0.9, 9.0.925.41916), X64 RyuJIT x86-64-v3
 *
 *  | Method                          | plyDepth | Mean       | Error    | StdDev  | Ratio | RatioSD | Allocated | Alloc Ratio |
 *  |-------------------------------- |--------- |-----------:|---------:|--------:|------:|--------:|----------:|------------:|
 *  | CopyPVTableMoves_Original       | 1        |   429.2 ns |  2.00 ns | 1.77 ns |  1.00 |    0.01 |         - |          NA |
 *  | CopyPVTableMoves_Pointers       | 1        | 4,442.1 ns |  4.29 ns | 3.35 ns | 10.35 |    0.04 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Original | 1        |   428.0 ns |  0.21 ns | 0.18 ns |  1.00 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Pointers | 1        | 5,725.8 ns | 10.58 ns | 8.83 ns | 13.34 |    0.06 |         - |          NA |
 *  |                                 |          |            |          |         |       |         |           |             |
 *  | CopyPVTableMoves_Original       | 5        |   400.7 ns |  0.22 ns | 0.21 ns |  1.00 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Pointers       | 5        | 4,213.9 ns |  4.06 ns | 3.39 ns | 10.52 |    0.01 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Original | 5        |   400.1 ns |  0.14 ns | 0.11 ns |  1.00 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Pointers | 5        | 5,409.1 ns |  8.39 ns | 7.01 ns | 13.50 |    0.02 |         - |          NA |
 *  |                                 |          |            |          |         |       |         |           |             |
 *  | CopyPVTableMoves_Original       | 10       |   372.8 ns |  0.20 ns | 0.17 ns |  1.00 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Pointers       | 10       | 3,912.9 ns |  1.98 ns | 1.65 ns | 10.50 |    0.01 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Original | 10       |   372.0 ns |  0.29 ns | 0.26 ns |  1.00 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Pointers | 10       | 5,024.0 ns |  4.64 ns | 3.62 ns | 13.48 |    0.01 |         - |          NA |
 *  |                                 |          |            |          |         |       |         |           |             |
 *  | CopyPVTableMoves_Original       | 20       |   319.4 ns |  0.17 ns | 0.14 ns |  1.00 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Pointers       | 20       | 3,353.0 ns |  3.01 ns | 2.51 ns | 10.50 |    0.01 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Original | 20       |   322.3 ns |  0.28 ns | 0.26 ns |  1.01 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Pointers | 20       | 4,309.3 ns |  3.24 ns | 2.71 ns | 13.49 |    0.01 |         - |          NA |
 *  |                                 |          |            |          |         |       |         |           |             |
 *  | CopyPVTableMoves_Original       | 30       |   269.0 ns |  0.17 ns | 0.14 ns |  1.00 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Pointers       | 30       | 2,850.0 ns |  2.34 ns | 2.08 ns | 10.59 |    0.01 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Original | 30       |   273.5 ns |  0.25 ns | 0.21 ns |  1.02 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Pointers | 30       | 3,667.9 ns |  5.31 ns | 4.96 ns | 13.64 |    0.02 |         - |          NA |
 *  |                                 |          |            |          |         |       |         |           |             |
 *  | CopyPVTableMoves_Original       | 40       |   229.5 ns |  0.14 ns | 0.11 ns |  1.00 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Pointers       | 40       | 2,398.2 ns |  0.88 ns | 0.69 ns | 10.45 |    0.01 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Original | 40       |   231.1 ns |  0.39 ns | 0.36 ns |  1.01 |    0.00 |         - |          NA |
 *  | CopyPVTableMoves_Clear_Pointers | 40       | 3,094.2 ns |  5.44 ns | 5.09 ns | 13.48 |    0.02 |         - |          NA |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;
using System.Runtime.CompilerServices;

namespace Lynx.Benchmark;

public class CopyPVTableMoves_Benchmark : BaseBenchmark
{
    private CopyPVTableMoves_Engine _engine = null!;

    public static IEnumerable<int> PlyDepths => [1, 5, 10, 20, 30, 40];

    [GlobalSetup]
    public void Setup()
    {
        _engine = new CopyPVTableMoves_Engine();

        for (int i = 0; i < _engine.PVTable.Length; ++i)
        {
            _engine.PVTable[i] = default;
        }
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(PlyDepths))]
    public void CopyPVTableMoves_Original(int plyDepth)
    {
        int source = PVTable.Indexes[plyDepth - 1];
        int target = PVTable.Indexes[plyDepth];
        int moveCountToCopy = Configuration.EngineSettings.MaxDepth - plyDepth - 1;

        _engine.CopyPVTableMoves_Original(target, source, moveCountToCopy);
    }

    [Benchmark]
    [ArgumentsSource(nameof(PlyDepths))]
    public void CopyPVTableMoves_Pointers(int plyDepth)
    {
        int source = PVTable.Indexes[plyDepth - 1];
        int target = PVTable.Indexes[plyDepth];
        int moveCountToCopy = Configuration.EngineSettings.MaxDepth - plyDepth - 1;

        _engine.CopyPVTableMoves_Pointers(target, source, moveCountToCopy);
    }

    [Benchmark]
    [ArgumentsSource(nameof(PlyDepths))]
    public void CopyPVTableMoves_Clear_Original(int plyDepth)
    {
        int source = PVTable.Indexes[plyDepth - 1];
        int target = PVTable.Indexes[plyDepth];
        int moveCountToCopy = Configuration.EngineSettings.MaxDepth - plyDepth - 1;

        _engine.PVTable[source] = 0;

        _engine.CopyPVTableMoves_Original(target, source, moveCountToCopy);
    }

    [Benchmark]
    [ArgumentsSource(nameof(PlyDepths))]
    public void CopyPVTableMoves_Clear_Pointers(int plyDepth)
    {
        int source = PVTable.Indexes[plyDepth - 1];
        int target = PVTable.Indexes[plyDepth];
        int moveCountToCopy = Configuration.EngineSettings.MaxDepth - plyDepth - 1;

        _engine.PVTable[source] = 0;

        _engine.CopyPVTableMoves_Pointers(target, source, moveCountToCopy);
    }
}

sealed class CopyPVTableMoves_Engine
{
    public readonly Move[] PVTable = GC.AllocateArray<Move>(Configuration.EngineSettings.MaxDepth * (Configuration.EngineSettings.MaxDepth + 1 + Constants.ArrayDepthMargin) / 2, pinned: true);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyPVTableMoves_Original(int target, int source, int moveCountToCopy)
    {
        if (PVTable[source] == default)
        {
            Array.Clear(PVTable, target, PVTable.Length - target);
            return;
        }

        //PrintPvTable(target: target, source: source, movesToCopy: moveCountToCopy);
        Array.Copy(PVTable, source, PVTable, target, moveCountToCopy);
        //PrintPvTable();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void CopyPVTableMoves_Pointers(int target, int source, int moveCountToCopy)
    {
        fixed (Move* pvTablePtr = PVTable)
        {
            if (pvTablePtr[source] == default)
            {
                // Clear the rest of the PV table using pointer math
                for (int i = target; i < PVTable.Length; ++i)
                {
                    pvTablePtr[i] = default;
                }
                return;
            }

            //PrintPvTable(target: target, source: source, movesToCopy: moveCountToCopy);

            // Copy using pointer math
            for (int i = 0; i < moveCountToCopy; ++i)
            {
                pvTablePtr[target + i] = pvTablePtr[source + i];
            }

            //PrintPvTable();
        }
    }
}
