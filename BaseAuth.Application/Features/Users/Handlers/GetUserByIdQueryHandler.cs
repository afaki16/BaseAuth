using AutoMapper;
using BaseAuth.Application.Features.Users.Queries;
using BaseAuth.Application.Interfaces;
using BaseAuth.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Users.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<Application.DTOs.UserListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Application.DTOs.UserListDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetUserWithRolesAsync(request.Id);
            
            if (user == null)
                return Result.Failure<Application.DTOs.UserListDto>("User not found");

            var userDto = _mapper.Map<Application.DTOs.UserListDto>(user);
            return Result.Success(userDto);
        }
    }
} 