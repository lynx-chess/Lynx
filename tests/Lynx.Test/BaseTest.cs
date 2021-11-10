using Lynx.Model;
using Moq;
using NUnit.Framework;
using System.Threading.Channels;

namespace Lynx.Test
{
    public abstract class BaseTest
    {
        protected static void TestBestMove(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString)
        {
            var mock = new Mock<ChannelWriter<string>>();

            mock
                .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            // Arrange
            var engine = new Engine(mock.Object);
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
