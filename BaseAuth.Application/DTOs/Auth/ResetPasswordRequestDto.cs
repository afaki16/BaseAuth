namespace BaseAuth.Application.DTOs.Auth
{
    public class ResetPasswordRequestDto
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
} 