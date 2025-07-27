using AutoMapper;
using JWTBaseAuth.Application.Features.Roles.Queries;
using JWTBaseAuth.Application.Interfaces;
using JWTBaseAuth.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JWTBaseAuth.Application.Features.Roles.Handlers
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<Application.DTOs.RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRoleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Application.DTOs.RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(request.Id);
            
            if (role == null)
                return Result.Failure<Application.DTOs.RoleDto>("Role not found");

            var roleDto = _mapper.Map<Application.DTOs.RoleDto>(role);
            return Result.Success(roleDto);
        }
    }
} 