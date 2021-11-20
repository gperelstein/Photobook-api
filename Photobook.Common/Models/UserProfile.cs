using Photobook.Common.Identity;
using System;

namespace Photobook.Common.Models
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public PhotobookUser User { get; set; }
    }
}
