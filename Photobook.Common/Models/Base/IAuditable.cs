using Photobook.Common.Identity;
using System;

namespace Photobook.Common.Models.Base
{
    public interface IAuditable
    {
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifiedBy { get; set; }
    }
}
