using BaseAuth.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace BaseAuth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return NotFound();

                return Ok(new { success = true, data = result.Data });
            }

            if (result.Errors?.Count > 1)
            {
                return BadRequest(new { success = false, errors = result.Errors });
            }

            return BadRequest(new { success = false, error = result.Error });
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok(new { success = true, message = "Operation completed successfully" });

            if (result.Errors?.Count > 1)
            {
                return BadRequest(new { success = false, errors = result.Errors });
            }

            return BadRequest(new { success = false, error = result.Error });
        }

        protected string GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        protected string GetUserAgent()
        {
            return HttpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown";
        }
    }
} 