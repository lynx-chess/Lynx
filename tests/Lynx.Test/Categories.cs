namespace Lynx.Test;

public static class Categories
{
    public const string Perft = "Perft";

    public const string PerftFRC = "PerftFRC";

    public const string PerftFRCExhaustive = "PerftFRCExhaustive";

    public const string LongRunning = "LongRunning";

    /// <summary>
    /// Need to run in isolation, since other tests might modify <see cref="Configuration"/> values
    /// </summary>
    public const string Configuration = "Configuration";

    /// <summary>
    /// Can't be run since it'd take way too long for regular CI
    /// </summary>
    public const string TooLong = "TooLongToBeRun";

    /// <summary>
    /// Can't be run since no pruning is required
    /// </summary>
    public const string NoPruning = "RequireNoPruning";

    /// <summary>
    /// Can't be run since our engine is simply not good enough yet
    /// </summary>
    public const string NotGoodEnough = "NotGoodEnough";
}
