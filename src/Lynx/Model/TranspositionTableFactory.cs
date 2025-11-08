using NLog;

namespace Lynx.Model;

public static class TranspositionTableFactory
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static ITranspositionTable Create()
    {
        if (Configuration.EngineSettings.TranspositionTableSize <= Constants.SingleTTArrayAbsoluteMaxTTSize)
        {
            return new SingleArrayTranspositionTable();
        }

        _logger.Info("Using multi TT array transposition table, since Hash is greater than {0} MB", Constants.SingleTTArrayAbsoluteMaxTTSize);

        return new MultiArrayTranspositionTable();
    }
}
