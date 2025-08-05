using System;
using System.Collections.Generic;

namespace BaseAuth.Application.DTOs
{
    public class UpdateRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
} 