using System.Text.Json;

namespace BooksLibrary.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message}", ex.Message);

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var response = new
                {
                    error = ex.Message
                };

                var json = JsonSerializer.Serialize(response);
                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}
