using BaseAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseAuth.Application.Interfaces
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<Role> GetByNameAsync(string name);
        Task<Role> GetRoleWithPermissionsAsync(int roleId);
        Task<IEnumerable<Role>> GetAllWithPermissionsAsync();
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId);
        Task<bool> RoleExistsAsync(string name);
        Task<bool> RoleHasPermissionAsync(int roleId, string permission);
        Task<IEnumerable<string>> GetRolePermissionsAsync(int roleId);
        Task<RolePermission> GetRolePermissionAsync(int roleId, int permissionId);
        Task AddRolePermissionAsync(RolePermission rolePermission);
        void RemoveRolePermission(RolePermission rolePermission);
    }
} 