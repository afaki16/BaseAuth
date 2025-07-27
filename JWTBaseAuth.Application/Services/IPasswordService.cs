using JWTBaseAuth.Domain.Common;

namespace JWTBaseAuth.Application.Services
{
    public interface IPasswordService
    {
        Result<string> HashPassword(string password);
        Result<bool> VerifyPassword(string password, string hashedPassword);
        Result<string> GenerateRandomPassword(int length = 12);
        Result<bool> ValidatePasswordStrength(string password);
    }
} 