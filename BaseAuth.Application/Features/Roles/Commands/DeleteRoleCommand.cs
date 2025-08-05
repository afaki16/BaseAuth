using BaseAuth.Domain.Common;
using MediatR;
using System;

namespace BaseAuth.Application.Features.Roles.Commands
{
    public class DeleteRoleCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
} 