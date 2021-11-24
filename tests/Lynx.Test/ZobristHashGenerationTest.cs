using Lynx.Model;
using NUnit.Framework;

namespace Lynx.Test;

public class ZobristHashGenerationTest
{
    [Test]
    public void Repetition_InitialPosition()
    {
        var originalPosition = new Position(Constants.InitialPositionFEN);

        var position = new Position(originalPosition, originalPosition.AllPossibleMoves().Single(m => m.UCIString() == "g1f3"));
        position = new Position(position, position.AllPossibleMoves().Single(m => m.UCIString() == "g8f6"));
        position = new Position(position, position.AllPossibleMoves().Single(m => m.UCIString() == "f3g1"));
        position = new Position(position, position.AllPossibleMoves().Single(m => m.UCIString() == "f6g8"));

        Assert.AreEqual(originalPosition.UniqueIdentifier, position.UniqueIdentifier);
    }

#pragma warning disable S4144 // Methods should not have identical implementations

    [TestCase("4k3/1P6/8/8/8/8/8/4K3 w - - 0 1", Description = "White promotion")]
    [TestCase("4k3/8/8/8/8/8/1p6/4K3 b - - 0 1", Description = "Black promotion")]
    [TestCase("rk6/1P6/8/8/8/8/8/4K3 w - - 0 1", Description = "White promotion and capture")]
    [TestCase("4k3/8/8/8/8/8/1p6/RK6 b - - 0 1", Description = "Black promotion and capture")]
    public void Promotion(string fen)
    {
        var originalPosition = new Position(fen);

        var fenDictionary = new Dictionary<string, (string Move, long Hash)>()
        {
            [originalPosition.FEN] = ("", originalPosition.UniqueIdentifier)
        };

        TransversePosition(originalPosition, fenDictionary);
    }

    [TestCase(Constants.TrickyTestPositionFEN, Category = Categories.LongRunning, Explicit = true)]
    [TestCase(Constants.KillerTestPositionFEN, Category = Categories.LongRunning, Explicit = true)]
    public void EnPassant(string fen)
    {
        var originalPosition = new Position(fen);

        var fenDictionary = new Dictionary<string, (string Move, long Hash)>()
        {
            [originalPosition.FEN] = ("", originalPosition.UniqueIdentifier)
        };

        TransversePosition(originalPosition, fenDictionary);
    }

#pragma warning restore S4144 // Methods should not have identical implementations

    private static void TransversePosition(Position originalPosition, Dictionary<string, (string Move, long Hash)> fenDictionary, int maxDepth = 10, int depth = 0)
    {
        foreach (var move in originalPosition.AllPossibleMoves())
        {
            var newPosition = new Position(originalPosition, move);
            if (!newPosition.IsValid())
            {
                continue;
            }

            if (fenDictionary.TryGetValue(newPosition.FEN, out var pair))
            {
                Assert.AreEqual(pair.Hash, newPosition.UniqueIdentifier, $"From {originalPosition.FEN} using {move}: {newPosition.FEN}");
            }
            else
            {
                fenDictionary.Add(newPosition.FEN, (move.ToString(), newPosition.UniqueIdentifier));
            }

            if (depth < maxDepth)
            {
                TransversePosition(newPosition, fenDictionary, maxDepth, ++depth);
            }

            Assert.AreEqual(fenDictionary.Count, fenDictionary.Values.Distinct().Count());
        }
    }
}
