using AutoMapper;
using BaseAuth.Application.Features.Users.Commands;
using BaseAuth.Application.Interfaces;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using BaseAuth.Domain.Entities;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Users.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Application.DTOs.UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordService passwordService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<Result<Application.DTOs.UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
                return Result.Failure<Application.DTOs.UserDto>("Email already exists");

            // Hash password
            var passwordResult = _passwordService.HashPassword(request.Password);
            if (!passwordResult.IsSuccess)
                return Result.Failure<Application.DTOs.UserDto>(passwordResult.Error);

            // Create user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordResult.Data,
                PhoneNumber = request.PhoneNumber,
                Status = request.Status,
                EmailConfirmed = false
            };

            // Assign roles if provided
            if (request.RoleIds?.Any() == true)
            {
                foreach (var roleId in request.RoleIds)
                {
                    user.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    });
                }
            }

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _mapper.Map<Application.DTOs.UserDto>(user);
            return Result.Success(userDto);
        }
    }
} 