using BaseAuth.Application.DTOs.Auth;
using BaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;

namespace BaseAuth.Application.Features.Auth.Queries
{
    public class GetUserSessionsQuery : IRequest<Result<IEnumerable<SessionDto>>>
    {
        public int UserId { get; set; }
    }
} 