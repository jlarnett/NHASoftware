
using NHA.Website.Software.Entities.Identity;

namespace NHA.Website.Software.Services.SessionHistory
{
    public interface IActiveSessionTracker
    {
        Task<bool> CreateLoginEvent(string email);
        Task<bool> CreateLogoutEvent(string email);
        Task<bool> CreateRenewEvent(string email);
        Task<DateTime?> GetUserLastActiveTime(ApplicationUser user);
    }
}