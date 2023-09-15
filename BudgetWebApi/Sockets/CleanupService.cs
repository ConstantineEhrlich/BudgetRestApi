namespace BudgetWebApi.Sockets;

public class CleanupService<T>: BackgroundService where T: IUpdateManager
{
    private readonly IUpdateManager _updateManager;

    public CleanupService(T updateManager)
    {
        _updateManager = updateManager;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _updateManager.RemoveDeadSockets();
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);  // Run every 5 minutes
        }
    }
}