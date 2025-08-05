using BaseAuth.Application.DTOs;
using BaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;

namespace BaseAuth.Application.Features.Permissions.Queries
{
    public class GetAllPermissionsQuery : IRequest<Result<IEnumerable<PermissionDto>>>
    {
    }
} 