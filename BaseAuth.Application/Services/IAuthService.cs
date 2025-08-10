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
        Task<Result<LoginResponseDto>> LoginAsync(string email, string password, string ipAddress, string userAgent);
        Task<Result<string>> GenerateAccessTokenAsync(User user);
        Task<Result<RefreshToken>> GenerateRefreshTokenAsync(User user, string ipAddress, string userAgent);
        Task<Result<LoginResponseDto>> RefreshTokenAsync(string accessToken, string refreshToken, string ipAddress, string userAgent);
        Task<Result> RevokeTokenAsync(string refreshToken);
        Task<Result> RevokeAllUserTokensAsync(int userId);
        ClaimsPrincipal GetClaimsFromExpiredToken(string token);
        Task<bool> ValidateTokenAsync(string token);
    }
} 