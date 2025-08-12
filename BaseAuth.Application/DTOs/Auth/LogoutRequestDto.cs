namespace BaseAuth.Application.DTOs.Auth
{
    public class LogoutRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
} 