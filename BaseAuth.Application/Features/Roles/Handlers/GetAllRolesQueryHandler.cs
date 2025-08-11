using AutoMapper;
using BaseAuth.Application.Features.Roles.Queries;
using BaseAuth.Application.Interfaces;
using BaseAuth.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BaseAuth.Application.Features.Roles.Handlers
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<IEnumerable<Application.DTOs.RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllRolesQueryHandler> _logger;

        public GetAllRolesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllRolesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Application.DTOs.RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting all roles with permissions...");
                
                // RoleRepository'deki özel metodu kullan
                var roles = await _unitOfWork.Roles.GetAllWithPermissionsAsync();
                
                _logger.LogInformation($"Found {roles.Count()} roles");
                
                var roleDtos = _mapper.Map<IEnumerable<Application.DTOs.RoleDto>>(roles);
                
                _logger.LogInformation($"Mapped {roleDtos.Count()} role DTOs");
                
                return Result.Success(roleDtos);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all roles");
                throw;
            }
        }
    }
} 