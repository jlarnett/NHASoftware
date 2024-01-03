using System.Security.AccessControl;

namespace NHA.Website.Software.Services.SessionHistory
{
    public static class SessionEvents
    {
        public const string Login = "LOGIN";
        public const string Logout = "LOGOUT";
        public const string Timeout = "TIMEOUT";
        public const string RenewActive = "RENEW";
    }
}
