// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Lynx;

namespace Microsoft.BotBuilderSamples;

internal class EngineHostedService : BackgroundService
{
    private readonly LinxDriver _linxDriver;

    public EngineHostedService(LinxDriver linxDriver)
    {
        _linxDriver = linxDriver;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _linxDriver.Run(stoppingToken);
    }
}