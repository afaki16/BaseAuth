using BaseAuth.API.Controllers;
using BaseAuth.Application.DTOs;
using BaseAuth.Application.Features.Permissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseAuth.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class PermissionsController : BaseController
    {
        private readonly IMediator _mediator;

        public PermissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all permissions
        /// </summary>
        /// <returns>List of permissions</returns>
        [HttpGet]
        [Authorize(Policy = "RequirePermissionsReadPermission")]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var query = new GetAllPermissionsQuery();
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }
    }
} 