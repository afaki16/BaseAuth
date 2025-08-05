using BaseAuth.Domain.Common;
using System;

namespace BaseAuth.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        // Navigation properties
        public User User { get; set; }

        public RefreshToken()
        {
            IsRevoked = false;
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
} 