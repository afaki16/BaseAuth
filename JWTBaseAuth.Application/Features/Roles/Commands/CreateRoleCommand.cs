using JWTBaseAuth.Application.DTOs;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace JWTBaseAuth.Application.Features.Roles.Commands
{
    public class CreateRoleCommand : IRequest<Result<RoleDto>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
} 