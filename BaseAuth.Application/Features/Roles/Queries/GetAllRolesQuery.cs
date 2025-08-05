using BaseAuth.Application.DTOs;
using BaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;

namespace BaseAuth.Application.Features.Roles.Queries
{
    public class GetAllRolesQuery : IRequest<Result<IEnumerable<RoleDto>>>
    {
    }
} 