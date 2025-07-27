using JWTBaseAuth.Application.DTOs;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;

namespace JWTBaseAuth.Application.Features.Users.Queries
{
    public class GetAllUsersQuery : IRequest<Result<IEnumerable<UserDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
    }
} 