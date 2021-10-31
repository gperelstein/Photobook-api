using Microsoft.AspNetCore.Identity;
using System;

namespace Photobook.Models.Identity
{
    public class PhotobookUser : IdentityUser<Guid>
    {
        public bool IsActive { get; set; }
    }
}
