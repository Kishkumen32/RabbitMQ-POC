using Core.Command;
using Core.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Handlers
{
    public class WhoAmIHandler : IRequestHandler<WhoAmICommand, User>
    {
        private readonly ILogger<WhoAmIHandler> _logger;

        public WhoAmIHandler(ILogger<WhoAmIHandler> logger)
        {
            _logger = logger;
        }

        Task<User> IRequestHandler<WhoAmICommand, User>.Handle(WhoAmICommand request
                                                              ,CancellationToken cancellationToken)
        {
            var user = new User()
            {
                Id = 1000,
                Firstname = "Luke",
                Lastname = "Skywalker"
            };

            _logger.LogInformation($"Handled: {request.Username}");

            return Task.FromResult(user);
        }
    }
}
