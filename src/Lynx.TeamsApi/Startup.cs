// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Threading.Channels;
using Lynx;
using Lynx.Api;
using Lynx.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.BotBuilderSamples;

public class Startup
{
    private static readonly Channel<string> _uciChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = true });
    private static readonly Channel<string> _engineChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = false });

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var driver = new LinxDriver(_uciChannel, _engineChannel, new Engine(_engineChannel));
        services.AddSingleton(new EngineOutputWrapper(_engineChannel));
        services.AddSingleton(new UserInputWrapper(_uciChannel));

        services.AddHostedService(_ => new EngineHostedService(driver));
        services.AddHostedService<WriterHostedService>();

        services.AddHttpClient().AddControllers().AddNewtonsoftJson();

        // Create the Bot Framework Authentication to be used with the Bot Adapter.
        services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

        // Create the Bot Adapter with error handling enabled.
        services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

        // Create a global hashset for our ConversationReferences
        services.AddSingleton<ConcurrentDictionary<string, ConversationReference>>();

        // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
        services.AddTransient<IBot, ProactiveBot>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseDefaultFiles()
            .UseStaticFiles()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints => endpoints.MapControllers());

        // app.UseHttpsRedirection();
    }
}
