using Lynx.Model;
using Moq;
using NUnit.Framework;
using System.Threading.Channels;

namespace Lynx.Test;

public abstract class BaseTest
{
    protected const int DefaultSearchDepth = 10;

    protected BaseTest()
    {
        Configuration.EngineSettings.TranspositionTableSize = 32;
    }

    protected static async Task<SearchResult> TestBestMove(string fen, string[]? allowedUCIMoveString, string[]? excludedUCIMoveString, int depth = DefaultSearchDepth)
    {
        var seachResult = await SearchBestMove(fen, depth);
        var bestMoveFound = seachResult.BestMove;

        if (allowedUCIMoveString is not null)
        {
            Assert.Contains(bestMoveFound.UCIString(), allowedUCIMoveString);
        }

        if (excludedUCIMoveString is not null)
        {
            Assert.False(excludedUCIMoveString.Contains(bestMoveFound.UCIString()));
        }

        return seachResult;
    }

    protected static async Task<SearchResult> SearchBestMove(string fen, int depth = DefaultSearchDepth)
    {
        var engine = GetEngine(fen);
        return await engine.BestMove(new($"go depth {depth}"));
    }

    protected static Engine GetEngine(string fen)
    {
        var engine = GetEngine();

        return SetEnginePosition(engine, fen);
    }

    protected static Engine GetEngine()
    {
        var mock = new Mock<ChannelWriter<string>>();

        mock
            .Setup(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        return new Engine(mock.Object);
    }

    protected static Engine SetEnginePosition(Engine engine, string fen)
    {
        engine.SetGame(new Game(fen));

        return engine;
    }
}
