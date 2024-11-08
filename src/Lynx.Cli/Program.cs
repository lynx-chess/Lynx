using Lynx;
using Lynx.Cli;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .Build();

config.GetSection(nameof(EngineSettings)).Bind(Configuration.EngineSettings);
config.GetSection(nameof(GeneralSettings)).Bind(Configuration.GeneralSettings);

if (Configuration.GeneralSettings.EnableLogging)
{
    LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
}

await Runner.Run(args);

Thread.Sleep(2_000);
