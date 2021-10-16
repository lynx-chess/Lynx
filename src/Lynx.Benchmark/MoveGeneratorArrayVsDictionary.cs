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
using System;
using System.Collections.Generic;

namespace Lynx.Benchmark
{
    public class MoveGeneratorArrayVsDictionary : BaseBenchmark
    {
        public static IEnumerable<string> Data => new[]
        {
            Constants.InitialPositionFEN,
            Constants.TrickyTestPositionFEN,
            Constants.TrickyTestPositionReversedFEN,
            Constants.CmkTestPositionFEN,
            Constants.KillerTestPositionFEN
        };

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public ulong Dictionary(string fen)
        {
            ulong sum = 0;
            var position = new Position(fen);

            for (int piece = (int)Piece.P; piece <= (int)Piece.k; ++piece)
            {
                var bitboard = position.PieceBitBoards[piece].Board;

                while (bitboard != default)
                {
                    var sourceSquare = BitBoard.GetLS1BIndex(bitboard);
                    bitboard = BitBoard.ResetLS1B(bitboard);

                    ulong attacks = _pieceAttacksDictionary[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both]);

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
                var bitboard = position.PieceBitBoards[piece].Board;

                while (bitboard != default)
                {
                    var sourceSquare = BitBoard.GetLS1BIndex(bitboard);
                    bitboard = BitBoard.ResetLS1B(bitboard);

                    ulong attacks = _pieceAttacksArray[piece](sourceSquare, position.OccupancyBitBoards[(int)Side.Both]);

                    sum += attacks;
                }
            }

            return sum;
        }

        private static readonly Func<int, BitBoard, ulong>[] _pieceAttacksArray = new Func<int, BitBoard, ulong>[]
        {
            (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White, origin].Board,
            (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,
            (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
            (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,

            (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black, origin].Board,
            (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,
            (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,
            (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
            (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,
        };

        private static readonly IReadOnlyDictionary<int, Func<int, BitBoard, ulong>> _pieceAttacksDictionary = new Dictionary<int, Func<int, BitBoard, ulong>>
        {
            [(int)Piece.P] = (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.White, origin].Board,
            [(int)Piece.p] = (int origin, BitBoard _) => Attacks.PawnAttacks[(int)Side.Black, origin].Board,

            [(int)Piece.K] = (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,
            [(int)Piece.k] = (int origin, BitBoard _) => Attacks.KingAttacks[origin].Board,

            [(int)Piece.N] = (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,
            [(int)Piece.n] = (int origin, BitBoard _) => Attacks.KnightAttacks[origin].Board,

            [(int)Piece.B] = (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,
            [(int)Piece.b] = (int origin, BitBoard occupancy) => Attacks.BishopAttacks(origin, occupancy).Board,

            [(int)Piece.R] = (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,
            [(int)Piece.r] = (int origin, BitBoard occupancy) => Attacks.RookAttacks(origin, occupancy).Board,

            [(int)Piece.Q] = (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
            [(int)Piece.q] = (int origin, BitBoard occupancy) => Attacks.QueenAttacks(origin, occupancy).Board,
        };
    }
}
