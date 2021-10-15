using Lynx.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lynx.Test
{
    public class ZobristHashGeneration
    {
        [Fact]
        public void Repetition_InitialPosition()
        {
            var originalPosition = new Position(Constants.InitialPositionFEN);

            var position = new Position(originalPosition, originalPosition.AllPossibleMoves().Single(m => m.UCIString() == "g1f3"));
            position = new Position(position, position.AllPossibleMoves().Single(m => m.UCIString() == "g8f6"));
            position = new Position(position, position.AllPossibleMoves().Single(m => m.UCIString() == "f3g1"));
            position = new Position(position, position.AllPossibleMoves().Single(m => m.UCIString() == "f6g8"));

            Assert.Equal(originalPosition.UniqueIdentifier, position.UniqueIdentifier);
        }

        [Theory]
        [InlineData("4k3/1P6/8/8/8/8/8/4K3 w - - 0 1")] // Promotion White
        [InlineData("4k3/8/8/8/8/8/1p6/4K3 b - - 0 1")] // Promotion Black
        [InlineData("rk6/1P6/8/8/8/8/8/4K3 w - - 0 1")] // Promotion with capture White
        [InlineData("4k3/8/8/8/8/8/1p6/RK6 b - - 0 1")] // Promotion with capture Black
        public void Promotion(string fen)
        {
            var originalPosition = new Position(fen);

            var fenDictionary = new Dictionary<string, (string Move, long Hash)>()
            {
                [originalPosition.FEN] = ("", originalPosition.UniqueIdentifier)
            };

            TransversePosition(originalPosition, fenDictionary);
        }

        /// <summary>
        /// TODO: mark as long running
        /// </summary>
        /// <param name="fen"></param>
        [Theory]
        [InlineData(Constants.TrickyTestPositionFEN)]
        [InlineData(Constants.KillerPositionFEN)]
        public void EnPassant(string fen)
        {
            var originalPosition = new Position(fen);

            var fenDictionary = new Dictionary<string, (string Move, long Hash)>()
            {
                [originalPosition.FEN] = ("", originalPosition.UniqueIdentifier)
            };

            TransversePosition(originalPosition, fenDictionary);
        }

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
                    if (pair.Hash != newPosition.UniqueIdentifier)
                    {
                        var expectedHash = ZobristTable.PositionHash(new Position(newPosition.FEN));

                        var errorMessage = $"From {originalPosition.FEN} using {move}: {newPosition.FEN}";
                        throw new(errorMessage);
                    }
                }
                else
                {
                    fenDictionary.Add(newPosition.FEN, (move.ToString(), newPosition.UniqueIdentifier));
                }

                if (depth < maxDepth)
                {
                    TransversePosition(newPosition, fenDictionary, maxDepth, ++depth);
                }

                Assert.Equal(fenDictionary.Count, fenDictionary.Values.Distinct().Count());
            }
        }
    }
}
