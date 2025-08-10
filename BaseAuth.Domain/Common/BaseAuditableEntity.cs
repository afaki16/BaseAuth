using System;

namespace BaseAuth.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
} 