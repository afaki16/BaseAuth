using System;

namespace BaseAuth.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }

        protected BaseEntity()
        {
            CreatedDate = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
} 