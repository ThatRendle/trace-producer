using System.Diagnostics;

namespace TraceProducer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int okCount = 0;
        int errorCount = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var apiTrace = Producer.ProduceApiTrace("GET /account/{id}", ActivityKind.Server, 10))
            {
                await Producer.PauseAsync(10..100, stoppingToken);
                using (var backendTrace = Producer.ProduceBackendTrace("GetUser", ActivityKind.Internal, 10))
                {
                    await Producer.PauseAsync(10..200, stoppingToken);
                    using (var dbTrace = Producer.ProduceDatabaseTrace("sp_GetUser", ActivityKind.Client, 10))
                    {
                        if (apiTrace?.Status == ActivityStatusCode.Error ||
                            backendTrace?.Status == ActivityStatusCode.Error ||
                            dbTrace?.Status == ActivityStatusCode.Error)
                        {
                            errorCount++;
                        }
                        else
                        {
                            okCount++;
                        }
                        await Producer.PauseAsync(100..1000, stoppingToken);
                    }
                    await Producer.PauseAsync(10..100, stoppingToken);
                }
                await Producer.PauseAsync(10..100, stoppingToken);
            }
            
            _logger.LogInformation("Produced {OkCount} successful traces and {ErrorCount} errors", okCount, errorCount);
            
            var pause = TimeSpan.FromMilliseconds(Random.Shared.Next(1000, 2000));
            await Task.Delay(pause, stoppingToken);
        }
    }
}
