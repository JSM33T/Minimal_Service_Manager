using System.Collections.Concurrent;
public class ServiceStatus
{
    public string ServiceId { get; set; }
    public bool IsRunning { get; set; }
}

public class ServiceManager
{
    private readonly ConcurrentDictionary<string, BGService> _services = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ServiceManager> _logger;

    public ServiceManager(IServiceProvider serviceProvider, ILogger<ServiceManager> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<bool> StartService(string serviceId)
    {
        if (_services.ContainsKey(serviceId))
        {
            return false;
        }

        var scope = _serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BGService>>();
        var service = new BGService(serviceId, logger);

        if (_services.TryAdd(serviceId, service))
        {
            await service.StartAsync(CancellationToken.None);
            return true;
        }

        return false;
    }

    public async Task<bool> StopService(string serviceId)
    {
        if (_services.TryRemove(serviceId, out var service))
        {
            await service.StopAsync(CancellationToken.None);
            return true;
        }
        return false;
    }

    public List<string> GetRunningServices()
    {
        return _services.Where(s => s.Value.IsRunning)
                       .Select(s => s.Key)
                       .ToList();
    }

    public ServiceStatus GetServiceStatus(string serviceId)
    {
        if (_services.TryGetValue(serviceId, out var service))
        {
            return new ServiceStatus
            {
                ServiceId = serviceId,
                IsRunning = service.IsRunning
            };
        }
        return null;
    }
}
