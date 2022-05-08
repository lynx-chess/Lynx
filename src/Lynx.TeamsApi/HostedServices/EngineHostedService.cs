namespace Lynx.TeamsApi.HostedServices;

internal class EngineHostedService : BackgroundService
{
    private readonly LinxDriver _linxDriver;
    private readonly ILogger<EngineHostedService> _logger;

    public EngineHostedService(LinxDriver linxDriver, ILogger<EngineHostedService> logger)
    {
        _linxDriver = linxDriver;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _linxDriver.Run(stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Exception in {className}", nameof(EngineHostedService));
        }
        finally
        {
            _logger.LogInformation("Finishing {className}", nameof(EngineHostedService));
        }
    }
}