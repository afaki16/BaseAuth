using AutoMapper;
using BaseAuth.Application.DTOs;
using BaseAuth.Application.Interfaces;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using BaseAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAuth.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<RoleDto>> GetRoleByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null)
                return Result.Failure<RoleDto>("Role not found");

            var roleDto = _mapper.Map<RoleDto>(role);
            return Result.Success(roleDto);
        }

        public async Task<Result<RoleDto>> GetRoleByNameAsync(string name)
        {
            var role = await _unitOfWork.Roles.GetByNameAsync(name);
            if (role == null)
                return Result.Failure<RoleDto>("Role not found");

            var roleDto = _mapper.Map<RoleDto>(role);
            return Result.Success(roleDto);
        }

        public async Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync()
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return Result.Success(roleDtos);
        }

        public async Task<Result<IEnumerable<RoleDto>>> GetRolesByUserIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserWithRolesAsync(userId);
            if (user == null)
                return Result.Failure<IEnumerable<RoleDto>>("User not found");

            var roles = user.UserRoles.Select(ur => ur.Role);
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return Result.Success(roleDtos);
        }

        public async Task<Result<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            if (await _unitOfWork.Roles.RoleExistsAsync(createRoleDto.Name))
                return Result.Failure<RoleDto>("Role name already exists");

            var role = new Role
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                IsSystemRole = false
            };

            await _unitOfWork.Roles.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            var roleDto = _mapper.Map<RoleDto>(role);
            return Result.Success(roleDto);
        }

        public async Task<Result<RoleDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(updateRoleDto.Id);
            if (role == null)
                return Result.Failure<RoleDto>("Role not found");

            if (role.IsSystemRole)
                return Result.Failure<RoleDto>("Cannot modify system roles");

            role.Name = updateRoleDto.Name;
            role.Description = updateRoleDto.Description;

            _unitOfWork.Roles.Update(role);
            await _unitOfWork.SaveChangesAsync();

            var roleDto = _mapper.Map<RoleDto>(role);
            return Result.Success(roleDto);
        }

        public async Task<Result> DeleteRoleAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null)
                return Result.Failure("Role not found");

            if (role.IsSystemRole)
                return Result.Failure("Cannot delete system roles");

            _unitOfWork.Roles.SoftDelete(role);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<bool>> RoleExistsAsync(string name)
        {
            var exists = await _unitOfWork.Roles.RoleExistsAsync(name);
            return Result.Success(exists);
        }
    }
} 