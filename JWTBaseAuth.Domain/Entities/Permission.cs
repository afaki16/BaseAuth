using JWTBaseAuth.Domain.Common;
using JWTBaseAuth.Domain.Enums;
using System.Collections.Generic;

namespace JWTBaseAuth.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Resource { get; set; }
        public PermissionType Type { get; set; }

        // Navigation properties
        public ICollection<RolePermission> RolePermissions { get; set; }

        public Permission()
        {
            RolePermissions = new HashSet<RolePermission>();
        }

        public string FullPermission => $"{Resource}.{Type}";
    }
} 