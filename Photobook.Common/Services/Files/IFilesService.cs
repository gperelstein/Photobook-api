using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Photobook.Common.Services.Files
{
    public interface IFilesService
    {
        public Task<string> SaveFile(IFormFile file, string folder, CancellationToken cancellationToken);
    }
}
