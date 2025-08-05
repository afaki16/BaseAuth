using BaseAuth.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace BaseAuth.Application.Interfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<RefreshToken> GetActiveTokenByUserIdAsync(Guid userId);
        Task RevokeAllUserTokensAsync(Guid userId);
        Task RevokeTokenAsync(string token);
        Task CleanupExpiredTokensAsync();
    }
} 