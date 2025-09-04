using AutoMapper;
using BaseAuth.Application.Features.Users.Queries;
using BaseAuth.Application.Interfaces;
using BaseAuth.Domain.Common;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Users.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<Application.DTOs.UserListDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<Application.DTOs.UserListDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetUsersWithRolesAsync(request.Page, request.PageSize, request.SearchTerm);
            var userDtos = _mapper.Map<IEnumerable<Application.DTOs.UserListDto>>(users);
            return Result.Success(userDtos);
        }
    }
} 