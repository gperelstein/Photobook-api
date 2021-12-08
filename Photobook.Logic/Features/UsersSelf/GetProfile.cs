using MediatR;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Services.CurrentUser;
using Photobook.Data;
using Photobook.Logic.Features.UsersSelf.Responses;
using System;
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
                try
                {
                    var userId = _currentUserService.GetUserId();

                    var profile = await _context.Profiles
                                                    .Include(x => x.User)
                                                    .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

                    if (profile == null)
                    {
                        return new Response<ProfileResponse>(
                                new Error(ErrorCodes.InvalidUser,
                                ErrorMessages.InvalidUser,
                                HttpStatusCode.BadRequest));
                    }

                    if (!profile.User.IsActive)
                    {
                        return new Response<ProfileResponse>(
                            new Error(ErrorCodes.UserNotActive,
                            ErrorMessages.UserNotActive,
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
                catch(Exception ex)
                {
                    return new Response<ProfileResponse>(
                            new Error(ErrorCodes.UnexpectedError,
                            ex.Message,
                            HttpStatusCode.InternalServerError));
                }
            }
        }
    }
}
