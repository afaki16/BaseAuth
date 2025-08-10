using BaseAuth.Application.DTOs;
using BaseAuth.Domain.Common;
using MediatR;
using System;

namespace BaseAuth.Application.Features.Roles.Queries
{
    public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
    {
        public int Id { get; set; }
    }
} 