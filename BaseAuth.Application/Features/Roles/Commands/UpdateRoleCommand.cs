using BaseAuth.Application.DTOs;
using BaseAuth.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace BaseAuth.Application.Features.Roles.Commands
{
    public class UpdateRoleCommand : IRequest<Result<RoleDto>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
} 