using Microsoft.AspNetCore.Mvc;

namespace TaskManagerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly ServiceManager _serviceManager;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(ServiceManager serviceManager, ILogger<ServiceController> logger)
        {
            _serviceManager = serviceManager;
            _logger = logger;
        }

        [HttpPost("start/{serviceId}")]
        public async Task<IActionResult> StartService(string serviceId)
        {
            _logger.LogInformation($"Request to start service: {serviceId}");

            if (await _serviceManager.StartService(serviceId))
            {
                return Ok($"Service {serviceId} started successfully");
            }

            return BadRequest($"Failed to start service {serviceId}");
        }

        [HttpPost("stop/{serviceId}")]
        public async Task<IActionResult> StopService(string serviceId)
        {
            _logger.LogInformation($"Request to stop service: {serviceId}");

            if (await _serviceManager.StopService(serviceId))
            {
                return Ok($"Service {serviceId} stopped successfully");
            }

            return NotFound($"Service {serviceId} not found");
        }

        [HttpGet("status/{serviceId}")]
        public IActionResult GetServiceStatus(string serviceId)
        {
            var status = _serviceManager.GetServiceStatus(serviceId);
            if (status != null)
            {
                return Ok(status);
            }

            return NotFound($"Service {serviceId} not found");
        }

        [HttpGet("list")]
        public IActionResult GetRunningServices()
        {
            var services = _serviceManager.GetRunningServices();
            return Ok(services);
        }
    }
}
