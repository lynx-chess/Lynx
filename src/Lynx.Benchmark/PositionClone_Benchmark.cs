/*
 * ParseFEN is slowe, and ArrayCopy seems slighly faster than the manual approach
 *
 *  |                Method |                  fen |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |---------------------- |--------------------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
 *  |              ParseFEN | /8/8/(...)- 0 1 [24] |  7.541 us | 0.0810 us | 0.0677 us |  7.523 us |  1.00 |    0.00 | 2.9831 |     - |     - |   6.12 KB |
 *  |           ManualClone | /8/8/(...)- 0 1 [24] |  3.992 us | 0.0588 us | 0.0521 us |  3.990 us |  0.53 |    0.01 | 1.6174 |     - |     - |   3.32 KB |
 *  | ManualClone_ArrayCopy | /8/8/(...)- 0 1 [24] |  3.842 us | 0.0517 us | 0.0403 us |  3.834 us |  0.51 |    0.01 | 1.5717 |     - |     - |   3.22 KB |
 *  |                       |                      |           |           |           |           |       |         |        |       |       |           |
 *  |              ParseFEN | r2q1r(...) 0 9  [69] | 13.408 us | 0.1234 us | 0.1154 us | 13.375 us |  1.00 |    0.00 | 4.1504 |     - |     - |    8.5 KB |
 *  |           ManualClone | r2q1r(...) 0 9  [69] |  6.853 us | 0.0464 us | 0.0411 us |  6.840 us |  0.51 |    0.01 | 2.2049 |     - |     - |   4.51 KB |
 *  | ManualClone_ArrayCopy | r2q1r(...) 0 9  [69] |  6.667 us | 0.0543 us | 0.0481 us |  6.670 us |  0.50 |    0.00 | 2.1591 |     - |     - |   4.41 KB |
 *  |                       |                      |           |           |           |           |       |         |        |       |       |           |
 *  |              ParseFEN | r3k2r(...)- 0 1 [68] | 17.012 us | 2.0290 us | 5.9825 us | 13.019 us |  1.00 |    0.00 | 3.9368 |     - |     - |   8.04 KB |
 *  |           ManualClone | r3k2r(...)- 0 1 [68] |  6.702 us | 0.0773 us | 0.0645 us |  6.704 us |  0.30 |    0.07 | 2.0828 |     - |     - |   4.28 KB |
 *  | ManualClone_ArrayCopy | r3k2r(...)- 0 1 [68] |  6.456 us | 0.0588 us | 0.0521 us |  6.449 us |  0.29 |    0.07 | 2.0447 |     - |     - |   4.19 KB |
 *  |                       |                      |           |           |           |           |       |         |        |       |       |           |
 *  |              ParseFEN | r3k2r(...)- 0 1 [68] | 13.423 us | 0.1218 us | 0.1017 us | 13.449 us |  1.00 |    0.00 | 3.9368 |     - |     - |   8.04 KB |
 *  |           ManualClone | r3k2r(...)- 0 1 [68] |  6.950 us | 0.0487 us | 0.0407 us |  6.963 us |  0.52 |    0.01 | 2.0905 |     - |     - |   4.28 KB |
 *  | ManualClone_ArrayCopy | r3k2r(...)- 0 1 [68] |  6.455 us | 0.0664 us | 0.0589 us |  6.446 us |  0.48 |    0.00 | 2.0447 |     - |     - |   4.19 KB |
 *  |                       |                      |           |           |           |           |       |         |        |       |       |           |
 *  |              ParseFEN | rnbqk(...)6 0 1 [67] | 12.410 us | 0.1686 us | 0.1408 us | 12.413 us |  1.00 |    0.00 | 3.8757 |     - |     - |   7.94 KB |
 *  |           ManualClone | rnbqk(...)6 0 1 [67] |  6.410 us | 0.1225 us | 0.1086 us |  6.368 us |  0.52 |    0.01 | 2.0676 |     - |     - |   4.23 KB |
 *  | ManualClone_ArrayCopy | rnbqk(...)6 0 1 [67] |  6.231 us | 0.0558 us | 0.0495 us |  6.229 us |  0.50 |    0.01 | 2.0218 |     - |     - |   4.13 KB |
 *  |                       |                      |           |           |           |           |       |         |        |       |       |           |
 *  |              ParseFEN | rnbqk(...)- 0 1 [56] |  9.439 us | 0.0828 us | 0.0734 us |  9.432 us |  1.00 |    0.00 | 3.0670 |     - |     - |   6.29 KB |
 *  |           ManualClone | rnbqk(...)- 0 1 [56] |  4.978 us | 0.0434 us | 0.0385 us |  4.978 us |  0.53 |    0.01 | 1.6632 |     - |     - |    3.4 KB |
 *  | ManualClone_ArrayCopy | rnbqk(...)- 0 1 [56] |  4.821 us | 0.0462 us | 0.0454 us |  4.810 us |  0.51 |    0.01 | 1.6174 |     - |     - |   3.31 KB |
 *
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

file readonly struct Position
{
    public string FEN { get; }

    /// <summary>
    /// Use <see cref="Piece"/> as index
    /// </summary>
    public BitBoard[] PieceBitBoards { get; }

    /// <summary>
    /// Black, White, Both
    /// </summary>
    public BitBoard[] OccupancyBitBoards { get; }

    public Side Side { get; }

    public BoardSquare EnPassant { get; }

    public CastlingRights Castle { get; }

    public Position(string fen)
    {
        FEN = fen;
        var parsedFEN = FENParser.ParseFEN(fen);

        PieceBitBoards = parsedFEN.PieceBitBoards;
        OccupancyBitBoards = parsedFEN.OccupancyBitBoards;
        Side = parsedFEN.Side;
        Castle = parsedFEN.Castle;
        EnPassant = parsedFEN.EnPassant;
    }

    /// <summary>
    /// 'Pasing FEN' Clone constructor
    /// </summary>
    public Position(Position position) : this(position.FEN)
    { }

    /// <summary>
    /// 'Manual' Clone constructor
    /// </summary>
    public Position(Position position, int _)
    {
        FEN = position.FEN;
        PieceBitBoards = position.PieceBitBoards;

        OccupancyBitBoards = position.OccupancyBitBoards;

        Side = position.Side;
        Castle = position.Castle;
        EnPassant = position.EnPassant;
    }

    /// <summary>
    /// 'Manual' Clone constructor using Array.Copy
    /// </summary>
    public Position(Position position, string _)
    {
        FEN = position.FEN;

        PieceBitBoards = new BitBoard[12];
        Array.Copy(position.PieceBitBoards, PieceBitBoards, position.PieceBitBoards.Length);

        OccupancyBitBoards = new BitBoard[3];
        Array.Copy(position.OccupancyBitBoards, OccupancyBitBoards, position.OccupancyBitBoards.Length);

        Side = position.Side;
        Castle = position.Castle;
        EnPassant = position.EnPassant;
    }
}

public class PositionClone_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data =>
        [
            Constants.EmptyBoardFEN,
            Constants.InitialPositionFEN,
            Constants.TrickyTestPositionFEN,
            "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQkq - 0 1",
            "rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1",
            "r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9 "
        ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public void ParseFEN(string fen)
    {
        var originalPosition = new Position(fen);
        _ = new Position(originalPosition);
    }

    /// <summary>
    /// ~2x faster and 0.5 less memory allocated
    /// </summary>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ManualClone(string fen)
    {
        var originalPosition = new Position(fen);
        _ = new Position(originalPosition, 0);
    }

    /// <summary>
    /// ~2x faster and 0.5 less memory allocated
    /// </summary>
    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public void ManualClone_ArrayCopy(string fen)
    {
        var originalPosition = new Position(fen);
        _ = new Position(originalPosition, "");
    }
}
