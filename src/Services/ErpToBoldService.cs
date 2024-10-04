using Bold.Integration.Base.Observability;

namespace Bold.Integration.Base.Services;

public class ErpToBoldService(IServiceScopeFactory scopeFactory, ILogger<ErpToBoldService> logger) : BackgroundService
{
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(30);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var activity = Diagnostics.StartActivity("Syncing ERP to Bold");
            try
            {
                using var scope = scopeFactory.CreateScope();
                var itemsService = scope.ServiceProvider.GetRequiredService<SkusService>();
                await itemsService.SyncErpToBold(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(exception: ex, message: "No se ha podido obtener la información del servidor del ERP.");
            }
            finally
            {
                await Task.Delay(delay: _pollingInterval, cancellationToken: stoppingToken);
            }
        }
    }
}