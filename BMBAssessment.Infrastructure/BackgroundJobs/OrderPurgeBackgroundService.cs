using BMBAssessment.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BMBAssessment.Infrastructure.BackgroundJobs;

public sealed class OrderPurgeBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderPurgeBackgroundService> _logger;

    public OrderPurgeBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<OrderPurgeBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var cutoff = DateTime.UtcNow.AddDays(-2);

                var orders = await unitOfWork.Orders.GetDeletedBefore(cutoff, 100, stoppingToken);

                foreach (var order in orders)
                    unitOfWork.Orders.Delete(order);

                await unitOfWork.SaveChangesAsync(stoppingToken);

                _logger.LogInformation( "Deleted {Count} old orders." , orders.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Order purge failed.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
