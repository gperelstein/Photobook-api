using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Photobook.Logic.Features.Users;
using Photobook.Logic.Features.Users.Responses;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Api.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "List available languages",
            description: "This endpoint returns a list of available languages"
        )]
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserResponse), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(ValidationProblemDetails), Description = "Error while processing the request")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser.Command request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        
        [HttpPost("register")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "asgasdgeas",
            description: "adgasdgasdg"
        )]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(ValidationProblemDetails), Description = "Error while processing the request")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUser.Command request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok("Todo bien");
        }
    }
}
