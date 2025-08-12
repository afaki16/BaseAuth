using BaseAuth.Domain.Common;
using MediatR;

namespace BaseAuth.Application.Features.Auth.Commands
{
    public class ForgotPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
} 