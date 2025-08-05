using BaseAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseAuth.Application.Interfaces
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<Role> GetByNameAsync(string name);
        Task<Role> GetRoleWithPermissionsAsync(Guid roleId);
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId);
        Task<bool> RoleExistsAsync(string name);
        Task<bool> RoleHasPermissionAsync(Guid roleId, string permission);
        Task<IEnumerable<string>> GetRolePermissionsAsync(Guid roleId);
        Task<RolePermission> GetRolePermissionAsync(Guid roleId, Guid permissionId);
        Task AddRolePermissionAsync(RolePermission rolePermission);
        void RemoveRolePermission(RolePermission rolePermission);
    }
} 