using Lynx.Model;

namespace Lynx.Benchmark;

/// <summary>
/// Span views of <see cref="Position"/>'s unchecked pointers, for benchmarks whose implementations take spans
/// or that can't use pointers (iterators, async methods)
/// </summary>
internal static unsafe class PositionSpanExtensions
{
    public static Span<Bitboard> PieceBitboardsSpan(this Position position) => new(position.PieceBitboards, Position.PieceBitboardsLength);

    public static Span<Bitboard> OccupancyBitboardsSpan(this Position position) => new(position.OccupancyBitboards, Position.OccupancyBitboardsLength);

    public static Span<int> BoardSpan(this Position position) => new(position.Board, Position.BoardLength);
}
