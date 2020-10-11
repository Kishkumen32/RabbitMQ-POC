using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Worker
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        readonly IBusControl _bus;

        public Worker(ILogger<Worker> logger,
                      IBusControl bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bus.StopAsync(cancellationToken);
        }
    }
}
