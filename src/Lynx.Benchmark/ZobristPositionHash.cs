using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;
public class ZobristPositionHash : BaseBenchmark
{
    public static IEnumerable<Position> Data => new[] {
        new Position(Constants.InitialPositionFEN),
        new Position(Constants.TrickyTestPositionFEN),
        new Position(Constants.TrickyTestPositionReversedFEN),
        new Position(Constants.CmkTestPositionFEN),
        new Position(Constants.ComplexPositionFEN),
        new Position(Constants.KillerTestPositionFEN),
    };

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public long Original(Position position) => PositionHash_Original_DoubleLoop(position);

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public long Improved(Position position) => PositionHash_Improved(position);

    private static long PositionHash_Original_DoubleLoop(Position position)
    {
        long positionHash = 0;

        for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
        {
            for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
            {
                if (position.PieceBitBoards[pieceIndex].GetBit(squareIndex))
                {
                    positionHash ^= ZobristTable.PieceHash(squareIndex, pieceIndex);
                }
            }
        }

        positionHash ^= ZobristTable.EnPassantHash((int)position.EnPassant)
            ^ ZobristTable.SideHash()
            ^ ZobristTable.CastleHash(position.Castle);

        return positionHash;
    }

    private static long PositionHash_Improved(Position position)
    {
        long positionHash = 0;

        for (int pieceIndex = 0; pieceIndex < 12; ++pieceIndex)
        {
            var bitboard = position.PieceBitBoards[pieceIndex];

            while (bitboard != default)
            {
                var pieceSquareIndex = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                positionHash ^= ZobristTable.PieceHash(pieceSquareIndex, pieceIndex);
            }
        }

        positionHash ^= ZobristTable.EnPassantHash((int)position.EnPassant)
            ^ ZobristTable.SideHash()
            ^ ZobristTable.CastleHash(position.Castle);

        return positionHash;
    }
}
