using System;

namespace Photobook.Common.Services
{
    public interface ICurrentUserService
    {
        Guid? GetUserId();
    }
}
