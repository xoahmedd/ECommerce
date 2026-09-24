using System.Net;
using System.Text.Json;
using ECommerce.APIs.Errors;

namespace ECommerce.APIs.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                // Take an action with the request

                await _next.Invoke(httpContext);

                // Take an action with the response
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); // Development Env
                // Log exception in (Database | Files) // Production Env

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var response = _env.IsDevelopment() ?
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                    : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                
                var json = JsonSerializer.Serialize(response, options);
                
                await httpContext.Response.WriteAsync(json);
            }
        }

    }
}

