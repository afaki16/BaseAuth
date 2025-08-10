using BaseAuth.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseAuth.Application.Services
{
    public interface IPermissionService
    {
        Task<Result<bool>> UserHasPermissionAsync(int userId, string permission);
        Task<Result<bool>> UserHasPermissionAsync(int userId, string resource, string action);
        Task<Result<IEnumerable<string>>> GetUserPermissionsAsync(int userId);
        Task<Result<bool>> RoleHasPermissionAsync(int roleId, string permission);
        Task<Result<IEnumerable<string>>> GetRolePermissionsAsync(int roleId);
        Task<Result> AssignPermissionToRoleAsync(int roleId, int permissionId);
        Task<Result> RemovePermissionFromRoleAsync(int roleId, int permissionId);
        Task<Result> AssignRoleToUserAsync(int userId, int roleId);
        Task<Result> RemoveRoleFromUserAsync(int userId, int roleId);
    }
} 