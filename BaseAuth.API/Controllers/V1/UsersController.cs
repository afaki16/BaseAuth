using BaseAuth.API.Controllers;
using BaseAuth.Application.DTOs;
using BaseAuth.Application.Features.Users.Commands;
using BaseAuth.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseAuth.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <returns>List of users</returns>
        [HttpGet]
        [Authorize(Policy = "RequireUsersReadPermission")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null)
        {
            var query = new GetAllUsersQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };

            var result = await _mediator.Send(query);
            return HandleResult(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id:int}")]
        [Authorize(Policy = "RequireUsersReadPermission")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="dto">User creation data</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [Authorize(Policy = "RequireUsersCreatePermission")]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var command = new CreateUserCommand
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                PhoneNumber = dto.PhoneNumber,
                Status = dto.Status,
                RoleIds = dto.RoleIds
            };

            var result = await _mediator.Send(command);
            
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetUserById), new { id = result.Data.Id }, new { success = true, data = result.Data });
            
            return HandleResult(result);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="dto">User update data</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "RequireUsersUpdatePermission")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var command = new UpdateUserCommand
            {
                Id = id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Status = dto.Status,
                ProfileImageUrl = dto.ProfileImageUrl,
                RoleIds = dto.RoleIds
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Delete a user (soft delete)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "RequireUsersDeletePermission")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
} 