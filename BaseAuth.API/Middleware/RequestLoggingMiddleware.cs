using Serilog;
using System.Diagnostics;

namespace BaseAuth.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            _logger.LogInformation("Request started: {Method} {Path} from {IpAddress}",
                request.Method, request.Path, context.Connection.RemoteIpAddress);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var response = context.Response;

                _logger.LogInformation("Request completed: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                    request.Method, request.Path, response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }
} 