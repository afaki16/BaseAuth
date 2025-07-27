using JWTBaseAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Interfaces
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        Task<Permission> GetByNameAsync(string name);
        Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(Guid roleId);
        Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(Guid userId);
        Task<bool> PermissionExistsAsync(string name);
    }
} 