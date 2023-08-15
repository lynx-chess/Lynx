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

config.GetRequiredSection(nameof(EngineSettings)).Bind(Configuration.EngineSettings);
config.GetRequiredSection(nameof(GeneralSettings)).Bind(Configuration.GeneralSettings);

if (!Configuration.GeneralSettings.DisableLogging)
{
    LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
}

var uciChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = true });
var engineChannel = Channel.CreateBounded<string>(new BoundedChannelOptions(100) { SingleReader = true, SingleWriter = false });

using CancellationTokenSource source = new();
CancellationToken cancellationToken = source.Token;

var tasks = new List<Task>
{
    new Writer(engineChannel).Run(cancellationToken),
    new LynxDriver(uciChannel, engineChannel, new Engine(engineChannel)).Run(cancellationToken),
    new Listener(uciChannel).Run(cancellationToken),
    uciChannel.Reader.Completion,
    engineChannel.Reader.Completion
};

try
{
    if (args.Length > 0 && args[0] == "bench")
    {
        await uciChannel.Writer.WriteAsync("bench");
    }
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