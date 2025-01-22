public class BGService(string serviceId, ILogger<BGService> logger) : BackgroundService
{
    private readonly string _serviceId = serviceId;
    private readonly ILogger<BGService> _logger = logger;

    public string ServiceId => _serviceId;
    public bool IsRunning { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            IsRunning = true;
            _logger.LogInformation($"Service {_serviceId} is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // Your long-running task logic here
                _logger.LogInformation($"Service {_serviceId} running at: {DateTime.UtcNow}");
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation($"Service {_serviceId} was cancelled.");
        }
        finally
        {
            IsRunning = false;
            _logger.LogInformation($"Service {_serviceId} has stopped.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Stopping service {_serviceId}");
        await base.StopAsync(cancellationToken);
    }
}
