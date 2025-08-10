using BaseAuth.Application.DTOs;
using BaseAuth.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace BaseAuth.Application.Features.Roles.Commands
{
    public class CreateRoleCommand : IRequest<Result<RoleDto>>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
} 