using BenchmarkDotNet.Attributes;
using EnvDTE;
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
