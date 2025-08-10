using BaseAuth.Application.DTOs;
using BaseAuth.Domain.Common;
using MediatR;
using System;

namespace BaseAuth.Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public int Id { get; set; }
    }
} 