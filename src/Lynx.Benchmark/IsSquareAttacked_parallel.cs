/*
 * :d
 *
 *  |       Method |         Mean |      Error |     StdDev |    Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |------------- |-------------:|-----------:|-----------:|---------:|------:|------:|----------:|
 *  | SingleThread |     6.957 us |  0.0614 us |  0.0512 us |        - |     - |     - |         - |
 *  |      WhenAll |   975.977 us | 13.0518 us | 10.8989 us | 160.1563 |     - |     - |  330906 B |
 *  |      WhenAny | 1,948.678 us |  9.0861 us |  8.4992 us | 394.5313 |     - |     - |  825760 B |
 *
 *  With the Queen attacks lines commented, just in case
 *
 *  |       Method |         Mean |      Error |    StdDev |    Gen 0 | Gen 1 | Gen 2 | Allocated |
 *  |------------- |-------------:|-----------:|----------:|---------:|------:|------:|----------:|
 *  | SingleThread |     6.192 us |  0.0513 us | 0.0480 us |        - |     - |     - |         - |
 *  |      WhenAll |   868.975 us |  4.6320 us | 4.1061 us | 140.6250 |     - |     - |  291841 B |
 *  |      WhenAny | 1,625.027 us | 10.1616 us | 9.5051 us | 339.8438 |     - |     - |  704981 B |
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class IsSquareAttacked_parallel : BaseBenchmark
{
    private readonly Position[] _positions = new[]
    {
            new Position(Constants.InitialPositionFEN),
            new Position(Constants.TrickyTestPositionFEN),
            new Position("rnbqkb1r/pp1p1pPp/8/2p1pP2/1P1P4/3P3P/P1P1P3/RNBQKBNR w KQkq e6 0 1"),
            new Position("r2q1rk1/ppp2ppp/2n1bn2/2b1p3/3pP3/3P1NPP/PPP1NPB1/R1BQ1RK1 b - - 0 9"),
        };

    [Benchmark]
    public bool SingleThread()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (SingleThreadImpl.IsSquaredAttacked_ExternalMethods(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
    public async Task<bool> WhenAll()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (await WhenAllImpl.IsSquaredAttacked_WhenAll(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    [Benchmark]
    public async Task<bool> WhenAny()
    {
        var b = false;
        foreach (var position in _positions)
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                if (await WhenAnyImpl.IsSquaredAttacked_WhenAny(squareIndex, position.Side, position.PieceBitBoards, position.OccupancyBitBoards))
                {
                    b = true;
                }
            }
        }

        return b;
    }

    private static class SingleThreadImpl
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

            return (Attacks.PawnAttacks[oppositeColorIndex, squareIndex] & pieces[offset]) != default;
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

    private static class WhenAllImpl
    {
        public static async Task<bool> IsSquaredAttacked_WhenAll(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var offset = Utils.PieceOffset(sideToMove);

            var tasks = new Task<bool>[]
            {
                    Task.Run(()=>IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition)),
                    Task.Run(()=>IsSquareAttackedByKnights(squareIndex, offset, piecePosition)),
                    Task.Run(()=>IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy)),
                    Task.Run(()=>IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy)),
                    Task.Run(()=>IsSquareAttackedByQueens(squareIndex, offset, piecePosition, occupancy)),
                    Task.Run(()=>IsSquareAttackedByKing(squareIndex, offset, piecePosition))
            };

            return (await Task.WhenAll(tasks)).Any(boolean => boolean);
        }

        private static bool IsSquareAttackedByPawns(int squareIndex, Side sideToMove, int offset, BitBoard[] pieces)
        {
            var oppositeColorIndex = ((int)sideToMove + 1) % 2;

            return (Attacks.PawnAttacks[oppositeColorIndex, squareIndex] & pieces[offset]) != default;
        }

        private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (Attacks.KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
        }

        private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (Attacks.KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
        }

        private static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var bishopAttacks = Attacks.BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (bishopAttacks & piecePosition[(int)Piece.B + offset]) != default;
        }

        private static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var rookAttacks = Attacks.RookAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (rookAttacks & piecePosition[(int)Piece.R + offset]) != default;
        }

        private static bool IsSquareAttackedByQueens(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var queenAttacks = Attacks.QueenAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (queenAttacks & piecePosition[(int)Piece.Q + offset]) != default;
        }
    }

    private static class WhenAnyImpl
    {
        public static async Task<bool> IsSquaredAttacked_WhenAny(int squareIndex, Side sideToMove, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var offset = Utils.PieceOffset(sideToMove);

            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            var tasks = new List<Task<bool>>
                {
                    Task.Run(()=>IsSquareAttackedByPawns(squareIndex, sideToMove, offset, piecePosition), token),
                    Task.Run(()=>IsSquareAttackedByKnights(squareIndex, offset, piecePosition), token),
                    Task.Run(()=>IsSquareAttackedByBishops(squareIndex, offset, piecePosition, occupancy), token),
                    Task.Run(()=>IsSquareAttackedByRooks(squareIndex, offset, piecePosition, occupancy), token),
                    Task.Run(()=>IsSquareAttackedByQueens(squareIndex, offset, piecePosition, occupancy), token),
                    Task.Run(()=>IsSquareAttackedByKing(squareIndex, offset, piecePosition), token)
                };

            while (tasks.Count > 0)
            {
                var resultTask = await Task.WhenAny(tasks);
                tasks.Remove(resultTask);

                var result = await resultTask;
                if (result)
                {
                    cancellationTokenSource.Cancel();
                    return result;
                }
            }

            return false;
        }

        private static bool IsSquareAttackedByPawns(int squareIndex, Side sideToMove, int offset, BitBoard[] pieces)
        {
            var oppositeColorIndex = ((int)sideToMove + 1) % 2;

            return (Attacks.PawnAttacks[oppositeColorIndex, squareIndex] & pieces[offset]) != default;
        }

        private static bool IsSquareAttackedByKnights(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (Attacks.KnightAttacks[squareIndex] & piecePosition[(int)Piece.N + offset]) != default;
        }

        private static bool IsSquareAttackedByKing(int squareIndex, int offset, BitBoard[] piecePosition)
        {
            return (Attacks.KingAttacks[squareIndex] & piecePosition[(int)Piece.K + offset]) != default;
        }

        private static bool IsSquareAttackedByBishops(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var bishopAttacks = Attacks.BishopAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (bishopAttacks & piecePosition[(int)Piece.B + offset]) != default;
        }

        private static bool IsSquareAttackedByRooks(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var rookAttacks = Attacks.RookAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (rookAttacks & piecePosition[(int)Piece.R + offset]) != default;
        }

        private static bool IsSquareAttackedByQueens(int squareIndex, int offset, BitBoard[] piecePosition, BitBoard[] occupancy)
        {
            var queenAttacks = Attacks.QueenAttacks(squareIndex, occupancy[(int)Side.Both]);
            return (queenAttacks & piecePosition[(int)Piece.Q + offset]) != default;
        }
    }
}
