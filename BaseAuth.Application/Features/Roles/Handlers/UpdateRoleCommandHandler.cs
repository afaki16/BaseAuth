using AutoMapper;
using BaseAuth.Application.Features.Roles.Commands;
using BaseAuth.Application.Interfaces;
using BaseAuth.Domain.Common;
using BaseAuth.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Roles.Handlers
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<Application.DTOs.RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;

        public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UpdateRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<Application.DTOs.RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Updating role with ID: {request.Id}");

                var role = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(request.Id);

                if (role == null)
                    return Result.Failure<Application.DTOs.RoleDto>("Role not found");

                if (role.IsSystemRole)
                    return Result.Failure<Application.DTOs.RoleDto>("Cannot modify system roles");

                // Check if role name already exists for another role
                var existingRole = await _unitOfWork.Roles.GetByNameAsync(request.Name);
                if (existingRole != null && existingRole.Id != request.Id)
                    return Result.Failure<Application.DTOs.RoleDto>("Role name already exists");

                // Update role properties
                role.Name = request.Name;
                role.Description = request.Description;

                _unitOfWork.Roles.Update(role);

                // Clear existing permissions
                var existingRolePermissions = role.RolePermissions.ToList();
                foreach (var rolePermission in existingRolePermissions)
                {
                    _unitOfWork.Roles.RemoveRolePermission(rolePermission);
                }

                // Add new permissions if provided
                if (request.PermissionIds?.Any() == true)
                {
                    _logger.LogInformation($"Adding {request.PermissionIds.Count()} permissions to role");

                    foreach (var permissionId in request.PermissionIds)
                    {
                        // Verify permission exists
                        var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                        if (permission == null)
                        {
                            _logger.LogWarning($"Permission with ID {permissionId} not found, skipping...");
                            continue;
                        }

                        var rolePermission = new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permissionId
                        };
                        await _unitOfWork.Roles.AddRolePermissionAsync(rolePermission);
                        _logger.LogInformation($"Added permission {permission.Name} to role {role.Name}");
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Get updated role with permissions
                var updatedRole = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(role.Id);
                var roleDto = _mapper.Map<Application.DTOs.RoleDto>(updatedRole);
                
                _logger.LogInformation($"Successfully updated role: {role.Name}");
                return Result.Success(roleDto);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating role with ID: {request.Id}");
                return Result.Failure<Application.DTOs.RoleDto>($"An error occurred while updating the role: {ex.Message}");
            }
        }
    }
} 