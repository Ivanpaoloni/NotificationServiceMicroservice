using System.Net;
using System.Text.Json;

namespace NotificationService.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        //Global error handler middleware

        // This middleware will catch all unhandled exceptions in the pipeline
        // and log them, returning a generic error response to the client.
        // It should be registered in the pipeline before any other middleware that handles exceptions.
        // This is a good place to log the exception and return a generic error response.
       
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = JsonSerializer.Serialize(new
            {
                error = exception.Message
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(result);
        }
    }
}
