using JWTBaseAuth.Application.Features.Auth.Commands;
using JWTBaseAuth.Application.Services;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Features.Auth.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<Application.DTOs.Auth.LoginResponseDto>>
    {
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<Application.DTOs.Auth.LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken, request.IpAddress, request.UserAgent);
        }
    }
} 