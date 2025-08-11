using AutoMapper;
using BaseAuth.Application.DTOs;
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
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Creating new role: {request.Name}");

                // Check if role name already exists
                if (await _unitOfWork.Roles.RoleExistsAsync(request.Name))
                    return Result.Failure<RoleDto>("Role name already exists");

                // Create role and assign permissions in a single transaction
                var role = new Role
                {
                    Name = request.Name,
                    Description = request.Description,
                    IsSystemRole = false
                };

                await _unitOfWork.Roles.AddAsync(role);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Created role with ID: {role.Id}");

                // Assign permissions to role if provided
                if (request.PermissionIds?.Any() == true)
                {
                    _logger.LogInformation($"Adding {request.PermissionIds.Count()} permissions to new role. Permission IDs: {string.Join(", ", request.PermissionIds)}");

                    var rolePermissions = new List<RolePermission>();

                    // First, get all permissions to verify they exist
                    var permissions = new List<Permission>();
                    foreach (var permissionId in request.PermissionIds)
                    {
                        var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                        if (permission == null)
                        {
                            _logger.LogWarning($"Permission with ID {permissionId} not found, skipping...");
                            continue;
                        }
                        permissions.Add(permission);
                        _logger.LogInformation($"Found permission: {permission.Name} (ID: {permission.Id})");
                    }

                    // Create role permissions for all valid permissions
                    foreach (var permission in permissions)
                    {
                        var rolePermission = new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permission.Id,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        
                        rolePermissions.Add(rolePermission);
                        await _unitOfWork.Roles.AddRolePermissionAsync(rolePermission);
                        _logger.LogInformation($"Added permission {permission.Name} (ID: {permission.Id}) to new role {role.Name} (ID: {role.Id})");
                        _logger.LogInformation($"RolePermission created: RoleId={rolePermission.RoleId}, PermissionId={rolePermission.PermissionId}, CreatedDate={rolePermission.CreatedDate}");
                    }

                    if (rolePermissions.Any())
                    {
                        try
                        {
                            await _unitOfWork.SaveChangesAsync();
                            _logger.LogInformation($"Saved {rolePermissions.Count} role permissions to database");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error saving role permissions to database: {ex.Message}");
                            throw;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No permission IDs provided in request");
                }

                // Get role with permissions for mapping
                var roleWithPermissions = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(role.Id);
                _logger.LogInformation($"Retrieved role with {roleWithPermissions?.RolePermissions?.Count ?? 0} permissions");
                
                var roleDto = _mapper.Map<RoleDto>(roleWithPermissions);
                _logger.LogInformation($"Mapped role DTO with {roleDto?.Permissions?.Count ?? 0} permissions");
                
                _logger.LogInformation($"Successfully created role: {role.Name}");
                return Result.Success(roleDto);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating role: {request.Name}");
                return Result.Failure<RoleDto>($"An error occurred while creating the role: {ex.Message}");
            }
        }
    }
} 