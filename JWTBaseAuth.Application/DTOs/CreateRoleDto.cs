using System;
using System.Collections.Generic;

namespace JWTBaseAuth.Application.DTOs
{
    public class CreateRoleDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
} 