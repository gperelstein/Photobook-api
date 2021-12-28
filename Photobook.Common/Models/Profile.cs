using Photobook.Common.Identity;
using Photobook.Common.Models.Base;
using System;

namespace Photobook.Common.Models
{
    public class Profile : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public Guid? ProfileImageId { get; set; }
        public Image ProfileImage { get; set; }
        public Guid UserId { get; set; }
        public PhotobookUser User { get; set; }
    }
}
