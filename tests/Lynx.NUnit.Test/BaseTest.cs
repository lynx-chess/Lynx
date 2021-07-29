using Lynx.Model;
using NUnit.Framework;
using System;
using System.Linq;

namespace Lynx.NUnit.Test
{
    public abstract class BaseTest
    {
        protected static void TestBestMove(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString)
        {
            // Arrange
            var engine = new Engine();
            engine.SetGame(new Game(fen));

            // Act
            var searchResult = engine.BestMove();
            var bestMoveFound = searchResult.BestMove;

            // Assert
            if (allowedUCIMoveString is not null)
            {
                Assert.Contains(bestMoveFound.UCIString(), allowedUCIMoveString);
            }

            if (excludedUCIMoveString is not null)
            {
                Assert.False(excludedUCIMoveString.Contains(bestMoveFound.UCIString()));
            }
        }
    }
}
