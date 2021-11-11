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
            SearchResult searchResult = SearchBestResult(fen);
            var bestMoveFound = searchResult.BestMove;

            if (allowedUCIMoveString is not null)
            {
                Assert.Contains(bestMoveFound.UCIString(), allowedUCIMoveString);
            }

            if (excludedUCIMoveString is not null)
            {
                Assert.False(excludedUCIMoveString.Contains(bestMoveFound.UCIString()));
            }
        }

        protected static SearchResult SearchBestResult(string fen)
        {
            var mock = new Mock<ChannelWriter<string>>();

            mock
                .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            var engine = new Engine(mock.Object);
            engine.SetGame(new Game(fen));

            return engine.BestMove();
        }
    }
}
