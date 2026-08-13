using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CampusERP.Application.Interfaces;

namespace CampusERP.BackgroundJobs.Attendance;

public class QrAttendanceExpiryBackgroundService: BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(15);

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<QrAttendanceExpiryBackgroundService> _logger;

    public QrAttendanceExpiryBackgroundService(IServiceScopeFactory scopeFactory, ILogger<QrAttendanceExpiryBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("QR attendance expiry background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredQrSessionsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                /*
                 * A failure for one cycle must not permanently
                 * stop the background worker.
                 */
                _logger.LogError(ex, "Error occurred while processing expired QR attendance sessions.");
            }

            try
            {
                await Task.Delay(CheckInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("QR attendance expiry background service stopped.");
    }

    private async Task ProcessExpiredQrSessionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var attendanceService =
            scope.ServiceProvider
                .GetRequiredService<IAttendanceService>();

        cancellationToken.ThrowIfCancellationRequested();

        await attendanceService .ExpireQrAttendanceSessionsAsync();
    }
}