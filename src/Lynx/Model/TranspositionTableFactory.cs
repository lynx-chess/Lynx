using NLog;

namespace Lynx.Model;

public static class TranspositionTableFactory
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static ITranspositionTable Create()
    {
        if (Configuration.EngineSettings.UseMultiArrayTT)
        {
            _logger.Info("Using multi TT array transposition table");

            return new MultiArrayTranspositionTable();
        }

        return new SingleArrayTranspositionTable();
    }
}
