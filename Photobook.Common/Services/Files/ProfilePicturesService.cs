using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Common.Services.Files
{
    public class ProfilePicturesService : IFilesService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IWebHostEnvironment _hostEnviornment;

        public ProfilePicturesService(IFileSystem fileSystem, IWebHostEnvironment hostEnviornment)
        {
            _fileSystem = fileSystem;
            _hostEnviornment = hostEnviornment;
        }

        public async Task<string> SaveFile(IFormFile file, string folder, CancellationToken cancellationToken)
        {
            var pictureDirectory = Path.Combine(_hostEnviornment.ContentRootPath, "Images", folder);
            Directory.CreateDirectory(pictureDirectory);
            var picturePath = Path.Combine(_hostEnviornment.ContentRootPath, pictureDirectory, "profile.jpg");
            using (var fileStream = new FileStream(picturePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream, cancellationToken);
            }
            return Path.Combine("Images", folder, "profile.jpg");
        }
    }
}
