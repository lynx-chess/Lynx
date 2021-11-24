using System.Collections.Immutable;

namespace Lynx.Model;

public static class PVTable
{
    public static readonly ImmutableArray<int> Indexes = Initialize();

    private static ImmutableArray<int> Initialize()
    {
        var indexes = new int[Configuration.EngineSettings.MaxDepth + 1];
        int previousPVIndex = 0;
        indexes[0] = previousPVIndex;

        for (int depth = 0; depth < Configuration.EngineSettings.MaxDepth; ++depth)
        {
            indexes[depth + 1] = previousPVIndex + Configuration.EngineSettings.MaxDepth - depth;
            previousPVIndex = indexes[depth + 1];
        }

        return indexes.ToImmutableArray();
    }
}
