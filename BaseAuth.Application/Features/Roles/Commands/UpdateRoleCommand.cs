using BaseAuth.Application.DTOs;
using BaseAuth.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace BaseAuth.Application.Features.Roles.Commands
{
    public class UpdateRoleCommand : IRequest<Result<RoleDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
} 