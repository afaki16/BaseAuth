using JWTBaseAuth.Application.DTOs;
using JWTBaseAuth.Domain.Common;
using JWTBaseAuth.Domain.Entities;
using JWTBaseAuth.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Services
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetUserByIdAsync(Guid id);
        Task<Result<UserDto>> GetUserByEmailAsync(string email);
        Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(int page = 1, int pageSize = 10, string searchTerm = null);
        Task<Result<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Result<UserDto>> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<Result> DeleteUserAsync(Guid id);
        Task<Result> ChangeUserStatusAsync(Guid id, UserStatus status);
        Task<Result> ResetPasswordAsync(Guid id, string newPassword);
        Task<Result<bool>> EmailExistsAsync(string email);
        Task<Result<bool>> PhoneExistsAsync(string phoneNumber);
    }
} 