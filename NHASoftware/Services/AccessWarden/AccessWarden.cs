using System.Security.Claims;

namespace NHASoftware.Services.AccessWarden
{
    public class AccessWarden : IWarden
    {
        /// <summary>
        /// Checks if the current logged in user is admin or forum admin
        /// </summary>
        /// <returns>Returns Bool if logged in user IS admin or forum admin</returns>
        public bool IsForumAdmin(ClaimsPrincipal User)
        {
            if (User.IsInRole("admin") || User.IsInRole("forum admin"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the current logged in user is admin
        /// </summary>
        /// <returns>Returns Bool if logged in user IS admin or forum admin</returns>
        public bool IsAdmin(ClaimsPrincipal User)
        {
            if (User.IsInRole("admin"))
            {
                return true;
            }

            return false;
        }
    }
}
