using BCrypt.Net;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseAuth.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        public Result<string> HashPassword(string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                    return Result.Failure<string>("Password cannot be empty");

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
                return Result.Success(hashedPassword);
            }
            catch (Exception ex)
            {
                return Result.Failure<string>($"Error hashing password: {ex.Message}");
            }
        }

        public Result<bool> VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
                    return Result.Success(false);

                var isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                return Result.Success(isValid);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"Error verifying password: {ex.Message}");
            }
        }

        public Result<string> GenerateRandomPassword(int length = 12)
        {
            try
            {
                if (length < 8)
                    return Result.Failure<string>("Password length must be at least 8 characters");

                const string lowercase = "abcdefghijklmnopqrstuvwxyz";
                const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                const string digits = "0123456789";
                const string special = "!@#$%^&*";
                
                var random = new Random();
                var password = new StringBuilder();

                // Ensure at least one character from each category
                password.Append(lowercase[random.Next(lowercase.Length)]);
                password.Append(uppercase[random.Next(uppercase.Length)]);
                password.Append(digits[random.Next(digits.Length)]);
                password.Append(special[random.Next(special.Length)]);

                // Fill the rest randomly
                string allChars = lowercase + uppercase + digits + special;
                for (int i = 4; i < length; i++)
                {
                    password.Append(allChars[random.Next(allChars.Length)]);
                }

                // Shuffle the password
                var chars = password.ToString().ToCharArray();
                for (int i = chars.Length - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (chars[i], chars[j]) = (chars[j], chars[i]);
                }

                return Result.Success(new string(chars));
            }
            catch (Exception ex)
            {
                return Result.Failure<string>($"Error generating password: {ex.Message}");
            }
        }

        public Result<bool> ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return Result.Failure<bool>("Password cannot be empty");

            if (password.Length < 8)
                return Result.Failure<bool>("Password must be at least 8 characters long");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return Result.Failure<bool>("Password must contain at least one lowercase letter");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return Result.Failure<bool>("Password must contain at least one uppercase letter");

            if (!Regex.IsMatch(password, @"\d"))
                return Result.Failure<bool>("Password must contain at least one digit");

            return Result.Success(true);
        }
    }
} 