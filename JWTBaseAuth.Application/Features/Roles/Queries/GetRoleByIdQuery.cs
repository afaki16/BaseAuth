using JWTBaseAuth.Application.DTOs;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System;

namespace JWTBaseAuth.Application.Features.Roles.Queries
{
    public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
    {
        public Guid Id { get; set; }
    }
} 