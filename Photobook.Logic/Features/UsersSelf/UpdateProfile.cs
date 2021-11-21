using FluentValidation;
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
            public string Description { get; set; }
            public IFormFile File { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly string[] _validExtensions = new string[] { ".jpg", ".png" };
            public Validator()
            {
                RuleFor(x => x.File)
                    .Must(HasValidExtension)
                    .WithMessage("File type not supported");
            }

            protected bool HasValidExtension(IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                return _validExtensions.Contains(extension);
            }
        }

        public class Handler : IRequestHandler<Command, Response<ProfileResponse>>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IFilesService _profilePicturesService;
            private readonly PhotobookDbContext _context;

            public Handler(ICurrentUserService currentUserService,
                IFilesService profilePicturesService,
                PhotobookDbContext context)
            {
                _currentUserService = currentUserService;
                _profilePicturesService = profilePicturesService;
                _context = context;
            }

            public async Task<Response<ProfileResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                var userId = _currentUserService.GetUserId();

                var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.UserId == userId);

                if(profile == null)
                {
                    return new Response<ProfileResponse>(
                            new Error(ErrorCodes.InvalidToken,
                            ErrorMessages.InvalidToken,
                            HttpStatusCode.BadRequest));
                }

                var imagePath = await _profilePicturesService.SaveFile(request.File, userId.ToString());

                profile.Description = request.Description;
                profile.ProfilePicture = imagePath;

                await _context.SaveChangesAsync();

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
