using JWTBaseAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Interfaces
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<Role> GetByNameAsync(string name);
        Task<Role> GetRoleWithPermissionsAsync(Guid roleId);
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId);
        Task<bool> RoleExistsAsync(string name);
    }
} 