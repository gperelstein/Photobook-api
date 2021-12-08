using System;

namespace Photobook.Common.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        Guid? GetUserId();
    }
}
