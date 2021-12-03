using MediatR;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Services;
using Photobook.Data;
using Photobook.Logic.Features.UsersSelf.Responses;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Logic.Features.UsersSelf
{
    public class GetProfile
    {
        [JsonSchema("GetProfileCommand")]
        public class Command : IRequest<Response<ProfileResponse>> { }

        public class Handler : IRequestHandler<Command, Response<ProfileResponse>>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly PhotobookDbContext _context;

            public Handler(ICurrentUserService currentUserService, PhotobookDbContext context)
            {
                _currentUserService = currentUserService;
                _context = context;
            }

            public async Task<Response<ProfileResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _currentUserService.GetUserId();

                var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

                if (profile == null)
                {
                    return new Response<ProfileResponse>(
                            new Error(ErrorCodes.InvalidToken,
                            ErrorMessages.InvalidToken,
                            HttpStatusCode.BadRequest));
                }

                var profileResponse = new ProfileResponse
                {
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Description = profile.Description,
                    ProfilePicture = profile.ProfilePicture
                };

                return new Response<ProfileResponse>(profileResponse);
            }
        }
    }
}
