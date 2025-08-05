using AutoMapper;
using BaseAuth.Application.DTOs;
using BaseAuth.Application.Interfaces;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using BaseAuth.Domain.Entities;
using BaseAuth.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAuth.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IPasswordService passwordService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetUserWithRolesAsync(id);
            if (user == null)
                return Result.Failure<UserDto>("User not found");

            var userDto = _mapper.Map<UserDto>(user);
            return Result.Success(userDto);
        }

        public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                return Result.Failure<UserDto>("User not found");

            var userDto = _mapper.Map<UserDto>(user);
            return Result.Success(userDto);
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var query = _unitOfWork.Users.Query();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.FirstName.Contains(searchTerm) || 
                                       u.LastName.Contains(searchTerm) || 
                                       u.Email.Contains(searchTerm));
            }

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return Result.Success(userDtos);
        }

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (await _unitOfWork.Users.EmailExistsAsync(createUserDto.Email))
                return Result.Failure<UserDto>("Email already exists");

            var passwordResult = _passwordService.HashPassword(createUserDto.Password);
            if (!passwordResult.IsSuccess)
                return Result.Failure<UserDto>(passwordResult.Error);

            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                PasswordHash = passwordResult.Data,
                PhoneNumber = createUserDto.PhoneNumber,
                Status = createUserDto.Status,
                EmailConfirmed = false
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return Result.Success(userDto);
        }

        public async Task<Result<UserDto>> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(updateUserDto.Id);
            if (user == null)
                return Result.Failure<UserDto>("User not found");

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.ProfileImageUrl = updateUserDto.ProfileImageUrl;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return Result.Success(userDto);
        }

        public async Task<Result> DeleteUserAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return Result.Failure("User not found");

            _unitOfWork.Users.SoftDelete(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> ChangeUserStatusAsync(Guid id, UserStatus status)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return Result.Failure("User not found");

            user.Status = status;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(Guid id, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return Result.Failure("User not found");

            var passwordResult = _passwordService.HashPassword(newPassword);
            if (!passwordResult.IsSuccess)
                return Result.Failure(passwordResult.Error);

            user.PasswordHash = passwordResult.Data;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<bool>> EmailExistsAsync(string email)
        {
            var exists = await _unitOfWork.Users.EmailExistsAsync(email);
            return Result.Success(exists);
        }

        public async Task<Result<bool>> PhoneExistsAsync(string phoneNumber)
        {
            var exists = await _unitOfWork.Users.PhoneExistsAsync(phoneNumber);
            return Result.Success(exists);
        }
    }
} 