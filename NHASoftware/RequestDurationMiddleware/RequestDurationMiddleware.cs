namespace NHA.Website.Software.RequestDurationMiddleware;

public class RequestDurationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestDurationMiddleware> _logger;

    public RequestDurationMiddleware(RequestDelegate next, ILogger<RequestDurationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var start = DateTime.UtcNow;
        await _next.Invoke(context);
        _logger.LogInformation($"Request {context.Request.Path}: {(DateTime.UtcNow - start).TotalMilliseconds}ms");
    }
}

public static class RequestDurationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestDurationMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestDurationMiddleware>();
    }
}
