// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.BotBuilderSamples;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureLogging((logging) =>
            {
                logging.AddDebug();
                logging.AddConsole();
            });
            webBuilder.UseStartup<Startup>();
        });
