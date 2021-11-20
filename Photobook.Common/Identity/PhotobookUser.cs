using Microsoft.AspNetCore.Identity;
using System;

namespace Photobook.Common.Identity
{
    public class PhotobookUser : IdentityUser<Guid>
    {
        public bool IsActive { get; set; }
        public string Picture { get; set; }
        public string Description { get; set; }
    }
}
