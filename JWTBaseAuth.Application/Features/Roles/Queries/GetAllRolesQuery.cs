using JWTBaseAuth.Application.DTOs;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;

namespace JWTBaseAuth.Application.Features.Roles.Queries
{
    public class GetAllRolesQuery : IRequest<Result<IEnumerable<RoleDto>>>
    {
    }
} 