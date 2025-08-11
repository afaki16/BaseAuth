using BaseAuth.Domain.Common;
using BaseAuth.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BaseAuth.Domain.Entities
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

        // Flag enum için yardımcı metodlar
        public bool HasPermission(PermissionType permissionType)
        {
            return (Type & permissionType) == permissionType;
        }

        public bool HasAnyPermission(PermissionType permissionTypes)
        {
            return (Type & permissionTypes) != PermissionType.None;
        }

        public IEnumerable<PermissionType> GetIndividualPermissions()
        {
            return System.Enum.GetValues<PermissionType>()
                .Where(pt => pt != PermissionType.None && HasPermission(pt));
        }

        public string FullPermission => $"{Resource}.{Type}";
    }
} 