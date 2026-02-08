/*
 *
 *  |     Method |                  fen |     Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Allocated |
 *  |----------- |--------------------- |---------:|----------:|----------:|------:|--------:|-------:|----------:|
 *  | Dictionary | r2q1r(...)- 0 9 [68] | 8.000 us | 0.1077 us | 0.1007 us |  1.00 |    0.00 | 1.6632 |      3 KB |
 *  |      Array | r2q1r(...)- 0 9 [68] | 7.606 us | 0.0991 us | 0.0927 us |  0.95 |    0.02 | 1.6632 |      3 KB |
 *  |            |                      |          |           |           |       |         |        |           |
 *  | Dictionary | r3k2r(...)- 0 1 [68] | 7.858 us | 0.0937 us | 0.0831 us |  1.00 |    0.00 | 1.6327 |      3 KB |
 *  |      Array | r3k2r(...)- 0 1 [68] | 7.529 us | 0.0777 us | 0.0727 us |  0.96 |    0.01 | 1.6251 |      3 KB |
 *  |            |                      |          |           |           |       |         |        |           |
 *  | Dictionary | r3k2r(...)- 0 1 [68] | 7.906 us | 0.1525 us | 0.1816 us |  1.00 |    0.00 | 1.6174 |      3 KB |
 *  |      Array | r3k2r(...)- 0 1 [68] | 7.569 us | 0.0694 us | 0.0615 us |  0.96 |    0.03 | 1.6251 |      3 KB |
 *  |            |                      |          |           |           |       |         |        |           |
 *  | Dictionary | rnbqk(...)6 0 1 [67] | 7.731 us | 0.0771 us | 0.0722 us |  1.00 |    0.00 | 1.6174 |      3 KB |
 *  |      Array | rnbqk(...)6 0 1 [67] | 7.341 us | 0.0895 us | 0.0793 us |  0.95 |    0.01 | 1.6251 |      3 KB |
 *  |            |                      |          |           |           |       |         |        |           |
 *  | Dictionary | rnbqk(...)- 0 1 [56] | 6.523 us | 0.0918 us | 0.0859 us |  1.00 |    0.00 | 1.4725 |      3 KB |
 *  |      Array | rnbqk(...)- 0 1 [56] | 6.155 us | 0.0812 us | 0.0720 us |  0.94 |    0.01 | 1.4725 |      3 KB |
 *
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class MoveGeneratorArrayVsDictionary_Benchmark : BaseBenchmark
{
    public static IEnumerable<string> Data =>
        [
            Constants.InitialPositionFEN,
            Constants.TrickyTestPositionFEN,
            Constants.TrickyTestPositionReversedFEN,
            Constants.CmkTestPositionFEN,
            Constants.KillerTestPositionFEN
        ];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Data))]
    public ulong Dictionary(string fen)
    {
        ulong sum = 0;
        var position = new Position(fen);

        for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
        {
            var bitboard = position.PieceBitboards[piece];

            while (bitboard != default)
            {
                var sourceSquare = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                ulong attacks = _pieceAttacksDictionary[piece](sourceSquare, position.OccupancyBitboards[(int)Side.Both]);

                sum += attacks;
            }
        }

        return sum;
    }

    [Benchmark]
    [ArgumentsSource(nameof(Data))]
    public ulong Array(string fen)
    {
        ulong sum = 0;
        var position = new Position(fen);

        for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
        {
            var bitboard = position.PieceBitboards[piece];

            while (bitboard != default)
            {
                var sourceSquare = bitboard.GetLS1BIndex();
                bitboard.ResetLS1B();

                ulong attacks = _pieceAttacksArray[piece](sourceSquare, position.OccupancyBitboards[(int)Side.Both]);

                sum += attacks;
            }
        }

        return sum;
    }

#pragma warning disable IDE0350 // Use implicitly typed lambda
    private static readonly Func<int, Bitboard, ulong>[] _pieceAttacksArray =
    [
        (int origin, Bitboard _) => Attacks.PawnAttacks[(int)Side.White][origin],
        (int origin, Bitboard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, Bitboard _) => Attacks.KingAttacks[origin],

        (int origin, Bitboard _) => Attacks.PawnAttacks[(int)Side.Black][origin],
        (int origin, Bitboard _) => Attacks.KnightAttacks[origin],
        Attacks.BishopAttacks,
        Attacks.RookAttacks,
        Attacks.QueenAttacks,
        (int origin, Bitboard _) => Attacks.KingAttacks[origin],
    ];

    private static readonly Dictionary<int, Func<int, Bitboard, ulong>> _pieceAttacksDictionary = new()
    {
        [(int)Piece.P] = (int origin, Bitboard _) => Attacks.PawnAttacks[(int)Side.White][origin],
        [(int)Piece.p] = (int origin, Bitboard _) => Attacks.PawnAttacks[(int)Side.Black][origin],

        [(int)Piece.K] = (int origin, Bitboard _) => Attacks.KingAttacks[origin],
        [(int)Piece.k] = (int origin, Bitboard _) => Attacks.KingAttacks[origin],

        [(int)Piece.N] = (int origin, Bitboard _) => Attacks.KnightAttacks[origin],
        [(int)Piece.n] = (int origin, Bitboard _) => Attacks.KnightAttacks[origin],

        [(int)Piece.B] = Attacks.BishopAttacks,
        [(int)Piece.b] = Attacks.BishopAttacks,

        [(int)Piece.R] = Attacks.RookAttacks,
        [(int)Piece.r] = Attacks.RookAttacks,

        [(int)Piece.Q] = Attacks.QueenAttacks,
        [(int)Piece.q] = Attacks.QueenAttacks,
    };
#pragma warning restore IDE0350 // Use implicitly typed lambda
}
