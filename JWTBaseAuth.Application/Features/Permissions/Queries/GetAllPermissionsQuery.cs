using JWTBaseAuth.Application.DTOs;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;

namespace JWTBaseAuth.Application.Features.Permissions.Queries
{
    public class GetAllPermissionsQuery : IRequest<Result<IEnumerable<PermissionDto>>>
    {
    }
} 