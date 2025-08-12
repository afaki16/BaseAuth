using BaseAuth.Domain.Common;
using MediatR;

namespace BaseAuth.Application.Features.Auth.Commands
{
    public class ChangePasswordCommand : IRequest<Result>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
} 