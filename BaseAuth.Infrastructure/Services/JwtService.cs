using BaseAuth.Application.DTOs.Auth;
using BaseAuth.Application.Interfaces;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using BaseAuth.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BaseAuth.Infrastructure.Services
{
    public class JwtService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public JwtService(IConfiguration configuration, IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(string email, string password, string ipAddress, string userAgent)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(
                    (await _unitOfWork.Users.FindFirstAsync(u => u.Email == email))?.Id ?? Guid.Empty);

                if (user == null)
                    return Result.Failure<LoginResponseDto>("Invalid email or password");

                var passwordVerification = _passwordService.VerifyPassword(password, user.PasswordHash);
                if (!passwordVerification.IsSuccess || !passwordVerification.Data)
                    return Result.Failure<LoginResponseDto>("Invalid email or password");

                if (user.Status != Domain.Enums.UserStatus.Active)
                    return Result.Failure<LoginResponseDto>("Account is not active");

                var accessTokenResult = await GenerateAccessTokenAsync(user);
                if (!accessTokenResult.IsSuccess)
                    return Result.Failure<LoginResponseDto>(accessTokenResult.Error);

                var refreshTokenResult = await GenerateRefreshTokenAsync(user, ipAddress, userAgent);
                if (!refreshTokenResult.IsSuccess)
                    return Result.Failure<LoginResponseDto>(refreshTokenResult.Error);

                user.LastLoginDate = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                var response = new LoginResponseDto
                {
                    AccessToken = accessTokenResult.Data,
                    RefreshToken = refreshTokenResult.Data.Token,
                    ExpiresAt = refreshTokenResult.Data.ExpiryDate,
                    User = new Application.DTOs.UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Status = user.Status,
                        EmailConfirmed = user.EmailConfirmed,
                        PhoneConfirmed = user.PhoneConfirmed,
                        ProfileImageUrl = user.ProfileImageUrl,
                        CreatedDate = user.CreatedDate,
                        Roles = user.UserRoles.Select(ur => new Application.DTOs.RoleDto
                        {
                            Id = ur.Role.Id,
                            Name = ur.Role.Name,
                            Description = ur.Role.Description
                        }).ToList(),
                        Permissions = user.UserRoles
                            .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission))
                            .Distinct()
                            .Select(p => new Application.DTOs.PermissionDto
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Resource = p.Resource,
                                Type = p.Type,
                                FullPermission = p.FullPermission
                            }).ToList()
                    }
                };

                return Result.Success(response);
            }
            catch (Exception ex)
            {
                return Result.Failure<LoginResponseDto>($"Login failed: {ex.Message}");
            }
        }

        public async Task<Result<string>> GenerateAccessTokenAsync(User user)
        {
            try
            {
                var userWithPermissions = await _unitOfWork.Users.GetUserWithPermissionsAsync(user.Id);
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.LastName),
                    new Claim("status", user.Status.ToString())
                };

                // Add role claims
                var roles = userWithPermissions.UserRoles.Select(ur => ur.Role).ToList();
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }

                // Add permission claims
                var permissions = userWithPermissions.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission))
                    .Distinct()
                    .ToList();

                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("permission", permission.FullPermission));
                }

                var jwtSettings = _configuration.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryInMinutes"])),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return Result.Success(tokenString);
            }
            catch (Exception ex)
            {
                return Result.Failure<string>($"Error generating access token: {ex.Message}");
            }
        }

        public async Task<Result<RefreshToken>> GenerateRefreshTokenAsync(User user, string ipAddress, string userAgent)
        {
            try
            {
                // Revoke existing active tokens
                await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(user.Id);

                var randomBytes = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);

                var refreshToken = new RefreshToken
                {
                    UserId = user.Id,
                    Token = Convert.ToBase64String(randomBytes),
                    ExpiryDate = DateTime.UtcNow.AddDays(int.Parse(_configuration["JwtSettings:RefreshTokenExpiryInDays"])),
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success(refreshToken);
            }
            catch (Exception ex)
            {
                return Result.Failure<RefreshToken>($"Error generating refresh token: {ex.Message}");
            }
        }

        public async Task<Result<LoginResponseDto>> RefreshTokenAsync(string accessToken, string refreshToken, string ipAddress, string userAgent)
        {
            try
            {
                var principal = GetClaimsFromExpiredToken(accessToken);
                if (principal == null)
                    return Result.Failure<LoginResponseDto>("Invalid access token");

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Result.Failure<LoginResponseDto>("Invalid user ID in token");

                var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
                if (storedRefreshToken == null || storedRefreshToken.UserId != userId)
                    return Result.Failure<LoginResponseDto>("Invalid refresh token");

                if (!storedRefreshToken.IsActive)
                    return Result.Failure<LoginResponseDto>("Refresh token is not active");

                var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(userId);
                if (user == null)
                    return Result.Failure<LoginResponseDto>("User not found");

                // Revoke old refresh token
                storedRefreshToken.IsRevoked = true;
                _unitOfWork.RefreshTokens.Update(storedRefreshToken);

                // Generate new tokens
                var newAccessTokenResult = await GenerateAccessTokenAsync(user);
                if (!newAccessTokenResult.IsSuccess)
                    return Result.Failure<LoginResponseDto>(newAccessTokenResult.Error);

                var newRefreshTokenResult = await GenerateRefreshTokenAsync(user, ipAddress, userAgent);
                if (!newRefreshTokenResult.IsSuccess)
                    return Result.Failure<LoginResponseDto>(newRefreshTokenResult.Error);

                var response = new LoginResponseDto
                {
                    AccessToken = newAccessTokenResult.Data,
                    RefreshToken = newRefreshTokenResult.Data.Token,
                    ExpiresAt = newRefreshTokenResult.Data.ExpiryDate,
                    User = new Application.DTOs.UserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Status = user.Status,
                        EmailConfirmed = user.EmailConfirmed,
                        PhoneConfirmed = user.PhoneConfirmed,
                        ProfileImageUrl = user.ProfileImageUrl,
                        CreatedDate = user.CreatedDate
                    }
                };

                return Result.Success(response);
            }
            catch (Exception ex)
            {
                return Result.Failure<LoginResponseDto>($"Error refreshing token: {ex.Message}");
            }
        }

        public async Task<Result> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                await _unitOfWork.RefreshTokens.RevokeTokenAsync(refreshToken);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error revoking token: {ex.Message}");
            }
        }

        public async Task<Result> RevokeAllUserTokensAsync(Guid userId)
        {
            try
            {
                await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId);
                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error revoking user tokens: {ex.Message}");
            }
        }

        public ClaimsPrincipal GetClaimsFromExpiredToken(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, // Don't validate lifetime for expired tokens
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 