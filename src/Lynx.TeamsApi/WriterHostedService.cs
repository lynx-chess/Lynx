// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Lynx.Api;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using NLog;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Microsoft.BotBuilderSamples;

internal class WriterHostedService : BackgroundService
{
    private readonly ChannelReader<string> _engineOutputReader;
    private readonly NLog.ILogger _logger;
    private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
    private readonly IBotFrameworkHttpAdapter _adapter;
    private readonly string _appId;

    public WriterHostedService(
        EngineOutputWrapper engineOutputWrapper,
        ConcurrentDictionary<string, ConversationReference> conversationReferences,
        IBotFrameworkHttpAdapter adapter,
        IConfiguration configuration)
    {
        _engineOutputReader = engineOutputWrapper.Channel;
        _logger = LogManager.GetCurrentClassLogger();
        _conversationReferences = conversationReferences;
        _adapter = adapter;
        _appId = configuration["MicrosoftAppId"] ?? string.Empty;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var output in _engineOutputReader.ReadAllAsync(stoppingToken))
            {
                foreach (var conversationReference in _conversationReferences.Values)
                {
                    await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference, async (context, token) => await BotCallback(output, context, token), default);
                }

                //_logger.Debug($"[Lynx]\t{output}");
                Console.WriteLine(output);
            }
        }
        catch (Exception e)
        {
            _logger.Fatal(e);
        }
        finally
        {
            _logger.Info($"Finishing {nameof(WriterHostedService)}");
        }
    }

    private static async Task BotCallback(string message, ITurnContext turnContext, CancellationToken cancellationToken)
    {
        await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken);
    }
}