using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Lynx.TeamsApi.HostedServices;

internal class WriterHostedService : BackgroundService
{
    private readonly ChannelReader<string> _reader;
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
    private readonly IBotFrameworkHttpAdapter _adapter;
    private readonly string _appId;

    public WriterHostedService(
        ChannelReader<string> reader,
        ConcurrentDictionary<string, ConversationReference> conversationReferences,
        IBotFrameworkHttpAdapter adapter,
        IConfiguration configuration,
        ILogger<WriterHostedService> logger)
    {
        _reader = reader;
        _conversationReferences = conversationReferences;
        _adapter = adapter;
        _appId = configuration["MicrosoftAppId"] ?? string.Empty;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var output in _reader.ReadAllAsync(stoppingToken))
            {
                foreach (var conversationReference in _conversationReferences.Values)
                {
                    await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference,
                        async (context, token) => await BotCallback(output, context, token), stoppingToken);
                }

                //_logger.Debug($"[Lynx]\t{output}");
                Console.WriteLine(output);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Exception in {className}", nameof(WriterHostedService));
        }
        finally
        {
            _logger.LogInformation($"Finishing {nameof(WriterHostedService)}");
        }
    }

    private static async Task BotCallback(string message, ITurnContext turnContext, CancellationToken cancellationToken)
    {
        await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken);
    }
}