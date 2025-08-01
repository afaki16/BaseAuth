using JWTBaseAuth.Domain.Enums;
using System;

namespace JWTBaseAuth.Application.DTOs
{
    public class UpdatePermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Resource { get; set; }
        public PermissionType Type { get; set; }
    }
} 