using BaseAuth.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace BaseAuth.Application.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetUserWithRolesAsync(int userId);
        Task<User> GetUserWithPermissionsAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phoneNumber);
        Task<UserRole> GetUserRoleAsync(int userId, int roleId);
        Task AddUserRoleAsync(UserRole userRole);
        void RemoveUserRole(UserRole userRole);
    }
} 