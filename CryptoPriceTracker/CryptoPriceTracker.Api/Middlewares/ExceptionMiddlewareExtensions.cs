using CryptoPriceTracker.Api.Middlewares;
using CryptoPriceTracker.Domain.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace Places.Api.Middleware;

/// <summary>
/// Extend the handler to capture the Exceptions
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    /// <summary>
    /// Handler the Exception and create a valid HttpResponse
    /// </summary>
    /// <param name="context">Current Http Context</param>
    /// <param name="exception">Exception Catched</param>
    public static Task HandleExceptionAsync(this HttpContext context, Exception exception, ILogger logger)
    {
        var httpStatusCode = GetStatusResponse(exception);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;

        var errors = new List<string>() { exception.Message };
        var innerException = exception;
        do
        {
            innerException = innerException.InnerException;
            if (innerException != null)
            {
                errors.Add(innerException.Message);
            }
        }
        while (innerException != null);

        var errorDetails = new ErrorDetails()
        {
            ErrorType = ReasonPhrases.GetReasonPhrase(context.Response.StatusCode),
            Errors = errors
        };

        // Log the exception
        logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        return context.Response.WriteAsync(errorDetails.ToString());
    }

    /// <summary>
    /// Allow to enable the Exception Middleware as service
    /// </summary>
    /// <param name="builder">Builder object to configure the service</param>
    /// <returns>The object to use in the Startup</returns>
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }

    /// <summary>
    /// Get the satus Code Response byt the Exception Type
    /// </summary>
    /// <param name="exception">Exception to handler</param>
    /// <returns>The HttpStatus Code</returns>
    private static HttpStatusCode GetStatusResponse(Exception exception)
    {
        var nameOfException = exception.GetType().BaseType!.Name;

        if (nameOfException.Equals("BusinessException"))
        {
            nameOfException = exception.GetType().Name;
        }

        return nameOfException switch
        {
            // Bad Request
            nameof(BadRequestException) => HttpStatusCode.BadRequest,

            // Not Found
            nameof(NotFoundException) => HttpStatusCode.NotFound,

            // Internal Server Error
            nameof(InternalServerErrorException) => HttpStatusCode.BadRequest,

            // Default
            _ => HttpStatusCode.BadRequest
        };
    }
}