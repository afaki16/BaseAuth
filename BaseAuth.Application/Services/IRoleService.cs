using BaseAuth.Application.DTOs;
using BaseAuth.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseAuth.Application.Services
{
    public interface IRoleService
    {
        Task<Result<RoleDto>> GetRoleByIdAsync(int id);
        Task<Result<RoleDto>> GetRoleByNameAsync(string name);
        Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync();
        Task<Result<IEnumerable<RoleDto>>> GetRolesByUserIdAsync(int userId);
        Task<Result<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<Result<RoleDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task<Result> DeleteRoleAsync(int id);
        Task<Result<bool>> RoleExistsAsync(string name);
    }
} 