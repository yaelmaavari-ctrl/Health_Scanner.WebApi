using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Service.Exceptions;

namespace Health_Scanner.WebApi
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);


            var (statusCode, title) = exception switch
            {
                ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),

                NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),

                KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),

                UnsupportedLanguageException => (StatusCodes.Status400BadRequest, "Unsupported Language"),

                ExternalServiceException => (StatusCodes.Status503ServiceUnavailable, "External API Error"),

                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };

            httpContext.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}