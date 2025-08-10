using System.Runtime.CompilerServices;

namespace Lynx;

public static class MoveGeneratorWrapper
{
    public static MoveGeneratorBase MoveGenerator {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        private set; } = MoveGenerator_Standard.Instance;

    public static void UpdateCurrentInstance()
    {
        MoveGenerator = Configuration.EngineSettings.IsChess960
            ? MoveGenerator_DFRC.Instance
            : MoveGenerator_Standard.Instance;
    }
}
