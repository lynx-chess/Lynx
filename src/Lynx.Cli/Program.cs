using Lynx;
using Lynx.Cli;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;

#if DEBUG
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
#endif

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: false)
    .AddJsonFile("appsettings.tournament.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

config.GetSection(nameof(EngineSettings)).Bind(Configuration.EngineSettings);
config.GetSection(nameof(GeneralSettings)).Bind(Configuration.GeneralSettings);

if (Configuration.GeneralSettings.EnableLogging)
{
    LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
}

await Runner.Run(args);

Thread.Sleep(2_000);
