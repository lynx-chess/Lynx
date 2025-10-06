using Lynx.UCI.Commands.Engine;
using NUnit.Framework;
using System.Buffers;
using System.Diagnostics;
using System.Threading.Channels;

namespace Lynx.Test;

public class UCIHandlerTest
{
    [Test]
    public async Task HandleUCI()
    {
        var engineChannel = Channel.CreateUnbounded<object>();
        var guiChannel = Channel.CreateUnbounded<string>();

        using var searcher = new Searcher(guiChannel.Reader, engineChannel.Writer);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(5_000);

        await new UCIHandler(guiChannel, engineChannel, searcher).HandleUCI(cts.Token);

        var booleansDetected = 0;

        var correctValues = SearchValues.Create(["true", "false"], StringComparison.Ordinal);
        var incorrectValues = SearchValues.Create([bool.TrueString, bool.FalseString], StringComparison.Ordinal);

        await foreach (var command in engineChannel.Reader.ReadAllAsync(cts.Token))
        {
            var str = command.ToString();
            var span = str.AsSpan();

            if (span == UciOKCommand.Id)
            {
                break;
            }

            Assert.False(span.ContainsAny(incorrectValues), "No 'True' or 'False' or expected, only lowercase in: " + str);

            if (span.ContainsAny(correctValues))
            {
                booleansDetected++;
            }
        }

        Debug.Assert(booleansDetected >= 4);
    }
}
