using BaseAuth.Domain.Common;
using MediatR;

namespace BaseAuth.Application.Features.Auth.Commands
{
    public class LogoutCommand : IRequest<Result>
    {
        public string RefreshToken { get; set; }
    }
} 