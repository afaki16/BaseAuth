using JWTBaseAuth.Domain.Common;
using MediatR;

namespace JWTBaseAuth.Application.Features.Auth.Commands
{
    public class LogoutCommand : IRequest<Result>
    {
        public string RefreshToken { get; set; }
    }
} 