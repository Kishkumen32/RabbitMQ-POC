using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PublisherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public IMediator Mediator { get; set; }
    }
}
