using Lynx.Model;

namespace Lynx;

public sealed class MoveGenerator_DFRC : MoveGenerator
{
#pragma warning disable CA1000 // Do not declare static members on generic types
    public new static MoveGenerator_DFRC Instance { get; } = new();
#pragma warning restore CA1000 // Do not declare static members on generic types

    /// <summary>
    /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
    /// https://csharpindepth.com/articles/singleton
    /// </summary>
#pragma warning disable S3253 // Constructor and destructor declarations should not be redundant
    static MoveGenerator_DFRC() { }
#pragma warning restore S3253 // Constructor and destructor declarations should not be redundant

    private MoveGenerator_DFRC() { }

    /// <inheritdoc/>
    internal override void GenerateCastlingMoves(ref int localIndex, Span<int> movePool, Position position)
    {
        base.GenerateCastlingMoves(ref localIndex, movePool, position);
    }

    /// <inheritdoc/>
    protected override bool IsAnyCastlingMoveValid(Position position)
    {
        return base.IsAnyCastlingMoveValid(position);
    }
}
