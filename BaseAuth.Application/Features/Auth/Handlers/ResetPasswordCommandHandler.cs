using BaseAuth.Application.Features.Auth.Commands;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Auth.Handlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;

        public ResetPasswordCommandHandler(IUserService userService, IPasswordService passwordService)
        {
            _userService = userService;
            _passwordService = passwordService;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement password reset logic
            return Result.Success();
        }
    }
} 