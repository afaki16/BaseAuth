using AutoMapper;
using BaseAuth.Application.DTOs;
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
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            // Check if role name already exists
            if (await _unitOfWork.Roles.RoleExistsAsync(request.Name))
                return Result.Failure<RoleDto>("Role name already exists");

            // Create role
            var role = new Role
            {
                Name = request.Name,
                Description = request.Description,
                IsSystemRole = false
            };

            // Assign permissions to role if provided
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

            await _unitOfWork.Roles.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            var roleDto = _mapper.Map<RoleDto>(role);
            return Result.Success(roleDto);
        }
    }
} 