namespace JWTBaseAuth.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
} 