using AutoMapper;
using BaseAuth.Application.Features.Users.Queries;
using BaseAuth.Application.Interfaces;
using BaseAuth.Domain.Common;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Users.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<Application.DTOs.UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Application.DTOs.UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetUserWithPermissionsAsync(request.Id);
            
            if (user == null)
                return Result.Failure<Application.DTOs.UserDto>("User not found");

            var userDto = _mapper.Map<Application.DTOs.UserDto>(user);
            return Result.Success(userDto);
        }
    }
} 