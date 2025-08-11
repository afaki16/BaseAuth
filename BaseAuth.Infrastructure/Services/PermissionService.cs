using BaseAuth.Application.Interfaces;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using BaseAuth.Domain.Entities;
using BaseAuth.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAuth.Infrastructure.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> UserHasPermissionAsync(int userId, string permission)
        {
            var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(userId);
            if (user == null)
                return Result.Failure<bool>("User not found");

            var hasPermission = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Any(rp => rp.Permission.Name == permission);

            return Result.Success(hasPermission);
        }

        public async Task<Result<bool>> UserHasPermissionAsync(int userId, string resource, string action)
        {
            var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(userId);
            if (user == null)
                return Result.Failure<bool>("User not found");

            var permissionName = $"{resource}.{action}";
            var hasPermission = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Any(rp => rp.Permission.Name == permissionName);

            return Result.Success(hasPermission);
        }

        // Yeni: Flag enum ile permission kontrolü
        public async Task<Result<bool>> UserHasPermissionTypeAsync(int userId, string resource, PermissionType permissionType)
        {
            var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(userId);
            if (user == null)
                return Result.Failure<bool>("User not found");

            var hasPermission = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Any(rp => rp.Permission.Resource == resource && 
                          rp.Permission.HasPermission(permissionType));

            return Result.Success(hasPermission);
        }

        // Yeni: Birden fazla permission type kontrolü
        public async Task<Result<bool>> UserHasAnyPermissionTypeAsync(int userId, string resource, PermissionType permissionTypes)
        {
            var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(userId);
            if (user == null)
                return Result.Failure<bool>("User not found");

            var hasPermission = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Any(rp => rp.Permission.Resource == resource && 
                          rp.Permission.HasAnyPermission(permissionTypes));

            return Result.Success(hasPermission);
        }

        public async Task<Result<IEnumerable<string>>> GetUserPermissionsAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(userId);
            if (user == null)
                return Result.Failure<IEnumerable<string>>("User not found");

            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToList();

            return Result.Success<IEnumerable<string>>(permissions);
        }

        // Yeni: Flag enum ile permission listesi
        public async Task<Result<IEnumerable<PermissionType>>> GetUserPermissionTypesAsync(int userId, string resource)
        {
            var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(userId);
            if (user == null)
                return Result.Failure<IEnumerable<PermissionType>>("User not found");

            var permissionTypes = user.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.Permission.Resource == resource)
                .SelectMany(rp => rp.Permission.GetIndividualPermissions())
                .Distinct()
                .ToList();

            return Result.Success<IEnumerable<PermissionType>>(permissionTypes);
        }

        public async Task<Result<bool>> RoleHasPermissionAsync(int roleId, string permission)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return Result.Failure<bool>("Role not found");

            var hasPermission = await _unitOfWork.Roles.RoleHasPermissionAsync(roleId, permission);
            return Result.Success(hasPermission);
        }

        public async Task<Result<IEnumerable<string>>> GetRolePermissionsAsync(int roleId)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return Result.Failure<IEnumerable<string>>("Role not found");

            var permissions = await _unitOfWork.Roles.GetRolePermissionsAsync(roleId);
            return Result.Success<IEnumerable<string>>(permissions);
        }

        public async Task<Result> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return Result.Failure("Role not found");

            var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
            if (permission == null)
                return Result.Failure("Permission not found");

            var existingRolePermission = await _unitOfWork.Roles.GetRolePermissionAsync(roleId, permissionId);
            if (existingRolePermission != null)
                return Result.Failure("Permission already assigned to role");

            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };

            await _unitOfWork.Roles.AddRolePermissionAsync(rolePermission);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var rolePermission = await _unitOfWork.Roles.GetRolePermissionAsync(roleId, permissionId);
            if (rolePermission == null)
                return Result.Failure("Permission not assigned to role");

            _unitOfWork.Roles.RemoveRolePermission(rolePermission);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> AssignRoleToUserAsync(int userId, int roleId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return Result.Failure("User not found");

            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return Result.Failure("Role not found");

            var existingUserRole = await _unitOfWork.Users.GetUserRoleAsync(userId, roleId);
            if (existingUserRole != null)
                return Result.Failure("User already has this role");

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };

            await _unitOfWork.Users.AddUserRoleAsync(userRole);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var userRole = await _unitOfWork.Users.GetUserRoleAsync(userId, roleId);
            if (userRole == null)
                return Result.Failure("User does not have this role");

            _unitOfWork.Users.RemoveUserRole(userRole);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
} 