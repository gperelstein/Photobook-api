using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Photobook.Common.HandlersResponses;
using Photobook.Logic.Features.UsersSelf;
using Photobook.Logic.Features.UsersSelf.Responses;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("users/self")]
    public class UsersSelfController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersSelfController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("profile")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "Retrive the user profile",
            description: "Retrive the user profile"
        )]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ProfileResponse), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error), Description = "Error while processing the request")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(Error), Description = "Error while processing the request")]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetProfile.Command(), cancellationToken);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response.Error);
            }

            return Ok(response.Value);
        }

        [HttpPut("profile")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "Update the user profile",
            description: "Update the user profile"
        )]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ProfileResponse), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error), Description = "Error while processing the request")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(Error), Description = "Error while processing the request")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfile.Command request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response.Error);
            }

            return Ok(response.Value);
        }

        [HttpPut("profilePicture")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "Update the user profile picture",
            description: "Update the user profile picture"
        )]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ProfileResponse), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error), Description = "Error while processing the request")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(Error), Description = "Error while processing the request")]
        public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePicture.Command request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response.Error);
            }

            return Ok(response.Value);
        }

        [HttpPut("changePassword")]
        [Produces("application/json")]
        [OpenApiOperation(
            summary: "Change the user password",
            description: "Change the user password"
        )]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(ProfileResponse), Description = "Ok")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(Error), Description = "Error while processing the request")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(Error), Description = "Error while processing the request")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword.Command request, CancellationToken cancellationToken)
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
