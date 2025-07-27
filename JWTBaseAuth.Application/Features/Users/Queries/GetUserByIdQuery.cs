using JWTBaseAuth.Application.DTOs;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System;

namespace JWTBaseAuth.Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public Guid Id { get; set; }
    }
} 