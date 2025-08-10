using BaseAuth.Domain.Common;
using MediatR;
using System;

namespace BaseAuth.Application.Features.Users.Commands
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
} 