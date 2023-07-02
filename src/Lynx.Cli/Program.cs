using Lynx;
using Lynx.Cli;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System.Threading.Channels;

#if DEBUG
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
#endif

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code - Application code isn't trimmed, see https://github.com/dotnet/runtime/discussions/59230
config.GetRequiredSection(nameof(EngineSettings)).Bind(Configuration.EngineSettings);
config.GetRequiredSection(nameof(GeneralSettings)).Bind(Configuration.GeneralSettings);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

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
    Task.Run(() => new Writer(engineChannel).Run(cancellationToken)),
    Task.Run(() => new LinxDriver(uciChannel, engineChannel, new Engine(engineChannel)).Run(cancellationToken)),
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
            Console.WriteLine($"Cancellation requested: {taskCanceledException.Message}");
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