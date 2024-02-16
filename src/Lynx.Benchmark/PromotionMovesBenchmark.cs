using BenchmarkDotNet.Attributes;
using Lynx.Model;
using static Lynx.PregeneratedMoves;

namespace Lynx.Benchmark;
public class PromotionMovesBenchmark : BaseBenchmark
{
    private Memory<Move> _movePool;

    [GlobalSetup]
    public void Setup()
    {
        _movePool = new Memory<Move>(new Move[Constants.MaxNumberOfPossibleMovesInAPosition]);
    }

    [Benchmark(Baseline = true)]
    public int Naive()
    {
        int localIndex = 0;
        return PregeneratedWhitePromotions_Naive(_movePool.Span, ref localIndex, (int)BoardSquare.e8);
    }

    [Benchmark]
    public int Increment_At_The_End()
    {
        int localIndex = 0;
        return PregeneratedWhitePromotions_Increment_At_The_End(_movePool.Span, ref localIndex, (int)BoardSquare.e8);
    }

    public int PregeneratedWhitePromotions_Naive(Span<Move> movePool, ref int localIndex, int singlePushSquare)
    {
        var whitePromotions = WhitePromotions[singlePushSquare];

        movePool[localIndex++] = whitePromotions[0];
        movePool[localIndex++] = whitePromotions[1];
        movePool[localIndex++] = whitePromotions[2];
        movePool[localIndex++] = whitePromotions[3];

        return localIndex;
    }

    public int PregeneratedWhitePromotions_Increment_At_The_End(Span<Move> movePool, ref int localIndex, int singlePushSquare)
    {
        var whitePromotions = WhitePromotions[singlePushSquare];

        movePool[localIndex] = whitePromotions[0];
        movePool[localIndex + 1] = whitePromotions[1];
        movePool[localIndex + 2] = whitePromotions[2];
        movePool[localIndex + 3] = whitePromotions[3];

        localIndex += 4;

        return localIndex;
    }
}
