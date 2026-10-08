// Entry point required by the runtime, but actual work happens via JSExport methods.
// The .NET WASM runtime calls Main() on startup, then JS calls exported methods.

using Lynx;
using Microsoft.Extensions.Configuration;
//using NLog;

Console.WriteLine("Hello, console here!");

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

//if (Configuration.GeneralSettings.EnableLogging)
//{
//    LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
//}

Console.SetOut(Lynx.Wasm.UciInterop.OutputWriter);

//TODO https://devblogs.microsoft.com/dotnet/use-net-7-from-any-javascript-app-in-net-7/