using Core.Command;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PublisherApi.Controllers
{
    public class PublisherApiController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> WhoAmI([FromBody]WhoAmICommand command)
        {
            var result = await Mediator.Send(command);

            return Ok(result);
        }
    }
}
