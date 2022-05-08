using Lynx;
using Lynx.TeamsApi;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using System.Collections.Concurrent;
using System.Threading.Channels;
using Lynx.TeamsApi.HostedServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient().AddControllers();
builder.Services.AddHealthChecks();

ConfigureEngineServices(builder, builder.Configuration);
ConfigureBotServices(builder);

var app = builder.Build();

app.UseHttpsRedirection();

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static WebApplicationBuilder ConfigureEngineServices(WebApplicationBuilder builder, IConfiguration configuration)
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code - Application code isn't trimmed, see https://github.com/dotnet/runtime/discussions/59230
    configuration.GetRequiredSection(nameof(EngineSettings)).Bind(Configuration.EngineSettings);
    configuration.GetRequiredSection(nameof(GeneralSettings)).Bind(Configuration.GeneralSettings);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    var uciChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = true });
    var engineChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = false });
    var driver = new LinxDriver(uciChannel, engineChannel, new Engine(engineChannel));

    builder.Services.AddSingleton(driver);
    builder.Services.AddSingleton(engineChannel.Reader);
    builder.Services.AddSingleton(uciChannel.Writer);

    builder.Services.AddHostedService<EngineHostedService>();
    builder.Services.AddHostedService<WriterHostedService>();

    return builder;
}

static WebApplicationBuilder ConfigureBotServices(WebApplicationBuilder builder)
{
    // Create a global hashset for our ConversationReferences
    builder.Services.AddSingleton<ConcurrentDictionary<string, ConversationReference>>();

    // Create the Bot Framework Authentication to be used with the Bot Adapter.
    builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

    // Create the Bot Adapter with error handling enabled.
    builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

    // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
    builder.Services.AddTransient<IBot, Bot>();

    return builder;
}