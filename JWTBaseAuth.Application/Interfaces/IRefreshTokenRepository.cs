using JWTBaseAuth.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Interfaces
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