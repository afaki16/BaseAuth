using BaseAuth.Domain.Enums;
using System;
using System.Collections.Generic;

namespace BaseAuth.Application.DTOs
{
    public class CreateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
} 