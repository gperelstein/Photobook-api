using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Api.Controllers
{
    [ApiController]
    [Route("users/self")]
    public class UsersSelfController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersSelfController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /*[HttpPost("create")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "List available languages",
            description: "This endpoint returns a list of available languages"
        )]
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserResponse), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(ValidationProblemDetails), Description = "Error while processing the request")]
        public async Task<IActionResult> UpdateUser([FromBody] CreateUser.Command request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }*/
    }
}
