using JWTBaseAuth.Application.DTOs;
using JWTBaseAuth.Domain.Common;
using JWTBaseAuth.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;

namespace JWTBaseAuth.Application.Features.Users.Commands
{
    public class CreateUserCommand : IRequest<Result<UserDto>>
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