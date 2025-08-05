using BaseAuth.Application.DTOs;
using BaseAuth.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseAuth.Application.Services
{
    public interface IRoleService
    {
        Task<Result<RoleDto>> GetRoleByIdAsync(Guid id);
        Task<Result<RoleDto>> GetRoleByNameAsync(string name);
        Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync();
        Task<Result<IEnumerable<RoleDto>>> GetRolesByUserIdAsync(Guid userId);
        Task<Result<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<Result<RoleDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task<Result> DeleteRoleAsync(Guid id);
        Task<Result<bool>> RoleExistsAsync(string name);
    }
} 