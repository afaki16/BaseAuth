using BaseAuth.Application.DTOs.Auth;
using BaseAuth.Domain.Common;
using BaseAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaseAuth.Application.Services
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(string email, string password, string ipAddress, string userAgent, string deviceId = null, string deviceName = null);
        Task<Result<string>> GenerateAccessTokenAsync(User user);
        Task<Result<RefreshToken>> GenerateRefreshTokenAsync(User user, string ipAddress, string userAgent, string deviceId = null, string deviceName = null);
        Task<Result<LoginResponseDto>> RefreshTokenAsync(string accessToken, string refreshToken, string ipAddress, string userAgent);
        Task<Result> RevokeTokenAsync(string refreshToken, string ipAddress = null, string userAgent = null, string reason = null);
        Task<Result> RevokeAllUserTokensAsync(int userId, string ipAddress = null, string userAgent = null, string reason = null);
        Task<Result> RevokeTokensByDeviceAsync(int userId, string deviceId, string ipAddress = null, string userAgent = null, string reason = null);
        ClaimsPrincipal GetClaimsFromExpiredToken(string token);
        Task<bool> ValidateTokenAsync(string token);
    }
} 