using Core.Command;
using Core.Entities;
using Core.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PublisherApi.Controllers
{
    public class PublisherApiController : BaseApiController
    {
        private readonly ILogger<PublisherApiController> _logger;
        private readonly IRequestClient<IWhoAmICommand> _whoAmIRequestClient;

        public PublisherApiController(ILogger<PublisherApiController> logger
                                     ,IRequestClient<IWhoAmICommand> whoAmIRequestClient)
        {
            _logger = logger;
            _whoAmIRequestClient = whoAmIRequestClient;
        }

        [HttpPost]
        public async Task<IActionResult> WhoAmI([FromBody]WhoAmICommand command)
        {
            var (accepted, rejected) = await _whoAmIRequestClient.GetResponse<IWhoAmIAccepted, IWhoAmIRejected>(command);

            if(accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;
                return Ok(response.Message);
            }
            else
            {
                var response = await rejected;
                return BadRequest(response.Message);
            }
        }
    }
}
