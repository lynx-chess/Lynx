using Lynx;
using Lynx.Model;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using System.Threading.Channels;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .Build();

config.GetSection(nameof(EngineSettings)).Bind(Configuration.EngineSettings);
config.GetSection(nameof(GeneralSettings)).Bind(Configuration.GeneralSettings);

if (Configuration.GeneralSettings.EnableLogging)
{
    LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
}

if (args.Length >= 1 && args[0] == "bench")
{
    var engineChannel = Channel.CreateBounded<object>(new BoundedChannelOptions(2 * Configuration.EngineSettings.MaxDepth) { SingleReader = true, SingleWriter = false, FullMode = BoundedChannelFullMode.DropOldest });

    var tt = new TranspositionTable();
    var engine = new Engine(engineChannel, tt);

    engine.NewGame();

    tt.Populate();

    for (var i = 0; i < tt.TT.Length; ++i)
    {
        if (tt.TT[i].Key != (ushort)i)
        {
            throw new Exception($"Item {i} shouldn't be {tt.TT[i].Key}!");
        }
    }

    engine.NewGame();
    engine.Bench(Configuration.EngineSettings.BenchDepth);
}

Thread.Sleep(2_000);
