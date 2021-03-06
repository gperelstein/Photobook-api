using System;

namespace Photobook.Common.Models.Base
{
    public abstract class BaseEntity : IAuditable
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid? LastModifiedBy { get; set; }
    }
}
