using Microsoft.AspNetCore.Identity;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.SessionHistory;

namespace NHA.Website.Software.SessionTrackingMiddleware
{
    public class SessionTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionTrackingMiddleware> _logger;

        public SessionTrackingMiddleware(RequestDelegate next, ILogger<SessionTrackingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IActiveSessionTracker sessionTracker, UserManager<ApplicationUser> userManager)
        {
            var currentUser = await userManager.GetUserAsync(context.User);
            if (currentUser != null)
                await sessionTracker.CreateRenewEvent(currentUser.Email!);

            await _next.Invoke(context);
        }
    }

    public static class SessionTrackingMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionTrackerMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SessionTrackingMiddleware>();
        }
    }
}
