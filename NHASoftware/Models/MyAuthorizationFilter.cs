using Hangfire.Dashboard;

namespace NHASoftware.Models
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            /**************************************************************************************
             *  Lets any user who is authenticated see the dashboard. Needs updated once roles are made.
             ***************************************************************************************/

            var httpContext = context.GetHttpContext();
            return httpContext.User.IsInRole("admin");
        }
    }
}
