using BaseAuth.Domain.Enums;

namespace BaseAuth.Application.DTOs
{
    public class CreatePermissionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Resource { get; set; }
        public PermissionType Type { get; set; }
    }
} 