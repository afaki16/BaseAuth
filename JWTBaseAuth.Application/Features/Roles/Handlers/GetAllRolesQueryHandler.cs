using AutoMapper;
using JWTBaseAuth.Application.Features.Roles.Queries;
using JWTBaseAuth.Application.Interfaces;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Features.Roles.Handlers
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<IEnumerable<Application.DTOs.RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllRolesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<Application.DTOs.RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _unitOfWork.Roles.GetAllAsync(r => r.RolePermissions);
            var roleDtos = _mapper.Map<IEnumerable<Application.DTOs.RoleDto>>(roles);
            return Result.Success(roleDtos);
        }
    }
} 