using System.Collections.Immutable;

namespace Lynx.Model;

public static class PVTable
{
    public static readonly ImmutableArray<int> Indexes = Initialize();

    private static ImmutableArray<int> Initialize()
    {
        Span<int> indexes = stackalloc int[Configuration.EngineSettings.MaxDepth + Constants.ArrayDepthMargin];
        int previousPVIndex = 0;
        indexes[0] = previousPVIndex;

        for (int depth = 0; depth < indexes.Length - 1; ++depth)
        {
            indexes[depth + 1] = previousPVIndex + Configuration.EngineSettings.MaxDepth - depth;
            previousPVIndex = indexes[depth + 1];
        }

        return [.. indexes];
    }
}
