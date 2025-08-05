using BaseAuth.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseAuth.Application.Services
{
    public interface IPermissionService
    {
        Task<Result<bool>> UserHasPermissionAsync(Guid userId, string permission);
        Task<Result<bool>> UserHasPermissionAsync(Guid userId, string resource, string action);
        Task<Result<IEnumerable<string>>> GetUserPermissionsAsync(Guid userId);
        Task<Result<bool>> RoleHasPermissionAsync(Guid roleId, string permission);
        Task<Result<IEnumerable<string>>> GetRolePermissionsAsync(Guid roleId);
        Task<Result> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
        Task<Result> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
        Task<Result> AssignRoleToUserAsync(Guid userId, Guid roleId);
        Task<Result> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    }
} 