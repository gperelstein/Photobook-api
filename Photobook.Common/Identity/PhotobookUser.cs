using Microsoft.AspNetCore.Identity;
using System;

namespace Photobook.Common.Identity
{
    public class PhotobookUser : IdentityUser<Guid>
    {
        public bool IsActive { get; set; }
    }
}
