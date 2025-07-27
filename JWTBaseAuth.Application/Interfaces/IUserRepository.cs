using JWTBaseAuth.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetUserWithRolesAsync(Guid userId);
        Task<User> GetUserWithPermissionsAsync(Guid userId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phoneNumber);
    }
} 