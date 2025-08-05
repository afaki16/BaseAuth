using BaseAuth.Domain.Enums;
using System;
using System.Collections.Generic;

namespace BaseAuth.Application.DTOs
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserStatus Status { get; set; }
        public string ProfileImageUrl { get; set; }
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
} 