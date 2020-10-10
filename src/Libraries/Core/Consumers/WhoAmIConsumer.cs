using Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Core.Consumers
{
    public class WhoAmIConsumer : IConsumer<IWhoAmICommand>
    {
        private readonly ILogger<WhoAmIConsumer> _logger;

        public WhoAmIConsumer(ILogger<WhoAmIConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IWhoAmICommand> context)
        {
            var whoami = context.Message;

            if(whoami == null 
              || string.IsNullOrEmpty(whoami.Username)
              || whoami.Username.Contains("string"))
            {
                await context.RespondAsync<IWhoAmIRejected>(new
                {
                    Reason = $"Username {whoami.Username} is not allowed",
                    InVar.Timestamp
                });

                return;
            }

            _logger.LogInformation($"Got the request for username: {whoami?.Username}");

            await context.RespondAsync<IWhoAmIAccepted>(new
            {
                Id = 1000,
                Firstname = "Luke",
                Lastname = "Skywalker",
                InVar.Timestamp
            });
        }
    }
}
