﻿using Lynx;
using Lynx.Cli;
using Lynx.UCI.Commands.Engine;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System.Threading.Channels;

Console.WriteLine($"{IdCommand.EngineName} {IdCommand.GetVersion()} by {IdCommand.EngineAuthor}");

#if DEBUG
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
#endif

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

config.GetSection(nameof(EngineSettings)).Bind(Configuration.EngineSettings);
config.GetSection(nameof(GeneralSettings)).Bind(Configuration.GeneralSettings);

// TODO remove when .NET sdk includes https://github.com/dotnet/runtime/issues/89732
var generalConfig = config.GetSection(nameof(GeneralSettings));
if (bool.TryParse(generalConfig[nameof(Configuration.GeneralSettings.EnableLogging)], out var enableLogging))
{
    Configuration.GeneralSettings.EnableLogging = enableLogging;
}

if (Configuration.GeneralSettings.EnableLogging)
{
    LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
}

var uciChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = true });
var engineChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = false });

using CancellationTokenSource source = new();
CancellationToken cancellationToken = source.Token;

var tasks = new List<Task>
{
    Task.Run(() => new Writer(engineChannel).Run(cancellationToken)),
    Task.Run(() => new LynxDriver(uciChannel, engineChannel, new Engine(engineChannel)).Run(cancellationToken)),
    Task.Run(() => new Listener(uciChannel).Run(cancellationToken)),
    uciChannel.Reader.Completion,
    engineChannel.Reader.Completion
};

try
{
    await Task.WhenAny(tasks);
}
catch (AggregateException ae)
{
    foreach (var e in ae.InnerExceptions)
    {
        if (e is TaskCanceledException taskCanceledException)
        {
            Console.WriteLine("Cancellation requested: {0}", taskCanceledException.Message);
        }
        else
        {
            Console.WriteLine("Exception: {0}", e.GetType().Name);
        }
    }
}
catch (Exception e)
{
    Console.WriteLine("Unexpected exception");
    Console.WriteLine(e.Message);
}
finally
{
    engineChannel.Writer.TryComplete();
    uciChannel.Writer.TryComplete();
    //source.Cancel();
    NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
}

Thread.Sleep(2_000);