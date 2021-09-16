using Lynx;
using Lynx.Cli;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#if DEBUG
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
#endif

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

config.GetRequiredSection(nameof(GameParameters)).Bind(Configuration.Parameters);

LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

var opts = new BoundedChannelOptions(1_000)
{
    SingleReader = true,
    SingleWriter = true
};

var uciChannel = Channel.CreateBounded<string>(opts);
var engineChannel = Channel.CreateBounded<string>(opts);

using CancellationTokenSource source = new();
CancellationToken cancellationToken = source.Token;

var tasks = new List<Task>
{
    Task.Run(() => new Writer(engineChannel).Run(cancellationToken)),
    Task.Run(() => new LinxDriver(uciChannel, engineChannel, new Engine()).Run(cancellationToken)),
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
            Console.WriteLine("Cancellation requested: {taskCanceledException.Message}");
        }
        else
        {
            Console.WriteLine("Exception: " + e.GetType().Name);
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
}

Thread.Sleep(2_000);
