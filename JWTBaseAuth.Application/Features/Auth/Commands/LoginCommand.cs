using JWTBaseAuth.Application.DTOs.Auth;
using JWTBaseAuth.Domain.Common;
using MediatR;

namespace JWTBaseAuth.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<Result<LoginResponseDto>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
} 