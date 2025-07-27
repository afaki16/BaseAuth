using JWTBaseAuth.Domain.Common;
using MediatR;
using System;

namespace JWTBaseAuth.Application.Features.Roles.Commands
{
    public class DeleteRoleCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
} 