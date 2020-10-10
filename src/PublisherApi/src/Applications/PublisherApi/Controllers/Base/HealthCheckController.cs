using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<HealthCheckController> _logger;
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckController(ILogger<HealthCheckController> logger,
                                     HealthCheckService healthCheckService)
        {
            _logger = logger;
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            return report.Status == HealthStatus.Healthy ? Ok(report) :
                StatusCode((int)HttpStatusCode.ServiceUnavailable, report);
        }
    }
}