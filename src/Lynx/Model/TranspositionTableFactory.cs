namespace Lynx.Model;

public static class TranspositionTableFactory
{
    public static ITranspositionTable Create()
    {
        var hashBytes = (ulong)Configuration.EngineSettings.TranspositionTableSize * 1024 * 1024;
        var ttEntries = hashBytes / TranspositionTableElement.Size;

        if (ttEntries <= (ulong)Constants.MaxTTArrayLength)
        {
            return new SingleArrayTranspositionTable();
        }

        return new MultiArrayTranspositionTable();
    }
}
