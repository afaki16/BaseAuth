using AutoMapper;
using BaseAuth.Application.Features.Roles.Commands;
using BaseAuth.Application.Interfaces;
using BaseAuth.Domain.Common;
using BaseAuth.Domain.Entities;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Roles.Handlers
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<Application.DTOs.RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Application.DTOs.RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
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

            // Clear existing permissions
            role.RolePermissions.Clear();

            // Add new permissions if provided
            if (request.PermissionIds?.Any() == true)
            {
                foreach (var permissionId in request.PermissionIds)
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permissionId
                    });
                }
            }

            _unitOfWork.Roles.Update(role);
            await _unitOfWork.SaveChangesAsync();

            var roleDto = _mapper.Map<Application.DTOs.RoleDto>(role);
            return Result.Success(roleDto);
        }
    }
} 