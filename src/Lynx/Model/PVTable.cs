namespace Lynx.Model
{
    public static class PVTable
    {
        public static readonly int[] Indexes;

        static PVTable()
        {
            Indexes = new int[Configuration.EngineSettings.MaxDepth];
            int previousPVIndex = 0;
            Indexes[0] = previousPVIndex;

            for (int depth = 0; depth < Configuration.EngineSettings.MaxDepth - 1; ++depth)
            {
                Indexes[depth + 1] = previousPVIndex + Configuration.EngineSettings.MaxDepth - depth;
                previousPVIndex = Indexes[depth + 1];
            }
        }
    }
}
