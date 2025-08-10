namespace Lynx;

public sealed class MoveGenerator_Standard : MoveGeneratorBase
{
#pragma warning disable CA1000 // Do not declare static members on generic types
    public static MoveGenerator_Standard Instance { get; } = new();
#pragma warning restore CA1000 // Do not declare static members on generic types

    /// <summary>
    /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
    /// https://csharpindepth.com/articles/singleton
    /// </summary>
#pragma warning disable S3253 // Constructor and destructor declarations should not be redundant
    static MoveGenerator_Standard() { }
#pragma warning restore S3253 // Constructor and destructor declarations should not be redundant

    private MoveGenerator_Standard() { }
}
