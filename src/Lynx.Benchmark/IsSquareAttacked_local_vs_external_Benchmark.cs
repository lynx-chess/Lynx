/*
 * Inconclusive results regarding local/external methods
 *
 *  Just one position:
 *
 *  |          Method |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |---------------- |---------:|----------:|----------:|---------:|------:|--------:|------:|------:|------:|----------:|
 *  | ExternalMethods | 2.129 us | 0.1179 us | 0.3457 us | 2.074 us |  1.00 |    0.00 |     - |     - |     - |         - |
 *  |    LocalMethods | 1.973 us | 0.0932 us | 0.2733 us | 1.808 us |  0.95 |    0.19 |     - |     - |     - |         - |
 *
 *  |          Method |     Mean |     Error |    StdDev | Ratio | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |---------------- |---------:|----------:|----------:|------:|------:|------:|------:|----------:|
 *  |    LocalMethods | 1.600 us | 0.0150 us | 0.0133 us |  1.00 |     - |     - |     - |         - |
 *  | ExternalMethods | 1.553 us | 0.0146 us | 0.0137 us |  0.97 |     - |     - |     - |         - |
 *
 *  Four positions:
 *
 *  |          Method |     Mean |     Error |    StdDev | Ratio | RatioSD | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |---------------- |---------:|----------:|----------:|------:|--------:|------:|------:|------:|----------:|
 *  | ExternalMethods | 7.095 us | 0.1405 us | 0.3340 us |  1.00 |    0.00 |     - |     - |     - |         - |
 *  |    LocalMethods | 6.891 us | 0.0373 us | 0.0291 us |  0.96 |    0.05 |     - |     - |     - |         - |
 *
 *  |          Method |     Mean |     Error |    StdDev | Ratio | Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |---------------- |---------:|----------:|----------:|------:|------:|------:|------:|----------:|
 *  |    LocalMethods | 6.934 us | 0.0453 us | 0.0424 us |  1.00 |     - |     - |     - |         - |
 *  | ExternalMethods | 6.900 us | 0.0547 us | 0.0512 us |  1.00 |     - |     - |     - |         - |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class IsSquareAttacked_local_vs_external_Benchmark : BaseBenchmark
{
    private readonly Position[] _positions = new[]
    {
            new Position(Constants.InitialPositionFEN),
            new Position(Constants.TrickyTestPositionFEN),
            new Position("rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1"),
            new Position("r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9"),
        };

    [Benchmark(Baseline = true)]
    public bool LocalMethods()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (LocalMethodsImpl.IsSquaredAttacked_LocalMethods(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
    public bool ExternalMethods()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (ExternalMethodsImpl.IsSquaredAttacked_ExternalMethods(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    private static class ExternalMethodsImpl
    {
        public static bool IsSquaredAttacked_ExternalMethods(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var offset = Utils.PieceOffset(sideToMove);

            // I tried to order them from most to least likely
            return
                IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition)
                || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
                || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
                || IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
                || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
                || IsSquareAttackedByKing(squareIndex, offset, piecePosition);
        }

        private static bool IsSquareAttackedByPawns(int squareIndex, Side sideToMove, int offset, BitBoard[] pieces)
        {
            var oppositeColorIndex = ((int)sideToMove + 1) % 2;

            return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & pieces[offset]) != default;
        }

        private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (Attacks.KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
        }

        private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (Attacks.KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
        }

        private static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard bishopAttacks)
        {
            bishopAttacks = Attacks.BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (bishopAttacks & piecePosition[(int)Piece.B + offset]) != default;
        }

        private static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard rookAttacks)
        {
            rookAttacks = Attacks.RookAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (rookAttacks & piecePosition[(int)Piece.R + offset]) != default;
        }

        private static bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard[] piecePosition)
        {
            var queenAttacks = Attacks.QueenAttacks(rookAttacks, bishopAttacks);
            return (queenAttacks & piecePosition[(int)Piece.Q + offset]) != default;
        }
    }

    private static class LocalMethodsImpl
    {
        public static bool IsSquaredAttacked_LocalMethods(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var offset = Utils.PieceOffset(sideToMove);

            return
                IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition)
                || IsSquareAttackedByKnights(squareIndex, offset, piecePosition)
                || IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy, out var bishopAttacks)
                || IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy, out var rookAttacks)
                || IsSquareAttackedByQueens(offset, bishopAttacks, rookAttacks, piecePosition)
                || IsSquareAttackedByKing(squareIndex, offset, piecePosition);

            static bool IsSquareAttackedByPawns(int squareIndex, Side sideToMove, int offset, BitBoard[] pieces)
            {
                var oppositeColorIndex = ((int)sideToMove + 1) % 2;

                return (Attacks.PawnAttacks[oppositeColorIndex][squareIndex] & pieces[offset]) != default;
            }

            static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
            {
                return (Attacks.KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
            }

            static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
            {
                return (Attacks.KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
            }

            static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard bishopAttacks)
            {
                bishopAttacks = Attacks.BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
                return (bishopAttacks & piecePosition[(int)Piece.B + offset]) != default;
            }

            static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy, out BitBoard rookAttacks)
            {
                rookAttacks = Attacks.RookAttacks(squareIndex, occupancy[(int)Side.Both]);
                return (rookAttacks & piecePosition[(int)Piece.R + offset]) != default;
            }

            static bool IsSquareAttackedByQueens(int offset, BitBoard bishopAttacks, BitBoard rookAttacks, BitBoard[] piecePosition)
            {
                var queenAttacks = Attacks.QueenAttacks(rookAttacks, bishopAttacks);
                return (queenAttacks & piecePosition[(int)Piece.Q + offset]) != default;
            }
        }
    }
}
