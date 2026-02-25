/*
 * Compares queen promotion detection approaches in ScoreMove:
 *  - Old: (promotedPiece + 2) % 6 == 0
 *  - New: isPromotion && (promotedPiece == 4 || promotedPiece == 10)
 *
 * The key difference is that the new approach short-circuits on non-promotion moves
 * (the vast majority) via the isPromotion guard, and avoids a modulo by non-power-of-2
 * for promotion moves.
 */

using BenchmarkDotNet.Attributes;
using Lynx.Model;

namespace Lynx.Benchmark;

public class QueenPromotionCheck_Benchmark : BaseBenchmark
{
    /// <summary>
    /// Simulated promotedPiece values extracted from moves.
    /// Realistic distribution: ~90% non-promotion (0), rest split among promotion pieces.
    /// </summary>
    private int[] _promotedPieces = null!;

    [Params(1_000, 10_000, 100_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Realistic distribution: most moves are not promotions
        // P=0 N=1 B=2 R=3 Q=4 K=5 p=6 n=7 b=8 r=9 q=10 k=11
        // promotedPiece == 0 means no promotion
        // Valid promotion pieces: N(1), B(2), R(3), Q(4), n(7), b(8), r(9), q(10)
        var rng = new Random(42);
        _promotedPieces = new int[Size];

        for (int i = 0; i < Size; ++i)
        {
            var roll = rng.Next(100);
            _promotedPieces[i] = roll switch
            {
                < 90 => 0,                     // 90% no promotion
                < 93 => (int)Piece.Q,           // 3% white queen promotion
                < 96 => (int)Piece.q,           // 3% black queen promotion
                < 97 => (int)Piece.N,           // 1% white knight promotion
                < 98 => (int)Piece.n,           // 1% black knight promotion
                < 99 => (int)Piece.R,           // 0.5% white rook promotion
                _ => (int)Piece.B,              // 0.5% white bishop promotion
            };
        }
    }

    [Benchmark(Baseline = true)]
    public int Modulo()
    {
        int count = 0;
        var data = _promotedPieces;

        for (int i = 0; i < data.Length; ++i)
        {
            var promotedPiece = data[i];
            var isPromotion = promotedPiece != default;

            if ((promotedPiece + 2) % 6 == 0)
            {
                ++count;
            }

            if (!isPromotion)
            {
                ++count;
            }
        }

        return count;
    }

    [Benchmark]
    public int DirectComparison()
    {
        int count = 0;
        var data = _promotedPieces;

        for (int i = 0; i < data.Length; ++i)
        {
            var promotedPiece = data[i];
            var isPromotion = promotedPiece != default;

            if (isPromotion && (promotedPiece == (int)Piece.Q || promotedPiece == (int)Piece.q))
            {
                ++count;
            }

            if (!isPromotion)
            {
                ++count;
            }
        }

        return count;
    }
}
