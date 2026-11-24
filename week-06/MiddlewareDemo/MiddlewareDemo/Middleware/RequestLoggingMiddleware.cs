
using System.Diagnostics;

namespace MiddlewareDemo.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get the incoming request path
            string path = context.Request.Path;

            // Start measuring time
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Request Started: {Path}", path);

            try
            {
                // Pass request to next middleware
                await _next(context);
            }
            finally
            {
                // Stop measuring time
                stopwatch.Stop();

                // Log request path and duration
                _logger.LogInformation(
                    "Request Path: {Path}, Duration: {Duration} ms",
                    path,
                    stopwatch.ElapsedMilliseconds
                );
            }
        }
    }
}
