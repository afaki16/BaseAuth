using BaseAuth.API.Controllers;
using BaseAuth.Application.DTOs.Auth;
using BaseAuth.Application.Features.Auth.Commands;
using BaseAuth.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseAuth.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Access token and user information</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password,
                RememberMe = request.RememberMe,
                DeviceId = request.DeviceId,
                DeviceName = request.DeviceName,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent()
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">Registration information</param>
        /// <returns>Created user information</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Application.DTOs.UserDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var command = new RegisterCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                PhoneNumber = request.PhoneNumber,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent()
            };

            var result = await _mediator.Send(command);
            
            if (result.IsSuccess)
                return CreatedAtAction(nameof(Login), new { }, new { success = true, data = result.Data });
            
            return HandleResult(result);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="request">Token refresh information</param>
        /// <returns>New access token and refresh token</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var command = new RefreshTokenCommand
            {
                AccessToken = request.AccessToken,
                RefreshToken = request.RefreshToken,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent()
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Logout and revoke refresh token
        /// </summary>
        /// <param name="request">Logout information</param>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            var command = new LogoutCommand
            {
                RefreshToken = request.RefreshToken,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent(),
                Reason = request.Reason
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Logout from all devices
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout-all")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> LogoutAll()
        {
            var command = new LogoutAllCommand
            {
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent(),
                Reason = "User requested logout from all devices"
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Logout from specific device
        /// </summary>
        /// <param name="deviceId">Device ID to logout from</param>
        /// <returns>Success message</returns>
        [HttpPost("logout-device/{deviceId}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> LogoutDevice(string deviceId)
        {
            var command = new LogoutDeviceCommand
            {
                DeviceId = deviceId,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent(),
                Reason = "User requested logout from specific device"
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        /// <returns>Current user details</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(Application.DTOs.UserDto), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (!int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var query = new Application.Features.Users.Queries.GetUserByIdQuery { Id = userIdInt };
            var result = await _mediator.Send(query);
            
            return HandleResult(result);
        }

        /// <summary>
        /// Get user's active sessions
        /// </summary>
        /// <returns>List of active sessions</returns>
        [HttpGet("sessions")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<SessionDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUserSessions()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (!int.TryParse(userId, out var userIdInt))
                return Unauthorized();

            var query = new GetUserSessionsQuery { UserId = userIdInt };
            var result = await _mediator.Send(query);
            
            return HandleResult(result);
        }

        /// <summary>
        /// Revoke a specific session
        /// </summary>
        /// <param name="request">Session revocation request</param>
        /// <returns>Success message</returns>
        [HttpPost("revoke-session")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RevokeSession([FromBody] RevokeSessionRequestDto request)
        {
            var command = new RevokeSessionCommand
            {
                RefreshToken = request.RefreshToken,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent(),
                Reason = request.Reason ?? "Session revoked by user"
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Password change request</param>
        /// <returns>Success message</returns>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var command = new ChangePasswordCommand
            {
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent()
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="request">Forgot password request</param>
        /// <returns>Success message</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var command = new ForgotPasswordCommand
            {
                Email = request.Email,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent()
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="request">Reset password request</param>
        /// <returns>Success message</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var command = new ResetPasswordCommand
            {
                Token = request.Token,
                NewPassword = request.NewPassword,
                IpAddress = GetIpAddress(),
                UserAgent = GetUserAgent()
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
} 