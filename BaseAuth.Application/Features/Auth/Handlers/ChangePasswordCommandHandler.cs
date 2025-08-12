using BaseAuth.Application.Features.Auth.Commands;
using BaseAuth.Application.Services;
using BaseAuth.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Auth.Handlers
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;
        private readonly ICurrentUserService _currentUserService;

        public ChangePasswordCommandHandler(
            IUserService userService, 
            IPasswordService passwordService, 
            ICurrentUserService currentUserService)
        {
            _userService = userService;
            _passwordService = passwordService;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                return Result.Failure("User not authenticated");

            // TODO: Implement password change logic
            return Result.Success();
        }
    }
} 