using AutoMapper;
using JWTBaseAuth.Application.Features.Permissions.Queries;
using JWTBaseAuth.Application.Interfaces;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Features.Permissions.Handlers
{
    public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, Result<IEnumerable<Application.DTOs.PermissionDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPermissionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<Application.DTOs.PermissionDto>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _unitOfWork.Permissions.GetAllAsync();
            var permissionDtos = _mapper.Map<IEnumerable<Application.DTOs.PermissionDto>>(permissions);
            return Result.Success(permissionDtos);
        }
    }
} 