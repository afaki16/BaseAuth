using BaseAuth.Application.Features.Auth.Commands;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Auth.Handlers
{
    public class RevokeSessionCommandHandler : IRequestHandler<RevokeSessionCommand, Result>
    {
        private readonly IAuthService _authService;

        public RevokeSessionCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(RevokeSessionCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RevokeTokenAsync(request.RefreshToken, request.IpAddress, request.UserAgent, request.Reason);
        }
    }
} 