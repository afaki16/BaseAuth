using BaseAuth.API.Controllers;
using BaseAuth.Application.DTOs;
using BaseAuth.Application.Features.Roles.Commands;
using BaseAuth.Application.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseAuth.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class RolesController : BaseController
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns>List of roles</returns>
        [HttpGet]
        [Authorize(Policy = "roles.read")]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllRoles()
        {
            var query = new GetAllRolesQuery();
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Role details</returns>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "roles.read")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var query = new GetRoleByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="dto">Role creation data</param>
        /// <returns>Created role</returns>
        [HttpPost]
        [Authorize(Policy = "roles.create")]
        [ProducesResponseType(typeof(RoleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            var command = new CreateRoleCommand
            {
                Name = dto.Name,
                Description = dto.Description,
                PermissionIds = dto.PermissionIds
            };

            var result = await _mediator.Send(command);
            
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetRoleById), new { id = result.Data.Id }, new { success = true, data = result.Data });
            
            return HandleResult(result);
        }

        /// <summary>
        /// Update an existing role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <param name="dto">Role update data</param>
        /// <returns>Updated role</returns>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "roles.update")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDto dto)
        {
            var command = new UpdateRoleCommand
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                PermissionIds = dto.PermissionIds
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "roles.delete")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var command = new DeleteRoleCommand { Id = id };
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
} 