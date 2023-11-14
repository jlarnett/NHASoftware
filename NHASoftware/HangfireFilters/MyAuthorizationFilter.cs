using Hangfire.Dashboard;

namespace NHA.Website.Software.HangfireFilters
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {

        /// <summary>
        /// Used by Hangfire to handle authorization. In our case user in role admin can access hangfire dashboard.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.IsInRole("admin");
        }
    }
}
