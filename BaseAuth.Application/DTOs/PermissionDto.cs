using BaseAuth.Domain.Enums;
using System;

namespace BaseAuth.Application.DTOs
{
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Resource { get; set; }
        public PermissionType Type { get; set; }
        public string FullPermission { get; set; }
        public DateTime CreatedDate { get; set; }
    }
} 