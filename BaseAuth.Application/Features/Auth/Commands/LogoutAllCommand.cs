using BaseAuth.Domain.Common;
using MediatR;

namespace BaseAuth.Application.Features.Auth.Commands
{
    public class LogoutAllCommand : IRequest<Result>
    {
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Reason { get; set; }
    }
} 