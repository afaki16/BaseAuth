using AutoMapper;
using BaseAuth.Application.DTOs;
using BaseAuth.Application.Features.Users.Commands;
using BaseAuth.Application.Interfaces;
using BaseAuth.Domain.Common;
using BaseAuth.Domain.Entities;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Users.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Application.DTOs.UserListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<UserListDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetUserWithRolesAsync(request.Id);

            if (user == null)
                return Result.Failure<UserListDto>("User not found");

            // Check if email already exists for another user
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (existingUser != null && existingUser.Id != request.Id)
                return Result.Failure<UserListDto>("Email already exists");

            // Update user properties
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Status = request.Status;
            user.ProfileImageUrl = request.ProfileImageUrl;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Clear existing roles
            var existingUserRoles = await _unitOfWork.Users.FindAsync(u => u.Id == request.Id);
            var userWithRoles = existingUserRoles.FirstOrDefault();
            if (userWithRoles != null)
            {
                foreach (var userRole in userWithRoles.UserRoles.ToList())
                {
                    _unitOfWork.Users.RemoveUserRole(userRole);
                }
            }

            // Add new roles if provided
            if (request.RoleIds?.Any() == true)
            {
                foreach (var roleId in request.RoleIds)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    };
                    await _unitOfWork.Users.AddUserRoleAsync(userRole);
                }
            }
            await _unitOfWork.SaveChangesAsync();

            // Reload user with roles to get complete data for mapping
            _ = await _unitOfWork.Users.GetUserWithRolesAsync(user.Id);
            var userDto = _mapper.Map<UserListDto>(userWithRoles);
            return Result.Success(userDto);
        }
    }
} 