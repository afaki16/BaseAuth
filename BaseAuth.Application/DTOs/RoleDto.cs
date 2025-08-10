using System;
using System.Collections.Generic;

namespace BaseAuth.Application.DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystemRole { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
} 