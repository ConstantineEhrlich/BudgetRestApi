using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BudgetServices.Excel;

public class DailyExportService : BackgroundService
{
    private readonly ILogger<DailyExportService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<ExportSettings> _settings;

    public DailyExportService(ILogger<DailyExportService> logger, IServiceProvider serviceProvider, IOptions<ExportSettings> settings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _settings = settings;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily export service started");
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                await DoWork(scope);
            }
            _logger.LogInformation("Daily export service finished");

            await DelayUntilMidnight(stoppingToken);
        }
    }

    private async Task DoWork(IServiceScope scope)
    {
        var myService = scope.ServiceProvider.GetRequiredService<DataExportService>();
        if (_settings.Value.Exports is null)
            return;
        foreach (var export in _settings.Value.Exports)
        {
            if (export.Budget is null || export.User is null)
            {
                _logger.LogError($"Invalid export configuration: {export.Budget ?? "null budget"} / {export.User ?? "null user"}");
                continue;
            }
            try
            {
                _logger.LogInformation("Exporting {export.Budget} for {export.User}", export.Budget, export.User);
                await myService.ExportTransactions(export.Budget, export.User);
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to export {export.Budget} for {export.User}", export.Budget, export.User);
            }
        }
    }
    
    private async Task DelayUntilMidnight(CancellationToken token)
    {
        _logger.LogInformation("Waiting until midnight");
        var now = DateTime.Now;
        var nextMidnight = now.Date.AddDays(1);
        var delay = nextMidnight - now;
        await Task.Delay(delay, token);
        _logger.LogInformation("Midnight arrived");
    }
}