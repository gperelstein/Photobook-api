using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Photobook.Common.HandlersResponses;
using Photobook.Logic.Features.Users;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Api.Controllers
{
    [ApiController]
    [Route("users")]
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
            summary: "Create a new user",
            description: "Create a new user"
        )]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(void), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error), Description = "Bad request")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(Error), Description = "Error while processing the request")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser.Command request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response.Error);
            }

            return NoContent();
        }
        
        [HttpPost("register")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "Register a user",
            description: "Complete the registration of a user"
        )]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(void), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error), Description = "Bad request")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(Error), Description = "Error while processing the request")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUser.Command request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response.Error);
            }

            return NoContent();
        }

        [HttpPost("initiatePasswordReset")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "Initiate the reset of the user password",
            description: "Initiate the reset of the user password"
        )]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(void), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error), Description = "Bad request")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(Error), Description = "Error while processing the request")]
        public async Task<IActionResult> InitiatePasswordReset([FromBody] InitiatePasswordReset.Command request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response.Error);
            }

            return NoContent();
        }

        [HttpPost("resetPassword")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "Reset the user password",
            description: "Reset the user password"
        )]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(void), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error), Description = "Bad request")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(Error), Description = "Error while processing the request")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword.Command request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response.Error);
            }

            return NoContent();
        }
    }
}
