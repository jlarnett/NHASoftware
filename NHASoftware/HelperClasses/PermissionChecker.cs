using System.Security.Claims;

namespace NHASoftware.HelperClasses
{
    public class PermissionChecker
    {
        public static PermissionChecker instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PermissionChecker();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Checks if the current logged in user is admin or forum admin
        /// </summary>
        /// <returns>Returns Bool if logged in user IS admin or forum admin</returns>
        public bool IsUserForumAdmin(ClaimsPrincipal User)
        {
            if (User.IsInRole("admin") || User.IsInRole("forum admin"))
            {
                return true;
            }

            return false;
        }
    }
}
