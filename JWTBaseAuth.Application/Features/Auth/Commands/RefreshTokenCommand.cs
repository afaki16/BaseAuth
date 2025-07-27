using JWTBaseAuth.Application.DTOs.Auth;
using JWTBaseAuth.Domain.Common;
using MediatR;

namespace JWTBaseAuth.Application.Features.Auth.Commands
{
    public class RefreshTokenCommand : IRequest<Result<LoginResponseDto>>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
} 