using BaseAuth.API.Controllers;
using BaseAuth.Application.DTOs;
using BaseAuth.Application.Features.Permissions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseAuth.API.Controllers
{
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
        [Authorize(Policy = "permissions.read")]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var query = new GetAllPermissionsQuery();
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }

        /// <summary>
        /// Get all permissions grouped by resource
        /// </summary>
        /// <returns>Permissions grouped by resource</returns>
        [HttpGet("grouped")]
        [Authorize(Policy = "permissions.read")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetPermissionsGroupedByResource()
        {
            var query = new GetAllPermissionsQuery();
            var result = await _mediator.Send(query);
            
            if (!result.IsSuccess)
                return HandleResult(result);

            var groupedPermissions = result.Data
                .GroupBy(p => p.Resource)
                .Select(g => new
                {
                    Resource = g.Key,
                    Permissions = g.Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Type,
                        p.FullPermission
                    }).OrderBy(p => p.Type)
                })
                .OrderBy(g => g.Resource);

            return Ok(new { success = true, data = groupedPermissions });
        }

        /// <summary>
        /// Get permissions by resource
        /// </summary>
        /// <param name="resource">Resource name (e.g., Users, Roles, Dashboard)</param>
        /// <returns>Permissions for the specified resource</returns>
        [HttpGet("resource/{resource}")]
        [Authorize(Policy = "permissions.read")]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPermissionsByResource(string resource)
        {
            var query = new GetAllPermissionsQuery();
            var result = await _mediator.Send(query);
            
            if (!result.IsSuccess)
                return HandleResult(result);

            var resourcePermissions = result.Data
                .Where(p => p.Resource.Equals(resource, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Type);

            if (!resourcePermissions.Any())
                return NotFound(new { success = false, message = $"No permissions found for resource: {resource}" });

            return Ok(new { success = true, data = resourcePermissions });
        }
    }
}
