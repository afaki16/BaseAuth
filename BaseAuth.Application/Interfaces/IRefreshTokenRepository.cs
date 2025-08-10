using BaseAuth.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace BaseAuth.Application.Interfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<RefreshToken> GetActiveTokenByUserIdAsync(int userId);
        Task RevokeAllUserTokensAsync(int userId);
        Task RevokeTokenAsync(string token);
        Task CleanupExpiredTokensAsync();
    }
} 