using BaseAuth.Domain.Common;
using BaseAuth.Domain.Enums;
using System;

namespace BaseAuth.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedDate { get; set; }
        public string? RevokedByIpAddress { get; set; }
        public string? RevokedByUserAgent { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceType { get; set; }
        public string? Location { get; set; }
        public TokenType Type { get; set; }
        public string? Reason { get; set; }

        // Navigation properties
        public User User { get; set; }

        public RefreshToken()
        {
            IsRevoked = false;
            Type = TokenType.RefreshToken;
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public bool IsActive => !IsRevoked && !IsExpired;
        
        public void Revoke(string? ipAddress = null, string? userAgent = null, string? reason = null)
        {
            IsRevoked = true;
            RevokedDate = DateTime.UtcNow;
            RevokedByIpAddress = ipAddress;
            RevokedByUserAgent = userAgent;
            Reason = reason;
        }

        public TimeSpan GetRemainingTime()
        {
            return ExpiryDate - DateTime.UtcNow;
        }

        public bool IsExpiringSoon(int minutesThreshold = 30)
        {
            return GetRemainingTime().TotalMinutes <= minutesThreshold;
        }
    }
} 