using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class CustomMiddleware
{
    private readonly ILogger<CustomMiddleware> _logger;
    private readonly RequestDelegate _next;

    public CustomMiddleware(RequestDelegate next,
        ILogger<CustomMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        _logger.LogInformation(" --- Custom Middleware Invoked --- ");

        await _next(httpContext);

        _logger.LogInformation(" --- Custom Middleware Completed --- ");
    }
}