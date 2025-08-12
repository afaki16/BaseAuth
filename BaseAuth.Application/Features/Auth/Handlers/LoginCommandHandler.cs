using AutoMapper;
using BaseAuth.Application.Features.Auth.Commands;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<Application.DTOs.Auth.LoginResponseDto>>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<Application.DTOs.Auth.LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(request.Email, request.Password, request.IpAddress, request.UserAgent, request.DeviceId, request.DeviceName);
        }
    }
} 