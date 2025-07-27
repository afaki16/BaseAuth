using JWTBaseAuth.Application.DTOs;
using JWTBaseAuth.Domain.Common;
using JWTBaseAuth.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;

namespace JWTBaseAuth.Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<Result<UserDto>>
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