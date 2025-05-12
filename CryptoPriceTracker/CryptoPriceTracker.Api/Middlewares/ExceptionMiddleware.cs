using Places.Api.Middleware;

namespace CryptoPriceTracker.Api.Middlewares;

/// <summary>
/// Handler the exceptions
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext == null)
        {
            return;
        }

        try
        {
            await _next(httpContext);
        }
        catch (Exception ex) when (!httpContext.Response.HasStarted)
        {
            await httpContext.HandleExceptionAsync(ex, _logger);
        }
    }
}