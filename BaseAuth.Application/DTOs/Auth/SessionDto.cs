using System;

namespace BaseAuth.Application.DTOs.Auth
{
    public class SessionDto
    {
        public string Token { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string Location { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public bool IsCurrentSession { get; set; }
    }
} 