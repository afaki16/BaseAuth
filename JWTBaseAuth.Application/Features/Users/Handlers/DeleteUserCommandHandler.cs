using JWTBaseAuth.Application.Features.Users.Commands;
using JWTBaseAuth.Application.Interfaces;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Features.Users.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
            
            if (user == null)
                return Result.Failure("User not found");

            _unitOfWork.Users.SoftDelete(user);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
} 