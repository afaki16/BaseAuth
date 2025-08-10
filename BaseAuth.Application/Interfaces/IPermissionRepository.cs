using BaseAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseAuth.Application.Interfaces
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<Permission> GetByNameAsync(string name);
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId);
        Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(int userId);
        Task<bool> PermissionExistsAsync(string name);
    }
} 