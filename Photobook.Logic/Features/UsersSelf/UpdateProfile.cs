﻿using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using Photobook.Common.HandlersResponses;
using Photobook.Common.Services;
using Photobook.Common.Services.Files;
using Photobook.Data;
using Photobook.Logic.Features.UsersSelf.Responses;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Logic.Features.UsersSelf
{
    public class UpdateProfile
    {
        [JsonSchema("UpdateProfileCommand")]
        public class Command : IRequest<Response<ProfileResponse>>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Description { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.FirstName)
                    .Cascade(CascadeMode.Stop)
                    .NotNull()
                    .NotEmpty();

                RuleFor(x => x.LastName)
                    .Cascade(CascadeMode.Stop)
                    .NotNull()
                    .NotEmpty();
            }
        }

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

                if(profile == null)
                {
                    return new Response<ProfileResponse>(
                            new Error(ErrorCodes.InvalidToken,
                            ErrorMessages.InvalidToken,
                            HttpStatusCode.BadRequest));
                }
                profile.FirstName = request.FirstName;
                profile.LastName = request.LastName;
                profile.Description = request.Description;

                await _context.SaveChangesAsync(cancellationToken);

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
