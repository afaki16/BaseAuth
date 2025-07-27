using JWTBaseAuth.Domain.Common;
using MediatR;
using System;

namespace JWTBaseAuth.Application.Features.Users.Commands
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
} 